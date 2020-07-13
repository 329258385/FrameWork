using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderRep : MonoBehaviour {

    public Shader RShader;
    public Material RMaterial;

	// Use this for initialization
	void Start () {
        foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
        {
            if (RShader != null)
            {
                mr.material.shader = RShader;
            }
            else if(RMaterial!=null)
            {
                mr.material = RMaterial;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
