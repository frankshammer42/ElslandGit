using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slicer2DVisuals {

	public Slicer2DLineEndingType lineEndingType = Slicer2DLineEndingType.Square;
	public int lineEndingEdgeCount = 6;

	public bool drawSlicer = true;
	public float visualScale = 1f;
	public float lineWidth = 1.0f;
	public float lineEndWidth = 1.0f;
	public float zPosition = 0f;
	public Color slicerColor = Color.white;
	public bool lineBorder = true;
	public float lineEndSize = 0.5f;
	public float vertexSpace = 0.25f;
	public float borderScale = 2f;
	public float minVertexDistance = 1f;

	public bool customMaterial = false;
	public Material customFillMaterial;
	public Material customBoarderMaterial;

	//public bool lineEndRotation = false;

	public bool customEndingsImage = false;
	public Material customEndingImageMaterial = null;
	public List<Pair2D> customEndingsPosition = new List<Pair2D>();

	// Mesh & Material
	private List<Mesh> mesh = new List<Mesh>();
	private List<Mesh> meshBorder = new List<Mesh>();

	private static SmartMaterial fillMaterial;
	private static SmartMaterial boarderMaterial;

	// Visuals
	//GameObject visualsGameObject;
	//MeshFilter visualMeshFilter;
	//MeshRenderer visualMeshRenderer;

	public void Clear() {
		customEndingsPosition.Clear();

		if (meshBorder.Count > 0) {
			foreach(Mesh m in meshBorder) {
				UnityEngine.Object.DestroyImmediate(m);
			}
			meshBorder.Clear();
		}
		if (mesh.Count > 0) {
			foreach(Mesh m in mesh) {
				UnityEngine.Object.DestroyImmediate(m);
			}
			mesh.Clear();
		}
	}

	public void GeneratePointMesh(Pair2D pair, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.GeneratePointMesh(pair, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f));
		mesh.Add( Slicer2DVisualsMesh.GeneratePointMesh(pair, transform, lineWidth * visualScale, zPosition));
	}

	public void GenerateTrailMesh(Vector2D pos, Dictionary<Slicer2D, SlicerTrailObject> trailList, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.Complex.GenerateTrailMesh(pos, trailList, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale));
		mesh.Add( Slicer2DVisualsMesh.Complex.GenerateTrailMesh(pos, trailList, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale));
	}

	public void GenerateCreateMesh(Vector2D pos, Polygon2D.PolygonType polygonType, float polygonSize, Slicer2DCreateControllerObject.CreateType createType, List<Vector2D> pointsList, Pair2D linearPair, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.GenerateCreateMesh(pos, polygonType, polygonSize, createType, pointsList, linearPair, minVertexDistance, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize, lineEndingType, lineEndingEdgeCount));
		mesh.Add( Slicer2DVisualsMesh.GenerateCreateMesh(pos, polygonType, polygonSize, createType, pointsList, linearPair, minVertexDistance, transform, lineWidth * visualScale, zPosition, lineEndSize, lineEndingType, lineEndingEdgeCount));
	}

	public void GenerateComplexMesh(List<Vector2D> points, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.Complex.GenerateMesh(points, transform, lineWidth * visualScale * borderScale, minVertexDistance, zPosition + 0.001f, lineEndSize * visualScale,  lineEndWidth * visualScale * borderScale, vertexSpace, lineEndingType, lineEndingEdgeCount));
		mesh.Add( Slicer2DVisualsMesh.Complex.GenerateMesh(points, transform, lineWidth * visualScale, minVertexDistance, zPosition, lineEndSize * visualScale, lineEndWidth * visualScale, vertexSpace, lineEndingType, lineEndingEdgeCount));
	}

	public void GeneratePolygonMesh(Vector2D pos, Polygon2D.PolygonType polygonType, float polygonSize, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.GeneratePolygonMesh(pos, polygonType, polygonSize * visualScale, minVertexDistance, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f));
		mesh.Add( Slicer2DVisualsMesh.GeneratePolygonMesh(pos, polygonType, polygonSize * visualScale, minVertexDistance, transform, lineWidth * visualScale, zPosition));
	}
	
	public void GenerateLinearMesh(Pair2D linearPair, Transform transform) {
		if (customEndingsImage) {
			customEndingsPosition.Add(linearPair);
		}
		meshBorder.Add( Slicer2DVisualsMesh.Linear.GenerateMesh(linearPair, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale, lineEndWidth * visualScale * borderScale, lineEndingType, lineEndingEdgeCount));
		mesh.Add( Slicer2DVisualsMesh.Linear.GenerateMesh(linearPair, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale, lineEndWidth * visualScale, lineEndingType, lineEndingEdgeCount));
	}

	public void GenerateComplexTrackerMesh(Vector2D pos, Dictionary<Slicer2D, SlicerTrackerObject> trackerList, Transform transform){
		meshBorder.Add( Slicer2DVisualsMesh.Complex.GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount));
		mesh.Add( Slicer2DVisualsMesh.Complex.GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount));
	}

	public void GenerateLinearTrackerMesh(Vector2D pos, Dictionary<Slicer2D, SlicerTrackerObject> trackerList, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.Linear.GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount));
		mesh.Add( Slicer2DVisualsMesh.Linear.GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount));
	}

	public void GenerateLinearCutMesh(Pair2D linearPair, float cutSize, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.Linear.GenerateCutMesh(linearPair, cutSize * visualScale, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f));
		mesh.Add( Slicer2DVisualsMesh.Linear.GenerateCutMesh(linearPair, cutSize * visualScale, transform, lineWidth * visualScale, zPosition));
	}

	public void GenerateComplexCutMesh(List<Vector2D> pointsList, float cutSize, Transform transform) {
		meshBorder.Add( Slicer2DVisualsMesh.Complex.GenerateCutMesh(pointsList, cutSize * visualScale, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f));
		mesh.Add( Slicer2DVisualsMesh.Complex.GenerateCutMesh(pointsList, cutSize * visualScale, transform, lineWidth * visualScale, zPosition));
	}

	public void Initialize(GameObject gameObject) {
		GameObject visualsGameObject = new GameObject("Visuals");
		visualsGameObject.transform.parent = gameObject.transform;

		//visualMeshFilter = visualsGameObject.AddComponent<MeshFilter>();
		//visualMeshRenderer = visualsGameObject.AddComponent<MeshRenderer>();
	}

	public Material GetBorderMaterial() {
		if (customMaterial) {
			return(customBoarderMaterial);
		} else {
			if (boarderMaterial == null) {
				boarderMaterial = MaterialManager.GetVertexLitCopy();
			}

			boarderMaterial.SetColor(Color.black);

			return(boarderMaterial.material);
		}
	}

	public Material GetFillMaterial() {
		if (customMaterial) {
			return(customFillMaterial);
		} else {
			if (fillMaterial == null) {
				fillMaterial = MaterialManager.GetVertexLitCopy();
			}

			fillMaterial.SetColor(slicerColor);

			return(fillMaterial.material);
		}
	}

	public void Draw() {
		if (lineBorder && meshBorder.Count > 0) {
			if (meshBorder.Count > 0) {
				foreach(Mesh m in meshBorder) {
					Max2DMesh.Draw(m, GetBorderMaterial());
				}
			}
		}

		if (mesh.Count > 0) {
			foreach(Mesh m in mesh) {
				Max2DMesh.Draw(m, GetFillMaterial());
			}
		}

		if (customEndingsPosition.Count > 0) {
			Matrix4x4 matrix;		
			foreach(Pair2D pair in customEndingsPosition) {
				Polygon2D polyA = Polygon2D.CreateFromRect(new Vector2(1, 1));
				//polyA.ToOffset(pair.A);
				Mesh mA = polyA.CreateMesh(new Vector2(2, 2), Vector2.zero);
				
				matrix = Matrix4x4.TRS( pair.A.ToVector3(zPosition), Quaternion.Euler(0, 0, 0),  new Vector3(1, 1, 1));

				Graphics.DrawMesh(mA, matrix, customEndingImageMaterial, 0);

				matrix = Matrix4x4.TRS( pair.B.ToVector3(zPosition), Quaternion.Euler(0, 0, 0),  new Vector3(1, 1, 1));

				Graphics.DrawMesh(mA, matrix, customEndingImageMaterial, 0);
			}	
		}
	}
}
