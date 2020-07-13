using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Which shader use curves's R/G/B/A
/// </summary>
[Serializable]
public class MatEXConfigShader
{
	public Shader shader;
	public string RField;
	public string GField;
	public string BField;
	public string AField;
}
public class MatEXConfig:ScriptableObject{
	public static string ConfigPath = "Assets/Artist/TARD/LSY/MatEX_Curve/Config.asset";
	public static string CurveLibPath = "Assets/Artist/TARD/LSY/MatEX_Curve/Lib/CurveLib.asset";

	public List<MatEXConfigShader> data; 
}
