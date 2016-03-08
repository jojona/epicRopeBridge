using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Point : MonoBehaviour {

	private static Color COLOR = new Color (220f / 255f, 200f / 255f, 195f / 255f);

	private struct Neighbour {
		public Point neighbour;
		public float segmentLength;
		public float breakLength;
		public float restLength;

		public Neighbour(Point p, float s, float restKonstant, float breakkonstant)  {
			neighbour = p; 
			segmentLength = s;
			restLength = restKonstant;
			breakLength = breakkonstant;
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
			Color c = COLOR;

			if (n.restLength != 0) {

				float r = ((position - n.neighbour.position).magnitude - n.restLength * n.segmentLength) / (n.breakLength * n.segmentLength - n.segmentLength);
				float g = -r;

				float param = 1f;
				c = new Color(r * param,g * param, 0.0f);


				//Debug.Log(255f / 0.5f*(Mathf.Abs((position - n.neighbour.position).magnitude) - n.segmentLength));
			}
			
			Debug.DrawRay (position, n.neighbour.position-position, c, 0.01f);


		}
	}

	public void AddNeigbour(Point p, float segmentLength, float restK, float breakK) {
		neighbour.Add (new Neighbour(p, segmentLength, restK, breakK));
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
