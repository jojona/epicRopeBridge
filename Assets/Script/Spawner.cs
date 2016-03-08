using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

public class Spawner : MonoBehaviour {
	/**
	 * Public fields that can be edited from GUI.
	 */
	// Prefabs for point, plank, middle rope and rope
	public Point pointPrefab;
	public Plank plankPrefab;
	public Rope ropePrefab;
	public MiddleRope middleRopePrefab;

	public Ball ball;

	// Anchor points for the bridge
	public Vector3 anchorPoint1Up;
	public Vector3 anchorPoint1Down;
	public Vector3 anchorPoint2Up;
	public Vector3 anchorPoint2Down;
	public Vector3 anchorPoint3Up;
	public Vector3 anchorPoint3Down;
	public Vector3 anchorPoint4Up;
	public Vector3 anchorPoint4Down;

	public float segLengthUpRopes = 1;
	public float segLengthDownRopes = 0.8f;
	public float segLengthVerticalRopes = 1;

	public float ropePointMass;
	public int extraUp = 10;
	public int extraDown = 1;
	public int planksPerTriangle = 3;
	[Tooltip("Delbart med planksPerTriangle")]
	public int numberOfPlanks = 21; // Delbart med planksPerTriangle
	public int pointsPerPlank = 5;
	public int plankSpacing = 1;
	public int heightPoints = 15;
	public int plankWidth = 5;

	// The time between each frame
	public float timestep = 1.0f / 60.0f;

	// Rope properties (stiffness and dampening)
	public float ropeStiffness = 800f;
	public float middleRopeStiffness = 800f;
	public float ropeDampening = 1f;

	[Range(1, 2)]
	[Tooltip("Euler 1, Runge kutta 2")]
	public int integrationMethod = 1;

	public float triangularRestK = 1.2f;
	public float triangularBreakK = 1.4f;
	public float verticalRestK = 1f;
	public float verticalBreakK = 1.2f;
	public float longRopesRestK = 866f;
	public float longRopesBreakK = 900f;

	/**
	 * Private fields.
	 */

	// Lists of ropes
	private List<PointController> ropes;

	// Length of each segment (space between rope points)
	private float segmentLength;

	// Runge-Kutta RK4 object
	private Integrator integrator;

	public int lap = 0;

	/**
	 * Initializes the ropes and planks.
	 */
	void Start () {

		// Init lists
		ropes = new List<PointController> ();

		Time.fixedDeltaTime = timestep;

		// Spawn all points and planks
		epicSpawn();
		// Creates RK4 object for further calculations of movement
		integrator = new Integrator ();
	}

	/**
	 * The One Spawn to Rule Them All.
	 */
	void epicSpawn() {
		int baseAmount1 = numberOfPlanks * (pointsPerPlank + plankSpacing) - plankSpacing;
		int baseAmount2 = numberOfPlanks * (2 + plankSpacing) - plankSpacing;

		float restK = longRopesRestK;
		float breakK = longRopesBreakK;

		// The 4 main ropes
		Rope r1 = spawnRope(anchorPoint1Up, anchorPoint2Up, "Upper right", extraUp * 2 + baseAmount1, true, segLengthUpRopes, true, restK, breakK);
		Rope r2 = spawnRope(anchorPoint3Up, anchorPoint4Up, "Upper left", extraUp * 2 + baseAmount1, true, segLengthUpRopes, true, restK, breakK);
		Rope r3 = spawnRope(anchorPoint1Down, anchorPoint2Down, "Down right", extraDown * 2 + baseAmount2, true, segLengthDownRopes, false, 0, 0);
		Rope r4 = spawnRope(anchorPoint3Down, anchorPoint4Down, "Down left", extraDown * 2 + baseAmount2, true, segLengthDownRopes, false, 0, 0);

		// Create the class containing all planks
		MiddleRope r = (MiddleRope) Instantiate(middleRopePrefab, Vector3.zero, Quaternion.identity);
		int offset = extraDown;
		for (int i = 0; i < numberOfPlanks; i++) {
			Point p1 = r3.getPoint (offset);
			Point p2 = r3.getPoint (offset + 1);
			Point p3 = r4.getPoint (offset);
			Point p4 = r4.getPoint (offset + 1);
			Plank plank = (Plank) Instantiate (plankPrefab, Vector3.zero, Quaternion.identity);
			plank.init (p1, p2, p3, p4, plankWidth);
			plank.name += " " + (i + 1);
			r.addPlank (plank);
			offset += 2 + plankSpacing;
		}
		ropes.Add(r);

		// The vertical end ropes
		spawnVerticalRopes(r1, r3);
		spawnVerticalRopes(r2, r4);

		float pointDensity = heightPoints / (r1.getPoint(extraUp).position - r3.getPoint(extraDown).position).magnitude;
		spawnTriangularRopes (r1, r3, pointDensity);
		spawnTriangularRopes (r2, r4, pointDensity);
	}

