using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public Text fps;
	public Text wind;

	public float updateIntervall = 0.5f;

	private string fpsString;
	private string windString;

	int frames = 0;
	float timeLeft = 0;

	private Vector3 windVector;
	private bool updateWind;

	// Use this for initialization
	void Start () {
		fpsString = fps.text;
		windString = wind.text;

		wind.text = windString + " " + windVector;
	}
	
	// Update is called once per frame
	void Update () {
		++frames;
		timeLeft -= Time.deltaTime;

		if (timeLeft <= 0.0f) {
			fps.text = fpsString + (frames / updateIntervall);
			frames = 0;
			timeLeft = updateIntervall;
		}

		if (updateWind) {
			wind.text = windString + " " + windVector;
			updateWind = false;
		}




	}

	public void setWind(Vector3 windV) {
		updateWind = true;
		windVector = windV;
	}
}
