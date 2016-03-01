﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Integrator {
	private class Derivative{
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

	private class IntegrateData{
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
	/// Integrator
	/////////////////////////

	private List<IntegrateData> m_calcData = new List<IntegrateData>();
	List<PointController> pcl;
	private int numPoints, prevNumPoints;
	private float timestep;


	public Integrator(List<PointController> pcl, float timestep){
		m_calcData = new List<IntegrateData> ();
		this.timestep = timestep;
		this.pcl = pcl;
		numPoints = 0;
		prevNumPoints = 0;
		prepIntegration ();
	}

	public void integrate(List<PointController> pcl, Action forceFunc){
		this.pcl = pcl;
		prepIntegration ();

		PointController pc;

		// a

		evaluate(forceFunc, timestep*0f);
		for (int i = 0, tot = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++j, ++tot) {
				m_calcData [tot].a = m_calcData [tot].evalResult;
			}
		}

		// b

		evaluate(forceFunc, timestep*0.5f);
		for (int i = 0, tot = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++j, ++tot) {
				m_calcData [tot].b = m_calcData [tot].evalResult;
			}
		}

		// c

		evaluate(forceFunc, timestep*0.5f);
		for (int i = 0, tot = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++j, ++tot) {
				m_calcData [tot].c = m_calcData [tot].evalResult;
			}
		}

		// d

		evaluate(forceFunc, timestep*1f);
		for (int i = 0, tot = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++j, ++tot) {
				m_calcData [tot].d = m_calcData [tot].evalResult;
			}
		}

		IntegrateData iData;
		Vector3 deltaPos, deltaVel;
		Point p;

		// Weighted sum
		for (int i = 0, tot = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++j, ++tot) {
				iData = m_calcData [tot];
				deltaPos = (1f / 6f) * (iData.a.deltaPosition + 2 * (iData.b.deltaPosition + iData.c.deltaPosition) + iData.d.deltaPosition);
				deltaVel = (1f / 6f) * (iData.a.deltaVelocity + 2 * (iData.b.deltaVelocity + iData.c.deltaVelocity) + iData.d.deltaVelocity);
				p = pc.getPoints () [j];
				p.position += deltaPos*timestep;
				p.velocity += deltaVel*timestep;
			}
		}

	}

	private void evaluate(Action updateForcesFunc, float dt){
		PointController pc;
		Point p;

		Derivative derivative;

		saveState ();
		for (int i = 0, tot = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++j, ++tot) {
				p = pc.getPoints () [j];
				derivative = m_calcData [tot].evalResult;
				p.position += derivative.deltaPosition * dt;
				p.velocity += derivative.deltaVelocity * dt;
			}
		}
		updateForcesFunc ();
		for (int i = 0, tot = 0; i < pcl.Count; ++i) {
			pc = pcl [i];
			for (int j = 0; j < pc.getPoints().Count; ++j, ++tot) {
				p = pc.getPoints () [j];
				derivative = m_calcData [tot].evalResult;
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
		foreach(PointController pc in pcl) {
			foreach(Point p in pc.getPoints()) {
				p.statePos = p.position;
				p.stateVel = p.velocity;
			}
		}
	}

	private void loadState(){
		foreach(PointController pc in pcl) {
			foreach(Point p in pc.getPoints()) {
				p.position = p.statePos;
				p.velocity = p.stateVel;
			}
		}
	}
}
