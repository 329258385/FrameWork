using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtTestCharControl : MonoBehaviour {
	public bool syncMain = false;
	#if UNITY_EDITOR
	void Start () {
		transform.parent = null;
		transform.rotation = Quaternion.identity;

		Camera cam = GetComponentInChildren<Camera> ();
		var cull = GameObject.FindObjectOfType<CameraFX> ();
		if (cull != null) {
			Camera mainCam = cull.GetComponent<Camera> ();

			if (syncMain) {
				transform.position = mainCam.transform.position;
				transform.rotation = mainCam.transform.rotation;
			}
			mainCam.transform.parent = cam.transform;
			mainCam.transform.localPosition = Vector3.zero;
			mainCam.transform.localRotation =  Quaternion.identity;
			cam.enabled = false;
		
		}
	}
	#else
	void Start () {
		Destroy (gameObject);	
	}
	#endif
}
