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
	}

	public static int lap = 0;

	/**
	 * RK4 Integration
	 */
	public void integrate(List<PointController> pcl, Action forceFunc){
		this.pcl = pcl;

		Integrator.lap = 0;
		// CLear
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.reset ();	
			}
		}

		// A
		evaluate(forceFunc, timestep*0f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepA ();	
			}
		}

		Integrator.lap = 1;
		// B
		evaluate(forceFunc, timestep*0.5f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepB ();	
			}
		}

		Integrator.lap = 2;
		// C
		evaluate(forceFunc, timestep*0.5f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepC ();	
			}
		}
		Integrator.lap = 3;

		// D
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
