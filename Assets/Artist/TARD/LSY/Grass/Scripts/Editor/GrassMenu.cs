using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class GrassMenu {

	[MenuItem("TARD/Grass/Add Grass")]
	private static void Grass()
	{
		GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath ("Assets/Artist/TARD/LSY/Grass/Prefabs/GrassGenerator.prefab");
		GameObject obj = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
		Selection.activeGameObject = obj;
	}
	[MenuItem("TARD/Grass/Add Terrain Info")]
	private static void Tex()
	{
		var sobj = Selection.activeGameObject;
		if (sobj == null)
			return;
		TerrainGrassInfo info = sobj.GetComponent<TerrainGrassInfo> ();
		if(info == null)
			info = sobj.AddComponent<TerrainGrassInfo> ();
	}
	[MenuItem("TARD/Grass/Add Grass Spawn Editor")]
	private static void SpawnEditor()
	{
		GameObject obj = null;
		GrassSpawner gs = SceneFind<GrassSpawner> ();
		if (gs != null) {
			obj = gs.gameObject;
		} else {
			GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath ("Assets/Artist/TARD/LSY/GrassSpawnEditor/Prefabs/Brush.prefab");
			obj = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
		}
		Selection.activeGameObject = obj;
	}

	public static T SceneFind<T> ()
		where T:Object 
	{
		T t = GameObject.FindObjectOfType<T> ();
		return t;
	}
}
