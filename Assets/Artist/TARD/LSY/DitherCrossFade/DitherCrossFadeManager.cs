using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitherCrossFadeManager : MonoBehaviour {
	public static DitherCrossFadeManager Ins;
	public Texture ditherTex;
	//[Range(0,100)]
	public static float _FadeFar=40;
	//[Range(0,100)]
	public static float _FadeNear=35;

	void Start () {
		Ins = this;
		Shader.SetGlobalTexture ("_DitherTex", ditherTex);
	}
	void Update () {
		Shader.SetGlobalFloat ("_FadeFar", _FadeFar*pcgTier[TARDSwitches.Quality]);
		Shader.SetGlobalFloat ("_FadeNear", _FadeNear*pcgTier[TARDSwitches.Quality]);
	}

	#region Quality
	public static float[] pcgTier = { 0.6f, 0.8f, 1f };
	#endregion
}
