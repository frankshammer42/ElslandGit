using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ComplexSlicerTracker {
	public Dictionary<Slicer2D, SlicerTrackerObject> trackerList = new Dictionary<Slicer2D, SlicerTrackerObject>();

	public void Update(Vector2 position, float minVertexDistance = 1f) {
		List<Slicer2D> slicer2DList = Slicer2D.GetListCopy();

		Vector2D trackedPos;

		foreach(Slicer2D slicer in slicer2DList) {

			SlicerTrackerObject tracker = null;
			trackerList.TryGetValue(slicer, out tracker);
			
			if (tracker == null) {
				tracker = new SlicerTrackerObject();
				trackerList.Add(slicer, tracker);
			}

			trackedPos = new Vector2D(slicer.transform.transform.InverseTransformPoint(position));

			if (tracker.lastPosition != null) {
				if (slicer.shape.GetLocal().PointInPoly(trackedPos)) {
					if (tracker.tracking == false) {
						tracker.pointsList.Add(tracker.lastPosition);
					}

					tracker.tracking = true;

					if (tracker.pointsList.Count < 1 || (Vector2D.Distance (trackedPos, tracker.pointsList.Last ()) > minVertexDistance / 4f)) {
						tracker.pointsList.Add(trackedPos);
					}

				} else if (tracker.tracking == true) {
					tracker.tracking = false;
					tracker.pointsList.Add(trackedPos);

					List<Vector2D> slicePoints = new List<Vector2D>();
					foreach(Vector2D point in tracker.pointsList) {
						slicePoints.Add(new Vector2D(slicer.transform.TransformPoint(point.ToVector2())));
					}

					Slice2D slice = slicer.ComplexSlice(slicePoints);
					if (slice.GetGameObjects().Count > 0) {
						CopyTracker(slice, slicer);
					};

					trackerList.Remove(slicer);
				}
			}

			if (tracker != null) {
				tracker.lastPosition = trackedPos;
			}
		}
	}

	public void CopyTracker(Slice2D slice, Slicer2D slicer) {
		foreach(Slicer2DComplexTrackedController trackerComponent in Object.FindObjectsOfType<Slicer2DComplexTrackedController>()) {
			if (trackerComponent.trackerObject == this) {
				continue;
			}

			Dictionary<Slicer2D, SlicerTrackerObject> list = new Dictionary<Slicer2D, SlicerTrackerObject>(trackerComponent.trackerObject.trackerList);
			foreach(KeyValuePair<Slicer2D, SlicerTrackerObject> pair in list) {
				if (pair.Key != slicer) {
					continue;
				}
				foreach(GameObject g in slice.GetGameObjects()){
					Slicer2D newSlicer = g.GetComponent<Slicer2D>();
					
					SlicerTrackerObject t = null;
					trackerList.TryGetValue(newSlicer, out t);

					if (t == null) {
						t = new SlicerTrackerObject();
						
						t.pointsList = new List<Vector2D>(pair.Value.pointsList);
						t.tracking = true;

						trackerComponent.trackerObject.trackerList.Add(newSlicer, t);
					}
				}
			}
		}
	}
}