//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-21
// Author: LJP
// Date: 2020-05-21
// Description: 资源加载控制器
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using AssetBundles;

namespace ActClient
{
    public class ResLoader : MonoBehaviour
    {
        /// <summary>
        /// 加载中的资源列表
        /// </summary>
        private LinkedList<ResRef>          _loadingResources      = new LinkedList<ResRef>();
        private Dictionary<string, ResBase> _LoadingPoolResource = new Dictionary<string, ResBase>();

        
        /// <summary>
        /// 加载管道
        /// </summary>
        private ResLoadPipeline[]   _loadPipelines = new ResLoadPipeline[10];

        /// <summary>
        /// 上次更新加载队列事件
        /// </summary>
        private uint                _lastTickLoadingTick;

        /// <summary>
        /// 开始加载时间
        /// </summary>
        private float               _startLoadTime;

        /// <summary>
        /// 是否正在需要阻塞其它管线
        /// </summary>
        private bool                _isBlocking;

        /// <summary>
        /// 加载进度
        /// </summary>
        private int                 _loadingIndex;
        private int                 _loadingCount;
        private float               _loadingProgress;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {

            TickManager.Start();
            // 创建加载管线
            for (int i = 0; i < _loadPipelines.Length; ++i)
            {
                _loadPipelines[i] = new ResLoadPipeline(this);
            }
            UtilityTools.InitResPath();
            ResManager.Init(gameObject);
        }

        /// <summary>
        /// 更新
        /// </summary>
        void Update()
        {
            TickManager.Update(Time.deltaTime);
            if (TickManager.tick - _lastTickLoadingTick > 0)
            {
                TickLoading();
                _lastTickLoadingTick = TickManager.tick;
            }
            UpdateLoadingProgress();
        }


        /// <summary>
        /// 更新加载进度
        /// </summary>
        private void UpdateLoadingProgress()
        {
            var loadingPipelineCount = 0;
            for(int i = 0; i < _loadPipelines.Length; ++i)
            {
                var p = _loadPipelines[i];
                if(p.isLoading)
                {
                    ++loadingPipelineCount;
                }
            }

            // 没有正在加载的资源
            if (loadingPipelineCount == 0)
            {
                _loadingProgress = 1.0f;
                return;
            }

            _loadingProgress = (_loadingIndex + _loadingProgress) / _loadingCount;
            _loadingProgress = Mathf.Clamp(_loadingProgress, 0.0f, 1.0f);
        }

        /// <summary>
        /// 取下一个资源
        /// </summary>
        private bool PeekNextRes(out ResRef r)
        {
            r = ResRef.Null;
            var count = _loadingResources.Count;
            if (count < 1)
            {
                if (_startLoadTime > MathUtil.fEpsilon)
                {
                    _startLoadTime = 0;
                }
                return false;
            }

            LinkedListNode<ResRef> temp = null;
            LinkedListNode<ResRef> node = _loadingResources.First;
            while(node != null )
            {
                r = node.Value;
                if (null == r.res)
                {
                    if (!IsAllPipelineIdle())
                    {
                        _isBlocking = true;
                        return false;
                    }

                    r.OnComplete();

                    temp = node;
                    node = node.Next;
                    _loadingResources.Remove(temp);
                    temp = null;
                    continue;
                }

                if (r.res.isPeeking)
                {
                    node = node.Next;
                    continue;
                }

                if (r.res.isLoading)
                {
                    node = node.Next;
                    continue;
                }

                temp    = node;
                node    = node.Next;
                _loadingResources.Remove(temp);
                temp    = null;
                ++_loadingIndex;
                if (r.res.isLoaded)
                {
                    r.OnComplete();
                    continue;
                }
                
                r.res.SetLoadingState(ELoadingState.Peek);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取空闲管线
        /// </summary>
        private ResLoadPipeline GetIdlePipeline()
        {
            for(int i = 0; i < _loadPipelines.Length; ++i)
            {
                var p = _loadPipelines[i];
                if(!p.isLoading)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// 是否所有管线都是空闲的
        /// </summary>
        private bool IsAllPipelineIdle()
        {
            for(int i = 0; i < _loadPipelines.Length; ++i)
            {
                var p = _loadPipelines[i];
                if(p.isLoading)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 处理加载队列
        /// </summary>
        private void TickLoading()
        {
            // 需要等待场景加载完再继续
            if(_isBlocking)
            {
                if(!IsAllPipelineIdle())
                {
                    return;
                }
                _isBlocking = false;
            }

            while(true)
            {
                // 取一个空闲管线
                var p = GetIdlePipeline();
                if(null == p)
                {
                    return;
                }

                ResRef r;
                do 
                {
                    if(!PeekNextRes(out r))
                    {
                        return;
                    }
                } 
                while(!p.Start(r));
                if(_isBlocking)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 查找资源
        /// </summary>
        /// <param name="url"></param>
        public ResBase FindRes(string url)
        {
            // 当前加载的资源
            for(int i = 0; i < _loadPipelines.Length; ++i)
            {
                var p = _loadPipelines[i];
                if(!p.isLoading)
                {
                    continue;
                }
                if(p.resRef.url.Equals( url ) )
                {
                    return p.resRef.res;
                }
            }

            // 在池子中，ResRef ----> ResBase 是引用关系
            ResBase r;
            if (_LoadingPoolResource.TryGetValue(url, out r ))
            {
                return r;
            }
            return null;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="r"></param>
        public void Push(ResRef r)
        {
            if(_startLoadTime < MathUtil.fEpsilon)
            {
                _startLoadTime = Time.realtimeSinceStartup;
            }

            ResBase res;
            if (!_LoadingPoolResource.TryGetValue(r.url, out res))
            {
                _LoadingPoolResource.Add(r.url, r.res );
            }
            _loadingResources.AddLast(r);
        }

        /// <summary>
        /// 移除
        /// </summary>
        public bool Remove(ResRef r)
        {
            for(int i = 0; i < _loadPipelines.Length; ++i)
            {
                if(_loadPipelines[i].Stop(r))
                {
                    return true;
                }
            }
            return _loadingResources.Remove(r);
        }

        /// <summary>
        /// 优化加载流程，单独出来一个列表
        /// </summary>
        public bool RemoePoolRes( string url )
        {
            return _LoadingPoolResource.Remove(url);
        }

        /// <summary>
        /// 重置进度
        /// </summary>
        /// <returns></returns>
        public void ResetProgress()
        {
            _loadingIndex       = 0;
            _loadingProgress    = 0;
            _loadingCount       = _loadingResources.Count;
        }

        /// <summary>
        /// 加载队列的数量
        /// </summary>
        public int loadingQueueCount { get { return _loadingResources.Count; } }
    }
}