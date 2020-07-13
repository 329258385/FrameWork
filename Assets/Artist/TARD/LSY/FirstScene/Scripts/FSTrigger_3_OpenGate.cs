using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FSTrigger_3_OpenGate : MonoBehaviour {
	public Animator ani;
	public float delay=1;
	public float duration=3;
	public Ease easeMethod= Ease.InOutCubic;
	public Vector3 move = new Vector3(-1.8f,0,0);
	public List<Transform> doorL;
	public List<Transform> doorR;

	Vector3 startPosL;
	Vector3 startPosR;
	void Awake()
	{
		ani.Play ("fx_door_anim");
		Invoke("Open",delay);
	}
	public void Open()
	{
		foreach (var item in doorL) {
			item.DOLocalMove (item.transform.localPosition - move, duration).SetEase (easeMethod);
		}
		foreach (var item in doorR) {
			item.DOLocalMove (item.transform.localPosition + move, duration).SetEase (easeMethod);
		}
	}

	public void Close()
	{
		foreach (var item in doorL) {
			item.DOLocalMove (item.transform.localPosition + move, duration).SetEase (easeMethod);
		}
		foreach (var item in doorR) {
			item.DOLocalMove (item.transform.localPosition - move, duration).SetEase (easeMethod);
		}
	}
}
