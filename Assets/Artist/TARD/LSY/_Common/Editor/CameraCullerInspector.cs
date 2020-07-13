using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraCuller))]
public class CameraCullerInspector : Editor {
	float disAll = 500;
	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI ();
		CameraCuller cull = (CameraCuller)target;
		Camera cam = cull.gameObject.GetComponent<Camera> ();
		if (cam != null) {
			//1 Far Plane
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Far Plane",GUILayout.Width(120));
			cam.farClipPlane = EditorGUILayout.FloatField (cam.farClipPlane);
			GUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();

			//2 Layers
			for (int i = 0; i < cull.dis.Length; i++) {
				string nm = LayerMask.LayerToName (i);
				if (string.IsNullOrEmpty (nm))
					continue;
				
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format("{0} {1}",i,nm),GUILayout.Width(120));
				cull.dis[i] = EditorGUILayout.FloatField (cull.dis [i]);

				GUILayout.EndHorizontal ();
			}
			EditorGUILayout.Separator ();

			if (GUILayout.Button ("应用")) {
				cull.Apply ();
			}
			GUILayout.Space (10);

			//3 Reset
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("重置:",GUILayout.Width(120))) {
				cull.SetAll (disAll);
			}
			disAll = EditorGUILayout.FloatField (disAll);
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("重置:0 (使用默认FarPlane)")) {
				cull.Reset ();
			}
		}
	}
}
