using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;

public enum GrassBrushMode
{
	Paint,
	Erase
}

[Serializable]
public class GrassBrush
{
	public GrassBrushMode mode = GrassBrushMode.Paint;
	[Range(0.2f,100f)]
	public float size = 10;
	[Range(0f,1f)]
	public float force = 1;
	public bool soft = true;
	[Range(0f,1f)]
	public float softEdge = 0.5f;
	[HideInInspector]
	public float texPaint01Size;
}

public static class GrassTextureProcess{
	public static void TexDraw(Texture2D t2d,Vector2 uv,Color color,int range)
	{
		int w = t2d.width;
		int h = t2d.height;

		int x = (int)((float)w * uv.x);
		int y = (int)((float)w * uv.x);

		//for(int )
	}


	public static void DrawPoint(Texture2D tex, Vector2 uv,GrassBrush brush)
	{
		int w = tex.width;
		int h = tex.height;
		int x = 0;
		int y = 0;
		Undo.RegisterCompleteObjectUndo (tex, "");
		var colorStartBuffer = tex.GetPixels ();
		TexUV2Index (w, h, uv, ref x, ref y);
		int count = (int)(brush.texPaint01Size * w * 1.5f);


		for (int i = -count; i < count; i++) {
			for (int j = -count; j < count; j++) {
				int _x = x + i;
				int _y = y + j;
				if (_x < 0 || _x >= w)
					continue;
				if (_y < 0 || _y >= h)
					continue;

				Vector2 uv2 = TexUV (w, h,_x ,_y);
				float dis = Vector2.Distance (uv, uv2);

				if (dis < brush.texPaint01Size) {
					int index = XYtoIndex (w, h, _x, _y);
					var c = colorStartBuffer [index];
					float f = 0;
					float mod = brush.force;
					if(brush.soft)
					{
						float pcg = dis / brush.texPaint01Size;
						mod *=  1- Mathf.Clamp01(pcg-(1-brush.softEdge)) /brush.softEdge ;
					}
					if (brush.mode == GrassBrushMode.Paint) {
						f = c.r+mod;
					}
					else if (brush.mode == GrassBrushMode.Erase) {
						f = c.r-mod;
					}
					f = Mathf.Clamp01 (f);


					colorStartBuffer [index] = new Color(f,f,f,f);
				}
			}
		}
		tex.SetPixels (colorStartBuffer);
		tex.Apply ();



//		colorStartBuffer = tex.GetPixels ();
//		brushBuffer = new float[tex.width, tex.height];
//
//		SWUndo.RegisterCompleteObjectUndo (tex);
//		SWTexThread_TexDrawPoint t = new SWTexThread_TexDrawPoint (tex, _brush);
//		t.Process (_uv);
	}




	public static int TexUV2Index(int width,int height,Vector2 uv)
	{
		int x = 0;
		int y = 0;
		TexUV2Index (width, height, uv, ref x, ref y);
		return (height - y - 1) * width + x;
	}
	public static int XYtoIndex(int width,int height,int x,int y)
	{
		return (height - y - 1) * width + x;
	}

	public static void TexUV2Index(int width,int height,Vector2 uv,ref int x,ref int y)
	{
		x = Mathf.RoundToInt(uv.x * (float)width);
		y = Mathf.RoundToInt(uv.y * (float)height);

		x = Mathf.Clamp (x, 0, width-1);
		y = Mathf.Clamp (y, 0, height-1);
	}
	public static void TexUV2IndexNoClamp(int width,int height,Vector2 uv,ref int x,ref int y)
	{
		float xx = uv.x;
		float yy = uv.y;
		xx = xx > 0f ? xx % 1f : 1f - Mathf.Abs (xx) % 1f;
		yy = yy > 0f ? yy % 1f : 1f - Mathf.Abs (yy) % 1f;

		x = Mathf.RoundToInt(xx * (float)width);
		y = Mathf.RoundToInt(yy * (float)height);
		x = Mathf.Clamp (x, 0, width-1);
		y = Mathf.Clamp (y, 0, height-1);
	}


	public static Vector2 TexUV(float width,float height,float x,float y)
	{
		x = Mathf.Clamp (x, 0, width-1);
		y = Mathf.Clamp (y, 0, height-1);
		return new Vector2 (x/width, y/height);
	}

	//lsy : new remap ratio
	public static Vector2 TexUV_NoClamp(float width,float height,float x,float y)
	{
		return new Vector2 (x/width, y/height);
	}


	public static void TextureReImport( string adbPath,bool alphaIsTransparency = true)
	{
		var tImporter = AssetImporter.GetAtPath( adbPath ) as TextureImporter;

		TextureImporterSettings setings = new TextureImporterSettings();
		tImporter.ReadTextureSettings (setings);

		if ( tImporter != null )
		{
			if (setings.readable == true && tImporter.isReadable == true &&  tImporter.mipmapEnabled == false   
				&&  tImporter.npotScale == TextureImporterNPOTScale.None
				//	&&  tImporter.textureFormat == TextureImporterFormat.ARGB32
				&&  tImporter.textureCompression == TextureImporterCompression.Uncompressed
				&&  tImporter.alphaIsTransparency == alphaIsTransparency
				//	&&  tImporter.normalmap == false
			)
				return;

			setings.readable = true;
			tImporter.SetTextureSettings (setings);

			//iNormal
			if (tImporter.textureType == TextureImporterType.NormalMap) {
				tImporter.isReadable = true;
			} else {
				//tImporter.textureType = TextureImporterType.Default;
				//tImporter.normalmap = false;
				tImporter.isReadable = true;
				tImporter.npotScale = TextureImporterNPOTScale.None;
				//tImporter.textureFormat = TextureImporterFormat.ARGB32;
				tImporter.textureCompression = TextureImporterCompression.Uncompressed;
				tImporter.mipmapEnabled = false;
				tImporter.alphaIsTransparency = alphaIsTransparency;
			}
			AssetDatabase.ImportAsset( adbPath);
			AssetDatabase.Refresh ();  
		}
	}
}
#endif