using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Point : MonoBehaviour {

	private static Color COLOR = new Color (220f / 255f, 200f / 255f, 195f / 255f);

	public class Neighbour {
		public Point neighbour;

		public float restForce;
		public float maxForce;
		public Vector3 force = Vector3.zero;

		public bool broken = false;

		public Neighbour(Point p, float restF, float maxF)  {
			neighbour = p; 
			restForce = restF;
			maxForce = maxF;
		}
	}

	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 force = Vector3.zero;
	[System.NonSerialized]
	public float mass;

	public Vector3 statePos = Vector3.zero;
	public Vector3 stateVel = Vector3.zero;
	public IntegrateDataPoint integrate;

	private List<Neighbour> neighbours = new List<Neighbour>();

	public Vector3 showForce;
	public float showForceSize;

	private bool showPointForce = false;

	void Start() {
	}


	// Update is called once per frame
	void LateUpdate () {
		transform.position = position;
		
		// Show spring forces and graviy force vectors on Point named "Rope: Upper left 25"
		if (showPointForce) {
			if (name == "Rope: Upper left25") {
				foreach (Neighbour n in neighbours) {
					if (n.neighbour.name == "Rope: Upper left26") {
						Debug.DrawLine (position, position + n.force / 100, Color.red, 0.01f, true);
					}
				}
				Debug.DrawLine (position, position + Vector3.down * 9.82f / 10, Color.red, 0.01f, true);
			}
			if (name == "Rope: Upper left24") {
				foreach (Neighbour n in neighbours) {
					if (n.neighbour.name == "Rope: Upper left25") {
						Debug.DrawLine (n.neighbour.position, n.neighbour.position - n.force / 100, Color.red, 0.01f, true);
					}
				}
			}
			if (name == "Rope: Triangular X 4 8") {
				foreach (Neighbour n in neighbours) {
					if (n.neighbour.name == "Rope: Upper left25") {
						Debug.DrawLine (n.neighbour.position, n.neighbour.position - n.force / 100, Color.red, 0.01f, true);
					}
				}
			}
			if (name == "Rope: Triangular Y 4 0") {
				foreach (Neighbour n in neighbours) {
					if (n.neighbour.name == "Rope: Upper left25") {
						Debug.DrawLine (n.neighbour.position, n.neighbour.position - n.force / 100, Color.red, 0.01f, true);
					}
				}
			}
		} else {

			foreach (Neighbour n in neighbours) {
				Color c = COLOR;

				if (n.restForce != 0) {
					float r = (n.force.magnitude - n.restForce) / (n.maxForce - n.restForce);
					float g = -r;

					float param = 1f;
					c = new Color(r * param,g * param, 0.0f);

					showForce = n.force;
					showForceSize = showForce.magnitude;

					if (n.force.magnitude > n.maxForce && Time.fixedTime > 10f) {
						n.broken = true;
					}
				}
				
				Debug.DrawRay (position, n.neighbour.position-position, c, 0.01f);
			}
		}


		// Remove broken neighbours
		for (int i = neighbours.Count - 1; i >= 0; --i) {
			if (neighbours [i].broken) {
				neighbours.RemoveAt (i);
			}
		}
	}

	public void AddNeighbour(Point p, float restF, float maxF) {
		neighbours.Add (new Neighbour(p, restF, maxF));
	}

	public List<Neighbour> GetNeighbours() {
		return neighbours;
	}

	public void clearMovement() {
		velocity = Vector3.zero;
		force = Vector3.zero;
	}
}
