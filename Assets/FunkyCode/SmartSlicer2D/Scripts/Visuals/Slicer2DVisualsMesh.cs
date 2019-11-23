using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Slicer2DLineEndingType {Square, Circle}

public class Slicer2DVisualsMesh {

	static public void GenerateSquare(Mesh2DMesh trianglesList, Vector2D point, float size, Transform transform, float width, float z, Slicer2DLineEndingType endingType, int edges) {
		if (endingType == Slicer2DLineEndingType.Square) {
			
			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(point.x - size, point.y - size), new Vector2D(point.x + size, point.y - size)), transform.localScale, width, z));
			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(point.x - size, point.y - size), new Vector2D(point.x - size, point.y + size)), transform.localScale, width, z));
			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(point.x + size, point.y + size), new Vector2D(point.x + size, point.y - size)), transform.localScale, width, z));
			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(point.x + size, point.y + size), new Vector2D(point.x - size, point.y + size)), transform.localScale, width, z));
		
		} else {
			float step = 360f / edges;

			for(int i = 0; i < edges; i++) {
				float x0 = Mathf.Cos((i - 1) * step * Mathf.Deg2Rad) * size;
				float y0 =  Mathf.Sin((i - 1) * step * Mathf.Deg2Rad) * size;
				float x1 = Mathf.Cos(i * step * Mathf.Deg2Rad) * size;
				float y1 =  Mathf.Sin(i * step * Mathf.Deg2Rad) * size;

				trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(point.x + x0, point.y + y0), new Vector2D(point.x + x1, point.y + y1)), transform.localScale, width, z));
			}	
		}
	}

	public class Complex {
		static public Mesh GenerateMesh(List<Vector2D> complexSlicerPointsList, Transform transform, float lineWidth, float minVertexDistance, float zPosition, float squareSize, float lineEndWidth, float vertexSpace, Slicer2DLineEndingType endingType, int edges) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			float size = squareSize;
			
			Vector2D vA, vB;
			List<Pair2D> list = Pair2D.GetList(complexSlicerPointsList, false);
			foreach(Pair2D pair in list) {
				vA = new Vector2D (pair.A);
				vB = new Vector2D (pair.B);

				vA.Push (Vector2D.Atan2 (pair.A, pair.B), -minVertexDistance * vertexSpace);
				vB.Push (Vector2D.Atan2 (pair.A, pair.B), minVertexDistance * vertexSpace);

				trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(vA, vB), transform.localScale, lineWidth, zPosition));
			}

			Pair2D linearPair = Pair2D.Zero();
			linearPair.A = new Vector2D(complexSlicerPointsList.First());
			linearPair.B = new Vector2D(complexSlicerPointsList.Last());

			GenerateSquare(trianglesList, linearPair.A, size, transform, lineEndWidth, zPosition, endingType, edges);

			GenerateSquare(trianglesList, linearPair.B, size, transform, lineEndWidth, zPosition, endingType, edges);

			return(Max2DMesh.Export(trianglesList));
		}

		static public Mesh GenerateCutMesh(List<Vector2D> complexSlicerPointsList, float cutSize, Transform transform, float lineWidth, float zPosition) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			ComplexCut complexCutLine = ComplexCut.Create(complexSlicerPointsList, cutSize);
			foreach(Pair2D pair in Pair2D.GetList(complexCutLine.GetPointsList(), true)) {
				trianglesList.Add(Max2DMesh.CreateLine(pair, transform.localScale, lineWidth, zPosition));
			}

			return(Max2DMesh.Export(trianglesList));
		}

		static public Mesh GenerateTrailMesh(Vector2D pos, Dictionary<Slicer2D, SlicerTrailObject>  trailList, Transform transform, float lineWidth, float zPosition, float squareSize) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			foreach(KeyValuePair<Slicer2D, SlicerTrailObject> s in trailList) {
				if (s.Key != null) {

					List<Vector2D> points = new List<Vector2D>();
					foreach(TrailPoint trailPoint in s.Value.pointsList) {
						points.Add(trailPoint.position);
					}
					foreach(Pair2D pair in Pair2D.GetList(points, false)) {
						trianglesList.Add(Max2DMesh.CreateLine(pair, new Vector3(1, 1, 1), lineWidth, zPosition));
					}
				}
			}

			return(Max2DMesh.Export(trianglesList));
		}
		
		static public Mesh GenerateTrackerMesh(Vector2D pos, Dictionary<Slicer2D, SlicerTrackerObject> trackerList, Transform transform, float lineWidth, float zPosition, float squareSize, Slicer2DLineEndingType endingType, int edges) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			float size = squareSize;

			GenerateSquare(trianglesList, pos, size, transform, lineWidth, zPosition, endingType, edges);

			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(pos, pos), transform.localScale, lineWidth, zPosition));

			foreach(KeyValuePair<Slicer2D, SlicerTrackerObject> trackerPair in trackerList) {
				SlicerTrackerObject tracker = trackerPair.Value;
				if (trackerPair.Key != null && tracker.tracking) {
					List<Vector2D> pointListWorld = Vector2DList.PointsToWorldSpace(trackerPair.Key.transform, tracker.pointsList);

					pointListWorld.Add(pos);

					List<Pair2D> pairList = Pair2D.GetList(pointListWorld, false);

					foreach(Pair2D pair in pairList) {
						trianglesList.Add(Max2DMesh.CreateLine(pair, transform.localScale, lineWidth, zPosition));
					}
				}
			}

			return(Max2DMesh.Export(trianglesList));
		}

		// Duplicate
		static public Mesh GenerateTrackerMesh(Dictionary<Slicer2D, SlicerTrackerObject> trackerList, Transform transform, float lineWidth, float zPosition) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			foreach(KeyValuePair<Slicer2D, SlicerTrackerObject> trackerPair in trackerList) {
				SlicerTrackerObject tracker = trackerPair.Value;
				if (trackerPair.Key != null && tracker.tracking) {
					List<Vector2D> pointListWorld = Vector2DList.PointsToWorldSpace(trackerPair.Key.transform, tracker.pointsList);
					pointListWorld.Add(new Vector2D(transform.TransformPoint(Vector2.zero)));

					List<Pair2D> pairList = Pair2D.GetList(pointListWorld, false);

					foreach(Pair2D pair in pairList) {
						trianglesList.Add(Max2DMesh.CreateLine(pair, transform.localScale, lineWidth, zPosition));
					}
				}
			}

			return(Max2DMesh.Export(trianglesList));
		}
	}

	public class Linear {

		static public Mesh GenerateMesh(Pair2D linearPair, Transform transform, float lineWidth, float zPosition, float size, float lineEndWidth, Slicer2DLineEndingType endingType, int edges) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			trianglesList.Add(Max2DMesh.CreateLine(linearPair, transform.localScale, lineWidth, zPosition));

			GenerateSquare(trianglesList, linearPair.A, size, transform, lineEndWidth, zPosition, endingType, edges);

			GenerateSquare(trianglesList, linearPair.B, size, transform, lineEndWidth, zPosition, endingType, edges);

			return(Max2DMesh.Export(trianglesList));
		}

		static public Mesh GenerateCutMesh(Pair2D linearPair, float cutSize, Transform transform, float lineWidth, float zPosition) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			LinearCut linearCutLine = LinearCut.Create(linearPair, cutSize);
			foreach(Pair2D pair in Pair2D.GetList(linearCutLine.GetPointsList(), true)) {
				trianglesList.Add(Max2DMesh.CreateLine(pair, transform.localScale, lineWidth, zPosition));
			}

			return(Max2DMesh.Export(trianglesList));
		}
			
		static public Mesh GenerateTrackerMesh(Vector2D pos, Dictionary<Slicer2D, SlicerTrackerObject> trackerList, Transform transform, float lineWidth, float zPosition, float size, Slicer2DLineEndingType endingType, int edges) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			GenerateSquare(trianglesList, pos, size, transform, lineWidth, zPosition, endingType, edges);

			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(pos, pos), transform.localScale, lineWidth, zPosition));

			foreach(KeyValuePair<Slicer2D, SlicerTrackerObject> trackerPair in trackerList) {
				SlicerTrackerObject tracker = trackerPair.Value;
				if (trackerPair.Key != null && tracker.tracking) {
					foreach(Pair2D pair in Pair2D.GetList(Vector2DList.PointsToWorldSpace(trackerPair.Key.transform, tracker.GetLinearPoints()), false)) {
						trianglesList.Add(Max2DMesh.CreateLine(pair, transform.localScale, lineWidth, zPosition));
					}
				}
			}

			return(Max2DMesh.Export(trianglesList));
		}

		static public Mesh GenerateTrackerMesh(Dictionary<Slicer2D, SlicerTrackerObject> trackerList, Transform transform, float lineWidth, float zPosition) {
			Mesh2DMesh trianglesList = new Mesh2DMesh();

			foreach(KeyValuePair<Slicer2D, SlicerTrackerObject> trackerPair in trackerList) {
				SlicerTrackerObject tracker = trackerPair.Value;
				if (trackerPair.Key != null && tracker.tracking) {
					if (tracker.firstPosition == null || tracker.lastPosition == null) {
						continue;
					}
					List<Vector2D> points = Vector2DList.PointsToWorldSpace(trackerPair.Key.transform, tracker.GetLinearPoints());
					foreach(Pair2D pair in Pair2D.GetList(points, false)) {
						trianglesList.Add(Max2DMesh.CreateLine(pair, transform.localScale, lineWidth, zPosition));
					}
				}
			}

			return(Max2DMesh.Export(trianglesList));
		}
	}

	



	
	
	static public Mesh GeneratePolygonMesh(Vector2D pos, Polygon2D.PolygonType polygonType, float polygonSize, float minVertexDistance, Transform transform, float lineWidth, float zPosition) {
		Mesh2DMesh trianglesList = new Mesh2DMesh();

		Polygon2D slicePolygon = Polygon2D.Create (polygonType, polygonSize).ToOffset(pos);

		Vector2D vA, vB;
		foreach(Pair2D pair in Pair2D.GetList(slicePolygon.pointsList, true)) {
			vA = new Vector2D (pair.A);
			vB = new Vector2D (pair.B);

			vA.Push (Vector2D.Atan2 (pair.A, pair.B), -minVertexDistance / 5);
			vB.Push (Vector2D.Atan2 (pair.A, pair.B), minVertexDistance / 5);

			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(vA, vB), transform.localScale, lineWidth, zPosition));
		}

		return(Max2DMesh.Export(trianglesList));
	}

	static public Mesh GenerateCreateMesh(Vector2D pos, Polygon2D.PolygonType polygonType, float polygonSize, Slicer2DCreateControllerObject.CreateType createType, List<Vector2D> complexSlicerPointsList, Pair2D linearPair, float minVertexDistance, Transform transform, float lineWidth, float zPosition, float squareSize, Slicer2DLineEndingType endingType, int edges) {
		Mesh2DMesh trianglesList = new Mesh2DMesh();

		float size = squareSize;

		if (createType == Slicer2DCreateControllerObject.CreateType.Slice) {
			if (complexSlicerPointsList.Count > 0) {
				linearPair.A = new Vector2D(complexSlicerPointsList.First());
				linearPair.B = new Vector2D(complexSlicerPointsList.Last());

				GenerateSquare(trianglesList, linearPair.A, size, transform, lineWidth, zPosition, endingType, edges);

				GenerateSquare(trianglesList, linearPair.B, size, transform, lineWidth, zPosition, endingType, edges);

				Vector2D vA, vB;
				foreach(Pair2D pair in Pair2D.GetList(complexSlicerPointsList, true)) {
					vA = new Vector2D (pair.A);
					vB = new Vector2D (pair.B);

					vA.Push (Vector2D.Atan2 (pair.A, pair.B), -minVertexDistance / 5);
					vB.Push (Vector2D.Atan2 (pair.A, pair.B), minVertexDistance / 5);

					trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(vA, vB), transform.localScale, lineWidth, zPosition));
				}
			}
		} else {
			Polygon2D poly = Polygon2D.Create(polygonType, polygonSize).ToOffset(pos);

			Vector2D vA, vB;
			foreach(Pair2D pair in Pair2D.GetList(poly.pointsList, true)) {
				vA = new Vector2D (pair.A);
				vB = new Vector2D (pair.B);

				vA.Push (Vector2D.Atan2 (pair.A, pair.B), -minVertexDistance / 5);
				vB.Push (Vector2D.Atan2 (pair.A, pair.B), minVertexDistance / 5);

				trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(vA, vB), transform.localScale, lineWidth, zPosition));
			}
		}

		return(Max2DMesh.Export(trianglesList));
	}

	static public Mesh GeneratePolygon2DMesh(Transform transform, Polygon2D polygon, float lineOffset, float lineWidth, bool connectedLine) {
		Mesh2DMesh trianglesList = new Mesh2DMesh();

		Polygon2D poly = polygon;

		foreach(Pair2D p in Pair2D.GetList(poly.pointsList, connectedLine)) {
			trianglesList.Add(Max2DMesh.CreateLine(p, transform.localScale, lineWidth, lineOffset));
		}

		foreach(Polygon2D hole in poly.holesList) {
			foreach(Pair2D p in Pair2D.GetList(hole.pointsList, connectedLine)) {
				trianglesList.Add(Max2DMesh.CreateLine(p, transform.localScale, lineWidth, lineOffset));
			}
		}
		
		return(Max2DMesh.Export(trianglesList));
	}

	static public Mesh GeneratePointMesh(Pair2D linearPair, Transform transform, float lineWidth, float zPosition) {
		Mesh2DMesh trianglesList = new Mesh2DMesh();

		trianglesList.Add(Max2DMesh.CreateLine(linearPair, transform.localScale, lineWidth, zPosition));

		return(Max2DMesh.Export(trianglesList));
	}
}
