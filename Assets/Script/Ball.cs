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

		//transform.position = position;
		position = transform.position; // TODO Remove
	}

	// Update is called once per frame
	public void simulate (float timestep) {

		velocity *= 0.95f;

		velocity += force / mass * timestep;
		position += velocity * timestep;
	}

	public void collide(Point p) {
		Vector3 distance = p.position - position;
		if (distance.magnitude < 3) {
			//Debug.Log ("Collision" + distance + " " + distance.magnitude + " " + p.transform.localScale.x + transform.localScale.x);
		}

	}

	public void collide(Plank b) {
		// Bounding sphere
		Vector3 distance = b.position - position;
		if (distance.magnitude > 17) {
			// TODO return;
		}

		Debug.DrawRay (b.position, b.xAxis (), Color.green, 1000f, true); // Left
		Debug.DrawRay (b.position, -b.xAxis (), Color.yellow, 1000f, true); // Right

		Debug.DrawRay (b.position, b.yAxis (), Color.blue, 1000f, true); // Top
		Debug.DrawRay (b.position, -b.yAxis (), Color.red, 1000f, true); // Bottom

		Debug.DrawRay (b.position, b.zAxis (), Color.green, 1000f, true); // Front
		Debug.DrawRay (b.position, -b.zAxis (), Color.yellow, 1000f, true); // Back

		// in_right 	(b.position + b.xAxis(), b.xAxis());
		// in_left   	(b.position - b.xAxis(), -b.xAxis());
		// in_front  	(b.position + b.zAxis(), b.zAxis());
		// in_back   	(b.position - b.zAxis(), -b.zAxis());
		// in_top    	(b.position + b.yAxis(), b.yAxis());
		// in_bottom	(b.position - b.yAxis(), -b.yAxis());
		Debug.Log("Right " + plane_distance(b.position + b.xAxis(), b.xAxis()));
		Debug.Log("left " + plane_distance(b.position - b.xAxis(), -b.xAxis()));
		Debug.Log("front " + plane_distance(b.position + b.zAxis(), b.zAxis()));
		Debug.Log("back " + plane_distance(b.position - b.zAxis(), -b.zAxis()));
		Debug.Log("top " + plane_distance(b.position + b.yAxis(), b.yAxis()));
		Debug.Log("bottom " + plane_distance(b.position - b.yAxis(), -b.yAxis()));

		Debug.Log ("x " + b.xAxis ().magnitude);
		Debug.Log ("y " + b.yAxis ().magnitude);
		Debug.Log ("z " + b.zAxis ().magnitude);

		DrawPlane (b.position - b.xAxis (), -b.xAxis ());
		DrawPlane (b.position + b.xAxis (), b.xAxis ());
		DrawPlane (b.position + b.zAxis (), b.zAxis ());
		DrawPlane (b.position - b.zAxis (), -b.zAxis ());
		DrawPlane (b.position + b.yAxis (), b.yAxis ());
		DrawPlane (b.position - b.yAxis (), -b.yAxis ());
		if (sphere_intersects_box (b)) {
		//	Debug.Log ("Yes we do" + b.position);
		}
	}



	float plane_distance(Vector3 planePosition, Vector3 planeNormal) {
		return Vector3.Dot(position - planePosition, planeNormal);
	}

	bool sphere_inside_plane(Vector3 planePosition, Vector3 planeNormal) {
		return -plane_distance(planePosition, planeNormal) > 2.5f;
	}

	bool sphere_outside_plane(Vector3 planePosition, Vector3 planeNormal) {
		return plane_distance(planePosition, planeNormal) > 2.5f;
	}

	bool sphere_intersects_plane(Vector3 planePosition, Vector3 planeNormal) {
		return Mathf.Abs(plane_distance(planePosition, planeNormal)) <= 2.5f;
	}

	bool sphere_intersects_box(Plank b) {
  
		// http://theorangeduck.com/page/correct-box-sphere-intersection


		bool in_right  = !sphere_outside_plane(b.position + b.xAxis(), b.xAxis());
		bool in_left   = !sphere_outside_plane(b.position - b.xAxis(), -b.xAxis());
		bool in_front  = !sphere_outside_plane(b.position + b.zAxis(), b.zAxis());
		bool in_back   = !sphere_outside_plane(b.position - b.zAxis(), -b.zAxis());
		bool in_top    = !sphere_outside_plane(b.position + b.yAxis(), b.yAxis());
		bool in_bottom = !sphere_outside_plane(b.position - b.yAxis(), -b.yAxis());

		Debug.DrawRay (position, Vector3.up * 2.5f, Color.green, 1000f, true);

		if (sphere_intersects_plane(b.position + b.yAxis(), b.yAxis()) /*&& in_left && in_right && in_front && in_back*/) {
	    	
			// Intersect top
			Debug.Log("Top intersect");
			//return true;
	 	 }
	  
		if (sphere_intersects_plane(b.position - b.yAxis(), -b.yAxis()) /*&& in_left && in_right && in_front && in_back*/) {
	    	
			// Intersect Bottom
			Debug.Log("Bottom intersect");
			//return true;
	  	}
	  
		if (sphere_intersects_plane(b.position - b.xAxis(), -b.xAxis()) /*&& in_top && in_bottom && in_front && in_back*/) {
	    	
			// Intersect left
			Debug.Log("Left intersect");

			//return true;
	  	}
	  
		if (sphere_intersects_plane(b.position + b.xAxis(), b.xAxis()) /*&& in_top && in_bottom && in_front && in_back*/) {
			// Intersect Right
			Debug.Log("Right intersect");

			//return true;
	  	}
	  
		if (sphere_intersects_plane(b.position + b.zAxis(), b.zAxis()) /*&& in_top && in_bottom && in_left && in_right*/) {
			// Intersect Front
			Debug.Log("Front intersect");

			//return true;
	  	}
	  
		if (sphere_intersects_plane(b.position - b.zAxis(), -b.zAxis()) /*&& in_top && in_bottom && in_left && in_right*/) {
			// Intersect back
			Debug.Log("Back intersect");

			//return true;
	  	}
	  
	  	return false;
	}

	public void DrawPlane(Vector3 position, Vector3 normal) {
 
		Vector3 v3;
		if (normal.normalized != Vector3.forward && normal.normalized != Vector3.back) {
			v3 = Vector3.Cross (normal, Vector3.forward).normalized * normal.magnitude;
		} else {
			v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;;
		}
		     
		Vector3 corner0 = position + v3;
		Vector3 corner2 = position - v3;
		Quaternion q = Quaternion.AngleAxis(90.0f, normal);
		v3 = q * v3;
		Vector3 corner1 = position + v3;
		Vector3 corner3 = position - v3;
			 
		Debug.DrawLine(corner0, corner2, Color.green);
		Debug.DrawLine(corner1, corner3, Color.green);
		Debug.DrawLine(corner0, corner1, Color.green);
		Debug.DrawLine(corner1, corner2, Color.green);
		Debug.DrawLine(corner2, corner3, Color.green);
		Debug.DrawLine(corner3, corner0, Color.green);

		Debug.DrawLine(position, corner0, Color.yellow);
		Debug.DrawLine(position, corner1, Color.yellow);
		Debug.DrawLine(position, corner2, Color.yellow);
		Debug.DrawLine(position, corner3, Color.yellow);

		Debug.DrawRay(position, normal, Color.red);
	}

}
