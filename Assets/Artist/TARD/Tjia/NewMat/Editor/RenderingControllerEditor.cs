using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(RenderingController))]
public class RenderingControllerEditor : Editor {

    public bool ShowFogRegion = false;
    public bool Show24HoursRegion = false;
    public bool ShowTimePoints = false;
    public bool ShowColorGradients = false;
    public bool ShowLightmapSetting = false;
    public bool ShowWindRegion = false;
    public bool ShowCharactorSetting = false;
    public bool ShowCurves = false;
    public bool ShowSkyBoxRegion = false;
    public bool ShowRainRegion = false;

    SerializedProperty DayTime;
    static GUIContent TextDayTime = new GUIContent("时间点");

    //SerializedProperty AutoSave;
    //static GUIContent TextAutoSave = new GUIContent("自动保存");

    SerializedProperty LightmapColor;
    SerializedProperty LightmapForce;
    SerializedProperty GlobalEmissionLevel;
    SerializedProperty InDoor;
    SerializedProperty DaySkyBox;
    SerializedProperty MorningSkyBox;
    SerializedProperty DuskSkyBox;
    SerializedProperty NightSkyBox;
    static GUIContent TextLightmapColor = new GUIContent("LM颜色");
    static GUIContent TextLightmapForce = new GUIContent("LM强度");
    static GUIContent TextGlobalEmissionLevel = new GUIContent("全局自发光强度");
    static GUIContent TextInDoor = new GUIContent("室内场景");

    SerializedProperty OpenFog;
    static GUIContent TextOpenFog = new GUIContent("开启雾效");
    SerializedProperty FogColor;
    static GUIContent TextFogColor = new GUIContent("雾色");
    SerializedProperty FogLightColor;
    static GUIContent TextFogLightColor = new GUIContent("光雾色");
    SerializedProperty FogDensity;
    static GUIContent TextFogDensity = new GUIContent("浓度");
    SerializedProperty FogStartDistance;
    static GUIContent TextFogStartDistance = new GUIContent("起始距离");
    SerializedProperty FogHeightFallOut;
    static GUIContent TextFogHeightFallOut = new GUIContent("高度消散强度");
    SerializedProperty FogHeightFallOutPos;
    static GUIContent TextFogHeightFallOutPos = new GUIContent("高度消散位置");
    SerializedProperty Sky_FalloutModif;
    static GUIContent TextSky_FalloutModif = new GUIContent("天空高度消散调整");
    SerializedProperty Sky_FalloutModifHeight;
    static GUIContent TextSky_FalloutModifHeight = new GUIContent("天空高度消散位置调整");
    SerializedProperty VerticalFogDensity;
    static GUIContent TextVerticalFogDensity = new GUIContent("高度雾强度");
    SerializedProperty VerticalFogHeight;
    static GUIContent TextVerticalFogHeight = new GUIContent("高度雾高度(距离相机)");

    SerializedProperty CharaLight;
    SerializedProperty CharaDark;
    SerializedProperty CharaSkinDark;
    //SerializedProperty UICharaLight;
    //SerializedProperty UICharaDark;
    //SerializedProperty UICharaSkinDark;
    static GUIContent TextCharaLight = new GUIContent("光色");
    static GUIContent TextCharaDark = new GUIContent("一般暗色");
    static GUIContent TextCharaSkinDark = new GUIContent("皮肤暗色");
    SerializedProperty IsUIChara;
    static GUIContent TextIsUIChara = new GUIContent("启动UI设置");

    SerializedProperty CharaLightGradient;
    SerializedProperty CharaDarkGradient;
    SerializedProperty CharaSkinDarkGradient;
    static GUIContent TextCharaLightGradient = new GUIContent("角色光色");
    static GUIContent TextCharaDarkGradient = new GUIContent("角色一般暗色");
    static GUIContent TextCharaSkinDarkGradient = new GUIContent("角色皮肤暗色");

    SerializedProperty WindForce;
    SerializedProperty WindSpeed;
    static GUIContent TextWindForce = new GUIContent("全局风力(目前非24小时)");
    static GUIContent TextWindSpeed = new GUIContent("全局风频(目前非24小时)");
    SerializedProperty GrassColor;
    static GUIContent TextGrassColor = new GUIContent("草色");

