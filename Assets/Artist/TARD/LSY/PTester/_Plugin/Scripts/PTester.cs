using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TesterInfo
{
	protected string key;
	protected List<float> msList = new List<float> ();

	public TesterInfo(string _key)
	{
		key = _key;
	}

	public bool HasRecord()
	{
		return msList.Count > 0;
	}
	public void AddMSRecord(float ms)
	{
		msList.Add (ms);
	}

	public float GetAvgMS()
	{
		float f = 0;
		for (int i = 0; i < msList.Count; i++) {
			f += msList [i];
		}
		f /= msList.Count;
		return f;
	}
}

public class PTester : MonoBehaviour {
	#region UI
	protected const float UIWidth = 80;
	protected const float UIHeight = 30;
	protected const float UIColumnWidth = 320;
	protected const int UITitleFontSize = 20;
	#endregion
	int FPS{get{return PTester_FPS.FPS;}}
	float MS{get{return PTester_FPS.MS;}}

	bool isTesting = false;
	bool show = true;
	Dictionary<string,TesterInfo> infos = new Dictionary<string, TesterInfo>();

	protected string testGroup1_title;
	protected string testGroup2_title;
	protected string testGroup3_title;
	protected string testGroup4_title;
	protected string testGroup5_title;

	Vector2 scrollPos;
	private void Start()
	{
		Init ();
	}
	protected virtual void Init()
	{
		gameObject.AddComponent<PTester_FPS> ();
	}

	protected virtual void MainTitle()
	{

	}

	private void OnGUI()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (300);
		GUILayout.Label (string.Format("{0} FPS", PTester_FPS.FPS),GUILayout.Width(UIWidth),GUILayout.Height(UIHeight));
		GUILayout.Label (string.Format("{0} MS", PTester_FPS.MS.ToString("f1")),GUILayout.Width(UIWidth),GUILayout.Height(UIHeight));
		GUILayout.Space (600);
		if (show) {
			if (GUILayout.Button ("隐藏", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
				show = false;
			}
		} else {
			if (GUILayout.Button ("显示", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
				show = true;
			}
		}
		MainTitle ();
		GUILayout.EndHorizontal ();
		if (!show)
			return;

		scrollPos = GUILayout.BeginScrollView (scrollPos);

		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical (GUILayout.Width(UIColumnWidth));
		OnGUI_Mission1 ();
		GUILayout.EndVertical ();

		GUILayout.BeginVertical (GUILayout.Width(UIColumnWidth));
		OnGUI_Mission2 ();
		GUILayout.EndVertical ();

		GUILayout.BeginVertical (GUILayout.Width(UIColumnWidth));
		OnGUI_Mission3 ();
		GUILayout.EndVertical ();

		GUILayout.BeginVertical (GUILayout.Width(UIColumnWidth));
		OnGUI_Mission4 ();
		GUILayout.EndVertical ();

		GUILayout.BeginVertical (GUILayout.Width(UIColumnWidth));
		OnGUI_Mission5 ();
		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();

		GUILayout.EndScrollView ();
	}

	protected virtual void  OnGUI_Mission1()
	{
		int s = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = UITitleFontSize;
		GUILayout.Label (testGroup1_title,GUILayout.Height (UIHeight*2));
		GUI.skin.label.fontSize = s;
	}
	protected virtual void  OnGUI_Mission2()
	{
		int s = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = UITitleFontSize;
		GUILayout.Label (testGroup2_title,GUILayout.Height (UIHeight*1.2f));
		GUI.skin.label.fontSize = s;
	}
	protected virtual void  OnGUI_Mission3()
	{
		int s = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = UITitleFontSize;
		GUILayout.Label (testGroup3_title,GUILayout.Height (UIHeight*1.2f));
		GUI.skin.label.fontSize = s;
	}
	protected virtual void  OnGUI_Mission4()
	{
		int s = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = UITitleFontSize;
		GUILayout.Label (testGroup4_title,GUILayout.Height (UIHeight*1.2f));
		GUI.skin.label.fontSize = s;
	}
	protected virtual void  OnGUI_Mission5()
	{
		int s = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = UITitleFontSize;
		GUILayout.Label (testGroup5_title,GUILayout.Height (UIHeight*1.2f));
		GUI.skin.label.fontSize = s;
	}



	protected void TesterGroup (string titleMain,string title1,Action act1,
		string title2="",Action act2=null,string title3="",Action act3=null,string title4="",Action act4=null)
	{
		if(!string.IsNullOrEmpty(titleMain))
			GUILayout.Label (titleMain);
		GUILayout.BeginHorizontal ();

		Tester (titleMain,title1,act1,true,false);
		Tester (titleMain,title2,act2,true);
		Tester (titleMain,title3,act3,true);
		Tester (titleMain,title4,act4,true);
		GUILayout.EndHorizontal ();
		Space ();
	}

	protected void Tester(string titleMain,string title,Action act,bool isChild = false,bool showChange = true)
	{
		if (string.IsNullOrEmpty (title))
			return;
		string key = titleMain + title;
		GUILayout.BeginVertical (GUILayout.Width(UIWidth));
		if(!isChild)
			GUILayout.Label (titleMain,GUILayout.Height (UIHeight*0.5f));


		if (!infos.ContainsKey (key)) {
			infos.Add (key, new TesterInfo (key));
		}
		var info = infos [key];

		if (!isTesting) {
			if (GUILayout.Button (title, GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
				StartCoroutine (ActRunner (key, act));
			}
		} else {
			GUILayout.Space (UIHeight);
		}


		if (showChange && info.HasRecord ()) {
			float ms = infos [key].GetAvgMS ();
			if (ms > 0f) {
				GUI.color = Color.red;
				GUILayout.Label (string.Format ("+{0} MS", ms.ToString ("f1"), GUILayout.Width (UIWidth), GUILayout.Height (UIHeight*0.5f)));
			} else {
				GUI.color = Color.green;
				GUILayout.Label (string.Format ("-{0} MS", ms.ToString ("f1"), GUILayout.Width (UIWidth), GUILayout.Height (UIHeight*0.5f)));
			}
			GUI.color = Color.white;
		} else {
			GUILayout.Space (UIHeight*0.5f);
		}

		GUILayout.EndVertical ();

		if (!isChild)
			Space ();
	}
	private IEnumerator ActRunner(string key,Action act)
	{
		isTesting = true;
		float msBefore = MS;
		float msAfter;
		if (act != null) {
			act ();
		}
	

		yield return new WaitForSeconds (3);
		msAfter = MS;

		float msDif = msAfter - msBefore;
		infos[key].AddMSRecord(msDif);

		isTesting = false;
	}

	protected void Space()
	{
		GUILayout.Space (2);
	}
}
