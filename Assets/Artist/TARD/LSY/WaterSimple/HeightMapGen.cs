using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum HeightMapGenState
{
	idle,
	building,
	done
}

public class HeightMapGen : MonoBehaviour {
	public string folder = "Artist/TARD/LSY/WaterSimple/Res";
	public float  rayY=10f;
	public float maxDepth=2;
	public int sampleCount=6;
	public float  sampleSize=0.05f;

	public int size = 32;
	public int blurStep=1;

	//List<Vector3> pts = new List<Vector3> ();
	Collider col;
	Material mat;
	Shader shader;
	Bounds bounds;
	int layer;
	public HeightMapGenState State{get{return state;}}
	[HideInInspector]
	[SerializeField]
	private HeightMapGenState state = HeightMapGenState.idle;
	public float BuildProgess{get{return buildProgess;}}
	[HideInInspector]
	[SerializeField]
	private float buildProgess=0;

	public string FileName
	{
		get{ 
			return string.Format("{0}/{1}-{2}",folder,SceneManager.GetActiveScene().name,gameObject.name);
		}
	}
	public string MapName
	{
		get{ 
			return string.Format("{0}.png",FileName);
		}
	}

	public void Cancel()
	{
		state = HeightMapGenState.idle;
	}

	public IEnumerator Build()
	{
		state = HeightMapGenState.building;
		yield return Create ();
		state = HeightMapGenState.done;
	}
		
	public IEnumerator Create()
	{
		yield return null;
		#if UNITY_EDITOR
		//pts.Clear();
		layer = ~(1 << 2);
		shader = UnityUtils.FindShader ("SAO_TLsy/Water/WaterSimple");
		AddCol();

		var render =  GetComponent<MeshRenderer> ();
		mat = render.sharedMaterial;

		float height = transform.position.y;
		float xStart = bounds.min.x;
		float zStart = bounds.min.z;
		float xEnd = bounds.max.x;
		float zEnd = bounds.max.z;


		float xUnit = (xEnd - xStart)/size;
		float zUnit = (zEnd - zStart)/size;


		Texture2D t2d = new Texture2D (size, size);
		for (int i = 0; i < size; i++) {
			buildProgess = (float)i / (float)size * 0.5f;
			yield return null;
			for (int j = 0; j < size; j++) {
				Color color = new Color (0, 0, 0, 0);

				var pos = new Vector3 (
					xStart+xUnit*i,
					height+rayY,
					zStart+zUnit*j);
				GetWaterHeight(pos,ref height);
				float rayHeight = GetRayHeight(pos);
				float alpha = Mathf.Clamp01 ((height - rayHeight) / maxDepth);
				color = new Color (alpha,alpha,alpha,alpha );
				t2d.SetPixel (i, j, color);
			}
		}
		t2d.Apply ();
		yield return Texture2DBlur(t2d);

		var bs = t2d.EncodeToPNG ();
		File.WriteAllBytes(Application.dataPath+"/"+MapName,bs);
		string adbPath_tex = "Assets/"+MapName;
		AssetDatabase.ImportAsset(adbPath_tex);
		Texture2D t = (Texture2D)AssetDatabase.LoadMainAssetAtPath (adbPath_tex);

		Material newMat = new Material (mat);
		newMat.shader = shader;
		AssetDatabase.CreateAsset (newMat, "Assets/"+FileName + ".asset");
		render.sharedMaterial = newMat;
		newMat.SetTexture("_EdgeAlphaTex",t);
		newMat.SetVector("_AABB",new Vector4(xStart,xEnd-xStart,zStart,zEnd-zStart));
		RemoveCol();
		#endif
	}





	void GetWaterHeight(Vector3 pos,ref float height)
	{
		int layer = (1 << 2);
		RaycastHit info;
		if (Physics.Raycast (pos, Vector3.down, out info,float.MaxValue,layer)) {
			height = info.point.y;
			//pts.Add (info.point);
		}
	}

