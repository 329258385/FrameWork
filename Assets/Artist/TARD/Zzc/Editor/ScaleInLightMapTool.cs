#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScaleInLightMapTool : EditorWindow
{

    private string allValue;
    private string selectValue;

    ScaleInLightMapTool()
    {
        this.titleContent = new GUIContent("ScaleInLightMapTool");
    }

    [MenuItem("Tools/ScaleInLightMap工具")]
    static void showWindow()
    {
        GetWindow(typeof(ScaleInLightMapTool));
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.Label("ScaleInLightMap工具");
        GUILayout.Space(10);
        allValue = EditorGUILayout.TextField("设置所有值", allValue, GUILayout.Width(300f));
        if (GUILayout.Button("设置所有ScaleInLightMap",GUILayout.Width(300f)))
        {
            FindAllGameObject();
        }
        selectValue = EditorGUILayout.TextField("设置选中值", selectValue, GUILayout.Width(300f));
        if (GUILayout.Button("设置所选ScaleInLightMap", GUILayout.Width(300f)))
        {
            FindSelectedGameObject();
        }
        if (GUILayout.Button("根据所选基准物改变相同预制体LightMap值", GUILayout.Width(300f)))
        {
            ChangeSamePrefab();
        }
        GUILayout.EndVertical();
    }

    private void FindAllGameObject()
    {
        MeshRenderer[] objs = FindObjectsOfType<MeshRenderer>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].name.EndsWith("LOD0"))
            {
                SerializedObject so = new SerializedObject(objs[i]);
                so.FindProperty("m_ScaleInLightmap").floatValue =float.Parse(allValue);
                so.ApplyModifiedProperties();
            }
        }
    }

    private void FindSelectedGameObject()
    {
        GameObject[] selectedGameObjects=Selection.gameObjects;
        for (int i = 0; i < selectedGameObjects.Length; i++)
        {
            MeshRenderer[] grandFa = selectedGameObjects[i].GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < grandFa.Length; j++)
            {
                if (grandFa[j].name.EndsWith("LOD0"))
                {
                    SerializedObject so = new SerializedObject(grandFa[j]);
                    so.FindProperty("m_ScaleInLightmap").floatValue = float.Parse(selectValue);
                    so.ApplyModifiedProperties();
                }
            }
        }

    }

    private void ChangeSamePrefab()
    {
        float factor=1;
        string thisPath="";
        if (PrefabUtility.GetPrefabType(Selection.gameObjects[0])==PrefabType.PrefabInstance)
        {
            Object parentObject = EditorUtility.GetPrefabParent(Selection.gameObjects[0]);
            thisPath = AssetDatabase.GetAssetPath(parentObject);
        }

        GameObject g = Selection.gameObjects[0];

        MeshRenderer[] mrs = Selection.gameObjects[0].transform.GetComponentsInChildren<MeshRenderer>();
        float[] factors = new float[mrs.Length];
        for (int k = 0; k < mrs.Length; k++)
        {
            SerializedObject so = new SerializedObject(mrs[k]);
            factors[k]= (Selection.gameObjects[0].transform.localScale.x +
                Selection.gameObjects[0].transform.localScale.y +
                Selection.gameObjects[0].transform.localScale.z) * 0.3333333333f /
                so.FindProperty("m_ScaleInLightmap").floatValue;
        }

        //if (Selection.gameObjects[0].GetComponent<MeshRenderer>()!=null)
        //{
        //    SerializedObject so = new SerializedObject(Selection.gameObjects[0].GetComponent<MeshRenderer>());
        //    factor = (Selection.gameObjects[0].transform.localScale.x+ 
        //        Selection.gameObjects[0].transform.localScale.y+
        //        Selection.gameObjects[0].transform.localScale.z)*0.3333333333f/ 
        //        so.FindProperty("m_ScaleInLightmap").floatValue;
        //}

        MeshRenderer[] objs = FindObjectsOfType<MeshRenderer>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].name.EndsWith("LOD0"))
            {
                if (PrefabUtility.GetPrefabType(objs[i]) == PrefabType.PrefabInstance)
                {
                    GameObject father = GetFather(objs[i].transform).gameObject;
                    if (father!=g)
                    {
                        //Debug.Log(father.name);
                        Object parentObject = EditorUtility.GetPrefabParent(father);
                        string path = AssetDatabase.GetAssetPath(parentObject);
                        if (path == thisPath)
                        {
                            MeshRenderer[] mrs2 = father.transform.GetComponentsInChildren<MeshRenderer>();
                            for (int j = 0; j < mrs2.Length; j++)
                            {
                                SerializedObject so2 = new SerializedObject(mrs2[j]);
                                float factorValue = (father.transform.localScale.x +
                                                 father.transform.localScale.y +
                                                 father.transform.localScale.z) * 0.3333333333f / factors[j];
                                so2.FindProperty("m_ScaleInLightmap").floatValue = factorValue;
                                so2.ApplyModifiedProperties();
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Same");
                    }
                }
            }
        }
    }

    private Transform GetFather(Transform t)
    {
        //if (PrefabUtility.GetPrefabType(t) == PrefabType.PrefabInstance)
        //{
            if (t.parent==null)
            {
                return t;
            }
            else
            {
                if (PrefabUtility.GetPrefabType(t.parent) == PrefabType.PrefabInstance)
                {
                    GetFather(t.parent);
                    return t.parent;
                }
                else
                {
                    return t;
                }
            }
       // }
       
    }
}
#endif