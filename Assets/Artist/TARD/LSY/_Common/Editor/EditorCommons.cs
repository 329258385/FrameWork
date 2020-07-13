using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


public static class EditorCommons{

	private static void CreatePrefab(string title)
	{
		string path = string.Format ("Assets/Artist/TARD/LSY/{0}.prefab", title);
		GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath (path);
		GameObject obj = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
		Selection.activeGameObject = obj;

		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
	}
}
