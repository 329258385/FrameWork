//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 场景树节点
//---------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using ActClient;





namespace ActEditor
{
    /// <summary>
    /// 场景树节点
    /// </summary>
    public class SceneTreeNode
    {
        /// <summary>
        /// id
        /// </summary>
        public int          id { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        int                 _depth;

        /// <summary>
        /// 包围盒
        /// </summary>
        Bounds              _bounds;

        /// <summary>
        /// 子节点
        /// </summary>
        SceneTreeNode[]     _children;

        /// <summary>
        /// 对象列表
        /// </summary>
        SceneTree.SceneItem[] _items;

        /// <summary>
        /// 树
        /// </summary>
        SceneTree           _tree;

        /// <summary>
        /// 依赖
        /// </summary>
        List<ResDepBase>    _deps       = new List<ResDepBase>();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="depth"></param>
        public SceneTreeNode(SceneTree tree, int depth)
        {
            _tree           = tree;
            _depth          = depth;
        }

        /// <summary>
        /// 划分
        /// </summary>
        public void Split(List<SceneTree.SceneItem> items, Bounds bounds, int maxDepth)
        {
            do
            {
                // 包围盒
                var itemCount = items.Count;
                if(0 == _depth)
                {
                    _bounds = bounds;
                }
                else
                {
                    _bounds = new Bounds();
                    for(int i = 0; i < itemCount; ++i)
                    {
                        var item = items[i];
                        if(0 == i)
                        {
                            _bounds = item.bounds;
                            continue;
                        }
                        _bounds.Encapsulate(item.bounds);
                    }
                }

                // 没有对象了
                if(itemCount < 1)
                {
                    Debug.LogErrorFormat("Empty items! Tree depth is {0}", _depth);
                    break;
                }

                // 最大深度
                if(_depth >= maxDepth)
                {
                    break;
                }

                // 子节点包围盒
                var childrenBounds = GenChildrenBounds(_tree.settings.splitType);
                if(null == childrenBounds)
                {
                    break;
                }

                // 尝试放入子节点
                var its     = new List<SceneTree.SceneItem>();
                for(int i = 0; i < childrenBounds.Length; ++i)
                {
                    its.Clear();
                    var childBounds = childrenBounds[i];
                    for(int j = 0, cj = items.Count; j < cj; ++j)
                    {
                        var item = items[j];
                        if(MathUtil.BoundsContains(childBounds, item.bounds))
                        {
                            its.Add(item);
                            items.RemoveAt(j--);
                            --cj;
                        }
                    }
                    if(its.Count > 0)
                    {
                        if(null == _children)
                        {
                            _children = new SceneTreeNode[childrenBounds.Length];
                        }
                        _children[i] = new SceneTreeNode(_tree, depth + 1);
                        _children[i].Split(its, childBounds, maxDepth);
                    }
                }
            }
            while(false);

            // 剩下的，放到自己里面
            _items = items.ToArray();
        }

        /// <summary>
        /// 生成子节点包围盒
        /// </summary>
        Bounds[] GenChildrenBounds(SceneTreeSplitType splitType)
        {
            var size        = _bounds.size * (0.5f * 1.25f);
            var half        = _bounds.size * 0.25f;
            var mc          = _bounds.center;
            Bounds[] bounds = null;

            // 四叉
            if(SceneTreeSplitType.Quad == splitType)
            {
                bounds      = new Bounds[4];
                size.y      = _bounds.size.y + 1.0f;

                // 左上
                var center  = new Vector3(mc.x - half.x, mc.y, mc.z + half.z);
                bounds[0]   = new Bounds(center, size);

                // 右上
                center      = new Vector3(mc.x + half.x, mc.y, mc.z + half.z);
                bounds[1]   = new Bounds(center, size);

                // 左下
                center      = new Vector3(mc.x - half.x, mc.y, mc.z - half.z);
                bounds[2]   = new Bounds(center, size);

                // 右下
                center      = new Vector3(mc.x + half.x, mc.y, mc.z - half.z);
                bounds[3]   = new Bounds(center, size);
            }

            // 八叉
            else if(SceneTreeSplitType.Oct == splitType)
            {
                bounds      = new Bounds[8];

                var by      = mc.y - half.y;
                var ty      = mc.y + half.y;

                // 左上(bottom)
                var center  = new Vector3(mc.x - half.x, by, mc.z + half.z);
                bounds[0]   = new Bounds(center, size);

                // 右上(bottom)
                center      = new Vector3(mc.x + half.x, by, mc.z + half.z);
                bounds[1]   = new Bounds(center, size);

                // 左下(bottom)
                center      = new Vector3(mc.x - half.x, by, mc.z - half.z);
                bounds[2]   = new Bounds(center, size);

                // 右下(bottom)
                center      = new Vector3(mc.x + half.x, by, mc.z - half.z);
                bounds[3]   = new Bounds(center, size);

                // 左上(top)
                center      = new Vector3(mc.x - half.x, ty, mc.z + half.z);
                bounds[4]   = new Bounds(center, size);

                // 右上(top)
                center      = new Vector3(mc.x + half.x, ty, mc.z + half.z);
                bounds[5]   = new Bounds(center, size);

                // 左下(top)
                center      = new Vector3(mc.x - half.x, ty, mc.z - half.z);
                bounds[6]   = new Bounds(center, size);

                // 右下(top)
                center      = new Vector3(mc.x + half.x, ty, mc.z - half.z);
                bounds[7]   = new Bounds(center, size);
            }

            return bounds;
        }

        /// <summary>
        /// 获取子节点数量
        /// </summary>
        /// <returns></returns>
        public int GetChildrenCount()
        {
            if(null == _children)
            {
                return 0;
            }

            var count = 0;
            for(int i = 0; i < _children.Length; ++i)
            {
                if(null != _children[i])
                {
                    ++count;
                }
            }
            return count;
        }

        /// <summary>
        /// 收集依赖
        /// </summary>
        public int CollectDeps(Dictionary<string, ResExportDesc> dictRes)
        {
            // 没有对象
            if(_items.Length < 1)
            {
                return 0;
            }

            // 收集对象中的依赖
            int nDepCount = 0;
            for(int i = 0; i < _items.Length; ++i)
            {
                _deps.Clear();
                var item = _items[i];

                // 检查游戏对象, 收集go对象所依赖对象
                ResDepBase.CollectRendererDependencies(item.go, false, _deps, null);

                // 没有依赖
                if (_deps.Count <= 0)
                {
                    continue;
                }

                nDepCount += _deps.Count;

                // LOD0
                var resDep = new TreeNodeObjectDep();
                resDep.lods.Add(new TreeNodeObjectLodDep { go = item.go, deps = new List<ResDepBase>(_deps) });

                // 只有地形才导出Lightmap
                var isTerrain = item.go.tag.Equals("Terrain");
                if (null != item.cfg)
                {
                    if(null != item.cfg.lods)
                    {
                        for(int n = 0; n < item.cfg.lods.Length; n++)
                        {
                            _deps.Clear();

                            var lod = item.cfg.lods[n];
                            if(lod == null)
                                continue;

                            ResDepBase.CollectRendererDependencies(lod.go, false, _deps, item.go, isTerrain);
                            if(_deps.Count <= 0)
                            {
                                continue;
                            }
                            resDep.lods.Add(new TreeNodeObjectLodDep { go = lod.go, deps = new List<ResDepBase>(_deps) });
                            nDepCount += _deps.Count;
                        }
                    }
                }
                item.resDep = resDep;
                _items[i] = item;
            }

            return nDepCount;
        }

        /// <summary>
        /// 收集资源
        /// </summary>
        public bool CollectRes(Dictionary<string, ResExportDesc> dictRes)
        {
            // 收集依赖资源
            for( int i = 0; i < _items.Length; ++i)
            {
                var item = _items[i];
                var resDep = item.resDep;
                if(null == resDep)
                {
                    continue;
                }
                var nLods = resDep.lods.Count;
                for (int n = 0; n < nLods; ++n)
                {
                    var lod = resDep.lods[n];
                    lod.resCollector = new ResDescCollector();
                    for (int j = 0, cj = lod.deps.Count; j < cj; j++)
                    {
                        var dep = lod.deps[j];
                        dep.CollectRes(lod.resCollector, dictRes);
                        dep.RemoveDependencies();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 导出碰撞体
        /// </summary>
        public Collider[] ExportColliders()
        {
            var cos = new List<Collider>();
            for(int i = 0; i < _items.Length; ++i)
            {
                var item = _items[i];
                if(null == item.colliders)
                {
                    continue;
                }

                for(int n = 0; n < item.colliders.Length; ++n)
                {
                    var c = item.colliders[n];
                    cos.Add(c);
                    c.enabled = false;
                }
            }
            return cos.ToArray();
        }

        /// <summary>
        /// 导出对象
        /// </summary>
        public void ExportObjects(SceneTreeData treeData, SceneTreeNodeData nodeData, ScenePrefabCollection prefabCollection)
        {
            var objs            = new List<SceneTreeNodeObjectData>();
            var instances       = new List<SceneObjectInstanceData>();

            for (int i = 0; i < _items.Length; ++i)
            {
                var item        = _items[i];
                var obj         = new SceneTreeNodeObjectData();
                obj.go          = item.go;
                obj.trans       = item.go.transform;
                obj.bounds      = item.bounds;
                obj.go.layer    = LayerDefine.Invisible;
                obj.lods        = new SceneTreeNodeObjectLodData[item.resDep.lods.Count];
                for(int n = 0; n < obj.lods.Length; ++n)
                {
                    var lod      = item.resDep.lods[n];
                    var renderer = lod.go.GetComponent<Renderer>();
                    if(null == renderer)
                    {
                        Debug.LogErrorFormat("Null renderer of {0} lod {1}!", item.go.name, n);
                        continue;
                    }

                    var lodData = new SceneTreeNodeObjectLodData();
                    lodData.resCollection = lod.resCollector.Export();

                    // Cfg
                    if(null != item.cfg)
                    {
                        if(null != item.cfg.lods && n > 0 && n <= item.cfg.lods.Length)
                        {
                            var lodCfg          = item.cfg.lods[n - 1];
                            lodData.viewDist    = lodCfg.viewDist;
                            lodData.isBillboard = lodCfg.isBillboard;
                        }
                    }
                    lodData.mats = renderer.sharedMaterials;
                    obj.lods[n]  = lodData;
                    if(0 == n)
                    {
                        obj.renderer = renderer;
                        renderer.enabled = false;
                    }
                }

                // 无效对象
                if(null == obj.renderer)
                {
                    continue;
                }

                // 显示配置
                if (null != item.cfg)
                {
                    if(item.cfg.usePrefab)
                    {
                        if((item.objTypeFlag & (1 << (int)SceneTreeObjType.Collider)) != 0)
                        {
                            Debug.LogErrorFormat("Item {0} has collider, can not use prefab!", item.go.name);
                        }
                        else
                        {
                            var prefabId = prefabCollection.AddPrefab(obj);
                            if (prefabId >= 0)
                            {
                                var instance = new SceneObjectInstanceData
                                {
                                    prefabId    = prefabId,
                                };
                                instances.Add(instance);

                                // 销毁对象
                                //if (prefabCollection.GetRefCount(prefabId) > 1)
                                //{
                                //    GameObject.DestroyImmediate(obj.go);
                                //    obj.go = null;
                                //}
                            }
                        }
                    }
                }

                objs.Add(obj);
            }
            nodeData.objects    = objs.ToArray();
            nodeData.instances  = instances.ToArray();
        }

        /// <summary>
        /// get
        /// </summary>
        public int depth { get { return _depth; } }
        public SceneTree.SceneItem[] items { get { return _items; } }
        public SceneTreeNode[] children { get { return _children; } }
        public Bounds bounds { get { return _bounds; } }
    }
}