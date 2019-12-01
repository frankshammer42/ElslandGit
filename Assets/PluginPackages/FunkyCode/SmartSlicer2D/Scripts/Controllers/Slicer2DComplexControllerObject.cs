using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Slicer2DComplexControllerObject : Slicer2DControllerObject {
	// Algorhitmic
	public List<Vector2D>[] pointsList = new List<Vector2D>[10];
	bool startedSlice = false;

	// Settings
	public Slicer2D.SliceType complexSliceType = Slicer2D.SliceType.SliceHole;

	// Autocomplete
	public bool autocomplete = false;
	public bool autocompleteDisplay = false;
	public float autocompleteDistance = 1;

	public float minVertexDistance = 1f;
	public bool endSliceIfPossible = false;
	public bool startSliceIfPossible = false;
	public bool sliceJoints = false;

	public bool addForce = true;
	public float addForceAmount = 5f;

	public void Initialize() {
		for(int id = 0; id < 10; id++) {
			pointsList[id] = new List<Vector2D>();
		}
	}

	public List<Vector2D> GetList(int id) {
		List<Vector2D> list = new List<Vector2D>(pointsList[id]);
		if (list.Count > 0) {
			Vector2D pos = new Vector2D(input.GetInputPosition(id));
			if (Vector2D.Distance (list.Last(), pos) > 0.01f) {
				list.Add(pos);
			}
		}
			
		return(list);
	}

	public List<Vector2D> GetPoints(int id) {
		if (autocomplete) {
			return(Slicer2DAutoComplete.GetPoints(GetList(id), autocompleteDistance));
		}
		return(GetList(id));
	}

	public void Update() {
		for(int id = 0; id < 10; id++) {
			Vector2D pos = new Vector2D(input.GetInputPosition(id));
				
			if (input.GetInputClicked(id)) {
				pointsList[id].Clear ();
				pointsList[id].Add (pos);
				startedSlice = false;
			}

			if (pointsList[id].Count < 1) {
				return;
			}
			
			if (input.GetInputHolding(id)) {
				Vector2D posMove = pointsList[id].Last ().Copy();
				bool added = false;
				int loopCount = 0;

				while ((Vector2D.Distance (posMove, pos) > minVertexDistance * visuals.visualScale)) {
					float direction = (float)Vector2D.Atan2 (pos, posMove);
					posMove.Push (direction, minVertexDistance * visuals.visualScale);

					if (startSliceIfPossible == true && startedSlice == false) {
						if (Slicer2D.PointInSlicerComponent(posMove.Copy()) != null) {
							while (pointsList[id].Count > 2) {
								pointsList[id].RemoveAt(0);
							}

							startedSlice = true;
						}
					}

					pointsList[id].Add (posMove.Copy());

					added = true;

					loopCount ++;
					if (loopCount > 150) {
						break;
					}
				}

				if (endSliceIfPossible == true && added) {
					if (ComplexSlice (GetPoints(id)) == true) {
						pointsList[id].Clear ();

						if (startSliceIfPossible) {
							pointsList[id].Add (pos);
							startedSlice = false;
						}
					}
				}
			}

			if (input.GetInputReleased(id)) {
				startedSlice = false;
				Slicer2D.complexSliceType = complexSliceType;
				ComplexSlice (GetPoints(id));
				pointsList[id].Clear ();
			}
		}
	}

	public void Draw(Transform transform) {
		visuals.Clear();

		for(int id = 0; id < 10; id++) {
			if (input.GetInputHolding(id) ) {
				if (pointsList[id].Count > 0) {
					if (startSliceIfPossible == false || startedSlice == true) {
						List<Vector2D> points = GetList(id);

						if (autocompleteDisplay) {
							points = GetPoints(id);
						}
						
						visuals.GenerateComplexMesh(points, transform);
					}
				}
			}
		}

		visuals.Draw();
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
}
