using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderSetterSwitcher : MonoBehaviour {

    public ShaderSetter ShaderSetter1;
    public ShaderSetter ShaderSetter2;
    public ShaderSetter TargetShaderSetter;
    [Range(0, 1)] public float LerpForce = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ShaderSetter.MixShaderSetter (TargetShaderSetter, ShaderSetter1, ShaderSetter2, LerpForce);
    }
}
