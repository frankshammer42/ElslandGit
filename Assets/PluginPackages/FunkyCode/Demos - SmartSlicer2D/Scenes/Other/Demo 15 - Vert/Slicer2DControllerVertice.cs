using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slicer2DControllerVertice : MonoBehaviour {
	// Controller Visuals
	public Slicer2DVisuals visuals = new Slicer2DVisuals();

	public Slicer2D target = null;
	public int verticeID = 0;

	// Physics Force
	public bool addForce = true;
	public float addForceAmount = 5f;

	// Mouse Events
	private Pair2D linearPair = new Pair2D(Vector2D.Zero(), Vector2D.Zero());

	// Input
	public Slicer2DInputController input = new Slicer2DInputController();

	public void Start() {
		visuals.Initialize(gameObject);
	}

	public void Update() {
		input.Update();
	
		if (visuals.drawSlicer == false) {
			return;
		}

		if (linearPair.A.ToVector2() == Vector2.zero && linearPair.B.ToVector2() == Vector2.zero) {
			return;
		}
		
		visuals.Clear();
		visuals.GenerateLinearMesh(linearPair, transform);
		visuals.Draw();

	
		if (target != null) {
			Polygon2D poly = target.shape.GetWorld();

			int pointIDA = ((verticeID - 1) + poly.pointsList.Count) % poly.pointsList.Count;
			int pointIDB = verticeID;
			int pointIDC = (verticeID + 1) % poly.pointsList.Count;

			Vector2 pointA = poly.pointsList[pointIDA].ToVector2();
			Vector2 pointB = poly.pointsList[pointIDB].ToVector2();
			Vector2 pointC = poly.pointsList[pointIDC].ToVector2();

			double angle = Math2D.FindAngle(pointA, pointB, pointC);

			Vector2D offset = new Vector2D(pointB);

			double angleZero = Vector2D.Atan2(new Vector2D(pointA), new Vector2D(pointB));

			Debug.Log(angle * Mathf.Rad2Deg);

			offset.Push(-angle / 2 + angleZero, 0.5f);

			linearPair.A = offset;
		}
	
		if (Input.GetMouseButtonDown(1)) {
			Vector2D point = input.GetInputPosition(0);

			if (target != null) {
				Polygon2D poly = target.shape.GetWorld();
				if (poly.PointInPoly(point) == false) {
					target = null;

					linearPair.A = Vector2D.Zero();
					linearPair.B = Vector2D.Zero();
				}
			}

			foreach(Slicer2D slicer in Slicer2D.GetList()) {
				Polygon2D poly = slicer.shape.GetWorld();
				if (poly.PointInPoly(point)) {
					
					int id = 0;
					double distance = 1000000;

					foreach(Vector2D p in poly.pointsList) {
						double newDistance = Vector2D.Distance(p, point); 
						if (newDistance < distance) {
							distance = newDistance;
							id = poly.pointsList.IndexOf(p);
						}
					}		

					verticeID = id;
					target = slicer;

					break;
				}
			}
		}
	}

	// Checking mouse press and release events to get linear slices based on input
	public void LateUpdate()  {
		Vector2D pos = new Vector2D(input.GetInputPosition(0));
		
		if (input.GetInputPressed(0)) {
			linearPair.B.Set (pos);
		}

		if (input.GetInputReleased(0)) {
			LinearSlice (linearPair);
		}

		if (input.GetInputPressed(0) == false) {

			linearPair.B.Set (pos);
		}
	}

	private void LinearSlice(Pair2D slice) {
		List<Slice2D> results = Slicer2D.LinearSliceAll (slice, null);

		if (addForce == false) {
			return;
		}

		// Adding Physics Forces
		float sliceRotation = (float)Vector2D.Atan2 (slice.B, slice.A);
		foreach (Slice2D id in results) {
			foreach (GameObject gameObject in id.GetGameObjects()) {
				Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
				if (rigidBody2D == null) {
					continue;
				}
				foreach (Vector2D p in id.GetCollisions()) {
					Physics2DHelper.AddForceAtPosition(rigidBody2D, new Vector2 (Mathf.Cos (sliceRotation) * addForceAmount, Mathf.Sin (sliceRotation) * addForceAmount), p.ToVector2());
				}
			}
		}
	}
}


