using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowFPSCam : MonoBehaviour {

    private Camera mCam;
    int mCpt = 0;
    RenderTexture mRT;

	// Use this for initialization
	//void Start () {
 //       mCam = gameObject.GetComponent<Camera>();
 //       if (mRT == null)
 //       {
 //           mRT = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2, 24, RenderTextureFormat.ARGB32);
 //       }
 //       mCam.targetTexture = mRT;

 //   }

    void OnEnable()
    {
        mCam = gameObject.GetComponent<Camera>();
        if (mRT == null)
        {
            mRT = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2, 24, RenderTextureFormat.ARGB32);
        }
        mCam.targetTexture = mRT;
        Shader.EnableKeyword("_LDBG");
        Shader.SetGlobalTexture("_GameTex", mRT);
    }

    void OnDisable()
    {
        if (mRT != null)
        {
            RenderTexture.ReleaseTemporary(mRT);
        }
        Shader.DisableKeyword("_LDBG");
    }
	
	// Update is called once per frame
	void Update () {
        if (mCpt % 5 == 0)
        {
            mCpt = 0;
            mCam.Render();
        }
        mCpt++;

    }
}
