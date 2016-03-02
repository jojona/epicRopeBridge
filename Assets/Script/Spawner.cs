﻿using UnityEngine;
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

		// Spawn all points and planks
// ######################## Change spwan here ###################################################
		spawn();
		//miniSpawn();
		//plankSpawn();


		// Creates RK4 object for further calculations of movement
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
		}
	}

	/**
	 * Mini spawn
	 */
	void miniSpawn() {
		anchorPointEnd = anchorPointStart + Vector3.right * 10;
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

	/*
	 * Spawn test box for rigidbody simulation
	*/
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
	void FixedUpdate () {
		lap++;
		if (lap == 1400) {
			EditorApplication.isPaused = true;
		}

		for (int i = 0; i < amountOfPointsPerRope; ++i) {
			// Debug.Log (""); // Paus before decommenting this
		}
// ######################## Swap integration method here ###################################################
		integrator.euler(ropes, simulationStep);
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
}