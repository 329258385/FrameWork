using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using LuaInterface;
using DataStructures.ViliWonka.KDTree;
using UnityEngine.Rendering;

[System.Serializable]
public class GrassRendererSetting
{
	public float DisCache {get{ return RealDisVisible+20f;}}
	private float disVisible = 50;
	public float DisVisible {get{return disVisible;}}
	public float RealDisVisible{get{ return DisVisible * GrassRenderer.pcgTier [TARDSwitches.Quality];}}
	public float DisSurround{get{ return 25f;}}
	public float[] lodDis;
	[Range(0,1)]
	public float backView = 0.1f; 
//	[Range(0,110)]
//	public float visibleAngle = 60f; 
	public float visibleAngle
	{
		get{ return 80;}
	}
	public void SetDis(float d)
	{
		disVisible = d;
	}
	public int steps_Visible = 200;
	public int steps_Cache = 100;
}

public enum ChunkState
{
	idle,
	queueView,
	queueCache,
	done
}

[ExecuteInEditMode]
public class GrassRenderer : MonoBehaviour {
	public static GrassRenderer Instance;
	//[NoToLua]
	public GrassRendererSetting settings;
	//[NoToLua]
	public Transform PlayerCamera;

	public bool forceInitPos = false;
	public Transform forceInitPosPoint;
	private float forceInitPosTime = 10;

	private GrassBuilder Builder{
		get{ 
			if(builder==null)
				builder = GetComponentInChildren<GrassBuilder> ();
			return builder;
		}
	}
	private GrassBuilder builder;
	private GrassDataHolder holder;
	private GrassData data{
		get{ 
			if(holder ==null)
				holder = GetComponentInChildren<GrassDataHolder> ();
			return holder.data;
		}
	}

	private int count;
	private GrassDataChunk[] chunks;
	private ChunkState[] chunkState;
	private List<int> queue;

	private KDTree kdTree;
	private KDQuery kdQuery;
	private List<int> kdResults;
	public static Vector3 playerPos;
    [Range(0,1)] public float MultiGrassColor = 0;
    private Vector3 playerForward;
	private int LODCount;
	#if UNITY_EDITOR
	int grassRenderingCount;
	#endif
	private void Awake()
	{
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
        Instance = this;
		bool isDisableGrass = LuaGConfigManager.GetGConfig<bool>("G_IS_DISABLE_GPUGRASS");
		if (isDisableGrass) { this.gameObject.SetActive(false); return; }
//#if BUILD_SINGLESCNE_MODE
//        PlayerCamera = GameObject.Find("客户端测试节点_请勿删除/saoUIJoyPadCom/Camera").GetComponent<Camera>().transform;
//        return;
//#endif

        PlayerCamera = Camera.main.transform;
    }
    private void Start () {
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
        if (data.texSpawnMap == null) {
			gameObject.SetActive (false);
			#if UNITY_EDITOR
			Debug.LogError ("草烘焙失败,请检查草设置(如TerrainInfo贴图是否开读写)");
			#endif
		}
		Builder.InitForRender ();
        Builder.UpdateMaterial();
	}

	void OnEnable()
	{
		TARDSceneInit.AddMissionAync (SceneInit);
	}

	void OnDisable()
	{
		TARDSceneInit.RemoveMissionAync (SceneInit);
	}

#region Scene Init
	IEnumerator SceneInit()
	{
		yield return null;
		while(!CheckInit())
			yield return null;
	}

	bool CheckInit()
	{
		if (chunks == null)
			return false;
		if(kdResults == null || kdResults.Count<=0)
			return false;
		int listCount = kdResults.Count;
		for (int i = 0; i < listCount; i++) {
			int index = kdResults [i];
			if (chunkState [index] != ChunkState.done)
				return false;
		}
		return true;
	}
#endregion


	bool Switch()
	{
#if UNITY_EDITOR
			return TARDSwitchesPC.GrassSwitch();
#else
			return TARDSwitches.GrassSwitch;
#endif
	}


