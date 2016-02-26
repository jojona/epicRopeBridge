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

	public void init(bool withPlank, int amount, float segmentLength, Point p1, Point p2, Point p3, Point p4) {
		points = new List<Point> ();

		p1.name = "P1";
		p2.name = "P2";
		p3.name = "P3";
		p4.name = "P4";


		Vector3 position = p1.position + (p3.position - p1.position) / 2 + (p2.position - p1.position) / 2 + Vector3.down * segmentLength * amount;

		for (int i = 0; i < amount; ++i) {
			Point pnew1 = createPoint (position + (p1.position - position) * i / (amount) , "A"+i);
			points.Add (pnew1);
			Point pnew2 = createPoint (position + (p2.position - position) * i / (amount) , "B"+i);
			points.Add (pnew2);
			Point pnew3 = createPoint (position + (p3.position - position) * i / (amount) , "C"+i);
			points.Add (pnew3);
			Point pnew4 = createPoint (position + (p4.position - position) * i / (amount) , "D"+i);
			points.Add (pnew4);
		}
			
		Debug.DrawRay (position, p1.position - position , Color.green, 10000f, true);

		for (int i = 0; i < amount - 1; ++i) {
			points [i].AddNeigbour (points[i+1]);
			points [amount + i].AddNeigbour (points[amount + i +1]);
			points [amount * 2 + i].AddNeigbour (points[amount * 2 + i +1]);
			points [amount * 3 + i].AddNeigbour (points[amount * 3 + i +1]);
		}

		Vector3 plankposition = p1.position + (p3.position - p1.position) / 2 + (p2.position - p1.position) / 2 + Vector3.down * segmentLength * amount;
		plank = (Plank)Instantiate (plankPrefab, plankposition, Quaternion.identity);
		plank.position = plankposition;
		plank.transform.parent = transform;
		plank.init (points[0], points[1], points[2], points[3]);
		points [0].name = "Corner1";
		points [1].name = "Corner2";
		points [2].name = "Corner3";
		points [3].name = "Corner4";

		// TODO check below if right
		p1.AddNeigbour (points[amount - 1]);
		p2.AddNeigbour (points[amount  * 2 - 1]);
		p3.AddNeigbour (points[amount * 3 - 1]);
		p4.AddNeigbour (points[amount * 4 - 1]);

	}

	public Point getEdge1() {
		return points[0];

		// 
	}

	public Point getEdge2() {
		return points[points.Count/ 4];
	}

	public Point getEdge3() {
		return points[points.Count/ 4 * 2];
	}

	public Point getEdge4() {
		return points[points.Count/ 4 * 3];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
