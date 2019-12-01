using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Slicer2D))]
public class Slicer2DEditor : Editor {
	static bool foldout = true;

	override public void OnInspectorGUI() {
		serializedObject.Update();
		EditorGUI.BeginChangeCheck();

		Slicer2D script = target as Slicer2D;

		// Disable
		//script.shapeType = (Slicer2D.ShapeType)EditorGUILayout.EnumPopup ("Shape Type", script.shapeType);
		script.textureType = (Slicer2D.TextureType)EditorGUILayout.EnumPopup ("Texture Type", script.textureType);
		script.colliderType = (Slicer2D.ColliderType)EditorGUILayout.EnumPopup ("Collider Type", script.colliderType);

		SetTriangulation(script);

		SetComponentsCopy(script);

		script.centerOfMass = (Slicer2D.CenterOfMass)EditorGUILayout.EnumPopup ("Center of Mass", script.centerOfMass);
		script.slicingLayer = (Slicing2DLayer)EditorGUILayout.EnumPopup ("Slicing Layer", script.slicingLayer);
		script.supportJoints = EditorGUILayout.Toggle("Support Joints", script.supportJoints);
		script.limit.enabled = EditorGUILayout.Toggle("Slicing Limit", script.limit.enabled);

		if (script.limit.enabled) {
			script.limit.maxSlices = EditorGUILayout.IntSlider("Max Slices", script.limit.maxSlices, 1, 10);
		}

		script.recalculateMass = EditorGUILayout.Toggle("Recalculate Mass", script.recalculateMass);
	
		script.anchors = EditorGUILayout.Toggle("Anchors", script.anchors);

		if (script.anchors) {
			SerializedProperty anchorList = serializedObject.FindProperty ("anchorsList");

			EditorGUILayout.PropertyField (anchorList, true);
		}

		if (EditorGUI.EndChangeCheck()) {
			serializedObject.ApplyModifiedProperties();
		}

		foldout = EditorGUILayout.Foldout(foldout, "Material Settings" );
		if (foldout) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			SetBatching(script);

			switch(script.textureType) {
				case Slicer2D.TextureType.Mesh2D:
					script.materialSettings.material = (Material)EditorGUILayout.ObjectField("Material", script.materialSettings.material, typeof(Material), true);
					script.materialSettings.scale = EditorGUILayout.Vector2Field("Material Scale", script.materialSettings.scale);
					script.materialSettings.offset = EditorGUILayout.Vector2Field("Material Offset", script.materialSettings.offset);
			
					break;

				case Slicer2D.TextureType.Mesh3D:
					script.materialSettings.material = (Material)EditorGUILayout.ObjectField("Material", script.materialSettings.material, typeof(Material), true);
				
					break;

				case Slicer2D.TextureType.Sprite:

					break;
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}
	}

	public void SetTriangulation(Slicer2D script) {
		Slicer2DSettingsProfile profile = Slicer2DSettings.GetProfile();

		if (profile == null || profile.triangulation == Slicer2DSettings.Triangulation.Default) {
			script.materialSettings.triangulation = (PolygonTriangulator2D.Triangulation)EditorGUILayout.EnumPopup ("Triangulation", script.materialSettings.triangulation);
		} else {
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.EnumPopup ("Triangulation", Slicer2DSettings.GetTriangulation(script.materialSettings.triangulation));
			EditorGUI.EndDisabledGroup();
		}
	}

	public void SetComponentsCopy(Slicer2D script) {
		Slicer2DSettingsProfile profile = Slicer2DSettings.GetProfile();

		if (profile == null || profile.componentsCopy == Slicer2DSettings.InstantiationMethod.Default) {

			script.instantiateMethod = (Slicer2D.InstantiationMethod)EditorGUILayout.EnumPopup ("Instantiation Method", script.instantiateMethod);
		} else {
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.EnumPopup ("Instantiation", Slicer2DSettings.GetComponentsCopy(script.instantiateMethod));
			EditorGUI.EndDisabledGroup();
		}
	}

	public void SetBatching(Slicer2D script) {
		Slicer2DSettingsProfile profile = Slicer2DSettings.GetProfile();

		if (profile == null || profile.batching == Slicer2DSettings.Batching.Default) {
			script.materialSettings.batchMaterial = EditorGUILayout.Toggle("Batch Material", script.materialSettings.batchMaterial);
		} else {
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.Toggle("Batch Material", Slicer2DSettings.GetBatching(script.materialSettings.batchMaterial));
			EditorGUI.EndDisabledGroup();
		}
	}
}