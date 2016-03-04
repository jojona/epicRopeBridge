using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MiddleRope : PointController {

	public Plank plankPrefab;

	private List<Plank> planks;

	public MiddleRope() {
		planks = new List<Plank> ();
		points = new List<Point> ();
	}

	// Use this for initialization
	void Start () {
		
	}

	override public void simulationStep() {
		gravity ();
		springForces ();

		foreach(Plank p in planks) {
			p.simulation();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public void clearMovement() {
		foreach(Point p in points) {
			p.clearMovement();
		}

		foreach(Plank p in planks) {
			p.clearMovement();
		}
	}

	/**
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 */
	public void addPlank(Plank plank) {
		planks.Add (plank);
	}
}
