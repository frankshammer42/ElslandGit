using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LinearSlicerTracker {
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
						tracker.firstPosition = tracker.lastPosition;
					}

					tracker.tracking = true;

				} else if (tracker.tracking == true) {
					tracker.tracking = false;

					if (tracker.firstPosition != null) {
						tracker.lastPosition = trackedPos;

						Pair2D slicePair = new Pair2D(new Vector2D(slicer.transform.TransformPoint(tracker.firstPosition.ToVector2())), new Vector2D(slicer.transform.TransformPoint(tracker.lastPosition.ToVector2())));

						Slice2D slice = slicer.LinearSlice(slicePair);
						if (slice.GetGameObjects().Count > 0) {
							CopyTracker(slice, slicer);
						};
					}

					trackerList.Remove(slicer);
				}
			}

			if (tracker != null) {
				tracker.lastPosition = trackedPos;
			}
		}
	}

	public void CopyTracker(Slice2D slice, Slicer2D slicer) {
		foreach(Slicer2DLinearTrackedController trackerComponent in Object.FindObjectsOfType<Slicer2DLinearTrackedController>()) {
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

						t.firstPosition = pair.Value.firstPosition;
						t.lastPosition = pair.Value.lastPosition;
						t.tracking = true;

						trackerComponent.trackerObject.trackerList.Add(newSlicer, t);
					}
				}
			}
		}
	}
}
