using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class PointController : MonoBehaviour {

	public static InputHandler ih;

	public Point pointPrefab;
	protected List<Point> points;
	public List<IntegrateAbstract> integrateList = new List<IntegrateAbstract>();

	protected float ropeStiffness;
	protected float ropeDampening;
	public float segmentLength;

	public abstract void simulationStep ();
	public abstract void clearMovement();

	/**
	 * Creates a point and add it to list of points 
	 * Creates the point at given position.
	 */
	protected Point createPoint(Vector3 position, float mass) {
		Point p = (Point)Instantiate (pointPrefab, position, Quaternion.identity);
		p.position = position;
		p.transform.parent = transform;
		p.mass = mass;
		points.Add (p);
		IntegrateDataPoint ip = new IntegrateDataPoint(p);
		integrateList.Add(ip);
		p.integrate = ip;

		return p;
	}

	/**
	 * Creates a point and add it to list of points 
	 * Create the point at given position with a name.
	 */
	protected Point createPoint(Vector3 position, string name, float mass) {
		Point p = createPoint (position, 1f);
		p.name = name;
		return p;
	}

	protected Point createCorner(Vector3 position, string name) {
		Point p = (Point)Instantiate (pointPrefab, position, Quaternion.identity);
		p.position = position;
		p.transform.parent = transform;
		points.Add (p);
		p.name = name;
		return p;
	}

	protected void init(float stiffness, float dampening, float segmentLength) {
		ropeStiffness = stiffness;
		ropeDampening = dampening;
		this.segmentLength = segmentLength;
		points = new List<Point> ();
		integrateList = new List<IntegrateAbstract>();
	}

	/** 
	 * Clears old forces and sets all to zero.
	 */
	virtual public void clearForces() {
		foreach(Point p in points) {
			p.force = Vector3.zero;
		}
	}

	/**
	 * Applies gravity to all points.
	 */
	protected void gravity() {
		foreach(Point p in points) {
			p.force += ih.gravity * p.mass;
		}
	}

	protected void wind() {
		foreach(Point p in points) {
			p.force += ih.wind + ih.wind / 10f * (Mathf.Sin(Time.fixedTime/2));
		}

	}

	/**
	 * Applies spring forces to all points by walking through all points and making them affect their neighbours.
	 */
	protected void springForces() {

		foreach(Point p in points) {
			foreach (Point.Neighbour n in p.GetNeighbours()) {
				springforce (p, n);
			}
		}
	}

	/**
	 * Applies spring forces to two points p and n.
	 */
	protected void springforce(Point point, Point.Neighbour neighbour) {
		Vector3 force = Vector3.zero;
		Vector3 distance = neighbour.neighbour.position - point.position;

		force = ropeStiffness * (distance.magnitude - segmentLength) * (distance / distance.magnitude);
		force -= ropeDampening * (point.velocity - neighbour.neighbour.velocity);

		point.force += force;
		neighbour.neighbour.force -= force;
		neighbour.force = force;
	}

	virtual public void collideWith(Ball ball) {
		//foreach (Point p in points) {
		//	ball.collide (p);
		//}
	}

	public List<Point> getPoints() {
		return points;
	}
}
