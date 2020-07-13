using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCuller : MonoBehaviour {
	public float[] dis = new float[32];
	private Camera cam;

	void Start () {
		Apply ();
	}

	public void Apply()
	{
		cam = GetComponent<Camera> ();
        for (int i = 0; i < dis.Length; i++)
        {
            dis[i] = 10000;
        }
        if (cam != null)
            cam.layerCullDistances = dis;
	}
	public void Reset()
	{
		SetAll (0);
	}
	public void SetAll(float v)
	{
		for (int i = 0; i < dis.Length; i++) {
			dis [i] = v;
		}
	}
}
