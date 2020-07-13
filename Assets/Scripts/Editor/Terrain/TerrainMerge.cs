//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-12
// Author: LJP
// Date: 2020-05-12
// Description: 合并Unity地形
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ActClient;
using System.IO;

namespace ActEditor
{

    public class TerrainMergeBehaviour : MonoBehaviour
    {
        [MenuItem(ActEditorMenuItem.ExportTerrainMerge, false, ActEditorMenuItem.ExportTerrainMergePriority)]
        static void CTerrainSlicingWindow()
        {
            TerrainMergeEditor window = (TerrainMergeEditor)EditorWindow.GetWindowWithRect(typeof(TerrainMergeEditor), new Rect(0, 0, 386, 500), false, "合并地图");
            window.Show();
        }
    }


    public class TerrainMergeEditor : EditorWindow
    {
        /// <summary>
        /// 地形合并
        /// </summary>
        private TerrainMerge MergeTool = new TerrainMerge();

        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 绘制窗口布局
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        void OnGUI()
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Space(200);
            if (!Application.isPlaying)
            {
                if (GUILayout.Button("合并地图"))
                {
                    MergeTool.TerrainMergeMaker();
                    this.Close();
                }
            }
            GUILayout.EndHorizontal();
        }
    }


    class CTerrainPatch
    {
        public Terrain       ter = null;
    }


    /// -----------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 地形合并处理对象
    /// </summary>
    /// -----------------------------------------------------------------------------------------------------------
    public class TerrainMerge
    {
        /// <summary>
        /// 处理进度
        /// </summary>
        private float               _progress      = 0f;
        private float               ProgessScale   = 1f;

        /// <summary>
        /// 新的地形尺寸
        /// </summary>
        private int                 _newTerrainHeight = 0;
        private int                 _newTerrainLeght = 0;
        private int                 _newTerrainWidth = 0;
        private int                 _newAlphamapResolution = 0;
        private int                 _newBaseMapResolution = 0;

        /// <summary>
        /// 新的高度图和细节图分辨率
        /// </summary>
        private int                 _newHMapResolution;
        private int                 _newDetlResolution;
        private int                 _newresolutionPerPatch = 8;

        /// <summary>
        /// 草的密度数据
        /// </summary>
        private CTerrainPatch[,]    _TerrainPatchs;


        /// <summary>
        /// 统计地形细节信息
        /// </summary>
        private List<DetailPrototype> _listDetaill   = new List<DetailPrototype>();
        private List<TreePrototype>   _listTreeProto = new List<TreePrototype>();
        private List<SplatPrototype>  _listSplatPrototype = new List<SplatPrototype>();


        private void InitTerrainPatchs()
        {
            _listDetaill.Clear();
            _listTreeProto.Clear();
            _listSplatPrototype.Clear();
            GameObject[] terrain = GameObject.FindGameObjectsWithTag("EditorTerrain");
            if (terrain == null || terrain.Length <= 0)
                return;

            int nCount = terrain.Length;
            if ( !Mathf.IsPowerOfTwo(nCount))
            {
                return;
            }

            Terrain ter = terrain[0].GetComponent<Terrain>();
            if( ter == null )
            {
                return;
            }

            int nTerrainPatchSize   = (int)(ter.terrainData.size.x);
            int nWidth              = (int)Mathf.Sqrt(nCount);
            _TerrainPatchs          = new CTerrainPatch[nWidth, nWidth];
            for (int x = 0; x < nWidth; x++)
            {
                for (int z = 0; z < nWidth; z++)
                {
                    _TerrainPatchs[x, z]     = new CTerrainPatch();
                    _TerrainPatchs[x, z].ter = null;
                }
            }


            for( int n = 0; n < terrain.Length; n++ )
            {
                Terrain temp = terrain[n].GetComponent<Terrain>();
                if (temp == null)
                {
                    continue;
                }

                int x = (int)(terrain[n].transform.position.x / nTerrainPatchSize);
                int z = (int)(terrain[n].transform.position.z / nTerrainPatchSize);
                _TerrainPatchs[x, z].ter = temp;

                GetDetailPrototype(temp);
                GetSplatPrototype(temp);
                GettreePrototypes(temp);
            }

            /// 统计基本信息
            _newTerrainLeght        = (int)(nWidth * ter.terrainData.size.x);
            _newTerrainWidth        = (int)(nWidth * ter.terrainData.size.z);
            _newTerrainHeight       = (int)(ter.terrainData.size.y);
            _newHMapResolution      = (ter.terrainData.heightmapResolution - 1) * nWidth;
            _newAlphamapResolution  = ter.terrainData.alphamapResolution * nWidth;
            _newBaseMapResolution   = ter.terrainData.baseMapResolution * nWidth;
            _newDetlResolution      = ter.terrainData.detailResolution * nWidth;
            _newresolutionPerPatch  = ter.terrainData.detailResolutionPerPatch;
        }



        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 草自适应地形
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        public void TerrainMergeMaker( )
        {
            GameObject[] Objects = GameObject.FindGameObjectsWithTag("EditorTerrain");
            if (Objects == null || Objects.Length <= 0)
                return;

            // 清空历史记录
            string sceneName = UtilityTools.GetFileName(EditorApplication.currentScene);
            string mergeFile = "Assets/TerrainPatch/" + sceneName + "/" + sceneName + "Merge.asset";
            File.Delete(mergeFile);

            InitTerrainPatchs();
            GameObject terrainGameObject    = null;
            Terrain terrain                 = null;
            TerrainData data                = null;

            // 创建地图文件
            
            EditorUtility.DisplayProgressBar("progress", " progress terrain", 0.2f);
            AssetDatabase.CreateAsset(new TerrainData(), mergeFile);
            TerrainData terdata             = AssetDatabase.LoadAssetAtPath(mergeFile, typeof(TerrainData)) as TerrainData;
            terrainGameObject               = Terrain.CreateTerrainGameObject(terdata);
            terrainGameObject.name          = sceneName + "Merge";
            terrain                         = terrainGameObject.GetComponent<Terrain>();
            data                            = terrain.terrainData;
            data.heightmapResolution        = _newHMapResolution + 1;
            data.alphamapResolution         = _newAlphamapResolution;
            data.baseMapResolution          = _newBaseMapResolution;
            data.SetDetailResolution(_newDetlResolution, _newresolutionPerPatch);
            data.size                       = new Vector3(_newTerrainLeght, _newTerrainHeight, _newTerrainWidth);

            // 拷贝地图数据到新的地图中
            data.treePrototypes             = _listTreeProto.ToArray();
            data.detailPrototypes           = _listDetaill.ToArray();
            data.splatPrototypes            = _listSplatPrototype.ToArray();
            EditorUtility.DisplayProgressBar("progress", " progress terrain", 0.3f);

            Terrain ter = Objects[0].GetComponent<Terrain>();
            if (ter == null)
            {
                return;
            }

            // 处理地图细节转换
            int nTerrainPatchSize = (int)(ter.terrainData.size.x);
            int nWidth            = _TerrainPatchs.GetLength(0);
            int nLenght           = _TerrainPatchs.GetLength(1);
            ProgessScale          = ProgessScale / (nWidth * nLenght);
            for (int x = 0; x < nWidth; x++)
            {
                for (int z = 0; z < nLenght; z++)
                {
                    if( _TerrainPatchs[x,z].ter != null )
                    {

                        ProcessTerrainHeights(_TerrainPatchs[x, z].ter, terrain);
                        ProcessTerrainDetail(_TerrainPatchs[x, z].ter, terrain);

                        ProcessTerrainSplats(_TerrainPatchs[x, z].ter, terrain);
                        ProcessTerrainVegetation(_TerrainPatchs[x, z].ter, terrain);
                    }
                    _progress += ProgessScale;
                    EditorUtility.DisplayProgressBar("progress", " progress terrain", _progress);
                }
            }

            EditorUtility.ClearProgressBar();
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 处理草转化到大地图
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        private void ProcessTerrainDetail( Terrain src, Terrain dst )
        {
            int detaillWidth = src.terrainData.detailResolution;
            DetailPrototype[] layers = dst.terrainData.detailPrototypes;
            int layerLenght  = layers.Length;

            int LocalX       = (int)(src.transform.position.x / src.terrainData.size.x);
            int LocalZ       = (int)(src.transform.position.z / src.terrainData.size.z);
            for ( int n = 0; n < layers.Length; n++ )
            {
                int index    = GetLayerIndexByDetail( layers[n].prototype.name);
                if (index < 0)
                    continue;

                dst.terrainData.SetDetailLayer(LocalX * detaillWidth, LocalZ * detaillWidth, index, src.terrainData.GetDetailLayer(0, 0, detaillWidth, detaillWidth, n));
            }
        }


        private int GetLayerIndexByDetail( string name )
        {
            for( int i = 0; i < _listDetaill.Count; i++ )
            {
                if (_listDetaill[i].prototype.name == name)
                    return i;
            }
            return -1;
        }

        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 处理地形AlphaSplat
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        private void ProcessTerrainSplats(Terrain src, Terrain dst)
        {
            SplatPrototype[] protoTypes = src.terrainData.splatPrototypes;
            int detaillWidth    = src.terrainData.alphamapResolution;
            int detaillDst      = dst.terrainData.alphamapResolution;
            

            int LocalX          = (int)(src.transform.position.x / src.terrainData.size.x);
            int LocalZ          = (int)(src.transform.position.z / src.terrainData.size.z);

            float[,,] layers    = src.terrainData.GetAlphamaps(0, 0, detaillWidth, detaillWidth);
            float[,,] DstAlpha  = dst.terrainData.GetAlphamaps(0, 0, detaillDst, detaillDst);
            int layerLenght     = layers.GetLength(2);
            for ( int n = 0; n < layerLenght; n++ )
            {
                int index    = GetLayerIndexBySplatAlpha(protoTypes[n].texture.name);
                if (index < 0)
                    continue;

                for(int z = 0; z < detaillWidth; z++ )
                for(int x = 0; x < detaillWidth; x++ )
                {
                    DstAlpha[LocalZ * detaillWidth + x, LocalX * detaillWidth + z, index] = layers[x, z, n];
                }
            }
            dst.terrainData.SetAlphamaps(0, 0, DstAlpha);
            dst.terrainData.RefreshPrototypes();
        }


        private int GetLayerIndexBySplatAlpha(string name)
        {
            for (int i = 0; i < _listSplatPrototype.Count; i++)
            {
                if (_listSplatPrototype[i].texture.name == name)
                    return i;
            }
            return -1;
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 处理地形高度
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        private void ProcessTerrainHeights(Terrain src, Terrain dst)
        {
            int detaillWidth    = src.terrainData.heightmapResolution - 1;
            float[,] heights    = src.terrainData.GetHeights(0, 0, detaillWidth, detaillWidth);
            int LocalX          = (int)(src.transform.position.x / src.terrainData.size.x);
            int LocalZ          = (int)(src.transform.position.z / src.terrainData.size.z);
            dst.terrainData.SetHeights(LocalX * detaillWidth, LocalZ * detaillWidth, heights);
        }


        private int GetLayerIndexByTreeProto(string name)
        {
            for (int i = 0; i < _listTreeProto.Count; i++)
            {
                if (_listTreeProto[i].prefab.name == name)
                    return i;
            }
            return -1;
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 处理地形树
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        private void ProcessTerrainVegetation(Terrain src, Terrain dst)
        {
            TreePrototype[] treePrototyps = src.terrainData.treePrototypes;
            TreeInstance[]  trees         = src.terrainData.treeInstances;

            Vector3 a   = new Vector3(src.terrainData.size.x, 1, src.terrainData.size.z);
            Vector3 aa  = new Vector3(src.terrainData.size.x, 1, src.terrainData.size.z);
            Vector3 pos = src.transform.position;
            int nCount  = trees.Length;
            for ( int i = 0; i < nCount; i++ )
            {
                Vector3 b               = new Vector3(trees[i].position.x, trees[i].position.y, trees[i].position.z);
                Vector3 orig            = Vector3.Scale(a, b) + src.transform.position;

                TreeInstance tempInst   = new TreeInstance();
                tempInst.position       = orig / dst.terrainData.size.x;
                tempInst.widthScale     = trees[i].widthScale;
                tempInst.heightScale    = trees[i].heightScale;
                tempInst.color          = trees[i].color;
                tempInst.lightmapColor  = trees[i].lightmapColor;
                tempInst.prototypeIndex = GetLayerIndexByTreeProto(treePrototyps[trees[i].prototypeIndex].prefab.name);
                dst.AddTreeInstance(tempInst);
            }
        }



        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 得到地形草、树等细节描述
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        private void GettreePrototypes(Terrain terrain)
        {

            TerrainData UnityTerrainData = terrain.terrainData;
            TreePrototype[] treeProtos = UnityTerrainData.treePrototypes;
            if (treeProtos == null)
                return;

            if (treeProtos.Length <= 0)
            {
                return;
            }

            for (int n = 0; n < treeProtos.Length; n++)
            {
                if( GetLayerIndexByTreeProto(treeProtos[n].prefab.name) == -1 )
                    _listTreeProto.Add(treeProtos[n]);
            }
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 得到地形细节描述
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        private void GetDetailPrototype(Terrain terrain)
        {

            TerrainData UnityTerrainData = terrain.terrainData;
            DetailPrototype[] treeProtos = UnityTerrainData.detailPrototypes;
            if (treeProtos == null)
                return;

            if (treeProtos.Length <= 0)
            {
                return;
            }

            for (int n = 0; n < treeProtos.Length; n++)
            {
                if( GetLayerIndexByDetail(treeProtos[n].prototype.name) == -1 )
                    _listDetaill.Add(treeProtos[n]);
            }
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 得到地形细节描述
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        private void GetSplatPrototype( Terrain terrain )
        {
            TerrainData UnityTerrainData  = terrain.terrainData;
            SplatPrototype[] splatProtos  = UnityTerrainData.splatPrototypes;
            if (splatProtos == null)
                return;

            if (splatProtos.Length <= 0)
            {
                return;
            }

            for (int n = 0; n < splatProtos.Length; n++)
            {
                if( GetLayerIndexBySplatAlpha(splatProtos[n].texture.name) == -1 )
                    _listSplatPrototype.Add(splatProtos[n]);
            }
        }
    }
}