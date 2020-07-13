//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22
// Author: LJP
// Date: 2020-05-22
// Description: 资源集合
//---------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;





namespace ActClient
{
    public class ResCollection : IDisposable
    {
        /// <summary>
        /// 数据
        /// </summary>
        ResDescCollection   _data;

        /// <summary>
        /// 资源引用列表
        /// </summary>
        List<ResRef>        _resRefs = new List<ResRef>();

        /// <summary>
        /// 是否已经加载
        /// </summary>
        bool _isLoaded;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(ResDescCollection data)
        {
            _data = data;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            FreeRes();
            _data = null;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        public void LoadRes()
        {
            // 已经加载过了
            if(_isLoaded)
            {
                return;
            }

            // 没有数据
            if(null == _data)
            {
                return;
            }

            // Texture
            for(int i = 0; i < _data.arrTexture.Length; ++i)
            {
                var desc    = _data.arrTexture[i];
                var r       = ResManager.LoadTexture(desc);
                _resRefs.Add(r);
            }

            // Mesh
            for(int i = 0; i < _data.arrMesh.Length; ++i)
            {
                var desc    = _data.arrMesh[i];
                var r       = ResManager.LoadMesh(desc);
                _resRefs.Add(r);
            }

            // SkinnedMesh
            for(int i = 0; i < _data.arrSkinnedMesh.Length; ++i)
            {
                var desc    = _data.arrSkinnedMesh[i];
                var r       = ResManager.LoadSkinnedMesh(desc);
                _resRefs.Add(r);
            }

            // MeshCollider
            for (int i = 0; i < _data.arrColliderMesh.Length; ++i)
            {
                var desc    = _data.arrColliderMesh[i];
                var r       = ResManager.LoadMeshCollider(desc);
                _resRefs.Add(r);
            }

            _isLoaded = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void FreeRes()
        {
            // 没有加载
            if(!_isLoaded)
            {
                return;
            }

            for(int i = 0, ci = _resRefs.Count; i < ci; ++i)
            {
                var r = _resRefs[i];
                ResManager.Release(ref r);
            }
            _resRefs.Clear();
            _isLoaded = false;
        }

        /// <summary>
        /// 加载草的接口
        /// </summary>
        /// <param name="prefabID"></param>
        public void LoadGrass( int prefabID )
        {
            if(_data != null )
            {
                ResManager.LoadGrass(prefabID, _data);
            }
        }
    }
}