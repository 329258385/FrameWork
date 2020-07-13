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
    public class PrefabCollection : IDisposable
    {

        /// <summary>
        /// 原始数据
        /// </summary>
        ResDescCollection               _data;

        ///-----------------------------------------------------------------------------------------
        /// 要保证根ResDescCollection 内的数据一一对应
        /// <summary>
        /// 加载后 Mesh
        /// </summary>
        public ResRef[]            arrMesh;

        /// <summary>
        /// 加载后 SkinnedMesh
        /// </summary>
        public ResRef[]             arrSkinnedMesh;

        /// <summary>
        /// 加载后 Texture
        /// </summary>
        public ResRef[]             arrTexture;

        /// <summary>
        /// 加载后 LightMap
        /// </summary>
        public ResRef[]             arrLightMap;

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
            _data           = data;
            int nCount      = _data.arrMesh.Length;
            arrMesh         = new ResRef[nCount];

            nCount          = _data.arrSkinnedMesh.Length;
            arrSkinnedMesh  = new ResRef[nCount];

            nCount          = _data.arrTexture.Length;
            arrTexture      = new ResRef[nCount];

            nCount          = _data.arrLightMap.Length;
            arrLightMap     = new ResRef[nCount];
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
                var desc        = _data.arrTexture[i];
                var r           = ResManager.LoadTexture(desc);
                arrTexture[i]   = r;
            }

            // Mesh
            for(int i = 0; i < _data.arrMesh.Length; ++i)
            {
                var desc        = _data.arrMesh[i];
                var r           = ResManager.LoadMesh(desc);
                arrMesh[i]      = r;
            }

            // SkinnedMesh
            for(int i = 0; i < _data.arrSkinnedMesh.Length; ++i)
            {
                var desc    = _data.arrSkinnedMesh[i];
                var r       = ResManager.LoadSkinnedMesh(desc);
                arrSkinnedMesh[i]  = r;
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

            // Texture
            if(arrTexture != null )
            for (int i = 0; i < arrTexture.Length; ++i)
            {
                ResManager.Release(ref arrTexture[i]);
            }

            // Mesh
            if (arrMesh != null)
            for (int i = 0; i < arrMesh.Length; ++i)
            {
                ResManager.Release(ref arrMesh[i]);
            }

            // SkinnedMesh
            if (arrSkinnedMesh != null)
            for (int i = 0; i < arrSkinnedMesh.Length; ++i)
            {
                ResManager.Release(ref arrSkinnedMesh[i]);
            }

            _isLoaded = false;
        }

        /// <summary>
        /// 按默认加载完流程处理
        /// </summary>
        /// <param name="resCollection"></param>
        public void OnComplete(ResDescCollection resCollection )
        {
            if (!_isLoaded)
                return;

            // texture
            for (int i = 0; i < resCollection.arrTexture.Length; ++i)
            {
                var desc        = resCollection.arrTexture[i];
                if (arrTexture[i].res.isLoaded)
                    desc.ApplyRes(arrTexture[i].res.obj);
            }

            // Mesh
            for (int i = 0; i < resCollection.arrMesh.Length; ++i)
            {
                var desc        = resCollection.arrMesh[i];
                if (arrMesh[i].res.isLoaded)
                    desc.ApplyRes(arrMesh[i].res.obj);
            }

            // SkinnedMesh
            for (int i = 0; i < resCollection.arrSkinnedMesh.Length; ++i)
            {
                var desc        = resCollection.arrSkinnedMesh[i];
                if (arrSkinnedMesh[i].res.isLoaded)
                    desc.ApplyRes(arrSkinnedMesh[i].res.obj);
            }
        }
    }
}