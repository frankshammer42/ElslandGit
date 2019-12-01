using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Slicer2DSettingsEditor : EditorWindow {
	int tab = 0;

	[MenuItem("Tools/Slicer 2D")]
    public static void ShowWindow() {
        GetWindow<Slicer2DSettingsEditor>(false, "Slicer 2D", true);
    }

	 void OnGUI() {
		tab = GUILayout.Toolbar (tab, new string[] {"Preferences", "Profiler"});
		switch (tab) {
			case 0:
				Preferences();
				break;

			case 1:
				Profiler();
				break;
		}
    }

	void Preferences() {
		Slicer2DSettingsProfile profile = Slicer2DSettings.GetProfile();

		if (profile) {
			profile.renderingPipeline= (Slicer2DSettings.RenderingPipeline)EditorGUILayout.EnumPopup("Rendering Pipeline", profile.renderingPipeline);

			profile.garbageCollector = EditorGUILayout.Toggle("Garbage Collector", profile.garbageCollector);
			if (profile.garbageCollector) {
				profile.garbageCollectorSize = EditorGUILayout.FloatField("Garbage Collector Size", profile.garbageCollectorSize);
			}

			profile.explosionPieces = (int)EditorGUILayout.Slider("Explosion Pieces", profile.explosionPieces, 1, 30);

			profile.componentsCopy = (Slicer2DSettings.InstantiationMethod)EditorGUILayout.EnumPopup("Instatiation Method", profile.componentsCopy);

			profile.triangulation = (Slicer2DSettings.Triangulation)EditorGUILayout.EnumPopup("Triangulation", profile.triangulation);

			profile.batching = (Slicer2DSettings.Batching)EditorGUILayout.EnumPopup("Batching", profile.batching);
			
			if (GUILayout.Button("Default Settings")) {
				profile.garbageCollector = true;
				profile.garbageCollectorSize = 0.005f;

				profile.componentsCopy = Slicer2DSettings.InstantiationMethod.Default;
				profile.triangulation = Slicer2DSettings.Triangulation.Default;
				profile.batching = Slicer2DSettings.Batching.Default;
			}

			EditorGUILayout.HelpBox("Settings marked as 'default' will use local component setting", MessageType.Info);

			EditorGUILayout.HelpBox("Garbage Collector: When enabled, very small unuseful slices are removed", MessageType.None);
			EditorGUILayout.HelpBox("Instatiation Method: Performance mode would increase performance about 25%, however cannot be used in certain cases", MessageType.None);
			EditorGUILayout.HelpBox("Triangulation: The more reliable triangulation method, the slower most likely it performs. Simple shapes could use less complicated triangulation", MessageType.None);
			EditorGUILayout.HelpBox("Batching: when enabled, sliced parts of the object will use same material instance as it's origin (Improves Performance)", MessageType.None);

		} else {
			EditorGUILayout.HelpBox("Slicer2D Settings Profile Not Found!", MessageType.Error);
		}	
	}

	void Profiler() {
		EditorGUILayout.HelpBox("Advanced Triangulation: " + Slicer2DProfiler.GetAdvancedTriangulation(), MessageType.None);
		EditorGUILayout.HelpBox("Legacy Triangulation: " + Slicer2DProfiler.GetLegacyTriangulation(), MessageType.None);
		EditorGUILayout.HelpBox("Batched Objects: " + Slicer2DProfiler.GetBatchingApplied(), MessageType.None);
		EditorGUILayout.HelpBox("Objects Created: " + Slicer2DProfiler.GetObjectsCreated(), MessageType.None);
		EditorGUILayout.HelpBox("Objects Slices Created With Performance: " + Slicer2DProfiler.GetSlicesCreatedWithPeroformance(), MessageType.None);
		EditorGUILayout.HelpBox("Objects Slices Created With Quality: " + Slicer2DProfiler.GetSlicesCreatedWithQuality(), MessageType.None);
	}

}