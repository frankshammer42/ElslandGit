using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slicer2DLinearTrackerControllerObject : Slicer2DControllerObject {
	// Algorhitmic
	List<Vector2D> pointsList = new List<Vector2D>();

	public static LinearSlicerTracker linearTracker = new LinearSlicerTracker();

	// Settings
	float minVertexDistance = 1f;

	public void Update(Vector2D pos) {
		if (input.GetInputClicked()) {
			pointsList.Clear ();
			linearTracker.trackerList.Clear ();
			pointsList.Add (pos);
		}
						
		if (input.GetInputPressed() && pointsList.Count > 0) {
			linearTracker.Update(pos.ToVector2(), minVertexDistance);
		}

		if (input.GetInputReleased()) {
			pointsList.Clear();

			linearTracker.trackerList.Clear();
		}
	}

	public void Draw(Transform transform) {
		if (pointsList.Count > 0) {
			visuals.Clear();
			visuals.GenerateLinearTrackerMesh(input.GetInputPosition(), linearTracker.trackerList, transform);
			visuals.Draw();
		}
	}
}
