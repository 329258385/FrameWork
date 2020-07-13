using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZzcRandomGridController : MonoBehaviour {
    public Vector4 centerOffset1;
    public Vector4 centerOffset2;
    public Vector4 centerOffset3;
    public Material mat;
    private int ID1 = Shader.PropertyToID("_CenterOffset1");
    private int ID2 = Shader.PropertyToID("_CenterOffset2");
    private int ID3 = Shader.PropertyToID("_CenterOffset3");
    // Use this for initialization
    void Start () {
		 
	}
	
	// Update is called once per frame
	void Update () {
        mat.SetVector(ID1, centerOffset1);
        mat.SetVector(ID2, centerOffset2);
        mat.SetVector(ID3, centerOffset3);
    }
}
