using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		RenderSettings.fog = false;

	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (QualitySettings.shadows.ToString ());
	}
}
