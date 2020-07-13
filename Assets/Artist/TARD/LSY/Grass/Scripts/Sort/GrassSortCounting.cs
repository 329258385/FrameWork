using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSortCounting :GrassSortBase {
	protected override void SortAlgorithm (GrassDataChunkDataPerLM d)
	{
		int count = d.count;
		for (int i = 0; i < count; i++) {
			//d.dis[i] = (int)Vector3.Distance (d.pos [i], GrassRenderer.playerPos)*10;

			var x = d.pos [i];
			var y = GrassRenderer.playerPos;

			var xx = (x.x - y.x);
			var yy = (x.y - y.y);
			var zz = (x.z - y.z);

			d.dis[i] = (int)(xx*xx+yy*yy+zz*zz*10); 
		}
		var a = d.dis;


		//1 min max
		int max = int.MinValue;
		int min = int.MaxValue;
		for (int i = 0; i < a.Length; i++) {
			if (a [i] > max)
				max = a [i];
			if (a [i] < min)
				min = a [i];
		}

		//2 count
		int[] c = new int[max-min+1]; 
		for (int i = 0; i < a.Length; i++) {
			int index = a [i] - min;
			c [index]++;
		}
		int cLength = c.Length;
		//3 accum
		for (int i = 1; i < cLength; i++) {
			c [i] += c [i - 1];
		}


		for (int i = 0; i < a.Length; i++) {
			int indexInB = (c [a [i]- min]--) -1;
			d.posB [indexInB] = d.pos [i];
			d.masB [indexInB] = d.mas [i];
			d.uvB [indexInB] = d.uv [i];
		}
		d.Swap ();
	}
}
