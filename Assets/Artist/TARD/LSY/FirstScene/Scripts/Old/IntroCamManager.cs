using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamManager : MonoBehaviour {
	public Camera cam;
	public void Start()
	{
		Invoke ("SetMode", 0);
		Invoke ("SetMode", 1);
	}

	public void SetMode()
	{
		cam.depthTextureMode = DepthTextureMode.DepthNormals;
		//	cam.depthTextureMode = DepthTextureMode.Depth;
	}
}
