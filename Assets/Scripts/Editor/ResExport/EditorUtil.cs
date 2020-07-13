//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-19
// Author: LJP
// Date: 2020-05-19
// Description: 查找各种渲染资源组件工具
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System;
using ActClient;





namespace ActEditor
{
    public class EditorUtil
    {
       
        /// <summary>
        /// 是否父对象
        /// </summary>
        public static bool IsParent(GameObject parent, GameObject child)
        {
            if (null == child || null == parent)
            {
                return false;
            }

            if (child == parent)
            {
                return true;
            }
            if(child.transform.parent == parent.transform)
            {
                return true;
            }
            return IsParent(parent, child.transform.parent.gameObject);
        }

        /// <summary>
        /// 遍历组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="bVisitChildren"></param>
        /// <param name="visitor"></param>
        static public void VisitComponents<T>(GameObject go, bool bVisitChildren, Action<T> visitor) where T : Component
        {
            T[] cs = null;
            if(bVisitChildren)
            {
                cs = go.GetComponentsInChildren<T>();
            }
            else
            {
                cs = go.GetComponents<T>();
            }
            for(int i = 0; i < cs.Length; ++i)
            {
                var c = cs[i];
                visitor(c);
            }
        }

        /// <summary>
        /// 遍历材质
        /// </summary>
        /// <param name="go"></param>
        /// <param name="bVisitChildren"></param>
        /// <param name="visitor"></param>
        static public void VisitMaterials(GameObject go, bool bVisitChildren, Action<Material> visitor)
        {
            Renderer[] renderers = null;
            if(bVisitChildren)
            {
                renderers = go.GetComponentsInChildren<Renderer>();
            }
            else
            {
                renderers = go.GetComponents<Renderer>();
            }

            for(int i = 0; i < renderers.Length; ++i)
            {
                var mats = renderers[i].sharedMaterials;
                for(    int j = 0; j < mats.Length; ++j)
                {
                    var m = mats[j];
                    if(null == m)
                    {
                        //Debug.LogErrorFormat("Null material of \"{0}\"!", go.name);
                        continue;
                    }
                    visitor(m);
                }
            }
        }

        /// <summary>
        /// 遍历Shader属性
        /// </summary>
        /// <param name="go"></param>
        /// <param name="bVisitChildren"></param>
        /// <param name="visitor"></param>
        static public void VisitShaderProperties(GameObject go, bool bVisitChildren, Action<Material, int> visitor)
        {
            VisitMaterials(go, bVisitChildren, m =>
            {
                var propCount = UnityEditor.ShaderUtil.GetPropertyCount(m.shader);
                for(int np = 0; np < propCount; ++np)
                {
                    visitor(m, np);
                }
            });
        }

        /// <summary>
        /// 遍历Shader属性
        /// </summary>
        /// <param name="go"></param>
        /// <param name="visitor"></param>
        static public void VisitShaderProperties(Material m, Action<int> visitor)
        {
            var propCount = UnityEditor.ShaderUtil.GetPropertyCount(m.shader);
            for(int np = 0; np < propCount; ++np)
            {
                visitor(np);
            }
        }


        /// <summary>
        /// 获取场景输出路径
        /// </summary>
        public static string GetSceneOutputPath(string subPath)
        {
            var sceneName = UtilityTools.GetFileName(EditorApplication.currentScene);
            string path = null;
            if (string.IsNullOrEmpty(subPath))
            {
                path = string.Format("Assets/Export/Back/Output/{0}", sceneName);
                UtilityTools.CreateDir(path);
            }
            else
            {
                path = string.Format("Assets/Export/Back/Output/{0}/{1}", sceneName, subPath);
                UtilityTools.CreateDir(path);
            }
            return path;
        }
    }
}