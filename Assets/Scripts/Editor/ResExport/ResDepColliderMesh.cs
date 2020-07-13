//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-19
// Author: LJP
// Date: 2020-05-19
// Description: 碰撞模型依赖，只记录MeshCollider
//---------------------------------------------------------------------------------------
using UnityEngine;
using ActClient;
using System.Collections.Generic;
using System;




namespace ActEditor
{
    public class ResDepColliderMesh : ResDepBase
    {
        public List<MeshCollider> _colliders = new List<MeshCollider>();

        /// <summary>
        /// 资源类型
        /// </summary>
        override public ResType resType { get { return ResType.Mesh; } }

        /// <summary>
        /// 收集依赖
        /// </summary>
        public static ResDepColliderMesh CollectDependencies(List<MeshCollider> colliders)
        {
            ResDepColliderMesh dep  = null;
            for(int i = 0, ci = colliders.Count; i < ci; ++i)
            {
                var c = colliders[i];
                if(null == c.sharedMesh)
                {
                    continue;
                }
                if(null == dep)
                {
                    dep = new ResDepColliderMesh();
                }
                dep._colliders.Add(c);
            }

            return dep;
        }

        /// <summary>
        /// 遍历资源
        /// </summary>
        override protected void VisitAssets(ResDescCollector resCollector, VisitResDelegate visitor)
        {
            for(int i = 0, ci = _colliders.Count; i < ci;  i++)
            {
                var resName = visitor(_colliders[i].sharedMesh);
                resCollector.AddColliderMesh(resName, _colliders[i]);
            }
        }

        /// <summary>
        /// 移除依赖
        /// </summary>
        override public void RemoveDependencies()
        {
            for(int i = 0, ci = _colliders.Count; i < ci; i++)
            {
                _colliders[i].sharedMesh = null;
            }
        }
    }
}