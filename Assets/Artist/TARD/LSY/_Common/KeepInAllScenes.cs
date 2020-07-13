using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepInAllScenes : MonoBehaviour {
	void Awake()
	{
		DontDestroyOnLoad (gameObject);
	}
}
