//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-20
// Author: LJP
// Date: 2020-05-20
// Description: 资源基类
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;





namespace ActClient
{
    /// <summary>
    /// 资源加载状态
    /// </summary>
    public enum ELoadingState
    {
        None,
        Loading,
        Loaded,
        Peek,
    }


    /// <summary>
    /// 在内存中存在的形式
    /// </summary>
    public enum EMemoreyState
    {
        Temp,           // 临时内存 [切场景释放或运行时释放]
        Resident,       // 常驻内存 [关闭客户端释放]
    }


    /// <summary>
    /// 资源基类
    /// </summary>
    abstract public class ResBase
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        protected string _assetName;

        /// <summary>
        /// 全路径
        /// </summary>
        protected string _url;

        /// <summary>
        /// 引用计数
        /// </summary>
        protected int _refCount;

        /// <summary>
        /// 加载状态
        /// </summary>
        protected ELoadingState _loadingState;

        /// <summary>
        /// 存储状态
        /// </summary>
        protected EMemoreyState _memoryState;


        /// <summary>
        /// 保持Bundle不释放，不释放的话 AB 文件可能是个集合，释放机会自己考虑
        /// </summary>
        protected bool _holdBundle;

        /// <summary>
        /// AB 文件，针对_holdBundle = true 才持有
        /// </summary>
        protected AssetBundle _assetBundle;

        /// <summary>
        /// 没有引用的时刻
        /// </summary>
        protected uint _unusedTick;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(string url, string assetName, bool holdBundle)
        {
            _url            = url;
            _assetName      = assetName;
            _holdBundle     = holdBundle;
            _refCount       = 0;
            _loadingState   = ELoadingState.None;
            _memoryState    = EMemoreyState.Temp;
        }

        /// <summary>
        /// 增加引用
        /// </summary>
        public void IncRef()
        {
            ++_refCount;
            _unusedTick = 0;
        }

        /// <summary>
        /// 释放，要不要控制自动释放？
        /// </summary>
        public int Release()
        {
            if (--_refCount <= 0)
            {
                _unusedTick = 0;
            }
            return _refCount;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy()
        {
            _url            = null;
            _refCount       = 0;
            _loadingState   = ELoadingState.None;
            _unusedTick     = 0;

            if (_assetBundle != null)
                _assetBundle.Unload(false);
            _assetBundle    = null;
            _memoryState    = EMemoreyState.Temp;
            Cleanup();
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        virtual public IEnumerator LoadAsync(WWW www) { return LoadAsync(www.assetBundle); }

        /// <summary>
        /// 异步加载
        /// </summary>
        virtual public IEnumerator LoadAsync(AssetBundle bundle) { yield return null; }

        /// <summary>
        /// 同步加载
        /// </summary>
        virtual public void Load(WWW www) { Load(www.assetBundle); }

        /// <summary>
        /// 同步加载
        /// </summary>
        virtual public void Load(AssetBundle bundle) { }

        virtual public Object Load(AssetBundle bundle, string assetName ) { return null; }

        /// <summary>
        /// 清理资源
        /// </summary>
        abstract protected void Cleanup();

        /// <summary>
        /// 设置加载状态
        /// </summary>
        public void SetLoadingState(ELoadingState s) { _loadingState = s; }

        abstract public object[] objs { get; }
        abstract public object obj { get; }
        public string url { get { return _url; } }
        public string name { get { return _assetName; } }
        public int refCount { get { return _refCount; } }
        public bool isLoaded { get { return ELoadingState.Loaded == _loadingState; } }
        public bool isLoading { get { return ELoadingState.Loading == _loadingState; } }
        public bool isPeeking { get { return ELoadingState.Peek == _loadingState; } }
        public bool holdBundle { get { return _holdBundle; } }
        public uint unuseTick { get { return _unusedTick; } }
        public EMemoreyState memoreyState { get { return _memoryState; } }
        public AssetBundle assetbundle { get { return _assetBundle; }set { _assetBundle = value; } }
    }
}