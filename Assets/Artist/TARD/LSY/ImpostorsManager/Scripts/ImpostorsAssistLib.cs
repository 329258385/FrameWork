using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpostorsAssistLib {
	public static void ReplaceImpostor_SAO(GameObject obj)
	{
		Material mat = obj.GetComponent<MeshRenderer> ().sharedMaterial;
		mat.shader = UnityUtils.FindShader ("SAO_TLsy/Impostors/OctahedronSAO");
	}
	public static void ReplaceImpostor_SAOBillBoard(GameObject obj)
	{
		Material mat = obj.GetComponent<MeshRenderer> ().sharedMaterial;
		mat.shader = UnityUtils.FindShader ("SAO_TJia/BRDF_Billboard");
	}
}
