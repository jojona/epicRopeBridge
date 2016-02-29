using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Plank : MonoBehaviour {

	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 force = Vector3.zero;

	public Point point1;
	public Point point2;
	public Point point3;
	public Point point4;


	public float mass = 1;

	public float length;
	public float width;
	public float height;

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
		transform.localScale = new Vector3(length, 0.3f, width);
		transform.position = position;

		// TODO rotate
	}
	
	// Update is called once per frame
	void Update () {
		// TODO
	}
}
