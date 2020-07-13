//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-11
// Author: LJP
// Date: 2020-05-11
// Description: unity 地形转换 mesh, 地形植被到 gameobject for 四叉树管理场景对象
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;





namespace ActEditor
{
    
    class TerrainToMeshBehaviour : MonoBehaviour
    {
        [MenuItem(ActEditorMenuItem.ExportTerrainFenGeMesh, false, ActEditorMenuItem.ExportTerrainFenGeMeshPriority)]

        static void CTerrainToMeshWindow()
        {
            TerrainToMeshEditor window = (TerrainToMeshEditor)EditorWindow.GetWindowWithRect(typeof(TerrainToMeshEditor), new Rect(0, 0, 386, 500), false, "地形转Mesh");
            window.Show();
        }
    }


    /// <summary>
    /// 地形转Mesh操作集合
    /// </summary>
    public class TerrainToMeshEditor : EditorWindow
    {
        /// <summary>
        /// 植被处理对象
        /// </summary>
        private CVegetationProcess  mVegetationHandle = new CVegetationProcess();

        /// <summary>
        /// 地形到mesh
        /// </summary>
        private TerrainGenShapeMesh mTerrainGenMesh = new TerrainGenShapeMesh();


        private int X;
        private int Y;
        private int     T4MResolution = 64;
        private float   tRes = 4.1f;
        private float   HeightmapWidth = 0;
        private float   HeightmapHeight = 0;

        private bool    genGrassLod    = false;
        private bool    genLodMesh     = false;
        private bool    keepTexture    = false;


        private int     MenuToolbar = 0;
        GUIContent[]    MenuIcon    = new GUIContent[2];
    
        void OnInspectorUpdate()
        {
            Repaint();
        }

        /// -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 绘制窗口布局
        /// </summary>
        /// -----------------------------------------------------------------------------------------------------------
        void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginArea(new Rect(0,0,700, 32));
            MenuIcon[0] = new GUIContent("处理地形及LOD");
            MenuIcon[1] = new GUIContent("处理植被及LOD");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(25, 21, 363, 700));
            GUIStyle SelectionGridStyle = new GUIStyle(EditorStyles.miniButton);
            SelectionGridStyle.fixedHeight = 35;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            MenuToolbar = GUILayout.SelectionGrid(MenuToolbar, MenuIcon, 4, SelectionGridStyle, GUILayout.Height(68), GUILayout.Width(150 * Screen.width / Screen.dpi));
            EditorGUILayout.EndHorizontal();

            var HelpButtonStyle = new GUIStyle(GUI.skin.button);
            HelpButtonStyle.normal.textColor = Color.white;
            HelpButtonStyle.fontStyle = FontStyle.Bold;

            switch (MenuToolbar)
            {
                case 0:
                    ConverterMenu();
                    break;
                case 1:
                    ConverterMenuGrass();
                    break;
            }
            GUILayout.EndArea();
            GUILayout.EndHorizontal();

            
        }

        void ConverterMenu()
        {
            GetHeightmap();
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUILayout.Label(">>>>>>>> 地形到Mesh转化操作 <<<<<<<<", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Keep the textures", EditorStyles.boldLabel, GUILayout.Width(150));
            keepTexture = EditorGUILayout.Toggle(keepTexture, GUILayout.Width(53));
            GUILayout.EndHorizontal();
            GUILayout.Label("keep t4 splats and first Blend", GUILayout.Width(300));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gen Mesh Lod", EditorStyles.boldLabel, GUILayout.Width(150));
            genLodMesh = EditorGUILayout.Toggle(genLodMesh, GUILayout.Width(53));
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Mesh Quality", EditorStyles.boldLabel);


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Label(" <");
            GUILayout.FlexibleSpace();
            T4MResolution = EditorGUILayout.IntField(T4MResolution, GUILayout.Width(30));
            GUILayout.Label("x " + T4MResolution + " : " + (X * Y).ToString() + " Verts");
            GUILayout.FlexibleSpace();
            GUILayout.Label(" >");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                    T4MResolution = (int)GUILayout.HorizontalScrollbar(T4MResolution, 0, 32, 350, GUILayout.Width(350));
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            tRes    = (HeightmapWidth) / T4MResolution;
            X       = (int)((HeightmapWidth - 1) / tRes + 1);
            Y       = (int)((HeightmapHeight - 1) / tRes + 1);


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("PROCESS", GUILayout.Width(100), GUILayout.Height(30)))
            {
                GameObject[] terrain = GameObject.FindGameObjectsWithTag("EditorTerrain");
                if (terrain != null || terrain.Length > 0)
                {
                    if (!genLodMesh)
                    {
                        mTerrainGenMesh.InitRootDirectoryFolder();
                        for (int n = 0; n < terrain.Length; n++)
                        {
                            TerrainData tdata = terrain[n].GetComponent<Terrain>().terrainData;
                            tRes = tdata.heightmapResolution / T4MResolution;
                            mTerrainGenMesh.ConvertUTerrain(terrain[n], tRes, keepTexture);
                        }
                    }
                    //else
                    //{
                    //    /// 生产lod
                    //    mTerrainGenMesh.InitRootDirectoryFolder();
                    //    for (int n = 0; n < terrain.Length; n++)
                    //    {
                    //        TerrainData tdata = terrain[n].GetComponent<Terrain>().terrainData;
                    //        tRes = tdata.heightmapResolution / T4MResolution;
                    //        mTerrainGenMesh.ConvertUTerrainLOD(terrain[n], tRes * 2);
                    //    }
                    //}
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }


        void GetHeightmap()
        {
            Terrain[] terrain = GameObject.FindObjectsOfType<Terrain>();
            if (terrain != null && terrain.Length > 0 )
            {
                
                TerrainData terrainDat      = terrain[0].terrainData;
                HeightmapWidth              = terrainDat.heightmapResolution;
                HeightmapHeight             = terrainDat.heightmapResolution;
            }
        }


        void ConverterMenuGrass()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUILayout.Label(">>>>>>>> 植被转化操作 <<<<<<<<", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Is Gen Grass Lod", EditorStyles.boldLabel, GUILayout.Width(150));
            genGrassLod = EditorGUILayout.Toggle(genGrassLod, GUILayout.Width(53));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("处理植被及LOD"))
            {
                mVegetationHandle.CreateRootGameObject();
                mVegetationHandle.ClearChildrenFromRoot();
                Terrain[] terrain = GameObject.FindObjectsOfType<Terrain>();
                if (terrain != null)
                {
                    for (int n = 0; n < terrain.Length; n++)
                        mVegetationHandle.VegetationProcess(terrain[n]);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
