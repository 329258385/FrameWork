using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class FSManager:MonoBehaviour{
	public static FSManager Instance;
	public Color playerColor;
	public Transform player;

	public CB_VolumeFog fogFullCover;
	public CB_VolumeFog fog;
	public static bool IsON;

	private bool HasSetResolution = false;

	public GameObject doorOld;
	public GameObject doorNew;

	Dictionary<Renderer,Color> rendererColors = new Dictionary<Renderer, Color>();

	private GameObject shadowRoot;
	public void Awake()
	{
		Instance = this;

		shadowRoot = GameObject.Find ("SimpleShadows");
	}


	void Update()
	{
		if (shadowRoot != null) {
			foreach (var item in shadowRoot.GetComponentsInChildren<MeshRenderer>()) {
				item.enabled = false;
			}
		}

		if (player == null || !player.gameObject.activeInHierarchy) {
			player = LsyCommon.FindPlayer ();
		}

		var allR = LsyCommon.FindAllChars<Renderer> ();
		foreach (var item in allR) {
			//objs with Decal should not have real time shadow
			if (item.GetComponentInChildren<CB_Decal> () != null) {
				if (item.transform.parent == null) {
					item.gameObject.layer = 0;
				} else {
					foreach (var r in item.transform.parent.GetComponentsInChildren<Renderer>(true)) {
						r.gameObject.layer = 0;
					}
				}
			}

			//Set chars Darker for cave
			if (!rendererColors.ContainsKey (item) && item.material.HasProperty("_Color")) {
				rendererColors.Add(item,item.material.color);
			}
		}

		foreach (var item in rendererColors)
		{
			if (item.Key == null)
				continue;
			var newColor = item.Value;
			newColor = new Color (newColor.r*playerColor.r, newColor.g*playerColor.g, newColor.b*playerColor.b, newColor.a*playerColor.a);
			item.Key.material.SetColor("_Color", newColor);
		}
	}

	void OnDisable()
	{
		foreach (var item in rendererColors)
		{
			if (item.Key == null)
				continue;
			var newColor = item.Value;
			item.Key.material.SetColor("_Color",newColor);
		}
	}

	public void TurnOn()
	{
		if (!HasSetResolution) {
			HasSetResolution = true;
			int w = (int)((float)Screen.width * 0.8f);
			int h = (int)((float)Screen.height * 0.8f);
			Screen.SetResolution (w, h, true, 60);
		}
		IsON = true;
		Shader.DisableKeyword("SHADOW_MAP");
		FSMessager.TurnOn ();
		gameObject.SetActive (true);
		RenderSettings.fog = false;
		doorOld.SetActive (false);
		doorNew.SetActive (true);
	}
	public void TurnOff()
	{
		IsON = false;
		Shader.EnableKeyword("SHADOW_MAP");
		FSMessager.TurnOff ();
		gameObject.SetActive (false);
		RenderSettings.fog = true;
		doorOld.SetActive (true);
		doorNew.SetActive (false);
	}

	//	public void SetFogColor(Color c)
	//	{
	//		fogFullCover.SetFogColor (c);
	//		fog.SetFogColor (c);
	//	}
	public void SetFogDis(float dis,float duration,float delay)
	{
		//		fogFullCover.SetFogDis (dis);
		//		fog.SetFogDis (dis);
		DOTween.To(()=>fogDis,x=>fogDis=x,dis,duration).SetEase(Ease.InOutCubic).SetDelay(delay);
	}
	public void SetFogAlpha(float alpha,float duration,float delay)
	{
		//		fogFullCover.SetFogDis (dis);
		//		fog.SetFogDis (dis);
		DOTween.To(()=>fogAlpha,x=>fogAlpha=x,alpha,duration).SetEase(Ease.InOutCubic).SetDelay(delay);
	}

	public void Switch()
	{
		fogFullCover.gameObject.SetActive (false);
		fog.gameObject.SetActive (true);
	}

	public float fogDis
	{
		get{ return fogFullCover.matParams [0].fogEnd;}
		set{ 
			fogFullCover.SetFogDis (value);
			fog.SetFogDis (value);
		}
	}
	public float fogAlpha
	{
		get{ return fogFullCover.matParams [0].color.a;}
		set{ 
			fogFullCover.SetFogAlpha (value);
			fog.SetFogAlpha (value);
		}
	}
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using DG.Tweening;
//
//public class FSManager:MonoBehaviour{
//	public static FSManager Instance;
//	public Color playerColor;
//	public Transform player;
//
//	public CB_VolumeFog fogFullCover;
//	public CB_VolumeFog fog;
//	public static bool IsON;
//
//	private bool HasSetResolution = false;
//
//	public GameObject doorOld;
//	public GameObject doorNew;
//
//	Dictionary<string,Color> componentColors;
//	public void Awake()
//	{
//		Instance = this;
//	}
//
//
//	void Update()
//	{
//		if (player == null || !player.gameObject.activeInHierarchy) {
//			player = LsyCommon.FindPlayer ();
//		}
//		if (player == null) { return; }
//		if (player != null)
//		{
//			if (componentColors == null) {
//				componentColors = new Dictionary<string, Color> ();
//				foreach (var item in player.GetComponentsInChildren<SkinnedMeshRenderer>())
//				{
//					componentColors.Add(item.gameObject.name,item.material.GetColor("_Color"));
//				}
//			}
//			foreach (var item in player.GetComponentsInChildren<SkinnedMeshRenderer>())
//			{
//				var newColor = componentColors [item.gameObject.name];
//				newColor = new Color (newColor.r*playerColor.r, newColor.g*playerColor.g, newColor.b*playerColor.b, newColor.a*playerColor.a);
//				item.material.SetColor("_Color", newColor);
//			}
//		}
//	}
//
//	void OnDisable()
//	{
//		foreach (var item in player.GetComponentsInChildren<SkinnedMeshRenderer>())
//		{
//			var newColor = componentColors [item.gameObject.name];
//			item.material.SetColor("_Color", newColor);
//		}
//	}
//
//	public void TurnOn()
//	{
//		if (!HasSetResolution) {
//			HasSetResolution = true;
//			int w = (int)((float)Screen.width * 0.8f);
//			int h = (int)((float)Screen.height * 0.8f);
//			Screen.SetResolution (w, h, true, 60);
//		}
//		IsON = true;
//		Shader.DisableKeyword("SHADOW_MAP");
//		FSMessager.TurnOn ();
//		gameObject.SetActive (true);
//		RenderSettings.fog = false;
//		doorOld.SetActive (false);
//		doorNew.SetActive (true);
//	}
//	public void TurnOff()
//	{
//		IsON = false;
//		Shader.EnableKeyword("SHADOW_MAP");
//		FSMessager.TurnOff ();
//		gameObject.SetActive (false);
//		RenderSettings.fog = true;
//		doorOld.SetActive (true);
//		doorNew.SetActive (false);
//	}
//
//	//	public void SetFogColor(Color c)
//	//	{
//	//		fogFullCover.SetFogColor (c);
//	//		fog.SetFogColor (c);
//	//	}
//	public void SetFogDis(float dis,float duration,float delay)
//	{
//		//		fogFullCover.SetFogDis (dis);
//		//		fog.SetFogDis (dis);
//		DOTween.To(()=>fogDis,x=>fogDis=x,dis,duration).SetEase(Ease.InOutCubic).SetDelay(delay);
//	}
//	public void SetFogAlpha(float alpha,float duration,float delay)
//	{
//		//		fogFullCover.SetFogDis (dis);
//		//		fog.SetFogDis (dis);
//		DOTween.To(()=>fogAlpha,x=>fogAlpha=x,alpha,duration).SetEase(Ease.InOutCubic).SetDelay(delay);
//	}
//
//	public void Switch()
//	{
//		fogFullCover.gameObject.SetActive (false);
//		fog.gameObject.SetActive (true);
//	}
//
//	public float fogDis
//	{
//		get{ return fogFullCover.matParams [0].fogEnd;}
//		set{ 
//			fogFullCover.SetFogDis (value);
//			fog.SetFogDis (value);
//		}
//	}
//	public float fogAlpha
//	{
//		get{ return fogFullCover.matParams [0].color.a;}
//		set{ 
//			fogFullCover.SetFogAlpha (value);
//			fog.SetFogAlpha (value);
//		}
//	}
//}







//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using DG.Tweening;
//
//public class FSManager:MonoBehaviour{
//	public static FSManager Instance;
//	public Color playerColor;
//	public Transform player;
//
//	public CB_VolumeFog fogFullCover;
//	public CB_VolumeFog fog;
//	public static bool IsON;
//
//	private bool HasSetResolution = false;
//
//	public GameObject doorOld;
//	public GameObject doorNew;
//	public void Awake()
//	{
//		Instance = this;
//	}
//
//
//	void Update()
//	{
//		if (player == null || !player.gameObject.activeInHierarchy) {
//			player = LsyCommon.FindPlayer ();
//		}
//        if (player == null) { return; }
//        if (player != null)
//        {
//            foreach (var item in player.GetComponentsInChildren<SkinnedMeshRenderer>())
//            {
//                item.material.SetColor("_Color", playerColor);
//            }
//        }
//	}
//
//	void OnDisable()
//	{
//		foreach (var item in player.GetComponentsInChildren<SkinnedMeshRenderer>()) {
//			item.material.SetColor ("_Color", Color.white);
//		}
//	}
//
//	public void TurnOn()
//	{
//		if (!HasSetResolution) {
//			HasSetResolution = true;
//			int w = (int)((float)Screen.width * 0.8f);
//			int h = (int)((float)Screen.height * 0.8f);
//			Screen.SetResolution (w, h, true, 60);
//		}
//		IsON = true;
//		Shader.DisableKeyword("SHADOW_MAP");
//		FSMessager.TurnOn ();
//		gameObject.SetActive (true);
//		RenderSettings.fog = false;
//		doorOld.SetActive (false);
//		doorNew.SetActive (true);
//	}
//	public void TurnOff()
//	{
//		IsON = false;
//		Shader.EnableKeyword("SHADOW_MAP");
//		FSMessager.TurnOff ();
//		gameObject.SetActive (false);
//		RenderSettings.fog = true;
//		doorOld.SetActive (true);
//		doorNew.SetActive (false);
//	}
//
////	public void SetFogColor(Color c)
////	{
////		fogFullCover.SetFogColor (c);
////		fog.SetFogColor (c);
////	}
//	public void SetFogDis(float dis,float duration,float delay)
//	{
////		fogFullCover.SetFogDis (dis);
////		fog.SetFogDis (dis);
//		DOTween.To(()=>fogDis,x=>fogDis=x,dis,duration).SetEase(Ease.InOutCubic).SetDelay(delay);
//	}
//	public void SetFogAlpha(float alpha,float duration,float delay)
//	{
//		//		fogFullCover.SetFogDis (dis);
//		//		fog.SetFogDis (dis);
//		DOTween.To(()=>fogAlpha,x=>fogAlpha=x,alpha,duration).SetEase(Ease.InOutCubic).SetDelay(delay);
//	}
//
//	public void Switch()
//	{
//		fogFullCover.gameObject.SetActive (false);
//		fog.gameObject.SetActive (true);
//	}
//
//	public float fogDis
//	{
//		get{ return fogFullCover.matParams [0].fogEnd;}
//		set{ 
//			fogFullCover.SetFogDis (value);
//			fog.SetFogDis (value);
//		}
//	}
//	public float fogAlpha
//	{
//		get{ return fogFullCover.matParams [0].color.a;}
//		set{ 
//			fogFullCover.SetFogAlpha (value);
//			fog.SetFogAlpha (value);
//		}
//	}
//}
