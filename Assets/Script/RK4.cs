using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RK4 {
	public float timestep = 1f/60f;
	private List<Point> points;

	private int amountOfPointsPerRope;
	private int totalPoints;

	private float ropeStiffnes;
	private float ropeDampening;

	private float segmentLength;

	public RK4(List<Point> p, int appr, int tp, float rS, float rD, float sL){
		points = p;

		amountOfPointsPerRope = appr;
		totalPoints = tp;

		ropeStiffnes = rS;
		ropeDampening = rD;
		segmentLength = sL;
	}


	public void RungeKutta4(){
		int amount = points.Count;
		Point p;

		for (int i = 0; i < amount; ++i) {
			p = points [i];
			p.dPos = p.velocity;
			p.dVel = Vector3.zero;
		}

		//K1
		evaluate(timestep*0f);
		for (int i = 0; i < amount; ++i) {
			p = points [i];

			// Initialize accumulators
			p.RK4PosAcc = p.position;
			p.RK4VelAcc = p.velocity;

			//Calculating RK4
			p.RK4PosAcc += p.dPos/6f;
			p.RK4VelAcc += p.dVel/6f;
		}

		//K2
		evaluate(timestep*0.5f);
		for (int i = 0; i < amount; ++i) {
			p = points [i];
			p.RK4PosAcc += 2f*p.dPos/6f;
			p.RK4VelAcc += 2f*p.dVel/6f;
		}

		//K3
		evaluate(timestep*0.5f);
		for (int i = 0; i < amount; ++i) {
			p = points [i];
			p.RK4PosAcc += 2f*p.dPos/6f;
			p.RK4VelAcc += 2f*p.dVel/6f;
		}

		//K4
		evaluate(timestep*1f);
		for (int i = 0; i < amount; ++i) {
			p = points [i];
			p.RK4PosAcc += p.dPos/6f;
			p.RK4VelAcc += p.dVel/6f;

			// Update final pos & vel    
			p.position += p.RK4PosAcc;
			p.velocity += p.RK4VelAcc;
		}

	}

	void evaluate(float dt) {
		clearForces ();
		gravity ();
		springForces ();
		endPoints ();
		Point p;
		for (int i = 0; i < points.Count; ++i) {
			p = points [i];
			// TODO initialize dPos & dVel
			p.statePos = p.position + p.dPos * dt; 
			p.stateVel = p.velocity + p.dVel * dt;

			p.dPos = p.statePos;
			p.dVel = p.force / p.mass;
		}

	}

	private void clearForces() {
		for (int i = 0; i < totalPoints; ++i) {
			points [i].force = Vector3.zero;
		}
	}

	private void gravity() {
		for (int i = 0; i < totalPoints; ++i) {
			points [i].force += Vector3.down * 9.81f * points[i].mass;
		}

		// Rope 1 endpoints
		//points [0].force += (Random.onUnitSphere * points [0].mass + Vector3.left * 0.5f).normalized;
		//points [amountOfPointsPerRope - 1].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.right * 0.5f).normalized;

		// Rope 2 endpoints
		//points[amountOfPointsPerRope].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.left * 0.5f).normalized;
		//points [amountOfPointsPerRope * 2 - 1].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.right * 0.5f).normalized;
	}

	private void springForces() {
		for (int i = 0; i < totalPoints; ++i) {

			Point p = points [i];

			for(int j = 0; j < p.GetNeighours().Count; ++j) {
				Point n = p.GetNeighours() [j];

				Vector3 force = Vector3.zero;
				Vector3 distance = n.position - p.position;

				force = ropeStiffnes * (distance.magnitude - segmentLength) * (distance / distance.magnitude);
				force -= ropeDampening * (p.velocity - n.velocity);

				p.force += force;
				n.force -= force;

			}
		}
	}

	private void endPoints() {
		points [0].force = Vector3.zero;
		points [amountOfPointsPerRope].force = Vector3.zero;

		points [amountOfPointsPerRope - 1].force = Vector3.zero;
		points [(amountOfPointsPerRope * 2) - 1].force = Vector3.zero;
	}

	private void euler() {
		// Euler
		evaluate (1f);
		for (int i = 0; i < points.Count; ++i) {
			Point p = points [i];
			p.velocity += (timestep / p.mass) * p.force;
			p.position += timestep * p.velocity;
		}
	}

	public void oldEuler(List<Point> points) {
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
}
