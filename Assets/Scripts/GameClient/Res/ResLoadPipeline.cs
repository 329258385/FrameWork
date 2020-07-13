//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-21
// Author: LJP
// Date: 2020-05-21
// Description: 资源加载管道
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using System.Text;





namespace ActClient
{
    public class ResLoadPipeline
    {
        /// <summary>
        /// 资源引用
        /// </summary>
        private ResRef          _resRef = ResRef.Null;

        /// <summary>
        /// 加载器
        /// </summary>
        private ResLoader       _loader;

        /// <summary>
        /// WWW
        /// </summary>
        private WWW             _www;

        public ResLoadPipeline(ResLoader loader)
        {
            _loader         = loader;
        }

        /// <summary>
        /// 开始
        /// </summary>
        public bool Start(ResRef r)
        {
            if(!_resRef.IsNull)
                return false;

            _resRef = r;
            _loader.StartCoroutine(Load());
            return true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public bool Stop(ResRef r)
        {
            if(_resRef.IsNull)
            {
                return false;
            }
            if(r != _resRef)
            { 
                return false;
            }
            
            _resRef.RemoveCallback();
            return true;
        }

        /// <summary>
        /// 加载协程
        /// 由于这里兼容了 AssetBundleManager 与 lua 层的资源管理方式，因为它对资源的生命周期管理到了 assetbundle 这层，
        /// </summary>
        private IEnumerator Load()
        {
            _resRef.res.SetLoadingState(ELoadingState.Loading);
           
            // 加载场景之前，加载一个空的场景保证释放资源
            var resScene = _resRef.res as ResScene;
            if(null != resScene && !resScene.isRoom)
            {
                UnityEngine.Profiling.Profiler.BeginSample("Release Scene");
                ResManager.DestroyUnusedResources();

                Application.LoadLevel("empty");

                var async = Resources.UnloadUnusedAssets();
                yield return async;

                GC.Collect();
                UnityEngine.Profiling.Profiler.EndSample();
            }


            _www = null;
            _www = new WWW(_resRef.url);
            yield return _www;
            if (!string.IsNullOrEmpty(_www.error))
            {
                Debug.LogErrorFormat("Load res {0} error!", _resRef.url);
                OnLoadOneComplete();
                yield break;
            }
 
            
            var bundle = _www.assetBundle;
            if (null == bundle)
            {
                Debug.LogErrorFormat("Load bundle {0} failed!", _resRef.url);
                OnLoadOneComplete();
                yield break;
            }


            if (null == resScene )
            {
                if(!_resRef.res.holdBundle)
                    _resRef.res.Load(bundle);
            }
            else
            {
                yield return _loader.StartCoroutine(_resRef.res.LoadAsync(bundle));
            }

            if (!_resRef.res.holdBundle)
            {
                bundle.Unload(false);
            }
            else
            {
                // hold, 交由上层去管理
                _resRef.res.assetbundle = bundle;
            }

            // 加入字典
            ResManager.AddRes(_resRef.res);
            _loader.RemoePoolRes(_resRef.url);
            yield return null;
            OnLoadOneComplete();
        }

        /// <summary>
        /// 加载协程
        /// 由于这里兼容了 AssetBundleManager 与 lua 层的资源管理方式，因为它对资源的生命周期管理到了 assetbundle 这层，
        /// </summary>
        private IEnumerator LoadStageObj()
        {
            AssetBundle bundle = _resRef.res.assetbundle;
            if (bundle != null)
            {
                var resScene = _resRef.res as ResScene;
                if (null == resScene)
                {
                    yield return null;
                    _resRef.res.Load(bundle);
                }
                else
                {
                    yield return _loader.StartCoroutine(_resRef.res.LoadAsync(bundle));
                }
            }

            yield return null;
            OnLoadOneComplete();
        }


        /// <summary>
        /// 加载完成
        /// </summary>
        private void OnLoadOneComplete()
        {
            try
            {
                _resRef.res.SetLoadingState(ELoadingState.Loaded);
                _resRef.OnComplete();
            }
            catch(Exception e)
            {
                Debug.LogErrorFormat("OnLoadOneComplete {0} Exception!", _resRef.url);
                Debug.LogException(e);
            }
            finally
            {
                _resRef = ResRef.Null;
                _www.Dispose();
                _www = null;
            }
        }


        public ResRef   resRef      { get { return _resRef; } }
        public bool     isLoading   { get { return !_resRef.IsNull; } }
    }
}