//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-21
// Author: LJP
// Date: 2020-05-21
// Description: 客户端运行时场景树
//---------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;




namespace ActClient
{
    public class SceneTree : IDisposable
    {
        /// <summary>
        /// 检查资源时间间隔
        /// </summary>
        static readonly uint[]  CheckInterval   = new uint[(int)SceneTreeIndex.Num] { 100, 75, 50, 150, 200 };

        /// <summary>
        /// 视锥剔除检测间隔
        /// </summary>
        static readonly uint[] CheckViewTick    = new uint[(int)SceneTreeIndex.Num] { 60, 5, 10, 20, 50 };

        /// <summary>
        /// 变化小的不更新
        /// </summary>
        static readonly float[] CheckError      = new float[(int)SceneTreeObjType.Num] { 1 * 1, 1 * 1 };

        /// <summary>
        /// 检查资源时间
        /// </summary>
        uint _lastUpdateResTick;
        uint _lastUpdateViewTick;

        /// <summary>
        /// 数据
        /// </summary>
        SceneTreeData       _data;

        /// <summary>
        /// 所有节点
        /// </summary>
        List<SceneTreeNode> _nodes = new List<SceneTreeNode>();

        /// <summary>
        /// 根节点
        /// </summary>
        SceneTreeNode       _root;

        /// <summary>
        /// 可见范围
        /// </summary>
        Bounds              _viewBounds;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(SceneTreeData data)
        {
            // 重复初始化
            if(_nodes.Count > 0)
            {
                Debug.LogError("SceneTree init repeat!");
                Dispose();
            }

            _data = data;
            if(_data.nodes.Length < 1)
            {
                return;
            }

            // 可见范围
            _viewBounds         = new Bounds();
            _viewBounds.extents = new Vector3(_data.viewDistance, _data.viewDistance, _data.viewDistance);
            _nodes.Capacity     = _data.nodes.Length;
            for(int i = 0; i < _data.nodes.Length; ++i)
            {
                var node        = ObjectPool.New<SceneTreeNode>();
                node.Init(this, _data.nodes[i]);
                _nodes.Add(node);
            }

            if(_data.nodes.Length > 0)
            {
                _root = _nodes[0];
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            for(int i = 0, ci = _nodes.Count; i < ci; ++i)
            {
                var node = _nodes[i];
                ObjectPool.Release(ref node);
            }
            _nodes.Clear();
            _data = null;
            _root = null;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SceneTreeNode GetNode(int index)
        {
            return _nodes[index];
        }

        /// <summary>
        /// 检查资源
        /// </summary>
        /// <param name="center"></param>
        public void CheckResources(Vector3 center)
        {
            // 时间间隔
            if(TickManager.TickSince(_lastUpdateResTick) < CheckInterval[(int)_data.Index])
            {
                return;
            }
            _lastUpdateResTick = TickManager.tick;

            // 距离
            if (MathUtil.SqrMagnitude2D(_viewBounds.center - center) < CheckError[(int)_data.objType])
            {
                return;
            }
            _viewBounds.center = center;

            // 检查
            _root.CheckResources(_viewBounds);
        }


        /// <summary>
        /// 更新视锥，为的是错开各个树的tick
        /// </summary>
        private void CheckFrustum()
        {
            // 时间间隔
            if (TickManager.TickSince(_lastUpdateViewTick) < CheckViewTick[(int)_data.Index])
            {
                return;
            }

            _lastUpdateViewTick = TickManager.tick;
            _root.CheckFrustum(SceneController.frustum);
        }


        /// <summary>
        /// 更新可见性
        /// </summary>
        public void Update(Vector3 center)
        {
            if(null == _root)
            {
                return;
            }

            // 更新边界
            CheckResources(center);
            CheckFrustum();
        }

        /// <summary>
        /// getters
        /// </summary>
        public SceneTreeData data { get { return _data; } }
    }
}