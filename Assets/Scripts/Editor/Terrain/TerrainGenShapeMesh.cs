//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-11
// Author: LJP
// Date: 2020-05-11
// Description: unity 地形转换 mesh
//---------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using ActClient;

namespace ActEditor
{

    

    class TerrainGenShapeMesh
    {
        /// <summary>
        /// 资源根路径
        /// </summary>
        string T4MPrefabFolder      = "Assets/Export/T4MOBJ/";
        string terrainName          = "";
        string sceneName            = "";


        string textureFormat        = "{0}{1}{2}_tex.png";
        string materialFormat       = "{0}{1}{2}_mat.mat";
        string objectFormat         = "{0}{1}{2}_fbx.fbx";
        string prefabFormat         = "{0}{1}{2}_fab.prefab";
        string objectlodFormat      = "{0}{1}{2}_lod_fbx.fbx";
        string prefablodFormat      = "{0}{1}{2}_lod_fab.prefab";

        public void InitRootDirectoryFolder()
        {
            T4MPrefabFolder         = "Assets/Export/T4MOBJ/";
            sceneName               = UtilityTools.GetFileName(EditorApplication.currentScene);
            T4MPrefabFolder         += sceneName;

            /// 删除老的资源
            if (Directory.Exists(T4MPrefabFolder))
                Directory.Delete(T4MPrefabFolder, true );

            AssetDatabase.Refresh();
            T4MPrefabFolder         += "/";
            AssetDatabase.SaveAssets();
        }

        public void ConvertUTerrain( GameObject CurrentSelect, float tRes, bool keepTexture = false )
        {

            if (!System.IO.Directory.Exists(T4MPrefabFolder + "Terrains/"))
            {
                System.IO.Directory.CreateDirectory(T4MPrefabFolder + "Terrains/");
            }
            if (!System.IO.Directory.Exists(T4MPrefabFolder + "Terrains/Material/"))
            {
                System.IO.Directory.CreateDirectory(T4MPrefabFolder + "Terrains/Material/");
            }
            if (!System.IO.Directory.Exists(T4MPrefabFolder + "Terrains/Texture/"))
            {
                System.IO.Directory.CreateDirectory(T4MPrefabFolder + "Terrains/Texture/");
            }
            if (!System.IO.Directory.Exists(T4MPrefabFolder + "Terrains/Meshes/"))
            {
                System.IO.Directory.CreateDirectory(T4MPrefabFolder + "Terrains/Meshes/");
            }
            AssetDatabase.Refresh();

            TerrainData terrain = CurrentSelect.GetComponent<Terrain>().terrainData;
            int w               = terrain.heightmapResolution;
            int h               = terrain.heightmapResolution;
            Vector3 meshScale   = terrain.size;
            meshScale           = new Vector3(meshScale.x / (h - 1) * tRes, meshScale.y, meshScale.z / (w - 1) * tRes);
            Vector2 uvScale     = new Vector2((float)(1.0 / (w - 1)), (float)(1.0 / (h - 1)));

            float[,] tData      = terrain.GetHeights(0, 0, w, h);
            w                   = (int)((w - 1) / tRes + 1);
            h                   = (int)((h - 1) / tRes + 1);
            Vector3[] tVertices = new Vector3[w * h];
            Vector2[] tUV       = new Vector2[w * h];
            int[] tPolys        = new int[(w - 1) * (h - 1) * 6];
            int index = 0;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    index            = j * w + i;
                    float z          = tData[(int)(j * tRes), (int)(i * tRes)];
                    tVertices[index] = Vector3.Scale(new Vector3(i, z, j), meshScale);
                    tUV[index]       = Vector2.Scale(new Vector2(i * tRes, j * tRes ), uvScale);
                }
            }

            index = 0;
            for (int i = 0; i < w - 1; i++)
            {
                for (int j = 0; j < h - 1; j++)
                {
                    int a           = j * w + i;
                    int b           = (j + 1) * w + i;
                    int c           = (j + 1) * w + i + 1;
                    int d           = j * w + i + 1;

                    tPolys[index++] = a;
                    tPolys[index++] = b;
                    tPolys[index++] = c;

                    tPolys[index++] = a;
                    tPolys[index++] = c;
                    tPolys[index++] = d;
                }
            }

            Mesh mesh       = new Mesh();
            mesh.vertices   = tVertices;
            mesh.uv         = tUV;
            mesh.triangles  = tPolys;
            //mesh.colors     = alphasWeight;
            mesh.RecalculateNormals();

