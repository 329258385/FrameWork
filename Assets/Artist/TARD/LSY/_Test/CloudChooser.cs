using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudChooser : MonoBehaviour {
	public VolumetricClouds3.RaymarchedClouds rayCloud;
	public GameObject lsyCloud;

	void OnGUI()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (150);
		float size = 100;

		if (GUILayout.Button ("ObjSpaceFx True",GUILayout.Width(size),GUILayout.Height(size))) {
			CameraFX.ObjSpaceFx = true;
		}
		if (GUILayout.Button ("ObjSpaceFx False",GUILayout.Width(size),GUILayout.Height(size))) {
			CameraFX.ObjSpaceFx = false;
		}



		if (GUILayout.Button ("rayCloud",GUILayout.Width(size),GUILayout.Height(size))) {
			rayCloud.enabled = true;
			lsyCloud.SetActive (false);
			TARDSwitches.CloudSwitch = true;
		}

		if (GUILayout.Button ("lsy cloud",GUILayout.Width(size),GUILayout.Height(size))) {
			rayCloud.enabled = false;
			lsyCloud.SetActive (true);
			TARDSwitches.CloudSwitch = true;
		}


		if (GUILayout.Button ("Off",GUILayout.Width(size),GUILayout.Height(size))) {
			rayCloud.enabled = false;
			lsyCloud.SetActive (false);
			//TARDSwitches.CloudSwitch = true;
		}
		GUILayout.EndHorizontal ();
	}
}