	void spawnTriangularRopes(Rope upper, Rope lower, float pointDensity) {
		float breakK = triangularBreakK;
		float restK = triangularRestK;

		int upperPointsPerTriangle = (planksPerTriangle * (pointsPerPlank + plankSpacing)) - plankSpacing / 2;
		for (int i = 0; i < numberOfPlanks / planksPerTriangle; i++) {
			Point from = lower.getPoint (extraDown + i * (planksPerTriangle * (2 + plankSpacing)) - plankSpacing / 2 - 1);
			Point to = upper.getPoint (extraUp + upperPointsPerTriangle * i + upperPointsPerTriangle / 2);
			float lengthOfRope = (to.position - from.position).magnitude;
			int numberOfPoints = (int) (lengthOfRope * pointDensity);
			Vector3 diff = (to.position - from.position) / (numberOfPoints + 1);
			float segmentLength = segLengthVerticalRopes * 2f / 5f + 0.00f * Mathf.Abs(i - numberOfPlanks/planksPerTriangle/2);
			Rope rx = spawnRope(from.position + diff, to.position, "Triangular " + (i + 1) + " ", numberOfPoints, false, segmentLength, true, restK, breakK);
			rx.getPoint(0).AddNeigbour(from, segmentLength, restK, breakK);
			rx.getPoints ().Last ().AddNeigbour (to, segmentLength, restK, breakK);
			
			from = upper.getPoint (extraUp + upperPointsPerTriangle * i + upperPointsPerTriangle / 2);
			to = lower.getPoint (extraDown + (i + 1) * (planksPerTriangle * (2 + plankSpacing)) - plankSpacing / 2 - 1);
			diff = (to.position - from.position) / (numberOfPoints + 1);
			segmentLength = segLengthVerticalRopes * 2f / 5f + 0.00f * (numberOfPlanks/planksPerTriangle/2 - Mathf.Abs(i - numberOfPlanks/planksPerTriangle/2));
			Rope ry = spawnRope(from.position + diff, to.position, "Triangular " + (i + 1) + " ", numberOfPoints, false, segmentLength, true, restK, breakK);
			ry.getPoint(0).AddNeigbour(from, segmentLength, restK, breakK);
			ry.getPoints ().Last ().AddNeigbour (to, segmentLength, restK, breakK);
		}
	}

	void spawnVerticalRopes(Rope up, Rope down) {
		float breakK = verticalBreakK;
		float restK = verticalRestK;

		Vector3 dir1 = down.getPoint (extraDown - 1).position - up.getPoint (extraUp - 2).position;
		Rope rVerticalFirst = spawnRope(up.getPoint (extraUp - 2).position + dir1 / (heightPoints + 1), down.getPoint (extraDown - 1).position - dir1 / (heightPoints + 1), "Vertical rope", heightPoints + 1, false, segLengthVerticalRopes, true, restK, breakK);
		up.getPoint (extraUp - 2).AddNeigbour(rVerticalFirst.getPoint(0), up.segmentLength, restK, breakK);
		down.getPoint (extraDown - 1).AddNeigbour(rVerticalFirst.getPoint(heightPoints), down.segmentLength, restK, breakK);

		Vector3 dir2 = down.getPoint (down.length() - extraDown + 1).position - up.getPoint (up.length() - extraUp + 1).position;
		Rope rVerticalSecond = spawnRope(up.getPoint (up.length() - extraUp + 1).position + dir2 / (heightPoints + 1), down.getPoint (down.length() - extraDown + 1).position - dir1 / (heightPoints + 1), "Vertical rope", heightPoints + 1, false, segLengthVerticalRopes, true, restK, breakK);
		up.getPoint (up.length() - extraUp + 1).AddNeigbour(rVerticalSecond.getPoint(0), up.segmentLength, restK, breakK);
		down.getPoint (down.length() - extraDown).AddNeigbour(rVerticalSecond.getPoint(heightPoints), down.segmentLength, restK, breakK);
	}

	Rope spawnRope(Vector3 from, Vector3 to, string name, int amount, bool fixt, float segLength, bool upper, float restK, float breakK) {
		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r1.init(
			from,                                    
			to, 
			fixt, 
			amount, 
			"Rope: " + name, 
			ropeStiffness * (upper ? 2 : 1), 
			ropeDampening,
			ropePointMass,
			segLength,
			restK,
			breakK
		);
		ropes.Add(r1);
		return r1;
	}

		/** 
	 * Update that is called once per frame.
	 */
	void FixedUpdate () {
		lap++;


		// Check for collisions
		foreach (PointController pc in ropes) {
			pc.collideWith (ball);
		}
		if (integrationMethod == 1) {
			integrator.euler(ropes, simulationStep, timestep);
		} else {
			integrator.integrate(ropes, simulationStep, timestep);
		}
		ball.simulate (timestep);
	}

	/**
	 * Simulate one step in the simulation.
	 */
	void simulationStep() {
		foreach (PointController pc in ropes) {
			pc.clearForces ();
		}
		foreach (PointController pc in ropes) {
			pc.simulationStep ();
		}
	}
}