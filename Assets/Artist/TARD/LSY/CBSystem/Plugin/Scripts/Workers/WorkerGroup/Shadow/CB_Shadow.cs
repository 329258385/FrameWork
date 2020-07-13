using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CB_Shadow : CB_WorkerGroupBase {
	public RenderTexture rt;

	public override void OnEnable ()
	{
		base.OnEnable ();
		rt = new RenderTexture (Resolution, Resolution,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);
		rt.name = "Lsy RT-CB_Shadow";
	}
	public override void Render (CommandBuffer buf)
	{
//		foreach (var sub in subs) {
//			sub.Render (this);
//		}
		for (int i = 0; i < subs.Count; i++) {
			((CB_ShadowSub)subs [i]).clear = i == 0;
			subs[i].Render (this);
		}

		//Step2: Blur
		int idBlur = Shader.PropertyToID ("_VLBlur");
		buf.GetTemporaryRT (idBlur, Resolution, Resolution, 1);
		//buf.GetTemporaryRT (idBlur, Resolution, Resolution, 1,FilterMode.Trilinear,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear,3);
		if (blurOn) {
			buf.Blit (rt, idBlur, mat_2_blur, 1);
			buf.Blit (idBlur, rt, mat_2_blur, 2);
		}

		buf.SetGlobalTexture ("_VL", rt);
		//Step3: Use result
		buf.SetRenderTarget (BuiltinRenderTextureType.CameraTarget);
		foreach (var item in subs) {
			if (!item.gameObject.activeSelf)
				continue;
			foreach(var mat in mats)
				buf.DrawMesh(mesh,item.root.localToWorldMatrix,mat);
		}

		//base.Render (buf);
		buf.ReleaseTemporaryRT (idBlur);
	}
}
