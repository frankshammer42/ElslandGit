using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Slicer2D/Settings Profile", order = 1)]

public class Slicer2DSettingsProfile : ScriptableObject {
	public bool garbageCollector = true;
	public float garbageCollectorSize = 0.005f;

	public int explosionPieces = 15;

	public Slicer2DSettings.Batching batching = Slicer2DSettings.Batching.Default;
	
	public Slicer2DSettings.Triangulation triangulation = Slicer2DSettings.Triangulation.Default;
	
	public Slicer2DSettings.InstantiationMethod componentsCopy = Slicer2DSettings.InstantiationMethod.Default;

	public Slicer2DSettings.RenderingPipeline renderingPipeline = Slicer2DSettings.RenderingPipeline.Universal;
}
