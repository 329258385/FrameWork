using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImpostorsAssist : MonoBehaviour {
	public bool manual = false;
	[Range(0,360)]
	public float cameraAngle;
	[HideInInspector]
	public Transform root;
	[HideInInspector]
	public Transform rootLine;
	[HideInInspector]
	public Transform lineStart;
	[HideInInspector]
	public Transform lineEnd;
	public void Rotate()
	{
		if (manual)
			return;
		root.localEulerAngles = new Vector3 (0, cameraAngle, 0);
	}
	public void RotateBack()
	{
		if (manual)
			return;
		root.localEulerAngles = new Vector3 (0, 0, 0);
	}

	void Update()
	{
		if(rootLine!=null)
			rootLine.localEulerAngles = new Vector3 (0, -cameraAngle, 0);
	}
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if(lineStart!=null && lineEnd!=null)
			Gizmos.DrawLine (lineStart.position, lineEnd.position);
		Gizmos.DrawSphere (lineEnd.position, 0.5f);
		Gizmos.color = Color.white;
	}
}
