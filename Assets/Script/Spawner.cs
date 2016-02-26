using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour {

	public Point pointPrefab;

	public Vector3 anchorPointStart = Vector3.zero;
	public Vector3 anchorPointEnd = Vector3.zero + Vector3.right * 10;
	public int amountOfPointsPerRope = 10;
	public int totalPoints;

	private List<Point> points;
	private float segmentLength;


	public float ropeStiffnes = 800f;
	public float ropeDampening = 1f;

	// Use this for initialization
	void Start () {
		points = new List<Point> ();

		Vector3 position = anchorPointStart;
		Vector3 ropeDirection = (anchorPointEnd - anchorPointStart) / amountOfPointsPerRope;

		segmentLength = ropeDirection.magnitude;

		// Rep 1
		for (int i = 0; i < amountOfPointsPerRope; ++i) {
			points.Add(createPoint (position, "Rope1 " + i));
			position += ropeDirection;
		}
		for (int i = 0; i < amountOfPointsPerRope - 1; ++i) {
			//points [i].AddNeigbour(points [i + 1]);
		}
		for (int i = 1; i < amountOfPointsPerRope; ++i) {
			points [i].AddNeigbour (points [i - 1]);
		}

		position -= ropeDirection * amountOfPointsPerRope + Vector3.back * segmentLength * 3;

		// Rep 2
		for (int i = amountOfPointsPerRope; i < amountOfPointsPerRope * 2; ++i) {
			points.Add(createPoint (position, "Rope2 " + (i - amountOfPointsPerRope)));
			position += ropeDirection;
		}
		for (int i = amountOfPointsPerRope; i < amountOfPointsPerRope * 2 - 1; ++i) {
			//points [i].AddNeigbour(points [i + 1]);
		}
		for (int i = amountOfPointsPerRope + 1; i < amountOfPointsPerRope * 2; ++i) {
			points [i].AddNeigbour (points [i - 1]);
		}

		// Middle rope // No back pointers coded

		for (int i = 0; i < amountOfPointsPerRope / 2; ++i) {

			points.Add (createPoint ((anchorPointStart + ropeDirection * (2 * i)) + Vector3.forward * segmentLength, "Middle 1"));
			points.Add (createPoint ((anchorPointStart + ropeDirection * (2 * i)) + Vector3.forward * segmentLength * 2, "Middle 2"));
			
			points[amountOfPointsPerRope*2 + 2*i].AddNeigbour(points[2 * i]);
			points [amountOfPointsPerRope * 2 + 2 * i].AddNeigbour (points [amountOfPointsPerRope * 2 + 1 + 2 * i	]);
			points [amountOfPointsPerRope * 2 + 1 + 2*i].AddNeigbour (points[amountOfPointsPerRope + 2 * i]);
		}




		totalPoints = amountOfPointsPerRope * 3;
	}

	// Update is called once per frame
	void FixedUpdate () {

		for (int i = 0; i < amountOfPointsPerRope; ++i) {
			// Debug.Log ("");
		}

		simulationStep ();

		for (int i = 0; i < points.Count; ++i) {
			Point p = points [i];
			p.velocity += (Time.fixedDeltaTime / p.mass) * p.force;
			p.position += Time.fixedDeltaTime * p.velocity;
		}

	}


	void simulationStep() {
		clearForces ();
		gravity ();
		springForces ();
	}

	private void clearForces() {
		for (int i = 0; i < totalPoints; ++i) {
			points [i].force = Vector3.zero;
		}
	}

	private void gravity() {
		for (int i = 0; i < totalPoints; ++i) {
			//points [i].force += Vector3.down * 9.82f * points[i].mass;
		}

		// Rope 1 endpoints
		points [0].force += (Random.onUnitSphere * points [0].mass + Vector3.left * 0.5f).normalized;
		points [amountOfPointsPerRope - 1].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.right * 0.5f).normalized;


		// Rope 2 endpoints
		points[amountOfPointsPerRope].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.left * 0.5f).normalized;
		points [amountOfPointsPerRope * 2 - 1].force += (Random.onUnitSphere * points [amountOfPointsPerRope - 1].mass + Vector3.right * 0.5f).normalized;
	}

	private void springForces() {
		for (int i = 0; i < totalPoints; ++i) {

			Point p = points [i];

			for(int j = 0; j < p.GetNeighours().Count; ++j) {
				Point n = p.GetNeighours() [j];

				Vector3 force = Vector3.zero;
				Vector3 distance = n.position - p.position;

				force = ropeStiffnes * (distance.magnitude - segmentLength) * (distance / distance.magnitude);
				force -= ropeDampening * (p.velocity - n.velocity);

				p.force += force;
				n.force -= force;




				//Debug.Log ("Force " + i + " " + force);
			}
		}
	}

	private Point createPoint(Vector3 position) {
		Point p = (Point)Instantiate (pointPrefab, position, Quaternion.identity);
		p.position = position;
		p.transform.parent = transform;
		return p;
	}

	private Point createPoint(Vector3 position, string name) {
		Point p = createPoint (position);
		p.name = name;
		return p;
	}
}
