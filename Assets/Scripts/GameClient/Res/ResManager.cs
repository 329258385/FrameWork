//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-21
// Author: LJP
// Date: 2020-05-21
// Description: 资源管理器
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using AssetBundles;

namespace ActClient
{
    public class ResManager
    {
        /// <summary>
        /// 资源缓存
        /// </summary>
        static private Dictionary<string, ResBase> _dictResources = new Dictionary<string, ResBase>();

        /// <summary>
        /// 常驻ab包：
        /// 需要手动添加公共ab包进来，常驻包不会自动卸载（即使引用计数为0），引用计数为0时可以手动卸载 
        /// </summary>
        static private HashSet<string> assetbundleResident = new HashSet<string>();

        /// <summary>
        /// ab缓存包：
        /// 所有目前已经加载的ab包，包括临时ab包与公共ab包 
        /// </summary>
        static private Dictionary<string, ResBase> assetbundleResidentCaching = new Dictionary<string, ResBase>();


        /// <summary>
        /// 检查销毁资源时间间隔
        /// </summary>
        const uint                  CheckUnusedResInterval = 1 * 1000;

        /// <summary>
        /// 销毁资源的延迟时间
        /// </summary>
        const uint                  DestroyResTickDelay = 0;//30 * 1000;

        /// <summary>
        /// 资源加载器
        /// </summary>
        static private ResLoader        _loader;

        /// <summary>
        /// 场景中树和草的引用，加速查找 add by ljp 2020-6-01
        /// </summary>
        static private PrefabCollection[]  _sceneGrass;

        /// <summary>
        /// 没有引用的资源列表
        /// </summary>
        static private List<ResBase> _unusedResources = new List<ResBase>();
        static private uint          _laskCheckUnusedResTick;

        static string               _PersistentDataPath = string.Empty;
        /// <summary>
        /// 路径转Url
        /// </summary>
        static public string PathToUrl(string name, ResType nResType )
        {
            /// 需要再处理，现在路径要统一 fixed by ljp
            // 外部目录
            //string persistentPath = string.Empty;
            //switch (nResType)
            //{
            //    case ResType.Scene:
            //        persistentPath = StringHelper.Format("file:///{0}/scene/{1}/{2}.ab", UtilityTools._persistentDataPath, name, name);
            //        break;
            //    case ResType.Mesh:
            //        persistentPath = StringHelper.Format("file:///{0}/mesh/{1}.ab", UtilityTools._persistentDataPath, name);
            //        break;
            //    case ResType.Texture:
            //        persistentPath = StringHelper.Format("file:///{0}/texture/{1}.ab", UtilityTools._persistentDataPath, name);
            //        break;
            //}
            //if(persistentPath.Length > 0 && File.Exists(persistentPath))
            //{
            //    return persistentPath;
            //}

            // 这里要针对 android 修改一下
            switch (nResType)
            {
                case ResType.Scene:
                    return StringHelper.Format("file:///{0}/scene/{1}/{2}.ab", UtilityTools._streamingAssetsPath, name, name);
                case ResType.Mesh:
                    return StringHelper.Format("file:///{0}/mesh/{1}.ab",       UtilityTools._streamingAssetsPath, name);
                case ResType.Texture:
                    return StringHelper.Format("file:///{0}/texture/{1}.ab", UtilityTools._streamingAssetsPath, name);
            }
            return "";
        }

        /// <summary>
        /// 初始化
        /// </summary>
        static public void Init(GameObject mainGameObject)
        {
            _loader = mainGameObject.GetComponent<ResLoader>();
            _PersistentDataPath = Application.persistentDataPath;
        }

        /// <summary>
        /// 优化草和树资源快速检索
        /// </summary>
        static public void InitGrass(SceneObjectPrefabData[] grass )
        {
            int nGrass  = grass.Length;
            _sceneGrass = new PrefabCollection[nGrass];
            for( int i = 0; i < nGrass; i++ )
            {
                _sceneGrass[i] = new PrefabCollection();
                _sceneGrass[i].Init(grass[i].obj.lods[0].resCollection);
                _sceneGrass[i].LoadRes();
            }
        }

