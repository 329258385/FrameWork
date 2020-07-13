using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class GrassSkillData
{
	public int id;
	public int delay;
	public int duration;
	public int shape;
	public string size;
	public string angle;
	public string spreadDir;
	public string offset;
}
[System.Serializable]
public class GrassSkillInfo
{
	public string prefabID;
	public float delay;
	public float duration;
	public Vector3 scale;
	public float angle1 = -180;
	public float angle2 = 180;
	public Vector2 offset;
}


public class GrassSkillCut:MonoBehaviour{
	public static GrassSkillCut Instance;

	public List<GameObject> prefabs;
	Dictionary<string,GameObject> prefabDic = new Dictionary<string, GameObject> ();
	void Awake()
	{
		Instance = this;
		foreach (var item in prefabs)
			prefabDic.Add (item.name, item);
	}
	private GameObject GetPrefab(string key)
	{
		if (prefabDic.ContainsKey (key))
			return prefabDic [key];
		return prefabs [0];
	}

	public void PlaySkill(Vector3 pos,Vector3 dir,GrassSkillData data)
	{
		if (data == null) {
			Debug.LogError ("GrassSkillData数据为空，请联系陶恩恩");
		}
		GrassSkillInfo info = DataToInfo (data);
		//GameObject go = Instantiate(GetPrefab(info.prefabID)) as GameObject;
		GameObject go = EasyPoolManager.Instantiate(GetPrefab(info.prefabID)) as GameObject;


		GrassSkillShape shape = go.GetComponent<GrassSkillShape> ();
		shape.Play (pos,dir, info);
	}
	public void PlaySkill(Transform obj,GrassSkillData data)
	{
		PlaySkill (obj.position, obj.forward, data);
	}

	GrassSkillInfo DataToInfo(GrassSkillData data)
	{
		GrassSkillInfo info = new GrassSkillInfo ();
		info.prefabID = data.spreadDir;
		info.delay = (float)data.delay/1000f;
		info.duration =  (float)data.duration/1000f;
		info.scale = Vector3.one;
		if (!string.IsNullOrEmpty (data.size)) {
			if (data.spreadDir.StartsWith ("3")) {
				var strs = data.size.Split (',');
				info.scale = new Vector3 (float.Parse (strs [0])*1.5f, 1, float.Parse (strs [1])*1.5f);
			} else {
				float rad = float.Parse (data.size);
				info.scale = new Vector3 (rad * 2*1.5f, 1, rad * 2*1.5f);
			}
		}
		if (!string.IsNullOrEmpty (data.angle)) {
			var strs = data.angle.Split (',');
			info.angle1 = float.Parse (strs [0]);
			info.angle2 = float.Parse (strs [1]);
		}
		if (!string.IsNullOrEmpty (data.offset)) {
			var strs = data.offset.Split (',');
			info.offset = new Vector2 (float.Parse(strs [0]),float.Parse(strs [1]));
		}
	
		return info;
	}
}
