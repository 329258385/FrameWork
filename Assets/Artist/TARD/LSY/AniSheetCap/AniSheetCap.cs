using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AniSheetCap : MonoBehaviour {
	public bool awakeRec;
	public int screenRect = 512;
	public float duration=1;
	public int col=2;
	public float FPS{get{return frameCount/duration;}}
	private Material mat;
	public string imagePath;

	private int texSize{get{return screenRect*col;}}
	private int frameCount{get{return col*col;}}
	private bool cap;
	private int currentFrame;
	private Texture2D tex;
	private int frameSize;
	private float targetTime;

	void Start()
	{
		if(awakeRec)
			CapStart ();
	}
	void OnGUI()
	{
		if (GUILayout.Button ("Start Cap")) {
			CapStart ();
		}
	}
	void OnPostRender()
	{
		DrawWireFrame ();
		Cap ();
	}




	#region Cap 
	void CapStart()
	{
		Debug.Log ("Start Rec");
		frameSize = (int)((float)texSize / (float)col);
		currentFrame = 0;
		cap = true;
		tex = CreateTex ();
		targetTime = Time.time;
	}
	void Cap()
	{
		if (!cap)
			return;
		if (currentFrame < frameCount) {
			if (Time.time > targetTime) {
				targetTime += duration / (frameCount-1);
				RenderFrame (tex, currentFrame);
				currentFrame++;
			}
		} else {
			CapEnd ();
		}
	}
	void CapEnd()
	{
		Debug.Log ("End Rec");
		cap = false;
		SaveTex (tex);
	}
	#endregion


	#region Tex process
	Texture2D CreateTex()
	{
		Texture2D t = new Texture2D (texSize, texSize,TextureFormat.RGB24,false);
		return t;
	}
	void RenderFrame(Texture2D t,int frame)
	{
		Debug.Log (string.Format("Rec frame {0} at {1}",frame,Time.time));
		int x = frame % col * frameSize;
		int y = (col- 1 - frame / col) * frameSize;


		int screenX = Screen.width - screenRect;
		int screenY = Screen.height - screenRect;
		screenX /= 2;
		screenY /= 2;
		t.ReadPixels (new Rect (screenX, screenY, screenRect, screenRect),x,y,false);
	}
	void SaveTex(Texture2D t)
	{
		t.Apply ();
		var img = t.EncodeToPNG ();
		File.WriteAllBytes (imagePath, img);
	}
	#endregion

	void DrawWireFrame()
	{
		if (mat == null) {
			mat = new Material (UnityUtils.FindShader ("Unlit/Color"));
			mat.SetColor ("_Color", Color.red);
		}

		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();

		GL.Begin(GL.LINES);
		GL.Color(Color.red);

		int rect = screenRect + 5;
		float screenX = Screen.width - rect;
		screenX /= 2;
		screenX /= Screen.width;

		float screenY = Screen.height - rect;
		screenY /= 2;
		screenY /= Screen.height;

		GL.Vertex(new Vector3(screenX,screenY,0));
		GL.Vertex(new Vector3(screenX,1-screenY,0));

		GL.Vertex(new Vector3(screenX,1-screenY,0));
		GL.Vertex(new Vector3(1-screenX,1-screenY,0));

		GL.Vertex(new Vector3(1-screenX,1-screenY,0));
		GL.Vertex(new Vector3(1-screenX,screenY,0));
	
		GL.Vertex(new Vector3(1-screenX,screenY,0));
		GL.Vertex(new Vector3(screenX,screenY,0));
		GL.End();

		GL.PopMatrix();
	}
}