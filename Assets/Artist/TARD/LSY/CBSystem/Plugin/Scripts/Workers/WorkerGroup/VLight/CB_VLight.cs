using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CB_VLight : CB_WorkerGroupBase {
	protected override Mesh GenerateMesh ()
	{
		Mesh mesh = CB_LibVLight.CreateSpotLightMesh ();
		return mesh;
	}


	public override void Render (CommandBuffer buf)
	{
		//Step1: RayMarch
		int id = Shader.PropertyToID ("_VL");
		buf.GetTemporaryRT (id, Resolution, Resolution);
		//buf.GetTemporaryRT (id, Resolution, Resolution,1,FilterMode.Trilinear,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear,3);

		buf.Blit(BuiltinRenderTextureType.CameraTarget,id );
		buf.SetRenderTarget (id);
		buf.ClearRenderTarget (false, true, Color.black);
		foreach (var item in subs) {
			if (item == null || !item.gameObject.activeSelf)
				continue;
			item.SetupLightVPMatrix ();
			buf.DrawMesh(mesh,item.root.localToWorldMatrix,item.mat);
		}
		//buf.Blit (id, testRT);


		//Step2: Blur
		int idBlur = Shader.PropertyToID ("_VLBlur");
		buf.GetTemporaryRT (idBlur, Resolution, Resolution, 1);
		//buf.GetTemporaryRT (idBlur, Resolution, Resolution, 1,FilterMode.Trilinear,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear,3);
		if (blurOn) {
			buf.Blit (id, idBlur, mat_2_blur, 1);
			buf.Blit (idBlur, id, mat_2_blur, 2);
		}

		//Step3: Use result
		buf.SetRenderTarget (BuiltinRenderTextureType.CameraTarget);
		foreach (var item in subs) {
			if (!item.gameObject.activeSelf)
				continue;
			foreach(var mat in mats)
				buf.DrawMesh(mesh,item.root.localToWorldMatrix,mat);


		}

		//base.Render (buf);
		buf.ReleaseTemporaryRT (id);
		buf.ReleaseTemporaryRT (idBlur);
	}
}