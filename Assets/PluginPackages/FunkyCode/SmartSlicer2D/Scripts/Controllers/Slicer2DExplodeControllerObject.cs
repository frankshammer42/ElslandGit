using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slicer2DExplodeControllerObject : Slicer2DControllerObject {
	// Settings
	public bool addForce = true;
	public float addForceAmount = 5f;

	public bool explodeFromPoint = false;

	public void Update(Vector2D pos) {
		if (input.GetInputClicked()) {
			if (explodeFromPoint) {
				ExplodeFromPoint(pos);
			} else {
				Explode(pos);
			}
		}
	}

	void ExplodeFromPoint(Vector2D pos) {
		List<Slice2D> results =	Slicer2D.ExplodeByPointAll (pos, sliceLayer);

		foreach (Slice2D id in results) {
			eventHandler.Perform(id);
		}

		if (addForce == true) {
			foreach (Slice2D id in results) {
				Slicer2DAddForce.ExplodeByPoint(id, addForceAmount, pos);
			}
		}
	}

	void Explode(Vector2D pos) {
		List<Slice2D> results =	Slicer2D.ExplodeInPointAll (pos, sliceLayer);

		foreach (Slice2D id in results) {
			eventHandler.Perform(id);
		}

		if (addForce == true) {
			foreach (Slice2D id in results) {
				Slicer2DAddForce.ExplodeInPoint(id, addForceAmount, pos);
			}
		}
	}

	public void Draw(Transform transform) {
		Vector2D pos = input.GetInputPosition().Copy();
		
		visuals.Clear();
		visuals.GeneratePointMesh(new Pair2D(pos, pos), transform);
		visuals.Draw();
	}
}
