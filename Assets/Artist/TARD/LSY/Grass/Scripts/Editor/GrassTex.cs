using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
public class GrassTex{
	public static string folderPath;
	public static int textureSize
	{
		get{ 
			return builder.settings.genTextureSize;
		}
	}
	public static Texture2D tex;
	public static Texture2D texLM;
	public static Texture2D texSM;
	public static GrassBuilder builder;
	public static float heightMin;
	public static float heightMax;

	public static void GenTex(GrassBuilder _builder)
	{
		TexReimport_Reads (true);
		heightMin = float.MaxValue;
		heightMax = float.MinValue;

		builder = _builder;
		var settings = builder.settings;
		GetFolderPath ();

		tex = CreateTex (new Color(0,0,0.5f,0.5f));
		texLM =  CreateTex (new Color(0,0,0,0));
		texSM =  CreateTex (new Color(0,0,0,0));
		//Height Min Max
		for (int i = 0; i < textureSize; i++) {
			for (int j = 0; j < textureSize; j++) {
				Vector3 pos = GetPos (i, j, settings.MapCenter, settings.MapSize);
				CalHeightMinMax (i, j, pos);
			}
		}
		builder.data.heightMin = heightMin;
		builder.data.heightMax = heightMax;


		//Tex
		for (int i = 0; i < textureSize; i++) {
			for (int j = 0; j < textureSize; j++) {
				Vector3 pos = GetPos (i, j, settings.MapCenter, settings.MapSize);
				ProcessPixel (i, j, pos);
			}
		}




		#region save all
		SaveTex(ref tex,"grassInfo",true,TextureImporterType.Default,"png");



		SaveTex(ref texLM,"grassInfo_LM",false,TextureImporterType.Lightmap,"exr");
		SaveTex(ref texSM,"grassInfo_SM",false,TextureImporterType.SingleChannel,"png");
		#endregion
		SetMats ();

		TexReimport_Reads (false);
		builder.SetProgress (1);
	}
	static Texture2D CreateTex(Color defaultColor)
	{
		var t = new Texture2D (textureSize, textureSize,TextureFormat.RGBAFloat,false);
		var cs = t.GetPixels ();
		for(int i=0;i<cs.Length;i++)
		{
			cs [i] = defaultColor;
		}
		t.SetPixels (cs);
		return t;
	}


	static void GetFolderPath()
	{
		string path = Application.dataPath;
		path = path.Substring (0, path.Length - 6);
		var scene = SceneManager.GetActiveScene ();
		path += scene.path;
		path = path.Substring (0, path.Length - 6);
		folderPath = path;
	}

	private static Vector3 GetPos(int i,int j,Vector3 center,float size)
	{
		float xPcg = (float)i / textureSize;
		float zPcg = (float)j / textureSize;

		float x = center.x - size + size*2*xPcg;
		float z = center.z - size + size*2*zPcg;
		float y = Camera.main.transform.position.y + 500;
		Vector3 pos = new Vector3 (x, y, z);
		return pos;
	}

