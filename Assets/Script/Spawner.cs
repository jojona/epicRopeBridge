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
	public Box boxPrefab;
	public MiddleRope middleRopePrefab;

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

	public bool spawnSmall = false;
	public bool spawnNormal = true;
	public bool spawnBox = false;
	public bool spawnRopes = false;
	public bool spawnMini = false;

	/**
	 * Private fields.
	 */

	// Lists of ropes
	private List<PointController> ropes;

	// Experiment object // TODO remove
	private Box box;

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
		/*
		if (spawnNormal) spawn();
		if (spawnSmall) spawnSmaller();
		if (spawnMini) miniSpawn();
		if (spawnBox) plankSpawn();
		if (spawnRopes) spawnOnlyRopes ();
		*/
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
		Rope r1 = spawnRope(anchorPoint1Up, anchorPoint2Up, "Upper right", extraUp * 2 + baseAmount1, true, segLengthUpRopes);
		Rope r2 = spawnRope(anchorPoint3Up, anchorPoint4Up, "Upper left", extraUp * 2 + baseAmount1, true, segLengthUpRopes);
		Rope r3 = spawnRope(anchorPoint1Down, anchorPoint2Down, "Down right", extraDown * 2 + baseAmount2, true, segLengthDownRopes);
		Rope r4 = spawnRope(anchorPoint3Down, anchorPoint4Down, "Down left", extraDown * 2 + baseAmount2, true, segLengthDownRopes);

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
			Point from = lower.getPoint (extraDown + i * (planksPerTriangle * (2 + plankSpacing)) - plankSpacing / 2 - 1 + (i == 0 ? 1 : 0));
			Point to = upper.getPoint (extraUp + upperPointsPerTriangle * i + upperPointsPerTriangle / 2);
			float lengthOfRope = (to.position - from.position).magnitude;
			int numberOfPoints = (int) (lengthOfRope * pointDensity);
			Vector3 diff = (to.position - from.position) / (numberOfPoints + 1);
			Rope rx = spawnRope(from.position + diff, to.position, "Triangular " + (i + 1) + " ", numberOfPoints, false, segLengthVerticalRopes + 0.00f * Mathf.Abs(i - numberOfPlanks/planksPerTriangle/2));
			rx.getPoint(0).AddNeigbour(from);
			rx.getPoints ().Last ().AddNeigbour (to);

			from = upper.getPoint (extraUp + upperPointsPerTriangle * i + upperPointsPerTriangle / 2);
			to = lower.getPoint (extraDown + (i + 1) * (planksPerTriangle * (2 + plankSpacing)) - plankSpacing / 2 - 1 - (i == numberOfPlanks / planksPerTriangle - 1 ? 1 : 0));
			diff = (to.position - from.position) / (numberOfPoints + 1);
			Rope ry = spawnRope(from.position + diff, to.position, "Triangular " + (i + 1) + " ", numberOfPoints, false, segLengthVerticalRopes + 0.00f * (numberOfPlanks/planksPerTriangle/2 - Mathf.Abs(i - numberOfPlanks/planksPerTriangle/2)));
			ry.getPoint(0).AddNeigbour(from);
			ry.getPoints ().Last ().AddNeigbour (to);
		}
	}

	void spawnVerticalRopes(Rope up, Rope down) {
		Vector3 dir1 = down.getPoint (extraDown).position - up.getPoint (extraUp).position;
		Rope rVerticalFirst = spawnRope(up.getPoint (extraUp).position + dir1 / (heightPoints + 1), down.getPoint (extraDown).position - dir1 / (heightPoints + 1), "Vertical rope", heightPoints, false, segLengthVerticalRopes);
		up.getPoint (extraUp).AddNeigbour(rVerticalFirst.getPoint(0));
		down.getPoint (extraDown).AddNeigbour(rVerticalFirst.getPoint(heightPoints-1));

		Vector3 dir2 = down.getPoint (down.length() - extraDown).position - up.getPoint (up.length() - extraUp - 1).position;
		Rope rVerticalSecond = spawnRope(up.getPoint (up.length() - extraUp - 1).position + dir2 / (heightPoints + 1), down.getPoint (down.length() - extraDown).position - dir1 / (heightPoints + 1), "Vertical rope", heightPoints, false, segLengthVerticalRopes);
		up.getPoint (up.length() - extraUp - 1).AddNeigbour(rVerticalSecond.getPoint(0));
		down.getPoint (down.length() - extraDown - 1).AddNeigbour(rVerticalSecond.getPoint(heightPoints-1));
	}

	Rope spawnRope(Vector3 from, Vector3 to, string name, int amount, bool fixt, float segLength) {
		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r1.init(
			from,                                    
			to, 
			fixt, 
			amount, 
			"Rope: " + name, 
			ropeStiffness, 
			ropeDampening,
			ropePointMass,
			segLength
		);
		ropes.Add(r1);
		return r1;
	}

		/** 
	 * Update that is called once per frame.
	 */
	void FixedUpdate () {
		lap++;
		if (integrationMethod == 1) {
			integrator.euler(ropes, simulationStep, timestep);
		} else {
			integrator.integrate(ropes, simulationStep, timestep);
		}
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

		if (box != null) {
			box.force = Vector3.zero;
			box.simulation ();
		}
	}

	/**
	 * Spawns rope.
	 */
	 /*
	void spawn() {
		// Calculate start points, direction and how long the rope will be
		Vector3 ropeDirection = (anchorPointEnd - anchorPointStart) / amountOfPointsPerRope;
		segmentLength = ropeDirection.magnitude;

		// Copies the ropePrefab and creates to rope start points
		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r2 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		int x = spacing > 1 ? 6 : 0;
		r1.init(
			anchorPointStart,                                    
			anchorPointEnd, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope1 ", 
			ropeStiffness, 
			ropeDampening
		);
		r2.init(
			anchorPointStart + Vector3.back * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope2 ", 
			ropeStiffness, 
			ropeDampening
		);

		Rope r3 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r4 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r3.init(
			anchorPointStart + Vector3.down * segmentLength * 3,                                    
			anchorPointEnd + Vector3.down * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope3 ", 
			ropeStiffness, 
			ropeDampening
		);
		r4.init(
			anchorPointStart + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope4 ", 
			ropeStiffness, 
			ropeDampening
		);

		// Adds newly created points to rope
		ropes.Add(r1);
		ropes.Add(r2);
		ropes.Add(r3);
		ropes.Add(r4);

		// Creates middle rope points and adds them to list
		for (int i = 0; i * 2 + 3 < 2 * amountOfPointsPerRope - 1; ++i) {
			MiddleRope r = (MiddleRope) Instantiate(middleRopePrefab, Vector3.zero, Quaternion.identity);
			r.init(true, 5, segmentLength/5, r1.getPoint (i * spacing + 1), r1.getPoint (i * spacing + spacing), r2.getPoint (i * spacing + 1), r2.getPoint (i * spacing + spacing), r3.getPoint (i * spacing + 1), r3.getPoint (i * spacing + spacing), r4.getPoint (i * spacing + 1), r4.getPoint (i * spacing + spacing), ropeDirection, middleRopeStiffness, ropeDampening);
			ropes.Add(r);
			r.name = "MiddleRope " + i;
		}
	}
*/
	/**
	 * Mini spawn
	 */
	 /*
	void miniSpawn() {
		Vector3 minianchorPointEnd = anchorPointStart + Vector3.right * 10;
		int miniamountOfPointsPerRope = 4;

		Vector3 ropeDirection = (minianchorPointEnd - anchorPointStart) / miniamountOfPointsPerRope;
		segmentLength = ropeDirection.magnitude;

		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r2 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r1.init(anchorPointStart, minianchorPointEnd, true, miniamountOfPointsPerRope, "rope1", ropeStiffness, ropeDampening);
		r2.init(anchorPointStart + Vector3.back * segmentLength * 3, minianchorPointEnd + Vector3.back * segmentLength * 3, true, miniamountOfPointsPerRope, "rope2", ropeStiffness, ropeDampening);
	
		// Adds newly created points to rope
		ropes.Add(r1);
		ropes.Add(r2);

		MiddleRope r = (MiddleRope) Instantiate(middleRopePrefab, Vector3.zero, Quaternion.identity);
		r.init(true, 1, segmentLength/5, r1.getPoint (1), r1.getPoint (2), r2.getPoint (1), r2.getPoint (2), ropeDirection, middleRopeStiffness, ropeDampening);
		ropes.Add(r);
	}
*/
	/*
	 * Spawn test box for rigidbody simulation
	*/
	/*
	void plankSpawn() {
		Vector3 position = new Vector3(15, 0,0 );
		box = (Box)Instantiate (boxPrefab, position, Quaternion.identity);
		box.position = position;
		box.transform.parent = transform;
		box.transform.forward = Vector3.forward;
		box.init ();
	}
*/

	/**
	 * Spawns rope.
	 */
	 /*
	void spawnSmaller() {
		// Calculate start points, direction and how long the rope will be
		Vector3 ropeDirection = (anchorPointEnd - anchorPointStart) / amountOfPointsPerRope;
		segmentLength = ropeDirection.magnitude;

		// Copies the ropePrefab and creates to rope start points
		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r2 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		int x = spacing > 1 ? 6 : 0;
		r1.init(
			anchorPointStart,                                    
			anchorPointEnd, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope1 ", 
			ropeStiffness, 
			ropeDampening
		);
		r2.init(
			anchorPointStart + Vector3.back * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope2 ", 
			ropeStiffness, 
			ropeDampening
		);

		Rope r3 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r4 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r3.init(
			anchorPointStart + Vector3.down * segmentLength * 3,                                    
			anchorPointEnd + Vector3.down * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope3 ", 
			ropeStiffness, 
			ropeDampening
		);
		r4.init(
			anchorPointStart + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope4 ", 
			ropeStiffness, 
			ropeDampening
		);

		// Adds newly created points to rope
		ropes.Add(r1);
		ropes.Add(r2);
		ropes.Add(r3);
		ropes.Add(r4);

		// Creates middle rope points and adds them to list
		int i = amountOfPointsPerRope / 2;

		MiddleRope r = (MiddleRope) Instantiate(middleRopePrefab, Vector3.zero, Quaternion.identity);
		r.init(true, 5, segmentLength/5, r1.getPoint (i * spacing + 1), r1.getPoint (i * spacing + spacing), r2.getPoint (i * spacing + 1), r2.getPoint (i * spacing + spacing), r3.getPoint (i * spacing + 1), r3.getPoint (i * spacing + spacing), r4.getPoint (i * spacing + 1), r4.getPoint (i * spacing + spacing), ropeDirection, middleRopeStiffness, ropeDampening);
		ropes.Add(r);
	}

	public void spawnOnlyRopes () {
		// Calculate start points, direction and how long the rope will be
		Vector3 ropeDirection = (anchorPointEnd - anchorPointStart) / amountOfPointsPerRope;
		segmentLength = ropeDirection.magnitude;

		// Copies the ropePrefab and creates to rope start points
		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r2 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		int x = spacing > 1 ? 6 : 0;
		r1.init(
			anchorPointStart,                                    
			anchorPointEnd, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope1 ", 
			ropeStiffness, 
			ropeDampening
		);
		r2.init(
			anchorPointStart + Vector3.back * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope2 ", 
			ropeStiffness, 
			ropeDampening
		);

		Rope r3 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r4 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r3.init(
			anchorPointStart + Vector3.down * segmentLength * 3,                                    
			anchorPointEnd + Vector3.down * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope3 ", 
			ropeStiffness, 
			ropeDampening
		);
		r4.init(
			anchorPointStart + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope4 ", 
			ropeStiffness, 
			ropeDampening
		);

		// Adds newly created points to rope
		ropes.Add(r1);
		ropes.Add(r2);
		ropes.Add(r3);
		ropes.Add(r4);
	}
	*/
}