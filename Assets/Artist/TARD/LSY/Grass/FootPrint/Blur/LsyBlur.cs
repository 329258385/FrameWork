using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LsyBlur : MonoBehaviour {
	[Range(0, 2)]
	public int downsample = 2;

	public enum BlurType {
		StandardGauss = 0,
		SgxGauss = 1,
	}

	[Range(0.0f, 10.0f)]
	public float blurSize = 3.0f;
	[Range(1, 4)]
	public int blurIterations = 2;
	public BlurType blurType= BlurType.StandardGauss;
	public Material blurMaterial = null;

	public void OnRenderImage (RenderTexture source, RenderTexture destination) {
		//lsy
		downsample = 2;

		float widthMod = 1.0f / (1.0f * (1<<downsample));

		blurMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
		source.filterMode = FilterMode.Bilinear;

		int rtW = source.width >> downsample;
		int rtH = source.height >> downsample;

		// downsample
		RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
		rt.name = "Lsy RT-Blur downsample";
		rt.filterMode = FilterMode.Bilinear;
		Graphics.Blit (source, rt, blurMaterial, 0);

		var passOffs= blurType == BlurType.StandardGauss ? 0 : 2;

		for(int i = 0; i < blurIterations; i++) {
			float iterationOffs = (i*1.0f);
			blurMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

			// vertical blur
			RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.name = "Lsy RT-Blur Tap";
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, blurMaterial, 1 + passOffs);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;

			// horizontal blur
			rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.name = "Lsy RT-Blur Tap";
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, blurMaterial, 2 + passOffs);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
		}

		Graphics.Blit (rt, destination);
		RenderTexture.ReleaseTemporary (rt);
	}



	#region Quality
	private static List<LsyBlur> ins = new List<LsyBlur> ();
	void Awake()
	{
        Debug.Log("[LsyBlur]: " + GlobalGameDefine.mIsForceDisableAnyBloom);
#if BUILD_SINGLESCNE_MODE
            GlobalGameDefine.mIsForceDisableAnyBloom = true;
            if (GlobalGameDefine.mIsForceDisableAnyBloom) { Destroy(this); return; }
#endif
        if (GlobalGameDefine.mIsForceDisableAnyBloom) { Destroy(this); return; }

        if (!ins.Contains (this)) {
			ins.Add (this);
		}
		SetQuality (TARDSwitches.Quality);
	}

	void OnDestroy()
	{
		if (ins.Contains (this)) {
			ins.Remove (this);
		}
	}
	public static void SetQualityAll(int q)
	{
		foreach (var item in ins) {
			item.SetQuality (q);	
		}
	}
	public void SetQuality(int q)
	{
		enabled = q > 0;		
	}
	#endregion
}
