using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;

[ExecuteInEditMode]
public class GrassSpawner : MonoBehaviour {
	#if UNITY_EDITOR
	public GrassBrush brush;
	public Material mat;
	protected TerrainGrassInfo grassInfo;
	public GrassSpawnerDummy dummy;

	protected 
	void OnEnable()
	{
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}

	void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	void OnSceneGUI(SceneView sceneView)
	{
		Hotkeys ();
		BasicGUI ();
		SceneView.RepaintAll();
	}

	void Hotkeys()
	{
		if (Event.current.keyCode == KeyCode.E) {
			brush.mode = GrassBrushMode.Erase;
		}
		if (Event.current.keyCode == KeyCode.W) {
			brush.mode = GrassBrushMode.Paint;
		}
		SizeModule ();
	}

	bool GetRightHit(Ray ray,out RaycastHit hit)
	{
		RaycastHit[] infos;
		infos = Physics.RaycastAll (ray, 2000, 1 << 28 | 1 << 17);

		for (int i = 0; i < infos.Length; i++) {
			var temp = infos[i].collider.GetComponentInChildren<TerrainGrassInfo> ();
			if (temp != null) {
				hit =  infos [i];
				return true;
			}
		}
		hit = new RaycastHit ();
		return false;
	}

	void BasicGUI()
	{
		Vector3 mousePosition = Event.current.mousePosition;
		Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
		mousePosition = ray.origin;

		RaycastHit info;
		if (!GetRightHit (ray,out info))
			return;

		grassInfo = info.collider.GetComponentInChildren<TerrainGrassInfo> ();

		dummy.info = grassInfo;
		dummy.Show (Event.current.control);
		if (!Event.current.control)
			return;

		var mouseWorldPosition = info.point;
		Transform t = info.collider.transform;



		if (brush.mode == GrassBrushMode.Erase) {
			Handles.color = Color.red;
		} else if (brush.mode == GrassBrushMode.Paint) {
			Handles.color = Color.green;
		}
		Handles.DrawSolidDisc (mouseWorldPosition, Vector3.up, brush.size);
		Handles.color = Color.white;

		if (GetMouseDown (1, true)) {
			//Debug.Log ("GetMouseDown");
			Process (info);
		}
		if (GetMouse (1)) {
			//Debug.Log ("GetMouse");
			Process (info);
		}
	}

	void Process(RaycastHit info)
	{
		if (grassInfo != null) {
			Texture2D t = grassInfo.grassTex;
			grassInfo.grassTex = GeneratePNG (t);

			Edit (grassInfo,info);
		}
	}


	/// <summary>
	/// Step 1: GeneratePNG
	/// </summary>
	Texture2D GeneratePNG(Texture2D t)
	{
		string path = AssetDatabase.GetAssetPath (t);
		//Debug.Log (path);
		string[] str = path.Split ('.');
		if (str [str.Length - 1] != "png") {
			int len = path.Length - str [str.Length - 1].Length - 1;
			string newPath = path.Substring (0, len) + "PNG.png";
			return SaveTexture (t, newPath);
		} else {
			return t;
		}
	}

	/// <summary>
	/// Step 2: Edit
	/// </summary>
	void Edit(TerrainGrassInfo tInfo,RaycastHit info)
	{
		brush.texPaint01Size = brush.size / info.collider.bounds.size.x;
		Vector2 uv = info.textureCoord;
		uv = new Vector2 (uv.x, 1 - uv.y);
		GrassTextureProcess.DrawPoint (tInfo.grassTex, uv, brush);
	}

	public void Save()
	{
		grassInfo.grassTex = SaveTexture (grassInfo.grassTex);
	}
	/// <summary>
	/// Step 3: Save
	/// </summary>
	protected Texture2D SaveTexture(Texture2D t,string newPath = null)
	{
		if(newPath==null)
			newPath = AssetDatabase.GetAssetPath (t);
		string prePath = Application.dataPath;
		prePath = prePath.Substring (0, prePath.Length - 6);
		string fullPath = prePath + newPath;
		Debug.Log (fullPath);

		Texture2D pngTex = new Texture2D (t.width, t.height, TextureFormat.ARGB32, false);
		pngTex.SetPixels (t.GetPixels ());
		var bs = pngTex.EncodeToPNG ();
		File.WriteAllBytes (fullPath, bs);


		AssetDatabase.Refresh ();
		GrassTextureProcess.TextureReImport (newPath, true);
		Texture2D newT2d = (Texture2D)AssetDatabase.LoadMainAssetAtPath (newPath);

		return newT2d;
	}




	public static bool GetMouseDown(int id,bool containUsed = true)
	{
		//Event.current.type   outwindow->ignore   clickTextureField->used
		//Debug.Log (Event.current.type + " "+ Event.current.rawType);
		if (Event.current.rawType == EventType.MouseDown ||(containUsed && Event.current.rawType == EventType.Used)) {
			if (Event.current.button == id) {
				return true;
			}
		}
		return false;
	}

	public static bool GetMouseUp(int id)
	{
		if (Event.current.rawType == EventType.MouseUp) {
			if (Event.current.button == id) {
				return true;
			}
		}
		return false;
	}

	public static bool GetMouse(int id)
	{
		if (Event.current.rawType == EventType.MouseDrag) {
			if (Event.current.button == id) {
				return true;
			}
		}
		return false;
	}



	public void SizeModule()
	{
		if (Event.current.isKey) {
			if (Event.current.keyCode == KeyCode.LeftBracket) {
				DeSize ();
			}
			if (Event.current.keyCode == KeyCode.RightBracket) {
				AddSize ();
			}
		}
	}
	public void AddSize()
	{
		SetSize (brush.size + 0.2f);
	}
	public void DeSize()
	{
		SetSize (brush.size -0.2f);
	}
	public void SetSize(float newSize)
	{
		brush.size = Mathf.Clamp (newSize, 0.2f, 100f);
	}
	#endif
}
