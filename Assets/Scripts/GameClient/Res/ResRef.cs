//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-20
// Author: LJP
// Date: 2020-05-20
// Description: 资源引用
//---------------------------------------------------------------------------------------
using UnityEngine;




namespace ActClient
{
    /// <summary>
    /// 加载完回调
    /// </summary>
    /// <param name="res"></param>
    public delegate void ResLoadCompleteDelegate(ResBase res);

    /// <summary>
    /// 资源引用
    /// </summary>
    public struct ResRef
    {
        /// <summary>
        /// 实际资源
        /// </summary>
        ResBase                 _res;

        /// <summary>
        /// 加载完成回调
        /// </summary>
        ResLoadCompleteDelegate _onComplete;

        /// <summary>
        /// 资源描述
        /// </summary>
        IResDesc                _resDesc;

        /// <summary>
        /// 空值
        /// </summary>
        public static readonly ResRef Null = new ResRef(null, null, null);

        /// <summary>
        /// 创建
        /// </summary>
        public static ResRef Create(ResBase res, ResLoadCompleteDelegate onComplete, IResDesc resDesc)
        {
            ResRef r = new ResRef(res, onComplete, resDesc);
            if (null != r._res)
            {
                r._res.IncRef();
            }
            return r;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public static int Release(ref ResRef r)
        {
            int refCount = 0;
            if (null != r._res)
            {
                refCount = r._res.Release();
            }

            if (r != null && null != r._resDesc && refCount == 0  )
            {
                r._resDesc.RemoveRes();
            }

            
            return refCount;
        }

        private ResRef(ResBase res, ResLoadCompleteDelegate onComplete, IResDesc resDesc)
        {
            _res            = res;
            _onComplete     = onComplete;
            _resDesc        = resDesc;
        }


        public void OnComplete()
        {
            if(null != _resDesc)
            {
                _resDesc.ApplyRes(_res.obj);
            }
            if(null != _onComplete)
            {
                _onComplete(_res);
            }
        }

        /// <summary>
        /// 取消回调
        /// </summary>
        public void         RemoveCallback() { _onComplete = null; _resDesc = null; }

        /// <summary>
        /// 操作符
        /// </summary>
        public static bool  operator !=(ResRef r1, ResRef r2) { return r1._res != r2._res || r1._onComplete != r2._onComplete || r1._resDesc != r2._resDesc; }

        public static bool  operator ==(ResRef r1, ResRef r2) { return r1._res == r2._res && r1._onComplete == r2._onComplete && r1._resDesc == r2._resDesc; }

        public bool         IsNull { get { return null == _res && null == _onComplete && null == _resDesc; } }

        public string       url { get { return null == _res ? null : _res.url; } }

        public ResBase      res { get { return _res; } }

        public bool         isLoadComplete { get { return !IsNull && _res.isLoaded; } }
    }
}