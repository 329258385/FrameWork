//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-12
// Author: LJP
// Date: 2020-05-12
// Description: 资源公共定义，为插件导出和客户端加载，临时加框架
//---------------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;





namespace ActClient
{
    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResType
    {
        None,
        Mesh,
        Texture,
        Object,
        Animator,
        Animation,
        Audio,
        Scene,
        Num
    }

    /// <summary>
    /// 资源定义
    /// </summary>
    public static class ResDefine
    {
        public static readonly string[] ResTypeName = new string[] { "misc", "mesh", "texture", "prefab", "animator", "animation", "audio" };
    }

    /// <summary>
    /// 资源描述接口
    /// </summary>
    public interface IResDesc
    {
        void        ApplyRes(object obj);
        void        RemoveRes();
        IResDesc    Clone(GameObject cloned);
        ResType     resType { get; }
        string      resName { get; }
    }

    /// <summary>
    /// 模型信息
    /// </summary>
    [System.Serializable]
    public class ResDescMesh : IResDesc
    {
        public MeshFilter   filter;
        public string       name;

        /// <summary>
        /// 应用资源
        /// </summary>
        /// <param name="mesh"></param>
        public void ApplyRes(object mesh)
        {
            if (filter == null)
                return;
            filter.sharedMesh = (Mesh)mesh;
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public void RemoveRes()
        {
            if (filter == null)
                return;
            filter.sharedMesh = null;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="cloned"></param>
        /// <returns></returns>
        public IResDesc Clone(GameObject cloned)
        {
            var d = new ResDescMesh
            {
                name    = name,
                filter  = cloned.GetComponent<MeshFilter>()
            };
            return d;
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        public ResType resType { get { return ResType.Mesh; } }

        /// <summary>
        /// 资源名字
        /// </summary>
        public string resName { get { return name; } }
    }

    /// <summary>
    /// 碰撞模型信息
    /// </summary>
    [System.Serializable]
    public class ResDescColliderMesh : IResDesc
    {
        public string       name;
        public MeshCollider collider;

        /// <summary>
        /// 应用资源
        /// </summary>
        /// <param name="mesh"></param>
        public void ApplyRes(object mesh)
        {
            collider.sharedMesh = (Mesh)mesh;
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public void RemoveRes()
        {
            collider.sharedMesh = null;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="cloned"></param>
        /// <returns></returns>
        public IResDesc Clone(GameObject cloned)
        {
            var d = new ResDescColliderMesh
            {
                name     = name,
                collider = cloned.GetComponent<MeshCollider>()
            };
            return d;
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        public ResType resType { get { return ResType.Mesh; } }

        /// <summary>
        /// 资源名字
        /// </summary>
        public string resName { get { return name; } }
    }

    /// <summary>
    /// 蒙皮模型信息
    /// </summary>
    [System.Serializable]
    public class ResDescSkinnedMesh : IResDesc
    {
        public string               name;
        public SkinnedMeshRenderer  renderer;

        /// <summary>
        /// 应用资源
        /// </summary>
        /// <param name="mesh"></param>
        public void ApplyRes(object mesh)
        {
            renderer.sharedMesh = (Mesh)mesh;
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public void RemoveRes()
        {
            renderer.sharedMesh = null;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="cloned"></param>
        /// <returns></returns>
        public IResDesc Clone(GameObject cloned)
        {
            var d = new ResDescSkinnedMesh
            {
                name     = name,
                renderer = cloned.GetComponent<SkinnedMeshRenderer>()
            };
            return d;
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        public ResType resType { get { return ResType.Mesh; } }

        /// <summary>
        /// 资源名字
        /// </summary>
        public string resName { get { return name; } }
    }

    /// <summary>
    /// 纹理信息
    /// </summary>
    [System.Serializable]
    public class ResDescTexutre : IResDesc
    {
        public string       name;
        public Material     mat;
        public string       propName;
        public int          propId;
        //MaterialPropertyBlock prop = null;
        /// <summary>
        /// 生成属性ID
        /// </summary>
        private void CheckPropId()
        {
            if (propId > 0)
            {
                return;
            }

            //prop    = new MaterialPropertyBlock();
            propId  = Shader.PropertyToID(propName);
        }

        /// <summary>
        /// 应用资源
        /// </summary>
        /// <param name="texture"></param>
        public void ApplyRes(object texture)
        {
            CheckPropId();
            mat.SetTexture(propId, (Texture)texture);
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public void RemoveRes()
        {
            CheckPropId();
            mat.SetTexture(propId, null);
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="cloned"></param>
        /// <returns></returns>
        public IResDesc Clone(GameObject cloned)
        {
            var d = new ResDescTexutre
            {
                name        = name,
                mat         = mat,
                propName    = propName,
                propId      = propId
            };
            return d;
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        public ResType resType { get { return ResType.Texture; } }

        /// <summary>
        /// 资源名字
        /// </summary>
        public string resName { get { return name; } }
    }

    /// <summary>
    /// LightMap信息
    /// </summary>
    [System.Serializable]
    public class ResDescLightMap : IResDesc
    {
        public string       name;
        public Renderer     renderer;
        public int          lightMapIndex;
        public Vector4      lightmapScaleOffset;
        public bool         isColor;

        /// <summary>
        /// LightMaps
        /// </summary>
        static LightmapData[] _lightMaps;

        /// <summary>
        /// 创建LightMap数据
        /// </summary>
        /// <param name="lightMapCount"></param>
        static void CreateLightMapDataArray(int lightMapCount)
        {
            // 已经有了
            if (null != _lightMaps)
            {
                #if UNITY_EDITOR
                // 检查是否已经释放
                for (int i = 0; i < _lightMaps.Length; ++i)
                {
                    var data = _lightMaps[i];
                    if (null != data.lightmapColor)
                    {
                        Debug.LogErrorFormat("Lightmap {0} has not release!", data.lightmapColor.name);
                    }
                }
                #endif
                // 创建
                if (_lightMaps.Length == lightMapCount)
                {
                    return;
                }
            }

            // 创建
            _lightMaps = new LightmapData[lightMapCount];
            for (int i = 0; i < _lightMaps.Length; ++i)
            {
                _lightMaps[i] = new LightmapData();
            }
        }

        /// <summary>
        /// 初始化LightMap数据
        /// </summary>
        /// <param name="lightMapCount"></param>
        public static void InitLightMapData(int lightMapCount)
        {
            // 创建
            CreateLightMapDataArray(lightMapCount);

            // 设置到LightmapSettings
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
            LightmapSettings.lightmaps = _lightMaps;
        }

        /// <summary>
        /// 设置LightMap
        /// </summary>
        /// <param name="index"></param>
        /// <param name="lightMap"></param>
        /// <param name="isColor"></param>
        static void SetLightMap(int index, Texture2D lightMap, bool isColor)
        {
            // 没有初始化
            if (null == _lightMaps)
            {
                Debug.LogError("Null _lightMaps!");
                return;
            }

            // 索引越界
            if (index < 0 || index >= _lightMaps.Length)
            {
                Debug.LogErrorFormat("Invalid light map index of {0}!", index);
                return;
            }

            // 设置
            var data = _lightMaps[index];
            if (isColor)
            {
                if (lightMap == data.lightmapColor)
                {
                    return;
                }
                data.lightmapColor = lightMap;
            }
            else
            {
                if (lightMap == data.lightmapDir)
                {
                    return;
                }
                data.lightmapDir = lightMap;
            }

            // 设置到LightmapSettings
            LightmapSettings.lightmaps = _lightMaps;
        }

        /// <summary>
        /// 应用资源
        /// </summary>
        /// <param name="texture"></param>
        public void ApplyRes(object texture)
        {
            SetLightMap(lightMapIndex, (Texture2D)texture, isColor);
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public void RemoveRes()
        {
            SetLightMap(lightMapIndex, null, isColor);
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="cloned"></param>
        /// <returns></returns>
        public IResDesc Clone(GameObject cloned)
        {
            var d = new ResDescLightMap
            {
                name = name,
                renderer = cloned.GetComponent<Renderer>(),
                lightMapIndex = lightMapIndex,
                lightmapScaleOffset = lightmapScaleOffset,
                isColor = isColor
            };
            return d;
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        public ResType resType { get { return ResType.Texture; } }

        /// <summary>
        /// 资源名字
        /// </summary>
        public string resName { get { return name; } }
    }

    /// <summary>
    /// 资源依赖描述集合，暂时先为空
    /// </summary>
    [System.Serializable]
    public class ResDescCollection
    {
        /// <summary>
        /// Mesh
        /// </summary>
        public ResDescMesh[]        arrMesh;

        /// <summary>
        /// SkinnedMesh
        /// </summary>
        public ResDescSkinnedMesh[] arrSkinnedMesh;

        /// <summary>
        /// Texture
        /// </summary>
        public ResDescTexutre[]     arrTexture;

        /// <summary>
        /// LightMap
        /// </summary>
        public ResDescLightMap[]    arrLightMap;

        /// <summary>
        /// collider mesh
        /// </summary>
        public ResDescColliderMesh[] arrColliderMesh;

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool                 isEmpty { get { return arrMesh.Length == 0 && arrSkinnedMesh.Length == 0 && arrTexture.Length == 0 && arrLightMap.Length == 0 && arrColliderMesh.Length == 0; } }


        /// <summary>
        /// Clone
        /// </summary>
        public ResDescCollection Clone(GameObject cloned)
        {
            var data            = new ResDescCollection();
            data.arrMesh        = new ResDescMesh[arrMesh.Length];
            for (int i = 0; i < arrMesh.Length; ++i)
            {
                data.arrMesh[i] = (ResDescMesh)arrMesh[i].Clone(cloned);
            }

            data.arrSkinnedMesh = new ResDescSkinnedMesh[arrSkinnedMesh.Length];
            for (int i = 0; i < arrSkinnedMesh.Length; ++i)
            {
                data.arrSkinnedMesh[i] = (ResDescSkinnedMesh)arrSkinnedMesh[i].Clone(cloned);
            }

            data.arrTexture = new ResDescTexutre[arrTexture.Length];
            for (int i = 0; i < arrTexture.Length; ++i)
            {
                data.arrTexture[i] = (ResDescTexutre)arrTexture[i].Clone(cloned);
            }

            data.arrLightMap = new ResDescLightMap[arrLightMap.Length];
            for (int i = 0; i < arrLightMap.Length; ++i)
            {
                data.arrLightMap[i] = (ResDescLightMap)arrLightMap[i].Clone(cloned);
            }

            data.arrColliderMesh = new ResDescColliderMesh[arrColliderMesh.Length];
            for (int i = 0; i < arrColliderMesh.Length; ++i)
            {
                data.arrColliderMesh[i] = (ResDescColliderMesh)arrColliderMesh[i].Clone(cloned);
            }
            return data;
        }
    }
}