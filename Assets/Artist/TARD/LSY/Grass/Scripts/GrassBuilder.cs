using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using LuaInterface;
using DataStructures.ViliWonka.KDTree;

[System.Serializable]
public class GrassBuilderSetting
{
	public int genTextureSize = 512;	
	public Vector3 MapCenter;
	public float MapSize = 80;
	[Range(0.1f,2f)]
	public float Density = 2;
	public GameObject[] prefabsLOD;
	public int ChunkSize = 1023;
	public float yDeltaCoef = 0;
	[Range(0,1)]
	public float lightmapIntensity=1;
	//	[Range(0,10)]
	//	public float windForce=3.5f;
	//	[Range(0,10)]
	//	public float windSpeed=1;
	[Range(0,1)]
	public float _Shadow = 1f;
	[Range(0,1)]
	public float _NormalDistorion=0f;
	[Range(0f, 1.1f)]
	public float _CutoffValue = 0.5f;
	[Range(0, 50)]
	public float _Translucency = 1f;
	[Range(1, 50)]
	public float _TransScattering = 1f;
	[Range(0,1)]
	public float _LightMapBlend = 0.5f;
	//	[Range(0,10)]
	//	public float _WForce = 2;
	//    public Vector4 _WDir = new Vector4(1, 0, 0, 0);
	//    public Vector4 _WDirSubSpdTiling = new Vector4(1, 50, 2, 10);
	[Range(0, 2)]
	public float _FootPrintRate = 0.5f;
	[Range(0f, 0.5f)]
	public float _HeightY = 0.3f;
	[Range(-3f, 3f)]
	public float _HeightFactor = -3f;
	[Range(0f, 1f)]
	public float _SmoothFactor = 0f;

	public Material grassFXMat;
	[Range(0f, 10f)]
	public float _ShadowRelight = 1f;
}

public enum GrassBuildState
{
	idle,
	building,
	done
}

public class GrassBuilder : MonoBehaviour {
	public static GrassBuilder Instance;
	//[NoToLua]
	public GrassBuilderSetting settings;
	private GrassRenderer GrassRender{
		get{ 
			if(grassRender==null)
				grassRender = GetComponentInChildren<GrassRenderer> ();
			return grassRender;
		}
	}
	private GrassRenderer grassRender;
	private GrassDataHolder holder;
	public GrassData data{
		get{ 
			if(holder ==null)
				holder = GetComponentInChildren<GrassDataHolder> ();
			return holder.data;
		}
	}

	private int LightMapCount{get{return LightmapSettings.lightmaps.Length;}}
	private Ray mRay;
	[HideInInspector]
	[SerializeField]
	private Vector3 mOrigon;
	private RaycastHit mHitInfo;
	[HideInInspector]
	[SerializeField]
	private float SquareLength;
	[HideInInspector]
	[SerializeField]
	private float mHalfLength;
	private Vector3 mPos;
	private Vector3 mScale;
	private Quaternion mRot;
	private Matrix4x4 transMatrix;
	[HideInInspector]
	[SerializeField]
	private Vector3 yDelta;
	[HideInInspector]
	[SerializeField]
	private Material matOrgin;

	public GrassBuildState State{get{return state;}}
	[HideInInspector]
	[SerializeField]
	private GrassBuildState state = GrassBuildState.idle;
	public float BuildProgess{get{return buildProgess;}}
	[HideInInspector]
	[SerializeField]
	private float buildProgess=0;
	private int LODCount;

	//private float texAlpha=0f;
	public static bool buildError;
	void Start()
	{
		Instance = this;
	}

	#region Build
	public IEnumerator Build()
	{
		buildProgess = 0;
		state = GrassBuildState.building;
		Init ();
		yield return BuildChunkInfos ();
		GrassRender.Clean ();
		state = GrassBuildState.done;

		buildProgess = 0.9f;
	}

	public void SetProgress(float f)
	{
		buildProgess = f;
	}

	public void InitForRender()
	{
		mRay.origin = mOrigon;
		mRay.direction = Vector3.down;
	}

