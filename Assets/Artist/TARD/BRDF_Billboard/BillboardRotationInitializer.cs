using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardRotationInitializer : MonoBehaviour {

    public bool Calculate = true;
    public Shader BillboardShader;


	// Use this for initialization
	void Start () {

        BillboardShader = UnityUtils.FindShader("SAO_TJia_V3/Obj/BRDF_Billboard");
        Calculate = true;

}
	
	// Update is called once per frame
	void Update () {

        if (Calculate)
        {

            BillboardShader = UnityUtils.FindShader("SAO_TJia_V3/Obj/BRDF_Billboard");

            Calculate = false;
            foreach (Renderer mr in FindObjectsOfType<Renderer>())
            {
                if (mr!=null 
                    && mr.sharedMaterial != null 
                    && mr.sharedMaterial.shader!=null 
                    && mr.sharedMaterial.shader == BillboardShader)
                {
                    mr.transform.rotation = Quaternion.identity;
                }
            }
#if UNITY_EDITOR
            DestroyImmediate(this);
#else
        Destroy(this);
#endif
        }
    }
}
