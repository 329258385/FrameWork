using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Blur : MonoBehaviour
{

    public Material Mat;
    [Range(1,10)]
    public int DS1,DS2;

    // Use this for initialization
    void Start()
    {

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (Mat != null)
        {
            RenderTexture buffer = RenderTexture.GetTemporary(source.width / DS1, source.height / DS1);
			buffer.name = "TARD Blur buffer";
            RenderTexture buffer2 = RenderTexture.GetTemporary(source.width / DS2, source.height / DS2);
			buffer2.name = "TARD Blur buffer2";
            Graphics.Blit(source, buffer, Mat, 0);
            
            Graphics.Blit(buffer, buffer2, Mat, 1);

            Mat.SetTexture("_BlurTex", buffer2);

            Graphics.Blit(source, destination, Mat, 2);
            RenderTexture.ReleaseTemporary(buffer);
            RenderTexture.ReleaseTemporary(buffer2);
        }

    }
}
