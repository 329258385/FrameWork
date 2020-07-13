using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures.ViliWonka.Heap;

public class GrassSortHeap :GrassSortBase {
	protected override void SortAlgorithm (GrassDataChunkDataPerLM d)
	{
		int count = d.count;
		List<float> dis = new List<float> (count);
		MaxHeap<int> minHeap = new MaxHeap<int>(count);
		for (int i = 0; i < count; i++) {
			dis.Add (Vector3.Distance (d.pos [i], GrassRenderer.playerPos));
			minHeap.PushObj (i,Vector3.Distance (d.pos [i], GrassRenderer.playerPos));
		}
		List<int> rs = new List<int> ();
		minHeap.FlushResult (rs);

		for (int i = 0; i < rs.Count; i++) {
			int indexInB =  rs [i];
			d.posB [i] = d.pos [indexInB];
			d.masB [i] = d.mas [indexInB];
			d.uvB [i] = d.uv [indexInB];
		}
		d.Swap ();
	}
}
