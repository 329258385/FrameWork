using LuaInterface;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class SAO_AA : MonoBehaviour {

    [Space(20)]
    public bool AA = true;
    //[Range(0.01f, 4)] public float AASample = 1;
    public float EdgeThresholdMin = 0.05f;
    public float EdgeThreshold = 0.2f;
    public float EdgeSharpness = 4f;
    private Camera mCam;
    public Material Mat;
    // Use this for initialization
    void Start () {
		
	}
	
//	// Update is called once per frame
//	void OnRenderImage(RenderTexture source, RenderTexture destination) {
//        if (mCam == null)
//        {
//            if (gameObject.GetComponent<Camera>())
//            {
//                mCam = gameObject.GetComponent<Camera>();
//            }
//            //Debug.Log(1);
//        }
//        else
//        {
//            if (AA)
//            {
//                Mat.EnableKeyword("AA");
//                Mat.SetFloat("_EdgeThresholdMin", EdgeThresholdMin);
//                Mat.SetFloat("_EdgeThreshold", EdgeThreshold);
//                Mat.SetFloat("_EdgeSharpness", EdgeSharpness);
//                Graphics.Blit(source, destination, Mat);
//            }
//            else
//            {
//                Mat.DisableKeyword("AA");
//                Graphics.Blit(source, destination);
//            }
//            //Debug.Log(2);
//        }
//	}
}
