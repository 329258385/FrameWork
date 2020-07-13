//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-08
// Author: LJP
// Date: 2020-05-08
// Description: 场景描述
//---------------------------------------------------------------------------------------
using UnityEngine;





namespace ActClient
{
    /// <summary>
    /// 场景对象实例数据
    /// </summary>
    [System.Serializable]
    public class SceneObjectInstanceData
    {
        /// <summary>
        /// 预制ID
        /// </summary>
        public int prefabId;

    }


    /// <summary>
    /// 节点对象LOD配置
    /// </summary>
    [System.Serializable]
    public class SceneTreeNodeObjectLodData
    {
        /// <summary>
        /// 显示距离
        /// </summary>
        public float                viewDist;

        /// <summary>
        /// 材质
        /// </summary>
        public Material[]           mats;

        /// <summary>
        /// 资源
        /// </summary>
        public ResDescCollection    resCollection;

        /// <summary>
        /// 是否公告板
        /// </summary>
        public bool                 isBillboard;
    }

    /// <summary>
    /// 场景树节点对象
    /// </summary>
    [System.Serializable]
    public class SceneTreeNodeObjectData
    {
        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject       go;

        /// <summary>
        /// Transform
        /// </summary>
        public Transform        trans;

        /// <summary>
        /// Renderer
        /// </summary>
        public Renderer         renderer;

        /// <summary>
        /// 包围盒
        /// </summary>
        public Bounds           bounds;

        /// <summary>
        /// Lod数据
        /// </summary>
        public SceneTreeNodeObjectLodData[] lods;
    }

    /// <summary>
    /// 场景树节点数据
    /// </summary>
    [System.Serializable]
    public class SceneTreeNodeData
    {
        /// <summary>
        /// ID
        /// </summary>
        public int          id;

        /// <summary>
        /// 包围盒
        /// </summary>
        public Bounds       bounds;

        /// <summary>
        /// 碰撞体
        /// </summary>
        public Collider[]   colliders;

        /// <summary>
        /// 对象列表
        /// </summary>
        public SceneTreeNodeObjectData[] objects;

        /// <summary>
        /// 实例列表
        /// </summary>
        public SceneObjectInstanceData[] instances;

        /// <summary>
        /// 子节点
        /// </summary>
        public int[]        children;
    }

    /// <summary>
    /// 场景树数据
    /// </summary>
    [System.Serializable]
    public class SceneTreeData
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string               name;

        /// <summary>
        /// 划分类型
        /// </summary>
        public SceneTreeSplitType   splitType;

        /// <summary>
        /// 对象类型
        /// </summary>
        public SceneTreeObjType     objType;

        /// <summary>
        /// 树索引
        /// </summary>
        public SceneTreeIndex       Index;

        /// <summary>
        /// 视野距离
        /// </summary>
        public float                viewDistance;

        /// <summary>
        /// 最大对象包围盒大小
        /// </summary>
        public float                maxItemBoundsSize;

        /// <summary>
        /// 节点列表(第一个节点为根节点)
        /// </summary>
        public SceneTreeNodeData[]  nodes;
    }


    /// <summary>
    /// 场景对象预制数据
    /// </summary>
    [System.Serializable]
    public class SceneObjectPrefabData
    {
        /// <summary>
        /// 对象数据
        /// </summary>
        public SceneTreeNodeObjectData obj;

        /// <summary>
        /// 引用数量
        /// </summary>
        public int          refCountInScene;
    }

    /// <summary>
    /// 场景描述
    /// </summary>
    public class SceneDesc : MonoBehaviour
    {
        /// <summary>
        /// 主光源
        /// </summary>
        public Light                    mainLight;

        /// <summary>
        /// Lightmap数量
        /// </summary>
        public int                      lightMapCount;

        /// <summary>
        /// 场景资源描述集合
        /// </summary>
        public ResDescCollection        resCollection;


        /// <summary>
        /// 预制对象数据
        /// </summary>
        public SceneObjectPrefabData[]  prefabs;

        /// <summary>
        /// 场景树列表
        /// </summary>
        public SceneTreeData[]          trees;

        /// <summary>
        /// 当前场景
        /// </summary>
        public static SceneDesc         current { get { return Instance; } }

        /// <summary>
        /// 实例
        /// </summary>
        static SceneDesc                Instance;

        
        /// <summary>
        /// 创建
        /// </summary>
        void Awake()
        {
            if(null != Instance)
            {
                Debug.LogError("More than one SceneDesc in scene!");
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        void OnDestroy()
        {
            Instance = null;
        }
    }
}