using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SerializeField]
public class DrawInstancing {

    [Serializable]
    public class Data{
        /// <summary>
        /// 摄像机
        /// </summary>
        [HideInInspector]
        public Camera mainCamera;
        /// <summary>
        /// 中心位置
        /// </summary>
        [Tooltip("生成的中心位置(这群物件的中心)")]
        public Vector3 chunkPos;
        /// <summary>
        /// 个数
        /// </summary>
        [Tooltip("这群物件的个数(1个脚本上总数不能超过1023只，超过请挂第二个脚本)")]
        [Range(0,1023)]
        public int chunkLength;
        /// <summary>
        /// LOD用MeshFilter数组
        /// </summary>
        [HideInInspector]
        public MeshFilter[] meshFilters;
        /// <summary>
        /// LOD用MeshRenderer数组
        /// </summary>
        [HideInInspector]
        public MeshRenderer[] meshRenderers;
        /// <summary>
        /// 中心与相机距离
        /// </summary>
        [HideInInspector]
        public float chunkDistance;
        /// <summary>
        /// 位移
        /// </summary>
        [HideInInspector]
        public Vector3 mPos;
        /// <summary>
        /// 缩放
        /// </summary>
        [HideInInspector]
        public Vector3 mScale;
        /// <summary>
        /// 旋转
        /// </summary>
        [HideInInspector]
        public Quaternion mRot;
        /// <summary>
        /// 矩阵列表
        /// </summary>
        [HideInInspector]
        public List<Matrix4x4> matrixs;
        /// <summary>
        /// 位移列表
        /// </summary>
        [HideInInspector]
        public List<Vector3> positions;
        /// <summary>
        /// 与相机距离列表
        /// </summary>
        [HideInInspector]
        public List<float> distances;
        /// <summary>
        /// 距离裁剪
        /// </summary>
        [HideInInspector]
        public bool disCheck;
        /// <summary>
        /// 视锥体裁剪
        /// </summary>
        [HideInInspector]
        public bool fovCheck;
        /// <summary>
        /// 屏幕尺寸范围（需要稍稍扩大）
        /// </summary>
        [HideInInspector]
        public Rect screenRect;
        /// <summary>
        /// 在屏幕内的位置
        /// </summary>
        [HideInInspector]
        public Vector3 screenPos;
        /// <summary>
        /// 自身MeshFilter
        /// </summary>
        [HideInInspector]
        public MeshFilter mf;
        /// <summary>
        /// 自身MeshRenderer
        /// </summary>
        [HideInInspector]
        public MeshRenderer mr;
        /// <summary>
        /// 最远裁剪距离
        /// </summary>
        [HideInInspector]
        public float cullDistance;
        /// <summary>
        /// 第一层LOD距离
        /// </summary>
        [HideInInspector]
        public float lod0Distance;
        /// <summary>
        /// 第二层LOD距离
        /// </summary>
        [HideInInspector]
        public float lod1Distance;
        /// <summary>
        /// 是否使用矩阵排序
        /// </summary>
        [HideInInspector]
        public bool useSort;
        /// <summary>
        /// 当前单个物体的坐标
        /// </summary>
        [HideInInspector]
        public Vector3 curPos;
        [HideInInspector]
        public float disCamera2Pos;
        [HideInInspector]
        public List<int> dis;
        [HideInInspector]
        public List<Matrix4x4> masB;

        private int count;
        private List<int> a;
        private int max;
        private int min;
        private int cLength;
        private int indexInB;
        private int index;
        private List<Matrix4x4> tempM;

        public Data()
        {
            matrixs = new List<Matrix4x4>();
            positions = new List<Vector3>();
            distances = new List<float>();
            disCheck = false;
            fovCheck = false;
        }

        public void Swap()
        {
            tempM = masB;
            masB = matrixs;
            matrixs = tempM;
        }

