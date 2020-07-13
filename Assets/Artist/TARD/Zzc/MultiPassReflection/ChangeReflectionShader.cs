using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeReflectionShader : MonoBehaviour {
    public Shader reflectionShader;
    public Shader oriShader;
    private SkinnedMeshRenderer [] smr;

    private void Awake()
    {
        reflectionShader = UnityUtils.FindShader("Zzc/ZzcCharactorMultiPassReflection");
        oriShader = UnityUtils.FindShader("SAO_TJia_V3/NewChara/NewCharaMain");
    }

    private void OnEnable()
    {
        smr = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (reflectionShader!=null)
        {
            for (int i = 0; i < smr.Length; i++)
            {
                for (int ii = 0; ii < smr[i].materials.Length; ii++)
                {
                    smr[i].materials[ii].shader = reflectionShader;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (oriShader != null)
        {
            for (int i = 0; i < smr.Length; i++)
            {
                for (int ii = 0; ii < smr[i].materials.Length; ii++)
                {
                    smr[i].materials[ii].shader = oriShader;
                }
            }
        }
    }
}
