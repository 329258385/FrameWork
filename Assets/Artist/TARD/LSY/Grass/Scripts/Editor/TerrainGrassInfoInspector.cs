using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGrassInfo))]
public class TerrainGrassInfoInspector : Editor {
	GrassBuilder builder;
	public override void  OnInspectorGUI()
	{
		base.OnInspectorGUI ();
//		GUILayout.Space (30);
//	
//		TerrainGrassInfo info = (TerrainGrassInfo)target;
//		if (GUILayout.Button ("Set Tex"))
//			info.SetTex ();
	}
}