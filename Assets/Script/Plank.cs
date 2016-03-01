using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Plank : MonoBehaviour {

	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 force = Vector3.zero;

	private Point point1;
	private Point point2;
	private Point point3;
	private Point point4;

	public float mass = 1;

	private float length;
	private float width;
	private float height = 0.3f;

	// Use this for initialization
	void Start () {
	
	}

	public void init(Point p1, Point p2, Point p3, Point p4) {
		point1 = p1;
		point2 = p2;
		point3 = p3;
		point4 = p4;

		length = (point1.position - point2.position).magnitude * 2;
		width = (point3.position - point4.position).magnitude * 2;
		transform.localScale = new Vector3(length, height, width);
		transform.position = position;

		// Diagonal = (position - point1.position).magnitude

		// TODO rotate
	}

	public void simulation() {
		Vector3 force = point1.force + point2.force + point3.force + point4.force;

		Vector3 r = Vector3.Cross(point1.force, (point1.position - position));

		Debug.DrawRay (position, r, Color.green, 1f, true);
		Debug.DrawRay (point1.position, point1.force, Color.red, 1f, true);
		Debug.DrawRay (position, point1.force, Color.red, 1f, true);
		Debug.DrawRay (position, point1.position - position, Color.yellow, 1f, true);

		// Clear forces on points
	}
	
	// Update is called once per frame
	void Update () {
		// TODO
	}
}
