//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-06-20
// Author: LJP
// Date: 2020-06-20
// Description: 针对lua的 做的资源, 这里只存储key -------> 资源引用
//---------------------------------------------------------------------------------------
using ActClient;
using System.Collections.Generic;
using UnityEngine;






namespace AssetBundles
{
    public class AssetBundlePackage
    {
        /// <summary>
        /// 依赖资源列表
        /// </summary>
        List<string>            waitingList = new List<string>();

        /// <summary>
        /// 是否已经加载
        /// </summary>
        bool                    _isLoaded;
       
        /// <summary>
        /// Root
        /// </summary>
        ResRef                  _resRef = ResRef.Null;

        /// <summary>
        /// 依赖的所有资源集
        /// </summary>
        List<ResRef>            _resRefs = new List<ResRef>();

        /// <summary>
        /// 判断是否加载完成
        /// </summary>
        protected bool          _isMainOver         = false;
        protected bool          _isDependentOver  = false;


        public string assetbundleName
        {
            get;
            protected set;
        }


        public object AssetObject()
        {
            return _resRef.res.obj;
        }


        /// <summary>
        /// 初始话要处理的资源
        /// </summary>
        public void Init(string assetbundleName, string[] dependances)
        {
            this.assetbundleName    = assetbundleName;
            if (dependances != null && dependances.Length > 0)
            {
                for (int i = 0; i < dependances.Length; i++)
                {
                    waitingList.Add(dependances[i]);
                }
            }
        }

        /// <summary>
        /// 分步骤加载 prefab 资源
        /// </summary>
        public void Update()
        {
            // 没有加载命令返回！！！！！！！！！！！！！！！！！！
            if (!_isLoaded)
            {
                return;
            }

            if (!_isDependentOver)
            {
                _isDependentOver = IsDependentLoaded();
            }

            if (_isMainOver)
            {
                return;
            }

            _isMainOver =  _resRef.res.isLoaded;
            if( _isMainOver )
            {
                AssetBundleManager.Instance.AddresPackageBundleCache(assetbundleName, this );
            }
        }


        /// <summary>
        /// 加载 ab package 内包含的资源
        /// </summary>
        public void LoadRes()
        {
            // 已经加载过了
            if (_isLoaded)
            {
                return;
            }

            /// 加载 _resRef 依赖的资源
            for (int i = waitingList.Count - 1; i >= 0; i--)
            {
                /// 还的解析 assetname, 靠！！！
                var url         = AssetBundleUtility.GetAssetBundleFileUrl(waitingList[i]);
                var r           = ResManager.LoadObject(waitingList[i], url, true);
                _resRefs.Add(r);
            }

            {
                // 对于package 来说, resRef 可能只hold bundle, object,加载不了因为 ……
                var url         = AssetBundleUtility.GetAssetBundleFileUrl(assetbundleName);
                _resRef         = ResManager.LoadObject(assetbundleName, url, true);
            }
            _isLoaded   = true;
        }


        /// <summary>
        /// 加载bundle内的asset对象
        /// </summary>
        /// <param name="strBundleName"></param>
        private void LoadObj( )
        {
            /// 主包的
            {
                var assetNameList = AssetBundleManager.Instance.GetAssetNameList(_resRef.res.name);
                for (int n = 0; n < assetNameList.Count; n++)
                {
                    var assetName = assetNameList[n];
                    if (AssetBundleManager.Instance.IsAssetLoaded(assetName))
                    {
                        continue;
                    }

                    var assetPath = AssetBundleUtility.PackagePathToAssetsPath(assetName);
                    UnityEngine.Object obj = _resRef.res.Load(_resRef.res.assetbundle, assetPath);
                    if (obj != null)
                    {
                        AssetBundleManager.Instance.AddAssetCache(assetName, obj);
                    }

                    #if UNITY_EDITOR
                    ReBindShader(obj);
                    #endif
                }

                bool _isResident = AssetBundleManager.Instance.IsAssetBundleResident(_resRef.res.name);
                if (assetNameList != null && assetNameList.Count == 1 && _resRef.res.assetbundle != null && !_isResident)
                {
                    _resRef.res.assetbundle.Unload(false);
                    _resRef.res.assetbundle = null;
                }
            }

            /// 依赖包的
            for (int i = 0, ci = _resRefs.Count; i < ci; ++i)
            {
                var r = _resRefs[i];
                var assetNameList = AssetBundleManager.Instance.GetAssetNameList(r.res.name);
                for (int n = 0; n < assetNameList.Count; n++)
                {
                    var assetName = assetNameList[n];
                    if (AssetBundleManager.Instance.IsAssetLoaded(assetName))
                    {
                        continue;
                    }

                    var assetPath = AssetBundleUtility.PackagePathToAssetsPath(assetName);
                    UnityEngine.Object obj = r.res.Load( r.res.assetbundle, assetPath );
                    if( obj != null )
                    {
                        AssetBundleManager.Instance.AddAssetCache(assetName, obj);
                    }

                    #if UNITY_EDITOR
                    ReBindShader(obj);
                    #endif
                }

                bool _isResident = AssetBundleManager.Instance.IsAssetBundleResident(_resRef.res.name);
                if (assetNameList != null && assetNameList.Count == 1 && _resRef.res.assetbundle != null && !_isResident)
                {
                    r.res.assetbundle.Unload(false);
                    r.res.assetbundle = null;
                }
            }
        }


        /// <summary>
        /// 释放资源, 在这里不再去释放资源
        /// </summary>
        public void FreeRes()
        {
            // 没有加载
            if (!_isLoaded)
            {
                return;
            }

            ResManager.Release(ref _resRef, true );
            for (int i = 0, ci = _resRefs.Count; i < ci; ++i)
            {
                var r = _resRefs[i];
                ResManager.Release(ref r, true );
            }

            _resRefs.Clear();
            _isLoaded = false;
        }


        public void AddAssetsCache()
        {
            LoadObj();
        }


        /// <summary>
        /// 判断依赖的资源是否加载完成
        /// </summary>
        /// <returns></returns>
        public bool IsLoadOver()
        {
            if (!_resRef.res.isLoaded || !_isDependentOver )
                return false;

            return true;
        }

        /// <summary>
        /// 首先判断依赖是否加载完成
        /// </summary>
        public bool IsDependentLoaded()
        {
            for (int i = 0; i < _resRefs.Count; i++)
            {
                if (!_resRefs[i].res.isLoaded)
                    return false;
            }
            return true;
        }


        public void Dispose()
        {
            //_isLoaded       = false;
            //assetbundleName = null;
        }


        /// <summary>
        /// 在编辑器模式下加载AB,重新绑定一下shader
        /// </summary>
        private void ReBindShader(UnityEngine.Object asset)
        {
            // 说明：在Editor模拟时，Shader要重新指定
            var go = asset as GameObject;
            if (go != null)
            {
                var renderers = go.GetComponentsInChildren<Renderer>();
                for (int j = 0; j < renderers.Length; j++)
                {
                    var mat = renderers[j].sharedMaterial;
                    if (mat == null)
                    {
                        continue;
                    }

                    var shader = mat.shader;
                    if (shader != null)
                    {
                        var shaderName = shader.name;
                        mat.shader = Shader.Find(shaderName);
                    }
                }
            }
        }

    }
}
