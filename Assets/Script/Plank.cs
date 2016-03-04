﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Plank : MonoBehaviour {

	public Point point1;
	public Point point2;
	public Point point3;
	public Point point4;

	private float length; // z
	private float width; // x
	private float height = 0.3f; // y

	// Y(t)
	public Vector3 position;
	private Matrix R = new Matrix(3, 3); // Local space rotation R(t)
	public Quaternion q;
	public Vector3 P = Vector3.zero; // Linear momentum // TODO Size
	public Vector3 L = Vector3.zero; // Angular momentum	L(t) = I(t)w(t)

	// d/dt Y(t)
	private Vector3 velocity = Vector3.zero;
	public Vector3 w = Vector3.zero; // Omega Angular Velocity
	public Vector3 force = Vector3.zero;
	public Vector3 torque = Vector3.zero; // dL(t) = torque

	// Intertia (Ibody)	// Invers is (I^-1body) 1/xij from normal inertia matrix
	private Matrix Ibody;
	private Matrix IbodyInv;
	private Matrix Iinv;
	public float mass = 100;
	// Center of mass = position


	public Vector3 statePos;
	public Quaternion stateQ;
	public Vector3 stateP;
	public Vector3 stateL;

	// Use this for initialization
	void Start () {
	
	}

	public void init(Point p1, Point p2, Point p3, Point p4, float widthT) {
		position = p1.position + (p3.position - p1.position) / 2 + (p2.position - p1.position) / 2;

		point1 = p1;
		point2 = p2;
		point3 = p3;
		point4 = p4;

		point1.mass = mass / 4;
		point2.mass = mass / 4;
		point3.mass = mass / 4;
		point4.mass = mass / 4;

		//length = width;
		point1.position.x = position.x - widthT / 2;
		point3.position.x = position.x - widthT / 2;
		point2.position.x = position.x + widthT / 2;
		point4.position.x = position.x + widthT / 2;

		width = (point2.position - point1.position).magnitude;
		length = (point3.position - point1.position).magnitude;
		transform.localScale = new Vector3(width, height, length);
		transform.position = position;

		// Calculate inertia
		Ibody = new Matrix (3, 3);
		Ibody [0, 0] = (height * height + length * length) * mass / 12;
		Ibody [1, 1] = (width * width + length * length) * mass / 12;
		Ibody [2, 2] = (height * height + width * width) * mass / 12;
		IbodyInv = Ibody.Invert ();

		q = transform.rotation;
		calculateR();
		calculateIinv();

		Debug.DrawRay (point1.position, Vector3.up * 1, Color.green);
		Debug.DrawRay (point2.position, Vector3.up * 3, Color.red);
		Debug.DrawRay (point3.position, Vector3.up * 5, Color.blue);
		Debug.DrawRay (point4.position, Vector3.up * 7, Color.yellow);
	}

	public void clearPointForces() {
		point1.force = Vector3.zero;
		point2.force = Vector3.zero;
		point3.force = Vector3.zero;
		point4.force = Vector3.zero;
	}

	// Update is called once per frame
	void LateUpdate () {
		transform.position = position;
		transform.rotation = q;
	}


	public void simulation() {
		float timestep = 1f / 60f;

		// Torque = Sum((pi - x) X Fi)
		Vector3 t1 = Vector3.Cross ((point1.position - position), point1.force);
		Vector3 t2 = Vector3.Cross ((point2.position - position), point2.force);
		Vector3 t3 = Vector3.Cross ((point3.position - position), point3.force);
		Vector3 t4 = Vector3.Cross ((point4.position - position), point4.force);
		torque = t1 + t2 + t3 + t4;

		// Force Sum(Fi)
		force = point1.force + point2.force + point3.force + point4.force + Vector3.down * 9.82f * mass;
		
		// P += force * timestep;

		// L += torque * timestep;

		calculateW();
		
		// position = position + timestep * P / mass;
		velocity = P / mass;
		// point1.position += (velocity + Vector3.Cross (w, (point1.position - position))) * timestep;
		// point2.position += (velocity + Vector3.Cross (w, (point2.position - position))) * timestep;
		// point3.position += (velocity + Vector3.Cross (w, (point3.position - position))) * timestep;
		// point4.position += (velocity + Vector3.Cross (w, (point4.position - position))) * timestep;

		// Quaternion dq = (new Quaternion (w.x * 1/2, w.y * 1/2, w.z * 1/2, 0)) * q; // 1/2 [0 ; w(t)] q(t)
		// q.w += dq.w * timestep;
		// q.x += dq.x * timestep;
		// q.y += dq.y * timestep;
		// q.z += dq.z * timestep;
	
		calculateR();
		calculateIinv();
		normalizeQuaternion();

		clearPointForces ();

		// Y(t) ####################
		// x(t) position
		// q(t) quaternion
		// P(t) linearmomentum
		// L(t) angularMomentum-  { -X -Y -Z }			= Iw		dL = torque

		// dY(t) ##################
		// v(t)
		// w(t) * R(t)
		// F(t)
		// torque
	}

	private void calculateR() {

		// get R from q
		R[0,0] = (1 - 2 * q.y * q.y - 2 * q.z*q.z); R[0,1] = 2 * q.x * q.y - 2 * q.w * q.z; R[0,2] = 2 * q.x * q.z + 2 * q.w * q.y;
		R[1,0] = 2 * q.x * q.y + 2 * q.w * q.z; R[1,1] = (1 - 2 * q.x * q.x - 2 * q.z*q.z); R[1,2] = 2 * q.y * q.z - 2 * q.w * q.x;
		R[2,0] = 2 * q.x * q.z - 2 * q.w * q.y; R[2,1] = 2 * q.y * q.z + 2 * q.w * q.x; R[2,2] = (1 - 2 * q.x * q.x - 2 * q.y*q.y);
	}

	private void calculateIinv() {
		// Calculate I(t)	
		// I(t) = R(t) Ibody R(t)^T
		// I(t)^-1 = R(t) I^-1body R(t)^T
		//Matrix I = R * Ibody * Matrix.Transpose (R);
		Iinv = R * IbodyInv * Matrix.Transpose(R);
	}

	private void calculateW() {
		// Calculate w(t) = I(t)^-1 L(t)
		Matrix Ltmp = new Matrix(3,1);
		Ltmp [0, 0] = L.x;
		Ltmp [1, 0] = L.y;
		Ltmp [2, 0] = L.z;
		Matrix wMatrix = Iinv * Ltmp; 
		w = new Vector3 (wMatrix [0, 0], wMatrix [1, 0], wMatrix [2, 0]);
	}

	private void normalizeQuaternion() {
		float norm = Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y*q.y + q.z * q.z);
		q.w = q.w / norm;
		q.x = q.x / norm;
		q.y = q.y / norm;
		q.z = q.z / norm;
	}

	public void clearMovement() {
		P = Vector3.zero;
		L = Vector3.zero;
		w = Vector3.zero;
		torque = Vector3.zero;
		force = Vector3.zero;
	}
}

// TODO http://www.cc.gatech.edu/classes/AY2012/cs4496_spring/slides/RigidSim.pdf p 53
// http://www.cs.cmu.edu/~baraff/sigcourse/notesd1.pdf
// http://web.engr.illinois.edu/~yyz/teaching/cs598lect/RigidBodyImpl.pdf
// http://ocw.mit.edu/courses/aeronautics-and-astronautics/16-07-dynamics-fall-2009/lecture-notes/MIT16_07F09_Lec25.pdf
// http://www.cs.unc.edu/~lin/COMP768-F07/LEC/rbd1.pdf

// Visualisation of torque https://www.youtube.com/watch?v=Sm4pV3xyJRE
// Quaternion http://www.math.unm.edu/~vageli/papers/rrr.pdf
/*

//I_h = (m / 12) * (w^2+d^2)
//I_w = (m / 12) * (d^2+h^2)
//I_d = (m / 12) * (w^2+h^2)
		  
// When we have the force
// http://stackoverflow.com/questions/28922969/how-to-add-torque-to-a-rigidbody-until-its-angular-velocity-is-0
*/

/*

Crash at 
1425
1424
1424
1426

*/