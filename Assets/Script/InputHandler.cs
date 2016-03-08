using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

	public GameObject canvas;
	public UI ui;


	public Vector3 gravity = Vector3.down * 9.82f;
	[System.NonSerialized]
	public Vector3 wind = Vector3.zero;

	public Vector3 windStrength = Vector3.left * 3;

	private int windmode = 0;
	private bool showUI = false;
	// Use this for initialization
	void Start () {
		PointController.ih = this;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.F2)) {
			showUI = !showUI;
			canvas.SetActive (showUI);
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
	
	}
}
