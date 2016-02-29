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
	public MiddleRope middleRopePrefab;

	// Anchor points for the bridge
	public Vector3 anchorPointStart = Vector3.zero;
	public Vector3 anchorPointEnd   = Vector3.zero + Vector3.right * 10;

	// Number of points per rope
	public int amountOfPointsPerRope = 10;

	// The time between each frame
	public float timestep = 1.0f / 60.0f;

	// Rope properties (stiffness and dampening)
	public float ropeStiffnes = 800f;
	public float ropeDampening = 1f;

	/**
	 * Private fields.
	 */
	// Total number of points, set when initiating points
	private int totalPoints;

	// Lists of points, ropes and planks
	private List<Point> points;
	private List<Plank> planks;
	private List<PointController> ropes;

	// Length of each segment (space between rope points)
	private float segmentLength;

	// Runge-Kutta RK4 object
	private RK4 rk4;

	/**
	 * Initializes the ropes and planks.
	 */
	void Start () {
		// Init lists
		points = new List<Point> ();
		planks = new List<Plank> ();
		ropes = new List<PointController> ();

		// Spawn all points and planks
		//spawn();
		simpleSpawn ();

		// Creates RK4 object for further calculations of movement
		rk4 = new RK4 (points, amountOfPointsPerRope, totalPoints, ropeStiffnes, ropeDampening, segmentLength);
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
		r1.init(anchorPointStart, anchorPointEnd, true, amountOfPointsPerRope, "rope1");
		r2.init(anchorPointStart + Vector3.back * segmentLength * 3, anchorPointEnd + Vector3.back * segmentLength * 3, true, amountOfPointsPerRope, "rope2");

		// Adds newly created points to rope
		ropes.Add(r1);
		ropes.Add(r2);

		// Creates middle rope points and adds them to list
		for (int i = 0; i < 1/*amountOfPointsPerRope / 3*/; ++i) {
			MiddleRope r = (MiddleRope) Instantiate(middleRopePrefab, Vector3.zero, Quaternion.identity);
			r.init(true, 3, segmentLength, r1.getPoint (i * 2 + 2), r1.getPoint (i * 2 + 3), r2.getPoint (i * 2 + 2), r2.getPoint (i * 2 + 3));
			ropes.Add(r);
		}
		totalPoints = 0;
	}

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
	 * Update that is called once per frame.
	 */
	void FixedUpdate () {
		for (int i = 0; i < amountOfPointsPerRope; ++i) {
			// Debug.Log (""); // Paus before decommenting this
		}
		//rk4.euler();
	}

	/**
	 * Simulate one step in the simulation.
	 */
	void simulationStep() {
		clearForces();
		applyGravity();
		applySpringForces();
		endPoints(); // Make end points fixed
	}

	/**
	 * Sets force for endpoints to zero to make them fixed. 
	 * Should be called after calculating forces for all points.
	 */
	private void endPoints() {
		points [0].force 								= Vector3.zero;
		points [amountOfPointsPerRope].force 			= Vector3.zero;
		points [amountOfPointsPerRope - 1].force 		= Vector3.zero;
		points [(amountOfPointsPerRope * 2) - 1].force 	= Vector3.zero;
	}

	/** 
	 * Clears old forces and sets all to zero.
	 */
	private void clearForces() {
		for (int i = 0; i < totalPoints; ++i) {
			points [i].force = Vector3.zero;
		}
	}

	/**
	 * Applies gravity to all points.
	 */
	private void applyGravity() {
		for (int i = 0; i < totalPoints; ++i) {
			points [i].force += Vector3.down * 9.81f * points[i].mass;
		}
	}

	/**
	 * Applies spring forces to all points by walking through all points and making them affect their neighbours.
	 */
	private void applySpringForces() {
		for (int i = 0; i < totalPoints; ++i) {
			Point p = points[i];

			for (int j = 0; j < p.GetNeighours().Count; ++j) {
				Point n = p.GetNeighours() [j];

				Vector3 force = Vector3.zero;
				Vector3 distance = n.position - p.position;

				force = ropeStiffnes * (distance.magnitude - segmentLength) * (distance / distance.magnitude);
				force -= ropeDampening * (p.velocity - n.velocity);

				p.force += force;
				n.force -= force;
			}
		}
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
