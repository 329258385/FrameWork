using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GrassBuilder))]
public class GrassBuilderInspector : Editor {
	GrassBuilder builder;
	public override void  OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		GUILayout.Space (30);
		builder = (GrassBuilder)target;

		if (builder.State == GrassBuildState.idle) {
			Build ();

		} else if (builder.State == GrassBuildState.building) {
			Progress ();
			UICancel ();

		} else if (builder.State == GrassBuildState.done) {
			Build ();
			UpdateMaterial ();
		}
			
		if (GrassBuilder.buildError) {
			Cancel ();
			ShowWarning ();
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

	void UICancel()
	{
		if (GUILayout.Button ("Cancel")) {
			Cancel ();
		}
	}
	void Cancel()
	{
		GrassBuilder.buildError = false;
		EditorCoroutineRunner.StopEditorCoroutine ();
		builder.Cancel ();
	}

	void Build()
	{
		if (GUILayout.Button ("Build")) {
			PrefabUtility.DisconnectPrefabInstance (builder);
			Undo.RecordObject (builder,"");
			EditorCoroutineRunner.StartEditorCoroutine (IEBuild());
			EditorCoroutineRunner.StartEditorCoroutine (ShowProgress());

		}
	}

	IEnumerator IEBuild()
	{
		yield return builder.Build ();
		try{
		GrassTex.GenTex (builder);
		//EditorUtility.SetDirty (builder);
		EditorSceneManager.MarkAllScenesDirty();
		}
		catch(System.Exception e) {
			Debug.LogError (e);
			GrassBuilder.buildError = true;
		}
	}
	void UpdateMaterial()
	{
		if (GUILayout.Button ("Update Material")) {
			builder.UpdateMaterial ();
		}
	}

	IEnumerator ShowProgress()
	{
		GrassBuilder builder = (GrassBuilder)target;
		while (builder.State == GrassBuildState.building) {
			Repaint ();
			yield return null;
		}
		Repaint ();
	}

	public static void ShowWarning()
	{
		EditorUtility.DisplayDialog ("草烘焙失败", "草烘焙失败,请检查草设置(如TerrainInfo贴图是否开读写)", "好的");
	}
}
