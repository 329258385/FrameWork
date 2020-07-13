using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardMatChanger : MonoBehaviour {

    public sShowState ShowState = sShowState.Albedo;
    public Shader BillboardShader;
    public Material SkyMat;
    
    public enum sShowState
    {
        Albedo,
        Normal,
        Alpha
    };

    private float mCachedLod = 0;

	// Use this for initialization
	void OnEnable () {

        mCachedLod = QualitySettings.lodBias;
        QualitySettings.lodBias = 10000;

        foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
        {
            foreach (Material mat in mr.materials)
            {
                float cutoff = 0.5f;
                if (mat.shader.renderQueue < 2100)
                {
                    cutoff = -0.1f;                    
                }
                mat.shader = BillboardShader;
                mat.SetFloat("_Cutoff", cutoff);
            }
        }
	}

    void OnDisable()
    {
        QualitySettings.lodBias = mCachedLod;
    }
	
	// Update is called once per frame
	void Update () {
        switch (ShowState)
        {
            case sShowState.Albedo:
                Shader.SetGlobalFloat("_Mode_Albedo", 1);
                Shader.SetGlobalFloat("_Mode_Normal", 0);
                Shader.SetGlobalFloat("_Mode_Alpha", 0);
                SkyMat.SetColor("_Color", Color.black);
                break;
            case sShowState.Normal:
                Shader.SetGlobalFloat("_Mode_Albedo", 0);
                Shader.SetGlobalFloat("_Mode_Normal", 1);
                Shader.SetGlobalFloat("_Mode_Alpha", 0);
                SkyMat.SetColor("_Color", new Color(0.75f, 0.75f, 1, 0));
                break;
            case sShowState.Alpha:
                Shader.SetGlobalFloat("_Mode_Albedo", 0);
                Shader.SetGlobalFloat("_Mode_Normal", 0);
                Shader.SetGlobalFloat("_Mode_Alpha", 1);
                SkyMat.SetColor("_Color", Color.black);
                break;
        }

    }
}
