using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DControllerObject {
	public Slicer2DInputController input = new Slicer2DInputController();
	public Slicer2DControllerEventHandling eventHandler = new Slicer2DControllerEventHandling();
	public Slicer2DVisuals visuals = new Slicer2DVisuals();
	public Slice2DLayer sliceLayer = Slice2DLayer.Create();

	public void SetController(Slicer2DInputController inputController, Slicer2DVisuals visualsSettings, Slice2DLayer layerObject, Slicer2DControllerEventHandling eventHandling) {
		input = inputController;
		visuals = visualsSettings;
		sliceLayer = layerObject;
		eventHandler = eventHandling;
	}
}
