using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerAnimationData))]
public class PlayerAnimationDataEditor : Editor {

    private SerializedObject obj;
    private PlayerAnimationData pad;

    public override void OnInspectorGUI()
    {
        obj = new SerializedObject(target);
        Undo.RecordObject(target, "dad");
        obj.Update();
        pad = (PlayerAnimationData)target;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("自定义Assets目录下路径，前后不需要加/", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        pad.skillPath = EditorGUILayout.TextField("技能保存和读取的路径", pad.skillPath);

        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        obj.ApplyModifiedProperties();
    }
}
