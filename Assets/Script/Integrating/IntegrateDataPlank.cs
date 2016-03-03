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
		p.q += (1f / 6f) * (a.deltaQ + 2 * (b.deltaQ + c.deltaQ) + d.deltaQ) * timestep;
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
		p.position += evalResult.deltaPosition * timestep;
		p.velocity += evalResult.deltaVelocity * timestep;
	}

	public override void saveDerivate() {
		evalResult.deltaPosition = p.velocity;
		evalResult.deltaVelocity = p.force / p.mass;
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