	public float CalChunkSize()
	{
		int mGrassQuantity = (int)(settings.MapSize * settings.MapSize * 4 * settings.Density);
		int SquareNumber = mGrassQuantity / settings.ChunkSize;
		var SquareLength = settings.MapSize * 2 / Mathf.Sqrt((float)SquareNumber);
		return SquareLength;
	}
	private void Init()
	{
		//matOrgin = settings.Grass.GetComponentInChildren<MeshRenderer> ().sharedMaterial;
		SquareLength = CalChunkSize ();
		mHalfLength = SquareLength * 0.5f;
		//yDelta = Vector3.up * 0.49f * settings.Grass.transform.localScale.y * settings.yDeltaCoef;
		yDelta = Vector3.zero;
		mRay.direction = Vector3.down;
		mOrigon.y = Camera.main.transform.position.y + 500;
		//mRot = settings.Grass.transform.rotation;
		mRot = Quaternion.identity;
		//data.mats = new Material[LightMapCount+1];
		//data.mesh = settings.Grass.GetComponentInChildren<MeshFilter> ().sharedMesh;

		LODCount = settings.prefabsLOD.Length;
		data.lods = new GrassDataLOD[LODCount];
		for (int i = 0; i < LODCount; i++) {
			data.lods [i] = new GrassDataLOD ();
			data.lods [i].mesh = settings.prefabsLOD [i].GetComponentInChildren<MeshFilter> ().sharedMesh;
			data.lods [i].matOrigin = settings.prefabsLOD [i].GetComponentInChildren<MeshRenderer> ().sharedMaterial;
		}
		CreateMaterial ();
	}

	public void Cancel()
	{
		state = GrassBuildState.idle;
	}

	private IEnumerator BuildChunkInfos()
	{
		List<Vector3> chunkPosList = new List<Vector3>();
		for (float ix = settings.MapCenter.x - settings.MapSize; ix < settings.MapCenter.x + settings.MapSize; ix = ix + SquareLength)
		{
			for (float iz = settings.MapCenter.z - settings.MapSize; iz < settings.MapCenter.z + settings.MapSize; iz = iz + SquareLength)
			{
				chunkPosList.Add(new Vector3(ix + mHalfLength, 0, iz + mHalfLength));
			}
		}

		List<Vector3> posNeed = new List<Vector3> ();

		int count = chunkPosList.Count;
		for (int v = 0; v < count; v++)
		{
			Vector3 pos = chunkPosList[v];

			try{
				bool need = PrepareChunk (ref pos);
				if (need) {
					posNeed.Add (pos);
				}
			}
			catch(System.Exception e) {
				Debug.LogError (e);
				GrassBuilder.buildError = true;
			}

			buildProgess = (float)v / (float)count;
			yield return null;
		}

		data.chunkPos = posNeed.ToArray ();
	}

	private bool PrepareChunk(ref Vector3 centerVector)
	{
		Vector2 texUV;
		float channel = 0;

		//for (int i = 0; i < settings.ChunkSize; i++) {
		for (int i = 0; i < 30; i++) {
			if (i == 0) {
				mOrigon.x = centerVector.x;
				mOrigon.z = centerVector.z;
			} else {
				mOrigon.x = Random.Range (centerVector.x - mHalfLength, centerVector.x + mHalfLength);
				mOrigon.z = Random.Range (centerVector.z - mHalfLength, centerVector.z + mHalfLength);
			}
			mRay.origin = mOrigon;

			//Todo add layermask after [grass placement tex] fixed
			if (Physics.Raycast (mRay, out mHitInfo, 2000, 1 << 28 | 1 << 17)) {
				//if (Physics.Raycast (mRay, out mHitInfo, 2000)) {
				TerrainGrassInfo info = mHitInfo.collider.GetComponentInParent<TerrainGrassInfo> ();
				if (info == null || info.grassTex ==null)
					continue;
				//				if (!data.terrainInfo.Contains (info)) {
				//					data.terrainInfo.Add (info);
				//					data.terrainLMIndex.Add (mHitInfo.collider.GetComponentInChildren<MeshRenderer> ().lightmapIndex);
				//
				//					var bs = mHitInfo.collider.bounds;
				//					bs.size = new Vector3 (bs.size.x+1, bs.size.y+1, bs.size.z+1);
				//					data.terrainBounds.Add (bs);
				//				}
				mPos = mHitInfo.point;
				texUV = mHitInfo.textureCoord;
				channel = info.grassTex.GetPixel ((int)(texUV.x * (float)info.width), (int)(texUV.y * (float)info.height)).r;

				//Return true just when it is possible to have grass
				if (channel > 0)
				{
					//int lmIndex = mHitInfo.collider.GetComponentInChildren<MeshRenderer> ().lightmapIndex;
					//CreateMaterial (lmIndex);
					centerVector.y = mHitInfo.point.y;
					return true;
				}
			}
		}
		return false;
	}

