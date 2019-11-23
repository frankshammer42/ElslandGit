using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicerTrackerObject {
	public Vector2D firstPosition;
	public Vector2D lastPosition;
	public List<Vector2D> pointsList = new List<Vector2D>();
	public bool tracking = false;

	public List<Vector2D> GetLinearPoints() {
		List<Vector2D> points = new List<Vector2D>();
		points.Add(firstPosition);
		points.Add(lastPosition);
		return(points);
	}
}