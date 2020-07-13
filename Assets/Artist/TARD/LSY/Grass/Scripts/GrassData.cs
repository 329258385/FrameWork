using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Info
[System.Serializable]
public class GrassData
{
	public float heightMin;
	public float heightMax;


	public Texture2D texSpawnMap;
	public Texture2D texLM;
	public Texture2D texSM;
	public GrassDataLOD[] lods;
	public Vector3[] chunkPos;
}

[System.Serializable]
public class GrassDataLOD
{
	public Mesh mesh;

	public Material matOrigin;
	public Material mat;
}

#endregion

#region RunTime
public enum SortState
{
	idle,
	inQueue,
	done
}

public class GrassDataChunk
{
	public SortState sortState = SortState.idle;
	public Vector3 lastDir;

	public Vector3 center;
	public GrassDataChunkDataPerLM data = new GrassDataChunkDataPerLM ();
}
	
public class GrassDataChunkDataPerLM
{
	public int count;
	public MaterialPropertyBlock props;
	public Matrix4x4[] mas;
	public Vector4[] uv;
	public Vector3[] pos;

	public int[] dis;
	public Matrix4x4[] masB;
	public Vector4[] uvB;
	public Vector3[] posB;

	public void Swap()
	{
		var tempP = posB;
		var tempM = masB;
		var tempU = uvB;
		posB = pos;
		masB = mas;
		uvB = uv;

		pos = tempP;
		mas = tempM;
		uv = tempU;
	}
}
#endregion