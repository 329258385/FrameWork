using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class ShaderManager{
	ShaderInfo info;

	#region GUI prop
	Vector2 scrollPos;
	#endregion
	public void OnGUI(ShaderInfo _info)
	{
		info = _info;


		if (info == null || info.variantInfo.keywords==null)
			return;
		GUILayout.Label (""+info.adbPath);

		GUILayout.Space (10);
		GUILayout.Label ("变体数量:"+info.variantInfo.variantCount);

		GUILayout.Space (10);
		if (GUILayout.Button ("Update",GUILayout.Width(220))) {
			var newSkips = info.variantInfo.NewSkips ();
			ShaderTextProcess.AddKeywords (info.path, newSkips);
			AssetDatabase.ImportAsset (info.adbPath);
			info.variantInfo.Refresh ();
		}

		scrollPos = GUILayout.BeginScrollView (scrollPos);
		for (int i = 0; i < info.variantInfo.keywords.Count; i++) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label (info.variantInfo.keywords [i],GUILayout.Width(200));
			info.variantInfo.keywordsIfSkip [i] = GUILayout.Toggle (info.variantInfo.keywordsIfSkip [i],"",GUILayout.Width(60));
			GUILayout.EndHorizontal ();
		}
		GUILayout.EndScrollView ();
	}
}
