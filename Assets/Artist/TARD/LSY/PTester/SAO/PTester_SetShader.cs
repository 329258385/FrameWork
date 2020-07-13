using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTester_SetShader{
	public static Dictionary<Material,Shader> dic = new Dictionary<Material, Shader>();
	public static void RecShader()
	{
		if (dic.Count == 0) {
			var mrs = Object.FindObjectsOfType<MeshRenderer> ();
			foreach (var mr in mrs) {
				if (mr == null)
					continue;

				foreach (var mat in mr.sharedMaterials) {
					if (mat == null)
						continue;
					if (!dic.ContainsKey (mat)) {
						dic.Add (mat, mat.shader);
					}
				}
			}
		}
	}
	public static void RevertShader()
	{
		RecShader ();
		var mrs = Object.FindObjectsOfType<MeshRenderer> ();
		foreach (var mr in mrs) {
			if (mr == null)
				continue;

			foreach (var mat in mr.sharedMaterials) {
				if (mat == null)
					continue;
				mat.shader = dic[mat];		
			}
		}
	}
	public static void SetShader(Shader s)
	{
		RecShader ();
		var mrs = Object.FindObjectsOfType<MeshRenderer> ();
		foreach (var mr in mrs) {
			if (mr == null)
				continue;

			foreach (var mat in mr.sharedMaterials) {
				if (mat == null)
					continue;
				mat.shader = s;		
			}
		}
	}
}
