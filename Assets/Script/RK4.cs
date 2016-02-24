using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RK4 {
	public float timestep = 0.05f;

	public void Euler(List<Point> points) {
		int amount = points.Count;
		Point point;
		for (int i = 0; i < amount; ++i) {
			point = points [i];
			point.CalculateForces ();
			point.velocity += timestep * point.force / point.mass;
			point.transform.position += timestep * point.velocity;


		}
	}
}
