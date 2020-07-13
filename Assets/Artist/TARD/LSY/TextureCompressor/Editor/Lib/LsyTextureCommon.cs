namespace LsyTextureCompressor
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public class LsyTextureCommon {
		private static Dictionary<LsyTextureType,List<TextureImporterFormat>> formatDic;
		private static Dictionary<LsyTextureType,List<TextureImporterFormat>> FormatDic
		{
			get{ 
				if (formatDic == null) {
					formatDic = new Dictionary<LsyTextureType, List<TextureImporterFormat>> ();
					formatDic.Add(LsyTextureType.tex,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGB_10x10,TextureImporterFormat.ASTC_RGB_8x8,TextureImporterFormat.ASTC_RGB_6x6});
					formatDic.Add(LsyTextureType.normal,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGB_8x8,TextureImporterFormat.ASTC_RGB_6x6,TextureImporterFormat.ASTC_RGB_4x4});
					formatDic.Add(LsyTextureType.ui,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGB_6x6,TextureImporterFormat.ASTC_RGB_4x4,TextureImporterFormat.RGB24});
					formatDic.Add(LsyTextureType.shadowmask,new List<TextureImporterFormat>{TextureImporterFormat.EAC_R,TextureImporterFormat.EAC_R,TextureImporterFormat.EAC_R});
					formatDic.Add(LsyTextureType.impostor,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGB_10x10,TextureImporterFormat.ASTC_RGB_8x8,TextureImporterFormat.ASTC_RGB_6x6});
                    //formatDic.Add(LsyTextureType.lightmap, new List<TextureImporterFormat> { TextureImporterFormat.ASTC_RGB_10x10, TextureImporterFormat.ASTC_RGB_8x8, TextureImporterFormat.ASTC_RGB_6x6 });
                }
				return formatDic;
			}
		}
		private static Dictionary<LsyTextureType,List<TextureImporterFormat>> formatDicAlpha;
		private static Dictionary<LsyTextureType,List<TextureImporterFormat>> FormatDicAlpha
		{
			get{ 
				if (formatDicAlpha == null) {
					formatDicAlpha = new Dictionary<LsyTextureType, List<TextureImporterFormat>> ();
					formatDicAlpha.Add(LsyTextureType.tex,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGBA_8x8,TextureImporterFormat.ASTC_RGBA_6x6,TextureImporterFormat.ASTC_RGBA_4x4});
					formatDicAlpha.Add(LsyTextureType.normal,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGBA_8x8,TextureImporterFormat.ASTC_RGBA_6x6,TextureImporterFormat.ASTC_RGBA_4x4});
					formatDicAlpha.Add(LsyTextureType.ui,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGBA_6x6,TextureImporterFormat.ASTC_RGBA_4x4,TextureImporterFormat.RGBA32});
					formatDicAlpha.Add(LsyTextureType.shadowmask,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGBA_6x6,TextureImporterFormat.ASTC_RGBA_6x6, TextureImporterFormat.ASTC_RGBA_6x6 });
					formatDicAlpha.Add(LsyTextureType.impostor,new List<TextureImporterFormat>{TextureImporterFormat.ASTC_RGBA_8x8,TextureImporterFormat.ASTC_RGBA_6x6,TextureImporterFormat.ASTC_RGBA_4x4});
                    //formatDicAlpha.Add(LsyTextureType.lightmap, new List<TextureImporterFormat> { TextureImporterFormat.ASTC_RGBA_10x10, TextureImporterFormat.ASTC_RGBA_8x8, TextureImporterFormat.ASTC_RGBA_6x6 });
                }
				return formatDicAlpha;
			}
		}
		public static TextureImporterFormat GetFormat(LsyTextureInfo info,int i)
		{
			if(info.containAlpha)
				return FormatDicAlpha [info.type] [i];
			else
				return FormatDic [info.type] [i];
		}
		public static int GetTexQuality(LsyTextureInfo info,TextureImporterFormat format)
		{
			var dic = info.containAlpha ? LsyTextureCommon.FormatDicAlpha : LsyTextureCommon.FormatDic;

			if (dic.ContainsKey (info.type)) {
				var list = dic [info.type];
				for (int i = 0; i < list.Count; i++) {
					if (list [i] == format)
						return i;
				}
			} else {
				return -1;
			}
			return -1;
		}


		public static TextureImporterPlatformSettings Create_Setting(LsyTextureInfo info,string plat,int quality)
		{
			var oldSettings = info.GetPlatformSetting (plat);
			TextureImporterFormat format = GetFormat (info, quality);

			TextureImporterPlatformSettings s = new TextureImporterPlatformSettings ();
			s.name = plat;
			s.overridden = true;

			s.maxTextureSize = oldSettings.maxTextureSize;
			s.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
			s.format = format;
			s.compressionQuality = (int)TextureCompressionQuality.Best;

            if (info.type==LsyTextureType.shadowmask)
            {
                s.compressionQuality= (int)TextureCompressionQuality.Normal;
            }

			s.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;

			//s.allowsAlphaSplitting = true;
			//s.textureCompression = TextureImporterCompression.Uncompressed;
			//s.crunchedCompression = true;

			if (info.type == LsyTextureType.impostor)
				info.importer.mipmapEnabled = false;
			return s;
		}
		public static TextureImporter GetTextureImporter(Texture t)
		{
			string path = AssetDatabase.GetAssetPath (t);
			return GetTextureImporter(path);
		}
		public static TextureImporter GetTextureImporter(string path)
		{
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath (path);
			return importer;
		}
		public static bool HasTextureImporter(Texture t)
		{
			string path = AssetDatabase.GetAssetPath (t);
			var importer = AssetImporter.GetAtPath (path);
			return importer is TextureImporter;
		}



		public static Texture[] GetSceneTextures()
		{
			List<Texture> allScenesTex = new List<Texture> ();

			List<string> allScenes = new List<string> ();
			foreach (var item in Selection.objects) {
				string path = AssetDatabase.GetAssetPath (item);
				//Debug.Log (path);
				if (path.EndsWith (".unity"))
					allScenes.Add (path);
			}

			string[] v = AssetDatabase.GetDependencies (allScenes.ToArray ());
			for(int i=0;i<v.Length;i++)
			{
				string path = v[i];
				EditorUtility.DisplayProgressBar ("Searching", "Searching Textures", 0.5f * i / v.Length);

				if (AssetImporter.GetAtPath (path) is TextureImporter) {
					Texture t = AssetDatabase.LoadAssetAtPath<Texture> (path);
					if (t == null) {
						Debug.LogError ("不存在Texture:"+path);
						continue;
					}
					allScenesTex.Add (t);
				}
			}

			return allScenesTex.ToArray();
		}
	}
}