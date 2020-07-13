//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-12
// Author: LJP
// Date: 2020-05-12
// Description: 工具函数集【资源路径】
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using AssetBundles;




namespace ActClient
{
    public class UtilityTools
    {
        /// <summary>
        /// 客户端使用
        /// </summary>
        public static string _AssetPathSub = "Binary";
        public static string _streamingAssetsPath = string.Empty;
        public static string _persistentDataPath = string.Empty;

        public static void InitResPath()
        {
            _persistentDataPath     = StringHelper.Format("{0}/{1}/{2}", Application.persistentDataPath,  AssetBundleConfig.AssetBundlesFolderName, UtilityTools._AssetPathSub);
            _streamingAssetsPath    = StringHelper.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName, UtilityTools._AssetPathSub);
        }

        /// <summary>
        /// 路径检查
        /// </summary>
        public static bool CheckPathValid(string path, string subPath, bool displayError)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string pathForCheck = path;
            if (!Directory.Exists(pathForCheck))
            {
                return false;
            }

            return true;
        }


        //---------------------------------------------------------------------------------------
        // 获取文件名
        //---------------------------------------------------------------------------------------
        public static string GetFileName(string path)
        {
            path                = path.Replace('\\', '/');
            string[] arrPath    = path.Split('/');
            if (arrPath.Length < 1)
            {
                return "";
            }

            return arrPath[arrPath.Length - 1].Split('.')[0];
        }


        /// ---------------------------------------------------------------------------------------
        /// <summary>
        /// 生成资源导出路径
        /// </summary>
        /// ---------------------------------------------------------------------------------------
        public static string GenResExportPath(string subPath, string name)
        {
            if (string.IsNullOrEmpty(subPath) || string.IsNullOrEmpty(name))
            {
                Debug.LogError("Invalid param!");
                return null;
            }

            string aboutputPath = string.Empty;
            #if UNITY_EDITOR
                aboutputPath = Path.Combine(AssetBundleConfig.AssetBundlesBuildOutputPath, "Android/Android/AssetBundles/");
            #elif UNITY_IOS
                aboutputPath = Path.Combine(AssetBundleConfig.AssetBundlesBuildOutputPath, "iOS/iOS/AssetBundles/" );
            #endif

            // 路径
            string path         = string.Format("{0}/{1}/{2}", aboutputPath, _AssetPathSub, subPath);
            if (!CheckPathValid(path, null, false))
            {
                return null;
            }
            string pathName     = string.Format("{0}/{1}.ab", path, name);
            CheckExportPath(pathName);
            return pathName;
        }


        private static Regex regex = new Regex(@"^[A-Za-z0-9_().]+$");
        
        
        /// <summary>
        /// 检查导出路径
        /// </summary>
        private static void CheckExportPath(string path)
        {
            var fileName = Path.GetFileName(path);
            if (!regex.IsMatch(fileName))
            {
                // 故意多刷
                for (int i = 0; i < 5; ++i)
                {
                    Debug.LogError("INVALID PATH. FILE NAME IS {0}." + path);
                }
            }
        }


        //---------------------------------------------------------------------------------------
        // 获取目录名
        //---------------------------------------------------------------------------------------
        public static string GetDir(string fullPath)
        {
            var path = fullPath.Replace('\\', '/');
            string[] arrPath = path.Split('/');
            if (arrPath.Length < 2)
            {
                return "";
            }

            if (!arrPath[arrPath.Length - 1].Contains("."))
            {
                return fullPath;
            }
            return string.Join("/", arrPath, 0, arrPath.Length - 1);
        }


        //---------------------------------------------------------------------------------------
        // 创建文件夹
        //---------------------------------------------------------------------------------------
        public static void CreateDir(string fullPath)
        {
            var dir = GetDir(fullPath);
            if (string.IsNullOrEmpty(dir))
            {
                return;
            }
            if (Directory.Exists(dir))
            {
                return;
            }
            Directory.CreateDirectory(dir);
        }


        /// <summary>
        /// 判断Tag是否是EditorOnly
        /// </summary>
        public static bool IsEditorOnly(GameObject go)
        {
            if (null == go)
            {
                return false;
            }

            if (go.CompareTag("EditorOnly"))
            {
                return true;
            }

            if (go.CompareTag("EditorTerrain"))
            {
                return true;
            }

            var parent = go.transform.parent;
            if (null == parent)
            {
                return false;
            }
            return IsEditorOnly(parent.gameObject);
        }
    }
}