	private void Update () {
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
#if UNITY_EDITOR
        if (Builder == null || data == null || data.chunkPos == null || data.chunkPos.Length == 0) {
			Debug.LogError ("草的数据丢失！请重新Build草.");
			return;
		}
		if (Builder.State != GrassBuildState.done)
			return;
#endif

		if (!Switch ())
			return;
		
		Init ();

		UpdatePlayerPos ();
		RenderKDTree ();
		CacheKDTree ();
        Shader.SetGlobalFloat("_MultiGrassColor", MultiGrassColor);
	}

	void UpdatePlayerPos()
	{
		playerPos = PlayerCamera.position;
		//playerPos.y = data.chunkPos [0].y;
		playerForward = PlayerCamera.forward;


		if (forceInitPos && forceInitPosTime>0) {
			forceInitPosTime -= Time.deltaTime;
			playerPos = forceInitPosPoint.position;
			playerForward = forceInitPosPoint.forward;
		}
	}

	void Init()
	{
		if (chunks != null)
			return;
		LODCount = settings.lodDis.Length;
		count = data.chunkPos.Length;
		chunks = new GrassDataChunk[count];
		queue = new List<int> ();
		chunkState = new ChunkState[count];
		InitKDTree ();
		StartCoroutine (ProcessQueue ());
		StartCoroutine (Sort_ProcessQueue ());
	}
	public void Clean()
	{
		chunks = null;
	}
		
	private void RendChunk(int index)
	{
		if (chunks [index] == null)
			return;
		var chunk = chunks[index];
		int lod = CalLod (chunk);


		if (Sort_Need (chunk))
			Sort_AddToQueue (chunk);

		var chunkDataLM = chunk.data;
		//int lmIndex = chunk.dataLMIndex [i];
		if (chunkDataLM.props == null) {
			chunkDataLM.props = new MaterialPropertyBlock ();
			chunkDataLM.props.SetVectorArray ("lmuv", chunk.data.uv);
		}

		#if UNITY_EDITOR
		grassRenderingCount += chunkDataLM.mas.Length;
		//Debug.LogError (string.Format ("{0} - {1} :{2}", index, i, chunk.data [i].mas.Count));
		#endif


		Graphics.DrawMeshInstanced(data.lods[lod].mesh, 0,data.lods[lod].mat,chunkDataLM.mas,chunkDataLM.count,chunkDataLM.props,
			ShadowCastingMode.Off,true
		);
	}

	int CalLod(GrassDataChunk chunk)
	{
		float dis = Vector3.Distance (chunk.center, playerPos);
		for (int i = 0; i < LODCount; i++) {
			if (dis < settings.lodDis [i]) {
				return i;
			}
		}
		return LODCount - 1;
	}
#region Queue
	void Enqueue(int i, ChunkState state)
	{
		chunkState [i] = state;
		queue.Add (i);
	}

	int Dequeue()
	{
		int qc = queue.Count;
		int index = -1;
		for (int i = 0; i < qc; i++) {
			index = queue[i];
			if(chunkState[index] == ChunkState.queueView || i==qc-1)
			{
				queue.RemoveAt (i);
				return index;
			}
		}
		return -1;
	}
	IEnumerator ProcessQueue()
	{
		while (true) {
			if (!Switch ())
				yield return null;


			if (queue.Count > 0) {
				int index = Dequeue();
				GrassDataChunk chunk = new GrassDataChunk ();
				chunk.center = data.chunkPos [index];
				int steps = CalSteps (index);
				yield return Builder.GenChunk (chunk,steps,data.texSpawnMap);
				chunks [index] = chunk;
				chunkState [index] = ChunkState.done;
			} else {
				yield return null;
			}
		}
	}

	int CalSteps(int index)
	{
		if(TARDSceneInit.isLoading)
			return -1;
		int steps = chunkState[index] == ChunkState.queueView ? settings.steps_Visible:settings.steps_Cache;
		return steps;
	}
#endregion

#region KDTree
//	/// <summary>
//	/// Need to be fixed
//	/// </summary>
//	public void IsOnGrass(Vector3 pos)
//	{
//		kdResults.Clear ();
//		float halfsize = builder.CalChunkSize () * 0.5f;
//
//		List<int> ids = new List<int> ();
//		List<float> dis = new List<float> ();
//		kdQuery.ClosestPoint (kdTree, pos,ids,dis);
//		var chunk = chunks [ids [0]];
//		//chunk.
//
//		//Debug.Log (halfsize + " " + dis [0]);
//
//		var gDis = float.MaxValue;
//		foreach (var item in chunk.data) {
//			var _gDis = Vector3.Distance(new Vector2(pos.x,pos.z),new Vector2(item.pos[0].x,item.pos[0].z));
//			Debug.Log ("pos:"+pos +" "+"pos:"+item.pos[0]);
//
//			if (_gDis < gDis)
//				gDis = _gDis;
//		}
//		Debug.Log ("gDis"+gDis);
//	}


