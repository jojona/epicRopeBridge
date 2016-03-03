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