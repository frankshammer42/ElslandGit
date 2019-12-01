using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThinSlicerParticles {

	static public void Create() {
		if (Slicer2DController.Get().linearControllerObject.startedSlice == false) {
			return;
		}

		List<Vector2D> points = Slicer2DLinearControllerObject.GetLinearVertices(Slicer2DController.Get().linearControllerObject.GetPair(0),  Slicer2DController.Get().linearControllerObject.minVertexDistance);
		
		if (points.Count < 3) {
			return;
		}

		Max2DParticles.CreateSliceParticles(points);

		float size = 0.5f;
		Vector2 f = points.First().ToVector2();
		f.x -= size / 2;
		f.y -= size / 2;

		List<Vector2D> list = new List<Vector2D>();
		list.Add( new Vector2D (f.x, f.y));
		list.Add( new Vector2D (f.x + size, f.y));
		list.Add( new Vector2D (f.x + size, f.y + size));
		list.Add( new Vector2D (f.x, f.y + size));
		list.Add( new Vector2D (f.x, f.y));

		Max2DParticles.CreateSliceParticles(list).stripped = false;

		f = points.Last().ToVector2();
		f.x -= size / 2;
		f.y -= size / 2;

		list = new List<Vector2D>();
		list.Add( new Vector2D (f.x, f.y));
		list.Add( new Vector2D (f.x + size, f.y));
		list.Add( new Vector2D (f.x + size, f.y + size));
		list.Add( new Vector2D (f.x, f.y + size));
		list.Add( new Vector2D (f.x, f.y));
		
		Max2DParticles.CreateSliceParticles(list).stripped = false;
	}
}
