using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Integrator {
	/////////////////////////
	/// Integrator
	/////////////////////////


	List<PointController> pcl;
	private int numPoints, prevNumPoints;
	private float timestep;


	public Integrator(List<PointController> pcl, float timestep){
		this.timestep = timestep;
		this.pcl = pcl;
	}

	/*
	 * Euler integration
	 */
	public void euler(List<PointController> pcl, Action simulationStep) {
		simulationStep ();
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList	){
				i.eulerSum (timestep);
			}
		}
//		foreach (PointController pc in pcl) {
//			foreach (Point point in pc.getPoints ()) {
//				point.velocity += timestep * point.force / point.mass;
//				point.position += timestep * point.velocity;
//			}
//		}
	}

	/**
	 * RK4 Integration
	 */
	public void integrate(List<PointController> pcl, Action forceFunc){
		this.pcl = pcl;


		// a
		evaluate(forceFunc, timestep*0f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepA ();	
			}
		}

		// a
		evaluate(forceFunc, timestep*0.5f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepB ();	
			}
		}

		// a
		evaluate(forceFunc, timestep*0.5f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepC ();	
			}
		}

		// a
		evaluate(forceFunc, timestep*1f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepD ();	
			}
		}

	
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.weightedSum (timestep);	
			}
		}

	}

	private void evaluate(Action updateForcesFunc, float dt){

		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.saveState ();	
			}
		}


		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.tryDerivate (timestep);	
			}
		}
		updateForcesFunc ();

		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.saveDerivate ();	
			}
		}

		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.loadState ();	
			}
		}
	}

}