	int XYtoIndex(int width,int height,int x,int y)
	{
		//return (height - y - 1) * width + x;
		return width*(y) + x;
	}

	//	Color GetColorBili(float uvx,float uvy,int w,int h)
	//	{
	//		int x1 = Mathf.FloorToInt (uvx * w);
	//		int x2 = Mathf.FloorToInt (uvx * w);
	////
	////
	////		int idx = (int)(uvx * spawnMapTexWidth);
	////		int idy = (int)(uvy * spawnMapTexHeight);
	////		idx = Mathf.Clamp (idx,0, spawnMapTexWidth);
	////		uvy = Mathf.Clamp (idy,0, spawnMapTexHeight);
	////		//lsy Todo  dont use GetPixel
	////		//var c = spawnMapTex.GetPixel (idx, idy);
	////		var cid = XYtoIndex(spawnMapTexWidth,spawnMapTexHeight,idx,idy);
	////		var c = colors[cid];
	//	}

	Color[] colors;
	bool initColors = false;
	public IEnumerator GenChunk(GrassDataChunk chunk,int steps,Texture2D spawnMapTex)
	{
		if (initColors == false) {
			initColors = true;
			colors = spawnMapTex.GetPixels ();
		}
		var spawnMapTexWidth = spawnMapTex.width;
		var spawnMapTexHeight = spawnMapTex.height;

		var data = holder.data;
		float channel = 0;
		var centerVector = chunk.center;
		Vector2 texUV; 
		Vector2 lmUV; 

		Matrix4x4[] mas = new Matrix4x4[settings.ChunkSize];
		Vector4[] uv = new Vector4[settings.ChunkSize];
		Vector3[] pos = new Vector3[settings.ChunkSize];

		int index = 0;
		for (int i = 0; i < settings.ChunkSize; i++) {
			if (steps>=0 && (i % steps == 0))
				yield return null;

			mOrigon.x = Random.Range (centerVector.x - mHalfLength, centerVector.x + mHalfLength);
			mOrigon.z = Random.Range (centerVector.z - mHalfLength, centerVector.z + mHalfLength);
			mRay.origin = mOrigon;
			mPos = new Vector3(mOrigon.x,0,mOrigon.z);

			///////////////////////////
			/// Get Color
			///////////////////////////
			float uvx = (mPos.x - (settings.MapCenter.x - settings.MapSize)) / (settings.MapSize * 2);
			float uvy = (mPos.z - (settings.MapCenter.z - settings.MapSize)) / (settings.MapSize * 2);
			uvx = Mathf.Clamp (uvx,0f, 1f);
			uvy = Mathf.Clamp (uvy,0f, 1f);

			int idx = (int)(uvx * spawnMapTexWidth);
			int idy = (int)(uvy * spawnMapTexHeight);
			idx = Mathf.Clamp (idx,0, spawnMapTexWidth);
			idy = Mathf.Clamp (idy,0, spawnMapTexHeight);

			//lsy Todo  dont use GetPixel
			var c = spawnMapTex.GetPixelBilinear (uvx, uvy);
			//var cid = XYtoIndex(spawnMapTexWidth,spawnMapTexHeight,idx,idy);
			//var c = colors[cid];
			///////////////////////////
			///////////////////////////
			///////////////////////////

			channel = c.r;
			var height = data.heightMin + c.g*(data.heightMax-data.heightMin);

			//1 Pos from map
			mPos = new Vector3(mPos.x,height-0.03f,mPos.z);
			//2 Pos from raycast
			//			if (Physics.Raycast (mRay, out mHitInfo, 2000, 1 << 28 | 1 << 17)) {
			//				mPos = mHitInfo.point;
			//			}


			if (channel > 0 && CanPlace(channel)) {
				float scaleModi = Mathf.Clamp01 (channel);
				float scaleModi2 = scaleModi * scaleModi;
				mPos += yDelta * scaleModi;
				mScale = Random.Range (0.9f, 1.2f) * settings.prefabsLOD [0].transform.localScale;

				//1 For map
				//Rotation by terrain up vector
				var x = c.b * 2 - 1;
				var z = c.a * 2 - 1;
				//var up = new Vector3 (x, Mathf.Sqrt( 1-x*x-z*z),z);
				var up = new Vector3 (x,1,z);
				mRot = Quaternion.Euler (0, Random.Range (0, 360), 0);
				//Debug.Log (string.Format("{0} {1} {2}",up.x,up.y,up.z));
				mRot = Quaternion.FromToRotation(Vector3.up,up)*mRot;

				//2 For raycast
				//mRot = Quaternion.Euler (0, Random.Range (0, 360), 0);


				transMatrix = Matrix4x4.TRS (mPos, mRot, mScale);
				//int lmIndex = mHitInfo.collider.GetComponentInChildren<MeshRenderer> ().lightmapIndex;
				//int lmIndex = data.GetLMIndex(mPos);

				//CreateMaterial (lmIndex);

				//				if (!chunk.dataLMIndex.Contains (lmIndex)) {
				//					chunk.dataLMIndex.Add (lmIndex);
				//					chunk.data.Add (new GrassDataChunkDataPerLM ());
				//				}
				//				int id = FindIndex(chunk.dataLMIndex,lmIndex);
				//				if (id >= mas.Count) {
				//					mas.Add (new List<Matrix4x4> ());
				//					uv.Add (new List<Vector4> ());
				//					pos.Add (new List<Vector3> ());
				//				}
				//				mas [id].Add (transMatrix);
				//				pos [id].Add (mPos);
				//				//uv [id].Add (new Vector4 (lmUV.x, lmUV.y, channel, 0));
				//				uv [id].Add (new Vector4 (0, 0, channel, 0));


				mas[index] = transMatrix;
				pos[index]= mPos;
				//uv [id].Add (new Vector4 (lmUV.x, lmUV.y, channel, 0));
				uv[index] = new Vector4 (0, 0, channel, 0);
				++index;
			}
		}


		/////////
		var item = chunk.data;
		int count = index;

		item.count = count;

		item.mas = mas;
		item.uv = uv;
		item.pos = pos;
		item.masB = new Matrix4x4[count];
		item.uvB = new Vector4[count];
		item.posB = new Vector3[count];

		item.dis = new int[count];


	}

