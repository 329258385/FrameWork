using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraFogDensityController : MonoBehaviour {

    private int _FogDensityID = Shader.PropertyToID("_FogDensity");
    private float currentFogDensity;

    private void OnEnable()
    {
        currentFogDensity = Shader.GetGlobalFloat(_FogDensityID);        
    }

    private void Update()
    {
        if(Shader.GetGlobalFloat(_FogDensityID) != 0)
            Shader.SetGlobalFloat(_FogDensityID, 0f);
    }

    private void OnDisable()
    {
        Shader.SetGlobalFloat(_FogDensityID, currentFogDensity);
    }
}
