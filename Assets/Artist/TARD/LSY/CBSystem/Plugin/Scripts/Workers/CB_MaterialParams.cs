using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CB_MaterialParams{
	[ColorUsage(true,true)]
	public Color color = Color.white;
	[Range(0,10)]
	public float internsity = 1;
	[Range(0,1)]
	public float range = 1;
	[Range(0,3)]
	public float blurSize = 2;

	public float fogStart = 0;
	public float fogEnd = 10;

	public static readonly string KeyColor = "_Color";
	public static readonly string KeyInternsity = "_Intensity";
	public static readonly string KeyRange="_Range01";
	public static readonly string KeyBlur="_Parameter";

	public static readonly string KeyFogStart="_FogStart";
	public static readonly string KeyFogEnd="_FogEnd";

	public void Sync(Material mat)
	{
		if (mat == null)
			return;
		if (mat.HasProperty (KeyColor))
			mat.SetColor (KeyColor, color);
		
		if (mat.HasProperty (KeyInternsity))
			mat.SetFloat (KeyInternsity, internsity);
		
		if (mat.HasProperty (KeyRange))
			mat.SetFloat (KeyRange, range);

		if (mat.HasProperty (KeyBlur))
			mat.SetVector (KeyBlur, new Vector4(blurSize,-blurSize,0,0));


		if (mat.HasProperty (KeyFogStart))
			mat.SetFloat (KeyFogStart, fogStart);
		if (mat.HasProperty (KeyFogEnd))
			mat.SetFloat (KeyFogEnd, fogEnd);
	}
}
