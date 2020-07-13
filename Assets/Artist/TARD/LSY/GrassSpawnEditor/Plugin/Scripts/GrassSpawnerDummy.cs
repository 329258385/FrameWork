using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassSpawnerDummy : MonoBehaviour {
	public TerrainGrassInfo info;
	public MeshFilter filter;
	public MeshRenderer mr;
	void Update () {
		if (info == null)
			return;
		transform.position = info.transform.position;
		transform.rotation = info.transform.rotation;
		transform.localScale = info.transform.lossyScale;

		filter.sharedMesh = info.GetComponentInChildren<MeshFilter> ().sharedMesh;
		mr.sharedMaterial.SetTexture ("_MainTex", info.grassTex);
	}

	public void Show(bool show)
	{
		mr.enabled = show;
	}
}
