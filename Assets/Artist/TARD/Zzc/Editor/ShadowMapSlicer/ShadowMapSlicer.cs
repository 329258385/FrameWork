using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class ShadowMapSlicer : EditorWindow
{
    private static Color mixColor;
    private static float r, g, b, a;
    private static string path;

    [MenuItem("Assets/SaoUtils/Shadowmap切块")]
    private static void Slice()
    {
        //allFilePath = new List<string>();
        UnityEngine.Object[] selections = Selection.objects;

        for (int k = 0; k < selections.Length; k++)
        {
            path = AssetDatabase.GetAssetPath(selections[k]);

            Texture t = selections[k] as Texture;

            Texture2D tex = selections[k] as Texture2D;

            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(t));
            ti.isReadable = true;
            ti.SaveAndReimport();

            int halfWidth = (int)(tex.width * 0.5f);
            int halfHeight = (int)(tex.height * 0.5f);

            Texture2D mixTex = new Texture2D(halfWidth, halfHeight);
            for (int i = 0; i < halfHeight; i++)
            {
                for (int j = 0; j < halfWidth; j++)
                {
                    b = tex.GetPixel(i, j).r;
                    a = tex.GetPixel(i, j + halfWidth).r;
                    r = tex.GetPixel(i + halfHeight, j).r;
                    g = tex.GetPixel(i + halfHeight, j + halfWidth).r;

                    mixColor = new Color(a, g, b, r);
                    mixTex.SetPixel(i, j, mixColor);

                    //tex.SetPixel(i, j, new Color(1, 1, 1, 1));
                    
                }
            }

            SaveMixTex(mixTex);
            //tex.Resize(1000, 1000);
            //tex.Apply();
            

            ti.isReadable = false;
            
            ti.SaveAndReimport();

            

            AssetDatabase.Refresh();

            TextureImporter ti2 = (TextureImporter)TextureImporter.GetAtPath(path.Replace("shadowmask", "shadowmask"));

            ti2.textureType = TextureImporterType.Default;

            ti2.mipmapEnabled = false;
            ti2.anisoLevel = 3;

            TextureImporterPlatformSettings androidTIPS = new TextureImporterPlatformSettings();
            androidTIPS.name = "Android";
            androidTIPS.maxTextureSize = 2048;
            androidTIPS.format = TextureImporterFormat.ASTC_RGBA_6x6;
            androidTIPS.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            androidTIPS.compressionQuality = 50;
            androidTIPS.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;
            androidTIPS.overridden = true;

            TextureImporterPlatformSettings iOSTIPS = new TextureImporterPlatformSettings();
            iOSTIPS.name = "iPhone";
            iOSTIPS.maxTextureSize = 2048;
            iOSTIPS.format = TextureImporterFormat.ASTC_RGBA_6x6;
            iOSTIPS.compressionQuality = 50;
            iOSTIPS.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            iOSTIPS.overridden = true;

            ti2.SetPlatformTextureSettings(androidTIPS);
            ti2.SetPlatformTextureSettings(iOSTIPS);

            ti2.maxTextureSize = 2048;
            ti2.compressionQuality = 50;

            ti2.isReadable = false;
            ti2.mipmapEnabled = false;
            ti2.wrapMode = TextureWrapMode.Clamp;

            ti2.SaveAndReimport();
        }
    }

    private static void SaveMixTex(Texture2D mixTex)
    {
        var bytes = mixTex.EncodeToPNG();
        var file = File.Open(path.Replace("shadowmask","shadowmask"), FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }
}
