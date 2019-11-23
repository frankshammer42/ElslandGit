using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slicer2DLinearCutControllerObject : Slicer2DControllerObject {
	// Algorhitmic
	Pair2D linearPair = Pair2D.Zero();

	// Settings
	public float cutSize = 0.5f;

	public void Update(Vector2D pos) {

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		float newCutSize = cutSize + scroll;
		if (newCutSize > 0.05f) {
			cutSize = newCutSize;
		}

		if (input.GetInputClicked()) {
			linearPair.A.Set (pos);
		}

		if (input.GetInputHolding()) {
			linearPair.B.Set (pos);
		}

		if (input.GetInputReleased()) {
			LinearCut linearCutLine = LinearCut.Create(linearPair, cutSize * visuals.visualScale);
			Slicer2D.LinearCutSliceAll (linearCutLine, sliceLayer);
		}
	}

	public void Draw(Transform transform) {
		if (input.GetInputHolding()) {
			visuals.Clear();
			visuals.GenerateLinearCutMesh(linearPair, cutSize, transform);
			visuals.Draw();
		}
	}
}
