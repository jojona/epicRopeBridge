using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MiddleRope : PointController {

	public Plank plankPrefab;

	private Plank plank;

	// Use this for initialization
	void Start () {
		
	}

	public void init(bool withPlank, int amount, float segmentLength, Point p1, Point p2, Point p3, Point p4, Vector3 direction, float stiffness, float dampening) {
		init (stiffness, dampening, segmentLength);

		p1.name += " P1";
		p2.name += " P2";
		p3.name += " P3";
		p4.name += " P4";

		Vector3 position = p1.position + (p3.position - p1.position) / 2 + (p2.position - p1.position) / 2 + Vector3.down * segmentLength * amount;

		for (int i = 0; i < amount; ++i) {

			Vector3 corner1 = position - (p3.position - p1.position) / 5 - (p2.position - p1.position) / 5;
			Vector3 corner2 = position - (p3.position - p1.position) / 5 + (p2.position - p1.position) / 5;
			Vector3 corner3 = position + (p3.position - p1.position) / 5 - (p2.position - p1.position) / 5;
			Vector3 corner4 = position + (p3.position - p1.position) / 5 + (p2.position - p1.position) / 5; 

			//Debug.DrawRay (corner1, Vector3.up, Color.green, 1000f, true);
			//Debug.DrawRay (corner2, Vector3.up, Color.yellow, 1000f, true);
			//Debug.DrawRay (corner3, Vector3.up, Color.black, 1000f, true);
			//Debug.DrawRay (corner4, Vector3.up, Color.red, 1000f, true);

			Point pnew1 = createPoint (corner1 + (p1.position - corner1) * i / (amount) , "A"+i);
			points.Add (pnew1);
			Point pnew2 = createPoint (corner2 + (p2.position - corner2) * i / (amount) , "B"+i);
			points.Add (pnew2);
			Point pnew3 = createPoint (corner3 + (p3.position - corner3) * i / (amount) , "C"+i);
			points.Add (pnew3);
			Point pnew4 = createPoint (corner4 + (p4.position - corner4) * i / (amount) , "D"+i);
			points.Add (pnew4);
		}
			
		//Debug.DrawRay (position, p1.position - position , Color.green, 10000f, true);

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
		points [0].name += "Corner1";
		points [1].name += "Corner2";
		points [2].name += "Corner3";
		points [3].name += "Corner4";

		// TODO check below if right
		/*
		*/
		p1.AddNeigbour (points[amount * 4 - 4]);
		p2.AddNeigbour (points[amount * 4 - 3]);
		p3.AddNeigbour (points[amount * 4 - 2]);
		p4.AddNeigbour (points[amount * 4 - 1]);
		/*
		points [amount * 4 - 4].AddNeigbour (p1);
		points [amount * 4 - 3].AddNeigbour (p2);
		points [amount * 4 - 2].AddNeigbour (p3);
		points [amount * 4 - 1].AddNeigbour (p4);
		*/

		points[amount * 4 - 4].name += "Fel 1";
		points[amount * 4 - 3].name += "Fel 2";
		points[amount * 4 - 2].name += "Fel 3";
		points[amount * 4 - 1].name += "Fel 4";

	}

	override public void simulationStep() {
		gravity ();
		springForces ();

		// TODO Plankforce();

		plank.simulation ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
