using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Point : MonoBehaviour {

	private static Color COLOR = new Color (220f / 255f, 200f / 255f, 195f / 255f);

	public class Neighbour {
		public Point neighbour;
		public float segmentLength;

		public float restForce;
		public float maxForce;
		public Vector3 force = Vector3.zero;

		public bool broken = false;

		public Neighbour(Point p, float s, float restF, float maxF)  {
			neighbour = p; 
			segmentLength = s;
			restForce = restF;
			maxForce = maxF;
		}
	}

	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 force = Vector3.zero;
	public float mass = 10000f;

	public Vector3 statePos = Vector3.zero;
	public Vector3 stateVel = Vector3.zero;
	public IntegrateDataPoint integrate;

	private List<Neighbour> neighbours = new List<Neighbour>();

	public Vector3 showForce;
	public float showForceSize;

	void Start() {
	}


	// Update is called once per frame
	void LateUpdate () {
		transform.position = position;
		foreach (Neighbour n in neighbours) {
			Color c = COLOR;

			if (n.restForce != 0) {

				//float r = ((position - n.neighbour.position).magnitude - n.restForce * n.segmentLength) / (n.maxForce * n.segmentLength - n.segmentLength); // TODO fix
				float r = (n.force.magnitude - n.restForce) / (n.maxForce - n.restForce);
				float g = -r;

				float param = 1f;
				c = new Color(r * param,g * param, 0.0f);

				showForce = n.force;
				showForceSize = showForce.magnitude;

				if (n.force.magnitude > n.maxForce && Time.fixedTime > 10f) {
					Debug.Log ("Break " + name + " | " + n.neighbour.name + " | Force=" + n.force.magnitude + " Max" + n.maxForce );
					n.broken = true;
				}

				//Debug.Log(255f / 0.5f*(Mathf.Abs((position - n.neighbour.position).magnitude) - n.segmentLength));
			}
			
			Debug.DrawRay (position, n.neighbour.position-position, c, 0.01f);
		}


		// Remove broken neighbours
		for (int i = neighbours.Count - 1; i >= 0; --i) {
			if (neighbours [i].broken) {
				neighbours.RemoveAt (i);
			}
		}
	}

	public void AddNeighbour(Point p, float segmentLength, float restF, float maxF) {
		neighbours.Add (new Neighbour(p, segmentLength, restF, maxF));
	}

	public List<Neighbour> GetNeighbours() {
		return neighbours;
	}

	public void clearMovement() {
		velocity = Vector3.zero;
		force = Vector3.zero;
	}
}
