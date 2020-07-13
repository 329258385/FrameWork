using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TARDSceneInitAsync : MonoBehaviour {
	public static TARDSceneInitAsync Instance
	{
		get{ 
			if (instance == null) {
				GameObject go = new GameObject ("TARDSceneInitAsync");
				instance = go.AddComponent<TARDSceneInitAsync> ();
				go.AddComponent<KeepInAllScenes> ();
			}
			return instance; 
		}
	}
	private static TARDSceneInitAsync instance;
	private float maxTimeAllow = 10f;
	#region Mission
	public List<Func<IEnumerator>> funcs = new List<Func<IEnumerator>> ();
	public bool HasMission()
	{
		return funcs.Count > 0;
	}
	public void AddMission(Func<IEnumerator> func)
	{
		funcs.Add (func);
	}
	public void RemoveMission(Func<IEnumerator> func)
	{
		funcs.Remove (func);
	}
	#endregion

	#region Scene Init
	public void LoadScene(Action actEnd)
	{
		StartCoroutine (ILoadScene (actEnd));
		StartCoroutine (ILoadSceneTimer (actEnd));
	}
	private IEnumerator ILoadScene(Action actEnd)
	{
		float timeStart = Time.realtimeSinceStartup;
		yield return null;
		for (int i = 0; i < funcs.Count; i++) {
			yield return funcs[i]();
		}


		StopCoroutine ("ILoadSceneTimer");
		if(actEnd!=null)
			actEnd ();
		funcs.Clear ();
	}

	private IEnumerator ILoadSceneTimer(Action actEnd)
	{
		float timeStart = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - timeStart < maxTimeAllow) {
			yield return null;
		}

		#if UNITY_EDITOR
		//Debug.LogError ("TARD Scene Load Error");
		Debug.LogError ("请检查场景角色初始位置是否配置有误,没有错误的话可以忽略");
		#endif
		StopCoroutine ("ILoadScene");
		if(actEnd!=null)
			actEnd ();
		funcs.Clear ();
	}
	#endregion
}
