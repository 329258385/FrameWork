//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-11
// Author: LJP
// Date: 2020-05-11
// Description: 编辑器场景对象
//---------------------------------------------------------------------------------------
using UnityEngine;



namespace ActClient
{
    /// <summary>
    /// LOD
    /// </summary>
    [System.Serializable]
    public class EditorSceneObjectLod
    {
        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject go;

        /// <summary>
        /// 是否是公告板
        /// </summary>
        public bool isBillboard;

        /// <summary>
        /// 视野距离
        /// </summary>
        public float viewDist;
    }

    /// <summary>
    /// EditorSceneObject
    /// </summary>
    public class EditorSceneObject : MonoBehaviour
    {
        /// <summary>
        /// 指定场景树
        /// </summary>
        public string specSceneTree;

        /// <summary>
        /// 使用预制
        /// </summary>
        public bool usePrefab;

        /// <summary>
        /// Lod
        /// </summary>
        public EditorSceneObjectLod[] lods;
    }
}