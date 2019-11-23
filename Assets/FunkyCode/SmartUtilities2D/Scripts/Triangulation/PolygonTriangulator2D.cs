using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PolygonTriangulator2D : MonoBehaviour {
	public enum Triangulation {Advanced, Legacy};

	public static Mesh Triangulate3D(Polygon2D polygon, float z, Vector2 UVScale, Vector2 UVOffset, Triangulation triangulation) {
		Mesh result = null;
		switch (triangulation) {
			case Triangulation.Advanced:
				Slicer2DProfiler.IncAdvancedTriangulation();

				Polygon2D newPolygon = polygon;

				List<Vector3> sideVertices = new List<Vector3>();
				List<int> sideTriangles = new List<int>();
				int vCount = 0;

				foreach(Pair2D pair in Pair2D.GetList(polygon.pointsList)) {
					Vector3 pointA = new Vector3((float)pair.A.x, (float)pair.A.y, 0);
					Vector3 pointB = new Vector3((float)pair.B.x, (float)pair.B.y, 0);
					Vector3 pointC = new Vector3((float)pair.B.x, (float)pair.B.y, 1);
					Vector3 pointD = new Vector3((float)pair.A.x, (float)pair.A.y, 1);

					sideVertices.Add(pointA);
					sideVertices.Add(pointB);
					sideVertices.Add(pointC);
					sideVertices.Add(pointD);
					
					sideTriangles.Add(vCount + 2);
					sideTriangles.Add(vCount + 1);
					sideTriangles.Add(vCount + 0);
					
					sideTriangles.Add(vCount + 0);
					sideTriangles.Add(vCount + 3);
					sideTriangles.Add(vCount + 2);

					vCount += 4;
				}

				Mesh meshA = PerformTriangulation(newPolygon, UVScale, UVOffset);

				Mesh meshB = new Mesh();
				List<Vector3> verticesB = new List<Vector3>();
				foreach(Vector3 v in meshA.vertices) {
					verticesB.Add(new Vector3(v.x, v.y, v.z + z));
				}
				meshB.vertices = verticesB.ToArray();
				meshB.triangles = meshA.triangles.Reverse().ToArray();
		
				Mesh mesh = new Mesh();
				mesh.SetVertices(sideVertices);
				mesh.SetTriangles(sideTriangles, 0);
			
				List<Vector3> vertices = new List<Vector3>();
				foreach(Vector3 v in meshA.vertices) {
					vertices.Add(v);
				}
				foreach(Vector3 v in meshB.vertices) {
					vertices.Add(v);
				}
				foreach(Vector3 v in sideVertices) {
					vertices.Add(v);
				}
				mesh.vertices = vertices.ToArray();

				List<int> triangles = new List<int>();
				foreach(int p in meshA.triangles) {
					triangles.Add(p);
				}
				int count = meshA.vertices.Count();
				foreach(int p in meshB.triangles) {
					triangles.Add(p + count);
				}
				count = meshA.vertices.Count() + meshB.vertices.Count();
				foreach(int p in sideTriangles) {
					triangles.Add(p + count);
				}
				mesh.triangles = triangles.ToArray();

				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
			
				result = mesh;

			break;
		}

		return(result);
	}

	public static Mesh Triangulate(Polygon2D polygon, Vector2 UVScale, Vector2 UVOffset, Triangulation triangulation) {
		polygon.Normalize();

		Mesh result = null;
		switch (triangulation) {
			case Triangulation.Advanced:
				Slicer2DProfiler.IncAdvancedTriangulation();

				float GC = Slicer2DSettings.GetGarbageCollector();
				if (GC > 0 & polygon.GetArea() < GC) {
					Debug.LogWarning("SmartUtilities2D: Garbage Collector Removed Object Because it was too small");
					
					return(null);
				}

				Polygon2D newPolygon = new Polygon2D(PreparePolygon.Get(polygon));
				
				if (newPolygon.pointsList.Count < 3) {
					Debug.LogWarning("SmartUtilities2D: Mesh is too small for advanced triangulation, using simplified triangulations instead (size: " + polygon.GetArea() +")");
					
					result = PerformTriangulation(polygon, UVScale, UVOffset);
				
					return(result);
				}
				
				foreach (Polygon2D hole in polygon.holesList) {
					newPolygon.AddHole(new Polygon2D(PreparePolygon.Get(hole, -1)));
				}

				result = PerformTriangulation(newPolygon, UVScale, UVOffset);

			break;

			case Triangulation.Legacy:
				Slicer2DProfiler.IncLegacyTriangulation();

				List<Vector2> list = new List<Vector2>();
				foreach(Vector2D p in polygon.pointsList) {
					list.Add(p.ToVector2());
				}
				result = Triangulator.Create(list.ToArray(), UVScale, UVOffset);

				
				return(result);
		}

		return(result);
	}

	public static Mesh PerformTriangulation(Polygon2D polygon, Vector2 UVScale, Vector2 UVOffset) {
		List<Vector2D> pointList = new List<Vector2D>(polygon.pointsList);
		
		// Optimize!!!
		Pair2D pair = new Pair2D(pointList.Last(), null);
		List<Vector2D> points = new List<Vector2D>(polygon.pointsList);

		for(int i = 0; i < points.Count; i++) {
			pair.B = points[i];

			if (points.Count < 4) {
				break;
			}

			if (Vector2D.Distance(pair.A, pair.B) < 0.005f) {
				//Debug.LogWarning("SmartUtilities2D: Polygon points are too close");
				polygon.pointsList.Remove(pair.A);
			}
			
			pair.A = pair.B;
		}

		TriangulationWrapper.Polygon poly = new TriangulationWrapper.Polygon();

		List<Vector2> pointsList = null;
		List<Vector2> UVpointsList = null;

		Vector3 v = Vector3.zero;

		foreach (Vector2D p in polygon.pointsList) {
			v.x = (float)p.x;
			v.y = (float)p.y;

			poly.outside.Add (v);
			poly.outsideUVs.Add (new Vector2(v.x / UVScale.x + .5f + UVOffset.x, v.y / UVScale.y + .5f + UVOffset.y));
		}

		foreach (Polygon2D hole in polygon.holesList) {
			pointsList = new List<Vector2>();
			UVpointsList = new List<Vector2>();
			
			foreach (Vector2D p in hole.pointsList) {
				v.x = (float)p.x;
				v.y = (float)p.y;

				pointsList.Add (v);

				UVpointsList.Add (new Vector2(v.x / UVScale.x + .5f, v.y / UVScale.y + .5f));
			}

			poly.holes.Add (pointsList);
			poly.holesUVs.Add (UVpointsList);
		}

		return(TriangulationWrapper.CreateMesh (poly));
	}
}