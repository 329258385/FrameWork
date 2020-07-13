using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileActivator : MonoBehaviour {
	public List<GameObject> pcObjs;
	public List<GameObject> mobileObjs;

	void Awake()
	{
		bool m = Application.isMobilePlatform;
		foreach (var item in pcObjs) {
			if (item == null)
				continue;
			item.SetActive (!m);
		}
		foreach (var item in mobileObjs) {
			if (item == null)
				continue;
			item.SetActive (m);
		}
	}
}
