using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEditor;
using System.Collections.Generic;
/*
 * Reference: TinyEXR, Syoyo Fujita(syoyo@lighttransport.com)
 * https://github.com/syoyo/tinyexr
 * License:
 * 3-clause BSD
 * tinyexr uses miniz, which is developed by Rich Geldreich richgel99@gmail.com, and licensed under public domain.
 * tinyexr tools uses stb, which is licensed under public domain: https://github.com/nothings/stb tinyexr uses some code from OpenEXR, which is licensed under 3-clause BSD license.
 */

public class FantasyEXRTool : Editor
{
    public class TinyEXRTool : EditorWindow
    {
        public static bool useLXImage = false;
        public static string SourcePath = "Src Path";
        public static string TargetPath = "Target Path";
        public static int Width = 0;
        public static int Height = 0;
        public static int SourceChannels = 0;
        public static int TargetChannels = 0;
        public static float[] buffer;
        public static Color[] colorBuffer;
        public static string err = "";
        public static List<bool> RGBAChannels = new List<bool> { true, true, true, true }; // R, G, B, A
        public static Texture exrTex;


        [DllImport("TinyEXR", EntryPoint = "LoadEXR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadEXR(ref IntPtr data, ref int w, ref int h, [MarshalAs(UnmanagedType.LPStr)]string path, ref int channels, [MarshalAs(UnmanagedType.LPStr)]string err);

        [DllImport("TinyEXR", EntryPoint = "SaveEXR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SaveEXR(IntPtr data, int w, int h, int channels, [MarshalAs(UnmanagedType.LPStr)]string path);

        public static Texture2D LoadEXRToTexture(string srcPath)
        {
            if (srcPath == string.Empty)
                return null;
            int w = 32; int h = 32; int channels = 4;
            float[] exrFloats = LoadEXRImage(srcPath, ref w, ref h, ref channels);
            Debug.Log("Loaded Image Format " + w + " X " + h + " channels: " + channels);
            Color[] colors = ConvertFloatAry2ColorAry(exrFloats, channels, RGBAChannels);

            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBAHalf, false, true);
            tex.SetPixels(colors);
            tex.Apply();

            return tex;
        }

        public static float[] LoadEXRImage(string srcPath, ref int width, ref int height, ref int channels)
        {
            if (srcPath != "")
            {
                HandlePathSymbol(ref srcPath);

                IntPtr loadBuffer = IntPtr.Zero;

                int ret = LoadEXR(ref loadBuffer, ref width, ref height, srcPath, ref channels, err);
                if (ret == 0)
                {
                    ////L12Debug.EditorLog("Load成功！");
                }
                else
                {
                    ////L12Debug.EditorLog("Load失败: " + GetErrorCode(ret));
                    return null;
                }

                channels = 4; //发现TinyEXR的接口，虽然输出一个channels的数量，然而float还是rgba的排列。。。

                float[] data = new float[width * height * channels];

                Marshal.Copy(loadBuffer, data, 0, data.Length);

                return data;
            }
            else
            {
                ////L12Debug.EditorLog("文件路径不能为空");
                return null;
            }
        }

        public static float[] ConvertColorAry2FloatAry(Color[] _colorArray, int srcChannels, List<bool> rgbaChannel)
        {
            if (_colorArray != null && _colorArray.Length != 0)
            {
                float[] rgbaArray = new float[_colorArray.Length * srcChannels];

                for (int i = 0; i < _colorArray.Length; i++)
                {
                    if (rgbaChannel[0])
                        rgbaArray[i * srcChannels] = _colorArray[i].r;
                    if (rgbaChannel[1] && srcChannels > 1)
                        rgbaArray[i * srcChannels + 1] = _colorArray[i].g;
                    if (rgbaChannel[2] && srcChannels > 2)
                        rgbaArray[i * srcChannels + 2] = _colorArray[i].b;
                    if (rgbaChannel[3] && srcChannels > 3)
                        rgbaArray[i * srcChannels + 3] = _colorArray[i].a;
                }

                if (rgbaArray != null && rgbaArray.Length != 0)
                {
                    //L12Debug.EditorLog("转换成功");
                }
                return rgbaArray;
            }
            else
            {
                return null;
            }
        }

        public static Color[] ConvertFloatAry2ColorAry(float[] data, int srcChannels, List<bool> rgbaChannel)
        {
            if (data != null && data.Length != 0)
            {
                int colorAryLength = 0;

                if (data.Length % srcChannels != 0)
                {
                    colorAryLength = (data.Length + data.Length % srcChannels) / srcChannels;
                }
                else
                {
                    colorAryLength = data.Length / srcChannels;
                }

                Color[] colorAry = new Color[colorAryLength];

                for (int i = 0; i < colorAryLength; i++)
                {
                    if (rgbaChannel[0])
                        colorAry[i].r = data[i * srcChannels];
                    if (rgbaChannel[1] && srcChannels > 1)
                        colorAry[i].g = data[i * srcChannels + 1];
                    if (rgbaChannel[2] && srcChannels > 2)
                        colorAry[i].b = data[i * srcChannels + 2];
                    if (rgbaChannel[3] && srcChannels > 3)
                        colorAry[i].a = data[i * srcChannels + 3];
                }

                if (colorAry != null && colorAry.Length != 0)
                {
                    ////L12Debug.EditorLog("转换成功");
                }
                return colorAry;
            }
            else
            {
                return null;
            }
        }

        public static void SaveColors2Image(Color[] colors, string targetPath, int w, int h, int channels, List<bool> rgbaChannel)
        {
            if (colors != null && colors.Length != 0 && targetPath != "")
            {
                HandlePathSymbol(ref targetPath);
                FY.Utility.MiniEXR.MiniEXRWrite(targetPath, (uint)w, (uint)h, (uint)channels, colors);  // 这个库是存成16bit的half-float，试了下信息损失厉害
            }
            else
            {
                //L12Debug.EditorLog("必须先load，并且保存路径不能为空");
            }
        }

        public static void SaveEXRImage(float[] data, string targetPath, int w, int h, int srcChannel)
        {
            if (data != null && data.Length != 0 && targetPath != "")
            {
                HandlePathSymbol(ref targetPath);
                IntPtr saveBuffer = Marshal.AllocHGlobal(data.Length * sizeof(float));

                Marshal.Copy(data, 0, saveBuffer, data.Length);

                int ret = SaveEXR(saveBuffer, w, h, srcChannel, targetPath);

                if (ret == 0)
                {
                    //L12Debug.EditorLog("Save成功！");
                }
                else
                {
                    //L12Debug.EditorLog("Save失败: " + GetErrorCode(ret));
                    return;
                }
            }
            else
            {
                //L12Debug.EditorLog("必须先load，并且保存路径不能为空: ");
            }
        }

        private static void ShowExrImage(Color[] colors, int w, int h)
        {
            if (colors != null && colors.Length != 0)
            {
                Texture2D image = new Texture2D(w, h, TextureFormat.RGBAFloat, false);  // 目前colors里是RGBAFloat
                image.SetPixels(colors);
                GameObject plane = new GameObject();
                plane.transform.position = new Vector3(0, 1, 0);
                Material mat = new Material(Shader.Find("Standard"));
                mat.SetTexture("_MainTex", image);
                MeshRenderer renderer = plane.AddComponent<MeshRenderer>();
                MeshFilter filter = plane.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                mesh = new Mesh();
                mesh.vertices = new Vector3[]{new Vector3(-0.5f,0,-0.5f),
                                      new Vector3(0.5f,0,-0.5f),
                                      new Vector3(0.5f,0,0.5f),
                                      new Vector3(-0.5f,0,0.5f)};

                mesh.uv = new Vector2[] {new Vector2(0,0),
                                 new Vector2(0,1),
                                 new Vector2(1,1),
                                 new Vector2(1,0)};

                mesh.triangles = new int[] {0,1,2,
                                    0,2,3};
                mesh.RecalculateNormals();
                filter.mesh = mesh;
                renderer.material = mat;
                plane.transform.localRotation = new Quaternion(90, 0, 0, 0);
                plane.name = "EXRExample";
                Selection.activeGameObject = plane;
            }
            else
            {
                //L12Debug.EditorLog("必须先转换成Color");
            }
        }


        private static void HandlePathSymbol(ref string path)
        {
            path = path.Replace("\\", "/");
        }

        private static string GetErrorCode(int ret)
        {
            switch (ret)
            {
                case 0:
                    return "TINYEXR_SUCCESS";
                case -1:
                    return "TINYEXR_ERROR_INVALID_MAGIC_NUMBER";
                case -2:
                    return "TINYEXR_ERROR_INVALID_EXR_VERSION";
                case -3:
                    return "TINYEXR_ERROR_INVALID_ARGUMENT";
                case -4:
                    return "TINYEXR_ERROR_INVALID_DATA";
                case -5:
                    return "TINYEXR_ERROR_INVALID_FILE";
                case -6:
                    return "TINYEXR_ERROR_CANT_OPEN_FILE";
                case -7:
                    return "TINYEXR_ERROR_UNSUPPORTED_FORMAT";
                case -8:
                    return "TINYEXR_ERROR_INVALID_HEADER";
                default:
                    return "";
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            useLXImage = EditorGUILayout.Toggle(new GUIContent("Use LX Image"), useLXImage);

            if (useLXImage)
            {
                exrTex = (Texture)EditorGUILayout.ObjectField("EXR: ", exrTex, typeof(Texture), true);
                if (exrTex != null)
                    SourcePath = AssetDatabase.GetAssetPath(exrTex.GetInstanceID());
                int idx = Application.dataPath.IndexOf("Assets");
                string prefix = Application.dataPath.Substring(0, idx);
                SourcePath = prefix + SourcePath;
            }

            SourcePath = EditorGUILayout.TextField(new GUIContent("Source Path"), SourcePath);
            TargetPath = EditorGUILayout.TextField(new GUIContent("Target Save Path"), TargetPath);
            if (TargetPath == SourcePath)
            {
                EditorUtility.DisplayDialog("警告", "保存路径不能和源文件相同！", "确定");
                TargetPath = "保存路径不能和源文件相同";
            }
            EditorGUILayout.LabelField("Width: " + Width.ToString());
            EditorGUILayout.LabelField("Height: " + Height.ToString());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output RGBA Channels");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("R");
            RGBAChannels[0] = EditorGUILayout.Toggle(RGBAChannels[0]);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("G");
            RGBAChannels[1] = EditorGUILayout.Toggle(RGBAChannels[1]);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("B");
            RGBAChannels[2] = EditorGUILayout.Toggle(RGBAChannels[2]);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("A");
            RGBAChannels[3] = EditorGUILayout.Toggle(RGBAChannels[3]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            int count = 0;
            for (int i = 0; i < RGBAChannels.Count; i++)
            {
                if (RGBAChannels[i] == true)
                {
                    count++;
                }
            }
            TargetChannels = count;
            EditorGUILayout.LabelField("Source Channels:  " + SourceChannels);
            EditorGUILayout.LabelField("Output Channels:  " + TargetChannels);
            EditorGUILayout.Space();
            if (buffer != null)
                EditorGUILayout.LabelField("Buffer Length: " + buffer.Length);
            if (colorBuffer != null)
                EditorGUILayout.LabelField("Color Buffer Length: " + colorBuffer.Length);

            EditorGUILayout.Space();
            if (GUILayout.Button("Load .EXR (get a float[])"))
            {
                buffer = LoadEXRImage(SourcePath, ref Width, ref Height, ref SourceChannels);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Save float[] to .EXR (lossless)"))
            {
                SaveEXRImage(buffer, TargetPath, Width, Height, SourceChannels);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Save Color[] to .EXR (lossless)"))
            {
                colorBuffer = ConvertFloatAry2ColorAry(buffer, SourceChannels, RGBAChannels);
                SaveEXRImage(ConvertColorAry2FloatAry(colorBuffer, SourceChannels, RGBAChannels), TargetPath, Width, Height, SourceChannels);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Show EXR Image"))
            {
                ShowExrImage(ConvertFloatAry2ColorAry(buffer, SourceChannels, RGBAChannels), Width, Height);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Save Color[] to .EXR (16bitRGB, MiniEXR Interface)"))
            {
                SaveColors2Image(ConvertFloatAry2ColorAry(buffer, SourceChannels, RGBAChannels), TargetPath, Width, Height, 3, new List<bool> { true, true, true });
            }

            EditorGUILayout.Space();
        }
    }
}
