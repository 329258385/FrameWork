using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSTester : MonoBehaviour {
	public bool expand = true;
	public ShadowManager shadow;
	public float FSOnDelay = 0.8f;
	public FSManager fs;
	public bool showTestGUI = false;

	public void Start()
	{
		//Invoke ("FSOn", FSOnDelay);
	}

	void OnEnable()
	{
		TARDSceneInit.AddMission (FSOn);
	}
	void OnDisable()
	{
		TARDSceneInit.RemoveMission (FSOn);
	}


	void FSOn()
	{
		if (!showTestGUI) {
			fs.TurnOn ();
		}
	}

	public void OnGUI()
	{
		if (!showTestGUI)
			return;
		float size = 60;
		GUILayout.BeginHorizontal ();
		if (expand) {
			GUILayout.Space (Screen.width - size*6-110);
			string str = string.Format("{0}\n{1}\n{2}",QualitySettings.shadows.ToString (),Screen.width,Screen.height);
			GUILayout.Box (str,GUILayout.Width(size),GUILayout.Height(size));
//			if (GUILayout.Button ("播放音效",GUILayout.Width(size),GUILayout.Height(size))) {
//				LsyCommon.PlayAudio ("amb_rockfalling", FSManager.Instance.player.gameObject);
//			}
			if (GUILayout.Button ("重开",GUILayout.Width(size),GUILayout.Height(size))) {
				FSMessager.SendGM ("ec 91");
			}
			if (GUILayout.Button ("系统开",GUILayout.Width(size),GUILayout.Height(size))) {
				fs.TurnOn ();
			}
			if (GUILayout.Button ("系统关",GUILayout.Width(size),GUILayout.Height(size))) {
				fs.TurnOff ();
			}	
			if (GUILayout.Button ("阴影开",GUILayout.Width(size),GUILayout.Height(size))) {
				shadow.gameObject.SetActive (true);
			}
			if (GUILayout.Button ("阴影关",GUILayout.Width(size),GUILayout.Height(size))) {
				shadow.gameObject.SetActive (false);
			}	
			if (GUILayout.Button ("收起",GUILayout.Width(size),GUILayout.Height(size))) {
				expand = false;
			}	
		} else {
			GUILayout.Space (Screen.width - size-105);
			if (GUILayout.Button ("展开",GUILayout.Width(size),GUILayout.Height(size))) {
				expand = true;
			}	
		}
	}
}
