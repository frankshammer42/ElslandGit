using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slicer2DPolygonControllerObject : Slicer2DControllerObject {
	// Settings
	public Polygon2D.PolygonType polygonType = Polygon2D.PolygonType.Circle;
	public float polygonSize = 1;
	public bool polygonDestroy = true;	
	public int edgeCount = 30;

	public void Update(Vector2D pos) {
		float newPolygonSize = polygonSize + Input.GetAxis("Mouse ScrollWheel");
		if (newPolygonSize > 0.05f) {
			polygonSize = newPolygonSize;
		}

		if (input.GetInputClicked()) {
			PolygonSlice (pos);
		}
	}

	private void PolygonSlice(Vector2D pos) {
		Polygon2D.defaultCircleVerticesCount = edgeCount;

		Slicer2D.PolygonSliceAll(pos, Polygon2D.Create (polygonType, polygonSize * visuals.visualScale), polygonDestroy, sliceLayer);
	}

	public void Draw(Transform transform) {
		visuals.Clear();
		visuals.GeneratePolygonMesh(input.GetInputPosition(), polygonType, polygonSize, transform);
		visuals.Draw();
	}
}
