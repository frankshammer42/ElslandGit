using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialSettings {
	public PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced;
	public Material material;
	public Vector2 scale = new Vector2(1, 1);
	public Vector2 offset = Vector2.zero;
	public bool batchMaterial = false;

	public void CreateMesh(GameObject gameObject, Polygon2D polygon) {
		polygon.CreateMesh (gameObject, scale, offset, GetTriangulation());

		if (material) {
			MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer> ();
			if (Slicer2DSettings.GetBatching(batchMaterial)) {
				Slicer2DProfiler.IncBatchingApplied();
				meshRenderer.sharedMaterial = material;
			} else {
				meshRenderer.sharedMaterial = new Material(material);
			}
		}
	}

	public PolygonTriangulator2D.Triangulation GetTriangulation() {
		return(Slicer2DSettings.GetTriangulation(triangulation));
	}
}
