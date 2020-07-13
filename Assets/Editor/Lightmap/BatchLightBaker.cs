using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering;
using System;

namespace BigWorldTool
{
    public class BatchLightBaker : EditorWindowSection
    {
        public BatchLightBaker(EditorWindow host) : base(host)
        {
        }
        public const float LIGHTMAP_HDR_RANGE = 6f;
        public const string PATH_ROOT = "D:/FantasyLightmap";

        static string s_pathRoot = "Assets/Resources/Scene";
        static string s_pathDir;
        static string s_pathCol;
        static string s_pathMask;
        static string s_pathAO;
        static string s_pathPoint;
        static string s_pathPointDir;
        static string s_pathMain;
        static string s_pathMainDir;
        static string s_pathDirOriginal;
        static string s_pathColOriginal;
        static string s_pathMaskOriginal;
        static string s_pathAOOriginal;
        static string s_pathPointOriginal;
        static string s_pathPointDirOriginal;
        static string s_pathMainOriginal;
        static string s_pathMainDirOriginal;
        static string s_pathPostFix;
        static string s_pathRootOriginal;

        static RenderSettingsData renderSettingsData = new RenderSettingsData();
        static GameObject mainLightRoot;
        static GameObject pointLightRoot;
        static GameObject aoLightRoot;
        static GameObject mainLightObj;

        static Light mainLight;

        public enum BakeProgress { Stopped = 0, BakingAO = 1, BakingPoint = 2, BakingDirectinal = 3, BakingShadowMask = 4, Fixing = 5, BakeNext = 6, Finished = 7, Error = 8 };
        public string[] ProgressTips = new string[] { "", "正在烘焙AO...", "正在烘焙点光...", "正在烘焙方向光...", "正在烘焙ShadowMask...", "正在修正Lightmap...", "烘焙下个场景", "烘焙完成", "出错了！！！" };
        public BakeProgress progress = BakeProgress.Stopped;

        static float aoIndirect = 2;
        static float aoDirect = 4;
        static float indirectIntensity = 1;
        static float albedoBoost = 1;
        static bool fixDirection = true;
        static bool enableDirectionAO = false;
        static bool enablePointAO = false;
        static bool autoFixSaturate = true;
        static float aoIntensity = 1;
        static bool nonDirection = false;
        static bool shadowMaskToDirA = false;
        static bool batchBake = false;

        private List<string> selectedScenes;
        

        public override void Update()
        {
            base.Update();
        }

        public override void OnInspectorUpdate()
        {
            base.OnInspectorUpdate();
            RepaintHost();
        }

        public override void OnGUI()
        {
            try
            {
                ProcessGUI();
            }
            catch (Exception e)
            {
                Debug.LogError("Exception Message " + e.Message);
                Debug.LogError("Exception Stack" + e.StackTrace);
                progress = BakeProgress.Error;
            }
        }

        void DrawParams()
        {

            EditorGUILayout.LabelField("Lighting Params");
            aoIndirect = EditorGUILayout.FloatField("AO Indirect Contribution", aoIndirect, GUI.skin.textField);
            aoDirect = EditorGUILayout.FloatField("AO Direct Contribution", aoDirect, GUI.skin.textField);
            indirectIntensity = EditorGUILayout.FloatField("Indirect Intensity", indirectIntensity, GUI.skin.textField);
            albedoBoost = EditorGUILayout.FloatField("Albedo Boost", albedoBoost, GUI.skin.textField);
            fixDirection = EditorGUILayout.Toggle("是否修正方向图", fixDirection, GUI.skin.toggle);
            enableDirectionAO = EditorGUILayout.Toggle("方向光是否烘焙AO", enableDirectionAO, GUI.skin.toggle);
            enablePointAO = EditorGUILayout.Toggle("点光是否烘焙AO", enablePointAO, GUI.skin.toggle);
            autoFixSaturate = EditorGUILayout.Toggle("自动修正饱和度", autoFixSaturate, GUI.skin.toggle);
            shadowMaskToDirA = EditorGUILayout.Toggle("ShadowMask存方向图A通道", shadowMaskToDirA, GUI.skin.toggle);
            aoIntensity = EditorGUILayout.FloatField("AO Intensity", aoIntensity, GUI.skin.textField);
        }

        void ProcessGUI()
        {
            nonDirection = LightmapSettings.lightmapsMode == LightmapsMode.NonDirectional;
            if (nonDirection)
                fixDirection = false;
            GUILayout.Label(ProgressTips[(int)progress]);
            if (progress != BakeProgress.Stopped && progress != BakeProgress.Finished && progress != BakeProgress.Error)
            {
                if (GUILayout.Button("取消"))
                {
                    Lightmapping.Cancel();
                    progress = BakeProgress.Stopped;
                }
            }
            switch (progress)
            {
                case BakeProgress.Stopped:
                case BakeProgress.Finished:
                case BakeProgress.Error:
                    {
                        if (progress == BakeProgress.Finished)
                        {
                            GUILayout.Label("已完成！", GUI.skin.label);
                        }

                        DrawParams();

                        if (GUILayout.Button("合并AO到颜色贴图(不要乱点)", GUI.skin.button))
                        {
                            ProcessCombineAOToColorAndShadowMaskToDir();
                        }

                        if (GUILayout.Button("烘焙(普通)", GUI.skin.button))
                        {
                            batchBake = false;
                            progress = BakeProgress.BakingAO;
                            AddLight();
                            SaveSettings();
                            InitAOSettings();
                            Lightmapping.Clear();
                            Lightmapping.BakeAsync();
                        }
                        if (GUILayout.Button("批量烘焙(普通)", GUI.skin.button))
                        {
                            BakeBatch();
                        }
                        //if (GUILayout.Button("设置lightmapIndex", GUI.skin.button))
                        //{
                        //    SetLightMapIndex();
                        //}
                    }
                    break;
                case BakeProgress.BakeNext:
                    {
                        if (!batchBake)
                        {
                            progress = BakeProgress.Finished;
                        }
                        else
                        {
                            BakeNextScene();
                        }
                    }
                    break;
                case BakeProgress.BakingAO:
                    {
                        if (Lightmapping.isRunning)
                            return;
                        if (!LightmapExist())
                            return;
                        ProcessClearOriginalLightmapTex();
                        ProcessBackupAO();

                        RecoverSettings();
                        if (fixDirection)
                        {
                            progress = BakeProgress.BakingPoint;
                            InitPointSettings();
                        }
                        else
                        {
                            progress = BakeProgress.BakingDirectinal;
                            InitMainSettings();
                        }


                        Lightmapping.Clear();
                        Lightmapping.BakeAsync();
                    }
                    break;
                case BakeProgress.BakingPoint:
                    {
                        if (Lightmapping.isRunning)
                            return;
                        if (!LightmapExist())
                            return;
                        ProcessBackupPoint();

                        progress = BakeProgress.BakingDirectinal;
                        InitMainSettings();
                        Lightmapping.Clear();
                        Lightmapping.BakeAsync();
                    }
                    break;
                case BakeProgress.BakingDirectinal:
                    {
                        if (Lightmapping.isRunning)
                            return;
                        if (!LightmapExist())
                            return;
                        RecoverSettings();
                        ProcessBackupDirection();

                        progress = BakeProgress.Fixing;
                    }
                    break;
                case BakeProgress.Fixing:
                    {
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                        ProcessFixDirection();
                        RecoverSettings();
                        progress = BakeProgress.BakeNext;
                    }
                    break;
            }
        }

        //void SetLightMapIndex()
        //{
        //    string sceneName = "";

