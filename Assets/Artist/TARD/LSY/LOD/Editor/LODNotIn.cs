//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//
//public class LODNotIn : MonoBehaviour {
//
//	[MenuItem("TARD/优化/LOD/MR不在LOD下排查")]
//	public static void Init()
//	{
//		List<Object> objs = new List<Object> ();
//
//		var mrs = FindObjectsOfType<MeshRenderer> ();
//		foreach (var mr in mrs) {
//			if (mr.enabled == false)
//				continue;
//			var g = mr.GetComponentInParent<LODGroup> ();
//			if (g == null) {
//				objs.Add (mr.gameObject);
//			}
//		}
//		Selection.objects = objs.ToArray ();
//	}
//}
