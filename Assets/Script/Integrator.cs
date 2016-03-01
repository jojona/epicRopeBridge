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
	List<PointController> pcl;
	private Action forceFunc;
	private int numPoints, prevNumPoints;


	public Integrator(List<PointController> pcl, Action updateForcesFunc){
		m_calcData = new List<IntegrateData> ();
		this.pcl = pcl;
		numPoints = 0;
		prevNumPoints = 0;
		prepIntegration ();

		forceFunc = updateForcesFunc;

	}

	public void integrate(List<PointController> pcl){
		this.pcl = pcl;
		prepIntegration ();

		PointController pc;

		// a

		evaluate(forceFunc, 0f); //TODO timestep?
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				m_calcData [i + j].a = m_calcData [i + j].evalResult;
			}
		}

		// b

		evaluate(forceFunc, 0.5f); //TODO timestep?
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				m_calcData [i + j].b = m_calcData [i + j].evalResult;
			}
		}

		// c

		evaluate(forceFunc, 0.5f); //TODO timestep?
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				m_calcData [i + j].c = m_calcData [i + j].evalResult;
			}
		}

		// d

		evaluate(forceFunc, 1f); //TODO timestep?
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				m_calcData [i + j].d = m_calcData [i + j].evalResult;
			}
		}

		IntegrateData iData;
		Vector3 deltaPos, deltaVel;
		Point p;


		// Weighted sum
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				iData = m_calcData [i + j];
				deltaPos = (1f / 6f) * (iData.a.deltaPosition + 2 * (iData.b.deltaPosition + iData.c.deltaPosition) + iData.d.deltaPosition);
				deltaVel = (1f / 6f) * (iData.a.deltaVelocity + 2 * (iData.b.deltaVelocity + iData.c.deltaVelocity) + iData.d.deltaVelocity);
				p = pc.getPoints () [j];
				p.position += deltaPos;
				p.velocity += deltaVel; // TODO multiply with timestep?
			}
		}

	}

	private void evaluate(Action updateForcesFunc, float timestep){
		PointController pc;
		Point p;

		Derivative derivative;

		saveState ();
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				p = pc.getPoints () [j];
				derivative = m_calcData [i + j].evalResult;
				p.position += derivative.deltaPosition * timestep;
				p.velocity += derivative.deltaVelocity * timestep;
			}
		}
		updateForcesFunc ();
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				p = pc.getPoints () [j];
				derivative = m_calcData [i + j].evalResult;
				derivative.deltaPosition = p.velocity;
				derivative.deltaVelocity = p.force / p.mass;
			}
		}
		loadState ();
	}

	private void prepIntegration(){
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


	private void saveState(){
		PointController pc;
		Point p;
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				p = pc.getPoints () [i + j];
				p.statePos = p.position;
				p.stateVel = p.velocity;
			}
		}
	}

	private void loadState(){
		PointController pc;
		Point p;
		for (int i = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++i) {
				p = pc.getPoints () [i + j];
				p.position = p.statePos;
				p.velocity = p.stateVel;
			}
		}
	}

}
