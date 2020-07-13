//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-20
// Author: LJP
// Date: 2020-05-20
// Description: 蒙皮模型依赖
//---------------------------------------------------------------------------------------
using UnityEngine;
using ActClient;
using System.Collections.Generic;





namespace ActEditor
{
    public class ResDepSkinnedMesh : ResDepBase
    {
        class Item
        {
            public GameObject go;
            public SkinnedMeshRenderer renderer;
            public Mesh mesh;
        }
        List<Item> _items = new List<Item>();

        /// <summary>
        /// 资源类型
        /// </summary>
        override public ResType resType { get { return ResType.Mesh; } }

        /// <summary>
        /// 收集依赖
        /// </summary>
        public static ResDepSkinnedMesh CollectDependencies(GameObject go, bool bCollectChildren)
        {
            ResDepSkinnedMesh dep = null;
            EditorUtil.VisitComponents<SkinnedMeshRenderer>(go, bCollectChildren, renderer =>
            {
                // 没有Mesh
                if(null == renderer.sharedMesh)
                {
                    Debug.LogErrorFormat("Null mesh of \"{0}\"!", go.name);
                    return;
                }

                if(null == dep)
                {
                    dep = new ResDepSkinnedMesh();
                }

                // 生成信息
                var item        = new Item();
                item.go         = go;
                item.renderer   = renderer;
                item.mesh       = renderer.sharedMesh;
                dep._items.Add(item);

                // 备份材质
                AssetsBackuper.inst.Backup(item.mesh);
            });

            return dep;
        }

        /// <summary>
        /// 遍历资源
        /// </summary>
        override protected void VisitAssets(ResDescCollector resCollector, VisitResDelegate visitor)
        {
            for(int i = 0, ci = _items.Count; i < ci; ++i)
            {
                var item    = _items[i];
                var resName = visitor(item.mesh);

                // res
                resCollector.AddSkinnedMesh(resName, item.renderer);
            }
        }

        /// <summary>
        /// 移除依赖
        /// </summary>
        override public void RemoveDependencies()
        {
            for(int i = 0, ci = _items.Count; i < ci; ++i)
            {
                var item                 = _items[i];
                item.renderer.sharedMesh = null;
                item.renderer.rootBone   = null;
            }
        }
    }
}