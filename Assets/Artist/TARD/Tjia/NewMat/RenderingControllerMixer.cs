using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumeMixer;

public partial class RenderingController : VMValue {
	public override void Mix (VMValue _a, float blend)
	{
		if (_a == null) {
			Debug.LogError ("未添加RenderingController");
			return;
		}
		if (_a is RenderingController) {
			RenderingController a = (RenderingController)_a;
			MixRenderingController (this, this, a, blend);
		} else {
			Debug.LogError (string.Format ("请清除{0}的ShaderSetter", _a.gameObject.name));
		}
	}


    // Use this for initialization
    public static void MixRenderingController(RenderingController z, RenderingController x, RenderingController y, float t)
    {
        z.LightmapForce = Mathf.Lerp(x.GetValue(x.LightmapForceCurve, x.LightmapForce), y.GetValue(y.LightmapForceCurve, y.LightmapForce), t);
        z.GlobalEmissionLevel = Mathf.Lerp(x.GetValue(x.GlobalEmissionLevelCurve, x.GlobalEmissionLevel), y.GetValue(y.GlobalEmissionLevelCurve, y.GlobalEmissionLevel), t);
        //z.SkyBoxLerp = Mathf.Lerp(x.GetValue(x.SkyBoxLerpCurve, x.SkyBoxLerp), y.GetValue(y.SkyBoxLerpCurve, y.SkyBoxLerp), t);
        z.SkyBoxLerp = Mathf.Lerp(x.SkyBoxLerp, y.SkyBoxLerp, t);
        z.WindForce = Mathf.Lerp(x.WindForce, y.WindForce, t);
        z.WindSpeed = Mathf.Lerp(x.WindSpeed, y.WindSpeed, t);
        z.FogDensity = Mathf.Lerp(x.GetValue(x.FogDensityCurve, x.FogDensity), y.GetValue(y.FogDensityCurve, y.FogDensity), t);
        z.FogStartDistance = Mathf.Lerp(x.GetValue(x.FogStartDistanceCurve, x.FogStartDistance), y.GetValue(y.FogStartDistanceCurve, y.FogStartDistance), t);
        z.FogHeightFallOut = Mathf.Lerp(x.GetValue(x.FogHeightFallOutCurve, x.FogHeightFallOut), y.GetValue(y.FogHeightFallOutCurve, y.FogHeightFallOut), t);
        z.FogHeightFallOutPos = Mathf.Lerp(x.GetValue(x.FogHeightFallOutPosCurve, x.FogHeightFallOutPos), y.GetValue(y.FogHeightFallOutPosCurve, y.FogHeightFallOutPos), t);
        z.VerticalFogDensity = Mathf.Lerp(x.GetValue(x.VerticalFogDensityCurve, x.VerticalFogDensity), y.GetValue(y.VerticalFogDensityCurve, y.VerticalFogDensity), t);
        z.VerticalFogHeight = Mathf.Lerp(x.GetValue(x.VerticalFogHeightCurve, x.VerticalFogHeight), y.GetValue(y.VerticalFogHeightCurve, y.VerticalFogHeight), t);
        z.Sky_FalloutModif = Mathf.Lerp(x.GetValue(x.Sky_FalloutModifCurve, x.Sky_FalloutModif), y.GetValue(y.Sky_FalloutModifCurve, y.Sky_FalloutModif), t);
        z.Sky_FalloutModifHeight = Mathf.Lerp(x.GetValue(x.Sky_FalloutModifHeightCurve, x.Sky_FalloutModifHeight), y.GetValue(y.Sky_FalloutModifHeightCurve, y.Sky_FalloutModifHeight), t);
        RenderSettings.sun.intensity = z.SunIntensityForMixer = Mathf.Lerp(x.GetValue(x.SunIntensityCurve, x.SunIntensityForMixer), y.GetValue(y.SunIntensityCurve, y.SunIntensityForMixer), t);

        t = Mathf.Pow(t, 0.4545f);
        z.LightmapColor = Color.Lerp(x.GetColor(x.LightmapColorGradient, x.LightmapColor), y.GetColor(y.LightmapColorGradient, y.LightmapColor), t);
        z.FogColor = Color.Lerp(x.GetColor(x.FogColorGradient, x.FogColor), y.GetColor(y.FogColorGradient, y.FogColor), t);
        z.FogLightColor = Color.Lerp(x.GetColor(x.FogLightColorGradient, x.FogLightColor), y.GetColor(y.FogLightColorGradient, y.FogLightColor), t);
        z.CharaLight = Color.Lerp(x.GetColor(x.CharaLightGradient, x.CharaLight), y.GetColor(y.CharaLightGradient, y.CharaLight), t);
        z.CharaDark = Color.Lerp(x.GetColor(x.CharaDarkGradient, x.CharaDark), y.GetColor(y.CharaDarkGradient, y.CharaDark), t);
        z.CharaSkinDark = Color.Lerp(x.GetColor(x.CharaSkinDarkGradient, x.CharaSkinDark), y.GetColor(y.CharaSkinDarkGradient, y.CharaSkinDark), t);
        z.GrassColor = Color.Lerp(x.GetColor(x.GrassColorGradient, x.GrassColor), y.GetColor(y.GrassColorGradient, y.GrassColor), t);
        RenderSettings.sun.color = z.SunColorForMixer = Color.Lerp(x.GetColor(x.SunColorGradient, x.SunColorForMixer), y.GetColor(y.SunColorGradient, y.SunColorForMixer), t);
        RenderSettings.ambientSkyColor = z.SkyColorForMixer = Color.Lerp(x.GetColor(x.SkyEnvColorGradient, x.SkyColorForMixer), y.GetColor(y.SkyEnvColorGradient, y.SkyColorForMixer), t);
        RenderSettings.ambientEquatorColor = z.EquatorColorForMixer = Color.Lerp(x.GetColor(x.EquatorEnvColorGradient, x.EquatorColorForMixer), y.GetColor(y.EquatorEnvColorGradient, y.EquatorColorForMixer), t);
        RenderSettings.ambientGroundColor = z.GroundColorForMixer = Color.Lerp(x.GetColor(x.GroundEnvColorGradient, x.GroundColorForMixer), y.GetColor(y.GroundEnvColorGradient, y.GroundColorForMixer), t);
    }
}
