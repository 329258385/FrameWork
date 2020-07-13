//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-13
// Author: LJP
// Date: 2020-05-13
// Description: 场景树
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ActClient;
using System.Text;
using System;





namespace ActEditor
{

    /// <summary>
    /// 场景树
    /// </summary>
    public class SceneTree
    {
        /// <summary>
        /// 对象
        /// </summary>
        public struct SceneItem
        {
            public GameObject           go;
            public Bounds               bounds;
            public float                radius;
            public float                size;
            public int                  objTypeFlag;
            public Collider[]           colliders;
            public EditorSceneObject    cfg;
            public TreeNodeObjectDep    resDep;
        }

        /// <summary>
        /// 根节点
        /// </summary>
        SceneTreeNode                   _root;

        /// <summary>
        /// 设置
        /// </summary>
        SceneTreeSettings               _settings;

        /// <summary>
        /// 构造
        /// </summary>
        SceneTree(SceneTreeSettings     settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// 设置
        /// </summary>
        public SceneTreeSettings settings { get { return _settings; }}


        /// <summary>
        /// 检查设置
        /// </summary>
        public static bool CheckSettings(List<SceneTreeSettings> settings, out int settingsObjTypeFlag)
        {
            settingsObjTypeFlag = 0;
            if(settings.Count < 1)
            {
                Debug.LogError("Empty SceneTreeSettings!");
                return false;
            }

            // 只留一个碰撞器树
            bool hasColliderSetting = false;
            bool hasGrassSetting = false;
            for(int i = 0, ci = settings.Count; i < ci; ++i)
            {
                var setting = settings[i];
                var objType = setting.objType;
                settingsObjTypeFlag |= (1 << (int)objType);
                if(objType == SceneTreeObjType.Collider)
                {
                    if(hasColliderSetting)
                    {
                        settings.RemoveAt(i--);
                        --ci;
                        continue;
                    }
                    hasColliderSetting = true;
                }
                if(setting.name == "grass")
                {
                    hasGrassSetting = true;
                }
            }

            // 没有碰撞设置
            if(!hasColliderSetting)
            {
                Debug.LogError("Must Add a Collider Tree!");
                return false;
            }

            // 没有草设置
            if(!hasGrassSetting)
            {
                Debug.LogError("Must Add a Tree of name \"grass\"!");
                return false;
            }

            // 排序
            settings.Sort((s1, s2) => 
            { 
                if(s1.objType != s2.objType)
                {
                    return (int)s2.objType - (int)s1.objType;
                }
                if(s1.name == "grass")
                {
                    return -1;
                }
                if(s2.name == "grass")
                {
                    return 1;
                }
                return s1.maxItemBoundsSize < s2.maxItemBoundsSize ? -1 : 1; 
            });

            return true;
        }


        
        /// <summary>
        /// 生成
        /// </summary>
        public static SceneTree[] Build(List<SceneTreeSettings> settings, List<MeshCollider> meshColliders)
        {
            // 检查设置
            int settingsObjTypeFlag = 0;
            if(!CheckSettings(settings, out settingsObjTypeFlag))
            {
                return null;
            }

            // 收集GameObject并计算包围盒
            var gos             = (GameObject[])GameObject.FindObjectsOfType<GameObject>();
            var items           = new List<SceneItem>();
            Bounds sceneBounds  = new Bounds();

            // 检测自动计算场景包围盒
            bool bCalculateSceneBound = false;
            var ters            = GameObject.FindGameObjectsWithTag("Terrain");
            if( ters == null || ters.Length <= 0 )
            {
                bCalculateSceneBound = true;
            }

            var otherColliders  = new List<Collider>();
            var sb              = new StringBuilder();
            sb.AppendFormat("Start build {0} items...\n", gos.Length);
            for(int i = 0; i < gos.Length; ++i)
            {
                var go = gos[i];

                EditorUtility.DisplayProgressBar(string.Format("Build items({0}/{1})", i + 1, gos.Length), go.name, (float)(i + 1) / gos.Length);
                if(UtilityTools.IsEditorOnly(go))
                {
                    continue;
                }

                // 包围盒
                int objTypeFlag = 0;
                otherColliders.Clear();
                var bounds = GetBounds(go, otherColliders, meshColliders, out objTypeFlag);
                if(objTypeFlag == 0)
                {
                    continue;
                }

                var size = Vector3.Distance(bounds.min, bounds.max);
                if(size < MathUtil.fEpsilon)
                {
                    Debug.LogErrorFormat("Invalid bounds of {0}!", go.name);
                    continue;
                }

                /// 场景中没有表示地形，自动累计
                if(bCalculateSceneBound )
                {
                    sceneBounds.Encapsulate(bounds);
                }
                else
                {
                    // 使用地形包围盒生成场景包围盒
                    if (go.CompareTag("Terrain"))
                    {
                        go.layer = LayerDefine.Terrain;
                        if (sceneBounds.size.x < MathUtil.fEpsilon)
                        {
                            sceneBounds = bounds;
                        }
                        else
                        {
                            sceneBounds.Encapsulate(bounds);
                        }
                    }
                }
                // 没有交集
                if ((settingsObjTypeFlag & objTypeFlag) == 0)
                {
                    continue;
                }

                // 加入列表
                var item            = new SceneItem();
                item.go             = go;
                item.colliders      = otherColliders.ToArray();
                item.bounds         = bounds;
                item.radius         = size * 0.5f;
                item.size           = size;
                item.objTypeFlag    = objTypeFlag;
                item.cfg            = go.GetComponent<EditorSceneObject>();
                items.Add(item);
            }
            EditorUtility.ClearProgressBar();
            sb.Append("Build items complete.");
            Debug.Log(sb.ToString());

            sb.Length = 0;
            var meshColliderCount = meshColliders.Count;
            sb.AppendFormat("MeshCollider count is {0}", meshColliderCount);
            Debug.Log(sb.ToString());

            // 场景包围盒大小为0
            if(sceneBounds.size.x < MathUtil.fEpsilon)
            {
                Debug.LogError("Build scene bounds failed!");
                return null;
            }

            // 列表
            var itemCount = items.Count;
            var treeItems = new List<SceneItem>[settings.Count];
            for(int i = 0; i < treeItems.Length; ++i)
            {
                treeItems[i] = new List<SceneItem>();
            }

            // 把对象分别填充到每棵树的列表中
            for(int i = 0, ni = 0, ci = items.Count, cn = ci; i < ci; ++i, ++ni)
            {
                var item = items[i];
                EditorUtility.DisplayProgressBar(string.Format("Insert items to tree({0}/{1})", ni + 1, cn), item.go.name, (float)(ni + 1) / cn);
                for(int j = 0, cj = settings.Count; j < cj; ++j)
                {
                    var setting = settings[j];
                    var list    = treeItems[j];
                    if((item.objTypeFlag & (1 << (int)setting.objType)) == 0)
                    {
                        continue;
                    }

                    if(SceneTreeObjType.Renderer == setting.objType)
                    {
                        if(null != item.cfg && !string.IsNullOrEmpty(item.cfg.specSceneTree))
                        {
                            if(item.cfg.specSceneTree == setting.name)
                            {
                                list.Add(item);
                                items.RemoveAt(i--);
                                --ci;
                                break;
                            }
                            continue;
                        }

                        // 只包含指定名字对象
                        if(setting.name == "grass")
                        {
                            continue;
                        }

                        // 大小
                        if(item.size < setting.maxItemBoundsSize)
                        {
                            list.Add(item);
                            items.RemoveAt(i--);
                            --ci;
                            break;
                        }
                        continue;
                    }

                    // 碰撞体
                    if(SceneTreeObjType.Collider == setting.objType)
                    {
                        list.Add(item);
                        if(item.objTypeFlag == (1 << (int)setting.objType))
                        {
                            items.RemoveAt(i--);
                            --ci;
                        }
                        continue;
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            // 剩下的，放到最后一个节点
            if(items.Count > 0)
            {
                Debug.LogWarningFormat("{0} items can not add to trees!", items.Count);
                treeItems[settings.Count - 1].AddRange(items);
                items.Clear();
            }

            // 生成
            var trees = new List<SceneTree>();
            for(int i = 0; i < treeItems.Length; ++i)
            {
                var setting = settings[i];
                EditorUtility.DisplayProgressBar(string.Format("SplitScene({0}/{1})", i + 1, treeItems.Length), setting.name, (float)(i + 1) / treeItems.Length);

                var list = treeItems[i];
                if(list.Count < 1)
                {
                    continue;
                }
                var tree = new SceneTree(setting);
                tree.SplitScene(list, sceneBounds);
                trees.Add(tree);
            }
            EditorUtility.ClearProgressBar();

            // 打印日志
            sb.Length = 0;
            var scenePath = EditorApplication.currentScene;
            if(string.IsNullOrEmpty(scenePath))
            {
                scenePath = "Untitled";
            }
            sb.AppendFormat("Scene build complete. [{0}]\n", scenePath);
            sb.Append("---------------------------------------------------------------\n");
            sb.AppendFormat("TreeCount[{0}]\n", trees.Count);
            sb.AppendFormat("ItemCount[{0}]\n", itemCount);
            Debug.Log(sb.ToString());
            for(int i = 0; i < trees.Count; ++i)
            {
                var tree = trees[i];
                if(null != tree)
                {
                    tree.LogTreeInfo();
                }
            }

            return trees.ToArray();
        }

        /// <summary>
        /// 导出
        /// </summary>
        static public void Export(SceneTree[] trees, List<MeshCollider> meshColliders, bool exportRes, bool compress)
        {
            // 收集依赖
            var sb              = new StringBuilder();
            int depsCount       = 0;
            var resCollector    = new ResDescCollector();
            var dictRes         = new Dictionary<string, ResExportDesc>();

            // 光照图(不能卸载第一张光照图，否则unity_Lightmap_HDR会置为1)
            var lightMaps       = LightmapSettings.lightmaps;
            if(null != lightMaps && lightMaps.Length > 0 && null != lightMaps[0].lightmapColor)
            {
                var resName     = ResDepBase.GetResName(lightMaps[0].lightmapColor);
                resCollector.AddLightMap(resName, 0, true);
            }

            // 场景
            for (int i = 0; i < trees.Length; ++i)
            {
                var tree = trees[i];
                if(null != tree && tree.settings.objType == SceneTreeObjType.Renderer)
                {
                    var n = tree.CollectDeps(dictRes);
                    sb.AppendFormat("Tree {0} deps count is {1}\n", tree.settings.name, n);
                    depsCount += n;
                }
            }
            sb.AppendFormat("All tree deps count is {0}\n", depsCount);
            Debug.Log(sb.ToString());

            // 生成节点数据
            var prefabCollection = new ScenePrefabCollection();
            var treeDatas = new List<SceneTreeData>();
            for(int i = 0; i < trees.Length; ++i)
            {
                var tree = trees[i];
                if(null != tree)
                {
                    var data = tree.BuildData(prefabCollection, dictRes);
                    treeDatas.Add(data);
                }
            }
            
            // LightMap这里要置空一下，仅清空LightmapSettings.lightmaps仍然有依赖
            Lightmapping.lightingDataAsset = null;

            // 只留一盏主光源
            var lights      = Light.GetLights(LightType.Directional, 0);
            Light mainLight = null;
            if(lights.Length > 0)
            {
                mainLight   = lights[0];
                for(int i = 1; i < lights.Length; ++i)
                {
                    GameObject.DestroyImmediate(lights[i].gameObject);
                }
            }

            // MeshCollider
            var deps            = new List<ResDepBase>();
            ResDepBase.CollectColliderDependencies(meshColliders, deps);
            for(int n = 0, cn = deps.Count; n < cn; ++n)
            {
                var dep         = deps[n];
                dep.CollectRes(resCollector, dictRes);
                dep.RemoveDependencies();
            }

            // 场景描述
            var goSceneDesc     = new GameObject("scene_desc");
            SceneDesc desc      = goSceneDesc.AddComponent<SceneDesc>();
            desc.mainLight      = mainLight;
            desc.lightMapCount  = LightmapSettings.lightmaps.Length;
            desc.resCollection  = resCollector.Export();
            desc.trees          = treeDatas.ToArray();
            desc.prefabs        = prefabCollection.Export();

            // 移除编辑脚本
            var cfgs = UnityEngine.Object.FindObjectsOfType<EditorSceneObject>();
            for (int i = 0; i < cfgs.Length; ++i)
            {
                UnityEngine.Object.DestroyImmediate(cfgs[i]);
            }

            // 资源
            if (exportRes)
            {
                var sbExport    = new StringBuilder();
                sbExport.AppendFormat("Resources Count[{0}]\n", dictRes.Count);
                sbExport.Append("---------------------------------------------------------------\n");
                foreach(var pair in dictRes)
                {
                    var resDesc = pair.Value;
                    ResExportUtil.ExportRes(resDesc, compress);
                    sbExport.AppendFormat("{0}/{1} - {2}\n", resDesc.resDir, resDesc.resName, resDesc.refCount);
                }
                sbExport.Append("---------------------------------------------------------------\n");
                Debug.Log(sbExport.ToString());
            }
        }


        /// <summary>
        /// 递归特效根节点的位置
        /// </summary>
        static Vector3 RecursionParitcleRootPosition(Transform chlidren )
        {
            Vector3 pos   = chlidren.position;
            if (chlidren.parent == null)
            {
                return pos;
            }

            while( chlidren.parent != null )
            {
                Transform tf = chlidren.parent;
                pos += tf.position;

                chlidren = chlidren.parent;
            }
            return pos;
        }


        /// <summary>
        /// 获取GameObject的包围盒
        /// </summary>
        static Bounds GetBounds(GameObject go, List<Collider> otherColliders, List<MeshCollider> meshColliders, out int objTypeFlag)
        {
            objTypeFlag = 0;
            var renderers = go.GetComponents<Renderer>();
            var colliders = go.GetComponents<Collider>();

            // 空对象
            if(renderers.Length < 1 && colliders.Length < 1)
            {
                return new Bounds();
            }

            // 地形
            var terrain = go.GetComponent<Terrain>();
            if(null != terrain)
            {
                objTypeFlag |= (1 << (int)SceneTreeObjType.Renderer);
            }

            // 初始化包围盒
            Bounds bounds;
            if(renderers.Length > 0)
            {
                // 处理一下场景特效
                ParticleSystemRenderer psr = renderers[0] as ParticleSystemRenderer;
                if (psr != null)
                {
                    Vector3 center = go.transform.position;
                    bounds = new Bounds(center, Vector3.one );
                }
                else
                {
                    bounds = new Bounds();
                    bounds = renderers[0].bounds;
                }
                
                objTypeFlag |= (1 << (int)SceneTreeObjType.Renderer);
            }
            else
            {
                bounds = colliders[0].bounds;
            }

            // 遍历Renderer
            for(int i = 0; i < renderers.Length; ++i)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            // 遍历Collider
            for(int i = 0; i < colliders.Length; ++i)
            {
                var collider = colliders[i];
                bounds.Encapsulate(collider.bounds);

                var meshCollider = collider as MeshCollider;
                if(null != meshCollider)
                {
                    meshColliders.Add(meshCollider);
                }
                else
                {
                    otherColliders.Add(collider);
                    objTypeFlag |= (1 << (int)SceneTreeObjType.Collider);
                }
            }

            return bounds;
        }

        /// <summary>
        /// 划分场景
        /// </summary>
        void SplitScene(List<SceneItem> items, Bounds rootBounds)
        {
            int itemCount = items.Count;

            // 划分
            _root = new SceneTreeNode(this, 0);
            _root.Split(items, rootBounds, _settings.maxDepth);
        }

        /// <summary>
        /// 遍历节点
        /// </summary>
        void VisitNodes(Action<SceneTreeNode> visitor)
        {
            var treeNodes = new List<SceneTreeNode>();
            treeNodes.Add(_root);
            while(treeNodes.Count > 0)
            {
                // 取出来
                var node = treeNodes[0];
                treeNodes.RemoveAt(0);

                visitor(node);

                // 子节点
                var children = node.children;
                if(null != children)
                {
                    for(int i = 0; i < children.Length; ++i)
                    {
                        if(null == children[i])
                        {
                            continue;
                        }
                        treeNodes.Add(children[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 收集依赖
        /// </summary>
        public int CollectDeps(Dictionary<string, ResExportDesc> dictRes)
        {
            int sum = 0;
            VisitNodes(node =>
            {
                var n = node.CollectDeps(dictRes);
                sum += n;
            });
            return sum;
        }

        /// <summary>
        /// 生成数据
        /// </summary>
        public SceneTreeData BuildData(ScenePrefabCollection prefabCollection, Dictionary<string, ResExportDesc> dictRes)
        {
            var childrenList        = new List<int>();
            var nodeDatas           = new List<SceneTreeNodeData>();

            var data                = new SceneTreeData();
            data.name               = _settings.name;
            data.splitType          = _settings.splitType;
            data.objType            = _settings.objType;
            data.Index              = _settings.Index;
            data.viewDistance       = _settings.viewDistance;
            data.maxItemBoundsSize  = _settings.maxItemBoundsSize;
            var id                  = 0;
            VisitNodes(node => node.id = id++);

            // 生成数据
            int n = 0;
            int nodeCount = id;
            VisitNodes(node =>
            {
                EditorUtility.DisplayProgressBar(string.Format("Build data({0}/{1})", n + 1, nodeCount), node.id.ToString(), (float)(n + 1) / nodeCount);

                // 收集资源
                node.CollectRes(dictRes);

                childrenList.Clear();
                var children = node.children;
                if(null != children)
                {
                    for(int j = 0; j < children.Length; ++j)
                    {
                        var child = children[j];
                        if(null == child)
                        {
                            continue;
                        }
                        childrenList.Add(child.id);
                    }
                }

                // 数据
                var nodeData    = new SceneTreeNodeData();
                nodeData.id     = node.id;
                nodeData.bounds = node.bounds;
                if(_settings.objType == SceneTreeObjType.Collider)
                {
                    nodeData.colliders = node.ExportColliders();
                }
                else
                {
                    node.ExportObjects(data, nodeData, prefabCollection);
                }
                nodeData.children = childrenList.ToArray();
                nodeDatas.Add(nodeData);

                ++n;
            });
            EditorUtility.ClearProgressBar();
            data.nodes = nodeDatas.ToArray();

            return data;
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        void LogTreeInfo()
        {
            int depth = 0;
            int nodeCount = 0;
            int itemCount = 0;
            int maxItemPerNode = 0;
            int depthItemCount = _root.items.Length;
            int depthNodeCount = 1;
            int depthMaxItemCount = depthItemCount;
            int containsItemNodeCount = 0;
            var nodes = new List<SceneTreeNode>();
            nodes.Add(_root);
            var sbDetail = new StringBuilder();
            sbDetail.Append("Detail\n");
            sbDetail.Append("---------------------------------------------------------------\n");
            sbDetail.AppendFormat("[{0}]", _root.depth);
            sbDetail.Append("\t");
            GenNodeInfo(sbDetail, _root, 0);
            sbDetail.Append("\n");
            while(nodes.Count > 0)
            {
                var node = nodes[0];
                nodes.RemoveAt(0);

                // 深度
                if(depth <= node.depth)
                {
                    depth = node.depth + 1;
                    sbDetail.AppendFormat("Item[{0}]", depthItemCount);
                    sbDetail.AppendFormat(", Node[{0}]", depthNodeCount);
                    sbDetail.Append("\n\n");
                    depthItemCount = 0;
                    depthNodeCount = 0;
                    sbDetail.AppendFormat("[{0}]", depth);
                }

                // 对象数量
                if(maxItemPerNode < node.items.Length)
                {
                    maxItemPerNode = node.items.Length;
                }
                depthItemCount += node.items.Length;
                itemCount += node.items.Length;
                if(depthMaxItemCount < depthItemCount)
                {
                    depthMaxItemCount = depthItemCount;
                }
                if(node.items.Length > 0)
                {
                    ++containsItemNodeCount;
                }
                ++depthNodeCount;
                ++nodeCount;

                // 子节点
                var children = node.children;
                if(null != children)
                {
                    sbDetail.Append("\t");
                    for(int i = 0; i < children.Length; ++i)
                    {
                        if(null == children[i])
                        {
                            continue;
                        }
                        nodes.Add(children[i]);
                        GenNodeInfo(sbDetail, children[i], i);
                    }
                    sbDetail.Append("\n");
                }
            }

            var sb = new StringBuilder();
            sb.AppendFormat("SceneTree[{0}]\n", _settings.name);
            sb.Append("---------------------------------------------------------------\n");
            sb.AppendFormat("SplitType[{0}]\n", _settings.splitType);
            sb.AppendFormat("ItemCount[{0}]\n", itemCount);
            sb.AppendFormat("Depth[{0}]\n", depth);
            sb.AppendFormat("NodeCount[{0}]\n", nodeCount);
            sb.AppendFormat("ContainsItemNodeCount[{0}]\n", containsItemNodeCount);
            sb.AppendFormat("MaxItemPerNode[{0}]\n", maxItemPerNode);
            sb.AppendFormat("MaxItemPerDepth[{0}]\n", depthMaxItemCount);
            var size = _root.bounds.size;
            sb.AppendFormat("Bounds[{0}, {1}, {2}]\n", Math.Round(size.x, 2), Math.Round(size.y, 2), Math.Round(size.z, 2));
            sb.Append("---------------------------------------------------------------");
            Debug.Log(sb.ToString());

            sbDetail.Append("\n---------------------------------------------------------------");
            sbDetail.Append("\nDetail End");
            Debug.Log(sbDetail.ToString());
        }

        /// <summary>
        /// 生成节点信息
        /// </summary>
        void GenNodeInfo(StringBuilder sb, SceneTreeNode node, int childIndex)
        {
            sb.AppendFormat("({0}) ", node.items.Length);
        }

    }
}