            // 保存 fbx
            string patchName        = sceneName +"_" + CurrentSelect.name;
            string terrainObjPath   = string.Format(objectFormat, T4MPrefabFolder, "Terrains/Meshes/", patchName);
            try
            {
                string buildMesh = MeshToString(mesh, terrainObjPath, false, false);
                if (System.IO.File.Exists(terrainObjPath))
                    System.IO.File.Delete(terrainObjPath);

                System.IO.File.WriteAllText(terrainObjPath, buildMesh);
            }
            catch (Exception err)
            {
                Debug.Log("Error saving file: " + err.Message);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 2.0 地形图层控制alpha
            string texturePath          = string.Format(textureFormat, T4MPrefabFolder, "Terrains/Texture/", patchName);
            TextureImporter TextureI    = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (TextureI == null)
            {
                string AssetName        = AssetDatabase.GetAssetPath(CurrentSelect.GetComponent<Terrain>().terrainData) as string;
                UnityEngine.Object[] AssetName2 = AssetDatabase.LoadAllAssetsAtPath(AssetName);
                if (AssetName2 != null && AssetName2.Length > 1 && keepTexture)
                {
                    for (var b = 0; b < AssetName2.Length; b++)
                    {
                        if (AssetName2[b].name == "SplatAlpha 0")
                        {
                            Texture2D texture = AssetName2[b] as Texture2D;
                            byte[] bytes = texture.EncodeToPNG();
                            File.WriteAllBytes(texturePath, bytes);
                            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                        }
                    }
                }
                else
                {
                    Texture2D NewMaskText   = new Texture2D(512, 512, TextureFormat.ARGB32, true);
                    Color[] ColorBase       = new Color[512 * 512];
                    for (var t = 0; t < ColorBase.Length; t++)
                    {
                        ColorBase[t]        = new Color(1, 0, 0, 0);
                    }
                    NewMaskText.SetPixels(ColorBase);
                    byte[] data = NewMaskText.EncodeToPNG();
                    File.WriteAllBytes(texturePath, data);
                    AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                }

                TextureI = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            }

            TextureI.textureFormat      = TextureImporterFormat.ARGB32;
            TextureI.textureCompression = TextureImporterCompression.Uncompressed;
            TextureI.isReadable         = true;
            TextureI.sRGBTexture        = false;
            TextureI.anisoLevel         = 9;
            TextureI.mipmapEnabled      = false;
            TextureI.wrapMode           = TextureWrapMode.Clamp;
            AssetDatabase.Refresh();

            /// 3.0 材质, Assets/Export/T4MOBJ/
            string materialPath          = string.Format(materialFormat, T4MPrefabFolder, "Terrains/Material/", patchName);
            AssetDatabase.ImportAsset(materialPath, ImportAssetOptions.ForceUpdate);
            Material Tmaterial;
            Tmaterial                   = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
            if (Tmaterial == null)
            {
                //Shader sd             = Resources.Load<Shader>("ZSY/Scene/SceneTerrain");
                Shader sd               = (Shader)AssetDatabase.LoadAssetAtPath("Assets/AssetsPackage/Shaders/Scene/SceneTerrain.shader", typeof(Shader));
                Tmaterial               = new Material(sd);
                AssetDatabase.CreateAsset(Tmaterial, materialPath);
                AssetDatabase.ImportAsset(materialPath, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();
            }

            //Recuperation des Texture du terrain
            if (keepTexture)
            {
                SplatPrototype[] texts = CurrentSelect.GetComponent<Terrain>().terrainData.splatPrototypes;
               for (int e = 0; e < texts.Length; e++)
                {
                    if ( e<= 4)
                    {
                        Tmaterial.SetTexture("_MainTex" + e,    texts[e].texture);
                        Tmaterial.SetTextureScale("_MainTex" + e, texts[e].tileSize);
                        Tmaterial.SetTextureOffset("_MainTex" + e, texts[e].tileOffset);
                    }
                }
            }

            //Attribution de la Texture Control sur le materiau
            Texture test = (Texture)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture));
            Tmaterial.SetTexture("_Control", test);
 
            //Force Update
            AssetDatabase.ImportAsset(terrainObjPath, ImportAssetOptions.ForceUpdate);


