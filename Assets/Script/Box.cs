using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Box : MonoBehaviour {

	public Vector3 p1;
	public Vector3 force1;
	public Vector3 p2;
	public Vector3 force2;

	private float length; // z
	private float width; // x
	private float height = 3; // y

	// Y(t)
	public Vector3 position;
	private Matrix R = new Matrix(3, 3); // Local space rotation R(t)
	private Quaternion q;
	private Vector3 P = Vector3.zero; // Linear momentum // TODO Size
	private Vector3 L = Vector3.zero; // Angular momentum	L(t) = I(t)w(t)

	// d/dt Y(t)
	private Vector3 velocity = Vector3.zero;
	private Vector3 w = Vector3.zero; // Omega Angular Velocity
	public Vector3 force = Vector3.zero;
	public Vector3 torque = Vector3.zero; // dL(t) = torque


	// Intertia (Ibody)	// Invers is (I^-1body) 1/xij from normal inertia matrix
	public Matrix Ibody;
	public Matrix IbodyInv;
	public float mass = 100;
	// Center of mass = position

	// Use this for initialization
	void Start () {
	
	}

	public void init() {

		length = 3;
		width = 3;
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

		p1 = new Vector3(3, 0, 0);
		force1 = new Vector3(0, 0, 0);
		p2 = new Vector3(-3, 0, 0);
		force2 = new Vector3(0, 0, 0);

	}

	// Update is called once per frame
	void LateUpdate () {
		transform.position = position;

		transform.rotation = q;
	}


	public void simulation() {
		float timestep = 1f / 60f;

		// Calculate torque and force
		// Force
		force = force1 + force2;
		// Torque = Sum((pi - x) X Fi)
		Vector3 t1 = Vector3.Cross ((p1 - position), force1);
		Vector3 t2 = Vector3.Cross ((p2 - position), force2);
		torque = t1 + t2;

		// get R from q
		R[0,0] = (1 - 2 * q.y * q.y - q.z*q.z); R[0,1] = 2 * q.x * q.y - 2 * q.w * q.z; R[0,2] = 2 * q.x * q.z + 2 * q.w * q.y;
		R[1,0] = 2 * q.x * q.y + 2 * q.w * q.z; R[0,1] = (1 - 2 * q.x * q.x - q.z*q.z); R[1,2] = 2 * q.y * q.z - 2 * q.w * q.x;
		R[2,0] = 2 * q.x * q.z - 2 * q.w * q.y; R[2,1] = 2 * q.y * q.z + 2 * q.w * q.x; R[2,2] = (1 - 2 * q.x * q.x - q.y*q.y);

		// Calculate I(t)	
		// I(t) = R(t) Ibody R(t)^T
		// I(t)^-1 = R(t) I^-1body R(t)^T
		//Matrix I = R * Ibody * Matrix.Transpose (R);
		Matrix IInv = R * IbodyInv * Matrix.Transpose(R);

		// Calculate w(t) = I(t)^-1 L(t)
		Matrix Ltmp = new Matrix(3,1);
		Ltmp [0, 0] = L.x;
		Ltmp [1, 0] = L.y;
		Ltmp [2, 0] = L.z;
		Matrix wMatrix = IInv * Ltmp; 
		w = new Vector3 (wMatrix [0, 0], wMatrix [1, 0], wMatrix [2, 0]);

		// Y(t) ####################
		// x(t) position
		// R(t) rotation { x y z }
		// P(t) velocity
		// L(t) angularMomentum-  { -X -Y -Z }			= Iw		dL = torque

		// dY(t) ##################
		// v(t)
		// w(t) * R(t)
		// F(t)
		// torque

		Matrix wStar = new Matrix (3, 3);
		wStar [0, 0] = 0; 	wStar [0, 1] = -w.z;wStar [0, 2] = w.y;
		wStar [1, 0] = w.z; wStar [1, 1] = 0; 	wStar [1, 2] = -w.x;
		wStar [2, 0] = -w.y;wStar [2, 1] = w.x; wStar [2, 2] = 0;

		Matrix dR = wStar * R;

		Quaternion dq = (new Quaternion (0, w.x * timestep * 1/2, w.y * timestep * 1/2, w.z * timestep * 1/2)) * q; // 1/2 [0 ; w(t)] q(t)

		q.w += dq.w;
		q.x += dq.x;
		q.y += dq.y;
		q.z += dq.z;

		float norm = Mathf.Sqrt(q.w*q.w + q.x * q.x + q.y*q.y + q.z*q.z);
		q.w = q.w / norm;
		q.x = q.x / norm;
		q.y = q.y / norm;
		q.z = q.z / norm;

		// Calculate P and L
		P += force * timestep;
		L += torque * timestep;

		// Calculate R, v, Iinv, w
		velocity = P / mass;

		position += timestep * velocity;

	}
}