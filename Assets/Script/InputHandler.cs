using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour {

	public GameObject canvas;
	public UI ui;

	public Text controls;

	public Camera cam;
	public Camera maincam;
	public Ball ball;

	[System.NonSerialized]
	public Vector3 gravity = Vector3.zero;
	[System.NonSerialized]
	public Vector3 wind = Vector3.zero;

	public Vector3 windStrength = Vector3.left * 3;
	public Vector3 gravityStrength = Vector3.down * 9.82f;

	public int windmode = 0;
	public int showUI = 0;

	private Vector3 center = new Vector3(100, 0, 0);

	// Use this for initialization
	void Start () {
		PointController.ih = this;
		gravity = gravityStrength;
		toggleWind ();
		toggleUI ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.F2)) {
			showUI++;
			toggleUI ();
		}

		if (Input.GetKeyDown(KeyCode.T)) {
			windmode++;
			toggleWind ();
		}

		if (Input.GetKeyDown (KeyCode.G)) {
			if (gravity == Vector3.zero) {
				gravity = gravityStrength;
			} else {
				gravity = Vector3.zero;
			}

		}

		if(Input.GetMouseButton(2)) {
			cam.gameObject.transform.RotateAround (center, Vector3.up, 1f);
		}

		if (Input.GetMouseButton (0)) {
			maincam.gameObject.transform.RotateAround (ball.position, Vector3.up, 2f);
		}
		if (Input.GetMouseButton (1)) {
			maincam.gameObject.transform.RotateAround (ball.position, Vector3.up, -2f);
		}
	
	}

	private void toggleUI() {
		
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

	private void toggleWind() {
		
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
}
