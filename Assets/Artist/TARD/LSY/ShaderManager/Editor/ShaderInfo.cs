using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class ShaderInfoVariant
{
	private ShaderInfo info;

	public List<string> keywords;
	public List<bool> keywordsIfSkip = new List<bool> ();
	public List<string> skips;
	public ulong variantCount;

	public ShaderInfoVariant(ShaderInfo _info)
	{
		info = _info;
		Refresh ();
	}

	public void Refresh()
	{
		keywords = ShaderUtilHelper.ShaderGetAllKeywords (info.shader);
		variantCount = ShaderUtilHelper.GetVariantCount (info.shader, true);

		skips = ShaderTextProcess.GetAllKeywordsInclude (info.path);
		for (int i = 0; i < skips.Count; i++) {
			var word = skips [i];
			if (!keywords.Contains (word))
				keywords.Add (word);
		}
		keywords.Sort ();


		keywordsIfSkip.Clear ();
		for (int i = 0; i < keywords.Count; i++) {
			var word = keywords [i];
			keywordsIfSkip.Add (skips.Contains (word));
		}
	}

	public List<string> NewSkips()
	{
		List<string> newskips = new List<string> ();
		for (int i = 0; i < keywords.Count; i++) {
			if (keywordsIfSkip [i]) {
				newskips.Add (keywords [i]);
			}
		}
		return newskips;
	}
}

public class ShaderInfo{
	public string adbPath;
	public string path;
	public Shader shader;
	public ShaderInfoVariant variantInfo;

	public ShaderInfo(string _path)
	{
		path = _path;
		adbPath = path.Substring (Application.dataPath.Length-6);
	}

	public void OnSelect()
	{
		shader = AssetDatabase.LoadAssetAtPath<Shader> (adbPath);
		variantInfo = new ShaderInfoVariant (this);
	}
}
