using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DAutoComplete {

	static public Pair2D GetPair(Pair2D pair, float distance) {
		double direction = Vector2D.Atan2(pair.A, pair.B);
		Vector2D resultA = pair.A;
		Vector2D resultB = pair.B;
		
		Vector2D pointA = pair.A.Copy();
		Vector2D pointB = pair.B.Copy();

		pointA.Push(direction, distance);
		pointB.Push(direction, -distance);

		Slicer2D slicerA = Slicer2D.PointInSlicerComponent(pair.A);
		Slicer2D slicerB = Slicer2D.PointInSlicerComponent(pair.B);

		Pair2D thresholdPairA = new Pair2D(pair.A, pointA);
		Pair2D thresholdPairB = new Pair2D(pair.B, pointB);

		if (slicerA != null) {
			List<Vector2D> pointsA = slicerA.shape.GetWorld().GetListLineIntersectPoly(thresholdPairA);

			if (pointsA.Count > 0) {
				pointsA = Vector2DList.GetListSortedToPoint(pointsA, pointA);

				resultA = pointsA[pointsA.Count - 1];
				resultA.Push(direction, 0.05f);
			}
		}

		if (slicerB != null) {
			List<Vector2D> pointsB = slicerB.shape.GetWorld().GetListLineIntersectPoly(thresholdPairB);

			if (pointsB.Count > 0) {
				pointsB = Vector2DList.GetListSortedToPoint(pointsB, pointB);

				resultB = pointsB[pointsB.Count - 1];
				resultB.Push(direction, -0.05f);
			}
		}
		return(new Pair2D(resultA, resultB));
	}

	static public List<Vector2D> GetPoints(List<Vector2D> list, float distance) {
		if (list.Count < 2) {
			return(list);
		}

		List<Vector2D> result = new List<Vector2D>(list);

		double directionA = Vector2D.Atan2(list[0], list[1]);
		double directionB = Vector2D.Atan2(list[list.Count - 2], list[list.Count - 1]);
		
		Vector2D pointA = list[0].Copy();
		Vector2D pointB = list[list.Count - 1].Copy();

		pointA.Push(directionA, distance);
		pointB.Push(directionB, -distance);

		Slicer2D slicerA = Slicer2D.PointInSlicerComponent(list[0]);
		Slicer2D slicerB = Slicer2D.PointInSlicerComponent(list[list.Count - 1]);

		Pair2D thresholdPairA = new Pair2D(list[0], pointA);
		Pair2D thresholdPairB = new Pair2D(list[list.Count - 1], pointB);

		Vector2D resultA = null;
		Vector2D resultB = null;

		if (slicerA != null) {
			List<Vector2D> pointsA = slicerA.shape.GetWorld().GetListLineIntersectPoly(thresholdPairA);

			if (pointsA.Count > 0) {
				pointsA = Vector2DList.GetListSortedToPoint(pointsA, pointA);

				resultA = pointsA[pointsA.Count - 1];
				resultA.Push(directionA, 0.05f);
			}
		}

		if (slicerB != null) {
			List<Vector2D> pointsB = slicerB.shape.GetWorld().GetListLineIntersectPoly(thresholdPairB);

			if (pointsB.Count > 0) {
				pointsB = Vector2DList.GetListSortedToPoint(pointsB, pointB);

				resultB = pointsB[pointsB.Count - 1];
				resultB.Push(directionB, -0.05f);
			}
		}

		if (resultA != null) {
			result.Insert(0, resultA);
		}

		if (resultB != null) {
			result.Add(resultB);
		}

		return(result);
	}	
}
