//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-12
// Author: LJP
// Date: 2020-05-12
// Description: 地形分割
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.IO;
using ActClient;





namespace ActEditor
{
    public enum SlicingType
    {
        _2x2        = 2,
        _4x4        = 4,
        _8x8        = 8,
        _16x16      = 16,
    }

	public class TerrainSlicingEditor : MonoBehaviour
    {
		[MenuItem(ActEditorMenuItem.ExportTerrainSlicing, false, ActEditorMenuItem.ExportTerrainSlicingPriority)]
		static void CTerrainSlicingWindow ()
        {
			CTerrainSlicing window = (CTerrainSlicing)EditorWindow.GetWindowWithRect(typeof (CTerrainSlicing), new Rect(0, 0, 386, 500), false, "切分地图");
			window.Show();
		}
	}

    public class CTerrainSlicing : EditorWindow
    {
        private GameObject          selection;
        private GameObject[]        terrainGameObjects;
        private Terrain[]           terrains;
        private TerrainData[]       data;


        static public Terrain       baseTerrain;
        private TerrainData         baseData;
        private int                 resolutionPerPatch = 8;


        /// UI Layout
        private GUIContent          lable1 = null;
        private GUIContent          lable3 = null;
        private GUIContent          lable4 = null;
        private GUIContent          lable5 = null;
        private GUIContent          lable6 = null;
        private GUIContent          lable7 = null;
        private GUIContent          lable8 = null;
        private GUIContent          lable9 = null;


        static bool                 IsOverWrite  = true;
        static bool                 IsSmoothEdge = true;
        static bool                 IsCopyNegtion= true;
        static bool                 IsCopyDetail = true;
        static SlicingType          excusuibType = SlicingType._8x8;
        static private string       TerrainPatchPath = "";

