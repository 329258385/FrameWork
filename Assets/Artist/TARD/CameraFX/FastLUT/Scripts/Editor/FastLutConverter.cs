using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public static class FastLutConverter
{
	[MenuItem("Assets/Create/FastLUT/Convert 2D LUT Texture to FastLUT")]
	private static void ConvertSelectionTo3DTexture()
	{
		Texture2D texture = Selection.activeObject as Texture2D;
		if(texture == null)
			return;

		ConvertTexture2DTo3DAndSave(texture);
	}

	[MenuItem("Assets/Create/FastLUT/Convert 2D LUT Texture to FastLUT", true)]
	private static bool ConvertSelectionTo3DTextureValidation()
	{
		return Selection.activeObject && Selection.activeObject.GetType() == typeof(Texture2D);
	}

	[MenuItem("Assets/Create/FastLUT/Create neutral LUT 2D Texture - 32 pix")]
	private static void CreateNeutralTexture32()
	{
		string path = GetPathForNeutralTexture();
		CreateNeutralLUTTexture2D(path, 32);
	}

	[MenuItem("Assets/Create/FastLUT/Create neutral LUT 2D Texture - 16 pix")]
	private static void CreateNeutralTexture16()
	{
		string path = GetPathForNeutralTexture();
		CreateNeutralLUTTexture2D(path, 16);
	}

	private static string GetPathForNeutralTexture()
	{
		var selection = Selection.activeObject;
		string path = AssetDatabase.GetAssetPath(selection).Remove(0, "Assets".Length);
		int pointIndex = path.LastIndexOf('.');
		int slashIndex = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
		if(pointIndex > slashIndex)
		{
			path = path.Remove(slashIndex);
		}
		path = Application.dataPath + path + "/NeutralLUTTexture.png";
		return path;
	}

	private static void CreateNeutralLUTTexture2D(string path, int textureSize)
	{
		Texture2D texture2D = new Texture2D(textureSize * textureSize, textureSize, TextureFormat.RGB24, false, true);
		Color[] pixels = new Color[textureSize * textureSize * textureSize];
		float maxPixelVal = (float)(textureSize - 1);
		for(int x = 0; x < textureSize; x++)
		{
			for(int y = 0; y < textureSize; y++)
			{
				for(int z = 0; z < textureSize; z++)
				{
					int pixel2DIndex = x + y * textureSize * textureSize + z * textureSize;
					pixels[pixel2DIndex] = new Color(x / maxPixelVal, y / maxPixelVal, z / maxPixelVal);
				}
			}
		}
		texture2D.SetPixels(pixels);
		texture2D.alphaIsTransparency = false;
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(path, bytes);
		Debug.Log("Saved texture to " + path);
		AssetDatabase.Refresh();
	}

	private static void ConvertTexture2DTo3DAndSave(Texture2D texture)
	{
		Texture3D texture3D = ConvertTexture2DTo3D(texture);
		if(texture3D == null)
			return;

		string path = AssetDatabase.GetAssetPath(texture);
		int extensionIndex = path.LastIndexOf('.');
		path = path.Remove(extensionIndex);
		path += "_LUT3D.asset";
		AssetDatabase.CreateAsset(texture3D, path);
		AssetDatabase.SaveAssets();
		Debug.Log("Saved asset to " + path);
	}

	private static Texture3D ConvertTexture2DTo3D(Texture2D texture)
	{
		if(texture == null)
		{
			Debug.LogError("Unable to convert null texture");
			return null;
		}

		if(texture.width * texture.width != texture.height
			&& texture.height * texture.height != texture.width)
		{
			Debug.LogError("Wrong LUT texture format, Height == Width^2 (vertical LUT) or Width == Height^2 (horizontal LUT), like usually in 2D LUT textures");
			return null;
		}

		string assetPath = AssetDatabase.GetAssetPath(texture);
		TextureImporter importer = TextureImporter.GetAtPath(assetPath) as TextureImporter;
		if(importer.mipmapEnabled || importer.sRGBTexture 
			|| importer.crunchedCompression || importer.textureCompression != TextureImporterCompression.Uncompressed)
		{
			importer.mipmapEnabled = false;
			importer.sRGBTexture = false;
			importer.crunchedCompression = false;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
		}

		bool wasTextureReadable = importer.isReadable;
		if(!wasTextureReadable)
		{
			importer.isReadable = true;
			AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
		}

		bool verticalLUTMode = texture.width < texture.height;
		int textureSize = verticalLUTMode ? texture.width : texture.height;

		Color[] tex2DPixels = texture.GetPixels();
		Texture3D tex3D = new Texture3D(textureSize, textureSize, textureSize, TextureFormat.RGB24, false);
		tex3D.wrapMode = TextureWrapMode.Clamp;
		tex3D.anisoLevel = 0;

		Color[] pixels = verticalLUTMode
						? Tex2DPixelsToTex3DVerticalMode(textureSize, tex2DPixels)
						: Tex2DPixelsToTex3DHorizontalMode(textureSize, tex2DPixels);

		tex3D.SetPixels(pixels);
		tex3D.Apply();
		if(!wasTextureReadable)
		{
			importer.isReadable = false;
			AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
		}

		return tex3D;
	}

	private static Color[] Tex2DPixelsToTex3DVerticalMode(int textureSize, Color[] tex2DPixels)
	{
		Color[] pixels = new Color[textureSize * textureSize * textureSize];
		for(int x = 0; x < textureSize; x++)
		{
			for(int y = 0; y < textureSize; y++)
			{
				for(int z = 0; z < textureSize; z++)
				{
					int pixelIndex = x + y * textureSize + z * textureSize * textureSize;
					pixels[pixelIndex] = tex2DPixels[pixelIndex];
				}
			}
		}

		return pixels;
	}

	private static Color[] Tex2DPixelsToTex3DHorizontalMode(int textureSize, Color[] tex2DPixels)
	{
		Color[] pixels = new Color[textureSize * textureSize * textureSize];
		for(int x = 0; x < textureSize; x++)
		{
			for(int y = 0; y < textureSize; y++)
			{
				for(int z = 0; z < textureSize; z++)
				{
					int pixelIndex = x + y * textureSize + z * textureSize * textureSize;
					int pixel2DIndex = x + y * textureSize * textureSize + z * textureSize;
					pixels[pixelIndex] = tex2DPixels[pixel2DIndex];
				}
			}
		}

		return pixels;
	}

	private static Texture3D CreateTexture3D(int size)
	{
		Color[] colorArray = new Color[size * size * size];
		Texture3D texture = new Texture3D(size, size, size, TextureFormat.RGBA32, true);
		float r = 1.0f / (size - 1.0f);
		for(int x = 0; x < size; x++)
		{
			for(int y = 0; y < size; y++)
			{
				for(int z = 0; z < size; z++)
				{
					Color c = new Color(x * r, y * r, z * r, 1.0f);
					colorArray[x + (y * size) + (z * size * size)] = c;
				}
			}
		}
		texture.SetPixels(colorArray);
		texture.Apply();
		return texture;
	}
}