	private static void CalHeightMinMax(int i,int j,Vector3 pos)
	{
		Vector2 texUV;
		float channel = 0;
		RaycastHit mHitInfo;
		Ray mRay = new Ray ();
		mRay.origin = pos;
		mRay.direction = Vector3.down;

		float rate=0;
		float height=0;

		//Todo add layermask after [grass placement tex] fixed
		if (Physics.Raycast (mRay, out mHitInfo, 2000, 1 << 28 | 1 << 17)) {
			//if (Physics.Raycast (mRay, out mHitInfo, 2000)) {
			TerrainGrassInfo info = mHitInfo.collider.GetComponentInParent<TerrainGrassInfo> ();
			if (info == null || info.grassTex == null) {

			} else {
				height = mHitInfo.point.y;
				if (height < heightMin)
					heightMin = height;
				if (height > heightMax)
					heightMax = height;
			}
		}

	}
	private static void ProcessPixel(int i,int j,Vector3 pos)
	{
		Vector2 texUV;
		Vector2 lmUV;
		float channel = 0;
		RaycastHit mHitInfo;
		Ray mRay = new Ray ();
		mRay.origin = pos;
		mRay.direction = Vector3.down;

		float rate=0;
		float height=0;

		//Todo add layermask after [grass placement tex] fixed
		if (Physics.Raycast (mRay, out mHitInfo, 2000, 1 << 28 | 1 << 17)) {
			//if (Physics.Raycast (mRay, out mHitInfo, 2000)) {
			TerrainGrassInfo info = mHitInfo.collider.GetComponentInParent<TerrainGrassInfo> ();
			if (info == null || info.grassTex == null) {
				rate = 0;
				height = 0;
			} else {
				texUV = mHitInfo.textureCoord;
				lmUV = mHitInfo.lightmapCoord;

				rate = info.grassTex.GetPixel ((int)(texUV.x * (float)info.width), (int)(texUV.y * (float)info.height)).r;
				height = (mHitInfo.point.y - heightMin)/(heightMax-heightMin);

				int lmIndex = mHitInfo.collider.GetComponentInChildren<MeshRenderer> ().lightmapIndex;
				if (lmIndex >= 0) {
					//Color cc = new Color (lmUV.x, lmUV.y, 0, 1);
					var lm =  LightmapSettings.lightmaps[lmIndex].lightmapColor;
					var sm =  LightmapSettings.lightmaps[lmIndex].shadowMask;
					int uvX = (int)(lmUV.x * lm.width);
					int uvY = (int)(lmUV.y * lm.height);

					Color cc = lm.GetPixel (uvX, uvY);
					texLM.SetPixel (i, j, cc);

					Color shadow = sm.GetPixel (uvX, uvY);
					texSM.SetPixel (i, j, shadow);
				}
			}
		}

		var normal = mHitInfo.normal;
		Color c = new Color (rate, height, normal.x*0.5f+0.5f,normal.z*0.5f+0.5f);
		tex.SetPixel (i, j, c);
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

	static void SetMats()
	{
		builder.data.texSpawnMap = tex;
		builder.data.texLM = texLM;
		builder.data.texSM = texSM;
		foreach (var lod in  builder.data.lods) {
			SetMat (lod.mat);
		}
	}

	static void SetMat(Material mat)
	{
		if (mat == null)
			return;

		mat.SetTexture ("_lmap", texLM);
		mat.SetTexture ("_smap", texSM);

		var c = builder.settings.MapCenter;
		var s = builder.settings.MapSize;
		mat.SetVector ("_MapInfo", new Vector4 (c.x, c.y, c.z, s));
	}

	#region Tex format
	static void SaveTex(ref Texture2D t,string name,bool read, TextureImporterType type, string encode)
	{
		if (t == null)
			return;
		byte[] bs = null;
		string path = "";
		if (encode == "png") {
			bs = t.EncodeToPNG ();
			path = string.Format ("{0}/{1}.png", folderPath, name);
		}
		else if (encode == "exr") {
			bs = t.EncodeToEXR ();
			path = string.Format ("{0}/{1}.exr", folderPath, name);
		}

		File.WriteAllBytes (path, bs);

		int p = Application.dataPath.Length - 6;
		string pathADB = path.Substring (p, path.Length - p);
		Debug.Log ("pathADB:" + pathADB);
		AssetDatabase.ImportAsset (pathADB);
		t = (Texture2D)AssetDatabase.LoadMainAssetAtPath (pathADB);
		var im = GetTextureImporter (t);
		im.mipmapEnabled = false;
		im.isReadable = read;
		im.wrapMode = TextureWrapMode.Clamp;
		im.sRGBTexture = false;
		im.textureType = type;
		im.textureCompression = TextureImporterCompression.Uncompressed;
		AssetDatabase.ImportAsset (pathADB);
	}
	#endregion

	#region LM SM set Readable 
	static void TexReimport_Reads(bool read)
	{
		for (int i = 0; i < LightmapSettings.lightmaps.Length; i++) {
			var ls = LightmapSettings.lightmaps [i];
			TexReimport_Read (ls.lightmapColor, read);
			TexReimport_Read (ls.lightmapDir, read);
			TexReimport_Read (ls.shadowMask, read);
		}
	}
	static void TexReimport_Read(Texture2D t,bool read)
	{
		if (t == null)
			return;
		var im = GetTextureImporter (t);
		im.isReadable = read;
		var pathADB = AssetDatabase.GetAssetPath (t);
		AssetDatabase.ImportAsset (pathADB);
	}
	#endregion
}
