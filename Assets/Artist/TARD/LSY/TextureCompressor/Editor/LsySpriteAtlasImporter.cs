namespace LsyTextureCompressor
{
	using UnityEngine;
	using UnityEngine.U2D;
	using UnityEditor;
	using UnityEditor.U2D;
	using System.Collections;
	using System.Collections.Generic;

	public class LsySpriteAtlasImporter : AssetPostprocessor
	{
		//static void OnPostprocessAllAssets(string[] importedAssets, string[] deleteAssets, string[] moveAssets, string[] moveFromAssetPaths)
		//{
		//	if (LsyTextureManager.Instance!=null && LsyTextureImporter.Mode == LsyTextureImporterMode.manual)
		//		return;
		//	for (int i = 0; i < importedAssets.Length; i++)
		//	{
		//		if (importedAssets[i].EndsWith(".spriteatlas")) { 
		//			OnPostprocessSpriteAtlas(importedAssets[i]); 
		//		}
		//	}
		//}
		static void OnPostprocessSpriteAtlas(string path)
		{
			SpriteAtlas sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
			if (sa != null)
			{
				sa.SetIncludeInBuild(false);

				SpriteAtlasPackingSettings saps = new SpriteAtlasPackingSettings();
				saps.enableRotation = false;
				saps.enableTightPacking = false;
                saps.padding = 4;
                saps.blockOffset = 1;
                sa.SetPackingSettings(saps);

				SpriteAtlasTextureSettings sats = new SpriteAtlasTextureSettings();
				sats.readable = false;
				sats.generateMipMaps = false;
				sats.sRGB = true;
				sats.filterMode = FilterMode.Bilinear;
				sa.SetTextureSettings(sats);

				SettingPlat (sa, "iPhone");
				SettingPlat (sa, "Android");
			}
		}

		static void SettingPlat(SpriteAtlas sa, string plat)
		{
			//设置iphone平台的贴图压缩
			TextureImporterPlatformSettings setting = sa.GetPlatformSettings(plat);
			if (setting == null)
			{
				setting = new TextureImporterPlatformSettings();
			}

			setting.overridden = true;
			setting.format = TextureImporterFormat.ASTC_RGBA_4x4;
			setting.compressionQuality = 100;
			sa.SetPlatformSettings(setting);
		}
	}
}