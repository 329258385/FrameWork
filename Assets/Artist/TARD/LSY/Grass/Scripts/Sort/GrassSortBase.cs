using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSortBase{
	public void Sort(GrassDataChunkDataPerLM d)
	{
		if (d.count == 0) {
			Debug.LogError ("No need to sort");
			return;
		}
		SortAlgorithm (d);
//		for (int i = 0; i < d.count; i++) {
//			d.mas [i] = Matrix4x4.TRS (d.pos[i], Quaternion.identity, Vector3.one*0.05f*i);
//		}
	}
	protected virtual void SortAlgorithm(GrassDataChunkDataPerLM d)
	{

	}
}
