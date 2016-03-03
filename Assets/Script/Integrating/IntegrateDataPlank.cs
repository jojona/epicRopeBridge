using UnityEngine;

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
		p.position += (1f / 6f) * (a.deltaX + 2 * (b.deltaX + c.deltaX) + d.deltaX) * timestep;

		p.q.x += (1f / 6f) * (a.deltaQ.x + 2 * (b.deltaQ.x + c.deltaQ.x) + d.deltaQ.x) * timestep;
		p.q.y += (1f / 6f) * (a.deltaQ.y + 2 * (b.deltaQ.y + c.deltaQ.y) + d.deltaQ.y) * timestep;
		p.q.z += (1f / 6f) * (a.deltaQ.z + 2 * (b.deltaQ.z + c.deltaQ.z) + d.deltaQ.z) * timestep;
		p.q.w += (1f / 6f) * (a.deltaQ.w + 2 * (b.deltaQ.w + c.deltaQ.w) + d.deltaQ.w) * timestep;

		p.P += (1f / 6f) * (a.deltaP + 2 * (b.deltaP + c.deltaP) + d.deltaP) * timestep;
		p.L += (1f / 6f) * (a.deltaL + 2 * (b.deltaL + c.deltaL) + d.deltaL) * timestep;
	}


	public override void eulerSum(float timestep) {
		p.position += p.P / p.mass * timestep;
		p.P += p.force * timestep;
		p.L += p.torque * timestep;

		p.q.x += p.dq.x * timestep;
		p.q.y += p.dq.y * timestep;
		p.q.z += p.dq.z * timestep;
		p.q.w += p.dq.w * timestep;

		Vector3 velocity = p.P / p.mass;
		p.point1.position += (velocity + Vector3.Cross (p.w, (p.point1.position - p.position))) * timestep;
		p.point2.position += (velocity + Vector3.Cross (p.w, (p.point2.position - p.position))) * timestep;
		p.point3.position += (velocity + Vector3.Cross (p.w, (p.point3.position - p.position))) * timestep;
		p.point4.position += (velocity + Vector3.Cross (p.w, (p.point4.position - p.position))) * timestep;
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

	public override void tryDerivate(float timestep) { //TODO
		
		p.position += evalResult.deltaX * timestep;

		p.q.x += evalResult.deltaQ.x * timestep;
		p.q.z += evalResult.deltaQ.z * timestep;
		p.q.y += evalResult.deltaQ.y * timestep;
		p.q.w += evalResult.deltaQ.w * timestep;

		p.P += evalResult.deltaP * timestep;

		p.L += evalResult.deltaL * timestep;


	}

	public override void saveDerivate() {
		evalResult.deltaX = p.P / p.mass;
		evalResult.deltaQ = (new Quaternion (p.w.x * 1 / 2, p.w.y * 1 / 2, p.w.z * 1 / 2, 0)) * p.q;
		evalResult.deltaP = p.force;
		evalResult.deltaL = p.torque;

		p.point1.integrate.evalResult.deltaPosition = p.P / p.mass + Vector3.Cross (p.w, (p.point1.position - p.position));
		p.point2.integrate.evalResult.deltaPosition = p.P / p.mass + Vector3.Cross (p.w, (p.point2.position - p.position));
		p.point3.integrate.evalResult.deltaPosition = p.P / p.mass + Vector3.Cross (p.w, (p.point3.position - p.position));
		p.point4.integrate.evalResult.deltaPosition = p.P / p.mass + Vector3.Cross (p.w, (p.point4.position - p.position));

		p.point1.integrate.evalResult.deltaVelocity = Vector3.zero;
		p.point2.integrate.evalResult.deltaVelocity = Vector3.zero;
		p.point3.integrate.evalResult.deltaVelocity = Vector3.zero;
		p.point4.integrate.evalResult.deltaVelocity = Vector3.zero;
//		evalResult.deltaPosition = p.velocity;
//		evalResult.deltaVelocity = p.force / p.mass;
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
	}

	public override void reset() {
		evalResult.reset();
	}

}