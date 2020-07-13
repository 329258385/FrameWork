using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FSMsgItem
{
	public string key;
	public GameObject obj;
}

public class FSMessagerManager:MonoBehaviour{
	public static FSMessagerManager Instance;
	public List<FSMsgItem> items;
	Dictionary<string,FSMsgItem> dic = new Dictionary<string, FSMsgItem>();

	public void Awake()
	{
		Instance = this;
		foreach (var item in items) {
			dic.Add (item.key, item);
		}
	}
	public void ReceiveMsg(string msg)
	{
		if (dic.ContainsKey (msg) && dic [msg].obj != null) {
			dic [msg].obj.SetActive (true);
		}
	}
}
