using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialAnimation : MonoBehaviour
{
    public List<Material> materials = new List<Material>();

    public Vector4 firstMaterialTilingOffset = new Vector4(1f, 1f, 0f, 0f);
    public Vector4 secondMaterialTilingOffset = new Vector4(1f, 1f, 0f, 0f);
    public Vector4 thirdMaterialTilingOffset = new Vector4(1f, 1f, 0f, 0f);
    public Vector4 forthMaterialTilingOffset = new Vector4(1f, 1f, 0f, 0f);

    private bool isInstantiated = false;


    private void Start()
    {

#if UNITY_EDITOR

#endif

        materials.Clear();

        if (GetComponent<SkinnedMeshRenderer>().materials.Length>=1)
        {
            materials.Add( GetComponent<SkinnedMeshRenderer>().materials[0]);
        }
        if (GetComponent<SkinnedMeshRenderer>().materials.Length >= 2)
        {
            materials.Add(GetComponent<SkinnedMeshRenderer>().materials[1]);
        }
        if (GetComponent<SkinnedMeshRenderer>().materials.Length >= 3)
        {
            materials.Add(GetComponent<SkinnedMeshRenderer>().materials[2]);
        }
        if (GetComponent<SkinnedMeshRenderer>().materials.Length >= 4)
        {
            materials.Add(GetComponent<SkinnedMeshRenderer>().materials[3]);
        }
    }

    private void Update()
    {

        if (materials.Count >= 1)
        {
            materials[0].SetVector("_MainTex_ST", firstMaterialTilingOffset);
        }
        if (materials.Count >= 2)
        {
            materials[1].SetVector("_MainTex_ST", secondMaterialTilingOffset);
        }
        if (materials.Count >= 3)
        {
            materials[2].SetVector("_MainTex_ST", thirdMaterialTilingOffset);
        }
        if (materials.Count >= 4)
        {
            materials[3].SetVector("_MainTex_ST", forthMaterialTilingOffset);
        }
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

}
