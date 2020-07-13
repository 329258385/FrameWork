namespace LsyTextureCompressor
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public class LsyTextureSizeCal{
		public static float GetBitsPerPixel(TextureImporterFormat format)
		{
			switch (format)
			{
			case TextureImporterFormat.Alpha8: // Alpha-only texture format.
				return 8;
			case TextureImporterFormat.RGB24: // A color texture format.
				return 24;
			case TextureImporterFormat.RGBA32: //Color with an alpha channel texture format.
				return 32;
			case TextureImporterFormat.ARGB32: //Color with an alpha channel texture format.
				return 32;
			case TextureImporterFormat.DXT1: // Compressed color texture format.
				return 4;
			case TextureImporterFormat.DXT5: // Compressed color with alpha channel texture format.
				return 8;
			case TextureImporterFormat.PVRTC_RGB2: // PowerVR (iOS) 2 bits/pixel compressed color texture format.
				return 2;
			case TextureImporterFormat.PVRTC_RGBA2: // PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
				return 2;
			case TextureImporterFormat.PVRTC_RGB4: // PowerVR (iOS) 4 bits/pixel compressed color texture format.
				return 4;
			case TextureImporterFormat.PVRTC_RGBA4: // PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
				return 4;
			case TextureImporterFormat.ETC_RGB4: // ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
				return 4;
			case TextureImporterFormat.ETC2_RGB4:// ATC (ATITC) 4 bits/pixel compressed RGB texture format.
				return 4;
			case TextureImporterFormat.ETC2_RGBA8:// ATC (ATITC) 8 bits/pixel compressed RGB texture format.
				return 8;


			case TextureImporterFormat.ASTC_RGBA_4x4:
				return 8f;
			case TextureImporterFormat.ASTC_RGBA_5x5:
				return 5.12f;
			case TextureImporterFormat.ASTC_RGBA_6x6:
				return 3.56f;
			case TextureImporterFormat.ASTC_RGBA_8x8:
				return 2f;
			case TextureImporterFormat.ASTC_RGBA_10x10:
				return 1.28f;
			case TextureImporterFormat.ASTC_RGBA_12x12:
				return 0.89f;
			case TextureImporterFormat.ASTC_RGB_4x4:
				return 8f;
			case TextureImporterFormat.ASTC_RGB_5x5:
				return 5.12f;
			case TextureImporterFormat.ASTC_RGB_6x6:
				return 3.56f;
			case TextureImporterFormat.ASTC_RGB_8x8:
				return 2f;
			case TextureImporterFormat.ASTC_RGB_10x10:
				return 1.28f;
			case TextureImporterFormat.ASTC_RGB_12x12:
				return 0.89f;
			case TextureImporterFormat.EAC_R:
				return 4f;
			case TextureImporterFormat.EAC_R_SIGNED:
				return 4f;
			case TextureImporterFormat.EAC_RG:
				return 8f;
			case TextureImporterFormat.EAC_RG_SIGNED:
				return 8f;
			case TextureImporterFormat.ARGB16:
				return 16f;
			case TextureImporterFormat.RGB16:
				return 16f;
			case TextureImporterFormat.RGBA16:
				return 16f;
				//Approximate bpp for Crunched
			case TextureImporterFormat.ETC2_RGBA8Crunched:
				return 0.69f;
			case TextureImporterFormat.ETC_RGB4Crunched:
				return 0.6f;

				#pragma warning disable 0618
			case TextureImporterFormat.AutomaticCompressed:
				return 4;
			case TextureImporterFormat.AutomaticTruecolor:
				return 32;
			default:
				return 32;
				#pragma warning restore 0618
			}
		}

		public static int CalculateTextureSizeBytes(Texture tTexture,float tWidth,float tHeight, TextureImporterFormat format,bool isReadWrite)
		{
			int memSize = 0;
			if (tTexture is Texture2D)
			{
				var tTex2D = tTexture as Texture2D;
				var bitsPerPixel = GetBitsPerPixel(format);
				var mipMapCount = tTex2D.mipmapCount;
				var mipLevel = 1;
				while (mipLevel <= mipMapCount)
				{
					memSize += (int)(tWidth * tHeight * bitsPerPixel / 8f);
					tWidth = tWidth / 2;
					tHeight = tHeight / 2;
					mipLevel++;
				}
			}
			else if (tTexture is Cubemap)
			{
				var bitsPerPixel = GetBitsPerPixel(format);
				memSize = (int)(tWidth * tHeight * 6 * bitsPerPixel / 8);
			}
			if (isReadWrite)
				memSize *= 2;
			return memSize;
		}
	}
}