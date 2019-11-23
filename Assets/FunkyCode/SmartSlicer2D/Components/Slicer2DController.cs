using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Slicer2DController : MonoBehaviour {
	public enum SliceType {Linear, LinearCut, LinearTracked, LinearTrail, Complex, ComplexCut, ComplexClick, ComplexTracked, ComplexTrail, Point, Polygon, Explode, Create};
	public static Color[] slicerColors = {Color.black, Color.green, Color.yellow , Color.red, new Color(1f, 0.25f, 0.125f)};

	public SliceType sliceType = SliceType.Complex;

	// Slicer2DController.Get()
	private static Slicer2DController instance;
	
	// Slicer Layer
	public Slice2DLayer sliceLayer = Slice2DLayer.Create();

	// Slicer Visuals
	public Slicer2DVisuals visuals = new Slicer2DVisuals();

	// Input
	public Slicer2DInputController input = new Slicer2DInputController();

	// Input Events Handler
	public Slicer2DControllerEventHandling eventHandler = new Slicer2DControllerEventHandling();

	// Different Slicer Type Managers
	public Slicer2DLinearControllerObject linearControllerObject = new Slicer2DLinearControllerObject();
	public Slicer2DComplexControllerObject complexControllerObject = new Slicer2DComplexControllerObject();
	
	public Slicer2DLinearCutControllerObject linearCutControlelrObject = new Slicer2DLinearCutControllerObject();
	public Slicer2DComplexCutControllerObject complexCutControllerObject = new Slicer2DComplexCutControllerObject();

	public Slicer2DLinearTrackerControllerObject linearTrackedControlelrObject = new Slicer2DLinearTrackerControllerObject();
	public Slicer2DComplexTrackerControllerObject complexTrackedControllerObject = new Slicer2DComplexTrackerControllerObject();

	public Slicer2DLinearTrailControllerObject linearTrailControllerObject = new Slicer2DLinearTrailControllerObject();
	public Slicer2DComplexTrailControllerObject complexTrailControllerObject = new Slicer2DComplexTrailControllerObject();

	public Slicer2DPolygonControllerObject polygonControllerObject = new Slicer2DPolygonControllerObject();
	public Slicer2DCreateControllerObject createControllerObject = new Slicer2DCreateControllerObject();

	public Slicer2DComplexClickControllerObject complexClickControllerObject = new Slicer2DComplexClickControllerObject();

	public Slicer2DPointControllerObject pointControllerObject = new Slicer2DPointControllerObject();
	public Slicer2DExplodeControllerObject explodeControllerObject = new Slicer2DExplodeControllerObject();

	public bool UIBlocking = true;

	public void AddResultEvent(Slicer2DControllerEventHandling.ResultEvent e) {
		eventHandler.sliceResultEvent += e;
	}

	public void Awake() {
		instance = this;
	}
	
	public void Start() {
		visuals.Initialize(gameObject);

		linearControllerObject.SetController(input, visuals, sliceLayer, eventHandler);
		linearControllerObject.Initialize();

		complexControllerObject.SetController(input, visuals, sliceLayer, eventHandler);
		complexControllerObject.Initialize();

		linearTrailControllerObject.SetController(input, visuals, sliceLayer, eventHandler);
		linearTrailControllerObject.Initialize();
		
		complexTrailControllerObject.SetController(input, visuals, sliceLayer, eventHandler);
		complexTrailControllerObject.Initialize();

		linearCutControlelrObject.SetController(input, visuals, sliceLayer, eventHandler);
		complexCutControllerObject.SetController(input, visuals, sliceLayer, eventHandler);

		linearTrackedControlelrObject.SetController(input, visuals, sliceLayer, eventHandler);
		complexTrackedControllerObject.SetController(input, visuals, sliceLayer, eventHandler);

		polygonControllerObject.SetController(input, visuals, sliceLayer, eventHandler);
		createControllerObject.SetController(input, visuals, sliceLayer, eventHandler);

		complexClickControllerObject.SetController(input, visuals, sliceLayer, eventHandler);	

		pointControllerObject.SetController(input, visuals, sliceLayer, eventHandler);

		explodeControllerObject.SetController(input, visuals, sliceLayer, eventHandler);
	}

	public bool BlockedByUI() {
		if (UIBlocking == false) {
			return(false);
		}

		if (EventSystem.current == null) {
			return(false);
		}

		if (EventSystem.current.IsPointerOverGameObject(0)) {
			return(true);
		}

		if (EventSystem.current.IsPointerOverGameObject(-1)) {
			return(true);
		}

		if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
			return(true);
		}

		return(false);
	}
	
	public void LateUpdate() {
		if (BlockedByUI() == false) {
			input.Update();
		}

		Vector2D pos = input.GetInputPosition().Copy();

		switch (sliceType) {	
			case SliceType.Linear:
				linearControllerObject.Update();
				break;

			case SliceType.LinearCut:
				linearCutControlelrObject.Update(pos);
				break;

			case SliceType.ComplexCut:
				complexCutControllerObject.Update(pos);
				break;

			case SliceType.Complex:
				complexControllerObject.Update();
				break;
			
			case SliceType.LinearTracked:
				linearTrackedControlelrObject.Update(pos);
				break;

			case SliceType.ComplexTracked:
				complexTrackedControllerObject.Update(pos);
				break;

			case SliceType.Point:
				pointControllerObject.Update(pos);
				break;
				
			case SliceType.Explode:			
				explodeControllerObject.Update(pos);
				break;

			case SliceType.ComplexClick:
				complexClickControllerObject.Update(pos);
				break;

			case SliceType.LinearTrail:
				linearTrailControllerObject.Update();
				break;

			case SliceType.ComplexTrail:
				complexTrailControllerObject.Update();
				break;

			case SliceType.Create:
				createControllerObject.Update(pos, transform);
				break;

			case SliceType.Polygon:
				polygonControllerObject.Update(pos);
				break;

			default:
				break; 
		}

		Draw();
	}
	
	public void Draw() {		
		if (visuals.drawSlicer == false) {
			return;
		}
		switch (sliceType) {
			case SliceType.Linear:
				linearControllerObject.Draw(transform);
				break;

			case SliceType.Complex:
				complexControllerObject.Draw(transform);
				break;

			case SliceType.LinearTracked:
				linearTrackedControlelrObject.Draw(transform);
				break;

			case SliceType.ComplexTracked:
				complexTrackedControllerObject.Draw(transform);
				break;

			case SliceType.LinearCut:
				linearCutControlelrObject.Draw(transform);			
				break;
				
			case SliceType.ComplexCut:
				complexCutControllerObject.Draw(transform);					
				break;
				
			case SliceType.Polygon:
				polygonControllerObject.Draw(transform);
				break;

			case SliceType.Create:
				createControllerObject.Draw(transform);
				break;

			case SliceType.ComplexTrail:
				complexTrailControllerObject.Draw(transform);
				break;

			case SliceType.LinearTrail:
				linearTrailControllerObject.Draw(transform);
				break;

			case SliceType.ComplexClick:
				complexClickControllerObject.Draw(transform);
				break;

			case SliceType.Point:
				pointControllerObject.Draw(transform);
				break;

			case SliceType.Explode:
				explodeControllerObject.Draw(transform);
				break;
		}
	}

	public void SetSliceType(int type) {
		sliceType = (SliceType)type;
	}

	public void SetLayerType(int type) {
		if (type == 0) {
			sliceLayer.SetLayerType((Slice2DLayerType)0);
		} else {
			sliceLayer.SetLayerType((Slice2DLayerType)1);
			sliceLayer.DisableLayers ();
			sliceLayer.SetLayer (type - 1, true);
		}
	}

	public void SetSlicerColor(int colorInt) {
		visuals.slicerColor = slicerColors [colorInt];
	}

	public static Slicer2DController Get() {
		return(instance);
	}
}