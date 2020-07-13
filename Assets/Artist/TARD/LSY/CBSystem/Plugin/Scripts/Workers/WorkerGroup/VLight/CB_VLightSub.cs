using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CB_VLightSub : CB_WorkerGroupSubBase {
	protected override void SetShadowMap ()
	{
		mat.SetTexture ("_ShadowMapp", lightCB.GetShadowMap());
	}
}
