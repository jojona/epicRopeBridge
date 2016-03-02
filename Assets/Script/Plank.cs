using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Plank : MonoBehaviour {

	private Point point1;
	private Point point2;
	private Point point3;
	private Point point4;

	private float length; // z
	private float width; // x
	private float height = 0.3f; // y

	// Y(t)
	public Vector3 position;
	private Matrix R = new Matrix(3, 3); // Local space rotation R(t)
	private Quaternion q;
	private Vector3 P = Vector3.zero; // Linear momentum // TODO Size
	private Vector3 L = Vector3.zero; // Angular momentum	L(t) = I(t)w(t)

	// d/dt Y(t)
	private Vector3 velocity = Vector3.zero;
	private Vector3 w = Vector3.zero; // Omega Angular Velocity
	private Vector3 force = Vector3.zero;
	private Vector3 torque = Vector3.zero; // dL(t) = torque
	private Vector3 rotation;

	// Intertia (Ibody)	// Invers is (I^-1body) 1/xij from normal inertia matrix
	public Matrix Ibody;
	public Matrix IbodyInv;
	public float mass = 100;
	// Center of mass = position

	// Use this for initialization
	void Start () {
	
	}

	public void init(Point p1, Point p2, Point p3, Point p4) {
		rotation = Vector3.zero;

		point1 = p1;
		point2 = p2;
		point3 = p3;
		point4 = p4;

		point1.mass = mass / 4;
		point2.mass = mass / 4;
		point3.mass = mass / 4;
		point4.mass = mass / 4;

		length = 2 * (point1.position - point2.position).magnitude;
		width = (point3.position - point1.position).magnitude;
		transform.localScale = new Vector3(width, height, length);
		transform.position = position;

		// Calculate intertia

		Ibody = new Matrix (3, 3);
		Ibody [0, 0] = (height * height + length * length) * mass / 12;
		Ibody [1, 1] = (width * width + length * length) * mass / 12;
		Ibody [2, 2] = (height * height + width * width) * mass / 12;
		IbodyInv = Ibody.Invert ();

		R [0, 0] = 1; R [0, 1] = 0; R [0, 2] = 0;
		R [1, 0] = 0; R [1, 1] = 1; R [1, 2] = 0;
		R [2, 0] = 0; R [2, 1] = 0; R [2, 2] = 1;
		q = transform.rotation;

	}

	public void simulationxxx() {
		float timestep = 1f / 60f;
		force = point1.force + point2.force + point3.force + point4.force + Vector3.down * 9.82f * mass * 4;

		velocity += timestep * force / mass;
		position += timestep * velocity;

		point1.position += timestep * velocity;
		point2.position += timestep * velocity;
		point3.position += timestep * velocity;
		point4.position += timestep * velocity;
		// Clear forces on points
		clearPointForces();
	}

	public void simulation2() {
		
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


		// Apply rotation matrix R

		//Debug.Log (R.ToString ());
		//transform.right += new Vector3(R[0, 0], R[1, 0], R[2, 0]); 
		//transform.up += new Vector3(R[0, 1], R[1, 1], R[2, 1]);
		//transform.forward += new Vector3(R[0, 2], R[1, 2], R[2, 2]);

		//Debug.Log (q);
	}


	public void simulation() {
		float timestep = 1f / 60f;
		double halfDia = Math.Sqrt (height * height + width * width) / 2;

		// Force
		force = point1.force + point2.force + point3.force + point4.force + Vector3.down * 9.82f * mass;
		/*
		// Moment on every point
		double momentX1 = point1.force.x * halfDia;
		double momentX2 = point2.force.x * halfDia;
		double momentX3 = point3.force.x * halfDia;
		double momentX4 = point4.force.x * halfDia;
		double momentY1 = point1.force.y * halfDia;
		double momentY2 = point2.force.y * halfDia;
		double momentY3 = point3.force.y * halfDia;
		double momentY4 = point4.force.y * halfDia;
		double momentZ1 = point1.force.z * halfDia;
		double momentZ2 = point2.force.z * halfDia;
		double momentZ3 = point3.force.z * halfDia;
		double momentZ4 = point4.force.z * halfDia;

		float momentZ = (float) (momentZ2 + momentZ4 - momentZ1 - momentZ3);
		float momentY = (float) (momentY1 - momentY2 - momentY3 + momentY4);
		float momentX = (float) (momentX3 + momentX4 - momentX1 - momentX2);

		q.x += momentX;
		q.y += momentY;
		q.z += momentZ;
		*/

		// Calculate R, v, Iinv, w
		velocity += timestep * force / mass;

		position += timestep * velocity;

		// Point velocity
		point1.position += timestep * velocity;
		//point1.position.x += Math.Sin(momentX) * width;
		//point1.position.y += Math.Sin(momentY) * width;
		//point1.position.y += (float) Math.Sin(momentZ) * width;
		point2.position += timestep * velocity;
		point3.position += timestep * velocity;
		point4.position += timestep * velocity;

		//Debug.Log ("New velocity " + Vector3.Cross (w, (point1.position - position)) + " Old vel " + velocity);

		/*
		point1.position += (velocity + Vector3.Cross (w, (point1.position - position))) * timestep;
		point2.position += (velocity + Vector3.Cross (w, (point2.position - position))) * timestep;
		point3.position += (velocity + Vector3.Cross (w, (point3.position - position))) * timestep;
		point4.position += (velocity + Vector3.Cross (w, (point4.position - position))) * timestep;
		*/

		clearPointForces ();
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
		// Angular properties
angularVelocity // Scalar in units of radians per timestep (or seconds) (omega)
angularAcceleration // (alpha)
rotationalForce // (Torque)
momentOfInertia // I

//I_h = (m / 12) * (w^2+d^2)
//I_w = (m / 12) * (d^2+h^2)
//I_d = (m / 12) * (w^2+h^2)
		 

// torque = I * angularAcceleration

//angularacc = sum (torqueVector) / I; 

// When we have the force
// http://stackoverflow.com/questions/28922969/how-to-add-torque-to-a-rigidbody-until-its-angular-velocity-is-0
*/

/*
// Center of mass
Vector3 centerofMass = position;

// Point of application
Vector3 momentArm1 =  point1.position - centerofMass;
Vector3 torqueVector1 = (point1.force - (momentArm1 * (Vector3.Dot(point1.force, momentArm1) / momentArm1.magnitude))) * momentArm1.magnitude;

Vector3 momentArm2 =  point2.position - centerofMass;
Vector3 torqueVector2 = (point2.force - (momentArm2 * (Vector3.Dot(point2.force, momentArm2) / momentArm2.magnitude))) * momentArm2.magnitude;

Vector3 momentArm3 =  point3.position - centerofMass;
Vector3 torqueVector3 = (point3.force - (momentArm3 * (Vector3.Dot(point3.force, momentArm3) / momentArm3.magnitude))) * momentArm3.magnitude;

Vector3 momentArm4 =  point4.position - centerofMass;
Vector3 torqueVector4 = (point4.force - (momentArm4 * (Vector3.Dot(point4.force, momentArm4) / momentArm4.magnitude))) * momentArm4.magnitude;

float inertia = 1;

torqueVector1 = torqueVector1 / inertia;
torqueVector2 = torqueVector2 / inertia;
torqueVector3 = torqueVector3 / inertia;
torqueVector4 = torqueVector4 / inertia;

Vector3 torque = torqueVector1 + torqueVector2 + torqueVector3 + torqueVector4;

//transform.Rotate(torque * timestep);
Debug.DrawRay (position, torque , Color.green, 1f, true);
*/