    SerializedProperty CloudySaturation;
    SerializedProperty CloudySwitch;
    SerializedProperty CloudyColor;
    SerializedProperty CloudySky;
    SerializedProperty CloudyLevel;
    static GUIContent TextCloudySaturation = new GUIContent("阴天饱和");
    static GUIContent TextCloudyColor = new GUIContent("阴天光影颜色");
    static GUIContent TextCloudySky = new GUIContent("阴天天空盒颜色");
    static GUIContent TextCloudySwitch = new GUIContent("阴天开关");
    static GUIContent TextCloudyLevel = new GUIContent("阴天程度");

    SerializedProperty Is24Hours;
    static GUIContent TextIs24Hours = new GUIContent("开启24小时(不支持HDR颜色)");
    SerializedProperty Test24;
    static GUIContent TextTest24 = new GUIContent("测试24小时变化(需运行,30秒为一天)");
    SerializedProperty TimeOf24;
    static GUIContent TextTimeOf24 = new GUIContent("时间");

    //static GUIContent TextSaveTimePoint = new GUIContent("保存时间点设置(更新24小时)");

    SerializedProperty NightEnd;
    SerializedProperty MorningStart;
    SerializedProperty MorningEnd;
    SerializedProperty DayStart;
    SerializedProperty DayEnd;
    SerializedProperty DuskStart;
    SerializedProperty DuskEnd;
    SerializedProperty NightStart;
    static GUIContent TextNightEnd = new GUIContent("黑夜 结束");
    static GUIContent TextMorningStart = new GUIContent("清晨 开始");
    static GUIContent TextMorningEnd = new GUIContent("清晨 结束");
    static GUIContent TextDayStart = new GUIContent("白天 开始");
    static GUIContent TextDayEnd = new GUIContent("白天 结束");
    static GUIContent TextDuskStart = new GUIContent("黄昏 开始");
    static GUIContent TextDuskEnd = new GUIContent("黄昏 结束");
    static GUIContent TextNightStart = new GUIContent("黑夜 开始");

    SerializedProperty FogColorGradient;
    SerializedProperty FogLightColorGradient;
    SerializedProperty LightmapColorGradient;
    SerializedProperty SunColorGradient;
    SerializedProperty SkyEnvColorGradient;
    SerializedProperty EquatorEnvColorGradient;
    SerializedProperty GroundEnvColorGradient;
    SerializedProperty GrassColorGradient;
    SerializedProperty CloudyColorGradient;
    SerializedProperty CloudySkyGradient;
    static GUIContent TextFogColorGradient = new GUIContent("雾色");
    static GUIContent TextFogLightColorGradient = new GUIContent("雾光色");
    static GUIContent TextLightmapColorGradient = new GUIContent("LM颜色");
    static GUIContent TextSunColorGradient = new GUIContent("主光颜色");
    static GUIContent TextSkyEnvColorGradient = new GUIContent("朝上环境色");
    static GUIContent TextEquatorEnvColorGradient = new GUIContent("侧向环境色");
    static GUIContent TextGroundEnvColorGradient = new GUIContent("朝下环境色");
    static GUIContent TextGrassColorGradient = new GUIContent("草色");
    static GUIContent TextCloudyColorGradient = new GUIContent("阴天光影颜色");
    static GUIContent TextCloudySkyGradient = new GUIContent("阴天天空盒颜色");

    SerializedProperty LightmapForceCurve;
    SerializedProperty GlobalEmissionLevelCurve;
    SerializedProperty FogDensityCurve;
    SerializedProperty FogHeightFallOutCurve;
    SerializedProperty FogStartDistanceCurve;
    SerializedProperty FogHeightFallOutPosCurve;
    SerializedProperty Sky_FalloutModifCurve;
    SerializedProperty Sky_FalloutModifHeightCurve;
    SerializedProperty VerticalFogDensityCurve;
    SerializedProperty VerticalFogHeightCurve;
    SerializedProperty SunIntensityCurve;
    SerializedProperty CloudySaturationCurve;
    SerializedProperty WaterFallSaturationCurve;
    SerializedProperty WaterFallStrengthCurve;


