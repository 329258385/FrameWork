//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 层定义
//---------------------------------------------------------------------------------------
using UnityEngine;





namespace ActClient
{
    /// <summary>
    /// 层定义
    /// </summary>
    public class LayerDefine
    {
        // default碰撞层，默认怪物、npc等受服务器支配的对象是无法到达的层
        public static readonly int layerMask = (1 << LayerMask.NameToLayer("TerrainWalkable")) | (1 << LayerMask.NameToLayer("Water"));

        /// <summary>
        /// 相机裁剪层
        /// </summary>
        static public readonly int Default   = LayerMask.NameToLayer("Default");

        /// <summary>
        /// 隐藏层
        /// </summary>
        static public readonly int Invisible = LayerMask.NameToLayer("Invisible");

        /// <summary>
        /// 地形层
        /// </summary>
        static public readonly int Terrain   = LayerMask.NameToLayer("Terrain");

        /// <summary>
        /// 
        /// </summary>
        public static readonly int ObstanceLayerMask = (1 << LayerMask.NameToLayer("ObjMesh_Collider")) | (1 << LayerMask.NameToLayer("Water")) | (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Scene_Mesh"));

    }
}