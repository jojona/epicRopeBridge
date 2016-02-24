using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rope : MonoBehaviour {

	public Transform anchorPointStart;
	public Transform anchorPointEnd;

	public int amount;

	private List<Point> points;
	private RK4 rk4;

	public Mesh mesh;

	// Use this for initialization
	void Start () {

		points = new List<Point> ();
		rk4 = new RK4 ();

		Vector3 length = anchorPointStart.position - anchorPointEnd.position;

		// Create ropepoints
		for (int i = 0; i < amount; ++i) {
			Point p = new Point (anchorPointStart.position + i * length / amount, 1);

			points.Add (p);
		}

		for (int i = 0; i < amount - 1; ++i) {
			points [i].addNeighbour (points [i + 1]);
		}


		// Generate Mesh

		generateMesh ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		// Clear forces

		for (int i = 0; i < amount; ++i) {
			points [i].ResetForce ();
		}


		// Update simulation

		rk4.Update (points);

		// Generate mesh
		generateMesh();
	}


	public void generateMesh() {




	}
}