        /// <summary>
        /// 新地图数据
        /// </summary>
        private int                 _newTerrainWide = 1;
        private int                 _newTerrainHeight = 1;
        private int                 _newHeightMapResolution;
        private int                 _newAlphaMapResolution;
        private int                 _newBaseMapResolution;
        private int                 _newDetailResolution;
        private float               _progress = 0f;
        private float               ProgessScale = 1f;
       
        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 初始化窗口的布局
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        void OnEnable()
        {
            if (Application.isPlaying)
                return;


            string sceneName = UtilityTools.GetFileName(EditorApplication.currentScene);
            TerrainPatchPath = "Assets/Export/TearrinPatch/" + sceneName;
            if ( !Directory.Exists(TerrainPatchPath) )
            {
                Directory.CreateDirectory( TerrainPatchPath );
            }
            
            /// 属性
            lable1              = new GUIContent("选择地图");
            lable3              = new GUIContent("分割维度");
            lable4              = new GUIContent("保存路径");
            lable5              = new GUIContent("重置路径");
            lable6              = new GUIContent("覆盖数据");
            lable7              = new GUIContent("平滑边缘");
            lable8              = new GUIContent("拷贝植被");
            lable9              = new GUIContent("拷贝细节");

            selection = Selection.activeObject as GameObject;
            if( selection != null )
            {
                baseTerrain = selection.GetComponent<Terrain>();
                resolutionPerPatch = baseTerrain.terrainData.detailResolutionPerPatch;
            }
            if (baseTerrain == null)
            {
                Debug.Log( "选择的GameOject 没有 Terrain 属性" );
                return;
            }
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 清空历史操作的数据
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        void ClearHistoryCache()
        {
            bool bRefresh       = false;
            DirectoryInfo dir   = new DirectoryInfo(TerrainPatchPath);
            FileSystemInfo[] fileList = dir.GetFileSystemInfos();
            foreach (FileSystemInfo f in fileList)
            {
                if (f is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(f.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(f.FullName);
                }
                bRefresh = true;
            }

            if(bRefresh )
                AssetDatabase.Refresh();
            return;
        }



        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 绘制窗口布局
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        void OnGUI()
        {
            if( !Application.isPlaying )
            {
                GUILayout.Label("配置选项", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                baseTerrain                 = (Terrain)EditorGUILayout.ObjectField( lable1, baseTerrain, typeof(Terrain ), false );

                EditorGUILayout.Space();
                excusuibType                = (SlicingType)EditorGUILayout.EnumPopup(lable3, excusuibType);

                EditorGUILayout.Space();
                TerrainPatchPath            = EditorGUILayout.TagField(lable4, TerrainPatchPath);

                EditorGUILayout.Space();
                if( GUILayout.Button( lable5 ) )
                {
                    string sceneName        = UtilityTools.GetFileName(EditorApplication.currentScene);
                    TerrainPatchPath        = "Assets/TerrainPatch/" + sceneName + "/TearrinData";
                }

                EditorGUILayout.Space();
                IsOverWrite                 = EditorGUILayout.Toggle(lable6, IsOverWrite);

                EditorGUILayout.Space();
                IsSmoothEdge                = EditorGUILayout.Toggle(lable7, IsSmoothEdge);

                EditorGUILayout.Space();
                IsCopyNegtion               = EditorGUILayout.Toggle(lable8, IsCopyNegtion);

                EditorGUILayout.Space();
                IsCopyDetail                = EditorGUILayout.Toggle(lable9, IsCopyDetail);

                /// 开始分割地形
                if (GUILayout.Button("开始分割地形"))
                {
                    if (baseTerrain == null )
                    {
                        this.ShowNotification(new GUIContent("base terrain is null "));
                        return;
                    }

                    ClearHistoryCache();

                    /// 计算一些参数
                    baseData                    = baseTerrain.terrainData;
                    int nSize                   = (int)excusuibType;
                    _newTerrainWide             = nSize;
                    _newTerrainHeight           = nSize;
                    _newHeightMapResolution     = (baseData.heightmapResolution - 1) / nSize + 1 ;
                    _newAlphaMapResolution      = baseData.alphamapResolution / nSize;
                    _newBaseMapResolution       = baseData.baseMapResolution / nSize;
                    _newDetailResolution        = baseData.detailResolution / nSize;


                    if( CheckConfigError()  )
                    {
                        SlicingTerrainData();
                    }

                    if( IsSmoothEdge )
                    {
                        SmoothPatchEdges();
                    }

                    SetNeighbors();
                    this.Close();
                }
            }
        }

        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 绘制窗口布局
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        bool CheckConfigError()
        {
            if( resolutionPerPatch < 8 )
            {
                this.ShowNotification(new GUIContent(" 每个 patch 的分辨率必须是 8 或大于 8 "));
                return false;
            }

            if( !Mathf.IsPowerOfTwo(resolutionPerPatch ) )
            {
                this.ShowNotification(new GUIContent(" 尺寸必须是2的N次方 "));
                return false;
            }


            if (!Mathf.IsPowerOfTwo(resolutionPerPatch))
            {
                this.ShowNotification(new GUIContent(" 尺寸必须是2的N次方 "));
                return false;
            }

            if (_newHeightMapResolution < 33 )
            {
                this.ShowNotification(new GUIContent(" Height map 分辨率太小 "));
                return false;
            }

            if ( _newAlphaMapResolution < 16)
            {
                this.ShowNotification(new GUIContent(" alpha map 分辨率太小 "));
                return false;
            }

            if ( _newBaseMapResolution < 16)
            {
                this.ShowNotification(new GUIContent(" base map 分辨率太小 "));
                return false;
            }

            return true;
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 分割地形成多个地块patch
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        void SlicingTerrainData()
        {
            _progress = 0;
            EditorUtility.DisplayProgressBar("progress", " progress terrain", _progress);
            
            
            int nSize           = _newTerrainWide * _newTerrainHeight;
            terrainGameObjects  = new GameObject[nSize];
            terrains            = new Terrain[ nSize ];
            data                = new TerrainData[ nSize ];
            ProgessScale        = .5f / nSize;

            /// 创建地形数据
            for( int y = 0; y < _newTerrainHeight; y++ )
            {
                for( int x = 0; x < _newTerrainWide; x++  )
                {
                    AssetDatabase.CreateAsset(new TerrainData(), TerrainPatchPath + "/" + "patch_" + (y + 1) + "_" + (x + 1) + ".asset");
                    _progress     += ProgessScale;
                    EditorUtility.DisplayProgressBar("progress", " progress terrain", _progress);
                }
            }

            /// 拷贝地形数据
            int arraypos = 0;
            ProgessScale = .4f / nSize;
            for( int y = 0; y < _newTerrainHeight; y++ )
            {
                for( int x = 0; x < _newTerrainWide; x++ )
                {
                    TerrainData terdata = AssetDatabase.LoadAssetAtPath(TerrainPatchPath + "/" + "patch_" + (y + 1) + "_" + (x + 1) + ".asset", typeof(TerrainData)) as TerrainData;
                    terrainGameObjects[arraypos]        = Terrain.CreateTerrainGameObject( terdata );
                    terrainGameObjects[arraypos].name   = "patch_" + (y + 1) + "_" + (x + 1);
                    terrainGameObjects[arraypos].tag    = "EditorTerrain";
                    terrains[arraypos]                  = terrainGameObjects[arraypos].GetComponent<Terrain>();
                    data[arraypos]                      = terrains[arraypos].terrainData;
                    data[arraypos].heightmapResolution  = _newHeightMapResolution;
                    data[arraypos].alphamapResolution   = _newAlphaMapResolution;
                    data[arraypos].baseMapResolution    = _newBaseMapResolution;
                    data[arraypos].SetDetailResolution(_newDetailResolution, resolutionPerPatch);
                    data[arraypos].size                 = new Vector3( baseData.size.x / _newTerrainWide, baseData.size.y,baseData.size.z / _newTerrainHeight  );

                    SplatPrototype[] tempSplats = new SplatPrototype[baseData.splatPrototypes.Length];
                    for (int i = 0; i < baseData.splatPrototypes.Length; i++)
                    {
                        tempSplats[i]                   = new SplatPrototype();
                        tempSplats[i].texture           = baseData.splatPrototypes[i].texture;
                        tempSplats[i].tileSize          = baseData.splatPrototypes[i].tileSize;
                        tempSplats[i].normalMap         = baseData.splatPrototypes[i].normalMap;

                        float offsetX                   = (baseData.size.x / _newTerrainWide * x) % baseData.splatPrototypes[i].tileSize.x + baseData.splatPrototypes[i].tileSize.x;
                        float offsetY                   = (baseData.size.z / _newTerrainHeight * x) % baseData.splatPrototypes[i].tileSize.y + baseData.splatPrototypes[i].tileSize.y;
                        tempSplats[i].tileOffset        = new Vector2(offsetX, offsetY);
                    }
                    data[arraypos].splatPrototypes      = tempSplats;
                    int[] layers = baseData.GetSupportedLayers(x * data[arraypos].detailWidth - 1, y * data[arraypos].detailHeight - 1, data[arraypos].detailWidth, data[arraypos].detailHeight);
                    int layerLenght = layers.Length;
    
                    data[arraypos].detailPrototypes = baseData.detailPrototypes;
                    for (int i = 0; i < layerLenght; i++)
                    {
                        data[arraypos].SetDetailLayer(0, 0, i, baseData.GetDetailLayer(x * data[arraypos].detailWidth, y * data[arraypos].detailHeight, data[arraypos].detailWidth, data[arraypos].detailHeight, layers[i]));
                    }
                    System.Array.Clear(layers, 0, layers.Length);


                    data[arraypos].treePrototypes       = baseData.treePrototypes;
                    data[arraypos].wavingGrassStrength  = baseData.wavingGrassStrength;
                    data[arraypos].wavingGrassAmount    = baseData.wavingGrassAmount;
                    data[arraypos].wavingGrassSpeed     = baseData.wavingGrassSpeed;
                    data[arraypos].wavingGrassTint      = baseData.wavingGrassTint;

                    float[,] height                     = baseData.GetHeights(x * (data[arraypos].heightmapResolution - 1), y * (data[arraypos].heightmapResolution - 1), data[arraypos].heightmapResolution, data[arraypos].heightmapResolution);
                    data[arraypos].SetHeights(0, 0, height);
                    float[,,] map                       = new float[_newAlphaMapResolution, _newAlphaMapResolution, baseData.splatPrototypes.Length];
                    map                                 = baseData.GetAlphamaps(x * data[arraypos].alphamapWidth, y * data[arraypos].alphamapHeight, data[arraypos].alphamapWidth, data[arraypos].alphamapHeight);
                    data[arraypos].SetAlphamaps(0, 0, map);

                    /// 省略设置terrainGameObject 的位置
                    terrainGameObjects[arraypos].GetComponent<TerrainCollider>().terrainData = data[arraypos];
                    terrainGameObjects[arraypos].transform.position = new Vector3( x * (baseData.size.x / _newTerrainWide) + baseTerrain.GetPosition().x, baseTerrain.GetPosition().y, y * ( baseData.size.z / _newTerrainHeight ) + baseTerrain.GetPosition().z );
                    arraypos++;
                    _progress += ProgessScale;
                    EditorUtility.DisplayProgressBar("progress", " progress terrain", _progress);
                }
            }

            for( int i = 0; i < terrains.Length; i++ )
            {
                terrains[i].treeDistance                = baseTerrain.treeDistance;
                terrains[i].treeBillboardDistance       = baseTerrain.treeBillboardDistance;
                terrains[i].treeCrossFadeLength         = baseTerrain.treeCrossFadeLength;
                terrains[i].treeMaximumFullLODCount     = baseTerrain.treeMaximumFullLODCount;
                terrains[i].detailObjectDistance        = baseTerrain.detailObjectDistance;
                terrains[i].detailObjectDensity         = baseTerrain.detailObjectDensity;
                terrains[i].heightmapPixelError         = baseTerrain.heightmapPixelError;
                terrains[i].heightmapMaximumLOD         = baseTerrain.heightmapMaximumLOD;
                terrains[i].basemapDistance             = baseTerrain.basemapDistance;
                terrains[i].lightmapIndex               = baseTerrain.lightmapIndex;
                terrains[i].shadowCastingMode           = baseTerrain.shadowCastingMode;
                terrains[i].editorRenderFlags           = baseTerrain.editorRenderFlags;
            }

           
            float newWidth  = baseData.size.x / _newTerrainWide;
            float newHeight = baseData.size.z / _newTerrainHeight;
            Vector3 a       = new Vector3(baseData.size.x, baseData.size.y, baseData.size.z);
            TreeInstance[] treeDetaill = baseData.treeInstances;
            int nLenghts    = baseData.treeInstances.Length;
            for (int i = 0; i < nLenghts; i++)
            {
                
                Vector3 b               = new Vector3(treeDetaill[i].position.x, treeDetaill[i].position.y, treeDetaill[i].position.z);
                Vector3 orig            = Vector3.Scale(a, b);
                int col                 = Mathf.FloorToInt(orig.x / (baseData.size.x / _newTerrainWide));
                int row                 = Mathf.FloorToInt(orig.z / (baseData.size.x / _newTerrainHeight));

                Vector3 tempVect        = new Vector3((orig.x - (newWidth * col)) / newWidth, orig.y, (orig.z - (newHeight * row)) / newHeight);
                TreeInstance tempInst   = new TreeInstance();
                tempInst.position       = tempVect;
                tempInst.widthScale     = treeDetaill[i].widthScale;
                tempInst.heightScale    = treeDetaill[i].heightScale;
                tempInst.color          = treeDetaill[i].color;
                tempInst.lightmapColor  = treeDetaill[i].lightmapColor;
                tempInst.prototypeIndex = treeDetaill[i].prototypeIndex;

                terrains[row * _newTerrainWide + col].AddTreeInstance(tempInst);
            }

            baseTerrain.gameObject.tag = "EditorOnly";
            for (int i = 0; i < _newTerrainHeight * _newTerrainWide; i++)
            {
                data[i].RefreshPrototypes();
            }
        }


        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 平滑patch边缘
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        void SmoothPatchEdges()
        {
            int alphawidth  = data[0].alphamapWidth;
            int alphaheight = data[0].alphamapHeight;
            int numOfSplats = data[0].splatPrototypes.Length;

            float avg;
            if( _newTerrainWide > 1 && _newTerrainHeight == 1 )
            {
                for( int x = 0; x < _newTerrainWide - 1; x++)
                {
                    float[, ,] mapLeft  = data[x].GetAlphamaps(0, 0, alphawidth, alphaheight);
                    float[, ,] mapRight = data[x + 1].GetAlphamaps(0, 0, alphawidth, alphaheight);

                    for( int i = 0; i < alphaheight; i++ )
                        for( int y = 0; y < numOfSplats; y++ )
                        {
                            avg                         = (mapLeft[i, alphawidth - 1, y] + mapRight[i, 0, y]) / 2f;
                            mapLeft[i, alphawidth-1, y] = avg;
                            mapRight[i, 0, y]           = avg;
                        }

                    data[x].SetAlphamaps(0, 0, mapLeft);
                    data[x + 1].SetAlphamaps(0, 0, mapRight);
                }
            }

            else if (_newTerrainHeight > 1 && _newTerrainWide == 1)
            {
                for( int x = 0; x < _newTerrainHeight; x++ )
                {
                    float[, ,] mapBottom    = data[x].GetAlphamaps(0, 0, alphawidth, alphaheight);
                    float[, ,] mapTop       = data[x + 1].GetAlphamaps(0, 0, alphawidth, alphaheight);

                    for( int i = 0; i < alphawidth; i++ )
                        for( int y = 0; y < numOfSplats; y++ )
                        {
                            avg                               = (mapBottom[ alphaheight -1, i, y] + mapTop[0, i, y ] ) / 2f;
                            mapBottom[alphaheight - 1, i, y ] = avg;
                            mapTop[i, i, y]                   = avg;
                        }

                    data[x].SetAlphamaps(0, 0, mapBottom);
                    data[x + 1].SetAlphamaps(0, 0, mapTop);
                }
            }

            else if (_newTerrainHeight > 1 && _newTerrainWide > 1)
            {
                int arraypos = -2;
                for( int z = 0; z < _newTerrainHeight - 1; z++ )
                {
                    arraypos++;
                    for( int x = 0; x < _newTerrainWide - 1; x++ )
                    {
                        arraypos++;
                        float[, ,] mapBLeft         = data[arraypos].GetAlphamaps(0, 0, alphawidth, alphaheight);
                        float[, ,] mapBRight        = data[arraypos + 1].GetAlphamaps(0, 0, alphawidth, alphaheight);
                        float[, ,] mapTLeft         = data[arraypos + _newTerrainWide].GetAlphamaps( 0, 0, alphawidth, alphaheight );
                        float[,,] mapTRight         = data[arraypos +_newTerrainWide+1].GetAlphamaps( 0, 0, alphawidth, alphaheight );

                        for( int i = 1; i < alphawidth - 1; i++ )
                            for( int y = 0; y < numOfSplats; y++ )
                            {
                                avg                                 = (mapBRight[alphaheight - 1, i, y] + mapTRight[0, i, y]) / 2f;
                                mapBRight[alphaheight - 1, i, y]    = avg;
                                mapTRight[0, i, y]                  = avg;
                            }

                        for( int i = 1; i < alphaheight - 1; i++ )
                            for( int y = 0; y < numOfSplats; y++ )
                            {
                                avg                                 = (mapTLeft[i, alphawidth - 1, y] + mapTRight[i, 0, y]) / 2f;
                                mapTLeft[i, alphawidth - 1, y]      = avg;
                                mapTRight[i, 0, y]                  = avg;
                            }

                        for (int y = 0; y < numOfSplats; y++)
                        {
                            avg                                     = (mapBLeft[alphaheight - 1, alphawidth - 1, y] + mapBRight[alphaheight - 1, 0, y] + mapTLeft[0, alphawidth - 1, y] + mapTRight[0, 0, y]) / 4f;
                            mapBLeft[alphaheight - 1, alphawidth - 1, y] = avg;
                            mapBRight[alphaheight - 1, 0, y]        = avg;
                            mapTLeft[0, alphawidth - 1, y]          = avg;
                            mapTRight[0, 0, y]                      = avg;
                        }


                        if( z == 0 )
                        {
                            for( int i = 1; i < alphaheight - 1; i++ )
                                for( int y = 0; y < numOfSplats; y++ )
                                {
                                    avg                             = (mapBLeft[i, alphawidth - 1, y] + mapBRight[i, 0, y]) / 2f;
                                    mapBLeft[i, alphawidth - 1, y]  = avg;
                                    mapBRight[i, 0, y]              = avg;

                                }

                            for( int y = 0; y < numOfSplats; y++ )
                            {
                                avg                                 = (mapBLeft[0, alphawidth - 1, y] + mapBRight[0, 0, y]) / 2f;
                                mapBLeft[0, alphawidth - 1, y]      = avg;
                                mapBRight[0, 0, y]                  = avg;
                            }
                        }

                        if( x == 0 )
                        {
                            for( int i = 1; i < alphawidth - 1; i++ )
                                for( int y = 0; y < numOfSplats; y++ )
                                {
                                    avg                             = (mapBLeft[alphaheight - 1, i, y] + mapTLeft[0, i, y]) / 2f;
                                    mapBLeft[alphaheight - 1, i, y] = avg;
                                    mapTLeft[0, i, y]               = avg;
                                }

                            for (int y = 0; y < numOfSplats; y++)
                            {
                                avg                                 = ( mapBLeft[ alphaheight - 1, 0, y ] + mapTLeft[ 0, 0, y]) / 2f;
                                mapBLeft[alphaheight - 1, 0, y]     = avg;
                                mapTLeft[0, 0, y]                   = avg;
                            }
                        }


                        if( x == _newTerrainWide - 2 )
                            for( int y = 0; y < numOfSplats; y++ )
                            {
                                avg                                 = (mapBRight[alphaheight - 1, alphawidth - 1, y] + mapTRight[0, alphawidth - 1, y]) / 2f;
                                mapBRight[alphaheight - 1, alphawidth - 1, y] = avg;
                                mapTRight[0, alphawidth - 1, y]     = avg;
                            }

                        if( z ==  _newTerrainHeight - 2 )
                            for( int y = 0; y < numOfSplats; y++ )
                            {
                                avg                                 = (mapTLeft[alphaheight - 1, alphawidth - 1, y] + mapTRight[alphaheight - 1, 0, y]) / 2f;
                                mapTLeft[alphaheight - 1, alphawidth - 1, y] = avg;
                                mapTRight[alphaheight - 1, 0, y]    = avg;
                            }


                        data[arraypos].SetAlphamaps(0, 0, mapBLeft);
                        data[arraypos + 1].SetAlphamaps(0, 0, mapBRight);
                        data[arraypos + _newTerrainWide].SetAlphamaps(0, 0, mapTLeft);
                        data[arraypos + _newTerrainWide + 1].SetAlphamaps(0, 0, mapTRight);
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 设置地块的邻接
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        public void SetNeighbors()
        {
            int arraypos    = 0;
            int width       = _newTerrainWide;
            int height      = _newTerrainHeight;
            for( int y = 0; y < _newTerrainHeight; y++ )
            {
                for( int x = 0; x < _newTerrainWide; x++ )
                {
                    if( y == 0 )
                    {
                        if (x == 0)
                            terrains[arraypos].SetNeighbors(null, terrains[arraypos + width], terrains[arraypos + 1], null);
                        else if (x == width - 1)
                            terrains[arraypos].SetNeighbors(terrains[arraypos - 1], terrains[arraypos + width], null, null);
                        else
                            terrains[arraypos].SetNeighbors(terrains[arraypos - 1], terrains[arraypos + width], terrains[arraypos + 1], null);
                    }
                    else if( y == height - 1 )
                    {
                        if (x == 0)
                            terrains[arraypos].SetNeighbors(null, null, terrains[arraypos + 1], terrains[arraypos - width]);
                        else if (x == width - 1)
                            terrains[arraypos].SetNeighbors(terrains[arraypos - 1], null, null, terrains[arraypos - width]);
                        else
                            terrains[arraypos].SetNeighbors(terrains[arraypos - 1], null, terrains[arraypos + 1], terrains[arraypos - width]);
                    }
                    else
                    {
                        if (x == 0)
                            terrains[arraypos].SetNeighbors(null, terrains[arraypos + width], terrains[arraypos + 1], terrains[arraypos - width]);
                        else if (x == width - 1)
                            terrains[arraypos].SetNeighbors(terrains[arraypos - 1], terrains[arraypos + width], null, terrains[arraypos - width]);
                        else
                            terrains[arraypos].SetNeighbors(terrains[arraypos - 1], terrains[arraypos + width], terrains[arraypos + 1], terrains[arraypos - width]);
                    }

                    arraypos++;
                }
            }
        }
    }
}