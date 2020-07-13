using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public enum LineState
{
	none,
	comment,
	word
}

public class ShaderTextProcess{
	private static List<string> excludeList = new List<string>{ "#pragma", "//","/t","skip_variants" };
	public static List<string> GetAllKeywordsInclude(string path)
	{
		List<string> keywords = new List<string> ();

		var lines = File.ReadAllLines (path);
		for (int i = 0; i < lines.Length; i++) {
			LineState skipLineState = GetLineState (lines [i], "skip_variants");
			if (skipLineState == LineState.word) {
				char[] ch = new char[]{' ','\t'};
				var words = lines [i].Split (ch,StringSplitOptions.RemoveEmptyEntries);
				foreach (var word in words) {
					if (excludeList.Contains (word))
						continue;
					if (keywords.Contains (word))
						continue;
					keywords.Add (word);
				}
			}
		}
		return keywords;
	}

	public static void AddKeywords(string path,List<string> newSkips)
	{
		List<string> keywords = new List<string> ();

		//1 Read Lines
		var lines = File.ReadAllLines (path);
		List<string> lineList = new List<string> (lines);

		//2 Remove Old skip_variants 
		for (int i = lineList.Count - 1; i >= 0; i--) {
			if (lineList [i].Contains ("skip_variants"))
				lineList.RemoveAt (i);
		}

		//3 Add New skip_variants 
		if (newSkips.Count > 0) {
			int idCGPROGRAM = 0;
			int indexCGPROGRAM = 0;
			for (int i = 0; i < lines.Length; i++) {
				if (lines [i].IndexOf ("CGPROGRAM")>-1) {
					idCGPROGRAM = i;
					indexCGPROGRAM = lines [i].IndexOf ("CGPROGRAM");
				}
			}
			string newLine = string.Format("{0}{1}",lineList[idCGPROGRAM].Substring(0,indexCGPROGRAM),"#pragma skip_variants");
			newLine = AddSkips (newLine, newSkips);
			lineList.Insert (idCGPROGRAM + 1, newLine);
		}
		//4 Save
		File.WriteAllLines(path,lineList.ToArray());
	}

	private static LineState GetLineState(string line,string word)
	{
		int indexWord =  line.IndexOf (word);
		int indexComment = line.IndexOf ("//");
		if (indexWord == -1)
			return LineState.none;
		else if (indexComment > -1 && indexComment < indexWord)
			return LineState.comment;
		else
			return LineState.word;
	}

	private static string AddSkips(string line,List<string> newSkips)
	{
		foreach (var item in newSkips)
			line += " " + item;
		return line;
	}
}
