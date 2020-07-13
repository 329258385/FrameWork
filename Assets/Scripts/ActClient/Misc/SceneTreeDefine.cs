//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-22  
// Author: LJP
// Date: 2020-05-22
// Description: 四叉树编辑器和客户端资源拆分
//---------------------------------------------------------------------------------------
using UnityEngine;






namespace ActClient
{
    /// <summary>
    /// 场景树划分类型
    /// </summary>
    public enum SceneTreeSplitType
    {
        Quad = 0,
        Oct = 1,
        Num
    }

    /// <summary>
    /// 场景树对象类型
    /// </summary>
    public enum SceneTreeObjType
    {
        Renderer,
        Collider,
        Num
    }


    public enum SceneTreeIndex
    {
        Collider,
        Grass,
        Near,
        Mid,
        Far,
        Num
    }


    /// <summary>
    /// 包围盒
    /// </summary>
    [System.Serializable]
    public struct AABB
    {
        public Vector3  min;
        public Vector3  max;
        public Vector3  center;
        public float    radius;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="bounds"></param>
        public AABB(Bounds bounds)
        {
            min     = bounds.min;
            max     = bounds.max;
            center  = bounds.center;
            radius  = (max - min).magnitude * 0.5f;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="center"></param>
        /// <param name="extends"></param>
        public AABB(Vector3 center, Vector3 extends)
        {
            min         = center - extends;
            max         = center + extends;
            this.center = center;
            radius      = extends.magnitude;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetMinMax(Vector3 min, Vector3 max)
        {
            this.min    = min;
            this.max    = max;
            center      = (min + max) * 0.5f;
            radius      = (max - min).magnitude * 0.5f;
        }

        /// <summary>
        /// 设置中心点
        /// </summary>
        /// <param name="center"></param>
        public void SetCenter(Vector3 center)
        {
            this.center = center;
            var extends = (max - min) * 0.5f;
            min         = center - extends;
            max         = center + extends;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="s"></param>
        public void Scale(float s)
        {
            var extends = (max - min) * 0.5f * s;
            min = center - extends;
            max = center + extends;
            radius *= s;
        }

        /// <summary>
        /// 插值
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="t"></param>
        public void LerpTo(AABB dst, float t)
        {
            min     = Vector3.Lerp(min, dst.min, t);
            max     = Vector3.Lerp(max, dst.max, t);
            center  = (min + max) * 0.5f;
            radius  = (max - min).magnitude * 0.5f;
        }

        /// <summary>
        /// 是否包含点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Vector3 p)
        {
            if(p.x < min.x) return false;
            if(p.y < min.y) return false;
            if(p.z < min.z) return false;
            if(p.x > max.x) return false;
            if(p.y > max.y) return false;
            if(p.z > max.z) return false;
            return true;
        }
    }

}
