using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point {

	public Vector3 position;
	public Vector3 velocity;
	public Vector3 force;

	public int mass;

	List<Point> neighbours;


	private int segLength = 2;
	private int ropeStiff = 800;
	private float ropeDamp = 7;

	public Point(Vector3 pos, int m) {
		position = pos;
		mass = m;

		neighbours = new List<Point>();
	}


	public void addNeighbour(Point p) {
		neighbours.Add(p);
	}

	public void CalculateForces() {

		// Spring force
		for (int i = 0; i < neighbours.Count; ++i) {
			Point n = neighbours [i];
			Vector3 localforce = Vector3.zero;
			Vector3 distance = n.position - position;

			localforce = ropeStiff * (distance.magnitude - segLength) * (distance / distance.magnitude);
			localforce -= ropeDamp * (velocity - n.velocity);

			force += localforce;
			n.force -= localforce;
		}


	}

	public void ResetForce() {
		force = Vector3.zero;
	}

}
