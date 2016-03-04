﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Point : MonoBehaviour {

	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 force = Vector3.zero;
	public float mass = 10000f;

	public Vector3 statePos = Vector3.zero;
	public Vector3 stateVel = Vector3.zero;

	private List<Point> neighbour = new List<Point>();

	// Update is called once per frame
	void LateUpdate () {
		transform.position = position;
		foreach (Point n in GetNeighours()) {
			Debug.DrawRay (position, n.position-position, Color.black, 0.01f);
		}
	}

	public void AddNeigbour(Point p) {
		neighbour.Add (p);
	}

	public List<Point> GetNeighours() {
		return neighbour;
	}

	public void clearMovement() {
		velocity = Vector3.zero;
		force = Vector3.zero;
	}
}
