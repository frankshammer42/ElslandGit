using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Slicer2DComplexCutControllerObject : Slicer2DControllerObject {
	// Algorhitmic
	List<Vector2D> pointsList = new List<Vector2D>();

	// Settings
	public float cutSize = 0.5f;
	public float minVertexDistance = 1f;

	public List<Vector2D> GetList() {
		List<Vector2D> list = new List<Vector2D>(pointsList);
		
		if (list.Count > 0) {
			Vector2D pos = new Vector2D(input.GetInputPosition());
			if (Vector2D.Distance (list.Last(), pos) > 0.01f) {
				list.Add(pos);
			}
		}

		return(list);
	}


	public void Update(Vector2D pos) {

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		float newCutSize = cutSize + scroll;
		if (newCutSize > 0.05f) {
			cutSize = newCutSize;
		}

		if (input.GetInputClicked()) {
			pointsList.Clear ();
			pointsList.Add (pos);
		}

		if (pointsList.Count < 1) {
			return;
		}
		
		if (input.GetInputHolding()) {
			Vector2D posMove = pointsList.Last ().Copy();
			int loopCount = 0;
			while ((Vector2D.Distance (posMove, pos) > minVertexDistance * visuals.visualScale)) {
				float direction = (float)Vector2D.Atan2 (pos, posMove);
				posMove.Push (direction, minVertexDistance * visuals.visualScale);

				pointsList.Add (posMove.Copy());

				loopCount ++;
				if (loopCount > 150) {
					break;
				}
			}
		}

		if (input.GetInputReleased()) {
			ComplexCut complexCutLine = ComplexCut.Create(GetList(), cutSize * visuals.visualScale);
			Slicer2D.ComplexCutSliceAll (complexCutLine, sliceLayer);

			pointsList.Clear ();
		}
	}

	public void Draw(Transform transform) {
		if (input.GetInputHolding()) {
			visuals.Clear();
			visuals.GenerateComplexCutMesh(GetList(), cutSize, transform);
			visuals.Draw();
		}
	}
}
