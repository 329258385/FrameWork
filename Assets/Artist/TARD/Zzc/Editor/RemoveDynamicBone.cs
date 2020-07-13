using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class RemoveDynamicBone : Editor {

    private static List<string> allFilePath;

    [MenuItem("GameObject/Tools/移除选择的DynamicBone")]
    private static void RemoveTheseDB()
    {
        GameObject[] selectedGOs = Selection.gameObjects;
        for (int i = 0; i < selectedGOs.Length; i++)
        {
            RemoveDB(selectedGOs[i].transform);
        }
    }


    [MenuItem("Assets/SaoUtils/移除DynamicBone")]
    private static void ZzcRemoveDynamicBone()
    {
        allFilePath = new List<string>();
        string[] selections = Selection.assetGUIDs;
        string[] paths = new string[selections.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = Application.dataPath.Replace("Assets", "") + AssetDatabase.GUIDToAssetPath(selections[i]);
            ZzcDirector(paths[i]);
        }

        for (int i = 0; i < allFilePath.Count; i++)
        {
            try
            {
                string str = "Assets" + allFilePath[i].Split(new string[] { "Assets" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //Debug.Log(str);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(str);
                if (go)
                {
                    RemoveDB(go.transform);
                }
                
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
    }

    private static void RemoveDB(Transform t)
    {
        if (t.GetComponent<DynamicBone>())
        {
            DestroyImmediate(t.GetComponent<DynamicBone>(),true);
        }
        if (t.childCount>0)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                RemoveDB(t.GetChild(i));
            }
        }
    }

    private static void ZzcDirector(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo d = new DirectoryInfo(path);
            FileSystemInfo[] fsInfos = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fsInfos)
            {
                if (fsinfo is DirectoryInfo)
                {
                    ZzcDirector(fsinfo.FullName);
                }
                else
                {
                    if (fsinfo.FullName.EndsWith(".prefab"))
                    {
                        allFilePath.Add(fsinfo.FullName);
                    }
                }
            }
        }
        else
        {
            if (path.EndsWith(".prefab"))
            {
                allFilePath.Add(path);
            }
        }
    }
}
