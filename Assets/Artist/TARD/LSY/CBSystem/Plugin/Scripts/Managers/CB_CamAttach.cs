using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class CB_CamAttach : MonoBehaviour {
	private Action actionPreRender;

	public void AddAction(Action _act)
	{
		//actionPreRender += _act;
		actionPreRender = _act;
	}
	public void RemoveAction(Action _act)
	{
		//actionPreRender -= _act;
		actionPreRender = null;
	}

	void OnPreRender()
	{
		if (actionPreRender != null)
			actionPreRender ();
	}

//	public void OnRenderImage(RenderTexture source, RenderTexture dest)
//	{
//		
//	}
}
