using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MirrorMesh : EditorWindow {

    private Mesh mesh;

    MirrorMesh()
    {
        titleContent = new GUIContent("镜像Mesh");
    }

    [MenuItem("TARD/镜像mesh")]
    private static void ShowMirrorMesh()
    {
        MirrorMesh mm = (MirrorMesh)GetWindow(typeof(MirrorMesh));
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        mesh = (Mesh)EditorGUILayout.ObjectField("Mesh:", mesh, typeof(Mesh), true);
        if (GUILayout.Button("X"))
        {
            Vector3[] newVertices = mesh.vertices;
            for (int i = 0; i < newVertices.Length; i++)
            {
                newVertices[i].x = newVertices[i].x * -1;
            }
            mesh.vertices = newVertices;
            mesh.triangles = GenerateTriangles(mesh.triangles);
        }
        if (GUILayout.Button("Y"))
        {
            Vector3[] newVertices = mesh.vertices;
            for (int i = 0; i < newVertices.Length; i++)
            {
                newVertices[i].y = newVertices[i].y * -1;
            }
            mesh.vertices = newVertices;
            mesh.triangles = GenerateTriangles(mesh.triangles);
        }
        if (GUILayout.Button("Z"))
        {
            Vector3[] newVertices = mesh.vertices;
            for (int i = 0; i < newVertices.Length; i++)
            {
                newVertices[i].z = newVertices[i].z * -1;
            }
            mesh.vertices = newVertices;
            mesh.triangles = GenerateTriangles(mesh.triangles);
        }
        GUILayout.EndVertical();
    }

    private int[] GenerateTriangles(int[] oldTriangles)
    {
        int[] newTriangles = new int[oldTriangles.Length];
        for (int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[newTriangles.Length - i - 1] = oldTriangles[i];
        }
        return newTriangles;
    }
}
