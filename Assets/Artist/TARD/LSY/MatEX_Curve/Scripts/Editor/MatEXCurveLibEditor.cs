using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

public class MatEXCurveLibEditor : EditorWindow {
	protected MatEXCurveLib lib; 
	protected MatEX matEX;
	protected int matEXIndex;
	protected Vector2 scrollPos;

	static float TitleWidth = 300;
	static float CurveWidth = 200;
	static float rowHeight = 50;
	Dictionary<string,AnimationCurve> equationCurves = new Dictionary<string, AnimationCurve> ();
//	[MenuItem("lsy/Create")]
//	public static void Create()
//	{
//		MatEXCurveLib lib = ScriptableObject.CreateInstance<MatEXCurveLib> ();
//		AssetDatabase.CreateAsset (lib,"Assets/CurveLib.asset");
//	}

	//[MenuItem("lsy/CurveLib")]
	public static void Show(MatEX _matEX,int index)
	{
		MatEXCurveLibEditor e = EditorWindow.GetWindow<MatEXCurveLibEditor> ();
		e.lib = AssetDatabase.LoadAssetAtPath<MatEXCurveLib> (MatEXConfig.CurveLibPath);
		e.matEX = _matEX;
		e.matEXIndex = index;
		e.titleContent = new GUIContent ("曲线库");
	}

	void OnGUI()
	{
		scrollPos = GUILayout.BeginScrollView (scrollPos);
		MatEX.ShowTitle ("曲线", 18);
		for (int i = 0; i < lib.data.Count; i++) {
			ShowCurve (i);
			GUILayout.Space (10);
		}
		MatEX.ShowTitle ("函数曲线", 18);
		ShowEquations ();

		GUILayout.EndScrollView ();

		if (GUILayout.Button ("编辑曲线库",GUILayout.Width(rowHeight*2),GUILayout.Height(rowHeight)))
		{
			Selection.activeObject = lib;
		}
	}

	void ShowCurve(int i)
	{
		LibCurveData data = lib.data [i];
		GUILayout.BeginHorizontal ();

		GUILayout.BeginVertical (GUILayout.Width (TitleWidth));
		ShowSubTitle (data.title);
		GUILayout.Label (data.description,GUILayout.Width(TitleWidth));
		GUILayout.EndVertical ();

		AnimationCurve c = new AnimationCurve (data.curve.keys);
		EditorGUILayout.CurveField (c,GUILayout.Width(CurveWidth),GUILayout.Height(rowHeight));
		if (GUILayout.Button ("选取",GUILayout.Width(rowHeight),GUILayout.Height(rowHeight)))
		{
			matEX.curves.curves [matEXIndex] = new AnimationCurve (data.curve.keys);
			matEX.Repaint ();
			Close ();
		}
		GUILayout.EndHorizontal ();
	}

	void ShowEquations()
	{
		Type t = typeof(MatEXEquationLib);
		var methods = t.GetMethods ();
		for (int i = 0; i < methods.Length; i++) {
			var method = methods [i];
			if (method.Name.Contains ("Equation")) {
				GUILayout.BeginHorizontal ();


				//Name and equation
				GUILayout.BeginVertical (GUILayout.Width (TitleWidth));
				var cs = method.GetCustomAttributes (false);
				foreach (var att in cs) {
					MatEXEquationAttAttribute e = (MatEXEquationAttAttribute)att;
					if (e != null) {
						ShowSubTitle (e.title);
						GUILayout.Label (e.description,GUILayout.Width(TitleWidth));
					} else {
						GUILayout.Label (method.Name);
					}
				}
				GUILayout.EndVertical ();

				//Curve
				if (equationCurves == null)
					equationCurves = new Dictionary<string, AnimationCurve> ();
				if (!equationCurves.ContainsKey (method.Name)) {
					AnimationCurve c = EquationToCurve (method);
					equationCurves.Add (method.Name, c);
				}

				EditorGUILayout.CurveField (equationCurves[method.Name],GUILayout.Width(CurveWidth),GUILayout.Height(rowHeight));
				if (GUILayout.Button ("选取",GUILayout.Width(rowHeight),GUILayout.Height(rowHeight)))
				{
					matEX.curves.curves [matEXIndex] = new AnimationCurve (equationCurves[method.Name].keys);
					matEX.Repaint ();
					Close ();
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.Space (10);
		}
	}

	#region common
	void ShowSubTitle(string str)
	{
		GUILayout.Label (string.Format("【{0}】",str,GUILayout.Width(TitleWidth)));
	}

	AnimationCurve EquationToCurve(MethodInfo method)
	{
		int seg = 500;
		Keyframe[] ks = new Keyframe[seg+1];
		for (int i = 0; i < ks.Length; i++) {
			float x = (float)i / seg;

			object[] param = new object[1];
			param [0] = x;
			float y = (float)method.Invoke (null, param);

			ks [i] = new Keyframe (x, y);
		}
		return new AnimationCurve (ks);
	}
	#endregion
}
