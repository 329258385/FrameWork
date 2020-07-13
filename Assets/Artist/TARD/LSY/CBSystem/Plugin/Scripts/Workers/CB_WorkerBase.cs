using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum MatParamsSyncType
{
	all,
	individual
}

/// <summary>
/// A worker who represents
/// A light / a shadow...
/// Work for only one camera
/// </summary>
[ExecuteInEditMode]
public class CB_WorkerBase : MonoBehaviour {
	public CameraEvent stage = CameraEvent.AfterForwardOpaque;
	public MatParamsSyncType matParamsSyncType = MatParamsSyncType.individual;
	public List<CB_MaterialParams> matParams;
	public CB_MaterialParams[] matParamsOther = new CB_MaterialParams[3];
	public int cameraIndex = 0;
	public int depth;
	public Transform root;
	public Mesh mesh;

	public List<Material> matOrigins;
	public List<Material> mats = new List<Material>();  
	public CB_CamManager camManager;

	protected bool needAdd = false;
	public void SetCamManager(CB_CamManager _camManager)
	{
		camManager = _camManager;
	}
	public virtual void OnEnable()
	{
		if (mesh == null) {  
			mesh = GenerateMesh ();
		}  
		if (root == null)  
			root = transform;

		InitMat ();
		MatSyncAll ();

		TryAdd ();
	}

	void TryAdd()
	{
		if (CB_Manager.Instance != null) {
			CB_Manager.Instance.AddWorker (this);
			needAdd = false;
		} else {
			needAdd = true;
		}
	}

	void InitMat()
	{
		for (int i = 0; i < matOrigins.Count; i++) {
			if (i >= mats.Count) {
				#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(gameObject);
				#endif
				Material mat = new Material (matOrigins [i]);
				mats.Add (mat);
			} else if(CB_Lib.MatNeedCreate(mats [i],matOrigins [i])){
				#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(gameObject);
				#endif
				Material mat = new Material (matOrigins [i]);
				mats [i] = mat;
			}
		}
	}
	public virtual void OnDisable()
	{
		if(CB_Manager.Instance!=null)
			CB_Manager.Instance.RemoveWorker (this);  
	}
		
	protected virtual Mesh GenerateMesh()
	{
		return null;
	}


	public virtual void Render( CommandBuffer buf)
	{
		foreach(var mat in mats)
			buf.DrawMesh(mesh,root.localToWorldMatrix,mat);
	}

	protected virtual void Update()
	{
		MatSyncAllEditor ();

		if (needAdd)
			TryAdd ();
	}

	protected void MatSyncAllEditor()
	{
		#if UNITY_EDITOR
		MatSyncAll();
		#endif
	}

	public virtual void MatSyncAll()
	{
		while(matParams.Count<mats.Count)
		{
			matParams.Add(new CB_MaterialParams());
		}

		for(int i=0;i<mats.Count;i++)
		{
			if(matParamsSyncType == MatParamsSyncType.all)
				matParams[0].Sync(mats[i]);
			else if(matParamsSyncType == MatParamsSyncType.individual)
				matParams[i].Sync(mats[i]);
		}
	}
	protected virtual void OnDrawGizmosSelected()
	{
		if (root == null)
			root = transform;

		DrawMesh (root,mats [0]);
	}

	protected void DrawMesh(Transform root, Material mat)
	{
		if (mesh == null || !root.gameObject.activeInHierarchy)
			return;
		Color col = Color.white;
		if (mat.HasProperty ("_Color")) {
			col = mat.GetColor("_Color");
		}
		Gizmos.color = col;
		//Gizmos.DrawMesh (mesh, root.position, root.rotation, root.lossyScale);
		Gizmos.DrawWireMesh (mesh, root.position, root.rotation, root.lossyScale);
		Gizmos.color = Color.white;
	}
}
