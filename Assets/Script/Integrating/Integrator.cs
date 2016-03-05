using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Integrator {
	/////////////////////////
	/// Integrator
	/////////////////////////

	private int numPoints, prevNumPoints;


	public Integrator(){

	}

	/*
	 * Euler integration
	 */
	public void euler(List<PointController> pcl, Action simulationStep, float timestep) {
		simulationStep ();

		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList	){
				i.eulerSum (timestep);
			}
		}
	}

	/**
	 * RK4 Integration
	 */
	public void integrate(List<PointController> pcl, Action forceFunc, float timestep){
		// CLear
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.reset ();	
			}
		}

		// A
		evaluate(pcl, forceFunc, timestep*0f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepA ();	
			}
		}
			
		// B
		evaluate(pcl, forceFunc, timestep*0.5f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepB ();	
			}
		}
			
		// C
		evaluate(pcl, forceFunc, timestep*0.5f);
		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.stepC ();	
			}
		}
			
		// D
		evaluate(pcl, forceFunc, timestep*1f);
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

	private void evaluate(List<PointController> pcl, Action updateForcesFunc, float dt){

		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.saveState ();	
			}
		}

		foreach(PointController pc in pcl) {
			foreach(IntegrateAbstract i in pc.integrateList){
				i.tryDerivate (dt);	
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