            // 4.0 perfab
            GameObject prefab           = (GameObject)AssetDatabase.LoadAssetAtPath(terrainObjPath, typeof(GameObject));
            GameObject forRotate        = (GameObject)GameObject.Instantiate(prefab, CurrentSelect.transform.position, Quaternion.identity) as GameObject;
            GameObject Child            = forRotate;
            Child.name                  = patchName;
            Child.tag                   = "Terrain";
            int countchild = Child.transform.childCount;
            if (countchild > 0)
            {
                Renderer[] T4MOBJPART = Child.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < T4MOBJPART.Length; i++)
                {
                    if (!T4MOBJPART[i].gameObject.AddComponent<MeshCollider>())
                        T4MOBJPART[i].gameObject.AddComponent<MeshCollider>();
                    T4MOBJPART[i].gameObject.isStatic = false;
                    T4MOBJPART[i].material = Tmaterial;
                }
            }
            else
            {
                Child.AddComponent<MeshCollider>();
                Child.isStatic = false;
                Child.GetComponent<Renderer>().material = Tmaterial;
            }


            string terrainPrefabPath        = string.Format(prefabFormat, T4MPrefabFolder, "Terrains/", patchName);
            GameObject BasePrefab2          = PrefabUtility.CreatePrefab(terrainPrefabPath, Child);
            AssetDatabase.ImportAsset(terrainPrefabPath, ImportAssetOptions.ForceUpdate);
            GameObject forRotate2           = (GameObject)PrefabUtility.InstantiatePrefab(BasePrefab2) as GameObject;
            Child                           = forRotate2.gameObject;

            
            CurrentSelect.GetComponent<Terrain>().enabled = false;
           
            AssetDatabase.StartAssetEditing();
            ModelImporter OBJI              = ModelImporter.GetAtPath(terrainObjPath) as ModelImporter;
            OBJI.globalScale                = 1;
            OBJI.splitTangentsAcrossSeams   = true;
            OBJI.normalImportMode           = ModelImporterTangentSpaceMode.Calculate;
            OBJI.tangentImportMode          = ModelImporterTangentSpaceMode.Calculate;
            OBJI.generateAnimations         = ModelImporterGenerateAnimations.None;
            OBJI.meshCompression            = ModelImporterMeshCompression.Off;
            OBJI.normalSmoothingAngle       = 180f;
            //OBJI.importCameras              = false;
            //OBJI.importBlendShapes          = true;
            //OBJI.importAnimation            = false;
            //OBJI.importLights               = false;
            //OBJI.optimizeMesh               = true;
            OBJI.importMaterials            = false;
            OBJI.isReadable                 = true;
            AssetDatabase.ImportAsset(terrainObjPath, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.StopAssetEditing();
            PrefabUtility.ResetToPrefabState(Child);

            /// 删除场景中临时对象
            GameObject.DestroyImmediate(forRotate);
            //GameObject.DestroyImmediate(forRotate2);
           
        }

        public void ConvertUTerrainLOD( GameObject CurrentSelect, float tRes )
        {
            var sceneName           = UtilityTools.GetFileName(EditorApplication.currentScene);
            T4MPrefabFolder         += sceneName;
            T4MPrefabFolder         += "/";

            if (!System.IO.Directory.Exists(T4MPrefabFolder + "Terrains/Lod/"))
            {
                System.IO.Directory.CreateDirectory(T4MPrefabFolder + "Terrains/Lod/");
            }
            if (!System.IO.Directory.Exists(T4MPrefabFolder + "Terrains/Lod/Meshes/"))
            {
                System.IO.Directory.CreateDirectory(T4MPrefabFolder + "Terrains/Lod/Meshes/");
            }
            AssetDatabase.Refresh();

            TerrainData terrain = CurrentSelect.GetComponent<Terrain>().terrainData;
            int w               = terrain.heightmapResolution;
            int h               = terrain.heightmapResolution;
            Vector3 meshScale   = terrain.size;
            meshScale           = new Vector3(meshScale.x / (h - 1) * tRes, meshScale.y, meshScale.z / (w - 1) * tRes);
            Vector2 uvScale     = new Vector2((float)(1.0 / (w - 1)), (float)(1.0 / (h - 1)));

            float[,] tData      = terrain.GetHeights(0, 0, w, h);
            w                   = (int)((w - 1) / tRes + 1);
            h                   = (int)((h - 1) / tRes + 1);
            Vector3[] tVertices = new Vector3[w * h];
            Vector2[] tUV       = new Vector2[w * h];
            int[] tPolys        = new int[(w - 1) * (h - 1) * 6];
            int index = 0;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    index            = j * w + i;
                    float z          = tData[(int)(j * tRes), (int)(i * tRes)];
                    tVertices[index] = Vector3.Scale(new Vector3(i, z, j), meshScale);
                    tUV[index]       = Vector2.Scale(new Vector2(i * tRes, j * tRes), uvScale);
                }
            }

            index = 0;
            for (int i = 0; i < w - 1; i++)
            {
                for (int j = 0; j < h - 1; j++)
                {
                    int a           = j * w + i;
                    int b           = (j + 1) * w + i;
                    int c           = (j + 1) * w + i + 1;
                    int d           = j * w + i + 1;

                    tPolys[index++] = a;
                    tPolys[index++] = b;
                    tPolys[index++] = c;

                    tPolys[index++] = a;
                    tPolys[index++] = c;
                    tPolys[index++] = d;
                }
            }

            Mesh mesh       = new Mesh();
            mesh.vertices   = tVertices;
            mesh.uv         = tUV;
            mesh.triangles  = tPolys;
            mesh.RecalculateNormals();

            // 保存 fbx
            string patchName        = CurrentSelect.name;
            string terrainObjPath   = string.Format(objectlodFormat, T4MPrefabFolder, "Terrains/Meshes/", patchName);
            try
            {
                string buildMesh = MeshToString(mesh, terrainObjPath, false, false);
                if (System.IO.File.Exists(terrainObjPath))
                    System.IO.File.Delete(terrainObjPath);

                System.IO.File.WriteAllText(terrainObjPath, buildMesh);
            }
            catch (Exception err)
            {
                Debug.Log("Error saving file: " + err.Message);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(terrainObjPath, ImportAssetOptions.ForceUpdate);

            // 2.0 perfab
            GameObject prefab           = (GameObject)AssetDatabase.LoadAssetAtPath(terrainObjPath, typeof(GameObject));
            GameObject forRotate        = (GameObject)GameObject.Instantiate(prefab, CurrentSelect.transform.position, Quaternion.identity) as GameObject;
            GameObject Child            = forRotate;
            Child.name                  = patchName;
            int countchild = Child.transform.childCount;
            if (countchild > 0)
            {
                Renderer[] T4MOBJPART = Child.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < T4MOBJPART.Length; i++)
                {
                    if (!T4MOBJPART[i].gameObject.AddComponent<MeshCollider>())
                        T4MOBJPART[i].gameObject.AddComponent<MeshCollider>();
                    T4MOBJPART[i].gameObject.isStatic = false;
                
                }
            }
            else
            {
                Child.AddComponent<MeshCollider>();
                Child.isStatic = false;
            }


            string terrainPrefabPath        = string.Format(prefablodFormat, T4MPrefabFolder, "Terrains/", patchName);
            GameObject BasePrefab2          = PrefabUtility.CreatePrefab(terrainPrefabPath, Child);
            AssetDatabase.ImportAsset(terrainPrefabPath, ImportAssetOptions.ForceUpdate);
            GameObject forRotate2           = (GameObject)PrefabUtility.InstantiatePrefab(BasePrefab2) as GameObject;
            Child                           = forRotate2.gameObject;

            CurrentSelect.GetComponent<Terrain>().enabled = false;

            AssetDatabase.StartAssetEditing();
            ModelImporter OBJI              = ModelImporter.GetAtPath(terrainObjPath) as ModelImporter;
            OBJI.globalScale                = 1;
            OBJI.splitTangentsAcrossSeams   = true;
            OBJI.normalImportMode           = ModelImporterTangentSpaceMode.Calculate;
            OBJI.tangentImportMode          = ModelImporterTangentSpaceMode.Calculate;
            OBJI.generateAnimations         = ModelImporterGenerateAnimations.None;
            OBJI.meshCompression            = ModelImporterMeshCompression.Off;
            OBJI.normalSmoothingAngle       = 180f;
            OBJI.importCameras              = false;
            OBJI.importBlendShapes          = true;
            OBJI.importAnimation            = false;
            OBJI.importLights               = false;
            OBJI.optimizeMesh               = true;
            OBJI.importMaterials            = false;
            OBJI.isReadable                 = true;
            AssetDatabase.ImportAsset(terrainObjPath, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.StopAssetEditing();
            PrefabUtility.ResetToPrefabState(Child);

            /// 删除场景中临时对象
            GameObject.DestroyImmediate(forRotate);
            GameObject.DestroyImmediate(forRotate2);
        }

        public string MeshToString(Mesh gameObj, string newPath, bool copyMaterials = false, bool copyTextures = false)
        {
            StringBuilder sb = new StringBuilder();

            StringBuilder objectProps = new StringBuilder();
            objectProps.AppendLine("; Object properties");
            objectProps.AppendLine(";------------------------------------------------------------------");
            objectProps.AppendLine("");
            objectProps.AppendLine("Objects:  {");

            StringBuilder objectConnections = new StringBuilder();
            objectConnections.AppendLine("; Object connections");
            objectConnections.AppendLine(";------------------------------------------------------------------");
            objectConnections.AppendLine("");
            objectConnections.AppendLine("Connections:  {");
            objectConnections.AppendLine("\t");

            Material[] materials = new Material[0];

            // First finds all unique materials and compiles them (and writes to the object connections) for funzies
            string materialsObjectSerialized = "";
            string materialConnectionsSerialized = "";
            // Run recursive FBX Mesh grab over the entire gameobject
            GetMeshToString(gameObj, materials, ref objectProps, ref objectConnections);


            // Close up both builders;
            objectProps.AppendLine("}");
            objectConnections.AppendLine("}");


            // ========= Create header ========

            // Intro
            sb.AppendLine("; FBX 7.3.0 project file");
            sb.AppendLine("; Copyright (C) 1997-2010 Autodesk Inc. and/or its licensors.");
            sb.AppendLine("; All rights reserved.");
            sb.AppendLine("; ----------------------------------------------------");
            sb.AppendLine();

            // The header
            sb.AppendLine("FBXHeaderExtension:  {");
            sb.AppendLine("\tFBXHeaderVersion: 1003");
            sb.AppendLine("\tFBXVersion: 7300");

            // Creationg Date Stamp
            System.DateTime currentDate = System.DateTime.Now;
            sb.AppendLine("\tCreationTimeStamp:  {");
            sb.AppendLine("\t\tVersion: 1000");
            sb.AppendLine("\t\tYear: " + 0);
            sb.AppendLine("\t\tMonth: " + 0);
            sb.AppendLine("\t\tDay: " + 0);
            sb.AppendLine("\t\tHour: " + 0);
            sb.AppendLine("\t\tMinute: " + 0);
            sb.AppendLine("\t\tSecond: " + 0);
            sb.AppendLine("\t\tMillisecond: " + 0);
            sb.AppendLine("\t}");

            // Info on the Creator
            sb.AppendLine("\tCreator: \"" + VersionInformation + "\"");
            sb.AppendLine("\tSceneInfo: \"SceneInfo::GlobalInfo\", \"UserData\" {");
            sb.AppendLine("\t\tType: \"UserData\"");
            sb.AppendLine("\t\tVersion: 100");
            sb.AppendLine("\t\tMetaData:  {");
            sb.AppendLine("\t\t\tVersion: 100");
            sb.AppendLine("\t\t\tTitle: \"\"");
            sb.AppendLine("\t\t\tSubject: \"\"");
            sb.AppendLine("\t\t\tAuthor: \"\"");
            sb.AppendLine("\t\t\tKeywords: \"\"");
            sb.AppendLine("\t\t\tRevision: \"\"");
            sb.AppendLine("\t\t\tComment: \"\"");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\tProperties70:  {");

            // Information on how this item was originally generated
            string documentInfoPaths = Application.dataPath + newPath + ".fbx";
            sb.AppendLine("\t\t\tP: \"DocumentUrl\", \"KString\", \"Url\", \"\", \"" + documentInfoPaths + "\"");
            sb.AppendLine("\t\t\tP: \"SrcDocumentUrl\", \"KString\", \"Url\", \"\", \"" + documentInfoPaths + "\"");
            sb.AppendLine("\t\t\tP: \"Original\", \"Compound\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"Original|ApplicationVendor\", \"KString\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"Original|ApplicationName\", \"KString\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"Original|ApplicationVersion\", \"KString\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"Original|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"Original|FileName\", \"KString\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"LastSaved\", \"Compound\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationVendor\", \"KString\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationName\", \"KString\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationVersion\", \"KString\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t\tP: \"LastSaved|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // The Global information
            sb.AppendLine("GlobalSettings:  {");
            sb.AppendLine("\tVersion: 1000");
            sb.AppendLine("\tProperties70:  {");
            sb.AppendLine("\t\tP: \"UpAxis\", \"int\", \"Integer\", \"\",1");
            sb.AppendLine("\t\tP: \"UpAxisSign\", \"int\", \"Integer\", \"\",1");
            sb.AppendLine("\t\tP: \"FrontAxis\", \"int\", \"Integer\", \"\",2");
            sb.AppendLine("\t\tP: \"FrontAxisSign\", \"int\", \"Integer\", \"\",1");
            sb.AppendLine("\t\tP: \"CoordAxis\", \"int\", \"Integer\", \"\",0");
            sb.AppendLine("\t\tP: \"CoordAxisSign\", \"int\", \"Integer\", \"\",1");
            sb.AppendLine("\t\tP: \"OriginalUpAxis\", \"int\", \"Integer\", \"\",-1");
            sb.AppendLine("\t\tP: \"OriginalUpAxisSign\", \"int\", \"Integer\", \"\",1");
            sb.AppendLine("\t\tP: \"UnitScaleFactor\", \"double\", \"Number\", \"\",100"); // NOTE: This sets the resize scale upon import
            sb.AppendLine("\t\tP: \"OriginalUnitScaleFactor\", \"double\", \"Number\", \"\",100");
            sb.AppendLine("\t\tP: \"AmbientColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
            sb.AppendLine("\t\tP: \"DefaultCamera\", \"KString\", \"\", \"\", \"Producer Perspective\"");
            sb.AppendLine("\t\tP: \"TimeMode\", \"enum\", \"\", \"\",11");
            sb.AppendLine("\t\tP: \"TimeSpanStart\", \"KTime\", \"Time\", \"\",0");
            sb.AppendLine("\t\tP: \"TimeSpanStop\", \"KTime\", \"Time\", \"\",479181389250");
            sb.AppendLine("\t\tP: \"CustomFrameRate\", \"double\", \"Number\", \"\",-1");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // The Object definations
            sb.AppendLine("; Object definitions");
            sb.AppendLine(";------------------------------------------------------------------");
            sb.AppendLine("");
            sb.AppendLine("Definitions:  {");
            sb.AppendLine("\tVersion: 100");
            sb.AppendLine("\tCount: 4");

            sb.AppendLine("\tObjectType: \"GlobalSettings\" {");
            sb.AppendLine("\t\tCount: 1");
            sb.AppendLine("\t}");


            sb.AppendLine("\tObjectType: \"Model\" {");
            sb.AppendLine("\t\tCount: 1"); // TODO figure out if this count matters
            sb.AppendLine("\t\tPropertyTemplate: \"FbxNode\" {");
            sb.AppendLine("\t\t\tProperties70:  {");
            sb.AppendLine("\t\t\t\tP: \"QuaternionInterpolate\", \"enum\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"RotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"ScalingOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"ScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"TranslationActive\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMinX\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMinY\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMinZ\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMaxX\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMaxY\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"TranslationMaxZ\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationSpaceForLimitOnly\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationStiffnessX\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationStiffnessY\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationStiffnessZ\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"AxisLen\", \"double\", \"Number\", \"\",10");
            sb.AppendLine("\t\t\t\tP: \"PreRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"PostRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"RotationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"RotationMinX\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationMinY\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationMinZ\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationMaxX\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationMaxY\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"RotationMaxZ\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"ScalingActive\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"ScalingMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",1,1,1");
            sb.AppendLine("\t\t\t\tP: \"ScalingMinX\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"ScalingMinY\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"ScalingMinZ\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"ScalingMaxX\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"ScalingMaxY\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"ScalingMaxZ\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"GeometricTranslation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"GeometricRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"GeometricScaling\", \"Vector3D\", \"Vector\", \"\",1,1,1");
            sb.AppendLine("\t\t\t\tP: \"MinDampRangeX\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MinDampRangeY\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MinDampRangeZ\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MaxDampRangeX\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MaxDampRangeY\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MaxDampRangeZ\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MinDampStrengthX\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MinDampStrengthY\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MinDampStrengthZ\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthX\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthY\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthZ\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"PreferedAngleX\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"PreferedAngleY\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"PreferedAngleZ\", \"double\", \"Number\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"LookAtProperty\", \"object\", \"\", \"\"");
            sb.AppendLine("\t\t\t\tP: \"UpVectorProperty\", \"object\", \"\", \"\"");
            sb.AppendLine("\t\t\t\tP: \"Show\", \"bool\", \"\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"NegativePercentShapeSupport\", \"bool\", \"\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",-1");
            sb.AppendLine("\t\t\t\tP: \"Freeze\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"LODBox\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",1,1,1");
            sb.AppendLine("\t\t\t\tP: \"Visibility\", \"Visibility\", \"\", \"A\",1");
            sb.AppendLine("\t\t\t\tP: \"Visibility Inheritance\", \"Visibility Inheritance\", \"\", \"\",1");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");

            // The geometry, this is IMPORTANT
            sb.AppendLine("\tObjectType: \"Geometry\" {");
            sb.AppendLine("\t\tCount: 1"); // TODO - this must be set by the number of items being placed.
            sb.AppendLine("\t\tPropertyTemplate: \"FbxMesh\" {");
            sb.AppendLine("\t\t\tProperties70:  {");
            sb.AppendLine("\t\t\t\tP: \"Color\", \"ColorRGB\", \"Color\", \"\",0.8,0.8,0.8");
            sb.AppendLine("\t\t\t\tP: \"BBoxMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"BBoxMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"Primary Visibility\", \"bool\", \"\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"Casts Shadows\", \"bool\", \"\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"Receive Shadows\", \"bool\", \"\", \"\",1");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");

            // The materials that are being placed. Has to be simple I think
            sb.AppendLine("\tObjectType: \"Material\" {");
            sb.AppendLine("\t\tCount: 1");
            sb.AppendLine("\t\tPropertyTemplate: \"FbxSurfacePhong\" {");
            sb.AppendLine("\t\t\tProperties70:  {");
            sb.AppendLine("\t\t\t\tP: \"ShadingModel\", \"KString\", \"\", \"\", \"Phong\"");
            sb.AppendLine("\t\t\t\tP: \"MultiLayer\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"EmissiveColor\", \"Color\", \"\", \"A\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"EmissiveFactor\", \"Number\", \"\", \"A\",1");
            sb.AppendLine("\t\t\t\tP: \"AmbientColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
            sb.AppendLine("\t\t\t\tP: \"AmbientFactor\", \"Number\", \"\", \"A\",1");
            sb.AppendLine("\t\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",0.8,0.8,0.8");
            sb.AppendLine("\t\t\t\tP: \"DiffuseFactor\", \"Number\", \"\", \"A\",1");
            sb.AppendLine("\t\t\t\tP: \"Bump\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"NormalMap\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"BumpFactor\", \"double\", \"Number\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"TransparencyFactor\", \"Number\", \"\", \"A\",0");
            sb.AppendLine("\t\t\t\tP: \"DisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"DisplacementFactor\", \"double\", \"Number\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"VectorDisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"VectorDisplacementFactor\", \"double\", \"Number\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"SpecularColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
            sb.AppendLine("\t\t\t\tP: \"SpecularFactor\", \"Number\", \"\", \"A\",1");
            sb.AppendLine("\t\t\t\tP: \"ShininessExponent\", \"Number\", \"\", \"A\",20");
            sb.AppendLine("\t\t\t\tP: \"ReflectionColor\", \"Color\", \"\", \"A\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"ReflectionFactor\", \"Number\", \"\", \"A\",1");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");

            // Explanation of how textures work
            sb.AppendLine("\tObjectType: \"Texture\" {");
            sb.AppendLine("\t\tCount: 2"); // TODO - figure out if this texture number is important
            sb.AppendLine("\t\tPropertyTemplate: \"FbxFileTexture\" {");
            sb.AppendLine("\t\t\tProperties70:  {");
            sb.AppendLine("\t\t\t\tP: \"TextureTypeUse\", \"enum\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"Texture alpha\", \"Number\", \"\", \"A\",1");
            sb.AppendLine("\t\t\t\tP: \"CurrentMappingType\", \"enum\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"WrapModeU\", \"enum\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"WrapModeV\", \"enum\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"UVSwap\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"PremultiplyAlpha\", \"bool\", \"\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"Translation\", \"Vector\", \"\", \"A\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"Rotation\", \"Vector\", \"\", \"A\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"Scaling\", \"Vector\", \"\", \"A\",1,1,1");
            sb.AppendLine("\t\t\t\tP: \"TextureRotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"TextureScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            sb.AppendLine("\t\t\t\tP: \"CurrentTextureBlendMode\", \"enum\", \"\", \"\",1");
            sb.AppendLine("\t\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"default\"");
            sb.AppendLine("\t\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t\tP: \"UseMipMap\", \"bool\", \"\", \"\",0");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");

            sb.AppendLine("}");
            sb.AppendLine("");

            sb.Append(objectProps.ToString());
            sb.Append(objectConnections.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Gets all the meshes and outputs to a string (even grabbing the child of each gameObject)
        /// </summary>
        /// <returns>The mesh to string.</returns>
        public long GetMeshToString(Mesh gameObj,
                                           Material[] materials,
                                           ref StringBuilder objects,
                                           ref StringBuilder connections,
                                           GameObject parentObject = null,
                                           long parentModelId = 0)
        {
            StringBuilder tempObjectSb = new StringBuilder();
            StringBuilder tempConnectionsSb = new StringBuilder();

            long geometryId = GetRandomFBXId();
            long modelId    = GetRandomFBXId();
            string meshName = gameObj.name;

            // A NULL parent means that the gameObject is at the top
            string isMesh = "Null";

            //if (filter != null)
            //{
            //    meshName = filter.sharedMesh.name;
            isMesh = "Mesh";
            //}

            if (parentModelId == 0)
                tempConnectionsSb.AppendLine("\t;Model::" + meshName + ", Model::RootNode");
            else
                tempConnectionsSb.AppendLine("\t;Model::" + meshName + ", Model::USING PARENT");
            tempConnectionsSb.AppendLine("\tC: \"OO\"," + modelId + "," + parentModelId);
            tempConnectionsSb.AppendLine();
            tempObjectSb.AppendLine("\tModel: " + modelId + ", \"Model::" + gameObj.name + "\", \"" + isMesh + "\" {");
            tempObjectSb.AppendLine("\t\tVersion: 232");
            tempObjectSb.AppendLine("\t\tProperties70:  {");
            tempObjectSb.AppendLine("\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",4");
            tempObjectSb.AppendLine("\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",1");
            tempObjectSb.AppendLine("\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",1");
            tempObjectSb.AppendLine("\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            tempObjectSb.AppendLine("\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
            // ===== Local Translation Offset =========
            //Vector3 position = gameObj.transform.localPosition;
            Vector3 position = new Vector3(0, 0, 0);

            tempObjectSb.Append("\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A+\",");

            // Append the X Y Z coords to the system
            tempObjectSb.AppendFormat("{0},{1},{2}", position.x * -1, position.y, position.z);
            tempObjectSb.AppendLine();

            // Rotates the object correctly from Unity space
            Vector3 localRotation = new Vector3();//gameObj.transform.localEulerAngles;
            tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A+\",{0},{1},{2}", localRotation.x, localRotation.y * -1, -1 * localRotation.z);
            tempObjectSb.AppendLine();

            // Adds the local scale of this object
            //Vector3 localScale = gameObj.transform.localScale;
            tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",{0},{1},{2}", 1, 1, 1);
            tempObjectSb.AppendLine();

            tempObjectSb.AppendLine("\t\t\tP: \"currentUVSet\", \"KString\", \"\", \"U\", \"map1\"");
            tempObjectSb.AppendLine("\t\t}");
            tempObjectSb.AppendLine("\t\tShading: T");
            tempObjectSb.AppendLine("\t\tCulling: \"CullingOff\"");
            tempObjectSb.AppendLine("\t}");


            // Adds in geometry if it exists, if it it does not exist, this is a empty gameObject file and skips over this
            if (gameObj != null)
            {
                Mesh mesh = gameObj;

                // =================================
                //         General Geometry Info
                // =================================
                // Generate the geometry information for the mesh created

                tempObjectSb.AppendLine("\tGeometry: " + geometryId + ", \"Geometry::\", \"Mesh\" {");

                // ===== WRITE THE VERTICIES =====
                Vector3[] verticies = mesh.vertices;
                int vertCount = mesh.vertexCount * 3; // <= because the list of points is just a list of comma seperated values, we need to multiply by three

                tempObjectSb.AppendLine("\t\tVertices: *" + vertCount + " {");
                tempObjectSb.Append("\t\t\ta: ");
                for (int i = 0; i < verticies.Length; i++)
                {
                    if (i > 0)
                        tempObjectSb.Append(",");

                    // Points in the verticies. We also reverse the x value because Unity has a reverse X coordinate
                    tempObjectSb.AppendFormat("{0},{1},{2}", verticies[i].x * -1, verticies[i].y, verticies[i].z);
                }

                tempObjectSb.AppendLine();
                tempObjectSb.AppendLine("\t\t} ");

                // ======= WRITE THE TRIANGLES ========
                int triangleCount = mesh.triangles.Length;
                int[] triangles = mesh.triangles;

                tempObjectSb.AppendLine("\t\tPolygonVertexIndex: *" + triangleCount + " {");

                // Write triangle indexes
                tempObjectSb.Append("\t\t\ta: ");
                for (int i = 0; i < triangleCount; i += 3)
                {
                    if (i > 0)
                        tempObjectSb.Append(",");

                    // To get the correct normals, must rewind the triangles since we flipped the x direction
                    tempObjectSb.AppendFormat("{0},{1},{2}",
                                              triangles[i],
                                              triangles[i + 2],
                                              (triangles[i + 1] * -1) - 1); // <= Tells the poly is ended

                }

                tempObjectSb.AppendLine();

                tempObjectSb.AppendLine("\t\t} ");
                tempObjectSb.AppendLine("\t\tGeometryVersion: 124");
                tempObjectSb.AppendLine("\t\tLayerElementNormal: 0 {");
                tempObjectSb.AppendLine("\t\t\tVersion: 101");
                tempObjectSb.AppendLine("\t\t\tName: \"\"");
                tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"Direct\"");

                // ===== WRITE THE NORMALS ==========
                bool containsNormals = mesh.normals.Length == verticies.Length;
                if (containsNormals)
                {
                    Vector3[] normals = mesh.normals;

                    tempObjectSb.AppendLine("\t\t\tNormals: *" + (triangleCount * 3) + " {");
                    tempObjectSb.Append("\t\t\t\ta: ");

                    for (int i = 0; i < triangleCount; i += 3)
                    {
                        if (i > 0)
                            tempObjectSb.Append(",");

                        // To get the correct normals, must rewind the normal triangles like the triangles above since x was flipped
                        Vector3 newNormal = normals[triangles[i]];

                        tempObjectSb.AppendFormat("{0},{1},{2},",
                                                 newNormal.x * -1, // Switch normal as is tradition
                                                 newNormal.y,
                                                 newNormal.z);

                        newNormal = normals[triangles[i + 2]];

                        tempObjectSb.AppendFormat("{0},{1},{2},",
                                                  newNormal.x * -1, // Switch normal as is tradition
                                                  newNormal.y,
                                                  newNormal.z);

                        newNormal = normals[triangles[i + 1]];

                        tempObjectSb.AppendFormat("{0},{1},{2}",
                                                  newNormal.x * -1, // Switch normal as is tradition
                                                  newNormal.y,
                                                  newNormal.z);
                    }

                    tempObjectSb.AppendLine();
                    tempObjectSb.AppendLine("\t\t\t}");
                }

                tempObjectSb.AppendLine("\t\t}");

                // ===== WRITE THE COLORS =====
                bool containsColors = mesh.colors.Length == verticies.Length;

                if (containsColors)
                {
                    Color[] colors = mesh.colors;

                    Dictionary<Color, int> colorTable = new Dictionary<Color, int>(); // reducing amount of data by only keeping unique colors.
                    int idx = 0;

                    // build index table of all the different colors present in the mesh            
                    for (int i = 0; i < colors.Length; i++)
                    {
                        if (!colorTable.ContainsKey(colors[i]))
                        {
                            colorTable[colors[i]] = idx;
                            idx++;
                        }
                    }

                    tempObjectSb.AppendLine("\t\tLayerElementColor: 0 {");
                    tempObjectSb.AppendLine("\t\t\tVersion: 101");
                    tempObjectSb.AppendLine("\t\t\tName: \"Col\"");
                    tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                    tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
                    tempObjectSb.AppendLine("\t\t\tColors: *" + colorTable.Count * 4 + " {");
                    tempObjectSb.Append("\t\t\t\ta: ");

                    bool first = true;
                    foreach (KeyValuePair<Color, int> color in colorTable)
                    {
                        if (!first)
                            tempObjectSb.Append(",");

                        tempObjectSb.AppendFormat("{0},{1},{2},{3}", color.Key.r, color.Key.g, color.Key.b, color.Key.a);
                        first = false;
                    }
                    tempObjectSb.AppendLine();

                    tempObjectSb.AppendLine("\t\t\t\t}");

                    // Color index
                    tempObjectSb.AppendLine("\t\t\tColorIndex: *" + triangles.Length + " {");
                    tempObjectSb.Append("\t\t\t\ta: ");

                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        if (i > 0)
                            tempObjectSb.Append(",");

                        // Triangles need to be fliped for the x flip
                        int index1 = triangles[i];
                        int index2 = triangles[i + 2];
                        int index3 = triangles[i + 1];

                        // Find the color index related to that vertice index
                        index1 = colorTable[colors[index1]];
                        index2 = colorTable[colors[index2]];
                        index3 = colorTable[colors[index3]];

                        tempObjectSb.AppendFormat("{0},{1},{2}", index1, index2, index3);
                    }

                    tempObjectSb.AppendLine();

                    tempObjectSb.AppendLine("\t\t\t}");
                    tempObjectSb.AppendLine("\t\t}");
                }
                else
                    Debug.LogWarning("Mesh contains " + mesh.vertices.Length + " vertices for " + mesh.colors.Length + " colors. Skip color export");



                // ================ UV CREATION =========================

                // -- UV 1 Creation
                int uvLength = mesh.uv.Length;
                Vector2[] uvs = mesh.uv;

                tempObjectSb.AppendLine("\t\tLayerElementUV: 0 {"); // the Zero here is for the first UV map
                tempObjectSb.AppendLine("\t\t\tVersion: 101");
                tempObjectSb.AppendLine("\t\t\tName: \"map1\"");
                tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
                tempObjectSb.AppendLine("\t\t\tUV: *" + uvLength * 2 + " {");
                tempObjectSb.Append("\t\t\t\ta: ");

                for (int i = 0; i < uvLength; i++)
                {
                    if (i > 0)
                        tempObjectSb.Append(",");

                    tempObjectSb.AppendFormat("{0},{1}", uvs[i].x, uvs[i].y);

                }
                tempObjectSb.AppendLine();

                tempObjectSb.AppendLine("\t\t\t\t}");

                // UV tile index coords
                tempObjectSb.AppendLine("\t\t\tUVIndex: *" + triangleCount + " {");
                tempObjectSb.Append("\t\t\t\ta: ");

                for (int i = 0; i < triangleCount; i += 3)
                {
                    if (i > 0)
                        tempObjectSb.Append(",");

                    // Triangles need to be fliped for the x flip
                    int index1 = triangles[i];
                    int index2 = triangles[i + 2];
                    int index3 = triangles[i + 1];

                    tempObjectSb.AppendFormat("{0},{1},{2}", index1, index2, index3);
                }

                tempObjectSb.AppendLine();

                tempObjectSb.AppendLine("\t\t\t}");
                tempObjectSb.AppendLine("\t\t}");

                // -- UV 2 Creation
                if (mesh.uv2.Length != 0)
                {
                    uvLength = mesh.uv2.Length;
                    uvs = mesh.uv2;

                    tempObjectSb.AppendLine("\t\tLayerElementUV: 1 {"); // the Zero here is for the first UV map
                    tempObjectSb.AppendLine("\t\t\tVersion: 101");
                    tempObjectSb.AppendLine("\t\t\tName: \"map2\"");
                    tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                    tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
                    tempObjectSb.AppendLine("\t\t\tUV: *" + uvLength * 2 + " {");
                    tempObjectSb.Append("\t\t\t\ta: ");

                    for (int i = 0; i < uvLength; i++)
                    {
                        if (i > 0)
                            tempObjectSb.Append(",");

                        tempObjectSb.AppendFormat("{0},{1}", uvs[i].x, uvs[i].y);

                    }
                    tempObjectSb.AppendLine();

                    tempObjectSb.AppendLine("\t\t\t\t}");

                    // UV tile index coords
                    tempObjectSb.AppendLine("\t\t\tUVIndex: *" + triangleCount + " {");
                    tempObjectSb.Append("\t\t\t\ta: ");

                    for (int i = 0; i < triangleCount; i += 3)
                    {
                        if (i > 0)
                            tempObjectSb.Append(",");

                        // Triangles need to be fliped for the x flip
                        int index1 = triangles[i];
                        int index2 = triangles[i + 2];
                        int index3 = triangles[i + 1];

                        tempObjectSb.AppendFormat("{0},{1},{2}", index1, index2, index3);
                    }

                    tempObjectSb.AppendLine();

                    tempObjectSb.AppendLine("\t\t\t}");
                    tempObjectSb.AppendLine("\t\t}");
                }
                // -- Smoothing
                // TODO: Smoothing doesn't seem to do anything when importing. This maybe should be added. -KBH

                // ============ MATERIALS =============

                tempObjectSb.AppendLine("\t\tLayerElementMaterial: 0 {");
                tempObjectSb.AppendLine("\t\t\tVersion: 101");
                tempObjectSb.AppendLine("\t\t\tName: \"\"");
                tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygon\"");
                tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");

                int totalFaceCount = 0;

                // So by polygon means that we need 1/3rd of how many indicies we wrote.
                int numberOfSubmeshes = mesh.subMeshCount;

                StringBuilder submeshesSb = new StringBuilder();

                // For just one submesh, we set them all to zero
                if (numberOfSubmeshes == 1)
                {
                    int numFaces = triangles.Length / 3;

                    for (int i = 0; i < numFaces; i++)
                    {
                        submeshesSb.Append("0,");
                        totalFaceCount++;
                    }
                }
                else
                {
                    List<int[]> allSubmeshes = new List<int[]>();

                    // Load all submeshes into a space
                    for (int i = 0; i < numberOfSubmeshes; i++)
                        allSubmeshes.Add(mesh.GetIndices(i));

                    // TODO: Optimize this search pattern
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        for (int subMeshIndex = 0; subMeshIndex < allSubmeshes.Count; subMeshIndex++)
                        {
                            bool breaker = false;

                            for (int n = 0; n < allSubmeshes[subMeshIndex].Length; n += 3)
                            {
                                if (triangles[i] == allSubmeshes[subMeshIndex][n]
                                   && triangles[i + 1] == allSubmeshes[subMeshIndex][n + 1]
                                   && triangles[i + 2] == allSubmeshes[subMeshIndex][n + 2])
                                {
                                    submeshesSb.Append(subMeshIndex.ToString());
                                    submeshesSb.Append(",");
                                    totalFaceCount++;
                                    break;
                                }

                                if (breaker)
                                    break;
                            }
                        }
                    }
                }

                tempObjectSb.AppendLine("\t\t\tMaterials: *" + totalFaceCount + " {");
                tempObjectSb.Append("\t\t\t\ta: ");
                tempObjectSb.AppendLine(submeshesSb.ToString());
                tempObjectSb.AppendLine("\t\t\t} ");
                tempObjectSb.AppendLine("\t\t}");

                // ============= INFORMS WHAT TYPE OF LATER ELEMENTS ARE IN THIS GEOMETRY =================
                tempObjectSb.AppendLine("\t\tLayer: 0 {");
                tempObjectSb.AppendLine("\t\t\tVersion: 100");
                tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
                tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementNormal\"");
                tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
                tempObjectSb.AppendLine("\t\t\t}");
                tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
                tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementMaterial\"");
                tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
                tempObjectSb.AppendLine("\t\t\t}");
                tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
                tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementTexture\"");
                tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
                tempObjectSb.AppendLine("\t\t\t}");
                if (containsColors)
                {
                    tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
                    tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementColor\"");
                    tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
                    tempObjectSb.AppendLine("\t\t\t}");
                }
                tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
                tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
                tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
                tempObjectSb.AppendLine("\t\t\t}");
                if (mesh.uv2.Length != 0)
                {
                    tempObjectSb.AppendLine("\t\t}");
                    tempObjectSb.AppendLine("\t\tLayer: 1 {");
                    tempObjectSb.AppendLine("\t\t\tVersion: 100");
                    tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
                    tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
                    tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 1");
                    tempObjectSb.AppendLine("\t\t\t}");
                }

                tempObjectSb.AppendLine("\t\t}");
                tempObjectSb.AppendLine("\t}");

                // Add the connection for the model to the geometry so it is attached the right mesh
                tempConnectionsSb.AppendLine("\t;Geometry::, Model::" + mesh.name);
                tempConnectionsSb.AppendLine("\tC: \"OO\"," + geometryId + "," + modelId);
                tempConnectionsSb.AppendLine();
            }

            objects.Append(tempObjectSb.ToString());
            connections.Append(tempConnectionsSb.ToString());

            return modelId;
        }

        public static long GetRandomFBXId()
        {
            return System.BitConverter.ToInt64(System.Guid.NewGuid().ToByteArray(), 0);
        }

        public static string VersionInformation
        {
            get { return "FBX Unity Export version 1.1.1 (Originally created for the Unity Asset, Building Crafter)"; }
        }
    }
}
