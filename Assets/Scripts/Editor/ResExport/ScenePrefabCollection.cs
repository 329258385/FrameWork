//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 场景预制收集器
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using ActClient;
using UnityEditor;






namespace ActEditor
{
    /// <summary>
    /// 场景预制收集器
    /// </summary>
    public class ScenePrefabCollection
    {
        /// <summary>
        /// 场景预制
        /// </summary>
        class ScenePrefab
        {
            public SceneTreeNodeObjectData data;
            public string assetPath;
            public int refCount;
        }

        /// <summary>
        /// 预制列表
        /// </summary>
        List<ScenePrefab> _prefabs = new List<ScenePrefab>();

        /// <summary>
        /// 添加
        /// </summary>
        public int AddPrefab(SceneTreeNodeObjectData obj)
        {
            // 预制
            var prefab = PrefabUtility.GetPrefabParent(obj.go) as GameObject;
            if (null == prefab)
            {
                Debug.LogErrorFormat("Null prefab of {0}", obj.go.name);
                return -1;
            }

            // 资源路径
            var assetPath = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(assetPath))
            {
                return -1;
            }

            // 查找
            for (int i = 0, ci = _prefabs.Count; i < ci; ++i)
            {
                var p = _prefabs[i];
                if (p.assetPath == assetPath)
                {
                    ++p.refCount;
                    return i;
                }
            }

            // 新建
            var newPrefab = new ScenePrefab
            {
                data = obj,
                assetPath = assetPath,
                refCount = 1
            };
            newPrefab.data.go.name = prefab.name;
            _prefabs.Add(newPrefab);

            return _prefabs.Count - 1;
        }

        /// <summary>
        /// 导出
        /// </summary>
        public SceneObjectPrefabData[] Export()
        {
            var arr = new SceneObjectPrefabData[_prefabs.Count];
            for (int i = 0; i < arr.Length; ++i)
            {
                var p = _prefabs[i];
                p.data.trans.localPosition = Vector3.zero;
                p.data.trans.localRotation = Quaternion.identity;
                p.data.trans.localScale = Vector3.one;
                var data = new SceneObjectPrefabData
                {
                    obj = p.data,
                    refCountInScene = p.refCount
                };
                arr[i] = data;
            }
            return arr;
        }

        /// <summary>
        /// 获取引用计数
        /// </summary>
        public int GetRefCount(int prefabId)
        {
            if (prefabId < 0 || prefabId >= _prefabs.Count)
            {
                return -1;
            }
            return _prefabs[prefabId].refCount;
        }
    }
}