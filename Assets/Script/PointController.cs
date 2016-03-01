using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class PointController : MonoBehaviour {

	public Point pointPrefab;
	protected List<Point> points;

	protected float ropeStiffness;
	protected float ropeDampening;
	protected float segmentLength;

	public abstract void simulationStep ();

	/**
	 * Creates a point and add it to list of points 
	 * Creates the point at given position.
	 */
	protected Point createPoint(Vector3 position) {
		Point p = (Point)Instantiate (pointPrefab, position, Quaternion.identity);
		p.position = position;
		p.transform.parent = transform;
		points.Add (p);
		return p;
	}

	/**
	 * Creates a point and add it to list of points 
	 * Create the point at given position with a name.
	 */
	protected Point createPoint(Vector3 position, string name) {
		Point p = createPoint (position);
		p.name = name;
		return p;
	}

	protected void init(float stiffness, float dampening, float segmentLength) {
		ropeStiffness = stiffness;
		ropeDampening = dampening;
		this.segmentLength = segmentLength;
		points = new List<Point> ();
	}

	/** 
	 * Clears old forces and sets all to zero.
	 */
	public void clearForces() {
		for (int i = 0; i < points.Count; ++i) {
			points [i].force = Vector3.zero;
		}
	}

	/**
	 * Applies gravity to all points.
	 */
	protected void gravity() {
		for (int i = 0; i < points.Count; ++i) {
			points[i].force += Vector3.down * 9.82f * points[i].mass;
		}
	}

	/**
	 * Applies spring forces to all points by walking through all points and making them affect their neighbours.
	 */
	protected void springForces() {

		for (int i = 0; i < points.Count; ++i) {
			Point p = points[i];
			List<Point> neighbours = p.GetNeighours();
			foreach (Point n in neighbours) {
				springforce (p, n);
			}
		}
	}

	/**
	 * Applies spring forces to two points p and n.
	 */
	protected void springforce(Point point, Point neighbour) {
		Vector3 force = Vector3.zero;
		Vector3 distance = neighbour.position - point.position;

		force = ropeStiffness * (distance.magnitude - segmentLength) * (distance / distance.magnitude);
		force -= ropeDampening * (point.velocity - neighbour.velocity);

		point.force += force;
		neighbour.force -= force;
	}

	public List<Point> getPoints() {
		return points;
	}
}
