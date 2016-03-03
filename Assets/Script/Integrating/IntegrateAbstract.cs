using UnityEngine;
using System.Collections;

public abstract class IntegrateAbstract {
	
	public abstract void stepA();
	public abstract void stepB();
	public abstract void stepC();
	public abstract void stepD();

	public abstract void tryDerivate(float timestep);
	public abstract void saveDerivate();

	public abstract void saveState();
	public abstract void loadState ();

	public abstract void weightedSum(float timestep);

	public abstract void eulerSum (float timestep);

	public abstract void reset ();
}
