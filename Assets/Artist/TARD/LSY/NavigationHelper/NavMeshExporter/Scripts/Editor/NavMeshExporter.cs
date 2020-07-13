// Add by yaukey(yaukeywang@gmail.com) at 2019-02-19.
// Demo editor for exporting unity NavMesh.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif
using UnityEditor;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif
using System.Runtime.InteropServices;
using System.IO;

public class NavMeshExporter
{
    // The exporter api from native plugin.
    [DllImport("NavMeshExporter")]
    private static extern void NavMeshExporter_Unity(
        float[] vertices,
        int vertexCount,
        int[] trianglesIndices,
        int triangleIndexCount,
        int regionId,
        int[] area,
        float[] boundMin,
        float[] boundMax,
        float cellSize,
        float cellHeight,
        float walkableHeight,
        float walkableRadius,
        float walkableClimb,
        [MarshalAs(UnmanagedType.LPStr)] string exportPath
        );

	[MenuItem("TARD/优化/NavMesh/Check NavMesh")]
	private static void CheckNavMesh()
	{
		// Get exported data.
		NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
		Vector3[] vertices = triangulation.vertices;
		Debug.Log (string.Format ("网格顶点数不可超过65535，当前NavMesh顶点数:{0}", vertices.Length));
	}
	[MenuItem("TARD/优化/NavMesh/Export NavMesh")]
    private static void ExportNavMesh()
    {
        // Get exported data.
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        Vector3[] vertices = triangulation.vertices;
        int[] indices = triangulation.indices;
#if UNITY_5_3_OR_NEWER
        int[] areas = triangulation.areas;
#else
        int[] areas = triangulation.layers;
#endif

        if ((0 == vertices.Length) || (0 == indices.Length) || (0 == areas.Length))
        {
            Debug.LogError("There is no NavMesh to export!");
            return;
        }

#if UNITY_5_3_OR_NEWER
        var curScene = EditorSceneManager.GetActiveScene().name;
#else
        var curScene = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
#endif

        string navmeshName = curScene + ".unitynavmesh";
        string savePath = EditorUtility.SaveFilePanel("Export NavMesh", Path.GetDirectoryName(Application.dataPath), navmeshName, "unitynavmesh");
        if (string.IsNullOrEmpty(savePath))
        {
            return;
        }

        // Fill data.
        float[] vertBuffData = new float[vertices.Length * 3];
        int vertIdx = 0;
        foreach (var vertex in vertices)
        {
            vertBuffData[vertIdx++] = vertex.x;
            vertBuffData[vertIdx++] = vertex.y;
            vertBuffData[vertIdx++] = vertex.z;
        }

        // The bound box.
        float[] boundBoxMin = { 0.0f, 0.0f, 0.0f };
        float[] boundBoxMax = { 0.0f, 0.0f, 0.0f };

        // Get build settings.
        float voxelSize = 0.1666667f;
        float voxelHeight = 0.1666667f;
        float agentHeight = 2.0f;
        float agentRadius = 0.5f;
        float agentClimb = 0.4f;

#if UNITY_2017_2_OR_NEWER
        NavMeshBuildSettings setting = NavMesh.GetSettingsByIndex(0);
        voxelSize = setting.voxelSize;
        voxelHeight = setting.voxelSize;
        agentHeight = setting.agentHeight;
        agentRadius = setting.agentRadius;
        agentClimb = setting.agentClimb;
#endif

        // Save NavMesh to file.
        NavMeshExporter_Unity(
            vertBuffData,
            vertices.Length,
            indices, 
            indices.Length, 
            0,
            areas,
            boundBoxMin, 
            boundBoxMax, 
            voxelSize, 
            voxelHeight, 
            agentHeight, 
            agentRadius, 
            agentClimb,
            savePath
            );

        Debug.Log("Export NavMesh to: " + savePath);
    }
}
