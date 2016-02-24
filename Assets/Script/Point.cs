using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point {

	Vector3 position;
	Vector3 velocity;

	List<Point> neighbours;


	public Point(Vector3 pos) {
		position = pos;

		neighbours = new List<Point>();
	}


	public void addNeighbour(Point p) {
		neighbours.Add(p);
	}

	public void CalculateForces() {

	}
}
