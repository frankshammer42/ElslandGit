using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DComplexTrackedController : MonoBehaviour {
	public ComplexSlicerTracker trackerObject = new ComplexSlicerTracker();
	public float lineWidth = 0.25f;

	private Mesh mesh;
	static private SmartMaterial material;

	public Material GetStaticMaterial() {
		if (material == null) {
			material = MaterialManager.GetVertexLitCopy();
			material.SetColor(Color.black);
		}
		return(material.material);
	}

	void Update () {
		trackerObject.Update(transform.position);

		mesh = Slicer2DVisualsMesh.Complex.GenerateTrackerMesh(trackerObject.trackerList, transform, lineWidth, transform.position.z + 0.001f);

		//Max2D.SetColor (Color.black);
		Max2DMesh.Draw(mesh, GetStaticMaterial());
	}

}