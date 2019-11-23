using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LinearSlicerTrail {
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
					trail.pointsList.Insert(0, new TrailPoint(position, timer));
				}
			} else {
				trail.pointsList.Insert(0, new TrailPoint(position, timer));
			}

			foreach(TrailPoint trailPoint in new List<TrailPoint>(trail.pointsList)) {
				if (trailPoint.Update() == false) {
					trail.pointsList.Remove(trailPoint);
				}
			}

            if (trail.pointsList.Count > 1) {
                Vector2D firstPoint = null;
                Vector2D lastPoint = null;
                bool insideState = false;

                foreach(TrailPoint trailPoint in trail.pointsList) {
                    bool inside = false;
                    if (slicer.shape.GetLocal().PointInPoly(trailPoint.position.InverseTransformPoint(slicer.transform))) {
                        inside = true;
                    }

                    switch(insideState) {
                        case true:
                            // Slice!
                            if (inside == false) {
                                lastPoint = trailPoint.position;

                                insideState = false;
                                break;
                            }
                        break;

                        case false:
                            if (inside == false) {
                            // Searching For Start of Slice
                                firstPoint = trailPoint.position;
                                insideState = true;
                            }
                        break;
                    }

                    if (lastPoint != null) {
                        break;
                    }
                }

                if (firstPoint != null && lastPoint != null) {
                    Slicer2D.complexSliceType = Slicer2D.SliceType.Regular;
                    Slice2D slice = slicer.LinearSlice(new Pair2D(firstPoint, lastPoint));
                    if (slice.GetGameObjects().Count > 0) {
                        trailList.Remove(slicer);

                        result.Add(slice);
                    };
                }
            }

			trail.lastPosition = position;
		}
        return(result);
	}
}
