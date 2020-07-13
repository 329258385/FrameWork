//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;
//using SleekRender;
//
//public class BloomChooser : MonoBehaviour {
//	public PostProcessLayer layer;
//	public FastBloom fb;
//
////	public fb fb;
//	public SleekRenderPostProcess sr;
////	public LsyBloom lb;
//	public Kino.Bloom kino;
//
//	void OnGUI()
//	{
//		GUILayout.BeginHorizontal ();
//		GUILayout.Space (200);
//		float size = 150;
//
//		if (GUILayout.Button ("ObjSpaceFx On",GUILayout.Width(size),GUILayout.Width(size))) {
//			CameraFX.ObjSpaceFx = true;
//		}
//		if (GUILayout.Button ("ObjSpaceFx Off",GUILayout.Width(size),GUILayout.Width(size))) {
//			CameraFX.ObjSpaceFx = false;
//		}
//
//		if (GUILayout.Button ("POST PROCESS",GUILayout.Width(size),GUILayout.Width(size))) {
//			layer.enabled = true;
//			kino.enabled = false;
//			fb.enabled = false;
//			sr.enabled = false;
//		}
//
//
//		if (GUILayout.Button ("Kino Std",GUILayout.Width(size),GUILayout.Width(size))) {
//			layer.enabled = false;
//			kino.enabled = true;
//			fb.enabled = false;
//			sr.enabled = false;
//		}
//
//		if (GUILayout.Button ("fast",GUILayout.Width(size),GUILayout.Width(size))) {
//			layer.enabled = false;
//			kino.enabled = false;
//			fb.enabled = true;
//			sr.enabled = false;
//		}
//		if (GUILayout.Button ("Sleek",GUILayout.Width(size),GUILayout.Width(size))) {
//			layer.enabled = false;
//			kino.enabled = false;
//			fb.enabled = false;
//			sr.enabled = true;
//		}
//		if (GUILayout.Button ("Off",GUILayout.Width(size),GUILayout.Width(size))) {
//			layer.enabled = false;
//			kino.enabled = false;
//			fb.enabled = false;
//			sr.enabled = false;
//		}
//		GUILayout.EndHorizontal ();
//	}
//}
