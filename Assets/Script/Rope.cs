﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Rope : PointController {
	// Start point for rope
	private Vector3 startPoint;

	// End point for rope
	private Vector3 endPoint;

	// Anchor points
	private List<Point> anchors;

	/**
	 * Empty constructor.
	 */
	public void Start() { }

	/**
	 * Inits this rope.
	 */
	public void init(Vector3 start, Vector3 end, bool anchor, int amountOfPoints, string name) {
		// Inits points list, sets posiiton and direction
		points = new List<Point>();
		Vector3 position = start;
		Vector3 ropeDirection = (end - start) / amountOfPoints;

		// Generates rope
		for (int i = 0; i < amountOfPoints; ++i) {
			points.Add(createPoint(position, name + i));
			position += ropeDirection;
		}

		// Adds links between internal points
		for (int i = 0; i < amountOfPoints - 1; ++i) {
			points [i].AddNeigbour(points [i + 1]);
		}

		// Sets anchor if so
		if (anchor) {
			anchors = new List<Point> ();
			anchors.Add(points [0]);
			anchors.Add(points[amountOfPoints - 1]);
		}
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
}
