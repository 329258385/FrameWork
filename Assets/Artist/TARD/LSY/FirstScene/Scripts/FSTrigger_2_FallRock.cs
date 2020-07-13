using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSTrigger_2_FallRock : MonoBehaviour {
	public Animator ani;
	public float bianfuDelay;
	public GameObject bianfu;
	public float bianfuFXDelay;
	public GameObject bianfuFX;
	public void Awake()
	{
		ani.Play ("rock_fall_anim");

		StartCoroutine (Bianfu ());
	}

	IEnumerator Bianfu()
	{
		yield return new WaitForSeconds (bianfuDelay);
		bianfu.SetActive (true);
		yield return new WaitForSeconds (bianfuFXDelay);
		bianfuFX.SetActive (true);
	}
}
