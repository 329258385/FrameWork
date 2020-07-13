using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;
using System.Reflection;
using UnityEditor;
using System;

public class ShaderUtilHelper{
	public static List<string> ShaderGetAllKeywords(Shader shader)
	{
		int[] passTypes;
		string[] keywordLists;
		ShaderVariantCollection svc = new ShaderVariantCollection ();
		GetShaderVariantEntries (shader, svc, out passTypes, out keywordLists);
		return KeywordCombine_GetKeywords (keywordLists);
	}

	private static void GetShaderVariantEntries(Shader shader,ShaderVariantCollection svc,out int[] passTypes,out string[] keywordLists)
	{
		Type t = typeof(ShaderUtil);
		MethodInfo m =  t.GetMethod ("GetShaderVariantEntries",BindingFlags.Static|BindingFlags.NonPublic);
		object[] s = new object[4];
		s [0] = shader;
		s [1] = svc;
		m.Invoke (null, s);

		passTypes = (int[])s [2];
		keywordLists = (string[])s [3];
	}

	private static List<string> KeywordCombine_GetKeywords(string[] combines)
	{
		List<string> keywords = new List<string> ();
		foreach (var combine in combines) {
			foreach (var word in combine.Split(' ')) {
				if (string.IsNullOrEmpty (word) || keywords.Contains (word))
					continue;
				keywords.Add (word);
			}
		}
		return keywords;
	}

	public static ulong GetVariantCount(Shader shader,bool usedBySceneOnly)
	{
		Type t = typeof(ShaderUtil);
		MethodInfo m =  t.GetMethod ("GetVariantCount",BindingFlags.Static|BindingFlags.NonPublic);
		object[] s = new object[2];
		s [0] = shader;
		s [1] = usedBySceneOnly;
		ulong v = (ulong)m.Invoke (null, s);
		return v;
//		[UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
//		[System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
//		extern internal static  ulong GetVariantCount (Shader s, bool usedBySceneOnly) ;
	}


	public static MethodInfo[] GetAllMethod<T>()
	{
		Type t = typeof(T);
		var ms = t.GetMethods (BindingFlags.Static|BindingFlags.NonPublic);
		return ms;
	}
}