        //    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        //    string[] paths = Selection.assetGUIDs;
        //    if (selectedScenes != null)
        //    {
        //        selectedScenes.Clear();
        //    }
        //    else
        //    {
        //        selectedScenes = new List<string>();
        //    }
        //    foreach (string guid in paths)
        //    {
        //        string path = AssetDatabase.GUIDToAssetPath(guid);
        //        if (path.StartsWith("Assets/Art/Worlds"))
        //        {
        //            selectedScenes.Add(path);
        //        }
        //    }
        //    if (selectedScenes.Count > 0)
        //    {
        //        for (int j = 0; j < selectedScenes.Count; j++)
        //        {
        //            EditorSceneManager.OpenScene(selectedScenes[j], OpenSceneMode.Single);
        //            string curPath = selectedScenes[j];
        //            sceneName = curPath.Split('/')[3];
        //            string chunkName = EditorSceneManager.GetActiveScene().name;
        //            //根节点添加chunklightmapsetting 组件
        //            GameObject prefab = GameObject.Find(chunkName);
        //            ChunkLightMapSetting chunkLm = prefab.GetComponent<ChunkLightMapSetting>();
        //            if (chunkLm == null)
        //            {
        //                chunkLm = prefab.AddComponent<ChunkLightMapSetting>();
        //            }
        //            //所有render子节点添加RendererLightMapSetting组件
        //            Renderer[] savers = Transform.FindObjectsOfType<Renderer>();
        //            RendererLightMapSetting rlms = null;
        //            foreach (Renderer s in savers)
        //            {
        //                if (s.lightmapIndex != -1)
        //                {
        //                    rlms = s.gameObject.GetComponent<RendererLightMapSetting>();
        //                    if (rlms == null)
        //                    {
        //                        rlms = s.gameObject.AddComponent<RendererLightMapSetting>();
        //                    }
        //                }
        //            }
        //            chunkLm.SaveSettings();
        //            PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, "Assets/Art/Worlds/" + sceneName + "/Chunks/Prefab/" + chunkName + ".prefab",InteractionMode.AutomatedAction);
        //        }
        //    }
        //}

