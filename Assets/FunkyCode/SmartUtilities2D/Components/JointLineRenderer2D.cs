using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class JointLineRenderer2D : MonoBehaviour {
	public bool customColor = false;
	public Color color = Color.white;
	public float lineWidth = 1;

	private List<Joint2D> joints = new List<Joint2D>();

	private SmartMaterial material = null;
	private static SmartMaterial staticMaterial = null;

	const float lineOffset = -0.001f;

	public SmartMaterial GetMaterial() {
		if (material == null || material.material == null) {
			material = MaterialManager.GetVertexLitCopy();
		}

		return(material);
	}

	public SmartMaterial GetStaticMaterial() {
		if (staticMaterial == null || staticMaterial.material == null)   {
			staticMaterial = MaterialManager.GetVertexLitCopy();
			staticMaterial.SetColor(Color.black);
		}
		
		return(staticMaterial);
	}

	public void Start() {
		joints = Joint2D.GetJoints(gameObject);
	}

	public void Update() {
		foreach(Joint2D joint in joints) {
			if (joint.gameObject == null) {
				continue;
			}
			if (joint.anchoredJoint2D == null) {
				continue;
			}
			if (joint.anchoredJoint2D.isActiveAndEnabled == false) {
				continue;
			}

			Vector2D originPoint = new Vector2D (transform.TransformPoint (joint.anchoredJoint2D.anchor));
			Vector2D connectedPoint = Vector2D.Zero();
			if (joint.anchoredJoint2D.connectedBody != null) {
				switch (joint.jointType) {
					case Joint2D.Type.HingeJoint2D:
					 	connectedPoint = new Vector2D (joint.anchoredJoint2D.connectedBody.transform.TransformPoint (Vector2.zero));
						break;

					default:
						connectedPoint = new Vector2D (joint.anchoredJoint2D.connectedBody.transform.TransformPoint (joint.anchoredJoint2D.connectedAnchor));
						break;
				}
			}

			Pair2D pair = new Pair2D(originPoint, connectedPoint);
			Draw(pair);
		}
	}

	public void Draw(Pair2D pair) {
		Mesh2DMesh trianglesList = new Mesh2DMesh();
		trianglesList.Add(Max2DMesh.CreateLine(pair, new Vector3(1, 1, 1), lineWidth, transform.position.z + lineOffset));
		Mesh mesh = Max2DMesh.Export(trianglesList);

		if (customColor) {
			material.SetColor(color);
		
			Max2DMesh.Draw(mesh, GetMaterial().material);
		} else {
			Max2DMesh.Draw(mesh, GetStaticMaterial().material);
		}
	}
}