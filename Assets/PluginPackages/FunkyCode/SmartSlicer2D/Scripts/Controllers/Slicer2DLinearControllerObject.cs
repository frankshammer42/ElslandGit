using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slicer2DLinearControllerObject : Slicer2DControllerObject {
	// Algorhitmic
	Pair2D[] linearPair = new Pair2D[10];
	public bool startedSlice = false;

	// Settings
	public bool endSliceIfPossible = false;
	public bool startSliceIfPossible = false;
	public bool strippedLinear = false;
	public float minVertexDistance = 1f;
	public bool displayCollisions = false;
	public bool sliceJoints = false;

	// Autocomplete
	public bool autocomplete = false;
	public bool autocompleteDisplay = false;
	public float autocompleteDistance = 1;

	public bool addForce = true;
	public float addForceAmount = 5f;

	public void Initialize() {
		for(int id = 0; id < 10; id++) {
			linearPair[id] = Pair2D.Zero();
		}
	}

	public Pair2D GetPair(int id) {
		if (autocomplete) {
			return(Slicer2DAutoComplete.GetPair(linearPair[id], autocompleteDistance));
		}
		return(linearPair[id]);
	}

	public void Update() {
		for(int id = 0; id < 10; id++) {
			Vector2D pos = new Vector2D(input.GetInputPosition(id));

			if (input.GetInputClicked(id)) {
				linearPair[id] = new Pair2D(pos.Copy(), pos.Copy());
				startedSlice = false;
			}
			
			// Start Slice If Possible
			if (startSliceIfPossible) {
				if (startedSlice == true) { 
					if (Slicer2D.PointInSlicerComponent(pos.Copy()) == null) {
						startedSlice = false;
					}
				} else if (startedSlice == false) {
					if (Slicer2D.PointInSlicerComponent(pos.Copy()) != null) {
						startedSlice = true;
					} else {
						linearPair[id].A.Set (pos.Copy());
					}
				}
			}

			// End Slice If Possible
			if (input.GetInputHolding(id)) {
				linearPair[id].B.Set (pos);
			
				if (endSliceIfPossible) {
					if (input.GetSlicingEnabled(id)) {
						if (LinearSlice (GetPair(id))) {
							linearPair[id].A.Set (pos);

							if (startSliceIfPossible) {
								linearPair[id] = new Pair2D(pos.Copy(), pos.Copy());
								startedSlice = false;
							}
						}
					}
				}
			}

			if (input.GetInputReleased(id)) {
				if (input.GetSlicingEnabled(id)) {
					LinearSlice (GetPair(id));
				}
			}
		}
	}
	
	public void Draw(Transform transform) {
		if (input.GetVisualsEnabled() == false) {
			return;
		}
		
		visuals.Clear();

		for(int id = 0; id < 10; id++) {

			if (input.GetInputHolding(id)) {

				if (startSliceIfPossible == false || startedSlice == true) {
					Pair2D pair = linearPair[id];

					if (autocompleteDisplay) {
						pair = GetPair(id);
					}
					
					// If Stripped Line
					if (strippedLinear) {
						List<Vector2D> linearPoints = GetLinearVertices(pair, minVertexDistance * visuals.visualScale);

						if (linearPoints.Count > 1) {
							visuals.GenerateComplexMesh(linearPoints, transform);
						}
					
					} else {
						visuals.GenerateLinearMesh(pair, transform);
					}

					if (displayCollisions) {
						List<Slice2D> results = Slicer2D.LinearSliceAll (linearPair[id], sliceLayer, false);
						foreach(Slice2D slice in results) {
							foreach(Vector2D collision in slice.GetCollisions()) {
								Pair2D p = new Pair2D(collision, collision);
								visuals.GenerateLinearMesh(p, transform);
							}
						}
					}
				}
			}
		}

		visuals.Draw();

		return;
	}

	private bool LinearSlice(Pair2D slice) {
		if (sliceJoints) {
			Slicer2DJoints.LinearSliceJoints(slice);
		}
		
		List<Slice2D> results = Slicer2D.LinearSliceAll (slice, sliceLayer);
		bool result = false;

		foreach (Slice2D id in results)  {
			if (id.GetGameObjects().Count > 0) {
				result = true;
			}

			eventHandler.Perform(id);
		}

		if (addForce == true) {
			foreach (Slice2D id in results)  {
				Slicer2DAddForce.LinearSlice(id, addForceAmount);
			}
		}

		return(result);
	}

	static public List<Vector2D> GetLinearVertices(Pair2D pair, float minVertexDistance) {
		Vector2D startPoint = pair.A.Copy();
		Vector2D endPoint = pair.B.Copy();

		List<Vector2D> linearPoints = new List<Vector2D>();
		int loopCount = 0;
		while ((Vector2D.Distance (startPoint, endPoint) > minVertexDistance)) {
			linearPoints.Add (startPoint.Copy());
			float direction = (float)Vector2D.Atan2 (endPoint, startPoint);
			startPoint.Push (direction, minVertexDistance);

			loopCount ++;
			if (loopCount > 150) {
				break;
			}
		}

		linearPoints.Add (endPoint.Copy());

		return(linearPoints);
	}
}
