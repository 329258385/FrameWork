//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-09
// Author: LJP
// Date: 2020-05-09
// Description: 地表材质数据
//---------------------------------------------------------------------------------------
using UnityEngine;





namespace ActClient
{

    public class TerrainMaterialDesc : ScriptableObject
    {
        /// <summary>
        /// 每个元素占位数量
        /// </summary>
        public int bitsPerElement;

        /// <summary>
        /// 每个元素表示的世界单位大小
        /// </summary>
        public float unitPerElement;

        /// <summary>
        /// 宽
        /// </summary>
        public int width;

        /// <summary>
        /// 高
        /// </summary>
        public int height;

        /// <summary>
        /// 数据列表
        /// </summary>
        public int[] data;

        /// <summary>
        /// 水面高度
        /// </summary>
        public float waterHeight;

        /// <summary>
        /// 每个Data可存储的元素数量
        /// </summary>
        [SerializeField]
        private int _elementPerData;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(int bitsPerElement, float unitPerElement, int width, int height)
        {
            this.bitsPerElement = bitsPerElement;
            this.unitPerElement = unitPerElement;
            this.width          = width;
            this.height         = height;
            _elementPerData     = (sizeof(int) * 8 / bitsPerElement);
            if (_elementPerData < 1)
            {
                Debug.LogError("Bits over follow!");
                return;
            }
            data = new int[width * height / _elementPerData + 1];
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData(int index, int n)
        {
            if (null == data)
            {
                Debug.LogError("Null data!");
                return;
            }

            // 检查数据大小
            if (n > ((1 << bitsPerElement) - 1))
            {
                Debug.LogError("Bits over follow!");
                return;
            }

            // 检查索引
            var dataIndex = index / _elementPerData;
            if (dataIndex >= data.Length)
            {
                Debug.LogError("Data over follow!");
                return;
            }

            // 保存
            var bitsOffset = (index % _elementPerData) * bitsPerElement;
            data[dataIndex] |= (n << bitsOffset);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetData(int x, int y)
        {
            if (null == data)
            {
                Debug.LogError("Null data!");
                return -1;
            }

            if(_elementPerData <= 0)
            {
                Debug.LogError("element percent data is zero!");
                return -1;
            }

            // 索引
            var index = y * width + x;
            var dataIndex = index / _elementPerData;
            if (dataIndex >= data.Length)
            {
                Debug.LogError("Data over follow!");
                return -1;
            }

            var bitsOffset = (index % _elementPerData) * bitsPerElement;
            return (data[dataIndex] >> bitsOffset) & ((1 << bitsPerElement) - 1);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int GetData(Vector3 p)
        {
            var x = (int)(p.x / unitPerElement);
            var y = (int)(p.z / unitPerElement);
            return GetData(x, y);
        }
    }
}
