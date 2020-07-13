using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BloomMenu {

	[MenuItem("TARD/Bloom/Add")]
	private static void AddBloom()
	{
		var sobj = Selection.activeGameObject;
		if (sobj == null)
			return;
		Kino.Bloom b = sobj.GetComponent<Kino.Bloom>();
		if (b == null) {
			b = sobj.AddComponent<Kino.Bloom>();
			b.thresholdGamma = 0.5f;
			b.softKnee = 0.5f;
			b.intensity = 0.06f;
			b.radius = 2.5f;
//			b.highQuality = false;
			b.antiFlicker = true;
		}
	}
}
