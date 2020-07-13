using UnityEngine;
using System.Collections;

public class LookAtMainCamera : MonoBehaviour 
{	
	public bool LookMode = false;

	void Update () 
	{
		var cameraPos = Camera.main.transform.position;
		var objPos = this.transform.position;
		var lookV = cameraPos - objPos;
		if (LookMode) 
		{
			var crossValue = Vector3.Cross(Vector3.up, lookV);
			var crossThis = Vector3.Cross(this.transform.up, crossValue);
			this.transform.localRotation = Quaternion.LookRotation(-crossThis, Vector3.up);
		} else 
		{
			this.transform.LookAt(cameraPos, Vector3.up);
		}
	}
}
