using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point : MonoBehaviour {

	public Vector3 velocity;
	public Vector3 force;

	public int mass;

	public List<Point> neighbours;


	private int segLength = 2;
	private int ropeStiff = 800;
	private float ropeDamp = 7;

	public Point(Vector3 pos, int m) {
		transform.position = pos;
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
			Vector3 distance = n.transform.position - transform.position;

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
