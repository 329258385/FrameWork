using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShaderSetter))]
[ExecuteInEditMode]
public class ShaderSetterEditor : Editor
{

    SerializedObject obj;
    ShaderSetter ss;
    bool showGradients;
    bool showCurves;
    bool showCloudSettings;
    bool showTexture;
    bool showFog;
    bool showChara;
    bool showRain;
    bool showGrass;
    bool showBrightnessSaturation;
    bool showLightConfig;
    bool showSky;
    bool showSkyboxSetting;
    bool showColorSelector;

    public void ShowGradient(string guiContent, string propertyName)
    {
        EditorGUI.BeginChangeCheck();
        SerializedProperty sp = obj.FindProperty(propertyName);
        GUIContent g = new GUIContent(guiContent);
        EditorGUILayout.PropertyField(sp, g);
        if (EditorGUI.EndChangeCheck())
            obj.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        obj = new SerializedObject(target);
        Undo.RecordObject(target, "dad");
        obj.Update();
        ss = (ShaderSetter)target;
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Weather Lock");
        ss.IsLock = EditorGUILayout.Toggle("Is Lock", ss.IsLock);

        ss.Player = (Transform)EditorGUILayout.ObjectField("PlayerTransform", ss.Player, typeof(Transform));

        //ColorSelector(是否使用?)
        showColorSelector = EditorGUILayout.Foldout(showColorSelector, "ColorSelector(是否使用?)");
        if (showColorSelector)
        {
            EditorGUILayout.LabelField("ColorSelector");
            ss.IsColorSetter = EditorGUILayout.Toggle("Is Color Setter", ss.IsColorSetter);
            ss.TimeHour = EditorGUILayout.Slider("Time Hour", ss.TimeHour, 0f, 24f);
            ss.FirstIndex = EditorGUILayout.IntField("First Index", ss.FirstIndex);
            ss.SecondIndex = EditorGUILayout.IntField("Second Index", ss.SecondIndex);
            ss.LerpValue = EditorGUILayout.Slider("Lerp Value", ss.LerpValue, 0f, 1f);
        }

        //贴图及材质
        showTexture = EditorGUILayout.Foldout(showTexture, "贴图及材质设置");
        if (showTexture)
        {
            //ss.ColorSelectorTex = (Texture2D)EditorGUILayout.ObjectField("Color Selector Tex", ss.ColorSelectorTex, typeof(Texture2D));
            ss.BRDFTex = (Texture2D)EditorGUILayout.ObjectField("BRDF Tex", ss.BRDFTex, typeof(Texture2D));
            ss.NoiseTex = (Texture2D)EditorGUILayout.ObjectField("Noise Tex", ss.NoiseTex, typeof(Texture2D));
            ss.CloudNoiseTex = (Texture2D)EditorGUILayout.ObjectField("Cloud Noise Tex", ss.CloudNoiseTex, typeof(Texture2D));
            ss.SkyBoxMat = (Material)EditorGUILayout.ObjectField("Sky Box Mat", ss.SkyBoxMat, typeof(Material));
        }

        //光效设置
        showLightConfig = EditorGUILayout.Foldout(showLightConfig, "光效设置");
        if (showLightConfig)
        {
            ss._FresnelLightColor = EditorGUILayout.ColorField("Fresnel Light Color", ss._FresnelLightColor);
            ss._FresnelDarkColor = EditorGUILayout.ColorField("Fresnel Dark Color", ss._FresnelDarkColor);
            ss._SOLightFresnel = EditorGUILayout.Slider("SO Light Fresnel", ss._SOLightFresnel, 0f, 1f);
            ss._SODarkFresnel = EditorGUILayout.Slider("SO Dark Fresnel", ss._SODarkFresnel, 0f, 1f);
            ss._SOLightFresnelFar = EditorGUILayout.Slider("SO Light Fresnel Far", ss._SOLightFresnelFar, 0f, 100f);
            ss._SODarkFresnelFar = EditorGUILayout.Slider("SO Dark Fresnel Far", ss._SODarkFresnelFar, 0f, 100f);
            ss._SOLightFresnelFarForce = EditorGUILayout.Slider("SO Light Fresnel Far Force", ss._SOLightFresnelFarForce, 0.001f, 5f);
            ss._SODarkFresnelFarForce = EditorGUILayout.Slider("SO Dark Fresnel Far Force", ss._SODarkFresnelFarForce, 0.001f, 5f);
            EditorGUILayout.LabelField("Debugger");
            ss.LightColor = EditorGUILayout.ColorField("LightColor", ss.LightColor);
            ss.ShadowColor = EditorGUILayout.ColorField("Shadow", ss.ShadowColor);
            ss.Bake = EditorGUILayout.Toggle("Bake", ss.Bake);
            ss.LightmapSlider = EditorGUILayout.Slider("Lightmap Slider", ss.LightmapSlider, 0f, 1f);
            ss.Brightness = EditorGUILayout.Slider("Brightness", ss.Brightness, 0f, 5f);
            ss.Saturation = EditorGUILayout.Slider("Saturation", ss.Saturation, 0.0f, 2.0f);
            ss.RealTimeLightOnLightmap = EditorGUILayout.Slider("Real Time Light On Lightmap", ss.RealTimeLightOnLightmap, 0.0f, 1.0f);
            ss.ShadowOnLightmap = EditorGUILayout.Slider("Shadow On Lightmap", ss.ShadowOnLightmap, 0.0f, 1.0f);
            GUIContent gc = new GUIContent("Lm Multiply");
            ss.LmMultiply = EditorGUILayout.ColorField(gc, ss.LmMultiply,true,true,true);
        }


        //角色
        showChara = EditorGUILayout.Foldout(showChara, "角色设置");
        if (showChara)
        {
            ss.IsUiChara = EditorGUILayout.Toggle("Is Ui Chara", ss.IsUiChara);
            ss.UiCharaLight = EditorGUILayout.ColorField("Ui Chara Light", ss.UiCharaLight);
            ss.UiCharaDark = EditorGUILayout.ColorField("Ui Chara Dark", ss.UiCharaDark);
            ss.UiCharaSkinDark = EditorGUILayout.ColorField("Ui Chara Skin Dark", ss.UiCharaSkinDark);
            ss.CharaBackLight = EditorGUILayout.Slider("Chara Back Light", ss.CharaBackLight, 0f, 1f);
            ss.CharaLightRot = EditorGUILayout.Slider("Chara Light Rot", ss.CharaLightRot, 0f, 6.28f);
            ss.CharaLmShadowForce = EditorGUILayout.Slider("Chara Lm Shadow Force", ss.CharaLmShadowForce, 0f, 1f);
            ss.LmLightForceInShadow = EditorGUILayout.Slider("Lm Light Force In Shadow", ss.LmLightForceInShadow, 0f, 1f);
            ss.LmLightForceInLight = EditorGUILayout.Slider("Lm Light Force In Light", ss.LmLightForceInLight, 0f, 1f);
            ss.CharaLightModelDiff = (Texture2D)EditorGUILayout.ObjectField("Chara Light Model Diff", ss.CharaLightModelDiff, typeof(Texture2D));
            ss.CharaLightModel = (Texture2D)EditorGUILayout.ObjectField("Chara Light Model", ss.CharaLightModel, typeof(Texture2D));
            ss.CharaLight = EditorGUILayout.ColorField("Chara Light", ss.CharaLight);
            ss.CharaDark = EditorGUILayout.ColorField("Chara Dark", ss.CharaDark);
            ss.CharaSkinDark = EditorGUILayout.ColorField("Chara Skin Dark", ss.CharaSkinDark);
        }

        //雾
        showFog = EditorGUILayout.Foldout(showFog, "雾");
        if (showFog)
        {
            ss.CameraFogOn = EditorGUILayout.Toggle("Camera Fog On", ss.CameraFogOn);
            ss.CameraFogStart = EditorGUILayout.Toggle("Camera Fog Start", ss.CameraFogStart);
            ss.FogStartY = EditorGUILayout.FloatField("Fog Start Y", ss.FogStartY);
            ss.FogHeight = EditorGUILayout.FloatField("Fog Height", ss.FogHeight);
            ss.VFogVariation1 = EditorGUILayout.Slider("V Fog Variation 1", ss.VFogVariation1, 0f, 1f);
            ss.FogMass = EditorGUILayout.FloatField("Fog Mass", ss.FogMass);
            ss.DFogHeight = EditorGUILayout.Slider("D Fog Height", ss.DFogHeight, -100f, 100f);
            ss.DFogMass = EditorGUILayout.Slider("D Fog Mass", ss.DFogMass, 0f, 10f);
            ss.FarFogDistance = EditorGUILayout.FloatField("Far Fog Distance", ss.FarFogDistance);
            ss.FogFar = EditorGUILayout.ColorField("Fog Far", ss.FogFar);
            ss.FogNear = EditorGUILayout.ColorField("Fog Near", ss.FogNear);
            ss.FogLow = EditorGUILayout.ColorField("Fog Low", ss.FogLow);
            ss.FogHigh = EditorGUILayout.ColorField("Fog High", ss.FogHigh);
            ss.DFogDensity = EditorGUILayout.Slider("D Fog Density", ss.DFogDensity, 0f, 100f);
            ss.VFogDensity = EditorGUILayout.Slider("V Fog Density", ss.VFogDensity, 0f, 100f);
            ss.SencondFogFar = EditorGUILayout.ColorField("Second Fog Far", ss.SencondFogFar);
            ss.SecondLevelFogDistance = EditorGUILayout.Slider("Second Level Fog Distance", ss.SecondLevelFogDistance, 0f, 10f);
            ss.SecondLevelFogForce = EditorGUILayout.Slider("Second Level Fog Force", ss.SecondLevelFogForce, 0f, 1f);
            ss.FogShowSky = EditorGUILayout.Slider("Fog Show Sky", ss.FogShowSky, 0f, 1f);
        }

        //云
        showCloudSettings = EditorGUILayout.Foldout(showCloudSettings, "云");
        if (showCloudSettings)
        {
            ss.CloudLight = EditorGUILayout.ColorField("Cloud Light", ss.CloudLight);
            ss.CloudDark = EditorGUILayout.ColorField("Cloud Dark", ss.CloudDark);
            ss.CloudDarkControl = EditorGUILayout.ColorField("Cloud Dark Control", ss.CloudDarkControl);
            ss.CloudCoverage = EditorGUILayout.Slider("Cloud Coverage", ss.CloudCoverage, 0f, 100f);
            ss.CloudShadow = EditorGUILayout.Slider("Cloud Shadow", ss.CloudShadow, 0f, 1f);
        }

        //雨
        showRain = EditorGUILayout.Foldout(showRain, "雨");
        if (showRain)
        {
            ss.CloudyColor = EditorGUILayout.ColorField("Rain Color", ss.CloudyColor);
            ss.Wetness = EditorGUILayout.Slider("Wetness", ss.Wetness, 0f, 1f);
            ss.RainForce = EditorGUILayout.Slider("Rain Force", ss.RainForce, 0f, 1f);
        }



        //天空盒
        showSkyboxSetting = EditorGUILayout.Foldout(showSkyboxSetting, "天空盒");
        if (showSkyboxSetting)
        {
            ss.skybox_TexLerp = EditorGUILayout.Slider("Skybox_Tex Lerp", ss.skybox_TexLerp, 0f, 1f);
        }

        //天空
        showSky = EditorGUILayout.Foldout(showSky, "天空");
        if (showSky)
        {
            ss.SkyLowColor = EditorGUILayout.ColorField("Sky Low Color", ss.SkyLowColor);
            ss.SkyHighColor = EditorGUILayout.ColorField("Sky High Color", ss.SkyHighColor);
            ss.RotationSun = EditorGUILayout.Slider("Rotation Sun", ss.RotationSun, 0f, 360f);
            ss.RotationMoonX = EditorGUILayout.Slider("Rotation Moon X", ss.RotationMoonX, 0f, 360f);
            ss.RotationMoonY = EditorGUILayout.Slider("Rotation Moon Y", ss.RotationMoonY, 0f, 360f);
        }

        //草
        showGrass = EditorGUILayout.Foldout(showGrass, "草");
        if (showGrass)
        {
            GUIContent gc1 = new GUIContent("Grass Color");
            GUIContent gc2 = new GUIContent("Grass Shadow Color");
            //ss.GrassColor = EditorGUILayout.ColorField("Grass Color", ss.GrassColor);
            ss.GrassColor = EditorGUILayout.ColorField(gc1,ss.GrassColor,true,true,true);
            ss.GrassShadowColor = EditorGUILayout.ColorField(gc2,ss.GrassShadowColor,true,true,true);
        }


        //杂项
        showBrightnessSaturation = EditorGUILayout.Foldout(showBrightnessSaturation, "杂项");
        if (showBrightnessSaturation)
        {
            
            ss.mIsDestroyed = EditorGUILayout.Toggle("M Is Destroyed", ss.mIsDestroyed);
        }

        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        obj.ApplyModifiedProperties();
    }
}
