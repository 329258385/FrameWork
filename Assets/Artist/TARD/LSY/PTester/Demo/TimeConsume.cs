using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeConsume : MonoBehaviour {
	public static int loop = 0;

	void Update () {
		for (int i = 0; i < loop; i++) {
			Debug.Log (Time.realtimeSinceStartup);
		}
	}	
}
