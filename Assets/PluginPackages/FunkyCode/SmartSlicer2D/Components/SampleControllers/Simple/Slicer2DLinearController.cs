using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slicer2DLinearController : MonoBehaviour {
	// Controller Visuals
	public Slicer2DVisuals visuals = new Slicer2DVisuals();

	// Physics Force
	public bool addForce = true;
	public float addForceAmount = 5f;

	// Mouse Events
	Pair2D[] linearPair = new Pair2D[10];

	// Input
	public Slicer2DInputController input = new Slicer2DInputController();

	public void Start() {
		visuals.Initialize(gameObject);

		for(int id = 0; id < 10; id++) {
			linearPair[id] = Pair2D.Zero();
		}
	}

	public void Update() {
		input.Update();

		if (visuals.drawSlicer == false) {
			return;
		}

		visuals.Clear();

		for(int id = 0; id < 10; id++) {
			if (linearPair[id].A.ToVector2() == Vector2.zero && linearPair[id].B.ToVector2() == Vector2.zero) {
				continue;
			}

			if (input.GetVisualsEnabled(id) == false) {
				continue;
			}

			visuals.GenerateLinearMesh(linearPair[id], transform);
		}
		
		visuals.Draw();
	}

	public void OnGUI() {
		input.OnGUI();
	}

	// Checking mouse press and release events to get linear slices based on input
	public void LateUpdate()  {
		for(int id = 0; id < 10; id++) {
			Vector2D pos = new Vector2D(input.GetInputPosition(id));

			if (input.GetInputClicked(id)) {
				linearPair[id].A.Set (pos);
			}
			
			if (input.GetInputPressed(id)) {
				linearPair[id].B.Set (pos);
			}

			if (input.GetInputReleased(id)) {
				if (input.GetSlicingEnabled(id)) {
					LinearSlice (linearPair[id]);
				}
			}

			if (input.GetInputPressed(id) == false) {
				linearPair[id].A.Set (pos);
				linearPair[id].B.Set (pos);
			}
		}
	}

	private void LinearSlice(Pair2D slice) {
		List<Slice2D> results = Slicer2D.LinearSliceAll (slice, null);

		if (addForce == false) {
			return;
		}

		// Adding Physics Forces
		foreach (Slice2D id in results) {
			Slicer2DAddForce.LinearSlice(id, addForceAmount);
		}
	}
}