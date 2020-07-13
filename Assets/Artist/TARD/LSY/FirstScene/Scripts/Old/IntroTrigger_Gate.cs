using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class IntroTrigger_Gate : MonoBehaviour {
	public float duration=3;
	public Ease easeMethod= Ease.InOutCubic;
	public float downh=-0.2f;
	public Vector3 move = new Vector3(-1.8f,0,0);
	public Transform doorL;
	public Transform doorR;

	Vector3 startPosL;
	Vector3 startPosR;
	void OnTriggerEnter(Collider col)
	{
		transform.position += new Vector3 (0, downh, 0);

		doorL.DOLocalMove (doorL.transform.localPosition - move, duration).SetEase (easeMethod);
		doorR.DOLocalMove (doorR.transform.localPosition + move, duration).SetEase (easeMethod);
	}

	void OnTriggerExit(Collider col)
	{
		transform.position += new Vector3 (0, -downh, 0);

		doorL.DOLocalMove (doorL.transform.localPosition + move, duration).SetEase (easeMethod);
		doorR.DOLocalMove (doorR.transform.localPosition - move, duration).SetEase (easeMethod);
	}
}
