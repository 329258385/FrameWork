using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;

public class AstarLerpHelper : EditorWindow {
    [MenuItem("Tools/路点/打开路点分段界面 &l")]
    static void Open()
    {
        EditorWindow.GetWindow(typeof(AstarLerpHelper));
    }

    [MenuItem("Tools/路点/路点分段 &o")]
    static void NodeLerp()
    {
        if (lerpLength <= 0)
        {
            Open();
        }
        if (Selection.gameObjects.Length == 2)
        {
            Transform start = null;
            Transform end = null;
            {
                NodeLink[] links = Selection.gameObjects[0].GetComponents<NodeLink>();
                for (int i = 0; i < links.Length; i++)
                {
                    if (links[i].End == Selection.gameObjects[1].transform)
                    {
                        start = Selection.gameObjects[0].transform;
                        end = Selection.gameObjects[1].transform;
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
                        start = Selection.gameObjects[1].transform;
                        end = Selection.gameObjects[0].transform;
                        GameObject.DestroyImmediate(links[i]);
                    }
                }
            }
            float length = Vector3.Distance(Selection.gameObjects[0].transform.position, Selection.gameObjects[1].transform.position);
            if (length > lerpLength)
            {
                float floatCount = length / lerpLength;
                int count = Mathf.FloorToInt(length / lerpLength);
                //有小数的话数量加一
                if (floatCount > count)
                {
                    count += 1;
                }
                ////起始点结束点已经有，要差值的点数量-1
                //count -= 1;

                if (count > 0)
                {
                    if (start == null || end == null)
                    {
                        start = Selection.gameObjects[0].transform;
                        end = Selection.gameObjects[1].transform;
                    }

                    Delete(start, end);

                    NodeCell nodeCell = AddNodeCell(start.gameObject, end.gameObject);

                    Transform pre = start;
                    for (int i = 1; i < count; i++)
                    {
                        pre = AddLerpObj(start.transform, pre, Vector3.Lerp(start.position, end.position, (float)i / (float)count));
                        nodeCell.lst.Add(pre);
                    }

                    AddLink(end.gameObject, pre);
                }
            }
            AStarHelpEditor.NodeAttachFloor();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

    }
    [MenuItem("Tools/路点/删除路点分段 &p")]
    static void DeleteLerp()
    {
        if (Selection.gameObjects.Length == 2)
        {
            Transform start = Selection.gameObjects[0].transform;
            Transform end = Selection.gameObjects[1].transform;

            Delete(start, end);
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

    }
    public static void Delete(Transform start, Transform end)
    {
        {
            NodeCell[] links = start.GetComponents<NodeCell>();
            for (int i = 0; i < links.Length; i++)
            {
                if ((links[i].start == start && links[i].end == end) || (links[i].start == end && links[i].end == start))
                {
                    if (links[i].lst != null)
                    {
                        foreach (var it in links[i].lst)
                        {
                            GameObject.DestroyImmediate(it.gameObject);
                        }
                    }
                    GameObject.DestroyImmediate(links[i]);
                }
            }
        }
        {
            NodeCell[] links = end.GetComponents<NodeCell>();
            for (int i = 0; i < links.Length; i++)
            {
                if ((links[i].start == start && links[i].end == end) || (links[i].start == end && links[i].end == start))
                {
                    if (links[i].lst != null)
                    {
                        foreach (var it in links[i].lst)
                        {
                            GameObject.DestroyImmediate(it.gameObject);
                        }
                    }
                    GameObject.DestroyImmediate(links[i]);
                }
            }
        }

        {
            NodeLink[] links = Selection.gameObjects[0].GetComponents<NodeLink>();
            for (int i = 0; i < links.Length; i++)
            {
                if (links[i].End == null)
                {
                    GameObject.DestroyImmediate(links[i]);
                }
            }
        }

        {
            NodeLink[] links = Selection.gameObjects[1].GetComponents<NodeLink>();
            for (int i = 0; i < links.Length; i++)
            {
                if (links[i].End == null)
                {
                    GameObject.DestroyImmediate(links[i]);
                }
            }
        }
    }
    public static Transform AddLerpObj(Transform parent, Transform linkEnd, Vector3 pos)
    {
        GameObject newGo = new GameObject("PathNode");
        newGo.transform.parent = parent;
        newGo.transform.position = pos;
        newGo.transform.localScale = Vector3.one;
        AddLink(newGo, linkEnd);
        return newGo.transform;
    }
    public static void AddLink(GameObject go,Transform end)
    {
        NodeLink link = go.AddComponent<NodeLink>();
        link.end = end;
    }
    public static NodeCell AddNodeCell(GameObject start,GameObject end)
    {
        {
            NodeCell[] links = start.GetComponents<NodeCell>();
            for (int i = 0; i < links.Length; i++)
            {
                if ((links[i].start == start && links[i].end == end) || (links[i].start == end && links[i].end == start))
                {
                    links[i].lst.Clear();
                    return links[i];
                }
            }
        }
        {
            NodeCell[] links = end.GetComponents<NodeCell>();
            for (int i = 0; i < links.Length; i++)
            {
                if ((links[i].start == start && links[i].end == end) || (links[i].start == end && links[i].end == start))
                {
                    links[i].lst.Clear();
                    return links[i];
                }
            }
        }

        NodeCell cell = start.gameObject.AddComponent<NodeCell>();
        cell.start = start.transform;
        cell.end = end.transform;
        return cell;
    }
    public static float lerpLength;
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        lerpLength = EditorGUILayout.FloatField("分段距离", lerpLength);
        if (lerpLength <= 0)
        {
            lerpLength = 3;
        }
        EditorGUILayout.EndVertical();
    }
}