        /// <summary>
        /// 根据当前世界坐标判断是否裁剪
        /// </summary>
        /// <param name="curPos"></param>
        /// <returns></returns>
        public bool Cull(Vector3 curPos)
        {
            disCamera2Pos = Vector3.Distance(curPos, mainCamera.transform.position);
            disCheck = disCamera2Pos < cullDistance;
            screenPos = mainCamera.WorldToScreenPoint(curPos);
            fovCheck = screenRect.Contains(screenPos)&&screenPos.z>0;
            if (!disCheck || !fovCheck)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 两层LOD的渲染状态切换
        /// </summary>
        public void LOD2()
        {
            if (chunkDistance <= lod0Distance)
            {
                mf = meshFilters[0];
                mr = meshRenderers[0];
            }
            else
            {
                mf = meshFilters[1];
                mr = meshRenderers[1];
            }
        }

        /// <summary>
        /// 三层LOD的渲染状态切换
        /// </summary>
        public void LOD3()
        {
            if (chunkDistance <= lod0Distance)
            {
                mf = meshFilters[0];
                mr = meshRenderers[0];
            }
            else if (chunkDistance > lod0Distance && chunkDistance <= lod1Distance)
            {
                mf = meshFilters[1];
                mr = meshRenderers[1];
            }
            else
            {
                mf = meshFilters[2];
                mr = meshRenderers[2];
            }

        }

        /// <summary>
        /// 计数排序
        /// </summary>
        public void CountSort()
        {           
            count = distances.Count;
            dis.Clear();
            masB.Clear();
            for (int i = 0; i < count; i++)
            {
                dis.Add((int)(distances[i] * 10));
                masB.Add(Matrix4x4.identity);
            }
            a = dis;

            max = int.MinValue;
            min = int.MaxValue;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i]>max)
                {
                    max = a[i];
                }
                if (a[i]<min)
                {
                    min = a[i];
                }
            }
            int[] c = new int[max - min + 1];
            for (int i = 0; i < a.Count; i++)
            {
                index = a[i] - min;
                c[index]++;
            }
            cLength = c.Length;
            for (int i = 1; i < cLength; i++)
            {
                c[i] += c[i - 1];
            }
            for (int i = 0; i < a.Count; i++)
            {
                indexInB = (c[a[i] - min]--) - 1;
                masB[indexInB] = matrixs[i];
            }
            Swap();
        }
    }

    [Serializable]
    public class BirdData : Data
    {
        [Tooltip("鸟群随机生成在范围内")]
        public Vector3 flyingAreaSize = new Vector3(10, 10, 10);
        [Tooltip("鸟群飞向的终点位置")]
        public Vector3 endPos;
        [Tooltip("飞行一次时长")]
        public float totalTime;
        [HideInInspector]
        public float curTime;
        [HideInInspector]
        public Vector3 res;
        [HideInInspector]
        public Vector3 center2Birds;
        [HideInInspector]
        public Vector3 invertZScale;
        [HideInInspector]
        public List<float> birdScale;
        [HideInInspector]
        public Vector3 chunk2End;
        [HideInInspector]
        [Range(0, 1)]
        public float birdAlpha;

        public Vector3 RandomPos()
        {
            res.x = UnityEngine.Random.Range(chunkPos.x - flyingAreaSize.x * 0.5f, chunkPos.x + flyingAreaSize.x * 0.5f);
            res.y = UnityEngine.Random.Range(chunkPos.y - flyingAreaSize.y * 0.5f, chunkPos.y + flyingAreaSize.y * 0.5f);
            res.z = UnityEngine.Random.Range(chunkPos.z - flyingAreaSize.z * 0.5f, chunkPos.z + flyingAreaSize.z * 0.5f);
            return res;
        }

    }

    [Serializable]
    public class FishData : Data
    {
        [Tooltip("第一种鱼主颜色")]
        [ColorUsage(true,true)]
        public Color color1Main;
        [Tooltip("第一种鱼副颜色")]
        [ColorUsage(true, true)]
        public Color color1Sub;
        [Tooltip("第二种鱼主颜色")]
        [ColorUsage(true, true)]
        public Color color2Main;
        [Tooltip("第二种鱼副颜色")]
        [ColorUsage(true, true)]
        public Color color2Sub;
        [Tooltip("这群鱼旋转的方向(顺时针或逆时针)")]
        public Direction direction;
        [Tooltip("鱼游的速度")]
        public float swimSpeed;
        [Tooltip("鱼被玩家惊动逃跑的速度")]
        public float fleeSpeed;
        [Tooltip("鱼逃跑后alpha值渐隐的速度")]
        public float fadeSpeed;
        [Tooltip("鱼被惊动后多久开始Alpha值渐隐")]
        public float startFadeTime;
        [Tooltip("鱼整体的scale大小")]
        public float fishSize;
        //public float centerRadius;
        [Tooltip("每只鱼绕自己的中心点最小的半径(太小会原地打转)")]
        public float fishMinRadius;
        [Tooltip("每只鱼绕自己的中心点最大的半径")]
        public float fishMaxRadius;
        [Tooltip("鱼随机生成的范围(XZ方向)")]
        public Vector2 centerRect;
        //public Vector2 fishMinRect;
        //public Vector2 fishMaxRect;
        [Tooltip("鱼随机生成的高度范围")]
        public float height;
        [Tooltip("鱼群中心离玩家小于多少值会被惊动")]
        public float fleeDistance;
        [Tooltip("鱼群被惊动后玩家离开中心多远会再次刷新出现")]
        public float resetDistance;
        [HideInInspector]
        public enum FishState
        {
            normal,
            fleeing
        }
        [HideInInspector]
        public enum Direction
        {
            clockwise,
            anticlockwise
        }
        [HideInInspector]
        public FishState fishState;
        [HideInInspector][Range(0,1)]
        public float fishAlpha;
        [HideInInspector]
        public List<Vector3> oriPositions;
        [HideInInspector]
        public List<Vector3> forwards;
        [HideInInspector]
        public List<Vector3> startPositions;
        [HideInInspector]
        public float chunkTime;
        [HideInInspector]
        public float fleeFactor;
        [HideInInspector]
        public List<Vector4> fishColors1;
        [HideInInspector]
        public List<Vector4> fishColors2;
    }

    [Serializable]
    public class ButterFlyData : Data
    {
        [HideInInspector]
        public enum ButterFlyState
        {
            idle,
            flyingTo,
            endIdle,
            flyingBack,

        }
        //蝴蝶状态
        [HideInInspector]
        public ButterFlyState butterFlyState;
        [Tooltip("飞行一次时长")]        
        public float flyTime;
        [Tooltip("贝塞尔曲线控制点系数(整体)(运行时调节无效)")]
        public float bezierFactor;
        [Tooltip("贝塞尔曲线控制点Y方向参数(运行时调节无效)")]
        public float bezierYFactor;
        //起飞方向的集合
        [HideInInspector]
        public List<Vector3> randomDirections;
        //待机时翅膀倍率
        //public float idleWingSpeed;
        [HideInInspector]
        public float currentTime;
        [HideInInspector]
        public List<float> randomSinStart;
        [Tooltip("较大影响的sin函数起伏参数")]
        public float sinYFactor1;
        [Tooltip("较小影响的sin函数起伏参数")]
        public float sinYFactor2;
        [HideInInspector]
        public List<Vector3> startPos;
        [HideInInspector]
        public List<Vector3> endPos;
        [HideInInspector]
        public List<Vector3> lastPositions;
        [HideInInspector]
        public List<Quaternion> quaternions;
        [Tooltip("策划想加的离终点80%时候的Y轴起伏曲线")]
        public AnimationCurve finalCurve;
        [Tooltip("蝴蝶整体scale缩放")]
        public float scale;
        [HideInInspector]
        public List<float> randomScale;
        [HideInInspector]
        public List<Vector4> tilingOffsets;
        [Tooltip("蝴蝶群距离玩家多远会逃跑")]
        public float fleeDistance;
        //抵达的先后差
        [HideInInspector]
        public List<float> randomArriveTimes;
        [Tooltip("终点区域的旋转")]
        public float endRotate;        
        [Tooltip("终点区域大小(物体终点位置随机生成在区域内)")]
        public Vector3 endArea;
        [Tooltip("终点位置中心")]
        public Vector3 endAreaPos;
        [Tooltip("起点区域的旋转")]
        public float startRotate;
        [Tooltip("出生区域大小(物体位置随机生成在区域内)")]
        public Vector3 startArea;
    }

}
