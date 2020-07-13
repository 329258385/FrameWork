using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LsyWaterRefCam : MonoBehaviour {
	public Material mat;
	public int iter = 4;
	public float length = 6; 
	void OnRenderImage(RenderTexture source,RenderTexture dest)
	{
		if (mat == null) {
			Graphics.Blit (source, dest);
			return;
		}

		mat.SetFloat("BlurLength", length / Screen.height);

		int hw = (int)source.width / 2;
		int hh = (int)source.height / 2;
		RenderTexture rt = RenderTexture.GetTemporary (hw, hh, 0, source.format);
		rt.name = "Lsy RT-Complex Water Blur";
		rt.filterMode = FilterMode.Bilinear;
		Graphics.Blit (source,rt,mat);

		for(int i=0;i<iter;i++)
		{
			RenderTexture rt2 = RenderTexture.GetTemporary (hw, hh, 0, source.format);
			rt2.name = "Lsy RT-Complex Water Blur Tap";
			rt.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt,rt2,mat);
			RenderTexture.ReleaseTemporary (rt);

			rt = RenderTexture.GetTemporary (hw, hh, 0, source.format);
			rt.name = "Lsy RT-Complex Water Blur Tap";
			rt.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt2,rt,mat);
			RenderTexture.ReleaseTemporary (rt2);
		}

		Graphics.Blit (rt,dest,mat);
		RenderTexture.ReleaseTemporary (rt);
	}
}
