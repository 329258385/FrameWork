using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CB_WorkerBase))]
public class CB_WorkerBaseInspector : Editor {
	CB_WorkerBase worker;
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		worker = (CB_WorkerBase)target;
		GUILayout.Space (50);
		GUILayout.Label ("[材质参数]");

		for (int i = 0; i < worker.mats.Count; i++) {
			ShowParams ("Mats " + i, worker.matParams [i], worker.mats [i],worker);
		}

	}

	public static void ShowParams(string title,CB_MaterialParams p,Material mat, Object obj = null)
	{
		GUILayout.Label ("参数:" + title);
		if (mat.HasProperty (CB_MaterialParams.KeyColor)) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("颜色");
			var temp = EditorGUILayout.ColorField (GUIContent.none,p.color,true,true,true);
			if (p.color != temp) {
				p.color = temp;
				SetItDirty (obj);
			}
			EditorGUILayout.EndHorizontal ();
		}

		if (mat.HasProperty (CB_MaterialParams.KeyInternsity)) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("强度");
			float temp = EditorGUILayout.Slider (p.internsity,0,10);
			if (p.internsity != temp) {
				p.internsity = temp;
				SetItDirty (obj);
			}
			EditorGUILayout.EndHorizontal ();
		}

		if (mat.HasProperty (CB_MaterialParams.KeyRange)) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("范围");
			float temp = EditorGUILayout.Slider (p.range,0,1);
			if (p.range != temp) {
				p.range = temp;
				SetItDirty (obj);
			}
			EditorGUILayout.EndHorizontal ();
		}

		if (mat.HasProperty (CB_MaterialParams.KeyBlur)) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("模糊尺寸");
			float temp = EditorGUILayout.Slider (p.blurSize,0,3);
			if (p.blurSize != temp) {
				p.blurSize = temp;
				SetItDirty (obj);
			}
			EditorGUILayout.EndHorizontal ();
		}


		if (mat.HasProperty (CB_MaterialParams.KeyFogStart)) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("雾效起始");
			float temp = EditorGUILayout.FloatField (p.fogStart);
			if (p.fogStart != temp) {
				p.fogStart = temp;
				SetItDirty (obj);
			}
			EditorGUILayout.EndHorizontal ();
		}
		if (mat.HasProperty (CB_MaterialParams.KeyFogEnd)) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("雾效结束");
			float temp = EditorGUILayout.FloatField (p.fogEnd);
			if (p.fogEnd != temp) {
				p.fogEnd = temp;
				SetItDirty (obj);
			}
			EditorGUILayout.EndHorizontal ();
		}

	}

	protected static void SetItDirty(Object obj = null)
	{
		if (obj != null) {
			EditorUtility.SetDirty (obj);
			Undo.RegisterCompleteObjectUndo (obj, "changed");
		}
	}
}
