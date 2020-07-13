//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-21
// Author: LJP
// Date: 2020-05-21
// Description: 客户端运行时场景树节点
//---------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;





namespace ActClient
{
    public class SceneTreeNode : IDisposable
    {
        /// <summary>
        /// 场景树
        /// </summary>
        SceneTree           _tree;

        /// <summary>
        /// 所有节点
        /// </summary>
        List<SceneTreeNodeObject> _nodeObjects = new List<SceneTreeNodeObject>();

        /// <summary>
        /// 数据
        /// </summary>
        SceneTreeNodeData   _data;

        /// <summary>
        /// 资源是否已经加载
        /// </summary>
        bool                _resLoaded;
        
        /// <summary>
        /// 开关
        /// </summary>
        bool                _enable;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(SceneTree tree, SceneTreeNodeData data)
        {
            _tree                   = tree;
            _data                   = data;
            int nCapacity           = data.objects.Length;
            _nodeObjects.Capacity   = nCapacity;
            for( int i = 0; i < nCapacity; ++i )
            {
                // 对于草四叉树来说 nCapacity 根 _data.instances.Length 是一样的
                var node            = ObjectPool.New<SceneTreeNodeObject>();
                int idxPrefab       = _data.instances.Length <= 0 ? -1 : i < _data.instances.Length ? _data.instances[i].prefabId : -1;
                node.Init(_tree, _data.objects[i], idxPrefab);
                _nodeObjects.Add(node);
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            for (int i = 0, ci = _nodeObjects.Count; i < ci; ++i)
            {
                var node = _nodeObjects[i];
                ObjectPool.Release(ref node);
            }

            _data           = null;
            _tree           = null;
            _resLoaded      = false;
            _enable         = false;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="recursion"></param>
        void LoadRes(bool recursion)
        {
            // 加载资源
            if(!_resLoaded)
            {
                _resLoaded = true;
                // 释放资源
                for (int i = 0, ci = _nodeObjects.Count; i < ci; ++i)
                {
                    _nodeObjects[i].LoadRes();
                }
            }

            // 子节点
            if(recursion)
            {
                for(int i = 0; i < _data.children.Length; ++i)
                {
                    var child = _tree.GetNode(_data.children[i]);
                    child.LoadRes(recursion);
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="recursion"></param>
        void FreeRes(bool recursion)
        {
            // 已经关闭了
            if(!_resLoaded)
            {
                return;
            }
            _resLoaded = false;

            // 释放资源
            for (int i = 0, ci = _nodeObjects.Count; i < ci; ++i)
            {
                _nodeObjects[i].FreeRes();
            }

            // 子节点
            if (recursion)
            {
                for(int i = 0; i < _data.children.Length; ++i)
                {
                    var child = _tree.GetNode(_data.children[i]);
                    child.FreeRes(recursion);
                }
            }
        }

        /// <summary>
        /// 检查资源
        /// </summary>
        public void CheckResources(Bounds bounds)
        {
            // 相交
            if (bounds.Intersects(_data.bounds))
            {
                if(MathUtil.BoundsContains(bounds, _data.bounds))
                {
                    LoadRes(true);
                }
                else
                {
                    LoadRes(false);
                    for(int i = 0; i < _data.children.Length; ++i)
                    {
                        var child = _tree.GetNode(_data.children[i]);
                        child.CheckResources(bounds);
                    }
                }
            }
            else
            {
                FreeRes(true);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Disable()
        {
            if(!_enable)
            {
                return;
            }
            _enable = false;


            for (int i = 0, ci = _nodeObjects.Count; i < ci; ++i)
            {
                _nodeObjects[i]._data.go.layer = LayerDefine.Invisible;
            }

            /// 针对 boxcollider
            int nLenght = _data.colliders.Length;
            for (int i = 0; i < _data.colliders.Length; i++)
            {
                _data.colliders[i].gameObject.layer = LayerDefine.Invisible;
                _data.colliders[i].enabled          = false;
            }

            // 子节点
            for (int i = 0; i < _data.children.Length; ++i)
            {
                var child       = _tree.GetNode(_data.children[i]);
                child.Disable();
            }
        }

        /// <summary>
        /// 检查视锥
        /// </summary>
        public void CheckFrustum(Frustum frustum)
        {
            /// 是否不是针对草节点加速一下，只有叶子节点才裁剪
            if (_resLoaded && _tree.data.Index == SceneTreeIndex.Grass)
            {
                if (_nodeObjects.Count > 0)
                {
                    if (_resLoaded && frustum.IsInFrustum(_data.bounds))
                    {
                        if (!_enable)
                        {
                            for (int i = 0, ci = _nodeObjects.Count; i < ci; ++i)
                            {
                                _nodeObjects[i]._data.go.layer      = 0;
                                if(!_nodeObjects[i]._data.renderer.enabled )
                                    _nodeObjects[i]._data.renderer.enabled = true;
                                //_nodeObjects[i].CheckFrustum(frustum);
                            }
                        }
                        _enable = true;
                    }
                    else
                    {
                        Disable();
                    }
                }
                else
                {
                    for (int i = 0; i < _data.children.Length; ++i)
                    {
                        var child = _tree.GetNode(_data.children[i]);
                        child.CheckFrustum(frustum);
                    }
                }
                return;
            }

            if (_resLoaded && frustum.IsInFrustum(_data.bounds))
            {
                if( !_enable )
                {
                    int nLenght = _data.colliders.Length;
                    for (int i = 0; i < _data.colliders.Length; i++)
                    {
                        _data.colliders[i].gameObject.layer = 0;
                        _data.colliders[i].enabled = true;
                    }
                }

                for (int i = 0, ci = _nodeObjects.Count; i < ci; ++i)
                {
                    _nodeObjects[i].CheckFrustum(frustum);
                }
                _enable = true;

                // 子节点
                for(int i = 0; i < _data.children.Length; ++i)
                {
                    var child = _tree.GetNode(_data.children[i]);
                    child.CheckFrustum(frustum);
                }
            }
            else
            {
                Disable();
            }
        }
    }
}