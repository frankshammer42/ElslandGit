using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DAddForce {
	static Vector2 force = new Vector2();

	static Pair2D pair = new Pair2D(null, null);

	static public void LinearSlice(Slice2D slice, float forceAmount) {
		float sliceRotation = (float)Vector2D.Atan2 (slice.slice[1], slice.slice[0]);
		Rigidbody2D rigidBody2D;
		Vector2 force = new Vector2();

		foreach (GameObject gameObject in slice.GetGameObjects()) {
			rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
			if (rigidBody2D) {
				List<Vector2D> collisions = slice.GetCollisions();
				
				foreach (Vector2D p in collisions) {
					force.x = Mathf.Cos (sliceRotation) * forceAmount;
					force.y = Mathf.Sin (sliceRotation) * forceAmount;

					Physics2DHelper.AddForceAtPosition(rigidBody2D, force, p.ToVector2());
				}
			}
		}
	}

	static public void ComplexSlice(Slice2D slice, float forceAmount) {
		foreach (GameObject gameObject in slice.GetGameObjects()) {
			Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();

			List<Vector2D> collisions = slice.GetCollisions();

			if (rigidBody2D) {
				float forceVal = 2.0f / collisions.Count;

				Vector2 force = new Vector2 ();
				float sliceRotation;
				Vector2 centerPos = new Vector2();

				for(int i = 0; i < collisions.Count - 1; i++) {
					pair.A = collisions[i];
					pair.B = collisions[i + 1];

					sliceRotation = (float)Vector2D.Atan2 (pair.B, pair.A);

					force.x = Mathf.Cos (sliceRotation) * forceAmount;
					force.y = Mathf.Sin (sliceRotation) * forceAmount;

					centerPos.x = (float)(pair.A.x + pair.B.x) / 2f;
					centerPos.y = (float)(pair.A.y + pair.B.y) / 2f;

					Physics2DHelper.AddForceAtPosition(rigidBody2D, forceVal * force, centerPos);
				}
			}
		}
	}

	static public void ExplodeByPoint(Slice2D slice, float forceAmount, Vector2D point) {
		foreach (GameObject gameObject in slice.GetGameObjects()) {
			Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
			if (rigidBody2D) {

				float sliceRotation = (float)Vector2D.Atan2 (point, new Vector2D (gameObject.transform.position));
				Rect rect = Polygon2DList.CreateFromGameObject (gameObject)[0].GetBounds ();
				
				Physics2DHelper.AddForceAtPosition(rigidBody2D, new Vector2 (Mathf.Cos (sliceRotation) * forceAmount, Mathf.Sin (sliceRotation) * forceAmount), rect.center);
			}
		}
	}

	static public void ExplodeInPoint(Slice2D slice, float forceAmount, Vector2D point) {
		foreach (GameObject gameObject in slice.GetGameObjects()) {
			Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
			if (rigidBody2D) {
				float sliceRotation = (float)Vector2D.Atan2 (point, new Vector2D (gameObject.transform.position));
				Rect rect = Polygon2DList.CreateFromGameObject (gameObject)[0].GetBounds ();
				Physics2DHelper.AddForceAtPosition(rigidBody2D, new Vector2 (Mathf.Cos (sliceRotation) * forceAmount, Mathf.Sin (sliceRotation) * forceAmount), rect.center);
			}
		}
	}

	static public void LinearTrail(Slice2D slice, float forceAmount) {
		foreach (GameObject gameObject in slice.GetGameObjects()) {
			Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
			if (rigidBody2D) {
				float sliceRotation = (float)Vector2D.Atan2 (slice.slice[0], slice.slice[1]);

				List<Vector2D> collisions = slice.GetCollisions();

				foreach (Vector2D p in collisions) {
					Vector2 force = new Vector2 (Mathf.Cos (sliceRotation) * forceAmount, Mathf.Sin (sliceRotation) * forceAmount);
					Physics2DHelper.AddForceAtPosition(rigidBody2D, force, p.ToVector2());
				}
			}
		}
	}

	static public void ComplexTrail(Slice2D slice, float forceAmount) {
		Rigidbody2D rigidBody2D;
		Vector2D vec;
		float sliceRotation;
		float forceVal;
		Vector2 vecSum = Vector2.zero;

		for(int i = 0; i < slice.GetGameObjects().Count; i++) {
			rigidBody2D = slice.GetGameObjects()[i].GetComponent<Rigidbody2D> ();
			if (rigidBody2D) {
				List<Vector2D> collisions = slice.GetCollisions();

				forceVal = 2.0f / collisions.Count;
				for(int x = 0; x < collisions.Count; x ++) {
					vec = collisions[x];

					pair.A = vec;

					if (pair.A != null && pair.B != null) {
						sliceRotation = (float)Vector2D.Atan2 (pair.A, pair.B);

						force.x = Mathf.Cos (sliceRotation) * forceAmount;
						force.y = Mathf.Sin (sliceRotation) * forceAmount;

						vecSum.x = (float)(pair.A.x + pair.B.x)/ 2f;
						vecSum.y = (float)(pair.A.y + pair.B.y) / 2f;

						Physics2DHelper.AddForceAtPosition(rigidBody2D, forceVal * force, vecSum);
					}

					pair.B = pair.A;						
				}
			}
		}
	}
}
