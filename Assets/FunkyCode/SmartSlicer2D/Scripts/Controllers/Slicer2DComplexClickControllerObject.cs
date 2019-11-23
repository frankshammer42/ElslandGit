using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[System.Serializable]
public class Slicer2DComplexClickControllerObject : Slicer2DControllerObject {
	// Algorithm
	List<Vector2D> pointsList = new List<Vector2D>();

	// Settings
	public Slicer2D.SliceType complexSliceType = Slicer2D.SliceType.SliceHole;
	public int pointsLimit = 3;
	public bool sliceJoints = false;
	public bool endSliceIfPossible = false;

	public bool addForce = true;
	public float addForceAmount = 5f;

	public void Update(Vector2D pos) {
	
		if (endSliceIfPossible) {
			bool ended = false;
			if (input.GetInputClicked()) {
				pointsList.Add(pos);
				Slicer2D.complexSliceType = complexSliceType;
				ended = ComplexSlice (pointsList);;
			}

			if (ended) {
				pointsList.Clear();
			}

		} else {
			if (input.GetInputClicked()) {
				pointsList.Add(pos);
			}

			if (pointsList.Count >= pointsLimit) {
				Slicer2D.complexSliceType = complexSliceType;
				ComplexSlice (pointsList);
				pointsList.Clear ();
			}
		}
	}
	
	bool ComplexSlice(List <Vector2D> slice) {
		if (sliceJoints) {
			Slicer2DJoints.ComplexSliceJoints(slice);
		}

		List<Slice2D> results = Slicer2D.ComplexSliceAll (slice, sliceLayer);
		bool result = false;

		foreach (Slice2D id in results) {
			if (id.GetGameObjects().Count > 0) {
				result = true;
			}

			eventHandler.Perform(id);
		}

		if (addForce == true) {
			foreach (Slice2D id in results)  {
				Slicer2DAddForce.ComplexSlice(id, addForceAmount);
			}
		}
		return(result);
	}

	public void Draw(Transform transform) {
		if (pointsList.Count > 0) {
			List<Vector2D> points = new List<Vector2D>(pointsList);
			Vector2D posA = input.GetInputPosition().Copy();
			points.Add(posA);
			
			visuals.Clear();
			visuals.GenerateComplexMesh(points, transform);
			visuals.Draw();
		}
	}
}
