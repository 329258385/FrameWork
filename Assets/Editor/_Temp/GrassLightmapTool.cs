using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

    public class TerrainInfo
    {
        public const string COPY_NAME = "_copyed";

        Dictionary<Material, List<MeshRenderer>> materialsList = new Dictionary<Material, List<MeshRenderer>>();
        public Vector4 LightmapScaleOffset = Vector4.zero;
        public Vector3 Max = Vector3.one * float.MinValue;
        public Vector3 Min = Vector3.one * float.MaxValue;
        public Vector2 MaxUV = Vector2.zero;
        public Vector2 MinUV = Vector2.zero;
        public Vector4 DeltaUV = Vector4.zero;
        public Vector4 Position = Vector4.zero;

        public bool Include(Transform grass)
        {
            if (grass.position.x > Min.x && grass.position.x < Max.x && grass.position.z > Min.z && grass.position.z < Max.z)
            {
                return true;
            }
            return false;
        }

        public void AddGrass(MeshRenderer render)
        {
            if (render.lightmapIndex == int.MaxValue)
                return;
            List<MeshRenderer> list = null;
        if (render.sharedMaterial != null)
        {
            if (!materialsList.TryGetValue(render.sharedMaterial, out list))
            {
                list = new List<MeshRenderer>();
                materialsList.Add(render.sharedMaterial, list);
            }
            list.Add(render);
        }

    }

        public static string GetCopyedName(string path)
        {
            if (path == null || path.Equals(string.Empty))
                return null;
            int index = path.LastIndexOf('.');
            int indexName = path.LastIndexOf('/');
            string postfix = path.Substring(index);
            string name = path.Substring(indexName + 1, index - indexName - 1);
            string root = SceneManager.GetActiveScene().path;
            indexName = root.LastIndexOf('/');
            root = root.Substring(0, indexName);
            string newPath = null;
            int i = 1;
            while (true)
            {
                newPath = string.Format("{0}/{1}{2}{3:D4}{4}", root, name, COPY_NAME, i, postfix);
                if (!File.Exists(newPath))
                {
                    break;
                }
                i++;
            }
            return newPath;
        }

        public void ProcessGrassMaterials()
        {
            Dictionary<Material, List<MeshRenderer>>.Enumerator e = materialsList.GetEnumerator();
            while (e.MoveNext())
            {
                Material mat = e.Current.Key;
                if (!e.Current.Key.name.Contains(COPY_NAME))
                {
                    string path = AssetDatabase.GetAssetPath(e.Current.Key);
                    string newPath = GetCopyedName(path);
                    Material newMaterial = null;
                    if (AssetDatabase.CopyAsset(path, newPath))
                    {
                        newMaterial = AssetDatabase.LoadAssetAtPath(newPath, typeof(Material)) as Material;
                    }
                    if (newMaterial != null)
                    {
                        foreach (MeshRenderer r in e.Current.Value)
                        {
                            r.sharedMaterial = newMaterial;
                        }
                        mat = newMaterial;
                    }
                }

                //mat.SetVector("_LightmapParam", LightmapScaleOffset);
                //mat.SetVector("_LightmapPosition", new Vector4(Max.x, Max.z, Min.x, Min.z));
                //mat.SetVector("_LightmapUVOffset", new Vector4(0, 0, DeltaUV.z, DeltaUV.w));
            }
        }
    }

    public class GrassLightmapTool : EditorWindow
    {
        public const string COPY_NAME = "_copyed";
        public static string[] TERRAIN_ROOTS = new string[]
        {
            "root/terrain", "root/scene/terrain", "root/battle/terrain"
        };

        public static string[] GRASS_ROOTS = new string[]
        {
            "root/grassa", "root/scene/grassa", "root/battle/grassa"
        };

        static GameObject target = null;

        static Dictionary<Transform, TerrainInfo> terrainList = new Dictionary<Transform, TerrainInfo>();

        [MenuItem("Menu/Lightmap/GetMeshLightmapInfoNew")]
        public static void GetMeshLightmapInfo()
        {
            GrassLightmapTool win = EditorWindow.GetWindow<GrassLightmapTool>();

            ProcessTerrainInfo();
        }

        public static void ProcessTerrainInfo()
        {
            terrainList.Clear();
            foreach (string root in TERRAIN_ROOTS)
            {
                GameObject troot = GameObject.Find(root);
                if (troot != null)
                {
                    MeshRenderer[] terrains = troot.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer render in terrains)
                    {
                        if (render.gameObject.activeSelf && render.lightmapIndex != int.MaxValue)
                        {
                            DoProcessTerrainInfo(render);
                        }
                    }
                }
            }
        }

        [MenuItem("Menu/Lightmap/GrassLightmapToolNew")]
        public static void DoGrassLightmapTool()
        {
            ProcessTerrainInfo();
            ProcessGrass();
        }

        public static void DoProcessTerrainInfo(MeshRenderer render)
        {
            TerrainInfo terrainInfo = new TerrainInfo();
            Vector3 Max = Vector3.one * float.MinValue;
            Vector3 Min = Vector3.one * float.MaxValue;
            Vector2 MaxUV = Vector2.zero;
            Vector2 MinUV = Vector2.zero;

            MeshFilter filter = render.gameObject.GetComponent<MeshFilter>();
            if (filter == null)
                return;
            Mesh mesh = filter.sharedMesh;
            if (mesh == null)
                return;

            List<Vector3> vectors = new List<Vector3>();
            List<Vector2> uvs2 = new List<Vector2>();
            mesh.GetVertices(vectors);
            mesh.GetUVs(1, uvs2);

            for (int i = 0; i < vectors.Count; i++)
            {
                Vector3 v = vectors[i];
                if (v.x > Max.x)
                {
                    Max.x = v.x;
                    MaxUV.x = uvs2[i].x;
                }
                if (v.x < Min.x)
                {
                    Min.x = v.x;
                    MinUV.x = uvs2[i].x;
                }
                if (v.z > Max.z)
                {
                    Max.z = v.z;
                    MaxUV.y = uvs2[i].y;
                }
                if (v.z < Min.z)
                {
                    Min.z = v.z;
                    MinUV.y = uvs2[i].y;
                }
            }

            Max.y = 0;
            Min.y = 0;
            terrainInfo.Max = render.transform.TransformPoint(Max);
            terrainInfo.Min = render.transform.TransformPoint(Min);
            terrainInfo.Position = new Vector4(terrainInfo.Max.x, terrainInfo.Max.z, terrainInfo.Min.x, terrainInfo.Min.z);
            terrainInfo.LightmapScaleOffset = render.lightmapScaleOffset;
            terrainInfo.DeltaUV = new Vector4(0, 0, Mathf.Abs(MaxUV.x - MinUV.x), Mathf.Abs(MaxUV.y - MinUV.y));

            terrainList[render.transform] = terrainInfo;
        }

        public void OnGUI()
        {
            Dictionary<Transform, TerrainInfo>.Enumerator e = terrainList.GetEnumerator();
            EditorGUILayout.BeginScrollView(Vector2.zero);
            while (e.MoveNext())
            {
                TerrainInfo info = e.Current.Value;
                EditorGUILayout.LabelField(e.Current.Key.name);
                EditorGUILayout.ObjectField(e.Current.Key.gameObject as Object, typeof(Object));
                EditorGUILayout.Vector4Field("LightmapScaleOffset", info.LightmapScaleOffset);
                EditorGUILayout.Vector4Field("Position", info.Position);
                EditorGUILayout.Vector4Field("Delta UV", info.DeltaUV);
            }
            EditorGUILayout.EndScrollView();
        }

        public static void ProcessGrass()
        {
            foreach (string rootName in GRASS_ROOTS)
            {
                GameObject grassRoot = GameObject.Find(rootName);
                if (grassRoot != null)
                {
                    MeshRenderer[] renders = grassRoot.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer render in renders)
                    {
                        if (render.lightmapIndex == int.MaxValue)
                            continue;
                        Dictionary<Transform, TerrainInfo>.Enumerator e = terrainList.GetEnumerator();
                        while (e.MoveNext())
                        {
                            if (e.Current.Value.Include(render.transform))
                            {
                                e.Current.Value.AddGrass(render);
                                break;
                            }
                        }
                    }
                }
            }

            Dictionary<Transform, TerrainInfo>.Enumerator et = terrainList.GetEnumerator();
            while (et.MoveNext())
            {
                et.Current.Value.ProcessGrassMaterials();
            }
        }
    }
