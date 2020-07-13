//using UnityEngine;
//using UnityEngine.Rendering;
//
//
//public class CB_LightShadowMap : CB_LightCB {
//	public int Resolution = 256;
//	public RenderTexture rt;
//	public RenderTexture GetShadowMap () {
//		rt = new RenderTexture (Resolution, Resolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
//		rt.name = gameObject.name;
//		buf = new CommandBuffer();
//		buf.SetShadowSamplingMode(BuiltinRenderTextureType.CurrentActive, ShadowSamplingMode.RawDepth);
//		RenderTargetIdentifier id = new RenderTargetIdentifier(rt);
//		buf.Blit(BuiltinRenderTextureType.CurrentActive,id );
//		ml.AddCommandBuffer(LightEvent.AfterShadowMap, buf);
//		return rt;
//	}
//}


using UnityEngine;
using UnityEngine.Rendering;


public class CB_LightShadowMap : CB_LightCB {
	

}
