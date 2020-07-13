using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderChanger : MonoBehaviour {

#if UNITY_EDITOR

    [NoToLua]
    public GameObject go;
    [NoToLua]
    public Shader OriginShader;
    [NoToLua]
    public Shader NewShader;
    [NoToLua]
    public bool Change = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Change)
        {
            Change = false;
            if (go != null)
            {
                OriginShader = go.GetComponent<Renderer>().sharedMaterial.shader;
            }
            foreach (Renderer rd in FindObjectsOfType<Renderer>())
            {
                if (rd.sharedMaterials != null)
                {
                    foreach (Material mat in rd.sharedMaterials)
                    {
                        if (mat != null && mat.shader != null && mat.shader == OriginShader && OriginShader != null && NewShader !=null)
                        {
                            mat.shader = NewShader;
                        }
                    }
                }
            }
        }
	}
#endif
}
