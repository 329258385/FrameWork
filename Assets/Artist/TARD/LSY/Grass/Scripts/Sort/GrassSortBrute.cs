using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSortBrute :GrassSortBase {
	protected override void SortAlgorithm (GrassDataChunkDataPerLM d)
	{
		int count = d.count;
		float[] dis = new float[count];
		for (int i = 0; i < count; i++) {
			dis[i] = Vector3.Distance (d.pos [i], GrassRenderer.playerPos);
		}
		for (int i = 0; i < dis.Length; i++) {
			float min= float.MaxValue;
			int minID = -1;
			for (int j = i; j < dis.Length; j++) {
				if (dis [j] < min) {
					min = dis [j];
					minID = j;
				}
			}

			dis [minID] = dis [i];
			dis [i] = min;

			var posMin = d.pos [minID];
			d.pos [minID] = d.pos [i];
			d.pos [i] = posMin;

			var uvMin = d.uv [minID];
			d.uv [minID] = d.uv [i];
			d.uv [i] = uvMin;

			var masMin = d.mas [minID];
			d.mas [minID] = d.mas [i];
			d.mas [i] = masMin;

			d.mas [i] = Matrix4x4.TRS (d.pos[i], Quaternion.identity, Vector3.one*0.05f*i);
		}
	}
}
