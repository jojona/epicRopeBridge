using UnityEngine;
using System.Collections;

public class Derivative {
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
