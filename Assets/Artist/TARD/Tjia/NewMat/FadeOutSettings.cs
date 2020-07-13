using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FadeOutSettings : MonoBehaviour {

    public Vector2 Tree = new Vector2(0.3f, 8.0f);
    public Vector2 Flower = new Vector2(0.3f, 4.0f);
    public Vector2 Flag = new Vector2(0.3f, 4.0f);
    public Vector2 Opaque = new Vector2(0.3f, 3.0f);
    public Vector2 StaticCutoff = new Vector2(0.3f, 3.0f);
    public Vector2 Rock = new Vector2(0.3f, 3.0f);

    float mCachedValue = -1;
	
	// Update is called once per frame
	void Update ()
    {
        float value = Tree.y + Flower.y + Flag.y + Opaque.y + StaticCutoff.y + Rock.y;
        if (value != mCachedValue)
        {
            mCachedValue = value;
            Shader.SetGlobalVector("_TreeFadeOut", Tree);
            Shader.SetGlobalVector("_FlowerFadeOut", Flower);
            Shader.SetGlobalVector("_FlagFadeOut", Flag);
            Vector2 opaqueInput = Opaque;
            opaqueInput.y = Mathf.Max(opaqueInput.y, 2f);
            Shader.SetGlobalVector("_OpaqueFadeOut", opaqueInput);
            Shader.SetGlobalVector("_StaticCutoffFadeOut", StaticCutoff);
            Shader.SetGlobalVector("_RockFadeOut", Rock);
        }
    }
}
