//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 场景导出窗口
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using ActClient;
using UnityEngine.SceneManagement;






namespace ActEditor
{
    /// <summary>
    /// 场景导出窗口
    /// </summary>
    public class SceneExportWindow : EditorWindow
    {
        //---------------------------------------------------------------------------------------
        // 成员变量
        //---------------------------------------------------------------------------------------
        string          _sceneName;
        SceneSettings   _settings;
        bool            _backScene = true;
        bool            _compressRes = false;
        bool            _compressScene = false;
        bool            _recovertScene = false;

        //---------------------------------------------------------------------------------------
        // 开启
        //---------------------------------------------------------------------------------------
        void OnEnable()
        {
            _sceneName = UtilityTools.GetFileName(EditorApplication.currentScene);
            LoadSettings();
        }

        //---------------------------------------------------------------------------------------
        // 关闭
        //---------------------------------------------------------------------------------------
        void OnDisable()
        {
            _sceneName = null;
        }

        /// <summary>
        /// 导出当前场景
        /// </summary>
        [MenuItem(ActEditorMenuItem.ExportScene, false, ActEditorMenuItem.ExportScenePriority)]
        public static void ExportCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            if(null == scene)
            {
                return;
            }
            SceneExportWindow window    = (SceneExportWindow)EditorWindow.GetWindowWithRect(typeof(SceneExportWindow), new Rect(0, 0, 960, 750), false, "导出场景");
            window.ShowPopup();
        }

