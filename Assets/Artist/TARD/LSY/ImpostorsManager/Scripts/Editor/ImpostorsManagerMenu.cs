using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AmplifyImpostors;
public static class ImpostorsManagerMenu {
	public static Shader AIShader
	{
		get{ 
			if (aiShader == null) {
				aiShader = UnityUtils.FindShader ("Standard");
			}
			return aiShader;
		}
	}
	private static Shader aiShader;
	private static Dictionary<Renderer,Material> matDic = new Dictionary<Renderer, Material>();

	[MenuItem("TARD/优化/Impostors/1 创建AI(基于LOD0)")]
	private static void Add0()
	{
		AddAI (0);
	}
	[MenuItem("TARD/优化/Impostors/1 创建AI(基于LOD1)")]
	private static void Add1()
	{
		AddAI (1);
	}
	[MenuItem("TARD/优化/Impostors/1 创建AI(基于LOD2)")]
	private static void Add2()
	{
		AddAI (2);
	}

	[MenuItem("TARD/优化/Impostors/2 完成")]
	private static void Done()
	{
		foreach (var item in matDic) {
			item.Key.sharedMaterial = item.Value;
		}
	}

	[MenuItem("TARD/优化/Impostors/3 清除(慎用)")]
	private static void DoneAll()
	{
		DeleteAll<AmplifyImpostor> ();
		DeleteAll<BillboardRotationInitializer> ();
	}

	private static void DeleteAll<T>()
		where T:MonoBehaviour
	{
		var items = GameObject.FindObjectsOfType<T> ();
		foreach (var item in items) {
			Object.DestroyImmediate (item);
		}
	}


	private static void AddAI(int index)
	{
		var obj = Selection.activeGameObject;
		LODGroup g = obj.GetComponent<LODGroup> ();
		if (g == null)
			return;


		var lods = g.GetLODs ();

		matDic.Clear ();
		var rs = lods [index].renderers;
		foreach (var r in rs) {
			matDic.Add (r, r.sharedMaterial);
			ReplaceMat (r);
		}


		AmplifyImpostor ai = obj.GetComponent<AmplifyImpostor> ();
		if(ai == null)
			ai = obj.AddComponent<AmplifyImpostor> ();
		ai.Renderers = rs;

		ImpostorsAssist assist = obj.GetComponentInChildren<ImpostorsAssist> ();
		if (assist == null) {
			var prefab = AssetDatabase.LoadAssetAtPath<Object> ("Assets/Artist/TARD/LSY/ImpostorsManager/Prefabs/Assist.prefab");
			var assistObj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			assistObj.transform.parent = obj.transform;
			assistObj.transform.localPosition = Vector3.zero;
			assistObj.transform.localRotation = Quaternion.identity;
			assistObj.transform.localScale = Vector3.one;

			foreach (var r in rs) {
				r.transform.parent = assistObj.transform;
			}
		}


		BillboardRotationInitializer rotateInit = obj.GetComponent<BillboardRotationInitializer> ();
		if(rotateInit == null)
			rotateInit = obj.AddComponent<BillboardRotationInitializer> ();
	}

	public static void ReplaceMat(Renderer mr)
	{
		var oldMat = mr.sharedMaterial;
		Material mat = new Material (AIShader);
		mat.SetTexture ("_MainTex",oldMat.GetTexture("_MainTex"));
		mat.SetTexture ("_BumpMap",oldMat.GetTexture("_BumpTex"));
		mr.sharedMaterial = mat;
	}
}