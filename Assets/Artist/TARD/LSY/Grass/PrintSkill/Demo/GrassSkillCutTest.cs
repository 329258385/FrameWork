using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSkillCutTest : MonoBehaviour {
	public Transform player;
	public GrassSkillCut grassSkillCut;
	public GrassSkillData data;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.K)) {
			grassSkillCut.PlaySkill (player, data);
		}
	}
//	void OnGUI()
//	{
//		if (GUILayout.Button ("Play")) {
//			grassSkillCut.PlaySkill (player, data);
//		}
//	}
}
