using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class CB_LightCB : MonoBehaviour {
	public Light ml;
	public CommandBuffer buf;
	public int Resolution = 256;
	protected RenderTexture rtShadowMap;

	public virtual void On_Enable()
	{
		buf = new CommandBuffer();
		ml.AddCommandBuffer(LightEvent.AfterShadowMap, buf);
	}

	public virtual void On_Disable()
	{
		ml.RemoveCommandBuffer(LightEvent.AfterShadowMap, buf);
	}
		
	public RenderTexture GetShadowMap () {
		rtShadowMap = new RenderTexture (Resolution, Resolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		rtShadowMap.name = "Lsy RT-CB_LightCB ShadowMap "+gameObject.name;

		buf.SetShadowSamplingMode(BuiltinRenderTextureType.CurrentActive, ShadowSamplingMode.RawDepth);
		RenderTargetIdentifier id = new RenderTargetIdentifier(rtShadowMap);
		buf.Blit(BuiltinRenderTextureType.CurrentActive,id );

		return rtShadowMap;
	}


	public Matrix4x4 GetMatrixVP()
	{
		if (ml.type == LightType.Spot) {
			return GetMatrixVP_Spot ();
		}

		return Matrix4x4.identity;
	}

	private Matrix4x4 GetMatrixVP_Spot()
	{
		Matrix4x4 view = Matrix4x4.TRS (ml.transform.position, ml.transform.rotation,Vector3.one).inverse;
		Matrix4x4 proj = Matrix4x4.Perspective(ml.spotAngle, 1, ml.range, ml.shadowNearPlane);
		if(CB_VLight._reversedZ)
			proj = Matrix4x4.Perspective(ml.spotAngle, 1, ml.range, ml.shadowNearPlane);
		else
			proj = Matrix4x4.Perspective(ml.spotAngle, 1, ml.shadowNearPlane, ml.range);
		Matrix4x4 clip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
		Matrix4x4 m = clip * proj;
		m[0, 2] *= -1;
		m[1, 2] *= -1;
		m[2, 2] *= -1;
		m[3, 2] *= -1;

		return m * view;
	}
}