        /// <summary>
        /// 测试导出场景AB
        /// </summary>
        /// <returns></returns>
        [MenuItem(ActEditorMenuItem.TestExportScene, false, ActEditorMenuItem.TestExportScenePriority)]
        private static bool TestExportCurrentScene()
        {
            ResExportUtil.ExportCurrentScene(true, false);
            return true;
        }

        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        /// <returns></returns>
        string GetSettingsPath()
        {
            if (string.IsNullOrEmpty(_sceneName))
            {
                return null;
            }

            var dir = string.Format("Assets/Export/Config/Scene/{0}", _sceneName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var path = string.Format("{0}/setting.asset", dir);
            return path;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        void LoadSettings()
        {
            if(string.IsNullOrEmpty(_sceneName))
            {
                _settings   = GenDefaultSceneSettings();
                return;
            }

            var path        = GetSettingsPath();
            if(string.IsNullOrEmpty(path))
            {
                _settings   = GenDefaultSceneSettings();
                return;
            }

            _settings = AssetDatabase.LoadMainAssetAtPath(path) as SceneSettings;
            if(null == _settings)
            {
                _settings = GenDefaultSceneSettings();
                SaveSettings();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        void SaveSettings()
        {
            var path = GetSettingsPath();
            if(string.IsNullOrEmpty(path))
            {
                return;
            }

            if(null == _settings)
            {
                _settings = GenDefaultSceneSettings();
            }

            var s = AssetDatabase.LoadMainAssetAtPath(path) as SceneSettings;
            if(null == s)
            {
                AssetDatabase.CreateAsset(_settings, path);
            }
            else
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 生成默认场景配置
        /// </summary>
        SceneSettings GenDefaultSceneSettings()
        { 
            var settings        = ScriptableObject.CreateInstance<SceneSettings>();
            settings.trees      = new List<SceneTreeSettings>();
            // 碰撞
            var tree            = new SceneTreeSettings();
            tree.name           = "collider";
            tree.splitType      = SceneTreeSplitType.Quad;
            tree.objType        = SceneTreeObjType.Collider;
            tree.Index          = SceneTreeIndex.Collider;
            tree.maxDepth       = 6;
            tree.viewDistance   = 50f;
            tree.maxItemBoundsSize = 1.0e+7f;
            settings.trees.Add(tree);

            // 草 ,去掉草，草通过特殊的方式去管理和绘制
            tree = new SceneTreeSettings();
            tree.name           = "grass";
            tree.splitType      = SceneTreeSplitType.Quad;
            tree.objType        = SceneTreeObjType.Renderer;
            tree.Index          = SceneTreeIndex.Grass;
            tree.maxDepth       = 6;
            tree.viewDistance   = 25f;
            tree.maxItemBoundsSize = 1.0e+7f;
            settings.trees.Add(tree);

            // 近景
            tree = new SceneTreeSettings();
            tree.name           = "near";
            tree.splitType      = SceneTreeSplitType.Quad;
            tree.objType        = SceneTreeObjType.Renderer;
            tree.Index          = SceneTreeIndex.Near;
            tree.maxDepth       = 6;
            tree.viewDistance   = 25f;
            tree.maxItemBoundsSize = 10.0f;
            settings.trees.Add(tree);

            // 中景
            tree = new SceneTreeSettings();
            tree.name           = "mid";
            tree.splitType      = SceneTreeSplitType.Quad;
            tree.objType        = SceneTreeObjType.Renderer;
            tree.Index          = SceneTreeIndex.Mid;
            tree.maxDepth       = 6;
            tree.viewDistance   = 150f;
            tree.maxItemBoundsSize = 50.0f;
            settings.trees.Add(tree);

            // 远景
            tree = new SceneTreeSettings();
            tree.name           = "far";
            tree.splitType      = SceneTreeSplitType.Quad;
            tree.objType        = SceneTreeObjType.Renderer;
            tree.Index          = SceneTreeIndex.Far;
            tree.maxDepth       = 6;
            tree.viewDistance   = 250f;
            tree.maxItemBoundsSize = 1.0e+7f;
            settings.trees.Add(tree);

            return settings;
        }

        /// <summary>
        /// 生成场景
        /// </summary>
        void BuildScene(bool exportScene, bool exportRes, bool compressScene, bool compressRes)
        {
            // 先保存场景
            EditorApplication.SaveScene();

            // 场景名字
            var scenePath       = EditorApplication.currentScene;
            string sceneName    = UtilityTools.GetFileName(scenePath);

            // 导出目录
            var exportPath      = UtilityTools.GenResExportPath(string.Format("scene/{0}", sceneName), sceneName);
            var exportDir       = Path.GetDirectoryName(exportPath);

            // 删除目录
            if(exportRes && exportScene)
            {
                Directory.Delete(exportDir, true);
            }

            // 创建目录
            if(!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            /// 开始处理数据
            try
            {
                do
                {
                    AssetsBackuper.inst.Clear();
                    if (exportScene)
                    {
                        var outputDir  = EditorUtil.GetSceneOutputPath(null);
                        AssetDatabase.DeleteAsset(outputDir);
                        var outputPath = EditorUtil.GetSceneOutputPath(string.Format("{0}.unity", sceneName));
                        EditorApplication.SaveScene(outputPath);
                    }

                    // 树结构
                    var meshColliders = new List<MeshCollider>();
                    var trees = SceneTree.Build(_settings.trees, meshColliders);
                    if(null == trees)
                    {
                        break;
                    }
                    SaveSettings();
                    SceneTree.Export(trees, meshColliders, exportRes, compressRes);

                    // 准备导出
                    EditorApplication.SaveScene();
                    AssetDatabase.SaveAssets();

                    if (exportScene)
                    {
                        ResExportUtil.ExportCurrentScene(true, compressScene);
                    }
                }
                while(false);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                AssetsBackuper.inst.Recover();
                AssetDatabase.SaveAssets();
            }

            finally
            {
                // 还原修改过的资源和场景
                if (_recovertScene)
                {
                    AssetsBackuper.inst.Recover();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorApplication.OpenScene(scenePath);
                    Resources.UnloadUnusedAssets();
                }
            }
        }

        //---------------------------------------------------------------------------------------
        // OnGUI
        //---------------------------------------------------------------------------------------
        void OnGUI()
        {
            // 场景名字
            var scenePath               = EditorApplication.currentScene;
            string sceneName            = UtilityTools.GetFileName(scenePath);
            if(_sceneName != sceneName)
            {
                _sceneName = sceneName;
                LoadSettings();
            }

            EditorGUI.BeginChangeCheck();

            // 树列表
            EditorGUILayout.LabelField("场景树");
            for (int i = 0; i < _settings.trees.Count; ++i)
            {
                DrawSeparator(1, 1);

                var tree                = _settings.trees[i];
                tree.name               = EditorGUILayout.TextField("名称", tree.name);
                tree.splitType          = (SceneTreeSplitType)EditorGUILayout.EnumPopup("划分类型", tree.splitType);
                tree.objType            = (SceneTreeObjType)EditorGUILayout.EnumPopup("对象类型", tree.objType);
                tree.maxDepth           = EditorGUILayout.IntField("最大层数", tree.maxDepth);
                tree.viewDistance       = EditorGUILayout.FloatField("可视距离", tree.viewDistance);
                tree.maxItemBoundsSize  = EditorGUILayout.FloatField("最大物体大小", tree.maxItemBoundsSize);
            }

            // 导出
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _recovertScene              = GUILayout.Toggle(_recovertScene, "导出完是否恢复");
            _backScene                  = GUILayout.Toggle(_backScene,      "备份场景");
            _compressScene              = GUILayout.Toggle(_compressScene,  "压缩场景");
            _compressRes                = GUILayout.Toggle(_compressRes,    "压缩资源");
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if(GUILayout.Button("检查设置", GUILayout.Width(100)))
            {
                int settingsObjTypeFlag = 0;
                SceneTree.CheckSettings(_settings.trees, out settingsObjTypeFlag);
            }

            else if(GUILayout.Button("导出资源", GUILayout.Width(100)))
            {
                BuildScene(_backScene, true, _compressScene, _compressRes);
            }

            else
            {
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                if(EditorGUI.EndChangeCheck())
                {
                    SaveSettings();
                }
            }
        }

        public int DrawSeparator(int space, int height = 2)
        {
            GUILayout.Space(space);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
            GUILayout.Space(space);

            return space * 2 + height;
        }
    }
}