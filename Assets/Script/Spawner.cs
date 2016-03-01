﻿using UnityEngine;
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
	public float ropeStiffness = 800f;
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
		spawn();
		//simpleSpawn ();

		// Creates RK4 object for further calculations of movement
		rk4 = new RK4 (points, amountOfPointsPerRope, totalPoints, ropeStiffness, ropeDampening, segmentLength);
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
		r1.init(anchorPointStart, anchorPointEnd, true, amountOfPointsPerRope, "rope1", ropeStiffness, ropeDampening);
		r2.init(anchorPointStart + Vector3.back * segmentLength * 3, anchorPointEnd + Vector3.back * segmentLength * 3, true, amountOfPointsPerRope, "rope2", ropeStiffness, ropeDampening);

		// Adds newly created points to rope
		ropes.Add(r1);
		ropes.Add(r2);

		// Creates middle rope points and adds them to list
		for (int i = 0; i < 1/*i * 2 + 3 < amountOfPointsPerRope - 1*/; ++i) {
			MiddleRope r = (MiddleRope) Instantiate(middleRopePrefab, Vector3.zero, Quaternion.identity);
			r.init(true, 2, segmentLength, r1.getPoint (i * 2 + 2), r1.getPoint (i * 2 + 3), r2.getPoint (i * 2 + 2), r2.getPoint (i * 2 + 3), ropeDirection, ropeStiffness, ropeDampening);
			ropes.Add(r);
		}
		totalPoints = 0;
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
		rk4.newEuler(simulationStep, ropes);
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