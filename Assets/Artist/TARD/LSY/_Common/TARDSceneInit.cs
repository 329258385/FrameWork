using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TARDSceneInit{
	private static LuaFunction callback;

	#region Mission
	public static bool isLoading = false;
	private static Action act;
	public static void AddMission(Action _act)
	{
		act += _act;
	}
	public static void RemoveMission(Action _act)
	{
		act -= _act;
	}
	public static void AddMissionAync(Func<IEnumerator> _act)
	{
		TARDSceneInitAsync.Instance.AddMission (_act);
	}
	public static void RemoveMissionAync(Func<IEnumerator> _act)
	{
		TARDSceneInitAsync.Instance.RemoveMission (_act);
	}
	#endregion


	#region Scene Init
	public static void SceneInit(LuaFunction _callback)
	{
		isLoading = true;
		callback = _callback;

		if (act != null)
			act ();
		if (TARDSceneInitAsync.Instance.HasMission ()) {
			TARDSceneInitAsync.Instance.LoadScene (Finish);
		} else {
			Finish ();
		}
	}

	private static void Finish()
	{
		isLoading = false;
		if (callback != null)
		{
			callback.Call();
		}
	}
	#endregion
}