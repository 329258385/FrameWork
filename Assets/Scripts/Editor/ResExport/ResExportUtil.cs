//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-18
// Author: LJP
// Date: 2020-05-18
// Description: 资源导出公共方法
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using ActClient;





namespace ActEditor
{

    /// <summary>
    /// 导出资源描述
    /// </summary>
    public class ResExportUtil
    {
        /// <summary>
        /// 导出资源包
        /// </summary>
        public static bool ExportRes(ResExportDesc rd, bool compress)
        {
            // 导出
            string pathName = UtilityTools.GenResExportPath(rd.resDir, rd.resName);
            var names       = new string[] { rd.resName };
            var assets      = new UnityEngine.Object[] { rd.asset };
            var options     = BuildAssetBundleOptions.DeterministicAssetBundle;
            if (!compress)
            {
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }
            if (!BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, names, pathName, options, EditorUserBuildSettings.activeBuildTarget))
            {
                Debug.LogErrorFormat("ExportAssetBundle {0} failed!", pathName);
                return false;
            }

            return true;
        }
        /// <summary>
        /// 导出场景
        /// </summary>
        public static bool ExportCurrentScene(bool useSceneFolder, bool compress)
        {
            var sceneName = UtilityTools.GetFileName(EditorApplication.currentScene);
            string pathName;
            if(useSceneFolder)
            {
                pathName = UtilityTools.GenResExportPath(string.Format("scene/{0}", sceneName), sceneName);
            }
            else
            {
                pathName = UtilityTools.GenResExportPath("scene", sceneName);
            }

            if(string.IsNullOrEmpty(pathName))
            {
                return false;
            }

            var options     = BuildOptions.None;
            if(!compress)
            {
                options     |= BuildOptions.UncompressedAssetBundle;
            }

            // 导出
            var levels      = new string[]{ EditorApplication.currentScene };
            BuildPipeline.PushAssetDependencies();
            string error = BuildPipeline.BuildStreamedSceneAssetBundle(levels, pathName, EditorUserBuildSettings.activeBuildTarget, options);
            if (!string.IsNullOrEmpty(error))
            {
                EndExportAsset();
                Debug.LogErrorFormat("Export Scene \"{0}\" failed! error: {1}", sceneName, error);
                return false;
            }

            EndExportAsset();
            return true;
        }

        /// <summary>
        /// 结束导出资源
        /// </summary>
        public static void EndExportAsset()
        {
            BuildPipeline.PopAssetDependencies();
        }
    }
}