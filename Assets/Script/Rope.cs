using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Rope : PointController {
	// Start point for rope
	private Vector3 startPoint;

	// End point for rope
	private Vector3 endPoint;

	// Anchor points
	private List<Point> anchors = new List<Point>();

	/**
	 * Empty constructor.
	 */
	public void Start() { }

	/**
	 * Inits this rope.
	 */
	public void init(Vector3 start, Vector3 end, bool anchor, int amountOfPoints, string name, float stiffness, float damping, float pointMass, float segLength, float restForce, float maxForce) {
		init(stiffness, damping, segLength);
		this.name = name;
		// Inits points list, sets posiiton and direction
		Vector3 position = start;
		Vector3 ropeDirection = (end - start) / amountOfPoints;

		// Generates rope
		for (int i = 0; i < amountOfPoints; ++i) {
			createPoint(position, name + i, pointMass);
			position += ropeDirection;
		}

		// Adds links between internal points
		for (int i = 0; i < amountOfPoints - 1; ++i) {
			points [i].AddNeighbour(points [i + 1], segLength, restForce, maxForce);
		}

		// Sets anchor if so
		if (anchor) {
			anchors.Add(points [0]);
			anchors.Add(points[amountOfPoints - 1]);
		}
	}

	override public void simulationStep() {
		gravity ();
		wind ();
		springForces ();
		endPoints ();
	}

	/**
	 * Returns number of points.
	 */
	public int amountOfPoints() {
		return points.Count;
	}

	/**
	 * Returns a specific point.
	 */
	public Point getPoint(int index) {
		return points[index];
	}

	public int length() {
		return points.Count;
	}

	/**
	 * Sets force for endpoints to zero to make them fixed. 
	 * Should be called after calculating forces for all points.
	 */
	protected void endPoints() {
		foreach (Point endp in anchors) {
			endp.force = Vector3.zero;
		}
	}

	override public void clearMovement() {
		foreach(Point p in points) {
			p.clearMovement();
		}
	}
}
