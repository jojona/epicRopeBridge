using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MiddleRope : PointController {

	public Plank plankPrefab;

	private Plank plank;

	// Use this for initialization
	void Start () {
		points = new List<Point> ();
	}

	public void init(bool withPlank, int amount, int segmentLength, Point p1, Point p2, Point p3, Point p4) {

		Vector3 position1 = (p2.position - p1.position) / 2;
		Vector3 position2 = (p4.position - p3.position) / 2;
		Vector3 position = (position2 - position1) / 2 + Vector3.down * segmentLength * amount;

		plank = (Plank)Instantiate (plankPrefab, position, Quaternion.identity);

		for (int i = 0; i < amount; ++i) {

			Point pnew1 = createPoint ((position - p1.position) * i / amount);
			points.Add (pnew1);
			Point pnew2 = createPoint ((position - p2.position) * i / amount);
			points.Add (pnew2);
			Point pnew3 = createPoint ((position - p3.position) * i / amount);
			points.Add (pnew3);
			Point pnew4 = createPoint ((position - p4.position) * i / amount);
			points.Add (pnew4);
		}

		for (int i = 0; i < amount - 1; ++i) {
			points [i].AddNeigbour (points[i+1]);
			points [amount + i].AddNeigbour (points[amount + i +1]);
			points [amount * 2 + i].AddNeigbour (points[amount * 2 + i +1]);
			points [amount * 3 + i].AddNeigbour (points[amount * 3 + i +1]);
		}

			
		plank.init (points[0], points[amount], points[amount * 2], points[amount * 3]);

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
