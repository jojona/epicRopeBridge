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

	/** 
	 * Clears old forces and sets all to zero.
	 */
	override public void clearForces() {
		for (int i = 0; i < points.Count; ++i) {
			points [i].force = Vector3.zero;
		}
		foreach (Plank plank in planks) {
			plank.force = Vector3.zero;
			plank.torque = Vector3.zero;
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

	public void addPlank(Plank plank) {
		planks.Add (plank);
		plank.transform.parent = transform;
		integrateList.Add (new IntegrateDataPlank(plank));
	}

	override public void collideWith(Ball ball) {
		//foreach (Point p in points) {
		//	ball.collide (p);
		//}
		foreach (Plank plank in planks) {
			ball.collide (plank);
		}
	}
}
