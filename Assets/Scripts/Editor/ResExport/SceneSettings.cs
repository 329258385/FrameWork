//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-12
// Author: LJP
// Date: 2020-05-12
// Description: 场景资源
//---------------------------------------------------------------------------------------
using UnityEngine;
using ActClient;
using System.Collections.Generic;




namespace ActEditor
{
    /// <summary>
    /// 场景树设置
    /// </summary>
    [System.Serializable]
    public class SceneTreeSettings
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string               name;

        /// <summary>
        /// 划分类型
        /// </summary>
        public SceneTreeSplitType   splitType;

        /// <summary>
        /// 对象类型
        /// </summary>
        public SceneTreeObjType     objType;

        /// <summary>
        /// 树索引
        /// </summary>
        public SceneTreeIndex       Index;

        /// <summary>
        /// 最大深度
        /// </summary>
        public int                  maxDepth;

        /// <summary>
        /// 视野距离
        /// </summary>
        public float                viewDistance;

        /// <summary>
        /// 最大对象包围盒大小
        /// </summary>
        public float                maxItemBoundsSize;
    }

    /// <summary>
    /// 场景数据
    /// </summary>
    public class SceneSettings : ScriptableObject
    {
        /// <summary>
        /// 树列表
        /// </summary>
        public List<SceneTreeSettings> trees;
    }
}