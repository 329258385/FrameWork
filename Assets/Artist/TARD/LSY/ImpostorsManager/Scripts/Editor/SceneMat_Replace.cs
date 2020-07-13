using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
public class SceneMat_Replace{
	static Dictionary<int,Material> dic = new Dictionary<int, Material>();

	[MenuItem("TARD/优化/Impostors/Billboard复制材质球")]
	public static void BillboardDuplicateMat()
	{
		dic.Clear ();

		var scene = SceneManager.GetActiveScene ();
		foreach (var obj in scene.GetRootGameObjects()) {
			foreach (var mf in obj.GetComponentsInChildren<MeshRenderer>()) {
				if (mf.sharedMaterial == null)
					continue;
				if(Condition(mf))
					Replace (mf);
			}
		}
	}

	private static void Replace(MeshRenderer mr)
	{
		string sceneName = SceneManager.GetActiveScene ().name;
		Material matOigin = mr.sharedMaterial;
		int id = matOigin.GetInstanceID ();
		string path = AssetDatabase.GetAssetPath (matOigin);
		string newPath = path.Substring (0, path.Length - 4);
		if (newPath.EndsWith (sceneName)) {
			return;
		}
		newPath = string.Format ("{0}_{1}.mat", newPath, sceneName);


		if (!dic.ContainsKey (id)) {
			Material newMat = AssetDatabase.LoadAssetAtPath<Material> (newPath);
			if(newMat ==null)
			{
				newMat = new Material (matOigin);
				AssetDatabase.CreateAsset (newMat, newPath);
				AssetDatabase.ImportAsset (newPath);
			}
			Debug.Log ("**创建公告板材质球** "+newPath);
			dic.Add(id,newMat);
		}
		Material mat = dic[id];
		mr.sharedMaterial = mat;
	}

	private static bool Condition(MeshRenderer mr)
	{
		return mr.sharedMaterial.shader.name == "SAO_TJia/BRDF_Billboard";
		//return true;
	}
}
