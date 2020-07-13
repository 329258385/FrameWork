using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Hold command buffer for cam, and execute worker's rending task
/// </summary>
[ExecuteInEditMode]
public class CB_CamManager : MonoBehaviour {
	public Camera cam;
	public List<CB_WorkerBase> workers = new List<CB_WorkerBase>();
	private bool init = false;
	public CB_CamAttach camAttach; 
	protected Dictionary<CameraEvent,CommandBuffer> bufDic = new Dictionary<CameraEvent, CommandBuffer>();
	#region life cycle
	void Update()
	{
		//transform.position = cam.transform.position + cam.transform.forward;
	}
	public void OnCB_Enable()
	{
		if (cam == null)
			cam = Camera.main;
		cam.depthTextureMode = DepthTextureMode.DepthNormals;
	
		if (camAttach == null) {
			camAttach = cam.GetComponent<CB_CamAttach> ();
			if (camAttach == null) {
				camAttach = cam.gameObject.AddComponent<CB_CamAttach> ();
			}
		}
		camAttach.AddAction (RenderSystem);

		init = true;
	}

	public void OnCB_Disable()
	{
		if (cam == null)
			return;
		foreach(var item in bufDic)
			cam.RemoveCommandBuffer (item.Key,item.Value);
		bufDic.Clear ();
		camAttach.RemoveAction (RenderSystem);
	}

	void SimpleStableSort()
	{
		for (int i = workers.Count; i > 0; i--) {
			for (int j = 1; j < i; j++) {
				if (workers [j - 1].depth > workers [j].depth) {
					var temp = workers [j - 1];
					workers [j - 1] = workers [j]; 
					workers [j] = temp;
				}
			}
		}
	}

	//private void OnWillRenderObject()
	private void RenderSystem()
	{
		if (!init)
			return;
		
		for (int i = workers.Count-1; i >= 0; i--) {
			if (workers [i] == null)
				workers.RemoveAt (i);
		}

		SimpleStableSort ();

		foreach (var item in bufDic) {
			item.Value.Clear ();
		}
		foreach (var worker in workers) {
			if (worker.gameObject.activeInHierarchy) {
				var buf = bufDic [worker.stage];
				buf.SetRenderTarget (BuiltinRenderTextureType.CurrentActive);
				worker.Render (buf);
			}
		}
	}

	public Matrix4x4 GetMatrixVP()
	{
		Matrix4x4 proj = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, 0.01f, cam.farClipPlane);

		#if UNITY_2017_2_OR_NEWER
		if (UnityEngine.XR.XRSettings.enabled)
		{
			// when using VR override the used projection matrix
			proj = Camera.current.projectionMatrix;
		}
		#endif

		proj = GL.GetGPUProjectionMatrix(proj, true);
		var _viewProj = proj * cam.worldToCameraMatrix;
		return _viewProj;
	}
	public Matrix4x4 GetMatrixV()
	{
		return cam.worldToCameraMatrix;
	}
	#endregion


	#region worker
	public void AddWorker(CB_WorkerBase worker)
	{
		if (worker == null)
			return;
		worker.SetCamManager (this);
		if(!workers.Contains(worker))
			workers.Add (worker);

		AddWorkerBuff (worker);
	}
	void AddWorkerBuff(CB_WorkerBase worker)
	{
		var stage = worker.stage;
		if (!bufDic.ContainsKey (stage)) {
			bufDic.Add (stage, new CommandBuffer ());
			cam.AddCommandBuffer(stage,bufDic[stage]);
		}
	}
	public void RemoveWorker(CB_WorkerBase worker)
	{
		if (worker == null)
			return;
		if(workers.Contains(worker))
			workers.Remove (worker);
	}
	#endregion
}



/// <summary>
/// Todo: display in sceneview
/// </summary>
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Rendering;
//
///// <summary>
///// Hold command buffer for cam, and execute worker's rending task
///// </summary>
//[ExecuteInEditMode]
//public class CB_CamManager : MonoBehaviour {
////	public Camera cam;
////	private CommandBuffer buf;
//	public List<CB_WorkerBase> workers = new List<CB_WorkerBase>();
//	private bool init = false;
//
//	private Dictionary<Camera,CommandBuffer> dicCB = new Dictionary<Camera, CommandBuffer>();
//	#region life cycle
//	public void OnCB_Enable()
//	{
//	//	buf = new CommandBuffer ();
//
//		init = true;
//	}
//
//	public void OnCB_Disable()
//	{
////		if (cam == null || buf == null)
////			return;
////		cam.RemoveCommandBuffer (CameraEvent.AfterForwardAlpha,buf);
//		foreach (var item in dicCB) {
//			item.Key.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha,item.Value);
//		}
//	}
//
//	private void OnWillRenderObject()
//	{
//		if (!init)
//			return;
//		var cam = Camera.current;
//		if (!dicCB.ContainsKey (cam)) {
//			cam.depthTextureMode = DepthTextureMode.DepthNormals;
//			CommandBuffer buff = new CommandBuffer ();
//			cam.AddCommandBuffer (CameraEvent.AfterForwardAlpha, buff);
//			dicCB.Add (cam, buff);
//		}
//
//		var buf = dicCB[cam];
//		buf.Clear ();
//		buf.SetRenderTarget (BuiltinRenderTextureType.CurrentActive);
//		foreach (var worker in workers) {
//			if(worker.gameObject.activeSelf)
//				worker.Render (buf);
//		}
//	}
//	#endregion
//
//
//	#region worker
//	public void AddWorker(CB_WorkerBase worker)
//	{
//		if (worker == null)
//			return;
//		if(!workers.Contains(worker))
//			workers.Add (worker);
//	}
//	public void RemoveWorker(CB_WorkerBase worker)
//	{
//		if (worker == null)
//			return;
//		if(workers.Contains(worker))
//			workers.Remove (worker);
//	}
//	#endregion
//}