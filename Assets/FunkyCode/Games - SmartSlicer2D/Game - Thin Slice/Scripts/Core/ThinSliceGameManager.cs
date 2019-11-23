using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ThinSliceGameManager : MonoBehaviour {
	private double startingArea = 0; // Original Size Of The Map

	public double leftArea = 100;	// Percents Of Map Left

	static public ThinSliceGameManager instance;

	void Start () {
		ResetLevel();

		instance = this;
	}

	public void ResetLevel() {
		startingArea = 0;

		foreach(Slicer2D slicer in  Slicer2D.GetList()) {
			startingArea += slicer.shape.GetWorld().GetArea();
		}
	}

	// Recalculate area that is left
	static public void UpdateLevelProgress() {
		instance.leftArea = 0;

		foreach(Slicer2D slicer in Slicer2D.GetList()) {
			instance.leftArea += slicer.shape.GetWorld().GetArea();
		}

		instance.leftArea = ((instance.leftArea) / instance.startingArea) * 100f;
	}
}