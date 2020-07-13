//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-20
// Author: LJP
// Date: 2020-05-20
// Description: 模型依赖
//---------------------------------------------------------------------------------------
using UnityEngine;
using ActClient;
using System.Collections.Generic;





namespace ActEditor
{
    public class ResDepMesh : ResDepBase
    {
        class Item
        {
            public GameObject   go;
            public MeshFilter   filter;
            public Mesh         mesh;
        }
        List<Item> _items = new List<Item>();

        /// <summary>
        /// 资源类型
        /// </summary>
        override public ResType resType { get { return ResType.Mesh; } }

        /// <summary>
        /// 收集依赖
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static ResDepMesh CollectDependencies(GameObject go, bool bCollectChildren, MeshFilter meshfilter )
        {
            ResDepMesh dep = null;
            EditorUtil.VisitComponents<MeshFilter>(go, bCollectChildren, filter =>
            {
                // 没有Mesh
                if(null == filter.sharedMesh)
                {
                    Debug.LogErrorFormat("Null mesh of \"{0}\"!", go.name);
                    return;
                }

                if(null == dep)
                {
                    dep = new ResDepMesh();
                }

                
                var item            = new Item();
                item.go             = go;
                if(meshfilter != null )
                    item.filter     = meshfilter;
                else
                    item.filter     = filter;
                item.mesh           = filter.sharedMesh;
                dep._items.Add(item);
                AssetsBackuper.inst.Backup(item.filter);
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
                
                string res = visitor(item.mesh);
                resCollector.AddMesh(res, item.filter );
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
                item.filter.sharedMesh = null;
            }
        }
    }
}