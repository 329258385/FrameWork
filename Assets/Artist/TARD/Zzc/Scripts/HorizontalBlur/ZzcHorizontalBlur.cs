using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ZzcHorizontalBlur : MonoBehaviour {

    public Material mat;
    [Range(0f,1f)]
    public float size;
    [Range(0,8)]
    public int horizontalDownSample=8;
    [Range(0, 4)]
    public int verticalDownSample = 4;
    private RenderTexture rt;
    private int id=Shader.PropertyToID("_Size");

    // Use this for initialization
    void Start () {
		
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mat==null)
        {
            return;
        }        

        if (size<=0f)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            RenderTexture rt = RenderTexture.GetTemporary(2688 >> horizontalDownSample, 1242 >> verticalDownSample, 0, source.format);
            Graphics.Blit(source, rt);
            mat.SetFloat(id, size);
            Graphics.Blit(rt, destination, mat, 0);
            RenderTexture.ReleaseTemporary(rt);
        }        
    }
}
