using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class ShaderBrowser : EditorWindow {
	[SerializeField]
	ShaderInfo selected;
	[SerializeField]
	List<ShaderInfo> shaderInfos = new List<ShaderInfo>() ;

	ShaderManager shaderManager = new ShaderManager ();
	#region GUI prop
	Vector2 scrollPos;
	#endregion
	string searchText = "";
	List<ShaderInfo> shaderInfosResult = new List<ShaderInfo>() ;

	[MenuItem("TARD/优化/ShaderVariantManager")]
	public static void Show()
	{
		var win = (ShaderBrowser)EditorWindow.GetWindow<ShaderBrowser> ();
		win.Show (true);
		win.Init ();
	}

	void Init()
	{
		shaderInfos.Clear ();
		//Debug.Log ("1:"+Time.realtimeSinceStartup);
		string[] paths = Directory.GetFiles(Application.dataPath, "*.shader", SearchOption.AllDirectories);
		//Debug.Log (paths.Length);
		//Debug.Log ("2:"+Time.realtimeSinceStartup);
		foreach (var path in paths) {
			ShaderInfo info = new ShaderInfo (path);
			shaderInfos.Add (info);
		}
		//Debug.Log ("3:"+Time.realtimeSinceStartup);
		Search();
	}

	void Search()
	{
		shaderInfosResult.Clear ();
		if (string.IsNullOrEmpty (searchText)) {
			shaderInfosResult.AddRange (shaderInfos);
		} else {
			foreach (var item in shaderInfos) {
				if (item.adbPath.IndexOf (searchText,System.StringComparison.OrdinalIgnoreCase)>=0) {
					shaderInfosResult.Add (item);
				}
			}
		}
	}
	void OnGUI()
	{
		GUILayout.BeginHorizontal ();
		searchText = GUILayout.TextField(searchText,GUILayout.Width(200));
		if (GUILayout.Button ("Search", GUILayout.Width (70))) {
			Search ();
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical (GUILayout.Width(600));
		scrollPos = GUILayout.BeginScrollView (scrollPos);
		for (int i = 0; i < shaderInfosResult.Count; i++) {
			GUILayout.Label (shaderInfosResult [i].adbPath);
			Rect rect = GUILayoutUtility.GetLastRect ();

			if (GUI.Button (rect,"",GUIStyle.none)) {
				selected = shaderInfosResult [i];
				selected.OnSelect ();
				Selection.activeObject = selected.shader;
			}
			if (selected == shaderInfosResult [i]) {
				GUI.backgroundColor = new Color (0, 1, 1);
				GUI.Box (rect, "");
				GUI.backgroundColor = Color.white;
			}
		}
		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();

		GUILayout.BeginVertical ();
		shaderManager.OnGUI (selected);
		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
	}
}

