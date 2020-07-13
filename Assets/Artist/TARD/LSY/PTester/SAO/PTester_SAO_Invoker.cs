using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTester_SAO_Invoker : MonoBehaviour {
	void Awake()
	{
		DontDestroyOnLoad (gameObject);
	}
	void OnGUI()
	{
		if (GUI.Button (new Rect (0, 0, 50, 50), "",GUIStyle.none)) {
			PTester_SAO.Show (true);
		}
		if (GUI.Button (new Rect (0, 50, 50, 50), "",GUIStyle.none)) {
			PTester_SAO.Show (false);
		}
	}
}