    static GUIContent TextLightmapForceCurve = new GUIContent("LM强度");
    static GUIContent TextGlobalEmissionLevelCurve = new GUIContent("全局自发光强度");
    static GUIContent TextFogDensityCurve = new GUIContent("雾浓度");
    static GUIContent TextFogHeightFallOutCurve = new GUIContent("雾高度消散强度");
    static GUIContent TextFogStartDistanceCurve = new GUIContent("雾起始距离");
    static GUIContent TextFogHeightFallOutPosCurve = new GUIContent("雾高度消散位置");
    static GUIContent TextSky_FalloutModifCurve = new GUIContent("天空高度消散调整");
    static GUIContent TextSky_FalloutModifHeightCurve = new GUIContent("天空高度消散位置调整");
    static GUIContent TextVerticalFogDensityCurve = new GUIContent("高度雾强度");
    static GUIContent TextVerticalFogHeightCurve = new GUIContent("高度雾高度(距离相机)");
    static GUIContent TextSunIntensityCurve = new GUIContent("主光强度");
    static GUIContent TextCloudySaturationCurve = new GUIContent("阴天饱和");
    static GUIContent TextWaterFallSaturationCurve = new GUIContent("瀑布受灯光的饱和度");
    static GUIContent TextWaterFallStrengthCurve = new GUIContent("瀑布受灯光的权重");


    SerializedProperty SkyBoxLerp;
    static GUIContent TextSkyBoxLerp = new GUIContent("切换天空盒(目前非24小时)");

    private void OnEnable()
    {
        DayTime = serializedObject.FindProperty("DayTime");
        //AutoSave = serializedObject.FindProperty("AutoSave");

        LightmapColor = serializedObject.FindProperty("LightmapColor");
        LightmapForce = serializedObject.FindProperty("LightmapForce");
        GlobalEmissionLevel = serializedObject.FindProperty("GlobalEmissionLevel");
        InDoor = serializedObject.FindProperty("InDoor");

        OpenFog = serializedObject.FindProperty("OpenFog");
        FogColor = serializedObject.FindProperty("FogColor");
        FogLightColor = serializedObject.FindProperty("FogLightColor");
        FogDensity = serializedObject.FindProperty("FogDensity");
        FogStartDistance = serializedObject.FindProperty("FogStartDistance");
        FogHeightFallOut = serializedObject.FindProperty("FogHeightFallOut");
        FogHeightFallOutPos = serializedObject.FindProperty("FogHeightFallOutPos");
        Sky_FalloutModif = serializedObject.FindProperty("Sky_FalloutModif");
        Sky_FalloutModifHeight = serializedObject.FindProperty("Sky_FalloutModifHeight");
        VerticalFogDensity = serializedObject.FindProperty("VerticalFogDensity");
        VerticalFogHeight = serializedObject.FindProperty("VerticalFogHeight");

        CharaLight = serializedObject.FindProperty("CharaLight");
        CharaDark = serializedObject.FindProperty("CharaDark");
        CharaSkinDark = serializedObject.FindProperty("CharaSkinDark");
        //UICharaLight = serializedObject.FindProperty("UICharaLight");
        //UICharaDark = serializedObject.FindProperty("UICharaDark");
        //UICharaSkinDark = serializedObject.FindProperty("UICharaSkinDark");
        IsUIChara = serializedObject.FindProperty("IsUIChara");
        CharaLightGradient = serializedObject.FindProperty("CharaLightGradient");
        CharaDarkGradient = serializedObject.FindProperty("CharaDarkGradient");
        CharaSkinDarkGradient = serializedObject.FindProperty("CharaSkinDarkGradient");

        WindForce = serializedObject.FindProperty("WindForce");
        WindSpeed = serializedObject.FindProperty("WindSpeed");
        GrassColor = serializedObject.FindProperty("GrassColor");

        CloudySaturation = serializedObject.FindProperty("CloudySaturation");
        CloudyColor = serializedObject.FindProperty("CloudyColor");
        CloudySky = serializedObject.FindProperty("CloudySky");
        CloudySwitch = serializedObject.FindProperty("CloudySwitch");
        CloudyLevel = serializedObject.FindProperty("CloudyLevel");

        Is24Hours = serializedObject.FindProperty("Is24Hours");
        TimeOf24 = serializedObject.FindProperty("TimeOf24");
        Test24 = serializedObject.FindProperty("Test24");
        DaySkyBox = serializedObject.FindProperty("DaySkyBox");
        MorningSkyBox = serializedObject.FindProperty("MorningSkyBox");
        DuskSkyBox = serializedObject.FindProperty("DuskSkyBox");
        NightSkyBox = serializedObject.FindProperty("NightSkyBox");

        NightEnd = serializedObject.FindProperty("NightEnd");
        MorningStart = serializedObject.FindProperty("MorningStart");
        MorningEnd = serializedObject.FindProperty("MorningEnd");
        DayStart = serializedObject.FindProperty("DayStart");
        DayEnd = serializedObject.FindProperty("DayEnd");
        DuskStart = serializedObject.FindProperty("DuskStart");
        DuskEnd = serializedObject.FindProperty("DuskEnd");
        NightStart = serializedObject.FindProperty("NightStart");

        FogColorGradient = serializedObject.FindProperty("FogColorGradient");
        FogLightColorGradient = serializedObject.FindProperty("FogLightColorGradient");
        LightmapColorGradient = serializedObject.FindProperty("LightmapColorGradient");
        SunColorGradient = serializedObject.FindProperty("SunColorGradient");
        SkyEnvColorGradient = serializedObject.FindProperty("SkyEnvColorGradient");
        EquatorEnvColorGradient = serializedObject.FindProperty("EquatorEnvColorGradient");
        GroundEnvColorGradient = serializedObject.FindProperty("GroundEnvColorGradient");
        GrassColorGradient = serializedObject.FindProperty("GrassColorGradient");
        CloudySkyGradient = serializedObject.FindProperty("CloudySkyGradient");
        CloudyColorGradient = serializedObject.FindProperty("CloudyColorGradient");

        LightmapForceCurve = serializedObject.FindProperty("LightmapForceCurve");
        GlobalEmissionLevelCurve = serializedObject.FindProperty("GlobalEmissionLevelCurve");
        FogDensityCurve = serializedObject.FindProperty("FogDensityCurve");
        FogStartDistanceCurve = serializedObject.FindProperty("FogStartDistanceCurve");
        FogHeightFallOutCurve = serializedObject.FindProperty("FogHeightFallOutCurve");
        FogHeightFallOutPosCurve = serializedObject.FindProperty("FogHeightFallOutPosCurve");
        Sky_FalloutModifCurve = serializedObject.FindProperty("Sky_FalloutModifCurve");
        Sky_FalloutModifHeightCurve = serializedObject.FindProperty("Sky_FalloutModifHeightCurve");
        VerticalFogDensityCurve = serializedObject.FindProperty("VerticalFogDensityCurve");
        VerticalFogHeightCurve = serializedObject.FindProperty("VerticalFogHeightCurve");
        SunIntensityCurve = serializedObject.FindProperty("SunIntensityCurve");
        CloudySaturationCurve = serializedObject.FindProperty("CloudySaturationCurve");
        WaterFallSaturationCurve = serializedObject.FindProperty("waterFallCurveSaturation");
        WaterFallStrengthCurve = serializedObject.FindProperty("waterFallCurveStrength");

        SkyBoxLerp = serializedObject.FindProperty("SkyBoxLerp");
    }