	void OnDrawGizmosSelected()
	{
		var size = settings.MapSize * 2;
		Gizmos.DrawWireCube (settings.MapCenter,new Vector3(size,200,size));
	}
	#endregion


	#region common
	private void CreateMaterial()
	{
		for (int i = 0; i < data.lods.Length; i++) {
			var lod = data.lods [i];
			if (lod.mat == null) {
				var mat = new Material (lod.matOrigin);
				lod.mat= mat;
				SetMaterial (lod);
			}
		}
	}
	public void UpdateMaterial()
	{
		for (int i = 0; i < data.lods.Length; i++) {
			var lod = data.lods [i];

			Material mat = lod.mat;
			if (mat != null) {
				SetMaterial (lod);
			}
		}
	}
	private void SetMaterial(GrassDataLOD lod)
	{
		Material mat = lod.mat;
		mat.SetFloat ("_lmapBlend",settings.lightmapIntensity);

		//		mat.SetFloat ("_WindForce", settings.windForce);
		//		mat.SetFloat ("_WindSpeed", settings.windSpeed);


		mat.SetFloat("_Shadow", settings._Shadow);
		mat.SetFloat("_NormalDistortion", settings._NormalDistorion);
		mat.SetFloat("_Cutoff", settings._CutoffValue);
		mat.SetFloat("_Translucency", settings._Translucency);
		mat.SetFloat("_TransScattering", settings._TransScattering);
		mat.SetFloat("_lmapBlend", settings._LightMapBlend);
		//		mat.SetFloat("_WForce", settings._WForce);
		//		mat.SetVector("_WDir", settings._WDir);
		//		mat.SetVector("_WDirSubSpdTiling", settings._WDirSubSpdTiling);
		mat.SetFloat("_HeightY", settings._HeightY);
		mat.SetFloat("_HeightFactor", settings._HeightFactor);
		mat.SetFloat("_SmoothFactor", settings._SmoothFactor);
		mat.SetFloat("_ShadowRelight", settings._ShadowRelight);
	}


	//	private int FindIndex(List<int> array,int v)
	//	{
	//		for (int i = 0; i < array.Count; i++) {
	//			if (array [i] == v)
	//				return i;
	//		}	
	//		return 0;
	//	}
	private bool CanPlace(float channel)
	{
		if (Random.Range (0f, 1f) < channel) 
			return true;
		return false;
	}


	#endregion
}