using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	public Vector3 force = Vector3.zero;
	Vector3 position;
	Vector3 velocity = Vector3.zero;

	public int mass;
	public int speed = 10;
	[Range(10, 1000)]
	public int upspeed = 15;

	[Tooltip("Force 0, Velocity 1, position 2")]
	[Range(0, 2)]
	public int controlMode = 0;


	// Use this for initialization
	void Start () {
		position = transform.position;
	}

	void Update() {

		if (controlMode == 0) {
			force = Vector3.zero;

			force += Vector3.down * 9.82f * Time.deltaTime * 10;

			if (Input.GetKey ("up") || Input.GetKey ("w")) {
				force += Vector3.right * speed * 10 * Time.deltaTime;
			}
			if (Input.GetKey ("down") || Input.GetKey ("s")) {
				force += Vector3.left * speed * 10 * Time.deltaTime;
			}
			if (Input.GetKey ("left") || Input.GetKey ("a")) {
				force += Vector3.forward * speed * 10 * Time.deltaTime;
			}
			if (Input.GetKey ("right") || Input.GetKey ("d")) {
				force += Vector3.back * speed * Time.deltaTime;
			}
			if (Input.GetKey ("space") || Input.GetKey ("z") || Input.GetKey ("left shift")) {
				force += Vector3.up * upspeed * 10 * Time.deltaTime;
			}
		} else if (controlMode == 1) {

			velocity += Vector3.down * 9.82f * 10 * Time.deltaTime / mass;

			if (Input.GetKey ("up") || Input.GetKey ("w")) {
				velocity += Vector3.right * 10 * speed * Time.deltaTime;
			}
			if (Input.GetKey ("down") || Input.GetKey ("s")) {
				velocity += Vector3.left * 10 * speed * Time.deltaTime;
			}
			if (Input.GetKey ("left") || Input.GetKey ("a")) {
				velocity += Vector3.forward * 10 * speed * Time.deltaTime;
			}
			if (Input.GetKey ("right") || Input.GetKey ("d")) {
				velocity += Vector3.back * 10 * speed * Time.deltaTime;
			}
			if (Input.GetKey ("space") || Input.GetKey ("z") || Input.GetKey ("left shift")) {
				velocity += Vector3.up * 10 * upspeed * Time.deltaTime;
			}
		} else if (controlMode == 2) {
			if (Input.GetKey ("up") || Input.GetKey ("w")) {
				position += Vector3.right * speed * Time.deltaTime;
			}
			if (Input.GetKey ("down") || Input.GetKey ("s")) {
				position += Vector3.left * speed * Time.deltaTime;
			}
			if (Input.GetKey ("left") || Input.GetKey ("a")) {
				position += Vector3.forward * speed * Time.deltaTime;
			}
			if (Input.GetKey ("right") || Input.GetKey ("d")) {
				position += Vector3.back * speed * Time.deltaTime;
			}
			if (Input.GetKey ("space") || Input.GetKey ("z") || Input.GetKey ("left shift")) {
				position += Vector3.up * upspeed * Time.deltaTime;
			}
			if (Input.GetKey ("x")) {
				position += Vector3.down * upspeed * Time.deltaTime;
			}
		}


		// || Input.GetKey(KeyCode.Space) || Input.GetKey ("left shift") || Input.GetKey (KeyCode.Keypad1)
	}

	void LateUpdate() {
		transform.position = position;
	}

	// Update is called once per frame
	public void simulate (float timestep) {

		velocity *= 0.95f;

		velocity += force / mass * timestep;
		position += velocity * timestep;
	}
}
