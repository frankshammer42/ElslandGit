using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Slicer2DMerge : MonoBehaviour {
	public GameObject sliceableA;
	public GameObject sliceableB;
	public Material OutputMaterial;

	public GameObject resultObject = null;
	
	void Update () {
		Polygon2D polygonA = Polygon2DList.CreateFromGameObject(sliceableA)[0].ToWorldSpace(sliceableA.transform);
		Polygon2D polygonB = Polygon2DList.CreateFromGameObject(sliceableB)[0].ToWorldSpace(sliceableB.transform);

		Polygon2D MergedPolygon = Merge2D.Merge(polygonA, polygonB);

		if (resultObject != null) {
			UnityEngine.Object.Destroy(resultObject);
		}

		if (MergedPolygon != null) {
			//UnityEngine.Object.Destroy(sliceableA.gameObject);
			//UnityEngine.Object.Destroy(sliceableB.gameObject);


			resultObject = new GameObject("Merged Polygon");
			MergedPolygon.CreatePolygonCollider(resultObject);

			Mesh2D mesh = resultObject.AddComponent<Mesh2D>();
			mesh.material = OutputMaterial;

			resultObject.AddComponent<ColliderLineRenderer2D>();
		}
	}
}

public class Merge2D {

	static public Polygon2D Merge(Polygon2D polyA, Polygon2D polyB) {
		
		if (Math2D.PolyCollidePoly(polyA, polyB)) {
			MergeObject mergeObject = new MergeObject(polyA, polyB);

			return(mergeObject.resultPolygon);
		}

		return(null);
	}
}

public class MergeObject {
	public Polygon2D resultPolygon = null;

	List<Vector2D> originPointList;
	List<Vector2D> targetPointList;

	Polygon2D originPolygon;
	Polygon2D targetPolygon;

	List<Pair2D> pairsOrigin;
	List<Pair2D> pairsTarget;

	Vector2D originStartingPoint = null;
	Vector2D targetStartingPoint = null;

	public void GetStartingPointOrigin() {
		originStartingPoint = null;

		// First Non Intersected Point
		foreach(Vector2D point in originPointList) {
			if (targetPolygon.PointInPoly(point) == false) {
				int id = originPointList.IndexOf(point);
				originStartingPoint = originPointList[(id + 1) % originPointList.Count];
				break;
			}
		}

		if (originStartingPoint == null) {
			Debug.Log("No Origin Starting Point Found");
		}
	}

	public void GetStartingPointTarget() {
		targetStartingPoint = null;

		// First Non Intersected Point
		foreach(Vector2D point in targetPointList) {
			if (originPolygon.PointInPoly(point) == false) {
				int id = targetPointList.IndexOf(point);
				targetStartingPoint = targetPointList[(id + 1) % targetPointList.Count];
				break;
			}
		}

		if (targetStartingPoint == null) {
			Debug.Log("No Target Starting Point Found");
		}
	}


	public MergeObject (Polygon2D origin, Polygon2D target) {
		
		origin.Normalize();
		target.Normalize();

		originPolygon = origin;
		targetPolygon = target;

		originPointList = new List<Vector2D>(origin.pointsList);
		targetPointList = new List<Vector2D>(target.pointsList);

		// Count Intersections
		int collisionCount = 0;
		bool intoSameEdge = false;

		foreach(Pair2D pair in Pair2D.GetList(originPointList)) {
			List<Vector2D> resultPoints = Math2D.GetListLineIntersectPoly(pair, target);
			if (resultPoints.Count > 0) {
				collisionCount += resultPoints.Count;
				if (resultPoints.Count == 2) {
					intoSameEdge = true;
				}
			}
		}

		if (collisionCount == 2) {
			if (intoSameEdge == true) {
			}
			
			GetStartingPointOrigin();
			GetStartingPointTarget();

			// Sort Point List
			originPointList = Vector2DList.GetListStartingPoint(originPointList, originStartingPoint);

			// 
			targetPointList = Vector2DList.GetListStartingPoint(targetPointList, targetStartingPoint);
			
			int Count = 0;
			foreach(Pair2D pair in Pair2D.GetList(targetPointList)) {
				List<Vector2D> resultPoints = Math2D.GetListLineIntersectPoly(pair, origin);
				if (resultPoints.Count > 0) {
					Count += resultPoints.Count;
					if (Count > 1) {
						targetStartingPoint = pair.B;
						continue;
					}
				}
			}

			targetPointList = Vector2DList.GetListStartingPoint(targetPointList, targetStartingPoint);

			// Create Pairs
			pairsOrigin = Pair2D.GetList(originPointList);
			pairsTarget = Pair2D.GetList(targetPointList);


			List<Vector2D> slice = new List<Vector2D>();
			bool collided = false;

			List<Vector2D> sliceEnding = new List<Vector2D>();

			foreach(Pair2D pair in pairsTarget) {
				List<Vector2D> collisions = Math2D.GetListLineIntersectPoly(pair, originPolygon);

				if (collided == false) {

					if (collisions.Count > 0) {
						collided = true;

						collisions = Vector2DList.GetListSortedToPoint(collisions, pair.B);

						slice.Add(collisions[0]);

						if (collisions.Count == 2) {
							sliceEnding.Add(pair.A);
							sliceEnding.Add(collisions[1]);
						} else if (collisions.Count > 2) {
							Debug.Log("Error");
						}

						//foreach(Vector2D p in collisions) {
						//	slice.Add(p);
						//}
					}

				} else {
					slice.Add(pair.A);

					if (collisions.Count > 0) {
						collisions = Vector2DList.GetListSortedToPoint(collisions, pair.A);


						foreach(Vector2D p in collisions) {
							slice.Add(p);
						}

						

						break;
					}	
				}
			}

			foreach(Vector2D p in sliceEnding) {
				slice.Add(p);
			}


			resultPolygon = new Polygon2D();

			collided = false;

			foreach(Pair2D pair in pairsOrigin) {
				List<Vector2D> points = Math2D.GetListLineIntersectPoly(pair, targetPolygon);

				if (collided == false) {
					resultPolygon.AddPoint(pair.A);

					if (points.Count > 0) {
						collided = true;
						resultPolygon.AddPoints(slice);
					} else {
						
					}
				} else {
					if (points.Count > 0) {
						collided = false;
					}
				}
			}

		} else if (collisionCount == 0) {
			Debug.Log("No Collisions Detected");
		} else {
			Debug.Log("Incorrect Collision Count: " + collisionCount);
		}
	}
}