using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Point : MonoBehaviour {

	private struct Neighbour {
		public Point neighbour;
		public float segmentLength;
		public float breakLength;

		public Neighbour(Point p, float s)  {
			neighbour = p; 
			segmentLength = s;
			breakLength = s*1.4f;
		}

	}

	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 force = Vector3.zero;
	public float mass = 10000f;

	public Vector3 statePos = Vector3.zero;
	public Vector3 stateVel = Vector3.zero;
	public IntegrateDataPoint integrate;

	private List<Neighbour> neighbour = new List<Neighbour>();

	// Update is called once per frame
	void LateUpdate () {
		transform.position = position;
		foreach (Neighbour n in neighbour) {
			/*
			float g = n.segmentLength * 1.2f - (position - n.neighbour.position).magnitude;
			float r = -g;
			//*/
			float r = ((position - n.neighbour.position).magnitude - 1.2f * n.segmentLength) / (n.breakLength - n.segmentLength);
			float g = -r;

			float param = 1f;
			Color c = new Color(r * param,g * param, 0.0f);

			Debug.DrawRay (position, n.neighbour.position-position, c, 0.01f);

			//Debug.Log(255f / 0.5f*(Mathf.Abs((position - n.neighbour.position).magnitude) - n.segmentLength));

			// 0 - 0.5



		}
	}

	public void AddNeigbour(Point p, float segmentLength) {
		neighbour.Add (new Neighbour(p, segmentLength));
	}

	public IEnumerable<Point> GetNeighours() {
		foreach(Neighbour n in neighbour) {
			yield return n.neighbour;
		}
	}

	public void clearMovement() {
		velocity = Vector3.zero;
		force = Vector3.zero;
	}
}
