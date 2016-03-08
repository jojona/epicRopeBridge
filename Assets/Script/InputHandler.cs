using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour {

	public GameObject canvas;
	public UI ui;

	public Text controls;

	[System.NonSerialized]
	public Vector3 gravity = Vector3.zero;
	[System.NonSerialized]
	public Vector3 wind = Vector3.zero;

	public Vector3 windStrength = Vector3.left * 3;
	public Vector3 gravityStrength = Vector3.down * 9.82f;

	private int windmode = 0;
	private int showUI = 0;
	// Use this for initialization
	void Start () {
		PointController.ih = this;
		gravity = gravityStrength;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.F2)) {
			showUI++;
			showUI = showUI % 3;

			if (showUI == 0) {
				canvas.SetActive (true);
				controls.gameObject.SetActive (true);
			} else if (showUI == 1) {
				controls.gameObject.SetActive (false);
			} else if (showUI == 2) {
				canvas.SetActive (false);
			}
		}

		if (Input.GetKeyDown(KeyCode.T)) {
			windmode++;
			windmode = windmode % 3;

			if (windmode == 0) {
				wind = Vector3.zero;
				ui.setWind (wind);
			} else if (windmode == 1) {
				wind = windStrength;
				ui.setWind (wind);
			} else if (windmode == 2) {
				wind = -windStrength;
				ui.setWind (wind);
			}
		}

		if (Input.GetKeyDown (KeyCode.G)) {
			if (gravity == Vector3.zero) {
				gravity = gravityStrength;
			} else {
				gravity = Vector3.zero;
			}

		}
	
	}
}