    public static void ShowTitle(string title, int fontSize, Color col)
    {
        int size = GUI.skin.label.fontSize;
        var color = GUI.skin.label.normal.textColor;
        GUI.skin.label.fontSize = fontSize;
        GUI.skin.label.normal.textColor = col;

        GUILayout.Label(title);
        GUI.skin.label.fontSize = size;
        GUI.skin.label.normal.textColor = color;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();



        var rc = (RenderingController)target;
        serializedObject.Update();

        if (rc.Is24Hours)
        {
            ShowTitle("    自动保存已关闭", 18, Color.red);
        }
        else if (rc.CloudySwitch)
        {
            ShowTitle("    部分自动保存(天气)", 18, Color.yellow);
        }
        else
        {
            ShowTitle("    自动保存已打开", 18, Color.green);
        }

        EditorGUILayout.Space();
        if (!rc.Is24Hours)
        {
            EditorGUILayout.PropertyField(DayTime, TextDayTime);
        }

        //EditorGUILayout.PropertyField(AutoSave, TextAutoSave);

        //if (!rc.AutoSave)
        //{
        //    if (GUILayout.Button(TextSaveTimePoint))
        //    {
        //        rc.SaveTimePoint();
        //    }
        //}

        EditorGUILayout.Space();

        ShowLightmapSetting = EditorGUILayout.Foldout(ShowLightmapSetting, "光效");
        if (ShowLightmapSetting)
        {
            EditorGUILayout.PropertyField(LightmapColor, TextLightmapColor);
            EditorGUILayout.PropertyField(LightmapForce, TextLightmapForce);
            EditorGUILayout.PropertyField(GlobalEmissionLevel, TextGlobalEmissionLevel);
            EditorGUILayout.PropertyField(SkyBoxLerp, TextSkyBoxLerp);
            if (!Application.isPlaying)
            {
                EditorGUILayout.PropertyField(InDoor, TextInDoor);
            }
        }

        EditorGUILayout.Space();
        if (rc.OpenFog)
        {
            ShowFogRegion = EditorGUILayout.Foldout(ShowFogRegion, "雾效");
            if (ShowFogRegion)
            {
                EditorGUILayout.PropertyField(OpenFog, TextOpenFog);
                EditorGUILayout.PropertyField(FogColor, TextFogColor);
                EditorGUILayout.PropertyField(FogLightColor, TextFogLightColor);
                EditorGUILayout.PropertyField(FogDensity, TextFogDensity);
                EditorGUILayout.PropertyField(FogStartDistance, TextFogStartDistance);
                EditorGUILayout.PropertyField(FogHeightFallOut, TextFogHeightFallOut);
                EditorGUILayout.PropertyField(FogHeightFallOutPos, TextFogHeightFallOutPos);
                EditorGUILayout.PropertyField(Sky_FalloutModif, TextSky_FalloutModif);
                EditorGUILayout.PropertyField(Sky_FalloutModifHeight, TextSky_FalloutModifHeight);
                EditorGUILayout.PropertyField(VerticalFogDensity, TextVerticalFogDensity);
                EditorGUILayout.PropertyField(VerticalFogHeight, TextVerticalFogHeight);
            }
        }
        else
        {
            EditorGUILayout.PropertyField(OpenFog, TextOpenFog);
        }

        EditorGUILayout.Space();
        ShowCharactorSetting = EditorGUILayout.Foldout(ShowCharactorSetting, "角色");
        if (ShowCharactorSetting)
        {
            EditorGUILayout.PropertyField(IsUIChara, TextIsUIChara);
            if (!rc.IsUIChara)
            {
                EditorGUILayout.PropertyField(CharaLight, TextCharaLight);
                EditorGUILayout.PropertyField(CharaDark, TextCharaDark);
                EditorGUILayout.PropertyField(CharaSkinDark, TextCharaSkinDark);
            }
            //else
            //{
            //    EditorGUILayout.PropertyField(UICharaLight, TextCharaLight);
            //    EditorGUILayout.PropertyField(UICharaDark, TextCharaDark);
            //    EditorGUILayout.PropertyField(UICharaSkinDark, TextCharaSkinDark);
            //}
        }

        EditorGUILayout.Space();
        ShowWindRegion = EditorGUILayout.Foldout(ShowWindRegion, "风与草");
        if(ShowWindRegion)
        {
            EditorGUILayout.PropertyField(WindForce, TextWindForce);
            EditorGUILayout.PropertyField(WindSpeed, TextWindSpeed);
            EditorGUILayout.PropertyField(GrassColor, TextGrassColor);
        }

        EditorGUILayout.Space();
        ShowRainRegion = EditorGUILayout.Foldout(ShowRainRegion, "天气");        
        if (ShowRainRegion)
        {
            EditorGUILayout.PropertyField(CloudySwitch, TextCloudySwitch);
            if (rc.CloudySwitch)
            {
                EditorGUILayout.PropertyField(CloudyLevel, TextCloudyLevel);
                EditorGUILayout.PropertyField(CloudySaturation, TextCloudySaturation);
                EditorGUILayout.PropertyField(CloudyColor, TextCloudyColor);
                EditorGUILayout.PropertyField(CloudySky, TextCloudySky);
            }
        }


        EditorGUILayout.Space();
        if (rc.Is24Hours)
        {
            Show24HoursRegion = EditorGUILayout.Foldout(Show24HoursRegion, "24小时系统(不支持HDR颜色)");
            if (Show24HoursRegion)
            {
                EditorGUILayout.PropertyField(Is24Hours, TextIs24Hours);
                EditorGUILayout.PropertyField(Test24, TextTest24);
                EditorGUILayout.PropertyField(TimeOf24, TextTimeOf24);

                ShowSkyBoxRegion = EditorGUILayout.Foldout(ShowSkyBoxRegion, "  天空盒");
                if (ShowSkyBoxRegion)
                {
                    EditorGUILayout.PropertyField(NightSkyBox);
                    EditorGUILayout.PropertyField(MorningSkyBox);
                    EditorGUILayout.PropertyField(DaySkyBox);
                    EditorGUILayout.PropertyField(DuskSkyBox);
                }

                ShowColorGradients = EditorGUILayout.Foldout(ShowColorGradients, "   颜色梯度");
                if (ShowColorGradients)
                {
                    EditorGUILayout.PropertyField(FogColorGradient, TextFogColorGradient);
                    EditorGUILayout.PropertyField(FogLightColorGradient, TextFogLightColorGradient);
                    EditorGUILayout.PropertyField(LightmapColorGradient, TextLightmapColorGradient);
                    EditorGUILayout.PropertyField(SunColorGradient, TextSunColorGradient);
                    EditorGUILayout.PropertyField(SkyEnvColorGradient, TextSkyEnvColorGradient);
                    EditorGUILayout.PropertyField(EquatorEnvColorGradient, TextEquatorEnvColorGradient);
                    EditorGUILayout.PropertyField(GroundEnvColorGradient, TextGroundEnvColorGradient);
                    EditorGUILayout.PropertyField(CharaLightGradient, TextCharaLightGradient);
                    EditorGUILayout.PropertyField(CharaDarkGradient, TextCharaDarkGradient);
                    EditorGUILayout.PropertyField(CharaSkinDarkGradient, TextCharaSkinDarkGradient);
                    EditorGUILayout.PropertyField(GrassColorGradient, TextGrassColorGradient);
                    EditorGUILayout.PropertyField(CloudyColorGradient, TextCloudyColorGradient);
                    EditorGUILayout.PropertyField(CloudySkyGradient, TextCloudySkyGradient);

                }

                ShowCurves = EditorGUILayout.Foldout(ShowCurves, "   数值曲线");
                if (ShowCurves)
                {
                    EditorGUILayout.PropertyField(LightmapForceCurve, TextLightmapForceCurve);
                    EditorGUILayout.PropertyField(GlobalEmissionLevelCurve, TextGlobalEmissionLevelCurve);
                    EditorGUILayout.PropertyField(FogDensityCurve, TextFogDensityCurve);
                    EditorGUILayout.PropertyField(FogStartDistanceCurve, TextFogStartDistanceCurve);
                    EditorGUILayout.PropertyField(FogHeightFallOutCurve, TextFogHeightFallOutCurve);
                    EditorGUILayout.PropertyField(FogHeightFallOutPosCurve, TextFogHeightFallOutPosCurve);
                    EditorGUILayout.PropertyField(Sky_FalloutModifCurve, TextSky_FalloutModifCurve);
                    EditorGUILayout.PropertyField(Sky_FalloutModifHeightCurve, TextSky_FalloutModifHeightCurve);
                    EditorGUILayout.PropertyField(VerticalFogDensityCurve, TextVerticalFogDensityCurve);
                    EditorGUILayout.PropertyField(VerticalFogHeightCurve, TextVerticalFogHeightCurve);
                    EditorGUILayout.PropertyField(SunIntensityCurve, TextSunIntensityCurve);
                    EditorGUILayout.PropertyField(CloudySaturationCurve, TextCloudySaturationCurve);
                    EditorGUILayout.PropertyField(WaterFallSaturationCurve, TextWaterFallSaturationCurve);
                    EditorGUILayout.PropertyField(WaterFallStrengthCurve, TextWaterFallStrengthCurve);
                }

                ShowTimePoints = EditorGUILayout.Foldout(ShowTimePoints, "   时间划分");
                if (ShowTimePoints)
                {
                    EditorGUILayout.PropertyField(NightEnd, TextNightEnd);
                    EditorGUILayout.PropertyField(MorningStart, TextMorningStart);
                    EditorGUILayout.PropertyField(MorningEnd, TextMorningEnd);
                    EditorGUILayout.PropertyField(DayStart, TextDayStart);
                    EditorGUILayout.PropertyField(DayEnd, TextDayEnd);
                    EditorGUILayout.PropertyField(DuskStart, TextDuskStart);
                    EditorGUILayout.PropertyField(DuskEnd, TextDuskEnd);
                    EditorGUILayout.PropertyField(NightStart, TextNightStart);
                }
            }
        }
        else
        {
            EditorGUILayout.PropertyField(Is24Hours, TextIs24Hours);
            ShowTimePoints = false;
            ShowColorGradients = false;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