	void InitKDTree()
	{
		kdTree = new KDTree (data.chunkPos);
		kdQuery = new KDQuery (); 
		kdResults = new List<int> ();
	}


	private void RenderKDTree()
	{
#if UNITY_EDITOR
		grassRenderingCount = 0;
		//Debug.LogError ("RenderKDTree");
#endif

		//Clear all queue
		for (int i = 0; i < queue.Count; i++) {
			chunkState [queue [i]] = ChunkState.queueCache;
		}

		kdResults.Clear ();
		kdQuery.Radius (kdTree, playerPos, settings.RealDisVisible, kdResults);
		int listCount = kdResults.Count;
		for (int i = 0; i < listCount; i++) {
			int index = kdResults [i];

			bool canSee = false;
			var pos = data.chunkPos [index];
			float dis = Vector3.Distance(pos, playerPos);
			//if (dis < settings.DisSurround || (dis < settings.RealDisVisible && Vector3.Dot (pos - playerPos, playerForward) >= -settings.backView))
			if (dis < settings.DisSurround || (dis < settings.RealDisVisible && Vector3.Angle (pos - playerPos, playerForward) <= settings.visibleAngle))
				canSee = true;
			if (!canSee)
				continue;


			if (chunkState [index] == ChunkState.idle) {
				Enqueue (index,ChunkState.queueView);
			}
			else if (chunkState [index] == ChunkState.queueView||chunkState [index] == ChunkState.queueCache) {
				chunkState [index] = ChunkState.queueView;
			}
			else if (chunkState [index] == ChunkState.done) {
				RendChunk (index);
			}
		}

#if UNITY_EDITOR
		//Debug.LogError ("Render grass in one frame:"+grassRenderingCount);
#endif
	}

	private void CacheKDTree()
	{
		kdResults.Clear ();
		kdQuery.Radius (kdTree, playerPos, settings.DisCache, kdResults);
		int listCount = kdResults.Count;
		for (int i = 0; i < listCount; i++) {
			int index = kdResults [i];
			if (chunkState [index] == ChunkState.idle) {
				Enqueue (index,ChunkState.queueCache);
			}
		}
	} 
#endregion

#region Sort Mission
	Queue<GrassDataChunk> sortQueue = new Queue<GrassDataChunk> ();
	GrassSortBase sortMethod = new GrassSortCounting();

	bool Sort_Need(GrassDataChunk chunk)
	{
		if (chunk.sortState == SortState.idle) {
			chunk.lastDir = playerPos - chunk.center;
			return true;
		}
		else if (chunk.sortState == SortState.inQueue) {
			return false;
		}
		else if (chunk.sortState == SortState.done) {
			Vector3 dir = playerPos - chunk.center;
			if (Vector3.Angle (dir, chunk.lastDir) > 30) {
				chunk.lastDir = dir;
				return true;
			}
		}
		return false;
	}

	void Sort_AddToQueue(GrassDataChunk chunk)
	{
		chunk.sortState = SortState.inQueue;
		sortQueue.Enqueue (chunk);
	}

	IEnumerator Sort_ProcessQueue()
	{
		while (true) {
			if (sortQueue.Count > 0) {
				var chunk = sortQueue.Dequeue ();
				var item = chunk.data;
				sortMethod.Sort (item);
				item.props.SetVectorArray ("lmuv", item.uv);
				chunk.sortState = SortState.done;
			}
			yield return null;
		}
	}
#endregion

#region Quality
	public static float[] pcgTier = { 0.6f, 0.8f, 1f };
#endregion
}