﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Point : MonoBehaviour {

	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 force = Vector3.zero;
	public float mass = 1.0f;

	public Vector3 RK4force = Vector3.zero;
	public Vector3 savedPos = Vector3.zero;
	public Vector3 savedVel = Vector3.zero;


	private List<Point> neighbour = new List<Point>();

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void LateUpdate () {
		transform.position = position;
	}

	public void AddNeigbour(Point p) {
		neighbour.Add (p);
	}

	public List<Point> GetNeighours() {
		return neighbour;
	}
}
