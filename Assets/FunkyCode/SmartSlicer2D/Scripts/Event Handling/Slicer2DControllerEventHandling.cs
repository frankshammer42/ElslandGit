using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slicer2DControllerEventHandling {
	
	public delegate void ResultEvent(Slice2D slice);
	public event ResultEvent sliceResultEvent;

	public void Perform(Slice2D result) {
		if (sliceResultEvent != null) {
			sliceResultEvent(result);
		}
	}
	
}
