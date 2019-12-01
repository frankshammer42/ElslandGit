using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DSettings : MonoBehaviour {
	static public Slicer2DSettingsProfile profile = null;

	public enum InstantiationMethod {
		Default,
		Quality,
		Performance
	}

	public enum Triangulation {
		Default,
		Advanced,
		Legacy
	}

	public enum Batching {
		Default,
		On,
		Off
	}

	public enum RenderingPipeline {
		Universal,
		LightWeight
	}
		
	static public Slicer2DSettingsProfile GetProfile() {
		if (profile == null) {
			profile = Resources.Load("Profiles/Default") as Slicer2DSettingsProfile;
		}

		return(profile);
	}

	public static bool GetBatching(bool setting) {
		Slicer2DSettingsProfile profile = GetProfile();

		if (profile == null) {
			Debug.LogWarning("Profile Settings Are Missing");
			return(setting);
		}

		switch(profile.batching) {
			case Batching.On:
				return(true);
			case Batching.Off:
				return(false);
			default:
				return(setting);
		}
	}

	public static PolygonTriangulator2D.Triangulation GetTriangulation(PolygonTriangulator2D.Triangulation setting) {
		Slicer2DSettingsProfile profile = GetProfile();

		if (profile == null) {
			Debug.LogWarning("Profile Settings Are Missing");
			return(setting);
		}

		if (profile.triangulation == Triangulation.Default) {
			return(setting);
		} else {
			int triangulationID = (int)profile.triangulation - 1;
			return((PolygonTriangulator2D.Triangulation)triangulationID);
		}
	}

	public static Slicer2D.InstantiationMethod GetComponentsCopy(Slicer2D.InstantiationMethod setting) {
		Slicer2DSettingsProfile profile = GetProfile();

		if (profile == null) {
			Debug.LogWarning("Profile Settings Are Missing");
			return(setting);
		}

		if (profile.componentsCopy == InstantiationMethod.Default) {
			return(setting);
		} else {
			int copyID = (int)profile.componentsCopy - 1;
			return((Slicer2D.InstantiationMethod)copyID);
		}
	}

	public static float GetGarbageCollector() {
		Slicer2DSettingsProfile profile = GetProfile();

		if (profile == null) {
			Debug.LogWarning("Profile Settings Are Missing");
			return(-1);
		}

		if (profile.garbageCollector) {
			return(profile.garbageCollectorSize);
		} else {
			return(-1);
		}
	}

	public static RenderingPipeline GetRenderingPipeline() {
		Slicer2DSettingsProfile profile = GetProfile();

		if (profile == null) {
			Debug.LogWarning("Profile Settings Are Missing");
			return(RenderingPipeline.Universal);
		}


		return(profile.renderingPipeline);
	}

	public static int GetExplosionSlices() {
		Slicer2DSettingsProfile profile = GetProfile();

		if (profile == null) {
			Debug.LogWarning("Profile Settings Are Missing");
			return(2);
		}

		return(profile.explosionPieces);
	}
}
