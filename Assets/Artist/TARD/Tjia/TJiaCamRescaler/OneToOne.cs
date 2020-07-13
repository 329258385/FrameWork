using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneToOne : MonoBehaviour {

    public Material OneToOneMat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, OneToOneMat);
	}
}
