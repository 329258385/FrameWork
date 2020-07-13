using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class NavmeshOp : EditorWindow {
    private Mesh mesh;

    NavmeshOp()
    {
        titleContent = new GUIContent("NavmeshOp");
    }

    [MenuItem("TARD/NavmeshOp")]
    private static void ShowNavmeshOp()
    {
        NavmeshOp no = (NavmeshOp)GetWindow(typeof(NavmeshOp));
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        mesh = (Mesh)EditorGUILayout.ObjectField("Mesh:", mesh, typeof(Mesh), true);
        if (GUILayout.Button("Excute"))
        {
            Vector3[] newVec = NavMesh.CalculateTriangulation().vertices;
            int[] newTri = NavMesh.CalculateTriangulation().indices;
            mesh.vertices = newVec;
            mesh.triangles = newTri;
        }
        GUILayout.EndVertical();
    }
}
