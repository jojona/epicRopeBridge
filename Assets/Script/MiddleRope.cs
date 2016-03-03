﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MiddleRope : PointController {

	public Plank plankPrefab;

	private Plank plank;

	// Use this for initialization
	void Start () {
		
	}

	/*
	 * Initialize the ropes with a plank
	 */
	public void init(bool withPlank, int amount, float segmentLength, Point p1, Point p2, Point p3, Point p4, Vector3 direction, float stiffness, float dampening) {
		init (stiffness, dampening, segmentLength);

		// Names
		p1.name += " P1"; p2.name += " P2"; p3.name += " P3"; p4.name += " P4";

		Vector3 position = p1.position + (p3.position - p1.position) / 2 + (p2.position - p1.position) / 2 + Vector3.down * segmentLength * amount;

		for (int i = 0; i < amount; ++i) {

			Vector3 corner1 = position - (p3.position - p1.position) / 5 - (p2.position - p1.position) / 5;
			Vector3 corner2 = position - (p3.position - p1.position) / 5 + (p2.position - p1.position) / 5;
			Vector3 corner3 = position + (p3.position - p1.position) / 5 - (p2.position - p1.position) / 5;
			Vector3 corner4 = position + (p3.position - p1.position) / 5 + (p2.position - p1.position) / 5; 

			if (i == 0) {
					createCorner (corner1 + (p1.position - corner1) * i / (amount) , "A"+i);
					createCorner (corner2 + (p2.position - corner2) * i / (amount) , "B"+i);
					createCorner (corner3 + (p3.position - corner3) * i / (amount) , "C"+i);
					createCorner (corner4 + (p4.position - corner4) * i / (amount) , "D"+i);
				
				} else {
					createPoint (corner1 + (p1.position - corner1) * i / (amount) , "A"+i);
					createPoint (corner2 + (p2.position - corner2) * i / (amount) , "B"+i);
					createPoint (corner3 + (p3.position - corner3) * i / (amount) , "C"+i);
					createPoint (corner4 + (p4.position - corner4) * i / (amount) , "D"+i);
				}
		}

		for (int i = 0; i < amount - 1; ++i) {
			points [4 * i + 0].AddNeigbour (points[4 * (i + 1) + 0]);
			points [4 * i + 1].AddNeigbour (points[4 * (i + 1) + 1]);
			points [4 * i + 2].AddNeigbour (points[4 * (i + 1) + 2]);
			points [4 * i + 3].AddNeigbour (points[4 * (i + 1) + 3]);
		}

		plank = (Plank)Instantiate (plankPrefab, position, Quaternion.identity);
		plank.position = position;
		plank.transform.parent = transform;
		plank.transform.forward = direction;
		plank.init (points[0], points[1], points[2], points[3]);
		integrateList.Add(new IntegrateDataPlank(plank));


		// Names
		points [0].name += " Corner1"; points [1].name += " Corner2";	points [2].name += " Corner3"; points [3].name += " Corner4";

		points [amount * 4 - 4].AddNeigbour (p1);
		points [amount * 4 - 3].AddNeigbour (p2);
		points [amount * 4 - 2].AddNeigbour (p3);
		points [amount * 4 - 1].AddNeigbour (p4);
	}

	public void init(bool withPlank, int amount, float segmentLength, Point p1, Point p2, Point p3, Point p4, Point p5, Point p6, Point p7, Point p8, Vector3 direction, float stiffness, float dampening) {
		init (withPlank, amount, segmentLength, p1, p2, p3, p4, direction, stiffness, dampening);

		points[0].AddNeigbour(p5);
		points[1].AddNeigbour(p6);
		points[2].AddNeigbour(p7);
		points[3].AddNeigbour(p8);

	}

	override public void simulationStep() {
		gravity ();
		springForces ();

		plank.simulation ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public void clearMovement() {
		foreach(Point p in points) {
			p.clearMovement();
		}
		plank.clearMovement();
	}
}
