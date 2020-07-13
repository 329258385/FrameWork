//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 数学库
//---------------------------------------------------------------------------------------
using UnityEngine;





namespace ActClient
{
    static public class MathUtil
    {
        /// <summary>
        /// 浮点数精度
        /// MathUtil.fEpsilon在判断(a > b - MathUtil.fEpsilon)时会有问题
        /// </summary>
        public const float fEpsilon = 1.0e-6f;

        
        /// <summary>
        /// 包围盒包含
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool BoundsContains(Bounds b1, Bounds b2)
        {
            if(b1.min.x > b2.min.x) return false;
            if(b1.min.y > b2.min.y) return false;
            if(b1.min.z > b2.min.z) return false;
            if(b1.max.x < b2.max.x) return false;
            if(b1.max.y < b2.max.y) return false;
            if(b1.max.z < b2.max.z) return false;
            return true;
        }

        /// <summary>
        /// 向量2D长度平方
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float SqrMagnitude2D(this Vector3 v)
        {
            return v.x * v.x + v.z * v.z;
        }
    }
}