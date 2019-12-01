using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ComplexSlicerTrail {
	public Dictionary<Slicer2D, SlicerTrailObject> trailList = new Dictionary<Slicer2D, SlicerTrailObject>();

	public List<Slice2D> Update(Vector2D position, float timer, Slice2DLayer layer) {
		List<Slice2D> result = new List<Slice2D>();

		foreach(Slicer2D slicer in Slicer2D.GetListCopy()) {
			if (slicer.MatchLayers (layer) == false) {
					
                continue;
            }

			SlicerTrailObject trail = null;
			trailList.TryGetValue(slicer, out trail);
			
			if (trail == null) {
				trail = new SlicerTrailObject();
				trailList.Add(slicer, trail);
			}

			if (trail.lastPosition != null) {
				if (Vector2D.Distance(trail.lastPosition, position) > 0.05f) {
					trail.pointsList.Add(new TrailPoint(position, timer));
				}
			} else {
				trail.pointsList.Add(new TrailPoint(position, timer));
			}

			if (trail.pointsList.Count > 1) {
				foreach(TrailPoint trailPoint in new List<TrailPoint>(trail.pointsList)) {
					if (trailPoint.Update() == false) {
						trail.pointsList.Remove(trailPoint);
					}
				}

				List<Vector2D> points = new List<Vector2D>();
				foreach(TrailPoint trailPoint in trail.pointsList) {
					points.Add(trailPoint.position);
				}

				Slicer2D.complexSliceType = Slicer2D.SliceType.Regular;
				Slice2D slice = slicer.ComplexSlice(points);
				if (slice.GetGameObjects().Count > 0) {
					trailList.Remove(slicer);

					result.Add(slice);
				};
			}

			trail.lastPosition = position;
		}

		return(result);
	}
}