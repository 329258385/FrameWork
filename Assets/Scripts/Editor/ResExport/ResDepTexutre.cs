//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-19
// Author: LJP
// Date: 2020-05-19
// Description: 贴图依赖
//---------------------------------------------------------------------------------------
using UnityEngine;
using ActClient;
using System.Collections.Generic;
using UnityEditor;

namespace ActEditor
{
    public class ResDepTexture : ResDepBase
    {
        /// <summary>
        /// 条目
        /// </summary>
        class Item
        {
            public GameObject go;
            public Material mat;
            public string propName;
            public Texture texture;
        }
        List<Item> _items = new List<Item>();

        /// <summary>
        /// 资源类型
        /// </summary>
        override public ResType resType { get { return ResType.Texture; } }

        /// <summary>
        /// 收集依赖
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static ResDepTexture CollectDependencies(GameObject go, bool bCollectChildren)
        {
            ResDepTexture dep = null;
            EditorUtil.VisitShaderProperties(go, bCollectChildren, (m, n) =>
            {
                CollectDependencies(ref dep, go, m, n);
            });

            return dep;
        }

        /// <summary>
        /// 收集依赖
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static ResDepTexture CollectDependencies(Material m)
        {
            ResDepTexture dep = null;
            EditorUtil.VisitShaderProperties(m, (n) =>
            {
                CollectDependencies(ref dep, null, m, n);
            });

            return dep;
        }

        /// <summary>
        /// 收集依赖
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="m"></param>
        /// <param name="shaderPropIndex"></param>
        static void CollectDependencies(ref ResDepTexture dep, GameObject go, Material m, int shaderPropIndex)
        {
            var pt = UnityEditor.ShaderUtil.GetPropertyType(m.shader, shaderPropIndex);
            if(UnityEditor.ShaderUtil.ShaderPropertyType.TexEnv != pt)
            {
                return;
            }

            // 获取贴图
            var propName    = UnityEditor.ShaderUtil.GetPropertyName(m.shader, shaderPropIndex);
            var propId      = Shader.PropertyToID(propName);
            var tex         = m.GetTexture(propId);

            // 没有贴图
            if(null == tex)
            {
                return;
            }

            if(null == dep)
            {
                dep         = new ResDepTexture();
            }

            // 生成信息
            var item        = new Item();
            item.go         = go;
            item.mat        = m;
            item.propName   = propName;
            item.texture    = tex;
            dep._items.Add(item);

            // 备份材质
            AssetsBackuper.inst.Backup(item.mat);
        }

        /// <summary>
        /// 遍历资源
        /// </summary>
        override protected void VisitAssets(ResDescCollector resCollector, VisitResDelegate visitor)
        {
            for(int i = 0, ci = _items.Count; i < ci; ++i)
            {
                var item    = _items[i];
                var resName = visitor(item.texture);
                resCollector.AddTexture(resName, item.mat, item.propName);
            }
        }

        /// <summary>
        /// 移除依赖
        /// </summary>
        override public void RemoveDependencies()
        {
            for(int i = 0, ci = _items.Count; i < ci; ++i)
            {
                var item = _items[i];
                item.mat.SetTexture(item.propName, null);
            }
        }
    }
}