        void BakeBatch()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            string[] paths = Selection.assetGUIDs;
            if (selectedScenes != null)
            {
                selectedScenes.Clear();
            }
            else
            {
                selectedScenes = new List<string>();
            }
            foreach (string guid in paths)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.StartsWith("Assets/Art/Worlds"))
                {
                    selectedScenes.Add(path);
                }
            }
            if (selectedScenes.Count > 0)
            {
                batchBake = true;
                progress = BakeProgress.BakeNext;
            }
        }

        void BakeNextScene()
        {
            if(selectedScenes.Count > 0)
            {
                string path = selectedScenes[0];
                selectedScenes.RemoveAt(0);
                //打开场景
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                //开始烘焙
                progress = BakeProgress.BakingAO;
                AddLight();
                SaveSettings();
                InitAOSettings();
                Lightmapping.Clear();
                Lightmapping.BakeAsync();
            }
            else
            {
                progress = BakeProgress.Finished;
            }
           

        }

        void AddLight()
        {
            GameObject light = GameObject.Find("root/light");
            if(light == null)
            {
                string sceneName = "Test";
                string curPath = EditorSceneManager.GetActiveScene().path;
                sceneName = curPath.Split('/')[3];
                GameObject temp = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Worlds/" + sceneName + "/Base/bakeTemp.prefab");
                light = GameObject.Instantiate<GameObject>(temp);
                light.name = "root";
            }
        }

        //AO Point Directional ShadowMask
        void ProcessGUI_HL()
        {
            GUILayout.Label(ProgressTips[(int)progress]);
            if (progress != BakeProgress.Stopped && progress != BakeProgress.Finished && progress != BakeProgress.Error)
            {
                if (GUILayout.Button("取消"))
                {
                    Lightmapping.Cancel();
                    progress = BakeProgress.Stopped;
                }
            }
            switch (progress)
            {
                case BakeProgress.Stopped:
                case BakeProgress.Finished:
                case BakeProgress.Error:
                    {
                        if (progress == BakeProgress.Finished)
                        {
                            GUILayout.Label("已完成！", GUI.skin.label);
                        }
                        if (GUILayout.Button("烘焙", GUI.skin.button))
                        {
                            progress = BakeProgress.BakingAO;
                            SaveSettings();
                            InitAOSettings();
                            Lightmapping.Clear();
                            Lightmapping.BakeAsync();
                        }
                        if (GUILayout.Button("修正Lightmap", GUI.skin.button))
                        {
                            ProcessFixLightmap();
                        }
                    }
                    break;
                case BakeProgress.BakingAO:
                    {
                        if (Lightmapping.isRunning)
                            return;
                        if (!LightmapExist())
                            return;
                        ProcessBackupAO();

                        progress = BakeProgress.BakingPoint;
                        InitMainSettings();
                        Lightmapping.Clear();
                        Lightmapping.BakeAsync();
                    }
                    break;
                case BakeProgress.BakingPoint:
                    {
                        if (Lightmapping.isRunning)
                            return;
                        if (!LightmapExist())
                            return;
                        ProcessBackupPoint();

                        progress = BakeProgress.BakingDirectinal;
                        InitShadowMaskSettings();
                        Lightmapping.Clear();
                        Lightmapping.BakeAsync();
                    }
                    break;
                case BakeProgress.BakingDirectinal:
                    {
                        if (Lightmapping.isRunning)
                            return;
                        if (!LightmapExist())
                            return;
                        ProcessBackupDirection();

                        progress = BakeProgress.BakingShadowMask;
                        InitMainSettings();
                        Lightmapping.Clear();
                        Lightmapping.BakeAsync();
                    }
                    break;
                case BakeProgress.BakingShadowMask:
                    {
                        if (Lightmapping.isRunning)
                            return;
                        if (!LightmapExist())
                            return;
                        ProcessBackupShadowmask();
                        progress = BakeProgress.Fixing;
                    }
                    break;
                case BakeProgress.Fixing:
                    {
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                        ProcessFixLightmap();
                        RecoverSettings();
                        progress = BakeProgress.Finished;

                        //ProcessSpyLightmap.DoProcessSpyLightmap();
                    }
                    break;
            }
        }

        bool LightmapExist()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                if (data.lightmapColor == null || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(data.lightmapColor)))
                    return false;
            }
            string path = GetLightmapPath(0);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            return tex != null;
        }

        string GetLightmapPath(int index)
        {
            string path = EditorSceneManager.GetSceneAt(0).path;
            int startIndex = path.IndexOf('.');
            path = path.Substring(0, startIndex);
            path = string.Format("{0}/Lightmap-{1}_comp_light.exr", path, index);
            return path;
        }

        public static void InitLightObject()
        {
            mainLightRoot = GameObject.Find("root/light/mainlight");
            pointLightRoot = GameObject.Find("root/light/bakedlight");
            aoLightRoot = GameObject.Find("root/light/aolight");
            mainLightObj = GameObject.Find("root/light/mainlight/Directional Light");
            if (mainLightRoot)
            {
                mainLight = mainLightRoot.GetComponentInChildren<Light>();
                if (mainLightRoot.transform.childCount == 0)
                {
                    fixDirection = false;
                }
            }
            else
            {
                //EditorUtility.DisplayDialog("Opps", "路径'root/Light/mainLight'下没有主光Directional light", "好的");
            }
        }

        public static void InitAOSettings()
        {
            InitLightObject();
            if (mainLightRoot)
                mainLightRoot.SetActive(false);
            if (pointLightRoot)
                pointLightRoot.SetActive(false);
            if (aoLightRoot)
                aoLightRoot.SetActive(true);
            renderSettingsData.InitAOSettings(aoIndirect, aoDirect, indirectIntensity, albedoBoost);
        }

        public static void InitPointSettings()
        {
            InitLightObject();
            if (mainLightRoot)
                mainLightRoot.SetActive(false);
            if (pointLightRoot)
                pointLightRoot.SetActive(true);
            if (aoLightRoot)
                aoLightRoot.SetActive(false);
            renderSettingsData.InitPointColorSettings(fixDirection, enablePointAO);
        }

        public static void InitMainSettings()
        {
            InitLightObject();
            if (mainLightRoot)
                mainLightRoot.SetActive(true);
            if (fixDirection)
            {
                if (pointLightRoot)
                    pointLightRoot.SetActive(false);
            }
            else
            {
                if (pointLightRoot)
                    pointLightRoot.SetActive(true);
            }
            if (aoLightRoot)
                aoLightRoot.SetActive(false);
            //if (mainLight)
            //    mainLight.lightmapBakeType = LightmapBakeType.Baked;
            renderSettingsData.InitMainColorSettings(fixDirection, enableDirectionAO);
        }

        public static void InitShadowMaskSettings()
        {
            if (mainLightRoot)
                mainLightRoot.SetActive(true);
            if (pointLightRoot)
                pointLightRoot.SetActive(false);

            //if (mainLight)
            //    mainLight.lightmapBakeType = LightmapBakeType.Mixed;
        }

        static void SaveSettings()
        {
            renderSettingsData.SaveData();
        }

        static void RecoverSettings()
        {
            InitLightObject();
            if (mainLightRoot)
                mainLightRoot.SetActive(true);
            if (pointLightRoot)
                pointLightRoot.SetActive(false);
            if (aoLightRoot)
                aoLightRoot.SetActive(false);
            renderSettingsData.RecoverData();
        }

        //[MenuItem("Menu/Lightmap/BeckAO")]
        public static void GenerateAO()
        {
            InitAOSettings();
            Lightmapping.BakeAsync();
        }

        public static void GeneratePointColor()
        {
            InitPointSettings();
            MeshRenderer[] renders = GameObject.FindObjectsOfType<MeshRenderer>();
            foreach (MeshRenderer r in renders)
            {
                Material[] materials = r.sharedMaterials;
                foreach (Material mat in materials)
                {
                    if (mat)
                    {
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                        //mat.SetColor("_EmissionColor", new Color(0, 0, 0, 1));
                    }
                }
            }
            Lightmapping.BakeAsync();
        }

        //[MenuItem("Menu/Lightmap/BeckLightmap")]
        public static void GenerateLightmap()
        {
            InitMainSettings();
            MeshRenderer[] renders = GameObject.FindObjectsOfType<MeshRenderer>();
            foreach (MeshRenderer r in renders)
            {
                Material[] materials = r.sharedMaterials;
                foreach (Material mat in materials)
                {
                    if (mat)
                    {
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                        //mat.SetColor("_EmissionColor", new Color(0, 0, 0, 1));
                    }
                }
            }
            Lightmapping.BakeAsync();
        }

        //[MenuItem("Menu/Lightmap/BackupAO")]
        //public static void DoBackupAO()
        //{
        //    ProcessBackupAO();
        //}

        //[MenuItem("Menu/Lightmap/BackupPoint")]
        //public static void DoBackupPoint()
        //{
        //    ProcessBackupPoint();
        //}

        //[MenuItem("Menu/Lightmap/BackupDirection")]
        //public static void DoBackupDirection()
        //{
        //    ProcessBackupDirection();
        //}

        [MenuItem("Menu/Lightmap/只修正Lightmap颜色")]
        public static void DoFixLightmap()
        {
            if (EditorUtility.DisplayDialog("确认", "是否修正", "是", "否"))
                ProcessOnlyFixLightmap();
        }

        [MenuItem("Menu/Lightmap/增加饱和度")]
        public static void DoFixAddSaturate()
        {
            if (EditorUtility.DisplayDialog("确认", "是否增加饱和度", "是", "否"))
                ProcessFixAddSaturate();
        }

        [MenuItem("Menu/Lightmap/降低饱和度")]
        public static void DoFixSubSaturate()
        {
            if (EditorUtility.DisplayDialog("确认", "是否降低饱和度", "是", "否"))
                ProcessFixSubSaturate();
        }

        [MenuItem("Menu/Lightmap/FixDirection")]
        public static void DoFixDirection()
        {
            ProcessFixDirection();
        }

        //[MenuItem("Menu/Lightmap/CombineShadowMask")]
        //public static void DoCombineShadowMask()
        //{
        //    ProcessCombineShadowMaskToDir();
        //}

        //[MenuItem("Menu/Lightmap/UnDoLightmap")]
        //public static void DoUnDoLightmap()
        //{
        //    ProcessUnFixLightmap();
        //}

        public static bool CanFix()
        {
            TextureImporter texColImporter = TextureImporter.GetAtPath(s_pathCol) as TextureImporter;
            if (texColImporter && texColImporter.textureType == TextureImporterType.Default)
            {
                //EditorUtility.DisplayDialog("Opps", "Lightmap已经处理过了!", "好的");
                //return false;
            }
            return true;
        }

        public static bool CanRecover()
        {
            TextureImporter texColImporter = TextureImporter.GetAtPath(s_pathCol) as TextureImporter;
            if (texColImporter && texColImporter.textureType == TextureImporterType.Lightmap)
            {
                EditorUtility.DisplayDialog("Opps", "不需要回退!", "好的");
                return false;
            }
            return true;
        }

        public static void InitPath(LightmapData data)
        {
            Texture2D texDir = data.lightmapDir;
            Texture2D texCol = data.lightmapColor;
            Texture2D texMask = data.shadowMask;

            s_pathCol = AssetDatabase.GetAssetPath(texCol);

            if (texDir != null)
                s_pathDir = AssetDatabase.GetAssetPath(texDir);
            else
                s_pathDir = s_pathCol.Replace("_light", "_dir");

            if (texMask)
                s_pathMask = AssetDatabase.GetAssetPath(texMask);
            else
                s_pathMask = s_pathCol.Replace("_light", "_shadowmask");
            s_pathAO = s_pathCol.Replace(".exr", "_ao.exr");
            s_pathPoint = s_pathCol.Replace(".exr", "_point.exr");
            s_pathPointDir = s_pathDir.Replace("_dir", "_dir_point");
            s_pathMain = s_pathCol.Replace(".exr", "_main.exr");
            s_pathMainDir = s_pathDir.Replace("_dir", "_dir_main");
            string pathRoot = s_pathDir.Substring(0, s_pathDir.LastIndexOf('/'));
            s_pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf('/'));
            int index = s_pathDir.IndexOf(s_pathRoot) + s_pathRoot.Length;
            string pathDirPostFix = s_pathDir.Substring(index);
            string pathColPostFix = s_pathCol.Substring(index);
            string pathMaskPostFix = s_pathMask.Substring(index);
            string pathAOPostFix = s_pathAO.Substring(index);
            string pathPointPostFix = s_pathPoint.Substring(index);
            string pathPointDirPostFix = s_pathPointDir.Substring(index);
            string pathMainPostFix = s_pathMain.Substring(index);
            string pathMainDirPostFix = s_pathMainDir.Substring(index);
            s_pathMaskOriginal = PATH_ROOT + pathMaskPostFix;
            s_pathDirOriginal = PATH_ROOT + pathDirPostFix;
            s_pathColOriginal = PATH_ROOT + pathColPostFix;
            s_pathAOOriginal = PATH_ROOT + pathAOPostFix;
            s_pathPointOriginal = PATH_ROOT + pathPointPostFix;
            s_pathPointDirOriginal = PATH_ROOT + pathPointDirPostFix;
            s_pathMainOriginal = PATH_ROOT + pathMainPostFix;
            s_pathMainDirOriginal = PATH_ROOT + pathMainDirPostFix;

            s_pathRootOriginal = s_pathDirOriginal.Substring(0, s_pathDirOriginal.LastIndexOf('/'));
            InitLightmapFolder(s_pathRootOriginal);
        }

        public static void BakeupLightmap()
        {
            File.Copy(s_pathCol, s_pathColOriginal, true);
            if (File.Exists(s_pathDir))
                File.Copy(s_pathDir, s_pathDirOriginal, true);
            if (File.Exists(s_pathMask))
                File.Copy(s_pathMask, s_pathMaskOriginal, true);
        }

        public static void RecoverLightmap()
        {
            if (File.Exists(s_pathDirOriginal))
                File.Copy(s_pathDirOriginal, s_pathDir, true);
            if (File.Exists(s_pathColOriginal))
                File.Copy(s_pathColOriginal, s_pathCol, true);
            if (File.Exists(s_pathMaskOriginal))
                File.Copy(s_pathMaskOriginal, s_pathMask, true);

            TextureImporter texColOriginalImporter = TextureImporter.GetAtPath(s_pathCol) as TextureImporter;
            if (texColOriginalImporter)
                texColOriginalImporter.textureType = TextureImporterType.Lightmap;

            AssetDatabase.ImportAsset(s_pathCol, ImportAssetOptions.ForceUpdate);
        }

        public static void BackupAO()
        {
            if (File.Exists(s_pathCol))
            {
                File.Copy(s_pathCol, s_pathAOOriginal, true);
            }
        }

        public static void BackupPoint()
        {
            if (File.Exists(s_pathCol))
            {
                File.Copy(s_pathCol, s_pathPointOriginal, true);
            }
            if (File.Exists(s_pathDir))
            {
                File.Copy(s_pathDir, s_pathPointDirOriginal, true);
            }
        }

        public static void BackupMain()
        {
            if (File.Exists(s_pathCol))
            {
                File.Copy(s_pathCol, s_pathMainOriginal, true);
            }
            if (File.Exists(s_pathDir))
            {
                File.Copy(s_pathDir, s_pathMainDirOriginal, true);
            }
            if (File.Exists(s_pathMask))
            {
                File.Copy(s_pathMask, s_pathMaskOriginal, true);
            }
        }

        public static void BackupShadowmask()
        {
            if (File.Exists(s_pathMask))
            {
                File.Copy(s_pathMask, s_pathMaskOriginal, true);
            }
        }

        public static void PrepareLightmapTex()
        {
            if (File.Exists(s_pathAOOriginal))
            {
                File.Copy(s_pathAOOriginal, s_pathAO, true);
            }
            if (File.Exists(s_pathMaskOriginal))
            {
                File.Copy(s_pathMaskOriginal, s_pathMask, true);
            }

            if (File.Exists(s_pathPointOriginal))
            {
                File.Copy(s_pathPointOriginal, s_pathPoint, true);
            }
            if (File.Exists(s_pathPointDirOriginal))
            {
                File.Copy(s_pathPointDirOriginal, s_pathPointDir, true);
            }
            if (File.Exists(s_pathMainOriginal))
            {
                File.Copy(s_pathMainOriginal, s_pathMain, true);
                //File.Copy(s_pathMainOriginal, s_pathCol, true);
            }
            if (File.Exists(s_pathMainDirOriginal))
            {
                File.Copy(s_pathMainDirOriginal, s_pathMainDir, true);
                //File.Copy(s_pathMainDirOriginal, s_pathDir, true);
            }
            if (File.Exists(s_pathMaskOriginal))
            {
                File.Copy(s_pathMaskOriginal, s_pathMask, true);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ProcessLightmapTexFormat(bool isPreProcess)
        {
            TextureImporterPlatformSettings texSettingsAndroid = new TextureImporterPlatformSettings();
            texSettingsAndroid.name = "Android";
            texSettingsAndroid.overridden = true;
            texSettingsAndroid.format = isPreProcess ? TextureImporterFormat.RGBA32 : TextureImporterFormat.ETC2_RGBA8;
            //if (!isPreProcess)
            //    texSettingsAndroid.maxTextureSize = 1024;
            texSettingsAndroid.textureCompression = isPreProcess ? TextureImporterCompression.Uncompressed : TextureImporterCompression.Compressed;

            TextureImporterPlatformSettings texSettingsIOS = new TextureImporterPlatformSettings();
            texSettingsIOS.name = "iPhone";
            texSettingsIOS.overridden = true;
            texSettingsIOS.format = isPreProcess ? TextureImporterFormat.RGBA32 : TextureImporterFormat.PVRTC_RGBA4;
            //if (!isPreProcess)
            //    texSettingsIOS.maxTextureSize = 1024;
            texSettingsIOS.textureCompression = isPreProcess ? TextureImporterCompression.Uncompressed : TextureImporterCompression.Compressed;

            TextureImporterPlatformSettings texSettingsStandalone = new TextureImporterPlatformSettings();
            texSettingsStandalone.name = "Standalone";
            texSettingsStandalone.overridden = true;
            texSettingsStandalone.format = isPreProcess ? TextureImporterFormat.RGBA32 : TextureImporterFormat.DXT5;
            //if (!isPreProcess)
            //    texSettingsIOS.maxTextureSize = 1024;
            texSettingsStandalone.textureCompression = isPreProcess ? TextureImporterCompression.Uncompressed : TextureImporterCompression.Compressed;

            TextureImporter texDirImporter = AssetImporter.GetAtPath(s_pathDir) as TextureImporter;
            TextureImporter texMaskImporter = AssetImporter.GetAtPath(s_pathMask) as TextureImporter;
            TextureImporter texColImporter = AssetImporter.GetAtPath(s_pathCol) as TextureImporter;
            TextureImporter texAOImporter = AssetImporter.GetAtPath(s_pathAO) as TextureImporter;
            TextureImporter texPointImporter = AssetImporter.GetAtPath(s_pathPoint) as TextureImporter;
            TextureImporter texPointDirImporter = AssetImporter.GetAtPath(s_pathPointDir) as TextureImporter;
            TextureImporter texMainImporter = AssetImporter.GetAtPath(s_pathMain) as TextureImporter;
            TextureImporter texMainDirImporter = AssetImporter.GetAtPath(s_pathMainDir) as TextureImporter;
            if (texAOImporter)
            {
                texAOImporter.isReadable = isPreProcess;
                //if (!isPreProcess)
                //    texAOImporter.maxTextureSize = 1024;
                texAOImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texAOImporter.SetPlatformTextureSettings(texSettingsIOS);
                texAOImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texDirImporter)
            {
                texDirImporter.isReadable = isPreProcess;
                //if (!isPreProcess)
                //    texDirImporter.maxTextureSize = 1024;
                texDirImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texDirImporter.SetPlatformTextureSettings(texSettingsIOS);
                texDirImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texMaskImporter)
            {
                texMaskImporter.isReadable = isPreProcess;
                //if (!isPreProcess)
                //    texMaskImporter.maxTextureSize = 1024;
                texMaskImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texMaskImporter.SetPlatformTextureSettings(texSettingsIOS);
                texMaskImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texColImporter)
            {
                texColImporter.isReadable = isPreProcess;
                if (!isPreProcess)
                {
                    //texColImporter.maxTextureSize = 1024;
                    texColImporter.textureType = TextureImporterType.Default;
                    texColImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                }
                texColImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texColImporter.SetPlatformTextureSettings(texSettingsIOS);
                texColImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texPointImporter)
            {
                texPointImporter.isReadable = isPreProcess;
                if (!isPreProcess)
                {
                    //texPointImporter.maxTextureSize = 1024;
                    texPointImporter.textureType = TextureImporterType.Default;
                    texPointImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                }
                texPointImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texPointImporter.SetPlatformTextureSettings(texSettingsIOS);
                texPointImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texPointDirImporter)
            {
                texPointDirImporter.isReadable = isPreProcess;
                //if (!isPreProcess)
                //texPointDirImporter.maxTextureSize = 1024;
                texPointDirImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texPointDirImporter.SetPlatformTextureSettings(texSettingsIOS);
                texPointDirImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texMainImporter)
            {
                texMainImporter.isReadable = isPreProcess;
                if (!isPreProcess)
                {
                    //texMainImporter.maxTextureSize = 1024;
                    texMainImporter.textureType = TextureImporterType.Default;
                    texMainImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                }
                texMainImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texMainImporter.SetPlatformTextureSettings(texSettingsIOS);
                texMainImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texMainDirImporter)
            {
                texMainDirImporter.isReadable = isPreProcess;
                //if (!isPreProcess)
                //    texMainDirImporter.maxTextureSize = 1024;
                texMainDirImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texMainDirImporter.SetPlatformTextureSettings(texSettingsIOS);
                texMainDirImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }

            AssetDatabase.ImportAsset(s_pathDir, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathMask, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathCol, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathAO, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathPoint, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathPointDir, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathMain, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathMainDir, ImportAssetOptions.ForceUpdate);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ProcessShadowMaskTexFormat(bool isPreProcess)
        {
            TextureImporterPlatformSettings texSettingsAndroid = new TextureImporterPlatformSettings();
            texSettingsAndroid.name = "Android";
            texSettingsAndroid.overridden = true;
            texSettingsAndroid.format = isPreProcess ? TextureImporterFormat.RGBA32 : TextureImporterFormat.ETC2_RGBA8;
            //if (!isPreProcess)
            //    texSettingsAndroid.maxTextureSize = 1024;
            texSettingsAndroid.textureCompression = isPreProcess ? TextureImporterCompression.Uncompressed : TextureImporterCompression.Compressed;

            TextureImporterPlatformSettings texSettingsIOS = new TextureImporterPlatformSettings();
            texSettingsIOS.name = "iPhone";
            texSettingsIOS.overridden = true;
            texSettingsIOS.format = isPreProcess ? TextureImporterFormat.RGBA32 : TextureImporterFormat.PVRTC_RGBA4;
            //if (!isPreProcess)
            //    texSettingsIOS.maxTextureSize = 1024;
            texSettingsIOS.textureCompression = isPreProcess ? TextureImporterCompression.Uncompressed : TextureImporterCompression.Compressed;

            TextureImporterPlatformSettings texSettingsStandalone = new TextureImporterPlatformSettings();
            texSettingsStandalone.name = "Standalone";
            texSettingsStandalone.overridden = true;
            texSettingsStandalone.format = isPreProcess ? TextureImporterFormat.RGBA32 : TextureImporterFormat.DXT5;
            //if (!isPreProcess)
            //    texSettingsIOS.maxTextureSize = 1024;
            texSettingsStandalone.textureCompression = isPreProcess ? TextureImporterCompression.Uncompressed : TextureImporterCompression.Compressed;

            TextureImporter texDirImporter = AssetImporter.GetAtPath(s_pathDir) as TextureImporter;
            TextureImporter texMaskImporter = AssetImporter.GetAtPath(s_pathMask) as TextureImporter;

            if (texDirImporter)
            {
                texDirImporter.isReadable = isPreProcess;
                //if (!isPreProcess)
                //    texDirImporter.maxTextureSize = 1024;
                texDirImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texDirImporter.SetPlatformTextureSettings(texSettingsIOS);
                texDirImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }
            if (texMaskImporter)
            {
                texMaskImporter.isReadable = isPreProcess;
                //if (!isPreProcess)
                //    texMaskImporter.maxTextureSize = 1024;
                texMaskImporter.SetPlatformTextureSettings(texSettingsAndroid);
                texMaskImporter.SetPlatformTextureSettings(texSettingsIOS);
                texMaskImporter.SetPlatformTextureSettings(texSettingsStandalone);
            }

            AssetDatabase.ImportAsset(s_pathDir, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(s_pathMask, ImportAssetOptions.ForceUpdate);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ClearLightmapTex()
        {
            AssetDatabase.DeleteAsset(s_pathAO);
            AssetDatabase.DeleteAsset(s_pathPoint);
            AssetDatabase.DeleteAsset(s_pathPointDir);
            AssetDatabase.DeleteAsset(s_pathMain);
            AssetDatabase.DeleteAsset(s_pathMainDir);
            if (LightmapSettings.lightmapsMode == LightmapsMode.NonDirectional)
            {
                AssetDatabase.DeleteAsset(s_pathMainDir);
                AssetDatabase.DeleteAsset(s_pathPointDir);
                AssetDatabase.DeleteAsset(s_pathDir);
            }
            //AssetDatabase.DeleteAsset(s_pathMask);
        }

        public static void ClearOriginalLightmapTex()
        {
            Directory.Delete(s_pathRootOriginal, true);
        }

        public static void ClearShadowMaskTex()
        {
            AssetDatabase.DeleteAsset(s_pathMask);
        }

        //public static void ProcessAOTexFormat()
        //{
        //    TextureImporterPlatformSettings texSettingsAndroid = new TextureImporterPlatformSettings();
        //    texSettingsAndroid.name = "Android";
        //    texSettingsAndroid.overridden = true;
        //    texSettingsAndroid.format = TextureImporterFormat.RGBA32;
        //    texSettingsAndroid.textureCompression = TextureImporterCompression.Uncompressed;

        //    TextureImporterPlatformSettings texSettingsIOS = new TextureImporterPlatformSettings();
        //    texSettingsIOS.name = "iPhone";
        //    texSettingsIOS.overridden = true;
        //    texSettingsIOS.format = TextureImporterFormat.RGBA32;
        //    texSettingsIOS.textureCompression = TextureImporterCompression.Uncompressed;

        //    TextureImporter texAOImporter = AssetImporter.GetAtPath(s_pathAO) as TextureImporter;
        //    TextureImporter texPointImporter = AssetImporter.GetAtPath(s_pathPoint) as TextureImporter;
        //    TextureImporter texDirectionImporter = AssetImporter.GetAtPath(s_pathDirection) as TextureImporter;

        //    if (texAOImporter)
        //    {
        //        texAOImporter.isReadable = true;
        //        texAOImporter.textureType = TextureImporterType.Lightmap;
        //        texAOImporter.SetPlatformTextureSettings(texSettingsAndroid);
        //        texAOImporter.SetPlatformTextureSettings(texSettingsIOS);
        //    }

        //    if (texPointImporter)
        //    {
        //        texPointImporter.isReadable = true;
        //        texPointImporter.textureType = TextureImporterType.Lightmap;
        //        texPointImporter.SetPlatformTextureSettings(texSettingsAndroid);
        //        texPointImporter.SetPlatformTextureSettings(texSettingsIOS);
        //    }

        //    if (texDirectionImporter)
        //    {
        //        texDirectionImporter.isReadable = true;
        //        texDirectionImporter.textureType = TextureImporterType.Lightmap;
        //        texDirectionImporter.SetPlatformTextureSettings(texSettingsAndroid);
        //        texDirectionImporter.SetPlatformTextureSettings(texSettingsIOS);
        //    }

        //    AssetDatabase.ImportAsset(s_pathAO, ImportAssetOptions.ForceUpdate);
        //    AssetDatabase.ImportAsset(s_pathPoint, ImportAssetOptions.ForceUpdate);
        //    AssetDatabase.ImportAsset(s_pathDirection, ImportAssetOptions.ForceUpdate);

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        public static void FixLightmap()
        {
            Texture2D texDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathDir);
            Texture2D texMask = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMask);
            Texture2D texCol = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathCol);
            Texture2D texPointDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathPointDir);
            Texture2D texMainDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMainDir);
            Texture2D texAO = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathAO);
            Debug.LogErrorFormat("Fix {0} {1}", texDir.name, s_pathDir);
            Color[] sourceDir = texDir.GetPixels();
            Color[] sourceMask = texMask == null ? null : texMask.GetPixels();
            Color[] sourceCol = loadHDRLightmapPixels(s_pathCol);
            Color[] sourceAO = loadHDRLightmapPixels(s_pathAO);
            Color[] sourcePoint = loadHDRLightmapPixels(s_pathPoint);
            Color[] sourcePointDir = texPointDir.GetPixels();
            Color[] sourceMain = loadHDRLightmapPixels(s_pathMain);
            Color[] sourceMainDir = texMainDir.GetPixels();

            GameObject mainLight = GameObject.Find("root/light/mainlight");
            Light light = null;
            if (mainLight)
                light = mainLight.GetComponentInChildren<Light>();

            for (int i = 0; i < sourceDir.Length; i++)
            {

                //sourceAO[i] = encodeRGBMPixel(sourceAO[i]);
                //float ao = Mathf.Sqrt(Mathf.Pow(sourceAO[i].r, 2)+Mathf.Pow(sourceAO[i].g, 2)+Mathf.Pow(sourceAO[i].b, 2));
                //ao = Mathf.Min(1f, Mathf.Max(0.05f, ao));
                //if (sourceMask != null && light)
                //    sourceCol[i] += sourceMask[i].r * light.color * light.intensity;
                float ao = (sourceAO[i].r * 0.3333f + sourceAO[i].g * 0.3333f + sourceAO[i].b * 0.3333f) * 3;


                sourceMask[i] = encodeRGBMPixel(sourcePoint[i]);
                Vector3 pointColor = new Vector3(sourceMask[i].r, sourceMask[i].g, sourceMask[i].b);
                float pointIntensity = pointColor.magnitude / LIGHTMAP_HDR_RANGE * 2.5f;
                sourceDir[i].r = Mathf.Lerp(sourceMainDir[i].r, sourcePointDir[i].r, pointIntensity);
                sourceDir[i].g = Mathf.Lerp(sourceMainDir[i].g, sourcePointDir[i].g, pointIntensity);
                sourceDir[i].b = Mathf.Lerp(sourceMainDir[i].b, sourcePointDir[i].b, pointIntensity);
                sourceDir[i].a = ao;
                sourceCol[i] = encodeRGBMPixel(sourceMain[i]);
            }
            texDir.SetPixels(sourceDir);
            texDir.Apply();
            texMask.SetPixels(sourceMask);
            texMask.Apply();

            Texture2D texColNew = new Texture2D(texCol.width, texCol.height, TextureFormat.RGBAFloat, false, true);

            texColNew.SetPixels(sourceCol);
            texColNew.Apply();
            byte[] bytesDir = ImageConversion.EncodeToPNG(texDir);
            byte[] bytesCol = ImageConversion.EncodeToPNG(texColNew);
            byte[] bytesMask = ImageConversion.EncodeToPNG(texMask);


            File.WriteAllBytes(s_pathDir, bytesDir);
            File.WriteAllBytes(s_pathCol, bytesCol);
            File.WriteAllBytes(s_pathMask, bytesMask);
        }

        public static void OnlyFixLightmap()
        {
            Texture2D texCol = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathCol);

            Color[] sourceCol = loadHDRLightmapPixels(s_pathCol);

            GameObject mainLight = GameObject.Find("root/light/mainlight");
            Light light = null;
            if (mainLight)
                light = mainLight.GetComponentInChildren<Light>();

            for (int i = 0; i < sourceCol.Length; i++)
            {
                sourceCol[i] = encodeRGBMPixel(sourceCol[i]);
                sourceCol[i] = SubSaturation(sourceCol[i]);
            }

            texCol.SetPixels(sourceCol);
            texCol.Apply();
            byte[] bytesCol = ImageConversion.EncodeToPNG(texCol);

            File.WriteAllBytes(s_pathCol, bytesCol);
        }

        public static void FixSaturateSub()
        {
            Texture2D texCol = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathCol);
            Color[] sourceCol = texCol.GetPixels();

            for (int i = 0; i < sourceCol.Length; i++)
            {
                sourceCol[i] = SubSaturation(sourceCol[i]);
            }

            texCol.SetPixels(sourceCol);
            texCol.Apply();

            byte[] bytesCol = ImageConversion.EncodeToPNG(texCol);
            File.WriteAllBytes(s_pathCol, bytesCol);
        }

        public static void FixSaturateAdd()
        {
            Texture2D texCol = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathCol);
            Color[] sourceCol = texCol.GetPixels();

            for (int i = 0; i < sourceCol.Length; i++)
            {
                sourceCol[i] = AddSaturation(sourceCol[i]);
            }

            texCol.SetPixels(sourceCol);
            texCol.Apply();

            byte[] bytesCol = ImageConversion.EncodeToPNG(texCol);
            File.WriteAllBytes(s_pathCol, bytesCol);
        }

        public static void CombineShadowMaskToDir()
        {
            Texture2D texDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathDir);
            Texture2D texMask = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMask);


            Color[] sourceDir = texDir.GetPixels();
            Color[] sourceMask = texMask == null ? null : texMask.GetPixels();

            for (int i = 0; i < sourceDir.Length; i++)
            {
                sourceDir[i].a = sourceMask[i].r;
            }
            texDir.SetPixels(sourceDir);
            texDir.Apply();

            byte[] bytesDir = ImageConversion.EncodeToPNG(texDir);
            File.WriteAllBytes(s_pathDir, bytesDir);
        }

        public static void FixWithCombineAOToColorAndShadowMaskToDir()
        {
            Texture2D texCol = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathCol);
            Texture2D texDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathDir);
            Texture2D texPointDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathPointDir);
            Texture2D texMainDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMainDir);
            Texture2D texAO = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathAO);
            Texture2D texMask = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMask);
            Color[] sourceCol = texCol.GetPixels();
            Color[] sourceDir = texDir.GetPixels();
            Color[] sourcePoint = loadHDRLightmapPixels(s_pathPoint);
            Color[] sourcePointDir = new Color[sourceCol.Length];
            if (texPointDir)
                sourcePointDir = texPointDir.GetPixels();
            Color[] sourceMain = loadHDRLightmapPixels(s_pathMain);
            Color[] sourceMainDir = new Color[sourceCol.Length];
            if (texMainDir)
                sourceMainDir = texMainDir.GetPixels();
            Color[] sourceMask = new Color[sourceCol.Length];
            if (texMask)
                sourceMask = texMask.GetPixels();
            Color[] sourceAO = loadHDRLightmapPixels(s_pathAO);
            for (int i = 0; i < sourceDir.Length; i++)
            {
                float ao = (sourceAO[i].r * 0.3333f + sourceAO[i].g * 0.3333f + sourceAO[i].b * 0.3333f) * 3;
                //ao = Mathf.Clamp01(ao - aoIntensity);

                if (fixDirection)
                {
                    sourceCol[i] = sourcePoint[i] + sourceMain[i];
                    sourceCol[i] = encodeRGBMPixelEx(sourceCol[i]);
                    if (autoFixSaturate)
                    {
                        sourceCol[i] = SubSaturation(sourceCol[i]);
                    }
                    sourceCol[i].a = sourceMask[i].r;
                    sourcePoint[i] = encodeRGBMPixel(sourcePoint[i]);
                    Vector3 pointColor = new Vector3(sourcePoint[i].r, sourcePoint[i].g, sourcePoint[i].b);
                    Vector3 mainColor = new Vector3(sourceMain[i].r, sourceMain[i].g, sourceMain[i].b);
                    float pointIntensity = pointColor.magnitude;
                    float mainIntensity = mainColor.magnitude;
                    float factor = mainIntensity / (mainIntensity + pointIntensity);
                    factor = pointIntensity / LIGHTMAP_HDR_RANGE * 2.5f;

                    sourceDir[i].r = Mathf.Lerp(sourcePointDir[i].r, sourceMainDir[i].r, factor);
                    sourceDir[i].g = Mathf.Lerp(sourcePointDir[i].g, sourceMainDir[i].g, factor);
                    sourceDir[i].b = Mathf.Lerp(sourcePointDir[i].b, sourceMainDir[i].b, factor);
                }
                else
                {
                    sourceCol[i] = encodeRGBMPixelEx(sourceCol[i]);
                    if (autoFixSaturate)
                    {
                        sourceCol[i] = SubSaturation(sourceCol[i]);
                    }
                }

                sourceDir[i].a = ao;
            }
            if (fixDirection)
            {
                Texture2D newTex = new Texture2D(texDir.width, texDir.height);
                newTex.SetPixels(sourceDir);
                newTex.Apply();
                byte[] bytesDir = ImageConversion.EncodeToPNG(newTex);
                File.WriteAllBytes(s_pathDir, bytesDir);
            }

            Texture2D newColTex = new Texture2D(texCol.width, texCol.height);
            newColTex.SetPixels(sourceCol);
            newColTex.Apply();
            byte[] bytesCol = ImageConversion.EncodeToPNG(newColTex);
            File.WriteAllBytes(s_pathCol, bytesCol);
        }

        static Color SubSaturation(Color c)
        {
            return new Color(Mathf.Sqrt(c.r), Mathf.Sqrt(c.g), Mathf.Sqrt(c.b), c.a);
        }

        static Color AddSaturation(Color c)
        {
            return new Color(c.r * c.r, c.g * c.g, c.b * c.b, c.a);
        }

        public static void FixDirection()
        {
            Texture2D texCol = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathCol);
            Texture2D texDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathDir);
            Texture2D texPointDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathPointDir);
            Texture2D texMainDir = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMainDir);
            Texture2D texAO = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathAO);
            Texture2D texMask = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMask);
            Color[] sourceCol = loadHDRLightmapPixels(s_pathCol);
            Color[] sourceDir = new Color[sourceCol.Length];
            Color[] sourcePoint = loadHDRLightmapPixels(s_pathPoint);
            Color[] sourcePointDir = new Color[sourceCol.Length];
            if (texDir)
                sourceDir = texDir.GetPixels();
            if (texPointDir)
                sourcePointDir = texPointDir.GetPixels();
            Color[] sourceMain = loadHDRLightmapPixels(s_pathMain);
            Color[] sourceMainDir = new Color[sourceCol.Length];
            if (texMainDir)
                sourceMainDir = texMainDir.GetPixels();
            Color[] sourceAO = loadHDRLightmapPixels(s_pathAO);
            Color[] sourceMask = new Color[sourceCol.Length];
            if (texMask)
                sourceMask = texMask.GetPixels();

            for (int i = 0; i < sourceCol.Length; i++)
            {
                float ao = (sourceAO[i].r * 0.3333f + sourceAO[i].g * 0.3333f + sourceAO[i].b * 0.3333f);
                ao = Mathf.Min(ao, 1);
                //ao = Mathf.Clamp01(ao - aoIntensity);

                if (fixDirection)
                {
                    sourceCol[i] = sourcePoint[i] + sourceMain[i];
                    sourceCol[i] = encodeRGBMPixel(sourceCol[i]);
                    if (autoFixSaturate)
                    {
                        sourceCol[i] = SubSaturation(sourceCol[i]);
                    }
                    sourcePoint[i] = encodeRGBMPixel(sourcePoint[i]);
                    Vector3 pointColor = new Vector3(sourcePoint[i].r, sourcePoint[i].g, sourcePoint[i].b);
                    Vector3 mainColor = new Vector3(sourceMain[i].r, sourceMain[i].g, sourceMain[i].b);
                    float pointIntensity = pointColor.magnitude;
                    float mainIntensity = mainColor.magnitude;
                    float factor = mainIntensity / (mainIntensity + pointIntensity);
                    factor = pointIntensity / LIGHTMAP_HDR_RANGE * 2.5f;

                    sourceDir[i].r = Mathf.Lerp(sourcePointDir[i].r, sourceMainDir[i].r, factor);
                    sourceDir[i].g = Mathf.Lerp(sourcePointDir[i].g, sourceMainDir[i].g, factor);
                    sourceDir[i].b = Mathf.Lerp(sourcePointDir[i].b, sourceMainDir[i].b, factor);
                }
                else
                {
                    sourceCol[i] = encodeRGBMPixel(sourceCol[i]);
                    if (autoFixSaturate)
                    {
                        sourceCol[i] = SubSaturation(sourceCol[i]);
                    }
                }

                if (shadowMaskToDirA)
                {
                    if (texMask != null)
                        sourceMask[i].g = ao;
                    if (texDir != null)
                        sourceDir[i].a = sourceMask[i].r;
                }
                else
                {
                    //if (texMask != null)
                    //    sourceMask[i].g = ao;
                    if (texDir != null)
                        sourceDir[i].a = ao;
                }
            }
            if (fixDirection)
            {
                Texture2D newTex = new Texture2D(texDir.width, texDir.height);
                newTex.SetPixels(sourceDir);
                newTex.Apply();
                byte[] bytesDir = ImageConversion.EncodeToPNG(newTex);
                File.WriteAllBytes(s_pathDir, bytesDir);
            }

            Texture2D newColTex = new Texture2D(texCol.width, texCol.height);
            newColTex.SetPixels(sourceCol);
            newColTex.Apply();
            byte[] bytesCol = ImageConversion.EncodeToPNG(newColTex);
            File.WriteAllBytes(s_pathCol, bytesCol);

            if (texMask != null)
            {
                Texture2D newMaskTex = new Texture2D(texCol.width, texCol.height);
                newMaskTex.SetPixels(sourceMask);
                newMaskTex.Apply();
                byte[] bytesMask = ImageConversion.EncodeToPNG(newMaskTex);
                File.WriteAllBytes(s_pathMask, bytesMask);
            }
        }

        public static void ProcessClearOriginalLightmapTex()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                InitPath(data);
                ClearOriginalLightmapTex();
                break;
            }
        }

        public static void ProcessBackupLightmap()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                InitPath(data);
                BakeupLightmap();
            }
        }

        public static void ProcessBackupAO()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                InitPath(data);
                BackupAO();
            }
        }

        public static void ProcessBackupPoint()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                InitPath(data);
                BackupPoint();
            }
        }

        public static void ProcessBackupDirection()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                InitPath(data);
                BackupMain();
            }
        }

        public static void ProcessBackupShadowmask()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                InitPath(data);
                BackupShadowmask();
            }
        }

        public static void ProcessUnFixLightmap()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];

                InitPath(data);

                if (!CanRecover())
                    return;

                RecoverLightmap();

                Texture2D texDirOriginal = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathDir);
                Texture2D texColOriginal = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathCol);
                Texture2D texMaskOriginal = AssetDatabase.LoadAssetAtPath<Texture2D>(s_pathMask);

                LightmapData newData = new LightmapData();
                newData.lightmapDir = texDirOriginal;
                newData.lightmapColor = texColOriginal;
                newData.shadowMask = texMaskOriginal;
                lightmaps[k] = newData;
            }
            LightmapSettings.lightmaps = lightmaps;
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void ProcessFixLightmap(bool hasShadowMask = true)
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];
                InitPath(data);
                PrepareLightmapTex();
                ProcessLightmapTexFormat(true);
                PreProcessLightmapData();

                if (!CanFix())
                    return;

                FixLightmap();
                //ClearLightmapTex();
                ProcessLightmapTexFormat(false);
                PostProcessLightmapData();
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void ProcessOnlyFixLightmap(bool hasShadowMask = true)
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];
                InitPath(data);
                ProcessLightmapTexFormat(true);

                if (!CanFix())
                    return;

                OnlyFixLightmap();
                ProcessLightmapTexFormat(false);
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void ProcessFixAddSaturate(bool hasShadowMask = true)
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];
                InitPath(data);

                ProcessLightmapTexFormat(true);
                FixSaturateAdd();
                ProcessLightmapTexFormat(false);
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void ProcessFixSubSaturate(bool hasShadowMask = true)
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];
                InitPath(data);

                ProcessLightmapTexFormat(true);
                FixSaturateSub();
                ProcessLightmapTexFormat(false);
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void ProcessCombineShadowMaskToDir()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];
                InitPath(data);
                ProcessShadowMaskTexFormat(true);

                if (!CanFix())
                    return;

                CombineShadowMaskToDir();
                ClearShadowMaskTex();
                ProcessLightmapTexFormat(false);
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void ProcessCombineAOToColorAndShadowMaskToDir()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];
                InitPath(data);
                PrepareLightmapTex();
                ProcessLightmapTexFormat(true);

                //if (!CanFix())
                //    return;

                FixWithCombineAOToColorAndShadowMaskToDir();
                ClearLightmapTex();
                ProcessLightmapTexFormat(false);
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void ProcessFixDirection()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData data = lightmaps[k];
                InitPath(data);
                PrepareLightmapTex();
                ProcessLightmapTexFormat(true);

                if (!CanFix())
                    return;

                FixDirection();
                ClearLightmapTex();
                ProcessLightmapTexFormat(false);
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(EditorSceneManager.GetSceneAt(0).path);
        }

        public static void PreProcessLightmapData()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData newData = new LightmapData();
                newData.lightmapDir = lightmaps[k].lightmapDir;
                newData.lightmapColor = lightmaps[k].lightmapColor;
                newData.shadowMask = AssetDatabase.LoadAssetAtPath(s_pathMask, typeof(Texture2D)) as Texture2D;
                lightmaps[k] = newData;
            }
        }

        public static void PostProcessLightmapData()
        {
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            for (int k = 0; k < lightmaps.Length; k++)
            {
                LightmapData newData = new LightmapData();
                newData.lightmapDir = AssetDatabase.LoadAssetAtPath(s_pathDir, typeof(Texture2D)) as Texture2D;
                newData.lightmapColor = AssetDatabase.LoadAssetAtPath(s_pathCol, typeof(Texture2D)) as Texture2D;
                newData.shadowMask = AssetDatabase.LoadAssetAtPath(s_pathMask, typeof(Texture2D)) as Texture2D;
                lightmaps[k] = newData;
            }
        }

        public static void InitLightmapFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static Color[] loadHDRLightmapPixels(string path)
        {
            int width = 1024;
            int height = 1024;
            int channels = 4;
            if (!File.Exists(path))
            {
                Debug.LogWarning(string.Format("file [%0] don't exist!", path));
                return null;
            }
            float[] exrFloats = FantasyEXRTool.TinyEXRTool.LoadEXRImage(path, ref width, ref height, ref channels);
            Color[] rgba = FantasyEXRTool.TinyEXRTool.ConvertFloatAry2ColorAry(exrFloats, channels, new List<bool> { true, true, true, true });
            return revertColor(rgba, width, height);
        }

        public static Color[] revertColor(Color[] sourceColor, int width, int height)
        {
            Color[] targetColor = new Color[width * height];
            for (int i = 0; i < sourceColor.Length; i++)
            {
                int x = i % width;
                int y = i / width;
                int revIndex = Mathf.Min(x + (height - 1 - y) * width, sourceColor.Length - 1);
                targetColor[i] = sourceColor[revIndex];
                //targetColor[i] = encodeRGBMPixel(sourceColor[revIndex]);
            }
            return targetColor;
        }

        public static Color encodeRGBMPixel(Color input)
        {
            //input = new Color(Mathf.Sqrt(input.r), Mathf.Sqrt(input.g), Mathf.Sqrt(input.b), input.a);
            float m = Mathf.Max(Mathf.Max(input.r, 1e-4f), Mathf.Max(input.g, input.b));
            Color ret = new Color(input.r / m, input.g / m, input.b / m, Mathf.Sqrt(m / LIGHTMAP_HDR_RANGE)); //开个平方，来适应SRGB的取值范围
                                                                                                              //ret.a += (ret.a * 2.0f) * Mathf.Max(0, 0.3f - ret.a)/0.3f;
            return ret;
        }

        public static Color encodeRGBMPixelEx(Color input)
        {
            //input = new Color(Mathf.Sqrt(input.r), Mathf.Sqrt(input.g), Mathf.Sqrt(input.b), input.a);
            float m = Mathf.Max(Mathf.Max(input.r, 1e-4f), Mathf.Max(input.g, input.b));
            float f = Mathf.Sqrt(m / LIGHTMAP_HDR_RANGE) * 2.5f;
            Color ret = new Color(input.r / m * f, input.g / m * f, input.b / m * f, 0); //开个平方，来适应SRGB的取值范围  //ret.a += (ret.a * 2.0f) * Mathf.Max(0, 0.3f - ret.a)/0.3f;

            return ret;
        }

    }

    public class RenderSettingsData
    {
        public AmbientMode mode;
        public Color skyColor;
        public Color equatorColor;
        public Color groundColor;
        public float ambientIntensity;
        public float reflectionIntensity;
        public bool enableAO;

        public void SaveData()
        {
            mode = RenderSettings.ambientMode;
            skyColor = RenderSettings.ambientSkyColor;
            equatorColor = RenderSettings.ambientEquatorColor;
            groundColor = RenderSettings.ambientGroundColor;
            ambientIntensity = RenderSettings.ambientIntensity;
            reflectionIntensity = RenderSettings.reflectionIntensity;
        }

        public void RecoverData()
        {
            RenderSettings.ambientMode = mode;
            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = equatorColor;
            RenderSettings.ambientGroundColor = groundColor;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            //LightmapEditorSettings.enableAmbientOcclusion = enableAO;
        }

        public void InitAOSettings(float aoIndirect, float aoDirect, float indirectIntensity, float albedoBoost)
        {
            //RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientSkyColor = Color.white;
            RenderSettings.ambientEquatorColor = Color.white;
            RenderSettings.ambientGroundColor = Color.white;
            LightmapEditorSettings.enableAmbientOcclusion = true;
            LightmapEditorSettings.aoExponentIndirect = aoIndirect;
            LightmapEditorSettings.aoExponentDirect = aoDirect;
            //LightmapEditorSettings.aoMaxDistance = 1;
            Lightmapping.indirectOutputScale = indirectIntensity;
            Lightmapping.bounceBoost = albedoBoost;
        }

        public void InitPointColorSettings(bool halfAmbient = false, bool enablePointAO = false)
        {
            //RenderSettings.ambientMode = AmbientMode.Skybox;
            //RenderSettings.ambientIntensity = 0;
            if (halfAmbient)
            {
                RenderSettings.ambientSkyColor = skyColor / 2;
                RenderSettings.ambientEquatorColor = equatorColor / 2;
                RenderSettings.ambientGroundColor = groundColor / 2;
            }
            LightmapEditorSettings.enableAmbientOcclusion = enablePointAO;
            //LightmapEditorSettings.aoExponentIndirect = 3;
            //LightmapEditorSettings.aoExponentDirect = 1;
            //LightmapEditorSettings.aoMaxDistance = 1;
            //Lightmapping.indirectOutputScale = 1;
            //Lightmapping.bounceBoost = 1;
        }

        public void InitMainColorSettings(bool halfAmbient = false, bool enableDirectionAO = false)
        {
            //RenderSettings.ambientMode = AmbientMode.Skybox;
            //RenderSettings.ambientIntensity = 0;
            if (halfAmbient)
            {
                RenderSettings.ambientSkyColor = skyColor / 2;
                RenderSettings.ambientEquatorColor = equatorColor / 2;
                RenderSettings.ambientGroundColor = groundColor / 2;
            }
            LightmapEditorSettings.enableAmbientOcclusion = enableDirectionAO;
            //LightmapEditorSettings.aoExponentIndirect = 3;
            //LightmapEditorSettings.aoExponentDirect = 1;
            //LightmapEditorSettings.aoMaxDistance = 1;
            //Lightmapping.indirectOutputScale = 1;
            //Lightmapping.bounceBoost = 1;
        }
    }
}
