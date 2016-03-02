using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	public Vector3 anchorPointStart = Vector3.zero;
	public Vector3 anchorPointEnd   = Vector3.zero + Vector3.right * 10;
	public int spacing = 3;

	// Number of points per rope
	public int amountOfPointsPerRope = 10 * 4;

	// The time between each frame
	public float timestep = 1.0f / 60.0f;

	// Rope properties (stiffness and dampening)
	public float ropeStiffness = 800f;
	public float middleRopeStiffness = 800f;
	public float ropeDampening = 1f;

	private bool first = true;
	/**
	 * Private fields.
	 */
	// Total number of points, set when initiating points
	private int totalPoints;

	// Lists of points, ropes and planks
	private List<Point> points;
	private List<Plank> planks;
	private List<PointController> ropes;

	private Box box;

	// Length of each segment (space between rope points)
	private float segmentLength;

	// Runge-Kutta RK4 object
	private RK4 rk4;
	private Integrator integrator;

	/**
	 * Initializes the ropes and planks.
	 */
	void Start () {
		// Init lists
		points = new List<Point> ();
		planks = new List<Plank> ();
		ropes = new List<PointController> ();

		// Spawn all points and planks
// ######################## Change spwan here ###################################################
		spawn();
		//simpleSpawn ();
		//miniSpawn();
		//plankSpawn();

		// Creates RK4 object for further calculations of movement
		rk4 = new RK4 (points, amountOfPointsPerRope, totalPoints, ropeStiffness, ropeDampening, segmentLength);
		integrator = new Integrator (ropes, timestep);
	}

	/**
	 * Spawns rope.
	 */
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
			"rope1", 
			ropeStiffness, 
			ropeDampening
		);
		r2.init(
			anchorPointStart + Vector3.back * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope2", 
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
			"rope3", 
			ropeStiffness, 
			ropeDampening
		);
		r4.init(
			anchorPointStart + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			anchorPointEnd + Vector3.back * segmentLength * 3 + Vector3.down * segmentLength * 3, 
			true, 
			amountOfPointsPerRope * spacing - x, 
			"rope24", 
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
		}
		totalPoints = 0;
	}

	/**
	 * Mini spawn
	 */
	void miniSpawn() {
		anchorPointEnd = anchorPointStart + Vector3.right * 5;
		amountOfPointsPerRope = 4;

		Vector3 ropeDirection = (anchorPointEnd - anchorPointStart) / amountOfPointsPerRope;
		segmentLength = ropeDirection.magnitude;

		Rope r1 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		Rope r2 = (Rope) Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
		r1.init(anchorPointStart, anchorPointEnd, true, amountOfPointsPerRope, "rope1", ropeStiffness, ropeDampening);
		r2.init(anchorPointStart + Vector3.back * segmentLength * 3, anchorPointEnd + Vector3.back * segmentLength * 3, true, amountOfPointsPerRope, "rope2", ropeStiffness, ropeDampening);
	
		// Adds newly created points to rope
		ropes.Add(r1);
		ropes.Add(r2);

		MiddleRope r = (MiddleRope) Instantiate(middleRopePrefab, Vector3.zero, Quaternion.identity);
		r.init(true, 1, segmentLength/5, r1.getPoint (1), r1.getPoint (2), r2.getPoint (1), r2.getPoint (2), ropeDirection, middleRopeStiffness, ropeDampening);
		ropes.Add(r);
	}

	void plankSpawn() {
		Vector3 position = new Vector3(15, 0,0 );
		box = (Box)Instantiate (boxPrefab, position, Quaternion.identity);
		box.position = position;
		box.transform.parent = transform;
		box.transform.forward = Vector3.forward;
		box.init ();
	}

	/** 
	 * Update that is called once per frame.
	 */
	void Update () {
		if (!first) {
			first = !first;
		}

		for (int i = 0; i < amountOfPointsPerRope; ++i) {
			// Debug.Log (""); // Paus before decommenting this
		}
// ######################## Swap integration method here ###################################################
		rk4.newEuler(simulationStep, ropes);
		//integrator.integrate(ropes, simulationStep);
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

	/*
	 *  ###################################################################
	 * 
	 * Simple simulation spawner below
	 * 
	 *  ###################################################################
	 */

	/**
	 * Spawns a simple simulation.
	 */
	void simpleSpawn() {
		// Set position, space between points and rope direction
		Vector3 position = anchorPointStart;
		Vector3 ropeDirection = (anchorPointEnd - anchorPointStart) / amountOfPointsPerRope;
		segmentLength = ropeDirection.magnitude;

		// Spawn rope 1:s points
		position = spawnRope(position, ropeDirection, amountOfPointsPerRope, 1);

		// Set new position for rope 2 and spawn it
		position -= ropeDirection * amountOfPointsPerRope + Vector3.back * segmentLength * 3;
		position = spawnRope(position, ropeDirection, amountOfPointsPerRope, 2);

		// Spawn middle rope
		position = spawnMiddleRope(ropeDirection, position);

		// Be sure to set this to the correct value, count your points and your good to go
		totalPoints = amountOfPointsPerRope * 3;
	}

	/**
	 * Spawns points for a rope and adds neighbours.
	 */
	Vector3 spawnRope(Vector3 position, Vector3 ropeDirection, int amountOfPointsPerRope, int ropeNumber) {
		int ropeIndex = ropeNumber - 1;

		// Add points
		for (int i = 0; i < amountOfPointsPerRope; ++i) {
			points.Add(createPoint(position, "Rope " + ropeNumber + ", point " + i + " (" + (i + ropeIndex * amountOfPointsPerRope) + ")"));
			position += ropeDirection;
		}

		// Add links between points
		for (int i = 1 + amountOfPointsPerRope * ropeIndex; i < amountOfPointsPerRope * ropeIndex; ++i) {
			points[i].AddNeigbour(points[i - 1]);
		}

		// Uncomment for double linked points
		/*
		for (int i = 0; i < amountOfPointsPerRope - 1; ++i) {
			points[i + (amountOfPointsPerRope * ropeNumber)].AddNeigbour(points [i + 1]);
		}
		*/
		return position;
	}

	/** 
	 * Spawns middle ropes.
	 */
	Vector3 spawnMiddleRope(Vector3 ropeDirection, Vector3 position) {
		for (int i = 0; i < amountOfPointsPerRope / 2; ++i) {
			points.Add(createPoint((anchorPointStart + ropeDirection * (2 * i)) + Vector3.forward * segmentLength, "MiddlePoint 1, point " + i));
			points.Add(createPoint((anchorPointStart + ropeDirection * (2 * i)) + Vector3.forward * segmentLength * 2, "MiddlePoint 2, point " + i));

			if (i % 2 == 0) {
				points[amountOfPointsPerRope * 2 + 2 * i].AddNeigbour (points [amountOfPointsPerRope * 2 + 1 + 2 * i]);
			} else {
				Plank p = (Plank)Instantiate (plankPrefab, position, Quaternion.identity);
				p.init (points [amountOfPointsPerRope * 2 + 2 * i], points [amountOfPointsPerRope * 2 + 1 + 2 * i], points[0], points[0]);
				planks.Add (p);
			}
			points [amountOfPointsPerRope * 2 + 2 * i].AddNeigbour (points [2 * i]);
			points [amountOfPointsPerRope * 2 + 1 + 2*i].AddNeigbour (points[amountOfPointsPerRope + 2 * i]);
		}
		return position;
	}


	/**
	 * Creates a point at given position.
	 */
	private Point createPoint(Vector3 position) {
		Point p = (Point) Instantiate(pointPrefab, position, Quaternion.identity);
		p.position = position;
		p.transform.parent = transform;
		return p;
	}

	/**
	 * Creates a point at given position with a name.
	 */
	private Point createPoint(Vector3 position, string name) {
		Point p = createPoint(position);
		p.name = name;
		return p;
	}
}