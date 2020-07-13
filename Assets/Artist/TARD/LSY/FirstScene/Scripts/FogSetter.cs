using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSetter : MonoBehaviour {
	public bool switchOn = false;
	public float duration = 1;
	public float delay = 0;


	public bool setFogDis = true;
	public float farDis;
	public bool setFogAlpha = true;
	public float fogAlpha= 0.8f;


	void Start () {
		if (FSManager.Instance != null) {
			if(setFogDis)
				FSManager.Instance.SetFogDis (farDis,duration,delay);
			if(setFogAlpha)
				FSManager.Instance.SetFogAlpha(fogAlpha,duration,delay);
			if(switchOn)
				Invoke ("Switch", 2f);
		}
	}

	void Switch()
	{
		FSManager.Instance.Switch ();
	}
}
