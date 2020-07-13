using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZzcPlayAnim))]
public class ZzcPlayerAnimEditor : Editor {

    //private SerializedObject obj;
    //private ZzcPlayAnim zpa;

    //public void ShowGradient(string guiContent,string propertyName)
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

    //public override void OnInspectorGUI()
    //{
    //    obj=new SerializedObject(target);
    //    Undo.RecordObject(target, "dad");
    //    obj.Update();
    //    zpa = (ZzcPlayAnim)target;

    //    EditorGUILayout.BeginVertical();
    //    EditorGUILayout.Space();
    //    EditorGUILayout.LabelField("在DotweenPath中设置运动路径点",EditorStyles.boldLabel);
    //    EditorGUILayout.Space();
    //    zpa.totalTime = EditorGUILayout.FloatField("技能总时间,技能中无法移动", zpa.totalTime);
    //    zpa.skillName = EditorGUILayout.TextField("状态机中的技能名", zpa.skillName);
    //    zpa.setType = EditorGUILayout.IntField("状态机中的Type参数", zpa.setType);
    //    ShowGradient("运动曲线(x轴、y轴为0~1)", "curve");
    //    EditorGUILayout.EndVertical();

    //    if (GUI.changed)
    //    {
    //        EditorUtility.SetDirty(target);
    //    }
    //    obj.ApplyModifiedProperties();
    //}
}
