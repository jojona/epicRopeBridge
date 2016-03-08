using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Ball : MonoBehaviour {

	public InputHandler ih;

	public Vector3 force = Vector3.zero;
	public Vector3 position;
	private Vector3 lastPostion;
	private Vector3 velocity = Vector3.zero;

	public int mass;

	public Transform downwall;
	public Transform rightwall;
	public Transform leftwall;
	public Transform topwall;
	public Transform forwardwall;
	public Transform backwall;

	[Range(1, 100)]
	public int speed = 1;
	[Range(1, 100)]
	public int upSpeed = 10;

	[Tooltip("Force 0, Velocity 1, position 2")]
	[Range(0, 2)]
	public int controlMode = 1;

	public bool gravity = true;

	private float radius = 2.5f;

	public float penaltySpringKonstant = 200f;

	private struct Collision {
		public bool collision;
		public Vector3 normal;
		public Vector3 position;
		public float distance;
	}


	// Use this for initialization
	void Start () {
		position = transform.position;
		lastPostion = position;
	}

	void Update() {
		if (controlMode == 0) {

			if (Input.GetKey ("up") || Input.GetKey ("w")) {
				force += Vector3.right * speed * 10 * Time.deltaTime * mass;
			}
			if (Input.GetKey ("down") || Input.GetKey ("s")) {
				force += Vector3.left * speed * 10 * Time.deltaTime * mass;
			}
			if (Input.GetKey ("left") || Input.GetKey ("a")) {
				force += Vector3.forward * speed * 10 * Time.deltaTime * mass;
			}
			if (Input.GetKey ("right") || Input.GetKey ("d")) {
				force += Vector3.back * speed * Time.deltaTime * mass;
			}
			if (Input.GetKey ("space") || Input.GetKey ("z") || Input.GetKey ("left shift") || Input.GetKey ("q") ) {
				force += Vector3.up * upSpeed * 10 * Time.deltaTime * mass;
			}
			if (Input.GetKey ("x") || Input.GetKey("e")) {
				force += Vector3.down * 10 *upSpeed * Time.deltaTime * mass;
			}
		} else if (controlMode == 1) {

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
			if (Input.GetKey ("space") || Input.GetKey ("z") || Input.GetKey ("left shift") || Input.GetKey ("q") ) {
				velocity += Vector3.up * 10 * upSpeed * Time.deltaTime;
			}
			if (Input.GetKey ("x") || Input.GetKey("e")) {
				velocity += Vector3.down * 10 * upSpeed * Time.deltaTime;
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
			if (Input.GetKey ("space") || Input.GetKey ("z") || Input.GetKey ("left shift") || Input.GetKey ("q") ) {
				position += Vector3.up * upSpeed * Time.deltaTime;
			}
			if (Input.GetKey ("x") || Input.GetKey("e")) {
				position += Vector3.down * 10 * upSpeed * Time.deltaTime;
			}
		}
	}

	void LateUpdate() {

		transform.position = position;
	}

	// Update is called once per frame
	public void simulate (float timestep) {
		lastPostion = position;

		if (gravity) force += ih.gravity * mass;

		collideBoundary();

		velocity += force / mass * timestep;
		position += velocity * timestep;
		force = Vector3.zero;
	}

	public void collideBoundary() {
		if (!behindPlane(downwall.position, Vector3.up) && Vector3.Dot(velocity, Vector3.up) < 0) {
			velocity += Vector3.Dot(-velocity, Vector3.up) * Vector3.up;
		}
		if (!behindPlane(rightwall.position, Vector3.right) && Vector3.Dot(velocity, Vector3.right) < 0) {
			velocity += Vector3.Dot(-velocity, Vector3.right) * Vector3.right;
		}
		if (!behindPlane(leftwall.position, Vector3.left) && Vector3.Dot(velocity, Vector3.left) < 0) {
			velocity += Vector3.Dot(-velocity, Vector3.left) * Vector3.left;
		}
		if (!behindPlane(topwall.position, Vector3.down) && Vector3.Dot(velocity, Vector3.down) < 0) {
			velocity += Vector3.Dot(-velocity, Vector3.down) * Vector3.down;
		}
		if (!behindPlane(forwardwall.position, Vector3.forward) && Vector3.Dot(velocity, Vector3.forward) < 0) {
			velocity += Vector3.Dot(-velocity, Vector3.forward) * Vector3.forward;
		}
		if (!behindPlane(backwall.position, Vector3.back) && Vector3.Dot(velocity, Vector3.back) < 0) {
			velocity += Vector3.Dot(-velocity, Vector3.back) * Vector3.back;
		}
	}

	public void collide(Point p) {
		return;
		Vector3 distance = p.position - position;
		if (distance.magnitude < 3) {
			//Vector3 relativeVel = velocity - p.velocity;
			Vector3 normal = (position - p.position).normalized;

			Vector3 relativeVel = (velocity - p.velocity).magnitude * normal;
			
			//Debug.Break();
			//Debug.DrawRay(p.position, relativeVel, Color.green, 0.1f, true);

			if (Vector3.Dot(relativeVel, normal) >= 0) {

				Vector3 ra = -normal * radius;
				Vector3 rb = normal * 0.5f;

				float e = 1;
				float j = (-(1.0f + e) * relativeVel.magnitude) / ( (1.0f / mass) + (1.0f / p.mass) + (Vector3.Dot(normal, Vector3.Cross(2.0f/5 * mass * radius * radius * Vector3.Cross(ra, normal), ra))) + (Vector3.Dot(normal, Vector3.Cross(2.0f/5 * p.mass * 0.5f * 0.5f * Vector3.Cross(rb, normal), rb))) );
				velocity -= j * normal/ mass;
				p.velocity += j * normal / p.mass;

			}
			// Add penalty spring force
			force += relativeVel.normalized * (distance.magnitude - 3f) * penaltySpringKonstant;
		}

	}

	public void collide(Plank plank) {
		// Bounding sphere
		Vector3 distance = plank.position - position;
		if (distance.magnitude > 17) {
			return;
		}
		
		// OLD Collision c = sphere_intersects_box(plank);
		Collision c = boxCollision(plank);

		if (c.collision) {	// Planks are thin enough so ball cannot be completly inside a plank
			//Debug.Log ("Collision with plank " + plank.name);

			Vector3 pointVelocity = plank.P / plank.mass + Vector3.Cross(plank.w, (c.position - plank.position));

			float relativeVel = Vector3.Dot((velocity - pointVelocity), c.normal);

			if (relativeVel < 0) {
				Vector3 ra = position - c.position;
				Vector3 rb = plank.position - c.position;

				// Collision
				Vector3 tmpVector = Vector3.Cross(rb, c.normal);
				Matrix m = new Matrix(3,1);
				m[0, 0] = tmpVector[0];
				m[1, 0] = tmpVector[1];
				m[2, 0] = tmpVector[2];
				m = plank.Iinv * m;
				tmpVector[0] = m[0, 0];
				tmpVector[1] = m[1, 0];
				tmpVector[2] = m[2, 0];

				float e = 1;
				float j = -(-(1.0f + e) * relativeVel) / ( (1.0f / mass) + (1.0f / plank.mass) + (Vector3.Dot(c.normal, Vector3.Cross(2.0f/5 * mass * radius * radius * Vector3.Cross(ra, c.normal), ra))) + (Vector3.Dot(c.normal, Vector3.Cross(tmpVector, rb))) );

				// Add force relativeVel * collisionNormal at collisionPosition
				plank.P += j * c.normal;
				plank.L += Vector3.Cross(c.position - plank.position, j * c.normal);

				velocity += -j * c.normal / mass;


				// Add penalty spring force instead
				//position -= c.normal * (c.distance - radius); // TODO fix
			}
			force += c.normal * (c.distance - radius) * penaltySpringKonstant;
		}
	}
		
	float distanceToPlane(Vector3 planePosition, Vector3 planeNormal) {
		return Vector3.Dot(position - planePosition, planeNormal.normalized);
	}

	bool behindPlane(Vector3 planePosition, Vector3 planeNormal) {
		return distanceToPlane(planePosition, planeNormal) > radius;
	}

	bool intersectPlane(Vector3 planePosition, Vector3 planeNormal) {
		return Mathf.Abs(distanceToPlane(planePosition, planeNormal)) <= radius;
	}


	Collision boxCollision(Plank plank) {
		float rightDistance = distanceToPlane(plank.position + plank.xAxis(), plank.xAxis().normalized);
		float leftDistance = distanceToPlane(plank.position - plank.xAxis(), -plank.xAxis().normalized);
		float frontDistance = distanceToPlane(plank.position + plank.zAxis(), plank.zAxis().normalized);
		float backDistance = distanceToPlane(plank.position - plank.zAxis(), -plank.zAxis().normalized);
		float topDistance = distanceToPlane(plank.position + plank.yAxis(), plank.yAxis().normalized);
		float bottomDistance = distanceToPlane(plank.position - plank.yAxis(), -plank.yAxis().normalized);

		// Note: Behind if distance > radius;

		Collision c = new Collision();


		// Top Bottom
		if (rightDistance < radius && leftDistance < radius && frontDistance < radius && backDistance < radius) {
			if (Mathf.Abs(topDistance) <= radius || Mathf.Abs(bottomDistance) <= radius) {
				if (Vector3.Dot(lastPostion - position, plank.yAxis()) >= 0) {
					c.normal = plank.yAxis().normalized;
					c.distance = topDistance;
					c.collision = true;
				} else {
					c.normal = -plank.yAxis().normalized;
					c.distance = bottomDistance;
					c.collision = true;
				}
			}
		} 
		/*
		if (rightDistance < radius && leftDistance < radius && topDistance < radius && bottomDistance < radius) {
			if (Mathf.Abs(rightDistance) <= radius || Mathf.Abs(leftDistance) <= radius) {
				if (Vector3.Dot(lastPostion - position, plank.zAxis()) <= 0) {
					if (rightDistance < c.distance) {
						c.normal = plank.zAxis().normalized;
						c.distance = frontDistance;
						c.collision = true;
						Debug.Log("!updown collision");
					}
				} else {
					if (leftDistance < c.distance) {
						c.normal = -plank.zAxis().normalized;
						c.distance = backDistance;
						c.collision = true;
						Debug.Log("!updown collision");
					}
				}
			}
		} 
		// Front back
		if (frontDistance < radius && backDistance < radius && topDistance < radius && bottomDistance < radius) {
			if (Mathf.Abs(frontDistance) <= radius || Mathf.Abs(backDistance) <= radius) {
				if (Vector3.Dot(lastPostion - position, plank.xAxis()) <= 0) {
					if (frontDistance < c.distance) {
						c.normal = plank.xAxis().normalized;
						c.distance = topDistance;
						c.collision = true;
						Debug.Log("!updown collision");
					}
				} else {
					if (backDistance < c.distance) {
						c.normal = -plank.xAxis().normalized;
						c.distance = bottomDistance;
						c.collision = true;
						Debug.Log("!updown collision");
					}
				}
			}
		} 
		//*/

		c.position = position - c.distance * c.normal;

		//if (c.collision) {
		//	Debug.Break();
		//	Debug.DrawRay(c.position, Vector3.up, Color.red, 1f, true);
		//}
		return c;


	}

	Collision sphere_intersects_box(Plank plank) {
		// http://theorangeduck.com/page/correct-box-sphere-intersection
		bool in_right  = !behindPlane(plank.position + plank.xAxis(), plank.xAxis().normalized);
		bool in_left   = !behindPlane(plank.position - plank.xAxis(), -plank.xAxis().normalized);
		bool in_front  = !behindPlane(plank.position + plank.zAxis(), plank.zAxis().normalized);
		bool in_back   = !behindPlane(plank.position - plank.zAxis(), -plank.zAxis().normalized);
		bool in_top    = !behindPlane(plank.position + plank.yAxis(), plank.yAxis().normalized);
		bool in_bottom = !behindPlane(plank.position - plank.yAxis(), -plank.yAxis().normalized);

		Collision c = new Collision();

		if (intersectPlane(plank.position + plank.yAxis(), plank.yAxis()) && in_left && in_right && in_front && in_back) {
			c.collision = true;
	 	}
		if (intersectPlane(plank.position - plank.yAxis(), -plank.yAxis()) && in_left && in_right && in_front && in_back) {
			c.collision = true;
	  	}
		if (intersectPlane(plank.position - plank.xAxis(), -plank.xAxis()) && in_top && in_bottom && in_front && in_back) {
	    	c.collision = true;
	  	}
		if (intersectPlane(plank.position + plank.xAxis(), plank.xAxis()) && in_top && in_bottom && in_front && in_back) {
			c.collision = true;
	  	}
		if (intersectPlane(plank.position + plank.zAxis(), plank.zAxis()) && in_top && in_bottom && in_left && in_right) {
			c.collision = true;
	  	}
		if (intersectPlane(plank.position - plank.zAxis(), -plank.zAxis()) && in_top && in_bottom && in_left && in_right) {
			c.collision = true;
	  	}


	  	c.normal = plank.yAxis().normalized; // TODO correct axis
	  	c.distance = distanceToPlane(plank.position + plank.yAxis(), c.normal); // TODO

		c.position = position - c.distance * c.normal;
		Debug.DrawRay(c.position, c.normal * 5, Color.green, 100f, true);

	  	return c;
	}


	/**
	 * Draw a debug plane at <position> with normal <normal>
	 */
	public static void DrawPlane(Vector3 position, Vector3 normal) {
 
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
