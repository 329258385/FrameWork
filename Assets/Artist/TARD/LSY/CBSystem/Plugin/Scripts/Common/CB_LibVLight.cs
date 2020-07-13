using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class CB_LibVLight {
	public static Mesh CreateSphereMesh()
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var mesh = go.GetComponent<MeshFilter>().sharedMesh;
		GameObject.Destroy(go);
		return mesh;
	}

	public static Mesh CreateSpotLightMesh()
	{
		// copy & pasted from other project, the geometry is too complex, should be simplified
		Mesh mesh = new Mesh();

		const int segmentCount = 16;
		Vector3[] vertices = new Vector3[2 + segmentCount * 3];
		Color32[] colors = new Color32[2 + segmentCount * 3];

		vertices[0] = new Vector3(0, 0, 0);
		vertices[1] = new Vector3(0, 0, 1);

		float angle = 0;
		float step = Mathf.PI * 2.0f / segmentCount;
		float ratio = 0.9f;

		for (int i = 0; i < segmentCount; ++i)
		{
			vertices[i + 2] = new Vector3(-Mathf.Cos(angle) * ratio, Mathf.Sin(angle) * ratio, ratio);
			colors[i + 2] = new Color32(255, 255, 255, 255);
			vertices[i + 2 + segmentCount] = new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 1);
			colors[i + 2 + segmentCount] = new Color32(255, 255, 255, 0);
			vertices[i + 2 + segmentCount * 2] = new Vector3(-Mathf.Cos(angle) * ratio, Mathf.Sin(angle) * ratio, 1);
			colors[i + 2 + segmentCount * 2] = new Color32(255, 255, 255, 255);
			angle += step;
		}

		mesh.vertices = vertices;
		mesh.colors32 = colors;

		int[] indices = new int[segmentCount * 3 * 2 + segmentCount * 6 * 2];
		int index = 0;

		for (int i = 2; i < segmentCount + 1; ++i)
		{
			indices[index++] = 0;
			indices[index++] = i;
			indices[index++] = i + 1;
		}

		indices[index++] = 0;
		indices[index++] = segmentCount + 1;
		indices[index++] = 2;

		for (int i = 2; i < segmentCount + 1; ++i)
		{
			indices[index++] = i;
			indices[index++] = i + segmentCount;
			indices[index++] = i + 1;

			indices[index++] = i + 1;
			indices[index++] = i + segmentCount;
			indices[index++] = i + segmentCount + 1;
		}

		indices[index++] = 2;
		indices[index++] = 1 + segmentCount;
		indices[index++] = 2 + segmentCount;

		indices[index++] = 2 + segmentCount;
		indices[index++] = 1 + segmentCount;
		indices[index++] = 1 + segmentCount + segmentCount;

		//------------
		for (int i = 2 + segmentCount; i < segmentCount + 1 + segmentCount; ++i)
		{
			indices[index++] = i;
			indices[index++] = i + segmentCount;
			indices[index++] = i + 1;

			indices[index++] = i + 1;
			indices[index++] = i + segmentCount;
			indices[index++] = i + segmentCount + 1;
		}

		indices[index++] = 2 + segmentCount;
		indices[index++] = 1 + segmentCount * 2;
		indices[index++] = 2 + segmentCount * 2;

		indices[index++] = 2 + segmentCount * 2;
		indices[index++] = 1 + segmentCount * 2;
		indices[index++] = 1 + segmentCount * 3;

		////-------------------------------------
		for (int i = 2 + segmentCount * 2; i < segmentCount * 3 + 1; ++i)
		{
			indices[index++] = 1;
			indices[index++] = i + 1;
			indices[index++] = i;
		}

		indices[index++] = 1;
		indices[index++] = 2 + segmentCount * 2;
		indices[index++] = segmentCount * 3 + 1;

		mesh.triangles = indices;
		mesh.RecalculateBounds();
		//For show gizmos
		mesh.RecalculateNormals ();
		return mesh;
	}
}
