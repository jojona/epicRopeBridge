using UnityEngine;
using System.Collections;

public abstract class IntegrateAbstract {
	
	// Save derivate in derivate A
	public abstract void stepA();

	// Save derivate in derivate B
	public abstract void stepB();

	// Save derivate in derivate C
	public abstract void stepC();

	// Save derivate in derivate D 
	public abstract void stepD();

	// Calculate new derivate for current step 
	public abstract void tryDerivate(float timestep);

	// Save derivate for current step
	public abstract void saveDerivate();

	// Save initial state
	public abstract void saveState();

	// Load initial state
	public abstract void loadState ();

	// Calculate new poitions
	public abstract void weightedSum(float timestep);

	public abstract void reset ();

	// Euler
	public abstract void eulerSum (float timestep);

}
