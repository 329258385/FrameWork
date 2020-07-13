using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[ExecuteInEditMode]
public class CB_ShadowSub : CB_WorkerGroupSubBase {
	public bool clear = false;
	protected override void SetShadowMap ()
	{
		base.SetShadowMap ();
	}

	public override void Render (CB_WorkerGroupBase group)
	{
		CB_Shadow shadow =(CB_Shadow)group;

		var cb = lightCB.buf;
		mat.SetMatrix ("_vp", group.camManager.GetMatrixVP());
		mat.SetMatrix ("_v",  group.camManager.GetMatrixV());

		cb.Clear ();
		Matrix4x4 ma = Matrix4x4.TRS (root.position,root.rotation, root.lossyScale);

		cb.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
		cb.SetRenderTarget (shadow.rt);
		if(clear)
			cb.ClearRenderTarget (false, true, new Color(0,0,0,0));
		mat.EnableKeyword("SHADOWS_CUBE");
		mat.EnableKeyword("SHADOWS_CUBE_IN_DEPTH_TEX");
		mat.SetVector ("_LPos", lightCB.ml.transform.position);
		cb.DrawMesh (group.mesh, ma, mat);
	}
}
