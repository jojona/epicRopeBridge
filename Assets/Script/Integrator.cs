using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Integrator {
	public class Derivative{
		public Vector3 deltaVelocity;
		public Vector3 deltaPosition;
		public Derivative (){
			deltaPosition = Vector3.zero;
			deltaVelocity = Vector3.zero;
		}

		public void reset(){
			deltaPosition = Vector3.zero;
			deltaVelocity = Vector3.zero;
		}
	}

	public class IntegrateData{
		public Derivative evalResult;
		public Derivative a;
		public Derivative b;
		public Derivative c;
		public Derivative d;
		public IntegrateData(){
			evalResult = new Derivative();
			a = new Derivative();
			b = new Derivative();
			c = new Derivative();
			d = new Derivative();
		}
	}


	/////////////////////////
	/// Actual Integrator
	/////////////////////////

	private List<IntegrateData> m_calcData = new List<IntegrateData>();
	private Action forceFunc;
	private int numPoints, prevNumPoints;


	public Integrator(List<PointController> pcl, Action updateForcesFunc){
		m_calcData = new List<IntegrateData> ();

		numPoints = 0;
		prevNumPoints = 0;
		prepIntegrateData (pcl);

		forceFunc = updateForcesFunc;

	}

	public void integrate(List<PointController> pcl){
		PointController pc;

		// a

		evaluate(pcl, forceFunc, 0); //TODO
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				m_calcData [i + j].a = m_calcData [i + j].evalResult;
			}
		}


	}

	private void evaluate(List<PointController> pcl, Action updateForcesFunc, float timestep){
		foreach (PointController pc in pcl) {
			foreach (Point point in pc.getPoints ()) {

			}
		}
	}

	private void prepIntegrateData(List<PointController> pcl){
		numPoints = 0;
		foreach (PointController pc in pcl) {
			numPoints += pc.getPoints().Count;
		}
		int diff = numPoints - prevNumPoints;
		prevNumPoints = numPoints;

		//Do we need more? Create more! Never shrink
		if (diff > 0){
			for (int i = 0; i < diff; i++){
				m_calcData.Add(new IntegrateData());
			}
		}

		for (int i = 0; i < numPoints; i++){
			m_calcData[i].evalResult.reset();
		}
	}

}
