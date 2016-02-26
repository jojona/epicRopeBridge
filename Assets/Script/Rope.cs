using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Rope : PointController {

	private Vector3 startPoint;
	private Vector3 endPoint;

	private List<Point> anchors;

	public void Start() {
		
	}

	public void init(Vector3 start, Vector3 end, bool anchor, int amountOfPoints, string name) {
		points = new List<Point> ();
		Vector3 position = start;
		Vector3 ropeDirection = (end - start) / amountOfPoints;

		for (int i = 0; i < amountOfPoints; ++i) {
			points.Add(createPoint (position, name + i));
			position += ropeDirection;
		}
		for (int i = 0; i < amountOfPoints - 1; ++i) {
			points [i].AddNeigbour(points [i + 1]);
		}

		if (anchor) {
			anchors = new List<Point> ();
			anchors.Add (points [0]);
			anchors.Add(points[amountOfPoints - 1]);
		}
	}

	public int amountOfPoints() {
		return points.Count;
	}

	public Point getPoint(int index) {
		return points[index];
	}
}
