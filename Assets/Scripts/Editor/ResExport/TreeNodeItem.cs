//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 场景树节点
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;






namespace ActEditor
{
    /// <summary>
    /// 节点LOD资源依赖
    /// </summary>
    public class TreeNodeObjectLodDep
    {
        public GameObject go;
        public List<ResDepBase> deps = new List<ResDepBase>();
        public ResDescCollector resCollector;
    }

    /// <summary>
    /// 每个渲染对象的资源依赖
    /// </summary>
    public class TreeNodeObjectDep
    {
        /// <summary>
        /// 依赖
        /// </summary>
        public List<TreeNodeObjectLodDep> lods = new List<TreeNodeObjectLodDep>();
    }
}