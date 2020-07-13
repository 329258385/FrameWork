using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShadowCloser : MonoBehaviour {
    public bool CloseShadow = true;
	void OnEnable ()
    {
        CloseShadowFunc();
    }

    private void CloseShadowFunc()
    {
        MeshRenderer[] rs = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rs.Length; i++)
        {
            if (CloseShadow)
            {
                rs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            else
            {
                rs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }
    private void OnValidate()
    {
        CloseShadowFunc();
    }
}
