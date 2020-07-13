namespace LsyTextureCompressor
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;

	public enum LsyTextureType
	{
		tex,
		normal,
		ui,
		shadowmask,
		impostor
	}

	public enum LsyTextureImporterMode
	{
		none,
		auto,
		manual
	}

	public class LsyTextureImporter : AssetPostprocessor
	{
		public static LsyTextureImporterMode Mode = LsyTextureImporterMode.auto;
		public static TextureImporterPlatformSettings ManualSetting;
		void OnPreprocessTexture()
		{
			if (Mode == LsyTextureImporterMode.none) {
				return;
			}
			else {
				var importer = (assetImporter as TextureImporter);
				if (importer == null)
					return;

				if (Mode == LsyTextureImporterMode.auto) {
					OnPreprocessTextureAuto (importer);
				}
				else if (Mode == LsyTextureImporterMode.manual) {
					OnPreprocessTextureManual (importer);
				}
			}
		}

		void OnPreprocessTextureManual(TextureImporter importer)
		{
			if (ManualSetting != null) {
				importer.SetPlatformTextureSettings (ManualSetting);
			}
		}
		void OnPreprocessTextureAuto(TextureImporter importer)
		{	
			#region PathList
			//Debug.Log (importer.assetPath);
			GetPath();
			string folder = GetFolder(importer.assetPath);
			if(!path.Contains(folder))
				return;
			#endregion

			Dictionary<string,TextureImporterPlatformSettings> dic = new Dictionary<string, TextureImporterPlatformSettings> ();

			LsyTextureInfo info = new LsyTextureInfo (importer, assetPath);

			dic.Add("iPhone",LsyTextureCommon.Create_Setting(info,"iPhone",1));
			dic.Add("Android",LsyTextureCommon.Create_Setting(info,"Android",1));

			foreach (var item in dic) {
				var oldSettings = importer.GetPlatformTextureSettings (item.Key);
				if (IsSettingRight(oldSettings))
					continue;
				importer.SetPlatformTextureSettings (item.Value);
			}
		}

		public static bool IsSettingRight(TextureImporterPlatformSettings s)
		{
			var str = s.format.ToString ();
			return s.overridden == true && (str.Substring (0, 4) == "ASTC" || str.Substring (0, 3) == "EAC");
		}

		#region PathList
		public static List<string> path = new List<string>();
		public static void GetPath()
		{
			if (path.Count>0)
				return;
			string p = "/Artist/TARD/LSY/TextureCompressor/AutoCompressPathList.txt";
			string fullPath = Application.dataPath + p;
			//Debug.Log ("fullPath:" + fullPath);
			string txt = File.ReadAllText (fullPath);
			//Debug.Log ("txt:" + txt);

			txt = txt.Replace ("\r\n", "\n");
			txt = txt.Replace ("\t", "\n");
			string[] list = txt.Split ('\n');
			//Debug.Log ("list count:" + list.Length);
			path.AddRange (list);
		}

		public static string GetFolder(string fullPath)
		{
			int l = fullPath.LastIndexOf ("/");
			string s = fullPath.Substring (0, l);
			return s;
		}
		#endregion
	}
}