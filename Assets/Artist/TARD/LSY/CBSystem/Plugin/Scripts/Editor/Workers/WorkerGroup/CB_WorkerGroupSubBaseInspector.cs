using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CB_WorkerGroupSubBase))]
public class CB_WorkerGroupSubBaseInspector : Editor {
	CB_WorkerGroupSubBase worker;
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		worker = (CB_WorkerGroupSubBase)target;
		GUILayout.Space (50);
		GUILayout.Label ("[材质参数]");

		CB_WorkerBaseInspector.ShowParams ("", worker.matParam, worker.mat,worker);
	}
}