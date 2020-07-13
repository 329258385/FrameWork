using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CB_WorkerGroupBase))]
public class CB_WorkerGroupBaseInspector : CB_WorkerBaseInspector {
	CB_WorkerGroupBase worker;
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		worker = (CB_WorkerGroupBase)target;
		CB_WorkerBaseInspector.ShowParams ("模糊参数", worker.matParamsOther[0], worker.mat_2_blur,worker);
	}
}
