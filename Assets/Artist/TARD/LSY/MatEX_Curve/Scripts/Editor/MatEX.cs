using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


[Serializable]
public class MatCurves
{
	public AnimationCurve[] curves = new AnimationCurve[4];
}

[CustomEditor(typeof(Material))]
public class MatEX : MaterialEditor {
	public MatCurves curves;
	protected Material mat;
	protected MatEXConfig config;
	protected MatEXConfigShader configShader;
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		mat = (Material)target;
		Config ();
		if (configShader == null)
			return;

		GUILayout.Space (30);
		ShowTitle ("Curve Area");
		Load ();
		DrawCuves ();
	
		GUILayout.Space (10);
		if (GUILayout.Button ("烘焙",GUILayout.Height(26))) {
			Bake ();
		}
	}

	public static void ShowTitle(string title,int fontSize =22)
	{
		int size = GUI.skin.label.fontSize;
		var color = GUI.skin.label.normal.textColor;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.normal.textColor = new Color (0, 1, 1, 1);
	
		GUILayout.Label (title);
		GUI.skin.label.fontSize = size;
		GUI.skin.label.normal.textColor =  color;
	}
	protected void Config()
	{
		config = AssetDatabase.LoadAssetAtPath<MatEXConfig> (MatEXConfig.ConfigPath);
		foreach (var item in config.data) {
			if (item.shader == null)
				continue;
			if (item.shader.name == mat.shader.name)
				configShader = item;
		}
	}
	protected void Load()
	{
		if (curves == null) {
			string curveDataPath_File = CurveDataPath_File (mat);
			if (File.Exists (curveDataPath_File)) {
				string str = File.ReadAllText (curveDataPath_File);
				curves = JsonUtility.FromJson<MatCurves> (str);
			}
			else {
				Init ();
			}
		}
	}
	void Init()
	{
		curves = new MatCurves ();
		for (int i = 0; i < 4; i++) {
			var cr = new AnimationCurve ();
			curves.curves [i] = cr;

			if (i < 3) {
				cr.AddKey (new Keyframe (0f, 0.5f));
				cr.AddKey (new Keyframe (0.25f, 1f));
				cr.AddKey (new Keyframe (0.75f, 0f));
				cr.AddKey (new Keyframe (1f, 0.5f));
			} else {
				cr.AddKey (new Keyframe (0f, 1f));
				cr.AddKey (new Keyframe (1f, 1f));
			}
		}
	}
	protected void DrawCuves()
	{
		DrawCurve (configShader.RField, 0);
		DrawCurve (configShader.GField, 1);
		DrawCurve (configShader.BField, 2);
		DrawCurve (configShader.AField, 3);
	}

	void DrawCurve(string title,int i)
	{
		if (string.IsNullOrEmpty (title))
			return;
		float height = 30;
		GUILayout.BeginHorizontal ();
		GUILayout.Label (title, GUILayout.Height (height));
		curves.curves [i] = EditorGUILayout.CurveField (curves.curves [i],GUILayout.Width(90),GUILayout.Height (height));
		if (GUILayout.Button ("库",GUILayout.Width(32),GUILayout.Height (height))) {
			MatEXCurveLibEditor.Show (this,i);
		}
		GUILayout.EndHorizontal ();
	}
	protected void Bake()
	{
		//Path
		string curvePath_File = CurvePath_File(mat);
		string curvePath_ADB = CurvePath_ADB(mat);
		string curveDataPath_File = CurveDataPath_File(mat);
		string curveDataPath_ADB = CurveDataPath_ADB(mat);

		//Tex Create
		Texture2D tex = CurveToTexture (curves.curves, 128);

		//Tex Save
		var bs = tex.EncodeToPNG ();
		File.WriteAllBytes (curvePath_File, bs);

		//Tex set to mat
		AssetDatabase.ImportAsset (curvePath_ADB);
		tex = AssetDatabase.LoadAssetAtPath<Texture2D> (curvePath_ADB);
		mat.SetTexture ("_CurveTex", tex);

		//Curve save
		string str = JsonUtility.ToJson (curves);
		File.WriteAllText(curveDataPath_File,str);
		AssetDatabase.ImportAsset (curveDataPath_ADB);
	}


	#region common
	protected string CurvePath_File(Material mat)
	{
		string adbPath = CurvePath_ADB (mat);
		string path = string.Format("{0}/{1}",
			Application.dataPath.Substring(0,Application.dataPath.Length-7),adbPath);
		return path;
	}
	protected string CurvePath_ADB(Material mat)
	{
		string matPath = AssetDatabase.GetAssetPath (mat);
		string adbPath = string.Format("{0}_Curve.png",
			matPath.Substring (0, matPath.Length - 4)
		);
		return adbPath;
	}
	protected string CurveDataPath_File(Material mat)
	{
		string adbPath = CurveDataPath_ADB (mat);
		string path = string.Format("{0}/{1}",
			Application.dataPath.Substring(0,Application.dataPath.Length-7),adbPath);
		return path;
	}
	protected string CurveDataPath_ADB(Material mat)
	{
		string matPath = AssetDatabase.GetAssetPath (mat);
		string adbPath = string.Format("{0}_CurveData.txt",
			matPath.Substring (0, matPath.Length - 4)
		);
		return adbPath;
	}

	protected Texture2D CurveToTexture(AnimationCurve[] curves,int size = 2048)
	{
		var gradTexture = new Texture2D(size, 8, TextureFormat.RGBA32, false);
		gradTexture.wrapMode = TextureWrapMode.Clamp;
		gradTexture.filterMode = FilterMode.Bilinear;

		for (var i = 0; i < gradTexture.width; i++)
		{
			var x = 1.0f / gradTexture.width * i;

			var r = curves[0].Evaluate(x);
			var g = curves[1].Evaluate(x);
			var b = curves[2].Evaluate(x);
			var a = curves[3].Evaluate(x);

			for (var j = 0; j < gradTexture.height; j++)
			{
				gradTexture.SetPixel(i, j, new Color(r,g,b,a));
			}
		}
		gradTexture.Apply();
		return gradTexture;
	}
	#endregion
}
