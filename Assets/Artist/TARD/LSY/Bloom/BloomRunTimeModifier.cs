using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BloomRunTimeModifier : MonoBehaviour {
	public static float BloomEpsilon =0.0005f;
    void Update()
	{
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
        if (Application.platform == RuntimePlatform.Android) {
			if (SystemInfo.graphicsDeviceVendor == "Qualcomm") {
				Shader.SetGlobalFloat ("BloomEpsilon", BloomEpsilon);
			}
		}
	}
}