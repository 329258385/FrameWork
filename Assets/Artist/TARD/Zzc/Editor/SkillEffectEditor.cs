using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillEffect))]
public class SkillEffectEditor : Editor {

    //private SerializedObject obj;
    //private SkillEffect se;

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

    //public override void OnInspectorGUI()
    //{
    //    obj = new SerializedObject(target);
    //    Undo.RecordObject(target, "dad");
    //    obj.Update();
    //    se = (SkillEffect)target;

    //    EditorGUILayout.BeginVertical();
    //    EditorGUILayout.Space();
    //    EditorGUILayout.LabelField("在DotweenPath中设置运动路径点", EditorStyles.boldLabel);
    //    EditorGUILayout.Space();
    //    se.prefab = (GameObject)EditorGUILayout.ObjectField("特效Prefab", se.prefab, typeof(GameObject));
    //    se.waitTime = EditorGUILayout.FloatField("延迟出现时间", se.waitTime);
    //    se.destroyTime = EditorGUILayout.FloatField("出现后消失的时间", se.destroyTime);
    //    se.playTime = EditorGUILayout.FloatField("位移的总时间", se.playTime);
    //    ShowGradient("位移速度曲线(x轴y轴范围0~1)","curve");
    //    EditorGUILayout.EndVertical();

    //    if (GUI.changed)
    //    {
    //        EditorUtility.SetDirty(target);
    //    }
    //    obj.ApplyModifiedProperties();
    //}

}
