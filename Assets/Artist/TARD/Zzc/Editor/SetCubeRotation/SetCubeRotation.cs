using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetCubeRotation : EditorWindow {

    private static List<Transform> none0Cubes;
    private Vector2 scrollPos; 

    [MenuItem("GameObject/cube旋转归零(判断缩放)",false,11)]
    static void SetRotation1()
    {
        SetCubeRotation zzz = (SetCubeRotation)GetWindow(typeof(SetCubeRotation));
        none0Cubes = new List<Transform>();
        Transform[] ts = Selection.GetTransforms(SelectionMode.Assets);
        for (int i = 0; i < ts.Length; i++)
        {
            if ((ts[i].localEulerAngles.x!=0f|| ts[i].localEulerAngles.z!=0f)&&(ts[i].localScale.x>3f|| ts[i].localScale.y > 3f|| ts[i].localScale.z > 3f)&&ts[i].gameObject.activeInHierarchy==true)
            {
                if (ts[i].localScale.y > 3f && ((ts[i].localEulerAngles.x >=-0.1f&&ts[i].localEulerAngles.x<=10f)|| (ts[i].localEulerAngles.x>=350f&& ts[i].localEulerAngles.x <= 360.1f))&& ((ts[i].localEulerAngles.z<=10f&&ts[i].localEulerAngles.z >= -0.1f) || (ts[i].localEulerAngles.z>=350f&&ts[i].localEulerAngles.z <= 360.1f)))
                {
                    if (ts[i].gameObject.layer!=17)
                    {
                        none0Cubes.Add(ts[i]);
                    }                    
                }                
            }
        }
    }

    [MenuItem("GameObject/cube旋转归零(不判断缩放)", false, 11)]
    static void SetRotation2()
    {
        SetCubeRotation zzz = (SetCubeRotation)GetWindow(typeof(SetCubeRotation));
        none0Cubes = new List<Transform>();
        Transform[] ts = Selection.GetTransforms(SelectionMode.Assets);
        for (int i = 0; i < ts.Length; i++)
        {
            if ((ts[i].localEulerAngles.x != 0f || ts[i].localEulerAngles.z != 0f) && ts[i].gameObject.activeInHierarchy == true)
            {
                if (((ts[i].localEulerAngles.x >= -0.1f && ts[i].localEulerAngles.x <= 10f) || (ts[i].localEulerAngles.x >= 350f && ts[i].localEulerAngles.x <= 360.1f)) && ((ts[i].localEulerAngles.z <= 10f && ts[i].localEulerAngles.z >= -0.1f) || (ts[i].localEulerAngles.z >= 350f && ts[i].localEulerAngles.z <= 360.1f)))
                {
                    if (ts[i].gameObject.layer != 17)
                    {
                        none0Cubes.Add(ts[i]);
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        scrollPos=GUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        for (int i = 0; i < none0Cubes.Count; i++)
        {
            GUILayout.BeginHorizontal();
            Transform t=(Transform)EditorGUILayout.ObjectField(none0Cubes[i].name, none0Cubes[i], typeof(Transform), true);
            if (GUILayout.Button("清零"))
            {
                t.localEulerAngles = new Vector3(0f,t.localEulerAngles.y, 0f);
                none0Cubes.Remove(t);
                EditorUtility.SetDirty(t);
            }
            if (GUILayout.Button("跳过"))
            {
                none0Cubes.Remove(t);
                EditorUtility.SetDirty(t);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
