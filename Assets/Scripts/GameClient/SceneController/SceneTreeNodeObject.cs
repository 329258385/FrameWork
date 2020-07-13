//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-21
// Author: LJP
// Date: 2020-05-21
// Description: 客户端运行时场景树节点
//---------------------------------------------------------------------------------------
using UnityEngine;
using System;





namespace ActClient
{
    public class SceneTreeNodeObject : IDisposable
    {

        /// <summary>
        /// 场景树
        /// </summary>
        SceneTree                       _tree;

        /// <summary>
        /// 数据
        /// </summary>
        public SceneTreeNodeObjectData  _data;
            
        /// <summary>
        /// 资源
        /// </summary>
        ResCollection                   _resCollection;

        /// <summary>
        /// 资源是否已经加载
        /// </summary>
        bool                            _resLoaded;

        /// <summary>
        /// 是否使用prefab, 
        /// </summary>
        int                             _idxPrefab = -1;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(SceneTree tree, SceneTreeNodeObjectData data, int idxPrefab )
        {
            _tree       = tree;
            _data       = data;
            _idxPrefab  = idxPrefab;
            if (_tree.data.objType == SceneTreeObjType.Renderer && !_data.lods[0].resCollection.isEmpty)
            {
                _resCollection = ObjectPool.New<ResCollection>();
                _resCollection.Init(_data.lods[0].resCollection);
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            ObjectPool.Release(ref _resCollection);
            _data       = null;
            _tree       = null;
            _resLoaded  = false;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="recursion"></param>
        public void LoadRes(bool recursion= false)
        {
            // 加载资源
            if(!_resLoaded)
            {
                _resLoaded = true;
                if (_tree.data.Index == SceneTreeIndex.Grass )
                {
                    _resCollection.LoadGrass(_idxPrefab);
                }
                else
                {
                    _resCollection.LoadRes();
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="recursion"></param>
        public void FreeRes(bool recursion = false)
        {
            if (_tree.data.Index == SceneTreeIndex.Grass)
            {
                return;
            }

            // 已经关闭了
            if (!_resLoaded)
            {
                return;
            }

            _resLoaded = false;
            if (null != _resCollection)
            {
                _resCollection.FreeRes();
            }
        }

        /// <summary>
        /// 检查视锥
        /// </summary>
        public void CheckFrustum(Frustum frustum)
        {
            if(_resLoaded && frustum.IsInFrustum(_data.bounds))
            {
                _data.go.layer          = 0;
                _data.renderer.enabled  = true;
            }
            else
            {
                _data.go.layer          = LayerDefine.Invisible;
                _data.renderer.enabled  = false;
            }
        }
    }
}