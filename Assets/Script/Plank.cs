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

	private float length;
	private float width;
	private float height = 0.3f;

	// X
	public Vector3 position = Vector3.zero;
	public Matrix R = new Matrix(3, 3); // Local space rotation R(t)
	public Quaternion q;
	public Vector3 P = Vector3.zero; // Linear momentum // TODO Size
	public Vector3 L = Vector3.zero; // Angular momentum	L(t) = I(t)w(t)

	// Dx
	public Vector3 velocity = Vector3.zero;
	public Vector3 w = Vector3.zero; // Omega Angular Velocity
	public Vector3 force = Vector3.zero;
	public Vector3 torque = Vector3.zero; // dL(t) = torque

	public float mass = 100;
	// Center of mass = position

	// Intertia (Ibody)	// Invers is (I^-1body) 1/xij from normal inertia matrix
	public Matrix Ibody;
	public Matrix IbodyInv;

	// Use this for initialization
	void Start () {
	
	}

	public void init(Point p1, Point p2, Point p3, Point p4) {
		point1 = p1;
		point2 = p2;
		point3 = p3;
		point4 = p4;

		point1.mass = mass;
		point2.mass = mass;
		point3.mass = mass;
		point4.mass = mass;

		length = (point1.position - point2.position).magnitude * 2;
		width = (point3.position - point4.position).magnitude * 2;
		transform.localScale = new Vector3(length, height, width);
		transform.position = position;

		// Calculate intertia

		Ibody = new Matrix (3, 3);
		Ibody [0, 0] = height * height * length * length * mass / 12;
		Ibody [1, 1] = width * width * length * length * mass / 12;
		Ibody [2, 2] = height * height * width * width * mass / 12;
		IbodyInv = Ibody.Invert ();

		R [0, 0] = 1; R [0, 1] = 0; R [0, 2] = 0;
		R [1, 0] = 0; R [1, 1] = 1; R [1, 2] = 0;
		R [2, 0] = 0; R [2, 1] = 0; R [2, 2] = 1;
		q = transform.rotation;

	}

	public void simulationxxx() {
		float timestep = 1f / 60f;
		force = point1.force + point2.force + point3.force + point4.force + Vector3.down * 9.82f * mass;

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


		// Apply rotation matrix R

		//Debug.Log (R.ToString ());
		//transform.right += new Vector3(R[0, 0], R[1, 0], R[2, 0]); 
		//transform.up += new Vector3(R[0, 1], R[1, 1], R[2, 1]);
		//transform.forward += new Vector3(R[0, 2], R[1, 2], R[2, 2]);

		//Debug.Log (q);
		transform.rotation = q;
	}


	public void simulation() {
		float timestep = 1f / 60f;

		// TODO http://www.cc.gatech.edu/classes/AY2012/cs4496_spring/slides/RigidSim.pdf p 53

		// Calculate torque and force

		torque = Vector3.zero;

		// Force
		force = point1.force + point2.force + point3.force + point4.force + Vector3.down * 9.82f * mass;

		// Torque = Sum((pi - x) X Fi)
		Vector3 t1 = Vector3.Cross ((point1.position - position), point1.force);
		Vector3 t2 = Vector3.Cross ((point1.position - position), point2.force);
		Vector3 t3 = Vector3.Cross ((point1.position - position), point3.force);
		Vector3 t4 = Vector3.Cross ((point1.position - position), point4.force);
		torque += t1 + t2 + t3 + t4;


		// Y(t) ####################
		// x(t) position
		// R(t) rotation { x y z }
		// P(t) velocity
		// L(t) angularMomentum-  { -X -Y -Z }			= Iw		dL = torque

		// Calculate I(t)	
			// I(t) = R(t) Ibody R(t)^T
			// I(t)^-1 R(t) I^-1body R(t)^T
		Matrix I = R * Ibody * Matrix.Transpose (R);
		Matrix IInv = R * IbodyInv * Matrix.Transpose(R);

		// dY(t) ##################
		// v(t)
		// w(t) * R(t)
		// F(t)
		// torque

		velocity += timestep * force / mass;

		// Calculate w(t) = I(t)^-1 L(t)
		Matrix Ltmp = new Matrix(3,1);
		Ltmp [0, 0] = L.x;
		Ltmp [1, 0] = L.y;
		Ltmp [2, 0] = L.z;
		Matrix wMatrix = IInv * Ltmp; 
		Vector3 w = new Vector3 (wMatrix [0, 0], wMatrix [1, 0], wMatrix [1, 0]);

		Matrix wStar = new Matrix (3, 3);
		wStar [0, 0] = 0; 	wStar [0, 1] = -w.z;wStar [0, 2] = w.y;
		wStar [1, 0] = w.z; wStar [1, 1] = 0; 	wStar [1, 2] = -w.x;
		wStar [2, 0] = -w.y;wStar [2, 1] = w.x; wStar [2, 2] = 0;

		Matrix dR = wStar * R;
		/* dR (Inefficient)
		Vector3 drx = Vector3.Cross(w, new Vector3(R[0, 0], R[1, 0], R[2, 0]));
		Vector3 dry = Vector3.Cross(w, new Vector3(R[0, 1], R[1, 1], R[2, 1]));
		Vector3 drz = Vector3.Cross(w, new Vector3(R[0, 2], R[1, 2], R[2, 2]));
		dR [0, 0] = drx [0]; dR [0, 1] = drx [1]; dR [0, 2] = drx [2];
		dR [1, 0] = dry [0]; dR [1, 1] = dry [1]; dR [1, 2] = dry [2];
		dR [2, 0] = drz [0]; dR [2, 1] = drz [1]; dR [2, 2] = drz [2];*/

		position += timestep * velocity;

		R += timestep * dR;
		q = q * (new Quaternion (0, w.x * timestep * 1/2, w.y * timestep * 1/2, w.z * timestep * 1/2)); // 1/2 [0 ; w(t)] q(t)

		P += force * timestep;
		L += torque * timestep;

		point1.position += timestep * velocity;
		point2.position += timestep * velocity;
		point3.position += timestep * velocity;
		point4.position += timestep * velocity;
		clearPointForces ();

		// Point velocity
		/*
		point1.velocity = Vector3.Cross (w, (point1.position - position)) * timestep + velocity;
		point2.velocity = Vector3.Cross (w, (point2.position - position)) * timestep+ velocity;
		point3.velocity = Vector3.Cross (w, (point3.position - position)) * timestep+ velocity;
		point4.velocity = Vector3.Cross (w, (point4.position - position)) * timestep+ velocity;
		*/
	}
}

// http://www.cs.cmu.edu/~baraff/sigcourse/notesd1.pdf

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