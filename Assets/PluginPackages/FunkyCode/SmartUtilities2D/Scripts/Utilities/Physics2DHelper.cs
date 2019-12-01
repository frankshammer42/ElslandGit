using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2018 Unity Physics Changes
public class Physics2DHelper {
	public static void AddForceAtPosition(Rigidbody2D body, Vector2 force,  Vector2 position) {

		#if UNITY_2018_3_OR_NEWER
			body.AddForceAtPosition(force, position);
		#elif UNITY_2018_1_OR_NEWER
			body.AddForceAtPosition(force, body.gameObject.transform.InverseTransformPoint(position));
		#elif UNITY_2017_4_OR_NEWER
			body.AddForceAtPosition(force, body.gameObject.transform.InverseTransformPoint(position));
			// If body has attachment
			// body.AddForceAtPosition(force, position); 
		#else
			body.AddForceAtPosition(force, position);
		#endif
	}	
}

