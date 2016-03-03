using UnityEngine;
using System.Collections;

public class DerivativeRotation {
	public Vector3 deltaX;
	public Quaternion deltaQ;
	public Vector3 deltaL;
	public Vector3 deltaP;

	public DerivativeRotation (){
		deltaX = Vector3.zero;
		deltaQ = Quaternion.identity;
		deltaL = Vector3.zero;
		deltaP = Vector3.zero;
	}

	public void reset(){
		deltaX = Vector3.zero;
		deltaQ = Quaternion.identity;
		deltaL = Vector3.zero;
		deltaP = Vector3.zero;
	}
}
