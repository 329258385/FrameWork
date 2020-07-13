//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 资源依赖基类
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using ActClient;
using System.Collections.Generic;
using System.IO;





namespace ActEditor
{
    public abstract class ResDepBase
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        abstract public ResType resType { get; }

        /// <summary>
        /// 收集依赖
        /// </summary>
        public static void CollectRendererDependencies(GameObject go, bool bCollectChildren, List<ResDepBase> list, GameObject refObject = null, bool bCollectLightmap = true)
        {
            // 模型
            {
                MeshFilter meshfilter = null;
                if (refObject != null)
                    meshfilter = refObject.GetComponent<MeshFilter>();
                var dep = ResDepMesh.CollectDependencies(go, bCollectChildren, meshfilter);
                if (null != dep)
                {
                    list.Add(dep);
                }
            }

            // 蒙皮
            {
                var dep = ResDepSkinnedMesh.CollectDependencies(go, bCollectChildren);
                if (null != dep)
                {
                    list.Add(dep);
                }
            }

            // 贴图
            {
                var dep = ResDepTexture.CollectDependencies(go, bCollectChildren);
                if (null != dep)
                {
                    list.Add(dep);
                }
            }

            // Lightmap
            if (bCollectLightmap)
            {
                var dep = ResDepLightMap.CollectDependencies(refObject ?? go, bCollectChildren);
                if (null != dep)
                {
                    list.Add(dep);
                }
            }
        }

        /// <summary>
        /// 收集碰撞依赖
        /// </summary>
        /// <param name="colliders"></param>
        /// <param name="list"></param>
        public static void CollectColliderDependencies(List<MeshCollider> colliders, List<ResDepBase> list)
        {
            // 碰撞
            {
                var dep = ResDepColliderMesh.CollectDependencies(colliders);
                if (null != dep)
                {
                    list.Add(dep);
                }
            }
        }

        /// <summary>
        /// VisitResDelegate
        /// </summary>
        protected delegate string VisitResDelegate(UnityEngine.Object asset);

        /// <summary>
        /// 遍历资源
        /// </summary>
        abstract protected void VisitAssets(ResDescCollector resCollector, VisitResDelegate visitor);
        
        /// <summary>
        /// 移除依赖
        /// </summary>
        abstract public void RemoveDependencies();

        /// <summary>
        /// 获取资源目录
        /// </summary>
        virtual protected string GetResDir()
        {
            return ResDefine.ResTypeName[(int)resType];
        }

        /// <summary>
        /// 获取资源名字
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetResName(UnityEngine.Object asset)
        {
            var path        = AssetDatabase.GetAssetPath(asset);
            var resName     = Path.GetFileNameWithoutExtension(path).ToLower().Replace(' ', '_');

            // 子物体
            var assetName   = asset.name.ToLower().Replace(' ', '_');
            if(resName != assetName)
            {
                resName = string.Format("{0}_{1}", resName, assetName);
            }

            return resName;
        }

        /// <summary>
        /// 收集资源
        /// </summary>
        public void CollectRes(ResDescCollector resCollector, Dictionary<string, ResExportDesc> dict)
        {
            VisitAssets(resCollector, asset => { return CollectRes(asset, dict); });
        }

        /// <summary>
        /// 收集资源
        /// </summary>
        protected string CollectRes(UnityEngine.Object asset, Dictionary<string, ResExportDesc> dictResDesc)
        {
            // 名字
            var resName = GetResName(asset);
            int n = 0;
            while(true)
            {
                // 字典里没有
                ResExportDesc resDesc;
                if(!dictResDesc.TryGetValue(resName, out resDesc))
                {
                    resDesc             = new ResExportDesc();
                    resDesc.asset       = asset;
                    resDesc.resType     = resType;
                    resDesc.resName     = resName;
                    resDesc.resDir      = GetResDir();
                    resDesc.refCount    = 1;
                    dictResDesc.Add(resName, resDesc);
                    break;
                }

                // 相同类型，同名
                if(resDesc.resType == resType)
                {
                    var path    = AssetDatabase.GetAssetPath(asset);
                    var p       = AssetDatabase.GetAssetPath(resDesc.asset);
                    if(path != p)
                    {
                        Debug.LogErrorFormat("资源\"{0}\"和\"{1}\"重名！", path, p);
                    }
                    ++resDesc.refCount;
                    break;
                }

                // 不同类型、同名
                resName = string.Format("{0}_r{1}", resName, ++n);
            }
            return resName;
        }
    }
}