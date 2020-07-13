using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CB_VolumeFog : CB_WorkerBase {
	public void SetFogColor(Color c)
	{
		matParams [0].color = c;
		MatSyncAll ();
	}
	public void SetFogAlpha(float alpha)
	{
		var c = matParams [0].color;
		matParams [0].color = new Color(c.r,c.g,c.b,alpha);
		MatSyncAll ();
	}

	public void SetFogDis(float dis)
	{
		matParams [0].fogEnd = dis;
		MatSyncAll ();
	}
}
