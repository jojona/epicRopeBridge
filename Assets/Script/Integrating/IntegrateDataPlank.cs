using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class IntegrateDataPlank : IntegrateAbstract{

	public DerivativeRotation evalResult;
	public DerivativeRotation a;
	public DerivativeRotation b;
	public DerivativeRotation c;
	public DerivativeRotation d;

	Plank p;

	public IntegrateDataPlank(Plank p) {
		evalResult = new DerivativeRotation();
		a = new DerivativeRotation();
		b = new DerivativeRotation();
		c = new DerivativeRotation();
		d = new DerivativeRotation();

		this.p = p;
	}

	public override void weightedSum(float timestep) {
		p.P += (1.0f / 6.0f) * (a.deltaP + 2.0f * (b.deltaP + c.deltaP) + d.deltaP) * timestep;
		p.L += (1.0f / 6.0f) * (a.deltaL + 2.0f * (b.deltaL + c.deltaL) + d.deltaL) * timestep;
		
		p.position += (1.0f / 6.0f) * (a.deltaX + 2.0f * (b.deltaX + c.deltaX) + d.deltaX) * timestep;

		p.q.x += (1.0f / 6.0f) * (a.deltaQ.x + 2.0f * (b.deltaQ.x + c.deltaQ.x) + d.deltaQ.x) * timestep;
		p.q.y += (1.0f / 6.0f) * (a.deltaQ.y + 2.0f * (b.deltaQ.y + c.deltaQ.y) + d.deltaQ.y) * timestep;
		p.q.z += (1.0f / 6.0f) * (a.deltaQ.z + 2.0f * (b.deltaQ.z + c.deltaQ.z) + d.deltaQ.z) * timestep;
		p.q.w += (1.0f / 6.0f) * (a.deltaQ.w + 2.0f * (b.deltaQ.w + c.deltaQ.w) + d.deltaQ.w) * timestep;

		p.normalizeQuaternion();
		p.calculateR();
		p.pointPositions();
		p.calculateIinv();
	}


	public override void eulerSum(float timestep) {
		p.P += p.force * timestep;
		p.L += p.torque * timestep;
		
		p.calculateW();

		//p.position += p.P / p.mass * timestep;
		Quaternion dq = (new Quaternion (p.w.x * 1 / 2, p.w.y * 1 / 2, p.w.z * 1 / 2, 0)) * p.q;
		p.q.x += dq.x * timestep;
		p.q.y += dq.y * timestep;
		p.q.z += dq.z * timestep;
		p.q.w += dq.w * timestep;
		
		p.normalizeQuaternion();
		p.calculateR();
		p.pointPositions();
		p.calculateIinv();
	}

	public override void stepA() {
		a = evalResult;
	}
	public override void stepB() {
		b = evalResult;
	}
	public override void stepC() {
		c = evalResult;
	}
	public override void stepD(){
		d = evalResult;
	}

	public override void tryDerivate(float timestep) {
		
		p.position += evalResult.deltaX * timestep;

		p.q.x += evalResult.deltaQ.x * timestep;
		p.q.y += evalResult.deltaQ.y * timestep;
		p.q.z += evalResult.deltaQ.z * timestep;
		p.q.w += evalResult.deltaQ.w * timestep;

		p.P += evalResult.deltaP * timestep;

		p.L += evalResult.deltaL * timestep;

		p.normalizeQuaternion();
		p.calculateR();
		p.pointPositions();
		p.calculateIinv();
	}

	public override void saveDerivate() {
		evalResult.deltaP = p.force;
		evalResult.deltaL = p.torque;
		

		evalResult.deltaX = p.P / p.mass;

		p.calculateW();
		Quaternion dq = (new Quaternion (p.w.x * 1 / 2, p.w.y * 1 / 2, p.w.z * 1 / 2, 0));
		normalizeQuaternion(dq);
		evalResult.deltaQ =  dq * p.q;
		normalizeQuaternion(evalResult.deltaQ); 
	}

	public override void saveState() {
		p.statePos = p.position;
		p.stateP = p.P;
		p.stateL = p.L;
		p.stateQ = p.q;
	}

	public override void loadState () {
		p.position = p.statePos;
		p.P = p.stateP;
		p.L = p.stateL;
		p.q = p.stateQ;

		p.normalizeQuaternion();
		p.calculateR();
		p.pointPositions();
		p.calculateIinv();
	}

	public override void reset() {
		evalResult.reset();
	}

	public void normalizeQuaternion(Quaternion q) {
		float norm = Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y*q.y + q.z * q.z);
		q.w = q.w / norm;
		q.x = q.x / norm;
		q.y = q.y / norm;
		q.z = q.z / norm;
	}

}