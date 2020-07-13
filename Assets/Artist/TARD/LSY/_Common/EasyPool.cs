using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyPool
{
	public static int MAX = 3;
	protected GameObject prefab;
	protected int index = 0;
	protected List<GameObject> pool = new List<GameObject> ();

	public EasyPool(GameObject _prefab)
	{
		prefab = _prefab;
	}

	public GameObject Instantiate()
	{
		GameObject go = null;
		if (pool.Count < MAX) {
			go = GameObject.Instantiate (prefab);
			go.transform.parent = EasyPoolManager.root;
			pool.Add (go);
		} else {
			go = pool [index];
			index++;
			if (index == MAX)
				index = 0;
		}
		return go;
	}
}

public class EasyPoolManager {
	public static Transform root;
	protected static Dictionary<int,EasyPool> poolDic = new Dictionary<int, EasyPool>();

	public static GameObject Instantiate(GameObject prefab)
	{
		if (root == null) {
			GameObject rootGo = new GameObject ("LsyPool");
			root = rootGo.transform;
			root.localPosition = Vector3.zero;
			root.localScale = Vector3.one;
			root.localRotation = Quaternion.identity; 
			GameObject.DontDestroyOnLoad (rootGo);
		}
			
		int key = prefab.GetInstanceID (); 
		if (!poolDic.ContainsKey (key)) {
			poolDic.Add (key, new EasyPool (prefab)); 
		}

		return poolDic [key].Instantiate ();
	}
}