	void AddCol()
	{
		RemoveCol ();
		col = gameObject.AddComponent<MeshCollider> ();
		var cb = col.bounds;
		bounds = cb;
	}
	void RemoveCol()
	{
		col = GetComponent<Collider> ();
		if (col != null) {
			DestroyImmediate (col);
		}
	}

	float GetRayHeight(Vector3 pos)
	{
		float height = float.MinValue;
		RaycastHit info;
		for (int i = -sampleCount; i < sampleCount+1; i++) {
			for (int j = -sampleCount; j < sampleCount+1; j++) {
				if (Physics.Raycast (pos+ new Vector3(sampleSize*i,0,sampleSize*j), Vector3.down, out info,float.MaxValue,layer)) {
					var h = info.point.y;
					if (h > height)
						height = h;
				}
			}
		}
		return height;
	}

	IEnumerator Texture2DBlur(Texture2D t2d)
	{
		for (int i = 0; i < size; i++) {
			buildProgess = 0.5f + (float)i / (float)size * 0.5f;
			yield return null;

			for (int j = 0; j < size; j++) {
				Texture2DBlurPixel (t2d,i, j);
			}
		}
		t2d.Apply ();
	}


	void Texture2DBlurPixel(Texture2D t2d,int x,int y)
	{
		float[,] f = {
			{0.00000067f ,0.00002292f ,0.00019117f ,0.00038771f ,0.00019117f ,0.00002292f ,0.00000067f},
			{0.00002292f ,0.00078633f ,0.00655965f ,0.01330373f ,0.00655965f ,0.00078633f ,0.00002292f},
			{0.00019117f ,0.00655965f ,0.05472157f ,0.11098164f ,0.05472157f ,0.00655965f ,0.00019117f},
			{0.00038771f ,0.01330373f ,0.11098164f ,0.22508352f ,0.11098164f ,0.01330373f ,0.00038771f},
			{0.00019117f ,0.00655965f ,0.05472157f ,0.11098164f ,0.05472157f ,0.00655965f ,0.00019117f},
			{0.00002292f ,0.00078633f ,0.00655965f ,0.01330373f ,0.00655965f ,0.00078633f ,0.00002292f},
			{0.00000067f ,0.00002292f ,0.00019117f ,0.00038771f ,0.00019117f ,0.00002292f ,0.00000067f}
		} ;

		float alpha = 0;
		for (int i = -3; i < 4; i++) {
			for (int j = -3; j < 4; j++) {
				int xx = Mathf.Clamp (i*blurStep + x, 0, size);
				int yy = Mathf.Clamp (j*blurStep + y, 0, size);

				alpha+= t2d.GetPixel(xx,yy).a* f[i+3,j+3];
			}
		}
		Color color = new Color (alpha,alpha,alpha,alpha );
		t2d.SetPixel (x, y, color);
	}


	void OnDrawGizmosSelected()
	{
		float xStart = bounds.min.x;
		float zStart = bounds.min.z;
		float xEnd = bounds.max.x;
		float zEnd = bounds.max.z;
		float height = transform.position.y;

		Gizmos.color = Color.red;
		Gizmos.DrawLine (new Vector3 (xStart, height, zStart), new Vector3 (xEnd, height, zStart));
		Gizmos.DrawLine (new Vector3 (xStart, height, zEnd), new Vector3 (xEnd, height, zEnd));

		Gizmos.DrawLine (new Vector3 (xStart, height, zStart), new Vector3 (xStart, height, zEnd));
		Gizmos.DrawLine (new Vector3 (xEnd, height, zStart), new Vector3 (xEnd, height, zEnd));

//		for (int i = 0; i < pts.Count; i++)
//		{
//			if (i % 100 == 0) {
//				Gizmos.DrawSphere (pts[i], 1);
//			}
//		}
		Gizmos.color = Color.white;
	}
}
	