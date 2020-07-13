using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;

public class CenterRoadWnd : EditorWindow {
    public enum RoadMeshType
    {
        /// <summary>
        /// 
        /// </summary>
        ROAD_SINGLE,
        /// <summary>
        /// 
        /// </summary>
        ROAD_BATCH,
    }
    public enum RoadMeshHeightType
    {
        /// <summary>
        /// 以场景的navigation层作为高度
        /// </summary>
        MESH_HEIGHT_NAVIGATION = 1,
        /// <summary>
        /// 以场景的玩家碰撞层作为高度
        /// </summary>
        MESH_HEIGHT_GROUND_LAYER = 2,
        /// <summary>
        /// 直接以点的插值作为高度
        /// </summary>
        MESH_HEIGHT_POINT = 4,
    }
    [MenuItem("Tools/路点/中间道路设置")]
    static void Open()
    {
        EditorWindow.GetWindow<CenterRoadWnd>();
    }
    public float roadWidth = 0.5f;
    public float roadHeight = 0f;
    public int lineCount = 4;
    public RoadMeshType roadType = RoadMeshType.ROAD_BATCH;
    public RoadMeshHeightType roadHeightType = RoadMeshHeightType.MESH_HEIGHT_NAVIGATION;
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        roadWidth = EditorGUILayout.FloatField("路宽度", roadWidth);
        roadHeight = EditorGUILayout.FloatField("路整体高度", roadHeight);
        lineCount = EditorGUILayout.IntField("点个数", lineCount);
        roadType = (RoadMeshType)EditorGUILayout.EnumPopup("mesh的创建方法", roadType);
        roadHeightType = (RoadMeshHeightType)EditorGUILayout.EnumPopup("mesh的高度获得方式", roadHeightType);
        if (GUILayout.Button("查找出场景Navigation烘焙的mesh", GUILayout.Height(80)))
        {
            AStarHelpEditor.FindNavigationMeshAndInstantiate();
        }
        if (GUILayout.Button("创建中心路mesh", GUILayout.Height(80)))
        {
            CreateNodeMesh();
        }
        if (GUILayout.Button("直接创建mesh",GUILayout.Height(80)))
        {
            AStarHelpEditor.FindNavigationMeshAndInstantiate();
            CreateNodeMesh();
        }
        EditorGUILayout.EndVertical();
    }


    void CreateNodeMesh()
    {
        //CopyAllNavigation(FindAllNavigationMesh());
        AstarPath aStarPath = GameObject.FindObjectOfType<AstarPath>();
        if (aStarPath != null)
        {
            Transform nodeRoot = aStarPath.transform.Find("NodeRoot");
            if (nodeRoot)
            {
                int count = 1;
                //List<Vector3> offsets = new List<Vector3>() { Vector3.zero, -Vector3.left * 0.5f, Vector3.left * 0.5f, -Vector3.forward * 0.5f, Vector3.forward * 0.5f, };
                List<float> offsets = new List<float>() { 0, -0.3f, 0.3f, -0.6f, 0.6f };
                for (int j = 0; j < count; j++)
                {
                    GameObject old = GameObject.Find("road_layer" + j.ToString());
                    if (old != null) { GameObject.DestroyImmediate(old); }

                    GameObject road_layer = new GameObject("road_layer" + j.ToString());

                    NodeLink[] nodeLinks = nodeRoot.GetComponentsInChildren<NodeLink>();
                    switch(roadType)
                    {
                        case RoadMeshType.ROAD_BATCH:
                            {
                                int maxVerticesCount = 10000;
                                List<Vector3> vertices = new List<Vector3>();
                                List<int> triangles = new List<int>();
                                for (int i = 0; i < nodeLinks.Length; i++)
                                {
                                    if (nodeLinks[i].Start != null && nodeLinks[i].End != null)
                                    {
                                        Vector3 normal = Vector3.Cross((nodeLinks[i].End.position - nodeLinks[i].Start.position), Vector3.up).normalized;
                                        AStarHelpEditor.AddOneMesh(nodeLinks[i].Start.position + offsets[j] * normal, nodeLinks[i].End.position + offsets[j] * normal,
                                            roadWidth,
                                            lineCount,
                                            vertices,
                                            triangles,
                                            roadHeightType);
                                        if (i == nodeLinks.Length - 1 || vertices.Count > maxVerticesCount)
                                        {
                                            GameObject road = AStarHelpEditor.CreateMesh(vertices, triangles);
                                            road.transform.parent = road_layer.transform;
                                            GameObjectUtility.SetStaticEditorFlags(road, StaticEditorFlags.NavigationStatic);
                                            GameObjectUtility.SetNavMeshArea(road, 3 + j);
                                            vertices.Clear();
                                            triangles.Clear();
                                        }
                                    }
                                }
                                break;
                            }
                        case RoadMeshType.ROAD_SINGLE:
                            {
                                //单独创建mesh
                                for (int i = 0; i < nodeLinks.Length; i++)
                                {
                                    if (nodeLinks[i].Start != null && nodeLinks[i].End != null)
                                    {
                                        Vector3 normal = Vector3.Cross((nodeLinks[i].End.position - nodeLinks[i].Start.position), Vector3.up).normalized;

                                        GameObject road = AStarHelpEditor.CreateMesh(nodeLinks[i].Start.position + offsets[j] * normal,
                                            nodeLinks[i].End.position + offsets[j] * normal,
                                            roadWidth,
                                            lineCount,
                                            roadHeightType);
                                        road.transform.parent = road_layer.transform;
                                        GameObjectUtility.SetStaticEditorFlags(road, StaticEditorFlags.NavigationStatic);
                                        GameObjectUtility.SetNavMeshArea(road, 3 + j);
                                    }
                                }

                                break;
                            }
                    }

                    road_layer.transform.position = new Vector3(0, roadHeight, 0);
                }
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

}
