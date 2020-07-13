using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyGetAttack))]
public class EnemyGetHitEditor : Editor {

    //private SerializedObject obj;
    //private EnemyGetAttack ega;

    //public void ShowGradient(string guiContent, string propertyName)
    //{
    //    EditorGUI.BeginChangeCheck();
    //    SerializedProperty sp = obj.FindProperty(propertyName);
    //    GUIContent g = new GUIContent(guiContent);
    //    EditorGUILayout.PropertyField(sp, g);
    //    if (EditorGUI.EndChangeCheck())
    //    {
    //        obj.ApplyModifiedProperties();
    //    }
    //}

    //public void ShowArray(string guiContent,string propertyName)
    //{
    //    EditorGUI.BeginChangeCheck();
    //    SerializedProperty sp = obj.FindProperty(propertyName);
    //    GUIContent g = new GUIContent(guiContent);
    //    EditorGUILayout.PropertyField(sp,g,true);
    //    if (EditorGUI.EndChangeCheck())
    //    {
    //        obj.ApplyModifiedProperties();
    //    }
    //}

    //public override void OnInspectorGUI()
    //{
    //    obj = new SerializedObject(target);
    //    Undo.RecordObject(target, "dad");
    //    obj.Update();
    //    ega = (EnemyGetAttack)target;

    //    EditorGUILayout.BeginVertical();
    //    EditorGUILayout.Space();
    //    EditorGUILayout.LabelField("分别设置怪物受攻击的动画和特效出现时间", EditorStyles.boldLabel);
    //    EditorGUILayout.Space();
    //    ega.totalTime = EditorGUILayout.FloatField("位移总时间", ega.totalTime);
    //    ShowArray("怪物动画设置","hi");
    //    ShowArray("特效设置", "ei");
    //    EditorGUILayout.EndVertical();
    //    ShowGradient("运动曲线(x轴、y轴为0~1)", "curve");

    //    if (GUI.changed)
    //    {
    //        EditorUtility.SetDirty(target);
    //    }
    //    obj.ApplyModifiedProperties();
    //}

}
