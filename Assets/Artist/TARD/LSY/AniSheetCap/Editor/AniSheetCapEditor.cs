using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AniSheetCap))]
public class AniSheetCapEditor : Editor {

	[MenuItem("TARD/优化/Particle/转帧序列")]
	private static void AddAniSheetCap()
	{
		var obj = Selection.activeGameObject;
		LsyCommon.TryAddComponent<AniSheetCap> (obj);
	}

	public override void OnInspectorGUI ()
	{
		AniSheetCap cap = (AniSheetCap)target;
		base.OnInspectorGUI ();
		GUILayout.Label ("FPS:" + cap.FPS.ToString("f2"));
		if (GUILayout.Button ("Set Path")) {
			string path = EditorUtility.SaveFilePanel ("Save", Application.dataPath, "x", "png");
			Debug.Log (path);
			cap.imagePath = path;
		}
	}
}
