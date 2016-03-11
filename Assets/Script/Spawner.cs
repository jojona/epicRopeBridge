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
	public float segLengthDownRopes = 4f;
	public float segLengthVerticalRopes = 1;

	public float ropePointMass;
	public int extraUp = 4;
	public int extraDown = 2;
	public int planksPerTriangle = 2;
	[Tooltip("Delbart med planksPerTriangle")]
	public int numberOfPlanks = 12; // Delbart med planksPerTriangle
	public int pointsPerPlank = 2;
	public int plankSpacing = 1;
	public int heightPoints = 7;
	public int plankWidth = 10;

	// The time between each frame
	public float timestep = 1.0f / 60.0f;

	// Rope properties (stiffness and damping)
	public float ropeStiffness = 1000f;
	public float middleRopeStiffness = 1000f;
	public float ropeDamping = 7f;

	[Range(1, 2)]
	[Tooltip("Euler 1, Runge kutta 2")]
	public int integrationMethod = 2;

	public float triangluarRestForce = 540f;
	public float verticalRestForce = 100f;
	public float upRestForce = 5400f;
	public float downRestForce = 4500f;

	public float triangularMaxForce = 800f;
	public float verticalMaxForce = 150f;
	public float upMaxForce = 5600f;
	public float downMaxForce = 4501f;

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

		// The 4 main ropes
		Rope r1 = spawnRope(anchorPoint1Up, anchorPoint2Up, "Upper right", extraUp * 2 + baseAmount1, true, segLengthUpRopes, upRestForce, upMaxForce);
		Rope r2 = spawnRope(anchorPoint3Up, anchorPoint4Up, "Upper left", extraUp * 2 + baseAmount1, true, segLengthUpRopes, upRestForce, upMaxForce);
		Rope r3 = spawnRope(anchorPoint1Down, anchorPoint2Down, "Down right", extraDown * 2 + baseAmount2, true, segLengthDownRopes, downRestForce, downMaxForce);
		Rope r4 = spawnRope(anchorPoint3Down, anchorPoint4Down, "Down left", extraDown * 2 + baseAmount2, true, segLengthDownRopes, downRestForce, downMaxForce);

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

		int upperPointsPerTriangle = (planksPerTriangle * (pointsPerPlank + plankSpacing)) - plankSpacing / 2;
		for (int i = 0; i < numberOfPlanks / planksPerTriangle; i++) {
			Point from = lower.getPoint (extraDown + i * (planksPerTriangle * (2 + plankSpacing)) - plankSpacing / 2 - 1);
			Point to = upper.getPoint (extraUp + upperPointsPerTriangle * i + upperPointsPerTriangle / 2);
			float lengthOfRope = (to.position - from.position).magnitude;
			int numberOfPoints = (int) (lengthOfRope * pointDensity);
			Vector3 diff = (to.position - from.position) / (numberOfPoints + 1);
			float segmentLength = segLengthVerticalRopes * 2f / 5f + 0.00f * Mathf.Abs(i - numberOfPlanks/planksPerTriangle/2);
			Rope rx = spawnRope(from.position + diff, to.position, "Triangular X " + (i + 1) + " ", numberOfPoints, false, segmentLength, triangluarRestForce, triangularMaxForce);
			rx.getPoint(0).AddNeighbour(from, triangluarRestForce, triangularMaxForce);
			rx.getPoints ().Last ().AddNeighbour (to, triangluarRestForce, triangularMaxForce);
			
			from = upper.getPoint (extraUp + upperPointsPerTriangle * i + upperPointsPerTriangle / 2);
			to = lower.getPoint (extraDown + (i + 1) * (planksPerTriangle * (2 + plankSpacing)) - plankSpacing / 2 - 1);
			diff = (to.position - from.position) / (numberOfPoints + 1);
			segmentLength = segLengthVerticalRopes * 2f / 5f + 0.00f * (numberOfPlanks/planksPerTriangle/2 - Mathf.Abs(i - numberOfPlanks/planksPerTriangle/2));
			Rope ry = spawnRope(from.position + diff, to.position, "Triangular Y " + (i + 1) + " ", numberOfPoints, false, segmentLength, triangluarRestForce, triangularMaxForce);
			ry.getPoint(0).AddNeighbour(from, triangluarRestForce, triangularMaxForce);
			ry.getPoints ().Last ().AddNeighbour (to, triangluarRestForce, triangularMaxForce);
		}
	}

	void spawnVerticalRopes(Rope up, Rope down) {

		Vector3 dir1 = down.getPoint (extraDown - 1).position - up.getPoint (extraUp - 2).position;
		Rope rVerticalFirst = spawnRope(up.getPoint (extraUp - 2).position + dir1 / (heightPoints + 1), down.getPoint (extraDown - 1).position - dir1 / (heightPoints + 1), "Vertical rope", heightPoints + 1, false, segLengthVerticalRopes, verticalRestForce, verticalMaxForce);
		up.getPoint (extraUp - 2).AddNeighbour(rVerticalFirst.getPoint(0), verticalRestForce, verticalMaxForce);
		down.getPoint (extraDown - 1).AddNeighbour(rVerticalFirst.getPoint(heightPoints), verticalRestForce, verticalMaxForce);

		Vector3 dir2 = down.getPoint (down.length() - extraDown + 1).position - up.getPoint (up.length() - extraUp + 1).position;
		Rope rVerticalSecond = spawnRope(up.getPoint (up.length() - extraUp + 1).position + dir2 / (heightPoints + 1), down.getPoint (down.length() - extraDown + 1).position - dir1 / (heightPoints + 1), "Vertical rope", heightPoints + 1, false, segLengthVerticalRopes, verticalRestForce, verticalMaxForce);
		up.getPoint (up.length() - extraUp + 1).AddNeighbour(rVerticalSecond.getPoint(0), verticalRestForce, verticalMaxForce);
		down.getPoint (down.length() - extraDown).AddNeighbour(rVerticalSecond.getPoint(heightPoints), verticalRestForce, verticalMaxForce);
	}

	Rope spawnRope(Vector3 from, Vector3 to, string name, int amount, bool fixt, float segLength, float restF, float maxF) {
		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r1.init(
			from,                                    
			to, 
			fixt, 
			amount, 
			"Rope: " + name, 
			ropeStiffness, 
			ropeDamping,
			ropePointMass,
			segLength,
			restF,
			maxF
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