        /// <summary>
        /// 优化草和树资源快速检索
        /// </summary>
        static public void DestroyGrass()
        {
            int nGrass  = _sceneGrass.Length;
            for (int i = 0; i < nGrass; i++)
            {
                _sceneGrass[i].FreeRes();
            }
            _sceneGrass = null;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        static public void Destroy()
        {
            // Loader
            if(null != _loader)
            {
                UnityEngine.Object.Destroy(_loader);
                _loader = null;
            }

            // Resources
            foreach(var pair in _dictResources)
            {
                pair.Value.Destroy();
            }
            _dictResources.Clear();
            _unusedResources.Clear();
        }

        /// <summary>
        /// 清理资源，
        /// </summary>
        static public void Clear()
        {
            // Resources
            foreach (var pair in _dictResources)
            {
                pair.Value.Destroy();
            }
            _dictResources.Clear();
            _unusedResources.Clear();
        }


        /// <summary>
        /// 更新
        /// </summary>
        static public void Update()
        {
            // 检查空闲资源
            if(TickManager.TickSince(_laskCheckUnusedResTick) > CheckUnusedResInterval)
            {
                CheckUnusedResources();
                _laskCheckUnusedResTick = TickManager.tick;
            }
        }


        /// <summary>
        /// 查找资源
        /// </summary>
        static public ResBase FindRes(string url)
        {
            // 已经加载的
            ResBase res;
            if(_dictResources.TryGetValue(url, out res))
            {
                return res;
            }

            return _loader.FindRes(url);
        }

        
        /// <summary>
        /// 加载
        /// </summary>
        static private ResRef LoadResByUrl<T>(string url, string resName, ResLoadCompleteDelegate onComplete, IResDesc resDesc, bool holdbundle = false ) where T : ResBase, new()
        {
            // 查找
            var res = FindRes(url);
            if(null == res)
            {
                res = new T();
                res.Init(url, resName, holdbundle);
            }
            else
            {
                // 从空闲列表移除
                if(res.refCount <= 0)
                {
                    _unusedResources.Remove(res);
                }

                if (res.isLoaded)
                {
                    var Ref = ResRef.Create(res, onComplete, resDesc);
                    Ref.OnComplete();
                    return Ref;
                }
            }

            var r = ResRef.Create(res, onComplete, resDesc);

            // 加入列表
            _loader.Push(r);
            return r;
        }

   
        /// <summary>
        /// 加载
        /// </summary>
        static public ResRef LoadScene(string name, ResLoadCompleteDelegate onComplete)
        {
            var url = PathToUrl(name, ResType.Scene);
            return LoadResByUrl<ResScene>(url, name, onComplete, null);
        }

        /// <summary>
        /// 加载模型
        /// </summary>
        static public ResRef LoadMesh(ResDescMesh desc)
        {
            var url = PathToUrl( desc.name, ResType.Mesh);
            return LoadResByUrl<ResMesh>(url, desc.name, null, desc);
        }


        /// <summary>
        /// 加载碰撞模型
        /// </summary>
        static public ResRef LoadMeshCollider(ResDescColliderMesh desc)
        {
            var url = PathToUrl(desc.name, ResType.Mesh);
            return LoadResByUrl<ResMesh>(url, desc.name, null, desc);
        }


        /// <summary>
        /// 加载蒙皮
        /// </summary>
        static public ResRef LoadSkinnedMesh(ResDescSkinnedMesh desc)
        {
            var url = PathToUrl( desc.name, ResType.Mesh);
            return LoadResByUrl<ResMesh>(url, desc.name, null, desc);
        }

        /// <summary>
        /// 加载贴图
        /// </summary>
        static public ResRef LoadTexture(ResDescTexutre desc)
        {
            var url = PathToUrl(desc.name, ResType.Texture);
            return LoadResByUrl<ResTexture>(url, desc.name, null, desc);
        }

        /// <summary>
        /// 优化草和树资源快速检索
        /// </summary>
        static public ResRef LoadGrass(int nGrassID, ResDescCollection desc )
        {
            int nGrass  = _sceneGrass.Length;
            if( nGrassID >= 0 && nGrassID < _sceneGrass.Length)
            {
                _sceneGrass[nGrassID].OnComplete(desc);
            }
            return ResRef.Null;
        }

        /// <summary>
        /// 加载光照图
        /// </summary>
        //static public ResRef LoadLightMap(string sceneName, ResDescLightMap desc)
        //{
        //    var url = PathToUrl(StringHelper.Format("scene/{0}/{1}.ab", sceneName, desc.name));
        //    return LoadResByUrl<ResTexture>(url, desc.name, null, desc);
        //}

        /// <summary>
        /// 加载Object，在这里不区分类型
        /// </summary>
        static public ResRef LoadObject(string name, string strkeyPath, bool bHoldBundle )
        {
            return LoadResByUrl<ResObject>(strkeyPath, name, null, null, bHoldBundle);
        }

        static public ResRef LoadPackage(string name, string strkeyPath, bool bHoldBundle)
        {
            return LoadResByUrl<ResPackage>(strkeyPath, name, null, null, bHoldBundle);
        }

        /// <summary>
        /// 释放没有引用的资源
        /// </summary>
        static public void      DestroyUnusedResources()
        {
            for(int i = 0, ci = _unusedResources.Count; i < ci; ++i)
            {
                var res = _unusedResources[i];
                _dictResources.Remove(res.url);
                res.Destroy();
            }
            _unusedResources.Clear();
        }

        /// <summary>
        /// 检查没有用的资源
        /// </summary>
        static void            CheckUnusedResources()
        {
            for(int i = 0, ci = _unusedResources.Count; i < ci; ++i)
            {
                var res = _unusedResources[i];
                if(TickManager.TickSince(res.unuseTick) > DestroyResTickDelay)
                {
                    _dictResources.Remove(res.url);
                    _unusedResources.RemoveAt(i--);
                    --ci;
                    res.Destroy();
                }
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        static public void      Release(ref ResRef r, bool bReleaseAb = false )
        {
            // 空引用
            if(r.IsNull)
            {
                return;
            }
            var res = r.res;

            // 从加载器中移除
            bool isLoading = false; //_loader.Remove(r);

            // 场景直接销毁
            if(res is ResScene)
            {
                _dictResources.Remove(r.url);
                res.Destroy();
                r = ResRef.Null;
                return;
            }

            // 释放
            ResRef.Release(ref r);

            // 加入空闲列表
            if(!isLoading && null != res && res.refCount <= 0)
            {
                if(bReleaseAb)
                {
                    _dictResources.Remove(r.url);
                    res.Destroy();
                    r = ResRef.Null;
                }
                else
                {
                    _unusedResources.Add(res);
                }
                    
            }
            r = ResRef.Null;
        }


        /// <summary>
        /// 添加资源
        /// </summary>
        static public void      AddRes(ResBase res)
        {
            _dictResources.Add(res.url, res);
            if(res.refCount <= 0)
            {
                _unusedResources.Add(res);
            }
        }

        /// <summary>
        /// 添加回调（空资源引用）
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        static public ResRef PushDelegate(ResLoadCompleteDelegate onComplete)
        {
            var r   = ResRef.Create(null, onComplete, null);
            if( r.res == null )
            {
                r.OnComplete();
            }
            else
            {
                _loader.Push(r);
            }
            // 重置进度
            _loader.ResetProgress();

            return r;
        }

        /// <summary>
        /// 资源数量
        /// </summary>
        static public int       resCount { get { return _dictResources.Count; } }

        /// <summary>
        /// 加载队列的数量
        /// </summary>
        static public int       loadingQueueCount { get { return _loader.loadingQueueCount; } }
    }
}