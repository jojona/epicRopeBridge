using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RK4 {
	public float timestep = 0.05f;

	public void Euler(List<Point> points) {
		int amount = points.Count;
		Point point;
		for (int i = 0; i < amount; ++i) {
			//points[i].CalculateForces ();

		}
		for (int i = 0; i < amount; ++i) {
			point = points [i];
			point.velocity += timestep * point.force / point.mass;
			point.position += timestep * point.velocity;
		}

	}

	public void RungeKutta4(List<Point> points){
		int amount = points.Count;
	}
}
