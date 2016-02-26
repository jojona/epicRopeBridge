using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class PointController : MonoBehaviour {

	public Point pointPrefab;
	protected List<Point> points;

	protected Point createPoint(Vector3 position) {
		Point p = (Point)Instantiate (pointPrefab, position, Quaternion.identity);
		p.position = position;
		p.transform.parent = transform;
		return p;
	}

	protected Point createPoint(Vector3 position, string name) {
		Point p = createPoint (position);
		p.name = name;
		return p;
	}
}
