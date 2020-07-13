using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1 Manager all CamManagers' life cycle
/// 2 Accept worker and give it to CamManager
/// </summary>
[ExecuteInEditMode]
public class CB_Manager : MonoBehaviour {
	public static CB_Manager Instance;
	public List<CB_CamManager> cams;
	public float activeDelay = 1f;
	#region life cycle
	void Awake()
	{
		Instance = this;
	}
	void Update()
	{
		Instance = this;
	}
	private void OnEnable() 
	{
		//Deactive ();
		Active ();

		//Invoke ("Deactive", activeDelay);
		//Invoke ("Active",   activeDelay+0.2f);
	}
	private void OnDisable()
	{
		Deactive ();
	}

	void Active()
	{
		for (int i = 0; i < cams.Count; i++) {
			var cm = cams [i];  
			cm.OnCB_Enable ();
		}
	}
	void Deactive()
	{
		for (int i = 0; i < cams.Count; i++) {
			var cm = cams [i];
			cm.OnCB_Disable ();
		}
	}
	#endregion

	#region worker
	public void AddWorker(CB_WorkerBase worker)
	{
		if (worker == null)
			return;
		if(worker.cameraIndex< cams.Count)
			cams [worker.cameraIndex].AddWorker (worker);
	}
	public void RemoveWorker(CB_WorkerBase worker)
	{
		if (worker == null)
			return;
		if(worker.cameraIndex< cams.Count)
			cams [worker.cameraIndex].RemoveWorker (worker);
	}
	#endregion
}
