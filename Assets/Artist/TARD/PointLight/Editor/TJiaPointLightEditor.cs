using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TJiaPointLightEditor : EditorWindow
{

    [MenuItem("Light/TJiaPointLight")]
    [MenuItem("GameObject/Light/TJiaPointLight")]
    public static void CreatPointLight(MenuCommand menuCommand)
    {
        GameObject obj = new GameObject();
        obj.AddComponent<TJiaPointLight>();
        obj.name = "TJia Point Light";
        GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
        Selection.activeObject = obj;
    }
}
