using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HeightMapGen))]
public class HeightMapGenEditor : Editor {
	[MenuItem("TARD/Water/Add")]
	private static void AddWater()
	{
		var sobj = Selection.activeGameObject;
		if (sobj == null)
			return;
		HeightMapGen gen = sobj.GetComponent<HeightMapGen> ();
		if (gen == null) {
			gen = sobj.AddComponent<HeightMapGen> ();
		}
		sobj.layer = 2;
	}


	HeightMapGen builder;
	public override void  OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		GUILayout.Space (30);
		builder = (HeightMapGen)target;

		if (builder.State == HeightMapGenState.idle) {
			Build ();

		} else if (builder.State == HeightMapGenState.building) {
			Progress ();
			Cancel ();

		} else if (builder.State == HeightMapGenState.done) {
			Build ();
		}
	}

	void Progress()
	{
		float width = EditorGUIUtility.currentViewWidth-21;
		GUILayout.Box ("", GUILayout.Width(width),GUILayout.Height(20));
		GUI.color = new Color (0, 1, 1,1);
		Rect rect = GUILayoutUtility.GetLastRect ();
		rect = new Rect (rect.x, rect.y, rect.width * builder.BuildProgess, rect.height);
		GUI.Box (rect,"");
		GUI.color = Color.white;
	}

	void Cancel()
	{

		if (GUILayout.Button ("Cancel")) {
			EditorCoroutineRunner.StopEditorCoroutine ();
			builder.Cancel ();
		}
	}


	void Build()
	{
		if (GUILayout.Button ("Build")) {
			PrefabUtility.DisconnectPrefabInstance (builder);
			Undo.RecordObject (builder,"");
			EditorCoroutineRunner.StartEditorCoroutine (builder.Build());
			EditorCoroutineRunner.StartEditorCoroutine (ShowProgress());
		}
	}
		
	IEnumerator ShowProgress()
	{
		HeightMapGen builder = (HeightMapGen)target;
		while (builder.State == HeightMapGenState.building) {
			Repaint ();
			yield return null;
		}
		Repaint ();
	}
}
