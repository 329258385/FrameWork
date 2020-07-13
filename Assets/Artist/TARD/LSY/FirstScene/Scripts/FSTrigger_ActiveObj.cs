using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSTrigger_ActiveObj : MonoBehaviour {
	public FSTrigger_3_OpenGate gate;
	void OnTriggerEnter(Collider col)
	{
		gate.gameObject.SetActive (true);
		//gate.Open ();
	}

	void OnTriggerExit(Collider col)
	{
		//gate.Close ();
	}
}
