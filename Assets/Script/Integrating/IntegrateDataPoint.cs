using UnityEngine;
using System.Collections;

public class IntegrateDataPoint : IntegrateAbstract{
	public Derivative evalResult;
	public Derivative a;
	public Derivative b;
	public Derivative c;
	public Derivative d;

	Point p;

	public IntegrateDataPoint(Point p) {
		evalResult = new Derivative();
		a = new Derivative();
		b = new Derivative();
		c = new Derivative();
		d = new Derivative();

		this.p = p;
	}

	public override void weightedSum(float timestep) {
		Vector3 deltaPos = (1.0f / 6.0f) * (a.deltaPosition + 2.0f * (b.deltaPosition + c.deltaPosition) + d.deltaPosition);
		Vector3 deltaVel = (1.0f / 6.0f) * (a.deltaVelocity + 2.0f * (b.deltaVelocity + c.deltaVelocity) + d.deltaVelocity);

		p.position += deltaPos*timestep;
		p.velocity += deltaVel*timestep;
	}

	public override void eulerSum(float timestep) {
		p.velocity += timestep * p.force / p.mass;
		p.position += timestep * p.velocity;
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
		p.position += evalResult.deltaPosition * timestep;
		p.velocity += evalResult.deltaVelocity * timestep;
	}

	public override void saveDerivate() {
		evalResult.deltaPosition = p.velocity;
		evalResult.deltaVelocity = p.force / p.mass;
	}

	public override void saveState() {
		p.statePos = p.position;
		p.stateVel = p.velocity;
	}
	public override void loadState () {
		p.position = p.statePos;
		p.velocity = p.stateVel;
	}

	public override void reset() {
		evalResult.reset();
	}
}
