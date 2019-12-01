using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slicer2DComplexController : MonoBehaviour {
	// Controller Visuals
	public Slicer2DVisuals visuals = new Slicer2DVisuals();

	// Physics Force
	public bool addForce = true;
	public float addForceAmount = 5f;

	// Mouse Events
	private static List<Vector2D>[] points = new List<Vector2D>[10];
	private float minVertexDistance = 1f;

	// Input
	public Slicer2DInputController input = new Slicer2DInputController();

	// Complex Slice Type
	public Slicer2D.SliceType complexSliceType = Slicer2D.SliceType.SliceHole;

	public void Start() {
		visuals.Initialize(gameObject);

		for(int id = 0; id < 10; id++) {
			points[id] = new List<Vector2D>();
		}
	}

	public void Update() {
		input.Update();

		if (visuals.drawSlicer == false) {
			return;
		}

		for(int id = 0; id < 10; id++) {
			if (points[id].Count < 1) {
				continue;
			}

			if (input.GetVisualsEnabled(id) == false) {
				continue;
			}

			visuals.GenerateComplexMesh(points[id], transform);
			visuals.Draw();
		}
	}

	// Checking mouse press and release events to get linear slices based on input
	public void LateUpdate() {
		for(int id = 0; id < 10; id++) {
			Vector2D pos = new Vector2D(input.GetInputPosition(id));

			if (input.GetInputClicked(id)) {
				points[id].Clear ();
				points[id].Add (pos);
			}

			if (input.GetInputPressed(id)) {
				Vector2D posMove = new Vector2D (points[id].Last ());
				while ((Vector2D.Distance (posMove, pos) > minVertexDistance)) {
					float direction = (float)Vector2D.Atan2 (pos, posMove);
					posMove.Push (direction, minVertexDistance);
					points[id].Add (new Vector2D (posMove));
				}
			}

			if (input.GetInputReleased(id)) {
				Slicer2D.complexSliceType = complexSliceType;

				if (input.GetSlicingEnabled(id)) {
					ComplexSlice (points[id]);
				}

				points[id].Clear();
			}

			if (input.GetInputPressed(id) == false) {
				if (points[id].Count > 0) {
					points[id].Clear();
				}
			}
		}
	}

	private void ComplexSlice(List <Vector2D> slice) {
		List<Slice2D> results = Slicer2D.ComplexSliceAll (slice, null);

		if (addForce == false) {
			return;
		}

		foreach (Slice2D id in results) {
			Slicer2DAddForce.ComplexSlice(id, addForceAmount);
		}
	}
}