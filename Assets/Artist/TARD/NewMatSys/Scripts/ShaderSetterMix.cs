using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumeMixer;

public partial class ShaderSetter : VMValue {
	public override void Mix (VMValue _a, float blend)
	{
		ShaderSetter a = (ShaderSetter)_a;
		MixShaderSetter (this, this, a, blend);
	}

	public static void MixShaderSetter (ShaderSetter TargetShaderSetter, ShaderSetter ShaderSetter1, ShaderSetter ShaderSetter2, float LerpForce)
	{
		TargetShaderSetter.Wetness = Mathf.Lerp(ShaderSetter1.Wetness, ShaderSetter2.Wetness, LerpForce);
		TargetShaderSetter.FogStartY = Mathf.Lerp(ShaderSetter1.FogStartY, ShaderSetter2.FogStartY, LerpForce);
		TargetShaderSetter.FogHeight = Mathf.Lerp(ShaderSetter1.FogHeight, ShaderSetter2.FogHeight, LerpForce);
		TargetShaderSetter.FogMass = Mathf.Lerp(ShaderSetter1.FogMass, ShaderSetter2.FogMass, LerpForce);
		TargetShaderSetter.DFogHeight = Mathf.Lerp(ShaderSetter1.DFogHeight, ShaderSetter2.DFogHeight, LerpForce);
		TargetShaderSetter.DFogMass = Mathf.Lerp(ShaderSetter1.DFogMass, ShaderSetter2.DFogMass, LerpForce);
		TargetShaderSetter.LightmapSlider = Mathf.Lerp(ShaderSetter1.LightmapSlider, ShaderSetter2.LightmapSlider, LerpForce);
		TargetShaderSetter.Brightness = Mathf.Lerp(ShaderSetter1.Brightness, ShaderSetter2.Brightness, LerpForce);
		TargetShaderSetter.DFogDensity = Mathf.Lerp(ShaderSetter1.DFogDensity, ShaderSetter2.DFogDensity, LerpForce);
		TargetShaderSetter.VFogDensity = Mathf.Lerp(ShaderSetter1.VFogDensity, ShaderSetter2.VFogDensity, LerpForce);
		TargetShaderSetter.FogShowSky = Mathf.Lerp(ShaderSetter1.FogShowSky, ShaderSetter2.FogShowSky, LerpForce);
		TargetShaderSetter.RotationSun = Mathf.Lerp(ShaderSetter1.RotationSun, ShaderSetter2.RotationSun, LerpForce);
		TargetShaderSetter.RotationMoonX = Mathf.Lerp(ShaderSetter1.RotationMoonX, ShaderSetter2.RotationMoonX, LerpForce);
		TargetShaderSetter.RotationMoonY = Mathf.Lerp(ShaderSetter1.RotationMoonY, ShaderSetter2.RotationMoonY, LerpForce);
		TargetShaderSetter.CloudCoverage = Mathf.Lerp(ShaderSetter1.CloudCoverage, ShaderSetter2.CloudCoverage, LerpForce);
        //TargetShaderSetter.VFogDensity = Mathf.Lerp(ShaderSetter1.VFogDensity, ShaderSetter2.VFogDensity, LerpForce);

        TargetShaderSetter.CharaLmShadowForce = Mathf.Lerp(ShaderSetter1.CharaLmShadowForce, ShaderSetter2.CharaLmShadowForce, LerpForce);
        TargetShaderSetter.LmLightForceInShadow = Mathf.Lerp(ShaderSetter1.LmLightForceInShadow, ShaderSetter2.LmLightForceInShadow, LerpForce);
        TargetShaderSetter.LmLightForceInLight = Mathf.Lerp(ShaderSetter1.LmLightForceInLight, ShaderSetter2.LmLightForceInLight, LerpForce);

        TargetShaderSetter.FogFar = Color.Lerp(ShaderSetter1.FogFar, ShaderSetter2.FogFar, LerpForce);
		TargetShaderSetter.FogNear = Color.Lerp(ShaderSetter1.FogNear, ShaderSetter2.FogNear, LerpForce);
		TargetShaderSetter.FogLow = Color.Lerp(ShaderSetter1.FogLow, ShaderSetter2.FogLow, LerpForce);
		TargetShaderSetter.FogHigh = Color.Lerp(ShaderSetter1.FogHigh, ShaderSetter2.FogHigh, LerpForce);

		float gammaLerp = Mathf.Pow(LerpForce, 0.4545f);
		TargetShaderSetter.LightColor = Color.Lerp(ShaderSetter1.LightColor, ShaderSetter2.LightColor, gammaLerp);
		TargetShaderSetter.ShadowColor = Color.Lerp(ShaderSetter1.ShadowColor, ShaderSetter2.ShadowColor, gammaLerp);
		//TargetShaderSetter.CloudShadowColor = Color.Lerp(ShaderSetter1.CloudShadowColor, ShaderSetter2.CloudShadowColor, gammaLerp);
		TargetShaderSetter.SkyLowColor = Color.Lerp(ShaderSetter1.SkyLowColor, ShaderSetter2.SkyLowColor, gammaLerp);
		TargetShaderSetter.SkyHighColor = Color.Lerp(ShaderSetter1.SkyHighColor, ShaderSetter2.SkyHighColor, gammaLerp);
		TargetShaderSetter.CloudLight = Color.Lerp(ShaderSetter1.CloudLight, ShaderSetter2.CloudLight, gammaLerp);
		TargetShaderSetter.CloudDark = Color.Lerp(ShaderSetter1.CloudDark, ShaderSetter2.CloudDark, gammaLerp);
		TargetShaderSetter.CloudDarkControl = Color.Lerp(ShaderSetter1.CloudDarkControl, ShaderSetter2.CloudDarkControl, gammaLerp);
		TargetShaderSetter.CharaLight = Color.Lerp(ShaderSetter1.CharaLight, ShaderSetter2.CharaLight, gammaLerp);
		TargetShaderSetter.CharaDark = Color.Lerp(ShaderSetter1.CharaDark, ShaderSetter2.CharaDark, gammaLerp);
		TargetShaderSetter.CharaSkinDark = Color.Lerp(ShaderSetter1.CharaSkinDark, ShaderSetter2.CharaSkinDark, gammaLerp);
		TargetShaderSetter.GrassColor = Color.Lerp(ShaderSetter1.GrassColor, ShaderSetter2.GrassColor, gammaLerp);
		TargetShaderSetter.GrassShadowColor = Color.Lerp(ShaderSetter1.GrassShadowColor, ShaderSetter2.GrassShadowColor, gammaLerp);
		TargetShaderSetter.CloudyColor = Color.Lerp(ShaderSetter1.CloudyColor, ShaderSetter2.CloudyColor, gammaLerp);
        TargetShaderSetter.LmMultiply = Color.Lerp(ShaderSetter1.LmMultiply, ShaderSetter2.LmMultiply, gammaLerp);

        //Skybox
        TargetShaderSetter.skybox_TexLerp = Mathf.Lerp(ShaderSetter1.skybox_TexLerp, ShaderSetter2.skybox_TexLerp, gammaLerp);
	}

	#region SkyBox
	[Space(10)]
	[Header("Skybox settings")]
	[Range(0,1)]
	public float skybox_TexLerp;
	public void SetSkybox()
	{
		var skyMat = RenderSettings.skybox;
		if (skyMat != null) {
			if(skyMat.HasProperty ("_TexLerp"))
				skyMat.SetFloat ("_TexLerp", skybox_TexLerp);
		}
	}
	#endregion
}