namespace LsyTextureCompressor
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;


	public class LsyTextureInfoPlatform
	{
		public LsyTextureInfo info;
		public string platform;

		public TextureImporterPlatformSettings setting;
		public int size;
		public int quality;
		public int width;
		public int height;

		public LsyTextureInfoPlatform(LsyTextureInfo _info,string _platform)
		{
			info = _info;
			platform = _platform;
			Refresh ();
		}

		public void Refresh()
		{
			setting = info.importer.GetPlatformTextureSettings (platform);

			if (info.tex != null) {
				SetWidthHeight ();
				size = LsyTextureSizeCal.CalculateTextureSizeBytes (info.tex,width,height,setting.format,info.importer.isReadable);
			}
			quality = LsyTextureCommon.GetTexQuality (info,setting.format);
		}

		void SetWidthHeight()
		{
			width = info.tex.width;
			height = info.tex.height;
			float max = Mathf.Max (width, height);
			if (max > setting.maxTextureSize) {
				if (width == height) {
					width = setting.maxTextureSize;
					height = setting.maxTextureSize;
				}
				else if (width > height) {
					height = (int)((float)height * ((float)setting.maxTextureSize / width));
					width = setting.maxTextureSize;
				} else {
					width = (int)((float)width * ((float)setting.maxTextureSize / height));
					height = setting.maxTextureSize;
				}
			} 
		}
	}

	public class LsyTextureInfo
	{
		public Texture tex;
		public TextureImporter importer; 
		public string assetPath;

		public bool containAlpha;
		public LsyTextureType type;
		public List<LsyTextureInfoPlatform> platformInfos= new List<LsyTextureInfoPlatform>();

		public LsyTextureInfo(Texture t)
		{
			tex = t;
			Refresh ();
		}
		public LsyTextureInfo(TextureImporter _importer,string _assetPath)
		{
			importer = _importer;
			assetPath = _assetPath;
			Refresh (false);
		}

		public LsyTextureInfoPlatform GetLsyTextureInfoPlatform(string platform)
		{
			for (int i = 0; i < platformInfos.Count; i++) {
				if (platformInfos [i].platform == platform)
					return platformInfos [i];
			}
			return null;
		}
		public TextureImporterPlatformSettings GetPlatformSetting(string platform)
		{
			var p = GetLsyTextureInfoPlatform (platform);
			return p.setting;
		}

		public void Refresh(bool refreshAll = true)
		{
			if (refreshAll) {
				assetPath = AssetDatabase.GetAssetPath (tex);
				importer = LsyTextureCommon.GetTextureImporter (tex);
			}
			containAlpha = importer.alphaSource != TextureImporterAlphaSource.None;
			//Special Op for exr
			if (assetPath.EndsWith (".exr")) {
				containAlpha = false;
			} 


			if (importer.textureType == TextureImporterType.Sprite) {
				type = LsyTextureType.ui;
			} 
			else if (importer.textureType == TextureImporterType.NormalMap) {
				type = LsyTextureType.normal;
			} 
			else if (assetPath.EndsWith ("_shadowmask.png")) {
				type = LsyTextureType.shadowmask;
			} 
			else if (assetPath.Contains ("Impostor")) {
				type = LsyTextureType.impostor;
			} 
			else {
				type = LsyTextureType.tex;
			}

			platformInfos.Clear ();
			platformInfos.Add(new LsyTextureInfoPlatform(this,"iPhone"));
			platformInfos.Add(new LsyTextureInfoPlatform(this,"Android"));
		}
	}
}