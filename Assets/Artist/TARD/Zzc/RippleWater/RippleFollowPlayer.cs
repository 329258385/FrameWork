using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode][DisallowMultipleComponent]
public class RippleFollowPlayer : MonoBehaviour {
    public RenderTexture stepRT;
    public RenderTexture tempRT;
    public Material stepMat;

    private Vector3 lastPlayerPos;

    private void OnEnable()
    {
        if (stepRT)
        {
            tempRT = RenderTexture.GetTemporary(stepRT.descriptor);
        }        
        lastPlayerPos = transform.position;
    }

    private void OnDisable()
    {
        if (tempRT&&stepRT&&stepMat)
        {
            RenderTexture.ReleaseTemporary(tempRT);
            tempRT = null;
            Graphics.Blit(tempRT, stepRT, stepMat, 1);
        }        
    }

    void Update () {

        if (!ZzcFind.Instance.playerRippleOn)
        {
            return;
        }

        if (!tempRT&&stepRT)
        {
            tempRT = RenderTexture.GetTemporary(stepRT.descriptor);
        }

        if (stepRT&&tempRT&&stepMat)
        {
            Shader.SetGlobalVector("_PlayerPos", transform.position);
            Shader.SetGlobalVector("_DeltaPos", transform.position - lastPlayerPos);
            Graphics.Blit(stepRT, tempRT);
            Graphics.Blit(tempRT, stepRT, stepMat, 0);
            Shader.SetGlobalTexture("_PlayerRippleBump", stepRT);
            lastPlayerPos = transform.position;
        }        
	}
}
