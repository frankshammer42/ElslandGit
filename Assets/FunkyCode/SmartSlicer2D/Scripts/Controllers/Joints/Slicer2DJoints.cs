using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DJoints {

	
	static public void LinearSliceJoints(Pair2D slice) {
		foreach(Joint2D joint in Joint2D.GetJointsConnected()) {
			Vector2 localPosA = joint.anchoredJoint2D.connectedAnchor;
			Vector2 worldPosA = joint.anchoredJoint2D.connectedBody.transform.TransformPoint(localPosA);
			Vector2 localPosB = joint.anchoredJoint2D.anchor;
			Vector2 worldPosB = joint.anchoredJoint2D.transform.TransformPoint(localPosB);

			switch (joint.jointType) {
				case Joint2D.Type.HingeJoint2D:
					worldPosA = joint.anchoredJoint2D.connectedBody.transform.position;
					break;
				default:
					break;
			}
			
			Pair2D jointLine = new Pair2D(worldPosA, worldPosB);

			if (Math2D.LineIntersectLine(slice, jointLine)) {
				UnityEngine.Object.Destroy(joint.anchoredJoint2D);
			}
		}
	}

	static public void ComplexSliceJoints(List<Vector2D> slice) {
		foreach(Joint2D joint in Joint2D.GetJointsConnected()) {
			Vector2 localPosA = joint.anchoredJoint2D.connectedAnchor;
			Vector2 worldPosA = joint.anchoredJoint2D.connectedBody.transform.TransformPoint(localPosA);
			Vector2 localPosB = joint.anchoredJoint2D.anchor;
			Vector2 worldPosB = joint.anchoredJoint2D.transform.TransformPoint(localPosB);

			switch (joint.jointType) {
				case Joint2D.Type.HingeJoint2D:
					worldPosA = joint.anchoredJoint2D.connectedBody.transform.position;
					break;
				default:
					break;
			}

			Pair2D jointLine = new Pair2D(worldPosA, worldPosB);

			foreach(Pair2D pair in Pair2D.GetList(slice, false)) {
				if (Math2D.LineIntersectLine(pair, jointLine)) {
					UnityEngine.Object.Destroy(joint.anchoredJoint2D);
				}
			}
		}	
	}
}
