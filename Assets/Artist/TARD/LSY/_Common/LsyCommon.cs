using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LsyCommon{
	private static Transform player;
	public static Transform FindPlayer()
	{
		#if BUILD_SINGLESCNE_MODE
		FindPlayerTest();
		#elif UNITY_EDITOR
		FindPlayerTest();
		#endif

        if (player == null) {
			GameObject go = GameObject.Find ("PlayerRootNode");
			if (go != null && go.transform.childCount>0) {
				player = go.transform.GetChild (0);
			}
		}
		return player;
	}

	static void FindPlayerTest()
	{
		if (player == null) {
			var atc = Object.FindObjectOfType<ArtTestCharControl> ();
			if(atc!=null)
				player = atc.GetComponentInChildren<CharacterController> ().transform;
			var sm = Object.FindObjectOfType<SimpleMover> ();
			if (sm != null)
				player = sm.transform;
		}
	}
	public static List<T> FindAllChars<T>(bool includeInactive = false)
	{
		List<string> name = new List<string> ();
		name.Add ("MonsterRootNode");
		name.Add ("NPCRootNode");
		name.Add ("PlayerRootNode");

		var gos = FindAllGameObjects (name);

		return FindAll<T> (gos,includeInactive);
	}

	public static List<GameObject> FindAllGameObjects(List<string> names)
	{
		List<GameObject> gos = new List<GameObject> ();
		foreach (var nm in names) {
			var go = GameObject.Find (nm);
			if (go != null)
				gos.Add (go);
		}
		return gos;
	}

	public static List<T> FindAll<T>(List<GameObject> roots,bool includeInactive)
	{
		List<T> t = new List<T> ();
		foreach (var item in roots) {
			t.AddRange (item.GetComponentsInChildren<T> (includeInactive));
		}
		return t;
	}

	public static void PlayAudio(string str,GameObject obj)
	{
		SoundBanksManager.Instance.PlaySound (str, obj);
	}


	public static T TryAddComponent<T>(GameObject go)
		where T:MonoBehaviour
	{
		T t = go.GetComponent<T> (); 
		if(t==null)
			t = go.AddComponent<T> (); 
		return t;
	}
}
