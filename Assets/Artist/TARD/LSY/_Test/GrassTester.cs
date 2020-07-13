using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTester : MonoBehaviour {
	Transform player = null;
	void Awake()
	{
		DontDestroyOnLoad (gameObject);
	}
	void OnGUI()
	{
		var player = GrassFP_Manager.Instance.player;

		GUILayout.BeginHorizontal ();
		GUILayout.Space (150);
		float size = 60;

		if (GUILayout.Button ("GrassFX On",GUILayout.Width(size),GUILayout.Height(size))) {
			GrassFP_Manager.Instance.gameObject.SetActive (true); 
		}
		if (GUILayout.Button ("GrassFX Off",GUILayout.Width(size),GUILayout.Height(size))) {
			GrassFP_Manager.Instance.gameObject.SetActive (false);
		}
		if (GUILayout.Button ("Cut",GUILayout.Width(size),GUILayout.Height(size))) {
			GrassFP_Manager.Instance.AddPrint (player.transform.position, 4);
		}
		if (GUILayout.Button ("Burn",GUILayout.Width(size),GUILayout.Height(size))) {
			GrassFP_Manager.Instance.AddBurn (player.transform.position, 4);
		}
		if (GUILayout.Button ("Is on grass",GUILayout.Width(size),GUILayout.Height(size))) {
			bool b = GrassFP_Manager.Instance.IsOnGrass (player.transform.position);
			Debug.Log (b);
		}
		if (GUILayout.Button ("Cut",GUILayout.Width(size),GUILayout.Height(size))) {
			GrassFP_Manager.Instance.IsCut (player.transform.position);
		}
		if (GUILayout.Button ("关闭",GUILayout.Width(size),GUILayout.Height(size))) {
			enabled = false;
		}
		GUILayout.EndHorizontal ();
	}
}
