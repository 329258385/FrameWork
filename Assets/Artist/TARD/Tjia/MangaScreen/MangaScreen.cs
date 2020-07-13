using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class MangaScreen : MonoBehaviour {

    public Material MangaMat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, MangaMat);
    }
}
