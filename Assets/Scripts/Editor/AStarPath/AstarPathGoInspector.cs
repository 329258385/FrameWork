using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(AstarPathGo))]
public class AstarPathGoInspector : Editor {
    AstarPathGo pathGo;
    private void OnEnable()
    {
        pathGo = target as AstarPathGo;
    }
    public override void OnInspectorGUI()
    {
        pathGo.maxCalcCount = EditorGUILayout.IntField("maxCalcCount 每帧更新路点数量", pathGo.maxCalcCount);
        //pathGo.pointNavSetting = (AstarPathGo.PointNavModel)EditorGUILayout.EnumFlagsField("路点计算方式", pathGo.pointNavSetting);
        pathGo.startPointCalc = EditorGUILayout.FloatField("startPointCalc 起始点半径内的路点处理", pathGo.startPointCalc);
        pathGo.calcStartCanReach = EditorGUILayout.Toggle("calcStartCanReach 起始点半径内的路点处理(能否直接到达)", pathGo.calcStartCanReach);
        pathGo.endPointCalc = EditorGUILayout.FloatField("endPointCalc 目标点半径内直接去目标点", pathGo.endPointCalc);
        pathGo.calcY = EditorGUILayout.Toggle("calcY 计算距离是否计算Y轴", pathGo.calcY);

        if (GUI.changed && !EditorApplication.isPlaying)
        {
            EditorUtility.SetDirty(pathGo);
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(pathGo.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }
}
