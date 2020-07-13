using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RTColorCapturer{
	private static Texture2D t2d;
	public static Color Capture(RenderTexture rt,int x,int y)
	{
		if(t2d ==null)
			t2d = new Texture2D (1, 1);

		RenderTexture.active = rt;
		Rect rect = new Rect (x, rt.height -y, 1, 1);
		t2d.ReadPixels (rect, 0, 0);
		t2d.Apply ();

		RenderTexture.active = null;
		Color col = t2d.GetPixel (0, 0);
		//Debug.Log ("color:"+col);
		return col;
	}


//	public static Color CaptureCenter(RenderTexture rt)
//	{
//		if(t2d ==null)
//			t2d = new Texture2D (1, 1);
//
//
//		RenderTexture.active = rt;
//		int w = rt.width / 2-1;
//		int h = rt.height /2-1;
//		Debug.Log ("w:"+w);
//		Rect rect = new Rect (w, h, 1, 1);
//		t2d.ReadPixels (rect, 0, 0);
//		t2d.Apply ();
//
//		RenderTexture.active = null;
//		Color col = t2d.GetPixel (0, 0);
//		Debug.Log ("color:"+col);
//		return col;
//	}
}
