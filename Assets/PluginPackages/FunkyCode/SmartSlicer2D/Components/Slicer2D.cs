using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Slicer2D : MonoBehaviour {
	public enum ShapeType {Collider, SpriteCustomShape};
	public enum SliceType {Regular, SliceHole, FillSlicedHole}; // Customize More!
	public enum TextureType {Sprite, Mesh2D, Mesh3D, SpriteAnimation, ImageUI, None};
	public enum CenterOfMass {Default, RigidbodyOnly, TransformOnly};
	public enum AnchorType {AttachRigidbody, RemoveConstraints, CancelSlice, Nothing}; // DestroySlice
	public enum ColliderType {PolygonCollider2D, EdgeCollider2D};
	public enum InstantiationMethod {Quality, Performance};

	// Complex Slicer Options
	public static SliceType complexSliceType = SliceType.Regular;

	//[Tooltip("Type of texture to generate")]
	public TextureType textureType = TextureType.Sprite;

	public Slicing2DLayer slicingLayer = Slicing2DLayer.Layer1;

	public ColliderType colliderType = ColliderType.PolygonCollider2D;

	// Mesh Fields
	public MaterialSettings materialSettings = new MaterialSettings();

	public ShapeType shapeType = ShapeType.Collider;

	// Slicing Limit
	public Slicer2DLimit limit = new Slicer2DLimit();

	// Event Handling
	public Slicer2DEventHandling eventHandler = new Slicer2DEventHandling();

	public InstantiationMethod instantiateMethod = InstantiationMethod.Quality;
	
	// Mass
	public CenterOfMass centerOfMass = CenterOfMass.Default;
	public bool recalculateMass = false;

	// Anchors System
	public bool anchors = false;

	// Sprite Information
	public VirtualSpriteRenderer spriteRenderer = null;

	// Joints Support
	public bool supportJoints = false;
	private Rigidbody2D body = null;
	private List<Joint2D> joints = new List<Joint2D>();

	static private List<Slicer2D> slicer2DList = new List<Slicer2D>();
	static private List<Slicer2D> getLayerList = new List<Slicer2D>();

	// Shape API
	public Slicer2DShape shape = new Slicer2DShape();
	
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private SpriteRenderer spriteRendererComponent;

	public bool afterSliceRemoveOrigin = true;

	public void OnDestroy() {
		if (meshFilter != null) {
			if (meshFilter.sharedMesh != null) {
				DestroyImmediate(meshFilter.sharedMesh);
			}
		}
		eventHandler.ClearEvents();
	}

	private Polygon2D GetPolygonToSlice() {
		if (limit.counter >= limit.maxSlices && limit.enabled) {
			return(null);
		}

		return(shape.GetWorld());
	}

	public Rigidbody2D GetRigibody() {
		if (body == null) {
			body = GetComponent<Rigidbody2D>();
		}
		return(body); 
	}

	// Component Events
	public void AddAnchorEvent(Slicer2DEventHandling.Slice2DEvent slicerEvent) {
		eventHandler.anchorSliceEvent += slicerEvent;
	}
	
	public void AddAnchorResultEvent(Slicer2DEventHandling.Slice2DResultEvent slicerEvent) {
		eventHandler.anchorSliceResultEvent += slicerEvent;
	}

	public void AddEvent(Slicer2DEventHandling.Slice2DEvent slicerEvent) {
		eventHandler.sliceEvent += slicerEvent;
	}
	
	public void AddResultEvent(Slicer2DEventHandling.Slice2DResultEvent slicerEvent) {
		eventHandler.sliceResultEvent += slicerEvent;
	}

	// Static Events
	static public void AddGlobalAnchorEvent(Slicer2DEventHandling.Slice2DEvent slicerEvent) {
		Slicer2DEventHandling.anchorGlobalSliceEvent += slicerEvent;
	}
	
	static public void AddGlobalResultAnchorEvent(Slicer2DEventHandling.Slice2DResultEvent slicerEvent) {
		Slicer2DEventHandling.anchorGlobalSliceResultEvent += slicerEvent;
	}

	static public void AddGlobalEvent(Slicer2DEventHandling.Slice2DEvent slicerEvent) {
		Slicer2DEventHandling.globalSliceEvent += slicerEvent;
	}
	static public void AddGlobalResultEvent(Slicer2DEventHandling.Slice2DResultEvent slicerEvent) {
		Slicer2DEventHandling.globalSliceResultEvent += slicerEvent;
	}

	static public int GetListCount() {
		return(slicer2DList.Count);
	}

	static public List<Slicer2D> GetList() {
		return(slicer2DList);
	}

	static public List<Slicer2D> GetListCopy() {
		return(new List<Slicer2D>(slicer2DList));
	}

	static public List<Slicer2D> GetListLayer(Slice2DLayer layer) {
		getLayerList.Clear();

		foreach (Slicer2D id in slicer2DList) {
			if (id.MatchLayers (layer)) {
				getLayerList.Add(id);
			}
		}
		
		return(getLayerList);
	}
		
	public int GetLayerID() {
		return((int)slicingLayer);
	}

	public bool MatchLayers(Slice2DLayer sliceLayer) {
		return((sliceLayer == null || sliceLayer.GetLayerType() == Slice2DLayerType.All) || sliceLayer.GetLayerState(GetLayerID ()));
	}

	void OnEnable() {
		slicer2DList.Add (this);
	}

	void OnDisable() {
		slicer2DList.Remove (this);
	}

	// ANCHOR!!!!!!! ////////////////////
	public Collider2D[] anchorsList = new Collider2D[1];
	public AnchorType anchorType = AnchorType.AttachRigidbody;

	public List<Polygon2D> anchorPolygons = new List<Polygon2D>();
	public List<Collider2D> anchorColliders = new List<Collider2D>();
	
	////////////////////////

	void Awake() {
		Slicer2DProfiler.IncObjectsCreated();

		shape.SetSlicer2D(this);
	}

	void Start() {
		Initialize ();

		if (supportJoints == true) {
			RecalculateJoints();
		}

		StartAnchor ();
	}

	// Check Before Each Function - Then This Could Be Private
	public void Initialize() {
		shape.ForceUpdate();

		List<Polygon2D> result = Polygon2DList.CreateFromGameObject (gameObject);

		// Split collider if there are more polygons than 1
		if (result.Count > 1) {
			PerformResult(result, new Slice2D());
		}

		switch (textureType) {
			case TextureType.Mesh2D:
				if (Slicer2DSettings.GetBatching(materialSettings.batchMaterial) && spriteRenderer != null) {
					materialSettings.material = spriteRenderer.material;
				}

				// Needs Mesh UV Options
				materialSettings.CreateMesh(gameObject, shape.GetLocal());

				meshRenderer = GetComponent<MeshRenderer> ();

				meshFilter = GetComponent<MeshFilter>();
				
				break;

			case TextureType.Mesh3D:
				shape.GetLocal().CreateMesh3D (gameObject, 1, new Vector2 (1, 1), Vector2.zero, materialSettings.GetTriangulation());

				meshRenderer = GetComponent<MeshRenderer> ();
				meshRenderer.sharedMaterial = materialSettings.material;

				meshFilter = GetComponent<MeshFilter>();

				break;

			case TextureType.Sprite:
				if (spriteRenderer == null) {
					spriteRendererComponent = GetComponent<SpriteRenderer>();

					spriteRenderer = new VirtualSpriteRenderer(spriteRendererComponent);
				} else {
					if (Slicer2DSettings.GetBatching(materialSettings.batchMaterial) == false) {
						spriteRenderer.material = new Material(spriteRenderer.material);
					}
				}
				break;
			case TextureType.SpriteAnimation:
				if (spriteRenderer == null) {
					spriteRenderer = new VirtualSpriteRenderer(GetComponent<SpriteRenderer>());
				}
				break;

			default:
				break;
			}
	}
		
	public Slice2D LinearSlice(Pair2D slice, bool perform = true) {
		Slice2D slice2D = Slice2D.Create (gameObject, slice);

		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.API.LinearSlice (colliderPolygon, slice);

			if (perform) {
				PerformResult (sliceResult.GetPolygons(), sliceResult);
			}

			return(sliceResult);
		}
			
		return(slice2D);
	}

	public Slice2D LinearCutSlice(LinearCut slice) {
		Slice2D slice2D = Slice2D.Create (gameObject, slice);

		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Polygon2D slicePoly = new Polygon2D(slice.GetPointsList(1.01f));
			
			if (Math2D.PolyInPoly(slicePoly, colliderPolygon) == true) {
				Destroy (gameObject);
				return(slice2D);

			} else {
				Slice2D sliceResult = Slicer2D.API.LinearCutSlice (colliderPolygon, slice);
				
				foreach(Polygon2D poly in new List<Polygon2D> (sliceResult.GetPolygons())) {
					if (Math2D.PolyInPoly(slicePoly, poly)) {
						sliceResult.RemovePolygon(poly);
					}
				}

				PerformResult (sliceResult.GetPolygons(), sliceResult);

				return(sliceResult);
			}
		}
		
		return(Slice2D.Create (gameObject, slice));
	}
			
	public Slice2D ComplexSlice(List<Vector2D> slice) {
		Slice2D slice2D = Slice2D.Create (gameObject, slice);

		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.API.ComplexSlice (colliderPolygon, slice);
			PerformResult (sliceResult.GetPolygons(), sliceResult);

			return(sliceResult);
		}
		
		return(slice2D);
	}

	public Slice2D ComplexCutSlice(ComplexCut slice) {
		Slice2D slice2D = Slice2D.Create (gameObject, slice);

		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Polygon2D slicePoly = new Polygon2D(slice.GetPointsList(1.01f));
			
			if (Math2D.PolyInPoly(slicePoly, colliderPolygon) == true) {
				Destroy (gameObject);
				return(slice2D);

			} else {
				Slice2D sliceResult = Slicer2D.API.ComplexCutSlice (colliderPolygon, slice);
				
				foreach(Polygon2D poly in new List<Polygon2D> (sliceResult.GetPolygons())) {
					if (Math2D.PolyInPoly(slicePoly, poly)) {
						sliceResult.RemovePolygon(poly);
					}
				}

				PerformResult (sliceResult.GetPolygons(), sliceResult);

				return(sliceResult);
			}
		}
		
		return(Slice2D.Create (gameObject, slice));
	}

	public Slice2D PointSlice(Vector2D point, float rotation) {
		Slice2D slice2D = Slice2D.Create (gameObject, point, rotation);

		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.API.PointSlice (colliderPolygon, point, rotation);

			PerformResult (sliceResult.GetPolygons(), sliceResult);
			
			return(sliceResult);
		}

		return(slice2D);
	}

	public Slice2D PolygonSlice(Polygon2D slice, Polygon2D slicePolygonDestroy) {
		Slice2D slice2D = Slice2D.Create (gameObject, slice);

		if (isActiveAndEnabled == false) {
			return(slice2D);
		}
		
		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.API.PolygonSlice (colliderPolygon, slice);

			if (sliceResult.GetPolygons().Count > 0) {
				if (slicePolygonDestroy != null) {
					foreach (Polygon2D p in new List<Polygon2D>(sliceResult.GetPolygons())) {
						if (slicePolygonDestroy.PolyInPoly (p) == true) {
							sliceResult.RemovePolygon (p);
						}
					}
				}
				// Check If Slice Result Is Correct
				if (sliceResult.GetPolygons().Count > 0) {
					sliceResult.AddGameObjects (PerformResult (sliceResult.GetPolygons(), slice2D));
				} else if (slicePolygonDestroy != null) {
					Destroy (gameObject);
				}
	
				return(sliceResult);
			}
		}

		return(slice2D);
	}

	public Slice2D ExplodeByPoint(Vector2D point, int explosionSlices = 0) {
		Slice2D slice2D = Slice2D.Create (gameObject, point);
		
		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.API.ExplodeByPoint (colliderPolygon, point, explosionSlices);
			PerformResult (sliceResult.GetPolygons(), sliceResult);
			
			return(sliceResult);
		}

		return(slice2D);
	}

	public Slice2D ExplodeInPoint(Vector2D point, int explosionSlices = 0) {
		Slice2D slice2D = Slice2D.Create (gameObject, point);
		
		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.API.ExplodeInPoint (colliderPolygon, point, explosionSlices);
			PerformResult (sliceResult.GetPolygons(), sliceResult);
			
			return(sliceResult);
		}

		return(slice2D);
	}

	public Slice2D Explode(int explosionSlices = 0) {
		Slice2D slice2D = Slice2D.Create (gameObject, Slice2DType.Explode);

		if (isActiveAndEnabled == false) {
			return(slice2D);
		}

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.API.Explode (colliderPolygon, explosionSlices);
			PerformResult (sliceResult.GetPolygons(), sliceResult);
			
			return(sliceResult);
		}

		return(slice2D);
	}

	public List<GameObject> PerformResult(List<Polygon2D> result, Slice2D slice) {
		List<GameObject> resultGameObjects = new List<GameObject> ();

		if (result.Count < 1) {
			return(resultGameObjects);
		}

		slice.SetPolygons(result);

		slice.originGameObject = gameObject;

		if (eventHandler.SliceEvent(slice) == false) {
			return(resultGameObjects);
		}

		if (Slicer2DEventHandling.GlobalSliceEvent(slice) == false) {
			return(resultGameObjects);
		}

		double originArea = 1f;
		if (recalculateMass) {
			originArea = shape.GetLocal().GetArea();
		}

		Rigidbody2D originalRigidBody = gameObject.GetComponent<Rigidbody2D>();

		
		Collider2D collider2D = gameObject.GetComponent<Collider2D>();
		PhysicsMaterial2D material = collider2D.sharedMaterial;
		bool isTrigger = collider2D.isTrigger;	

		int name_id = 1;
		foreach (Polygon2D id in result) {
			GameObject gObject = null;

			switch(Slicer2DSettings.GetComponentsCopy(instantiateMethod)) {
				case InstantiationMethod.Performance: 
					Slicer2DProfiler.IncSlicesCreatedWithPerformance();

					switch (textureType) {
						case TextureType.Sprite:
							if (spriteRendererComponent) {
								DestroyImmediate(spriteRendererComponent);
								spriteRendererComponent = null;
							}
							break;
					}

					BoxCollider2D c1 = gameObject.GetComponent<BoxCollider2D>();
					if (c1 != null) {
						DestroyImmediate(c1);
					}

					CircleCollider2D c2 = gameObject.GetComponent<CircleCollider2D>();
					if (c2 != null) {
						DestroyImmediate(c2);
					}

					CapsuleCollider2D c3 = gameObject.GetComponent<CapsuleCollider2D>();
					if (c3 != null) {
						DestroyImmediate(c3);
					}

					gObject = Instantiate(gameObject);

			
				
					break;

				case InstantiationMethod.Quality: 
					Slicer2DProfiler.IncSlicesCreatedWithQuality();

					gObject = new GameObject();
					Slicer2DComponents.Copy(this, gObject);
					
					break;
			}
			
			resultGameObjects.Add (gObject);

			Slicer2D slicer = gObject.GetComponent<Slicer2D> ();
			slicer.limit.counter += 1;
			slicer.limit.maxSlices = limit.maxSlices;

			slicer.eventHandler = new Slicer2DEventHandling();

			slicer.shape = new Slicer2DShape();
			slicer.shape.SetSlicer2D(slicer);
			slicer.shape.ForceUpdate();

			gObject.name = name + " (" + name_id + ")";
			gObject.transform.parent = transform.parent;
			gObject.transform.position = transform.position;
			gObject.transform.rotation = transform.rotation;
			gObject.transform.localScale = transform.localScale;
			
			gObject.layer = gameObject.layer;
			gObject.tag = gameObject.tag;

			Slicer2DComponents.CopyRigidbody2D(originalRigidBody, slicer, id, originArea);

			if (centerOfMass == CenterOfMass.TransformOnly) {
				Polygon2D localPoly = id.ToLocalSpace (gObject.transform);
				Rect bounds = localPoly.GetBounds();
				Vector2 centerOffset = new Vector2(bounds.center.x * transform.lossyScale.x, bounds.center.y * transform.lossyScale.y);
				gObject.transform.Translate(centerOffset, 0);
			}

			Collider2D collider = null;
			switch (colliderType) {
				case ColliderType.PolygonCollider2D:
					collider = (Collider2D)id.ToLocalSpace (gObject.transform).CreatePolygonCollider (gObject);
					break;
				case ColliderType.EdgeCollider2D:
					collider = (Collider2D)id.ToLocalSpace (gObject.transform).CreateEdgeCollider (gObject);
					break;
			}

			collider.sharedMaterial = material;
			collider.isTrigger = isTrigger;

			switch (textureType) {
				case TextureType.Sprite:
					slicer.spriteRenderer = spriteRenderer;
					Polygon2D.SpriteToMesh(gObject, spriteRenderer, materialSettings.GetTriangulation());
					break;

				case TextureType.SpriteAnimation:
					slicer.textureType = TextureType.Sprite;
					Polygon2D.SpriteToMesh(gObject, spriteRenderer, materialSettings.GetTriangulation());
					break;
					
				default:
					break;
				}

			name_id += 1;
		}
			
		if (afterSliceRemoveOrigin) {	
			Destroy (gameObject);
		}

		if (resultGameObjects.Count > 0) {
			slice.originGameObject = gameObject;
			
			slice.SetGameObjects(resultGameObjects);

			if (supportJoints == true) {
				SliceJointEvent (slice);
			}

			eventHandler.Result(slice);
		}

		return(resultGameObjects);
	}
	
	static public List<Slice2D> LinearSliceAll(Pair2D slice, Slice2DLayer layer = null, bool perform = true) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		foreach (Slicer2D id in GetListLayer(layer)) {
			Slice2D sliceResult = id.LinearSlice (slice, perform);

			if (perform) {
				if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
				}
			} else {
				if (sliceResult.GetPolygons().Count > 0) {
					result.Add (sliceResult);
				}
			}
		}

		return(result);
	}
	
	static public List<Slice2D> LinearCutSliceAll(LinearCut linearCut, Slice2DLayer layer = null) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		foreach (Slicer2D id in GetListLayer(layer)) {
			Slice2D sliceResult = id.LinearCutSlice (linearCut);
			if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
			}
		}
				
		return(result);
	}

	static public List<Slice2D> ComplexSliceAll(List<Vector2D> slice, Slice2DLayer layer = null) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		foreach (Slicer2D id in GetListLayer(layer)) {
			Slice2D sliceResult = id.ComplexSlice (slice);
			if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
			}
		}
				
		return(result);
	}

	static public List<Slice2D> ComplexCutSliceAll(ComplexCut complexCut, Slice2DLayer layer = null) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		foreach (Slicer2D id in GetListLayer(layer)) {
			Slice2D sliceResult = id.ComplexCutSlice (complexCut);
			if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
			}
		}
				
		return(result);
	}

	static public List<Slice2D> PointSliceAll(Vector2D slice, float rotation, Slice2DLayer layer = null) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		foreach (Slicer2D id in GetListLayer(layer)) {
			Slice2D sliceResult = id.PointSlice (slice, rotation);
			if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
			}
		}

		return(result);
	}

	// Remove Position
	static public List<Slice2D> PolygonSliceAll(Vector2D position, Polygon2D slicePolygon, bool destroy, Slice2DLayer layer = null) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		Polygon2D slicePolygonDestroy = null;
		if (destroy) {
			slicePolygonDestroy = slicePolygon.ToScale(new Vector2(1.1f, 1.1f));
			slicePolygonDestroy = slicePolygonDestroy.ToOffset (position);
		}
		
		slicePolygon = slicePolygon.ToOffset (position);
		
		foreach (Slicer2D id in GetListLayer(layer)) {
			result.Add (id.PolygonSlice (slicePolygon, slicePolygonDestroy));
		}
		
		return(result);
	}
	
	static public List<Slice2D> ExplodeByPointAll(Vector2D point, Slice2DLayer layer = null) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		foreach (Slicer2D id in GetListLayer(layer)) {
			Slice2D sliceResult = id.ExplodeByPoint (point);
			if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
			}
		}

		return(result);
	}

	static public List<Slice2D> ExplodeInPointAll(Vector2D point, Slice2DLayer layer = null) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}
		
		foreach (Slicer2D id in GetListLayer(layer)) {
			Slice2D sliceResult = id.ExplodeInPoint (point);
			if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
			}
		}

		return(result);
	}

	static public List<Slice2D> ExplodeAll(Slice2DLayer layer = null, int explosionSlices = 0) {
		List<Slice2D> result = new List<Slice2D> ();

		if (layer == null) {
			layer = Slice2DLayer.Create();
		}

		foreach (Slicer2D id in	GetListLayer(layer)) {
			Slice2D sliceResult = id.Explode (explosionSlices);
			if (sliceResult.GetGameObjects().Count > 0) {
				result.Add (sliceResult);
			}
		}

		return(result);
	}
		
	public void RecalculateJoints() {
		Rigidbody2D body = GetRigibody();
		if (body) {
			joints = Joint2D.GetJointsConnected (body);
		}
	}

	void SliceJointEvent(Slice2D sliceResult) {
		RecalculateJoints() ;

		// Remove Slicer Component Duplicated From Sliced Components
		foreach (GameObject g in sliceResult.GetGameObjects()) {
			List<Joint2D> joints = Joint2D.GetJoints(g);
			foreach(Joint2D joint in joints) {
				if (Polygon2DList.CreateFromGameObject (g)[0].PointInPoly (new Vector2D (joint.anchoredJoint2D.anchor)) == false) {

					Destroy (joint.anchoredJoint2D);
				} else {

					if (joint.anchoredJoint2D != null && joint.anchoredJoint2D.connectedBody != null) {

						Slicer2D slicer2D = joint.anchoredJoint2D.connectedBody.gameObject.GetComponent<Slicer2D>();
						if (slicer2D != null) {
							slicer2D.RecalculateJoints();
						}
					}
				}
			}
		}
	
		if (GetRigibody() == null) {
			return;
		}

		// Reconnect Joints To Sliced Bodies
		foreach(Joint2D joint in joints) {
			if (joint.anchoredJoint2D == null) {
				continue;
			}
			
			foreach (GameObject g in sliceResult.GetGameObjects()) {
				Polygon2D poly = Polygon2DList.CreateFromGameObject (g)[0];

				switch (joint.jointType) {
					case Joint2D.Type.HingeJoint2D:
						if (poly.PointInPoly (new Vector2D (joint.anchoredJoint2D.connectedAnchor))) {
							joint.anchoredJoint2D.connectedBody = g.GetComponent<Rigidbody2D> ();
						}
						break;

					default:
						if (poly.PointInPoly (new Vector2D (joint.anchoredJoint2D.connectedAnchor))) {
							joint.anchoredJoint2D.connectedBody = g.GetComponent<Rigidbody2D> ();
						} else {
							
						}
						break;
				}
			}
		}
	}

		
	void StartAnchor () {
		bool addEvents = false;

		foreach(Collider2D collider in anchorsList) {
			if (collider != null) {
				addEvents = true;
			}
		}

		if (addEvents == false) {
			return;
		}

		Slicer2D slicer = GetComponent<Slicer2D> ();
		if (slicer != null) {
			slicer.AddResultEvent (OnAnchorSliceResult);
			slicer.AddEvent (OnAnchorSlice);
		}

		anchorPolygons = new List<Polygon2D>();
		anchorColliders = new List<Collider2D>();

		foreach(Collider2D collider in anchorsList) {
			anchorPolygons.Add(Polygon2DList.CreateFromGameObject (collider.gameObject)[0]);
			anchorColliders.Add(collider);
		}
	}

	bool OnAnchorSlice(Slice2D sliceResult) {
		return(Slicer2DAnchor.OnAnchorSlice(this, sliceResult));
	}

	void OnAnchorSliceResult(Slice2D sliceResult) {
		Slicer2DAnchor.OnAnchorSliceResult(this, sliceResult);
	}
	
	static public Slicer2D PointInSlicerComponent(Vector2D point) {
		foreach(Slicer2D slicer in Slicer2D.GetList()) {
			Polygon2D poly = slicer.shape.GetWorld();
			if (poly.PointInPoly(point)) {
				return(slicer);
			}
		}
		return(null);
	}

	public class API {
		static public Slice2D LinearSlice(Polygon2D polygon, Pair2D slice) {
			return(LinearSlicer.Slice (polygon, slice));
		}
		static public Slice2D LinearCutSlice(Polygon2D polygon, LinearCut linearCut) {
			return(ComplexSlicerExtended.LinearCutSlice (polygon, linearCut));
		}
		static public Slice2D ComplexSlice(Polygon2D polygon, List<Vector2D> slice) {
			return(ComplexSlicer.Slice (polygon, slice));
		}
		static public Slice2D ComplexCutSlice(Polygon2D polygon, ComplexCut complexCut) {
			return(ComplexSlicerExtended.ComplexCutSlice (polygon, complexCut));
		}
		static public Slice2D PointSlice(Polygon2D polygon, Vector2D point, float rotation) {
			return(LinearSlicerExtended.SliceFromPoint (polygon, point, rotation));
		}
		static public Slice2D PolygonSlice(Polygon2D polygon, Polygon2D polygonB) {
			return(ComplexSlicerExtended.Slice (polygon, polygonB)); 
		}
		static public Slice2D ExplodeByPoint(Polygon2D polygon, Vector2D point, int explosionSlices = 0) {
			return(LinearSlicerExtended.ExplodeByPoint (polygon, point, explosionSlices));
		}
		static public Slice2D ExplodeInPoint(Polygon2D polygon, Vector2D point, int explosionSlices = 0) {
			return(LinearSlicerExtended.ExplodeInPoint (polygon, point));
		}
		static public Slice2D Explode(Polygon2D polygon, int explosionSlices = 0) {
			return(LinearSlicerExtended.Explode (polygon, explosionSlices));
		}
		static public Polygon2D CreatorSlice(List<Vector2D> slice) {
			return(ComplexSlicerExtended.CreateSlice (slice));
		}
	}

	public class Debug {
		static public bool enabled = true;
	}
}