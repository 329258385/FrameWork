using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FSAudioPlayClip
{
	public string clip;
	public float delay = 0;
	public GameObject obj;
}

public class FSAudioPlay : MonoBehaviour {
	public List<FSAudioPlayClip> clips;

	void Start () {
		foreach (var item in clips) {
			StartCoroutine (PlayClip (item));
		}
	}
	


	IEnumerator PlayClip(FSAudioPlayClip clip)
	{
		yield return new WaitForSeconds (clip.delay);
		GameObject obj = null;
		if (clip.obj != null)
			obj = clip.obj;
		else
			obj = FSManager.Instance.player.gameObject;
		LsyCommon.PlayAudio (clip.clip, obj);
	}
}
