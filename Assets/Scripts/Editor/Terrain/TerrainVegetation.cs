//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-11
// Author: LJP
// Date: 2020-05-11
// Description: 地形植被到 gameobject for 四叉树管理场景对象
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using ActClient;





namespace ActEditor
{

    public class CVegetationProcess
    {
        /// <summary>
        /// 处理进度
        /// </summary>
        private float               _progress      = 0f;
        private float               ProgessScale   = 1f;

        private Transform           treeParent      = null;
        private Transform           grassParent     = null;


        /// ------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 在场景中创建所有植被的根节点
        /// </summary>
        /// ------------------------------------------------------------------------------------------------------
        public void CreateRootGameObject()
        {
            // 创建植被根目录
            if (treeParent == null)
            {
                treeParent      = new GameObject().transform;
                treeParent.name = "tree";
            }

            if (grassParent == null)
            {
                grassParent     = new GameObject().transform;
                grassParent.name = "grass";
            }
        }


        /// ------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 删除所有植被
        /// </summary>
        /// ------------------------------------------------------------------------------------------------------
        public void ClearChildrenFromRoot()
        {
            if (treeParent != null)
            {
                for (int i = 0; i < treeParent.childCount; i++)
                {
                    GameObject.DestroyImmediate(treeParent.GetChild(i).gameObject);
                }
            }

            if (grassParent != null)
            {
                for (int i = 0; i < grassParent.childCount; i++)
                {
                    GameObject.DestroyImmediate(grassParent.GetChild(i).gameObject);
                }
            }
        }


        /// ------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 植被与地形坡度对齐
        /// </summary>
        /// ------------------------------------------------------------------------------------------------------
        private Vector2 CaleObjectAlignTerrain(Vector3 center, Mesh mesh, Terrain terrain )
        {
            Bounds bound    = mesh.bounds;
            Vector3 Left    = Vector3.zero;
            Left.x          = center.x - bound.size.x / 2;
            Left.z          = center.z;
            Left.y          = terrain.SampleHeight(Left);

            Vector3 Right   = Vector3.zero;
            Vector3 Right2  = Vector3.zero;
            Right.x         = center.x + bound.size.x / 2;
            Right.z         = center.z;
            Right.y         = terrain.SampleHeight(Right);

            Vector3 forward = Vector3.zero;
            forward.x       = center.x;
            forward.z       = center.z + bound.size.z / 2;
            forward.y       = terrain.SampleHeight(forward);

            Vector3 back    = Vector3.zero;
            Vector3 back2   = Vector3.zero;
            back.x          = center.x;
            back.z          = center.z - bound.size.z / 2;
            back.y          = terrain.SampleHeight(back);

            Right2          = Right;
            Right2.y        = Left.y;
            back2           = back;
            back2.y         = forward.y;

            float fZAngle   = Vector3.Angle(Left - Right, Left - Right2);
            if (Left.y > Right.y)
                fZAngle     *= -1.0f;
            float fXAngle   = Vector3.Angle(forward - back, forward - back2);
            if(forward.y > back.y )
                fXAngle     *= -1.0f;

            return new Vector2(fXAngle, fZAngle);
        }


         /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 分割地形成多个地块patch
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        public void VegetationProcess( Terrain terrain )
        {

            TerrainData UnityTerrainData    = terrain.terrainData;
            TreePrototype[] treeProtos      = UnityTerrainData.treePrototypes;
            if (treeProtos == null)
                return;

            if (treeProtos.Length <= 0)
            {
                Debug.LogError(" scene have not grass model!!!!");
                return;
            }

            // 创建植被根目录
            var treeNames           = new HashSet<string>();
            var grassNames          = new HashSet<string>();

            EditorUtility.DisplayProgressBar("progress", " progress Vegetation", 0);
            int nCount              = UnityTerrainData.treeInstances.Length;
            _progress               = 0;
            ProgessScale            = 1f / nCount;

            TreeInstance[] Details  = UnityTerrainData.treeInstances;
            Vector3 a               = new Vector3(UnityTerrainData.size.x, UnityTerrainData.size.y, UnityTerrainData.size.z);
            float fHeight           = terrain.gameObject.transform.position.y;
            for (int i = 0; i < nCount; i++)
            {
                TreeInstance temp   = Details[i];
                GameObject perfab   = treeProtos[temp.prototypeIndex].prefab;

                bool bGrass         = false;
				if (perfab.name.StartsWith("grass_"))
                {
                    bGrass          = true;
                }
                else
                {
                    if(!perfab.name.StartsWith("tree_"))
                    {
                        Debug.LogError(string.Format("Vegetation {0} prefab name must start with \"tree_\" or \"grass\"!", perfab.name));
                        continue;
                    }
                }

                Vector3 b       = new Vector3(temp.position.x, temp.position.y, temp.position.z);
                Vector3 orig    = Vector3.Scale(a, b);
                orig.y          += fHeight;

                GameObject go   = PrefabUtility.InstantiatePrefab(perfab) as GameObject;
                Transform form  = go.transform;
                form.position   = orig;
                var cfg = go.GetComponent<EditorSceneObject>();
                if(null == cfg)
                {
                    cfg = go.AddComponent<EditorSceneObject>();
                }

                if(bGrass)
                {
                    form.parent         = grassParent;
                    grassNames.Add(perfab.name);
                    cfg.specSceneTree   = "grass";
                    cfg.usePrefab       = true;
                }
                else
                {
                    form.parent         = treeParent;
                    treeNames.Add(perfab.name);
                }

                if( bGrass )
                {
                    Mesh mesh = perfab.GetComponent<MeshFilter>().sharedMesh;
                    if (mesh == null)
                        continue;

                    MeshRenderer render = go.GetComponent<MeshRenderer>();
                    if (render != null)
                    {
                        render.receiveShadows = true;
                        render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }

                    Vector2 Euler = CaleObjectAlignTerrain(orig, mesh, terrain);
                    form.localRotation *= Quaternion.Euler(Euler.x, 0, Euler.y);
                }

                _progress += ProgessScale;
                EditorUtility.DisplayProgressBar(string.Format("Progress {0}/{1}", i + 1, nCount), perfab.name, _progress);
            }
            EditorUtility.ClearProgressBar();

            // LOG
            var sb = new StringBuilder();
            sb.Append("转换完成\n");
            sb.Append("-------------------------------------------------\n");
            sb.AppendFormat("树 种类({0}), 数量({1})\n", treeNames.Count, treeParent.childCount);
            foreach(var s in treeNames)
            {
                sb.AppendFormat("{0}\n", s);
            }
            sb.Append("-------------------------------------------------\n");
            sb.AppendFormat("草 种类({0}), 数量({1})\n", grassNames.Count, grassParent.childCount);
            foreach(var s in grassNames)
            {
                sb.AppendFormat("{0}\n", s);
            }
            sb.Append("-------------------------------------------------\n");
            Debug.Log(sb.ToString());
        }
    }
}