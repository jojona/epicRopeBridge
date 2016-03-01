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
	private Action <float> forceFunc;



	public Integrator(List <Point> points, Action<float> updateForcesFunc){
		m_calcData = new List<IntegrateData> ();
		initIntegrateData (points.Count);
		forceFunc = updateForcesFunc;

	}

	public void integrate(){

	}

	public void evaluate(List <Point> points, Action <float> updateForcesFunc, float timestep){

	}

	public void initIntegrateData(int numPoints){
		
		int diff = numPoints - m_calcData.Count;

		//Do we need more? Create more! Never shrink
		if (diff > 0){
			for (int i = 0; i < diff; i++){
				m_calcData.Add(new CalcData());
			}
		}

		for (int i = 0; i < numPoints; i++){
			m_calcData[i].evalResult.Reset();
		}
	}
}
