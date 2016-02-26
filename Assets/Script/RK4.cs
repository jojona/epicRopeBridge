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

	public RK4(List<Point> points, int appr, int tp, float rS, float rD, float sL){
		this.points = points;

		amountOfPointsPerRope = appr;
		totalPoints = tp;

		ropeStiffnes = rS;
		ropeDampening = rD;
		segmentLength = sL;

		Point p;
		for (int i = 0; i < points.Count; ++i) {
			p = points [i];
			p.dx = Vector3.zero;
			p.dv = Vector3.zero;
		}
	}


	public void RungeKutta4(){
		int amount = points.Count;
		Point p;

//		// Initialize derivative
//		for (int i = 0; i < amount; ++i) {
//			p = points [i];
//			p.dx = Vector3.zero;
//			p.dv = Vector3.zero;
//		}

		//K1
		evaluate(timestep*0f, true);
		for (int i = 0; i < amount; ++i) {
			p = points [i];

			//Calculating RK4
			p.aPos = p.dx;
			p.aVel = p.dv;
		}

		//K2
		evaluate(timestep*0.5f, true);
		for (int i = 0; i < amount; ++i) {
			p = points [i];
			p.bPos = p.dx;
			p.bVel = p.dv;
		}

		//K3
		evaluate(timestep*0.5f, true);
		for (int i = 0; i < amount; ++i) {
			p = points [i];
			p.cPos = p.dx;
			p.cVel = p.dv;
		}

		//K4
		evaluate(timestep*1f, true);
		for (int i = 0; i < amount; ++i) {
			p = points [i];
			p.dPos = p.dx;
			p.dVel = p.dv;

			// Update final pos & vel
			p.dx = (1f/6f)*(p.aPos + 2f*(p.bPos + p.cPos) + p.dPos);
			p.dv = (1f/6f)*(p.aVel + 2f*(p.bVel + p.cVel) + p.dVel);
			p.position += p.dx;
			p.velocity += p.dv;
		}

	}

	void evaluate(float dt, bool isRK4) {
		clearForces ();
		gravity ();

		Point p;
		for (int i = 0; i < points.Count && isRK4; ++i) {
			p = points [i];
			p.statePos = p.position + p.dx * dt;
			p.stateVel = p.velocity + p.dv * dt;
		}


		springForces (isRK4);
		endPoints ();
		for (int i = 0; i < points.Count && isRK4; ++i) {
			p = points [i];
			p.dx = p.stateVel;
			p.dv = p.stateForce / p.mass; //TODO fixa p.force + p.stateForce
		}

	}

	private void clearForces() {
		for (int i = 0; i < totalPoints; ++i) {
			points [i].force = Vector3.zero;
			points [i].stateForce = Vector3.zero;
		}
	}

	private void gravity() {
		for (int i = 0; i < totalPoints; ++i) {
			points [i].force += Vector3.down * 9.81f * points[i].mass;
			points [i].stateForce += Vector3.down * 9.81f * points[i].mass;
		}

		// Rope 1 endpoints
		//points [0].force += (Random.onUnitSphere * points [0].mass + Vector3.left * 0.5f).normalized;
		//points [amountOfPointsPerRope - 1].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.right * 0.5f).normalized;

		// Rope 2 endpoints
		//points[amountOfPointsPerRope].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.left * 0.5f).normalized;
		//points [amountOfPointsPerRope * 2 - 1].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.right * 0.5f).normalized;
	}

	private void springForces(bool isRK4) {
		for (int i = 0; i < totalPoints; ++i) {

			Point p = points [i];

			for(int j = 0; j < p.GetNeighours().Count; ++j) {
				Point n = p.GetNeighours() [j];

				Vector3 force = Vector3.zero;
				Vector3 distance;
				if (isRK4) {
					distance = n.statePos - p.statePos;
				} else {
					distance = n.position - p.position;
				}


				force = ropeStiffnes * (distance.magnitude - segmentLength) * (distance / distance.magnitude);
				if (isRK4) {
					force -= ropeDampening * (p.stateVel - n.stateVel);
				} else {
					force -= ropeDampening * (p.velocity - n.velocity);
				}

				if (isRK4) {
					p.stateForce += force;
					n.stateForce -= force;
				} else {
					p.force += force;
					n.force -= force;
				}


			}
		}
	}

	private void endPoints() {
		points [0].force = Vector3.zero;
		points [amountOfPointsPerRope].force = Vector3.zero;

		points [amountOfPointsPerRope - 1].force = Vector3.zero;
		points [(amountOfPointsPerRope * 2) - 1].force = Vector3.zero;

		points [0].stateForce = Vector3.zero;
		points [amountOfPointsPerRope].stateForce = Vector3.zero;

		points [amountOfPointsPerRope - 1].stateForce = Vector3.zero;
		points [(amountOfPointsPerRope * 2) - 1].stateForce = Vector3.zero;
	}

	public void euler() {
		// Euler
		evaluate (1f, false);
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
