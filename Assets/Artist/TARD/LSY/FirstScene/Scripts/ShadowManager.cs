using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowManager : MonoBehaviour {
	public Transform player;
	public Transform solidObj;
	public List<Transform> lights;
	public bool bruteMode = true;
	public List<CB_ShadowSub> shadows = new List<CB_ShadowSub>();

	void Update () {
		if (FSManager.Instance != null) {
			player = FSManager.Instance.player;
		} else {
			if (player == null || !player.gameObject.activeInHierarchy) {
				player = LsyCommon.FindPlayer ();
			}
		}
        if(player != null && solidObj != null)
		    solidObj.transform.position = player.transform.position+new Vector3(0,2,0);

		UpdateLight ();
		UpdateShadow ();
	}

	#region UpdateLight
	void UpdateLight()
	{
		if (bruteMode) {
			lights.Clear ();
			var ls = GameObject.FindObjectsOfType<Light> ();
			foreach (var l in ls) {
				if (l.type == LightType.Point && l.shadows == LightShadows.None) {
					lights.Add (l.transform);
				}
			}
		}
		SimpleStableSort();
	}
	void SimpleStableSort()
	{
		for (int i = lights.Count; i > 0; i--) {
			for (int j = 1; j < i; j++) {
				float dis1 = GetDis (player, lights [j - 1]);
				float dis2 = GetDis (player, lights [j]);
				if (dis1 > dis2) {
					var temp = lights [j - 1];
					lights [j - 1] = lights [j]; 
					lights [j] = temp;
				}
			}
		}
	}
	float GetDis(Transform x, Transform y)
	{
		return Vector3.Distance (x.position, y.position);
	}
	#endregion

	#region Update Shadow
	void UpdateShadow()
	{
		shadows.Clear ();
		shadows.AddRange(GetComponentsInChildren<CB_ShadowSub> ());



		for (int i = 0; i < shadows.Count; i++) {
			if (i >= lights.Count) {
				//shadows [i].gameObject.SetActive (false);
			} else {
				shadows [i].transform.position = lights [i].position;
			}
		}
	}
	#endregion
}
