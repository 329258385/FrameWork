using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour {
	public Color playerColor;
	public Transform player;

	//private IntroTrigger_1
	void Update () {
		foreach (var item in player.GetComponentsInChildren<SkinnedMeshRenderer>()) {
			item.material.SetColor ("_Color", playerColor);
		}
	}
}
