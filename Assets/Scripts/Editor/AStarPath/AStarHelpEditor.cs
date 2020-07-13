using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using ActClient;

public class AStarHelpEditor : Editor {
    [MenuItem("Tools/路点/增加路点 &c")]
    static void AddScenePointNode()
    {
        AstarPath aStarPath = GameObject.FindObjectOfType<AstarPath>();
        if (aStarPath != null)
        {
            Selection.activeGameObject = aStarPath.gameObject;
            EditorUtility.DisplayDialog("注意", "当前场景已经有路点节点", "关闭");
        }
        else
        {
            GameObject go = new GameObject("AStarPath");
            GameObject pointRoot = new GameObject("NodeRoot");
            pointRoot.transform.parent = go.transform;

            GameObject node0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            node0.name = "pathNode";
            node0.layer = LayerMask.NameToLayer("Scenes_mesh");
            node0.transform.parent = pointRoot.transform;
            Collider coll = node0.GetComponent<Collider>();
            if (coll)
            {
                GameObject.DestroyImmediate(coll);
            }
            MeshFilter filter = node0.GetComponent<MeshFilter>();
            if (filter)
            {
                GameObject.DestroyImmediate(filter);
            }
            Renderer ren = node0.GetComponent<Renderer>();
            if (ren)
            {
                GameObject.DestroyImmediate(ren);
            }

            go.AddComponent<AstarPathGo>();
            aStarPath = go.AddComponent<AstarPath>();
            aStarPath.colorSettings = aStarPath.colorSettings ?? new AstarColor();

            aStarPath.data = new AstarData();

            PointGraph graph = System.Activator.CreateInstance(typeof(PointGraph)) as PointGraph;
            graph.root = pointRoot.transform;
            graph.maxDistance = 1;
            graph.raycast = true;
            graph.optimizeForSparseGraph = true;
            graph.mask = 1 << LayerMask.NameToLayer("Scenes_mesh");

            graph.active = aStarPath;
            aStarPath.data.graphs = new NavGraph[1];
            aStarPath.data.graphs[0] = graph;

            NodeAttachFloor();
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/设置两个路点失效 &q")]
    static void DeleteNode()
    {
        if (Selection.gameObjects.Length == 2)
        {
            bool delete = false;
            {
                NodeLink[] links = Selection.gameObjects[0].GetComponents<NodeLink>();
                for (int i = 0; i < links.Length; i++)
                {
                    if (links[i].End == Selection.gameObjects[1].transform)
                    {
                        delete = true;
                        GameObject.DestroyImmediate(links[i]);
                    }
                }
            }
            {
                NodeLink[] links = Selection.gameObjects[1].GetComponents<NodeLink>();
                for (int i = 0; i < links.Length; i++)
                {
                    if (links[i].End == Selection.gameObjects[0].transform)
                    {
                        delete = true;
                        GameObject.DestroyImmediate(links[i]);
                    }
                }
            }
            //if (!delete)
            //{
            //    NodeLink link = Selection.gameObjects[0].AddComponent<NodeLink>();
            //    link.end = Selection.gameObjects[1].transform;
            //    link.deleteConnection = true;
            //}
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/设置两个路点链接 &w")]
    static void ConnectNode()
    {
        if (Selection.gameObjects.Length == 2)
        {
            bool delete = false;
            {
                NodeLink[] links = Selection.gameObjects[0].GetComponents<NodeLink>();
                for (int i = 0; i < links.Length; i++)
                {
                    if (links[i].End == Selection.gameObjects[1].transform)
                    {
                        delete = true;
                        links[i].deleteConnection = false;
                    }
                }
            }
            {
                NodeLink[] links = Selection.gameObjects[1].GetComponents<NodeLink>();
                for (int i = 0; i < links.Length; i++)
                {
                    if (links[i].End == Selection.gameObjects[0].transform)
                    {
                        delete = true;
                        links[i].deleteConnection = false;
                    }
                }
            }
            if (!delete)
            {
                NodeLink link = Selection.gameObjects[0].AddComponent<NodeLink>();
                link.end = Selection.gameObjects[1].transform;
                link.deleteConnection = false;
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/路点吸附地面 &e")]
    public static void NodeAttachFloor()
    {
        AstarPath aStarPath = GameObject.FindObjectOfType<AstarPath>();
        if (aStarPath != null)
        {
            Transform nodeRoot = aStarPath.transform.Find("NodeRoot");
            if (nodeRoot)
            {
                NodeLink[] links = nodeRoot.GetComponentsInChildren<NodeLink>();
                List<Transform> floorTransform = new List<Transform>();
                for (int j = 0; j < links.Length; j++)
                {
                    if (links[j].Start != null && !floorTransform.Contains(links[j].Start))
                    {
                        floorTransform.Add(links[j].Start);
                    }
                    if (links[j].End != null && !floorTransform.Contains(links[j].End))
                    {
                        floorTransform.Add(links[j].End);
                    }
                    //Transform child = links[j].transform;
                    //if (child)
                    //{
                    //    //float y1 = PengLuaKit.GlobalHelper.FastRaycast(child.position.x, child.position.z, false);
                    //    //if (y1 > -10000)
                    //    //{
                    //    //    child.position = new Vector3(child.position.x, y1, child.position.z);
                    //    //}
                    //    //else
                    //    //{
                    //    //    Debug.LogError("没有找到地面" + child.name);
                    //    //}
                    //    Vector3 rayOrigPoint = child.position + Vector3.up * 500;
                    //    //RaycastHit[] hits = Physics.RaycastAll(rayOrigPoint, Vector3.down, 1000, PengLuaKit.GlobalHelper.layerMask);
                    //    //if (hits.Length > 0)
                    //    //{
                    //    //}
                    //    //else
                    //    //{

                    //    //}
                    //    //for (int i = 0; i < hits.Length; i++)
                    //    //{
                    //    //    if (child.position.y < hits[i].point.y)
                    //    //    {
                    //    //        child.position = hits[i].point;
                    //    //    }
                    //    //}
                    //    //hits = null;

                    //    RaycastHit hitinfo;
                    //    if (Physics.Raycast(rayOrigPoint, Vector3.down, out hitinfo, 1000, PengLuaKit.GlobalHelper.layerMask))
                    //    {
                    //        child.position = hitinfo.point;
                    //    }
                    //    else
                    //    {
                    //        Debug.LogError("没有找到地面" + child.name);
                    //    }
                    //}
                }
                for (int i = 0; i < floorTransform.Count; i++)
                {
                    Transform child = floorTransform[i];
                    Vector3 rayOrigPoint = child.position + Vector3.up * 500;
                    RaycastHit hitinfo;
                    if (Physics.Raycast(rayOrigPoint, Vector3.down, out hitinfo, 1000, LayerDefine.layerMask))
                    {
                        child.position = hitinfo.point;
                    }
                    else
                    {
                        Debug.LogError("没有找到地面" + child.name);
                    }
                }
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/清除路点Mesh &r")]
    static void DeleteNodeMesh()
    {
        AstarPath aStarPath = GameObject.FindObjectOfType<AstarPath>();
        if (aStarPath != null)
        {
            Transform nodeRoot = aStarPath.transform.Find("NodeRoot");
            if (nodeRoot)
            {
                Transform[] trans = nodeRoot.GetComponentsInChildren<Transform>();
                for (int j = 0; j < trans.Length; j++)
                {
                    Transform child = trans[j];
                    if (child != nodeRoot)
                    {
                        MeshFilter filter = child.GetComponent<MeshFilter>();
                        if (filter)
                        {
                            GameObject.DestroyImmediate(filter);
                        }
                        Renderer ren = child.GetComponent<Renderer>();
                        if (ren)
                        {
                            GameObject.DestroyImmediate(ren);
                        }
                    }
                    child.hasChanged = true;
                }
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/删除无效的Link &y")]
    static void DeleteDisableNodeMesh()
    {
        AstarPath aStarPath = GameObject.FindObjectOfType<AstarPath>();
        if (aStarPath != null)
        {
            Transform nodeRoot = aStarPath.transform.Find("NodeRoot");
            if (nodeRoot)
            {
                NodeLink[] links = nodeRoot.GetComponentsInChildren<NodeLink>();
                for (int j = 0; j < links.Length; j++)
                {
                    if (links[j].deleteConnection || links[j].End == null)
                    {
                        GameObject.DestroyImmediate(links[j]);
                    }
                }
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/显示路点 &t")]
    static void ShowNode()
    {
        AstarPath aStarPath = GameObject.FindObjectOfType<AstarPath>();
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshFilter mesh = temp.GetComponent<MeshFilter>();
        if (aStarPath != null)
        {
            Transform nodeRoot = aStarPath.transform.Find("NodeRoot");
            if (nodeRoot)
            {
                Transform[] trans = nodeRoot.GetComponentsInChildren<Transform>();
                for (int j = 0; j < trans.Length; j++)
                {
                    Transform child = trans[j];
                    if (child != nodeRoot)
                    {
                        MeshFilter filter = child.GetComponent<MeshFilter>();
                        if (filter == null)
                        {
                            filter = child.gameObject.AddComponent<MeshFilter>();
                            filter.sharedMesh = mesh.sharedMesh;
                        }
                        Renderer ren = child.GetComponent<Renderer>();
                        if (ren == null)
                        {
                            child.gameObject.AddComponent<MeshRenderer>();
                        }
                    }
                }
            }
        }
        GameObject.DestroyImmediate(temp);
    }
    [MenuItem("Tools/辅助工具/吸附地面")]
    static void AttachTerrain()
    {
        foreach (var it in Selection.gameObjects)
        {
            Vector3 rayOrigPoint = it.transform.position + Vector3.up * 500;
            RaycastHit hitinfo;
            if (Physics.Raycast(rayOrigPoint, Vector3.down, out hitinfo, 1000, LayerDefine.layerMask))
            {
                it.transform.position = hitinfo.point;
                Debug.LogError(hitinfo.collider.gameObject.name, hitinfo.collider.gameObject);
            }
            else
            {
                Debug.LogError("没有找到地面" + it.transform.name);
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/复制节点 &d")]
    static void CopyNode()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject newGo = GameObject.Instantiate(Selection.activeGameObject, Selection.activeGameObject.transform.parent);
            NodeLink[] links = newGo.GetComponents<NodeLink>();
            for (int i = links.Length - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(links[i]);
            }
            NodeLink link = newGo.AddComponent<NodeLink>();
            link.end = Selection.activeGameObject.transform;
            newGo.name = "PathNode";
            Selection.activeGameObject = newGo;
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    [MenuItem("Tools/路点/清除配置方便保存 &s")]
    static void SaveScene()
    {
        DeleteDisableNodeMesh();
        NodeAttachFloor();
        DeleteNodeMesh();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
    #region 创建路点mesh
    //[MenuItem("Tools/路点/创建路点mesh")]
    //static void CreateNodeMesh()
    //{
    //    //CopyAllNavigation(FindAllNavigationMesh());
    //    AstarPath aStarPath = GameObject.FindObjectOfType<AstarPath>();
    //    if (aStarPath != null)
    //    {
    //        Transform nodeRoot = aStarPath.transform.Find("NodeRoot");
    //        if (nodeRoot)
    //        {
    //            int count = 1;
    //            //List<Vector3> offsets = new List<Vector3>() { Vector3.zero, -Vector3.left * 0.5f, Vector3.left * 0.5f, -Vector3.forward * 0.5f, Vector3.forward * 0.5f, };
    //            List<float> offsets = new List<float>() { 0, -0.3f, 0.3f, -0.6f, 0.6f };
    //            float height = 0.0f;
    //            for (int j = 0; j < count; j++)
    //            {
    //                GameObject road_layer = new GameObject("road_layer" + j.ToString());

    //                NodeLink[] nodeLinks = nodeRoot.GetComponentsInChildren<NodeLink>();
    //                //单独创建mesh
    //                //for (int i = 0; i < nodeLinks.Length; i++)
    //                //{
    //                //    if (nodeLinks[i].Start != null && nodeLinks[i].End != null)
    //                //    {
    //                //        Vector3 normal = Vector3.Cross((nodeLinks[i].End.position - nodeLinks[i].Start.position), Vector3.up).normalized;

    //                //        GameObject road = CreateMesh(nodeLinks[i].Start.position + offsets[j] * normal, nodeLinks[i].End.position + offsets[j] * normal);
    //                //        road.transform.parent = road_layer.transform;
    //                //        GameObjectUtility.SetStaticEditorFlags(road, StaticEditorFlags.NavigationStatic);
    //                //        GameObjectUtility.SetNavMeshArea(road, 3 + j);
    //                //    }
    //                //}

    //                int maxVerticesCount = 10000;
    //                List<Vector3> vertices = new List<Vector3>();
    //                List<int> triangles = new List<int>();
    //                for (int i = 0; i < nodeLinks.Length; i++)
    //                {
    //                    if (nodeLinks[i].Start != null && nodeLinks[i].End != null)
    //                    {
    //                        Vector3 normal = Vector3.Cross((nodeLinks[i].End.position - nodeLinks[i].Start.position), Vector3.up).normalized;
    //                        AddOneMesh(nodeLinks[i].Start.position + offsets[j] * normal, nodeLinks[i].End.position + offsets[j] * normal, 0.5f, 100, vertices, triangles);
    //                        if (i == nodeLinks.Length - 1 || vertices.Count > maxVerticesCount)
    //                        {
    //                            GameObject road = CreateMesh(vertices, triangles);
    //                            road.transform.parent = road_layer.transform;
    //                            GameObjectUtility.SetStaticEditorFlags(road, StaticEditorFlags.NavigationStatic);
    //                            GameObjectUtility.SetNavMeshArea(road, 3 + j);
    //                            road_layer.transform.position = new Vector3(0, height, 0);
    //                            vertices.Clear();
    //                            triangles.Clear();
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    //}
    #endregion
    public static GameObject CreateMesh(Vector3 start, Vector3 end, float width, int count, CenterRoadWnd.RoadMeshHeightType roadHeightType)
    {
        int segment = count + 1;
        Vector3 normal = Vector3.Cross((end - start), Vector3.up).normalized;
        Vector3[] vertices = new Vector3[segment * 2];
        for (int i = 0; i < segment; i++)
        {
            vertices[i * 2] = Vector3.Lerp(start, end, i / (float)count) + normal * width;
            vertices[i * 2 + 1] = Vector3.Lerp(start, end, i / (float)count) - normal * width;

            vertices[i * 2] = GetHeight(roadHeightType, vertices[i * 2]);
            vertices[i * 2 + 1] = GetHeight(roadHeightType, vertices[i * 2 + 1]);
        }
        int[] triangles = new int[count * 6];
        for (int i = 0; i < count; i++)
        {
            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 2;
            triangles[i * 6 + 2] = i * 2 + 1;

            triangles[i * 6 + 3] = i * 2 + 1;
            triangles[i * 6 + 4] = i * 2 + 2;
            triangles[i * 6 + 5] = i * 2 + 3;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GameObject go = new GameObject();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        return go;
    }
    public static void AddOneMesh(Vector3 start, Vector3 end, float width, int count, List<Vector3> _vertices, List<int> _triangles, CenterRoadWnd.RoadMeshHeightType roadHeightType)
    {
        int segment = count + 1;
        Vector3 normal = Vector3.Cross((end - start), Vector3.up).normalized;
        Vector3[] vertices = new Vector3[segment * 2];
        for (int i = 0; i < segment; i++)
        {
            vertices[i * 2] = Vector3.Lerp(start, end, i / (float)count) + normal * width;
            vertices[i * 2 + 1] = Vector3.Lerp(start, end, i / (float)count) - normal * width;

            vertices[i * 2] = GetHeight(roadHeightType, vertices[i * 2]);
            vertices[i * 2 + 1] = GetHeight(roadHeightType, vertices[i * 2 + 1]);
        }
        int[] triangles = new int[count * 6];
        for (int i = 0; i < count; i++)
        {
            triangles[i * 6] = _vertices.Count + i * 2;
            triangles[i * 6 + 1] = _vertices.Count + i * 2 + 2;
            triangles[i * 6 + 2] = _vertices.Count + i * 2 + 1;

            triangles[i * 6 + 3] = _vertices.Count + i * 2 + 1;
            triangles[i * 6 + 4] = _vertices.Count + i * 2 + 2;
            triangles[i * 6 + 5] = _vertices.Count + i * 2 + 3;
        }

        _vertices.AddRange(vertices);
        _triangles.AddRange(triangles);
    }
    public static GameObject CreateMesh(List<Vector3> vertices, List<int> triangles)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GameObject go = new GameObject();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();

        return go;
    }
    //[MenuItem("Tools/路点/找到路点烘焙mesh并实例化")]
    public static void FindNavigationMeshAndInstantiate()
    {
        GameObject old = GameObject.Find("Navigation_Root");
        if (old != null) { GameObject.DestroyImmediate(old); }
        CopyAllNavigation(FindAllNavigationMesh());
    }
    public static List<GameObject> FindAllNavigationMesh()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        List<GameObject> scene_lst = new List<GameObject>();
        scene.GetRootGameObjects(scene_lst);
        List<GameObject> navigationObjs = new List<GameObject>();
        for (int i = 0; i < scene_lst.Count; i++)
        {
            MeshRenderer[] renders = scene_lst[i].GetComponentsInChildren<MeshRenderer>(true);
            foreach (var it in renders)
            {
                if ((GameObjectUtility.GetStaticEditorFlags(it.gameObject) & StaticEditorFlags.NavigationStatic) != 0)
                {
                    //如果父节点也是导航的话，那么已经实例化了，
                    //if (it.transform.parent == null || ((GameObjectUtility.GetStaticEditorFlags(it.transform.parent.gameObject) & StaticEditorFlags.NavigationStatic) == 0))
                    {
                        navigationObjs.Add(it.gameObject);
                    }
                }
            }
        }
        return navigationObjs;
    }
    public static GameObject CopyAllNavigation(List<GameObject> lst )
    {
        GameObject old = GameObject.Find("Navigation_Root");
        if (old != null) { GameObject.DestroyImmediate(old); }
        GameObject root = new GameObject("Navigation_Root");
        for (int i = 0; i < lst.Count; i++)
        {
            MeshRenderer render = lst[i].GetComponent<MeshRenderer>();
            MeshFilter filter = lst[i].GetComponent<MeshFilter>();

            if (render == null || filter == null || !render.enabled)
            {
                continue;
            }
            GameObject item = GameObject.Instantiate(lst[i]);
            item.transform.position = lst[i].transform.position;
            item.transform.eulerAngles = lst[i].transform.eulerAngles;
            item.transform.localScale = lst[i].transform.lossyScale;
            item.SetActive(true);

            Collider col = item.GetComponent<Collider>();
            if (col == null)
            {
                if (render.enabled)
                {
                    if (filter.sharedMesh.isReadable)
                    {
                        //确保meshcollider可读
                        item.AddComponent<MeshCollider>();
                        //Debug.LogError(lst[i].name + "的mesh可读", lst[i]);
                    }
                    else
                    {
                        MeshFilter itemFilter = item.GetComponent<MeshFilter>();
                        itemFilter.sharedMesh = CopyNoReadableMesh(filter.sharedMesh);

                        //DestroyMesh destroyMesh = item.AddComponent<DestroyMesh>();
                        //destroyMesh.meshFilter = itemFilter;

                        item.AddComponent<MeshCollider>();
                    }

                }
                else
                {

                }
            }
            item.transform.parent = root.transform;
            item.gameObject.layer = LayerDefine.Invisible;
        }
        return root;
    }
    public static Mesh CopyNoReadableMesh(Mesh copyMesh)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = copyMesh.vertices;
        mesh.normals = copyMesh.normals;
        mesh.tangents = copyMesh.tangents;
        mesh.uv = copyMesh.uv;
        mesh.uv2 = copyMesh.uv2;
        mesh.uv3 = copyMesh.uv3;
        mesh.uv4 = copyMesh.uv4;
        mesh.uv5 = copyMesh.uv5;
        mesh.uv6 = copyMesh.uv6;
        mesh.uv7 = copyMesh.uv7;
        mesh.uv8 = copyMesh.uv8;
        mesh.colors = copyMesh.colors;
        mesh.colors32 = copyMesh.colors32;
        mesh.triangles = copyMesh.triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    public static Vector3 GetHeight(CenterRoadWnd.RoadMeshHeightType roadHeightType,Vector3 point)
    {
        switch (roadHeightType)
        {
            case CenterRoadWnd.RoadMeshHeightType.MESH_HEIGHT_GROUND_LAYER:
                {
                        Vector3 rayOrigPoint = point + Vector3.up * 500;
                        RaycastHit hitinfo;
                        if (Physics.Raycast(rayOrigPoint, Vector3.down, out hitinfo, 1000, LayerDefine.layerMask))
                        {
                            return hitinfo.point;
                        }
                    break;
                }
            case CenterRoadWnd.RoadMeshHeightType.MESH_HEIGHT_NAVIGATION:
                {
                    Vector3 rayOrigPoint = point + Vector3.up * 500;
                    RaycastHit hitinfo;
                    if (Physics.Raycast(rayOrigPoint, Vector3.down, out hitinfo, 1000, 1 << LayerDefine.Invisible))
                    {
                        return hitinfo.point;
                    }
                    break;
                }
            case CenterRoadWnd.RoadMeshHeightType.MESH_HEIGHT_POINT:
                {
                    break;
                }
        }
        return point;
    }
}
