//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 资源导出公共定义
//---------------------------------------------------------------------------------------
using UnityEngine;
using ActClient;
using System.Collections.Generic;




namespace ActEditor
{
    /// <summary>
    /// 导出资源描述
    /// </summary>
    public class ResExportDesc
    {
        public UnityEngine.Object   asset;
        public ResType              resType;
        public string               resName;
        public string               resDir;
        public int                  refCount;
    }

    /// <summary>
    /// 资源描述收集器
    /// </summary>
    public class ResDescCollector
    {
        /// <summary>
        /// Mesh
        /// </summary>
        List<ResDescMesh>           _meshes = new List<ResDescMesh>();

        /// <summary>
        /// SkinnedMesh
        /// </summary>
        List<ResDescSkinnedMesh>    _skinnedMeshes = new List<ResDescSkinnedMesh>();

        /// <summary>
        /// Texture
        /// </summary>
        List<ResDescTexutre>        _textures = new List<ResDescTexutre>();

        /// <summary>
        /// LightMap
        /// </summary>
        List<ResDescLightMap>       _lightMaps = new List<ResDescLightMap>();

        /// <summary>
        /// collider mesh
        /// </summary>
        List<ResDescColliderMesh>   _collider = new List<ResDescColliderMesh>();

        /// <summary>
        /// 添加Mesh
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        public void AddMesh(string name, MeshFilter filter )
        {
            var mesh             = new ResDescMesh();
            mesh.name            = name;
            mesh.filter          = filter;
            _meshes.Add(mesh);
        }

        /// <summary>
        /// 添加Collider
        /// </summary>
        public void AddColliderMesh(string name, MeshCollider collider)
        {
            var mesh            = new ResDescColliderMesh();
            mesh.name           = name;
            mesh.collider       = collider;
            _collider.Add(mesh);
        }

        /// <summary>
        /// 添加蒙皮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="renderer"></param>
        public void AddSkinnedMesh(string name, SkinnedMeshRenderer renderer)
        {
            var mesh            = new ResDescSkinnedMesh();
            mesh.name           = name;
            mesh.renderer       = renderer;
            _skinnedMeshes.Add(mesh);
        }

        /// <summary>
        /// 添加纹理
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="mat"></param>
        /// <param name="propId"></param>
        public void AddTexture(string name, Material mat, string propName)
        {
            var texture         = new ResDescTexutre();
            texture.name        = name;
            texture.mat         = mat;
            texture.propName    = propName;
            texture.propId      = 0;
            _textures.Add(texture);
        }

        /// <summary>
        /// 添加光照图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="renderer"></param>
        /// <param name="isColor"></param>
        public void AddLightMap(string name, Renderer renderer, bool isColor)
        {
            var lightMap            = new ResDescLightMap();
            lightMap.name           = name;
            lightMap.renderer       = renderer;
            lightMap.lightMapIndex  = renderer.lightmapIndex;
            lightMap.lightmapScaleOffset = renderer.lightmapScaleOffset;
            lightMap.isColor = isColor;
            _lightMaps.Add(lightMap);
        }

        /// <summary>
        /// 添加光照图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lightMapIndex"></param>
        /// <param name="isColor"></param>
        public void AddLightMap(string name, int lightMapIndex, bool isColor)
        {
            var lightMap            = new ResDescLightMap();
            lightMap.name           = name;
            lightMap.renderer       = null;
            lightMap.lightMapIndex  = lightMapIndex;
            lightMap.lightmapScaleOffset = Vector4.zero;
            lightMap.isColor        = isColor;
            _lightMaps.Add(lightMap);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public ResDescCollection Export()
        {
            var c = new ResDescCollection();
            c.arrMesh           = _meshes.ToArray();
            c.arrSkinnedMesh    = _skinnedMeshes.ToArray();
            c.arrTexture        = _textures.ToArray();
            c.arrLightMap       = _lightMaps.ToArray();
            c.arrColliderMesh   = _collider.ToArray();
            return c;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            _meshes.Clear();
            _skinnedMeshes.Clear();
            _textures.Clear();
            _lightMaps.Clear();
            _collider.Clear();
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() { return _meshes.Count > 0 || _skinnedMeshes.Count > 0 || _textures.Count > 0 || _lightMaps.Count > 0 || _collider.Count > 0; }
    }
}