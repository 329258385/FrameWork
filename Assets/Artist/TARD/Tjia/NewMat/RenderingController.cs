using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeMixer;

[ExecuteInEditMode]
public partial class RenderingController : VMValue
{
    //Add By Zzc LV01 WaterFallCurve according to 24H
    private int waterFallShaderParamsId1 = Shader.PropertyToID("_WaterFallSkyColorSaturation");
    private int waterFallShaderParamsId2 = Shader.PropertyToID("_WaterFallSkyColorStrength");
    public AnimationCurve waterFallCurveSaturation=new AnimationCurve();
    public AnimationCurve waterFallCurveStrength=new AnimationCurve();


    public Texture2D CharaLightModel;

    [SerializeField] DayInterval DayTime = DayInterval.Day;
    private DayInterval mDayTimeBuffer = DayInterval.Day;

    public Cubemap DaySkyBox;
    public Cubemap MorningSkyBox;
    public Cubemap DuskSkyBox;
    public Cubemap NightSkyBox;
    private Cubemap mCube1;
    private Cubemap mCube2;
    private float mSkyBox24 = 0;


    private bool AutoSave = true;

    public bool Is24Hours = false;
    [Range(0, 24)] public float TimeOf24 = 0;
    public bool Test24 = false;

    [ColorUsage(true, true)] public Color LightmapColor = Color.white;

    [Range(0, 10)] public float LightmapForce = 2;
    [Range(0, 2)] public float GlobalEmissionLevel = 1;
    [Range(0, 1)] public float SkyBoxLerp = 0;
    public bool InDoor = false;

    public bool OpenFog = true;
    [ColorUsage(true, true)] public Color FogColor = Color.white;
    [ColorUsage(true, true)] public Color FogLightColor = Color.white;

    public float FogDensity = 0.5f;
    public float FogStartDistance = 10;
    public float FogHeightFallOut = 0;
    public float FogHeightFallOutPos = 0;
    public float Sky_FalloutModif = 0;
    public float Sky_FalloutModifHeight = 0;
    public float VerticalFogDensity = 0;
    public float VerticalFogHeight = 0;


    [ColorUsage(false, true)] public Color CharaLight = Color.white;
    [ColorUsage(false, true)] public Color CharaDark = Color.gray;
    [ColorUsage(false, true)] public Color CharaSkinDark = new Color(1, 0.5f, 0.5f, 1);

    public bool IsUIChara = false;

    /*[ColorUsage(false, true)]*/
    private Color UICharaLight = new Color(1.0f, 0.9258f, 0.82f, 1f);                //王健宁设置的UI界面默认光照颜色
    /*[ColorUsage(false, true)]*/
    private Color UICharaDark = new Color(0.09f, 0.156f, 0.45f, 1f);                //王健宁设置的UI界面默认阴影颜色
                                                                                    /*[ColorUsage(false, true)]*/
    private Color UICharaSkinDark = new Color(0.75f, 0.4f, 0.34f, 1f);          //王健宁设置的UI界面默认皮肤阴影颜色

    [Range(0, 10)] public float WindForce = 1;
    [Range(0, 10)] public float WindSpeed = 1;
    [ColorUsage(false, true)] public Color GrassColor = Color.gray;

    internal Color EquatorColorMemo;
    internal Color SkyColorMemo;
    internal Color GroundColorMemo;
    internal bool Switch = false;
    internal Color SunColorMemo;
    internal float SunIntensityMemo;

    [Range(0, 24)] public float NightEnd = 4.0f;
    [Range(0, 24)] public float MorningStart = 5.0f;
    [Range(0, 24)] public float MorningEnd = 8.0f;
    [Range(0, 24)] public float DayStart = 9.0f;
    [Range(0, 24)] public float DayEnd = 16.0f;
    [Range(0, 24)] public float DuskStart = 17.0f;
    [Range(0, 24)] public float DuskEnd = 19.0f;
    [Range(0, 24)] public float NightStart = 20.0f;

    public Gradient FogColorGradient = new Gradient();
    public Gradient FogLightColorGradient = new Gradient();
    public Gradient LightmapColorGradient = new Gradient();
    public Gradient SunColorGradient = new Gradient();
    public Gradient SkyEnvColorGradient = new Gradient();
    public Gradient EquatorEnvColorGradient = new Gradient();
    public Gradient GroundEnvColorGradient = new Gradient();

    public AnimationCurve LightmapForceCurve = new AnimationCurve();
    public AnimationCurve GlobalEmissionLevelCurve = new AnimationCurve();
    public AnimationCurve FogDensityCurve = new AnimationCurve();
    public AnimationCurve FogHeightFallOutCurve = new AnimationCurve();
    public AnimationCurve FogStartDistanceCurve = new AnimationCurve();
    public AnimationCurve FogHeightFallOutPosCurve = new AnimationCurve();
    public AnimationCurve Sky_FalloutModifCurve = new AnimationCurve();
    public AnimationCurve Sky_FalloutModifHeightCurve = new AnimationCurve();
    public AnimationCurve VerticalFogDensityCurve = new AnimationCurve();
    public AnimationCurve VerticalFogHeightCurve = new AnimationCurve();
    public AnimationCurve SunIntensityCurve = new AnimationCurve();
    public AnimationCurve SkyBox24Curve = new AnimationCurve();
    public AnimationCurve CloudySaturationCurve = new AnimationCurve();

    public Gradient CharaLightGradient = new Gradient();
    public Gradient CharaDarkGradient = new Gradient();
    public Gradient CharaSkinDarkGradient = new Gradient();
    public Gradient GrassColorGradient = new Gradient();
    public Gradient CloudyColorGradient = new Gradient();
    public Gradient CloudySkyGradient = new Gradient();

    private float bTimeOf24;
    private Color bFogColor;
    private Color bFogLightColor;
    private float bFogStartDistance;
    private bool bOpenFog;
    private float bFogDensity;
    private float bFogHeightFallOut;
    private float bFogHeightFallOutPos;
    public float bSky_FalloutModif;
    public float bSky_FalloutModifHeight;
    private float bVerticalFogDensity;
    public float bVerticalFogHeight;
    private Color bLightmapColor;
    private float bLightmapForce;
    private float bGlobalEmissionLevel;
    private Color bCharaLight;
    private Color bCharaDark;
    private Color bCharaSkinDark;
    private Color bUICharaLight;
    private Color bUICharaDark;
    private Color bUICharaSkinDark;
    private bool bIsUIChara;
    private float bSkyBoxLerp;
    private Vector3 bSunAngle;
    private float bWindForce;
    private float bWindSpeed;
    private static bool bInDoor;
    private Color bGrassColor;
    private Color bCloudyColor;
    private Color bCloudySky;
    private float bCloudySaturation;
    private bool bIs24Hours;

    public float SunIntensityForMixer;
    public Color SunColorForMixer;
    public Color SkyColorForMixer;
    public Color EquatorColorForMixer;
    public Color GroundColorForMixer;

    private bool ReadOneTime = false;

    private float mSaveTimer = 0;

    public enum DayInterval
    {
        Night,
        Morning,
        Day,
        Dusk
    }

    MeshRenderer[] mrs;
    ShadowCastingMode[] mrShadows;

    public bool CloudySwitch = false;
    private bool bCloudySwitch = false;
    [Range(0, 1)] public float CloudyLevel = 0.0f;
    private float bCloudyLevel = -1;
    [Range(0, 1)]public float CloudySaturation = 1.0f;
    [ColorUsage(false)] public Color CloudyColor = new Color(0.25f, 0.25f, 0.5f);
    [ColorUsage(false)] public Color CloudySky = new Color(1f, 0f,0f);

    internal RainSky mRainSky;

    private void Start()
    {
        Test24 = false;
        mDayTimeBuffer = DayInterval.Night;
        if (DayTime == DayInterval.Night)
        {
            mDayTimeBuffer = DayInterval.Day;
        }
        bTimeOf24 = -1;

        Shader.SetGlobalFloat("_NoLightmap", 0);
        if (InDoor)
        {
            Shader.SetGlobalFloat("_SunOutDoor", 0);
        }
        else
        {
            Shader.SetGlobalFloat("_SunOutDoor", 1);
        }
    }

    private void OnEnable()
    {
        ReadOneTime = true;
        AutoSave = true;
#if !BUILD_SINGLESCNE_MODE
        foreach (RenderingController rc in FindObjectsOfType<RenderingController>())
        {
            if (rc != this)
            {
                rc.enabled = false;
            }
        }
#endif
        if (InDoor)
        {
            Shader.SetGlobalFloat("_SunOutDoor", 0);
        }
        else
        {
            Shader.SetGlobalFloat("_SunOutDoor", 1);
        }
        RealTimeEvaluate();
        bTimeOf24 = -1;

        if (mRainSky == null)
        {
            mRainSky = FindObjectOfType<RainSky>();
        }

        if(mRainSky == null)
        {
            Shader.SetGlobalVector("_SSR", new Vector3(1, 0, 1));
        }
    }

    private void OnDisable()
    {
        AutoSave = false;
        foreach (RenderingController rc in FindObjectsOfType<RenderingController>())
        {
            if (rc.enabled == true && rc != this)
            {
                rc.ReadOneTime = true;
            }
            //if (rc.enabled == false && rc != this && rc.gameObject.activeSelf)
            //{
            //    rc.enabled = true;
            //}
        }
    }

#if UNITY_EDITOR
    void OnRenderObject()
    {
        BakingHelper();
    }
#endif

    // Update is called once per frame
    void Update()
    {
        BakingHelper();
#if UNITY_EDITOR
        Shader.SetGlobalFloat("_NoLightmap", LightmapSettings.lightmaps.Length == 0 ? 1 : 0);
        if (!Lightmapping.isRunning)
#endif
        {
            //float cloudyTestTime = Time.time;
            //CloudyLevel = Mathf.Clamp(Mathf.Abs(cloudyTestTime - Mathf.Floor(cloudyTestTime) - 0.5f) - 0.25f, -0.0625f, 0.0625f);
            if (Test24)
            {
                //CloudyLevel = Mathf.Clamp(Mathf.Sin(Time.time * Mathf.PI * 0.25f), -0.25f, 0.25f) * 2f + 0.5f;
            }
            if (!CloudySwitch)
            {
                CloudyLevel = 0;
            }
            if (bCloudyLevel != CloudyLevel)
            {
                bCloudyLevel = CloudyLevel;
                Shader.SetGlobalFloat("_CloudyLevel", CloudyLevel);
                if (mRainSky != null)
                {
                    mRainSky.CloudyLevel = CloudyLevel;
                }
                if (Is24Hours)
                {
                    Evaluate24();
                }
                else
                {
                    ReadTimePoint();
                }
            }
            if (bIs24Hours != Is24Hours)
            {
                ReadOneTime = true;
                bIs24Hours = Is24Hours;
                if (mRainSky != null)
                {
                    mRainSky.Is24Hours = Is24Hours;
                }
                bTimeOf24 = -1;
            }
            if (mDayTimeBuffer != DayTime || (!Is24Hours && ReadOneTime))
            {
                switch (DayTime)
                {
                    case DayInterval.Night:
                        mSkyBox24 = 0;
                        break;
                    case DayInterval.Morning:
                        mSkyBox24 = 1;
                        break;
                    case DayInterval.Day:
                        mSkyBox24 = 2;
                        break;
                    case DayInterval.Dusk:
                        mSkyBox24 = 3;
                        break;
                }
                ReadTimePoint();
                ReadOneTime = false;
            }

            if ((Is24Hours && bTimeOf24 != TimeOf24) || Test24 || (Is24Hours && bIsUIChara != IsUIChara))
            {
                if (Test24)
                {

#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                    {
                        Test24 = false;
                    }
                    else
#endif
                    {
                        TimeOf24 = (TimeOf24 + Time.deltaTime / 60f / 60f * 60f * 24f * 2f / 2f) % 24;
                    }
                }
                bIsUIChara = IsUIChara;
                Evaluate24();
                ReadOneTime = true;
            }
            else if(!Is24Hours)
#if !UNITY_EDITOR
            if (bFogColor != FogColor
            || bFogLightColor != FogLightColor
            || bFogStartDistance != FogStartDistance
            || bOpenFog != OpenFog
            || bFogDensity != FogDensity
            || bFogHeightFallOut != FogHeightFallOut
            || bFogHeightFallOutPos != FogHeightFallOutPos
            || bSky_FalloutModif != Sky_FalloutModif
            || bSky_FalloutModifHeight != Sky_FalloutModifHeight
            || bVerticalFogDensity != VerticalFogDensity
            || bVerticalFogHeight != VerticalFogHeight
            || bLightmapColor != LightmapColor
            || bLightmapForce != LightmapForce
            || bGlobalEmissionLevel != GlobalEmissionLevel
            || bCharaLight != CharaLight
            || bCharaDark != CharaDark
            || bCharaSkinDark != CharaSkinDark
            || bUICharaLight != UICharaLight
            || bUICharaDark != UICharaDark
            || bUICharaSkinDark != UICharaSkinDark
            || bIsUIChara != IsUIChara
            || bSkyBoxLerp != SkyBoxLerp
            || bWindForce != WindForce
            || bWindSpeed != WindSpeed
            || bGrassColor != GrassColor
            || bIs24Hours != Is24Hours
            || bCloudyLevel != CloudyLevel
            )
#endif
            {
                RealTimeEvaluate();
            }
            if (bInDoor != InDoor)
            {
                bInDoor = InDoor;
                if (InDoor)
                {
                    ChangeShadowCastingMode(UnityEngine.Rendering.ShadowCastingMode.Off);
                    Shader.SetGlobalFloat("_SunOutDoor", 0);
                }
                else
                {
                    ChangeShadowCastingMode(UnityEngine.Rendering.ShadowCastingMode.On);
                    Shader.SetGlobalFloat("_SunOutDoor", 1);
                }
            }
        }
#if UNITY_EDITOR
        if (Time.time > mSaveTimer + 0.2f
            && AutoSave
            && !Application.isPlaying
            && !Lightmapping.isRunning
            && !Is24Hours)
        {
            mSaveTimer = Time.time;
            if (!CloudySwitch)
            {
                if (bCloudySwitch != CloudySwitch)
                {
                    bCloudySwitch = CloudySwitch;
                    if (Is24Hours)
                    {
                        Evaluate24();
                    }
                    else
                    {
                        ReadTimePoint();
                    }
                }
                else
                {
                    SaveTimePoint();
                }
            }
            else// if()
            {
                bCloudySwitch = CloudySwitch;
                if (Is24Hours)
                {
                    Evaluate24();
                }
                else
                {
                    if (bCloudyColor != CloudyColor || 
                        bCloudySaturation != CloudySaturation || 
                        bCloudySky != CloudySky)
                    {
                        bCloudyColor = CloudyColor;
                        bCloudySaturation = CloudySaturation;
                        bCloudySky = CloudySky;

                        if (mRainSky != null)
                        {
                            Shader.SetGlobalColor("_RainSkyColor", CloudySky);
                        }

                        SaveTo24Sys(CloudyColorGradient, CloudyColor);
                        SaveTo24Sys(CloudySkyGradient, CloudySky);
                        SaveTo24Sys(CloudySaturationCurve, CloudySaturation);
                    }
                    ReadTimePoint();
                }

            }
        }
#endif
    }

    private static void ChangeShadowCastingMode(UnityEngine.Rendering.ShadowCastingMode scm)
    {
        List<Renderer> rds = new List<Renderer>(FindObjectsOfType<MeshRenderer>());
        List<Shader> charaShaders = new List<Shader>();
        charaShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/NewChara/NewCharaMain"));
        charaShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/NewChara/NewCharaMain_WO"));
        charaShaders.Add(UnityUtils.FindShader("SAO_TJia_V3/NewChara/NewCharaTransparent"));
        foreach (SkinnedMeshRenderer smr in FindObjectsOfType<SkinnedMeshRenderer>())
        {
            rds.Add(smr);
        }
        for (int i = 0; i < rds.Count; i++)
        {
            if (!charaShaders.Contains(rds[i].sharedMaterial.shader))
            {
                rds[i].shadowCastingMode = scm;
            }
        }
    }

    private Color RainedColor(Color c, bool isChara = false)
    {
        if (!CloudySwitch)
        {
            return c;
        }
        Color res = c;
        float lumC = res.r * 0.22f + res.g * 0.707f + res.b * 0.071f;
        float rainSaturation = CloudySaturation;
        if (isChara)
        {
            rainSaturation = rainSaturation * 0.5f + 0.5f;
        }
        res.r = Mathf.Lerp(lumC, res.r, rainSaturation);
        res.g = Mathf.Lerp(lumC, res.g, rainSaturation);
        res.b = Mathf.Lerp(lumC, res.b, rainSaturation);
        res *= CloudyColor;

        res = Color.Lerp(c, res, CloudyLevel);

        return res;
    }

    float RainedFog(float v)
    {
        if (!CloudySwitch)
        {
            return v;
        }
        return Mathf.Lerp(v, v * 2f, CloudyLevel);
    }

    private void RealTimeEvaluate()
    {
        bFogColor = FogColor;
        bFogLightColor = FogLightColor;
        bFogStartDistance = FogStartDistance;
        bOpenFog = OpenFog;
        bFogDensity = FogDensity;
        bFogHeightFallOut = FogHeightFallOut;
        bFogHeightFallOutPos = FogHeightFallOutPos;
        bSky_FalloutModif = Sky_FalloutModif;
        bSky_FalloutModifHeight = Sky_FalloutModifHeight;
        bVerticalFogDensity = VerticalFogDensity;
        bVerticalFogHeight = VerticalFogHeight;        
        bLightmapColor = LightmapColor;
        bLightmapForce = LightmapForce;
        bGlobalEmissionLevel = GlobalEmissionLevel;
        bCharaLight = CharaLight;
        bCharaDark = CharaDark;
        bCharaSkinDark = CharaSkinDark;
        bUICharaLight = UICharaLight;
        bUICharaDark = UICharaDark;
        bUICharaSkinDark = UICharaSkinDark;
        bIsUIChara = IsUIChara;
        bSkyBoxLerp = SkyBoxLerp;
        bWindForce = WindForce;
        bWindSpeed = WindSpeed;
        bGrassColor = GrassColor;
        bIs24Hours = Is24Hours;
        bCloudyLevel = CloudyLevel;

        Shader.SetGlobalColor("_FogColor", RainedColor(FogColor));
        Shader.SetGlobalColor("_FogLightColor", RainedColor(FogLightColor));
        Shader.SetGlobalFloat("_FogStartDistance", FogStartDistance);
        Shader.SetGlobalFloat("_FogDensity", OpenFog ? RainedFog(FogDensity) * 6 : 0);
        Shader.SetGlobalFloat("_FogHeightFallOut", FogHeightFallOut * 0.1f);
        Shader.SetGlobalFloat("_FogHeightFallOutPos", FogHeightFallOutPos);
        Shader.SetGlobalFloat("_FalloutModif", Sky_FalloutModif);
        Shader.SetGlobalFloat("_FalloutModifHeight", Sky_FalloutModifHeight);
        Shader.SetGlobalFloat("_VerticalFogDensity", VerticalFogDensity);
        Shader.SetGlobalFloat("_VerticalFogHeight", VerticalFogHeight);

        Shader.SetGlobalColor("_LightmapColor", (LightmapColor));
        Shader.SetGlobalFloat("_LightmapForce", LightmapForce);
        Shader.SetGlobalFloat("_GlobalEmissionLevel", Mathf.Max(GlobalEmissionLevel, 0.3f));

        //Chara
        Shader.SetGlobalTexture("_LightModel", CharaLightModel);
        Shader.EnableKeyword("OBJECT_SPACE_FX");

        float t = SkyBoxLerp;

        LerpSky(t);

        if (!IsUIChara)
        {
            Shader.SetGlobalColor("_CharaLight", RainedColor(CharaLight, true));
            Shader.SetGlobalColor("_CharaDark", RainedColor(CharaDark, true));
            Shader.SetGlobalColor("_CharaSkinDark", RainedColor(CharaSkinDark, true));
        }
        else
        {
            Shader.SetGlobalColor("_CharaLight", UICharaLight);
            Shader.SetGlobalColor("_CharaDark", UICharaDark);
            Shader.SetGlobalColor("_CharaSkinDark", UICharaSkinDark);
        }

        Shader.SetGlobalColor("_GrassColor", (GrassColor));

        CharaSunSetter();
        Shader.SetGlobalFloat("_GWindForce", WindForce);
        Shader.SetGlobalFloat("_GWindSpeed", WindSpeed);

        Shader.SetGlobalFloat("_CloudyLevel", CloudyLevel);
    }

    private void CharaSunSetter()
    {
        if (TestSun() && bSunAngle != RenderSettings.sun.transform.eulerAngles)
        {
            bSunAngle = RenderSettings.sun.transform.eulerAngles;
            Vector3 sunAngle = RenderSettings.sun.transform.eulerAngles;
            Vector3 sunAngleMemo = sunAngle;
            sunAngle.x = 15;
            RenderSettings.sun.transform.eulerAngles = sunAngle;
            Shader.SetGlobalVector("_MainLightPos", RenderSettings.sun.transform.forward);
            sunAngle.y -= 90;
            RenderSettings.sun.transform.eulerAngles = sunAngle;
            Shader.SetGlobalVector("_BackLightPos", RenderSettings.sun.transform.forward);
            RenderSettings.sun.transform.eulerAngles = sunAngleMemo;
        }
    }

    private void LerpSky(float t)
    {
        Material skyMat = RenderSettings.skybox;
        if (skyMat != null)
        {
            if (t < 0.0001f)
            {
                skyMat.EnableKeyword("_TEXNUMTYPE_ONE");
                skyMat.DisableKeyword("_TEXNUMTYPE_TWO");
                skyMat.DisableKeyword("_TEXNUMTYPE_LERP");
                skyMat.SetFloat("_Texnumtype", 1);
            }
            else if (t > 0.9999f)
            {
                skyMat.DisableKeyword("_TEXNUMTYPE_ONE");
                skyMat.EnableKeyword("_TEXNUMTYPE_TWO");
                skyMat.DisableKeyword("_TEXNUMTYPE_LERP");
                skyMat.SetFloat("_Texnumtype", 2);
            }
            else
            {
                skyMat.DisableKeyword("_TEXNUMTYPE_ONE");
                skyMat.DisableKeyword("_TEXNUMTYPE_TWO");
                skyMat.EnableKeyword("_TEXNUMTYPE_LERP");
                skyMat.SetFloat("_TexLerp", t);
                skyMat.SetFloat("_Texnumtype", 0);
            }
        }
    }

    private void Evaluate24()
    {
        bTimeOf24 = TimeOf24;
        float time = TimeOf24 / 24f;

        Shader.SetGlobalColor("_FogColor", RainedColor(FogColorGradient.Evaluate(time)));
        Shader.SetGlobalColor("_FogLightColor", RainedColor(FogLightColorGradient.Evaluate(time)));
        Shader.SetGlobalFloat("_FogDensity", RainedFog(FogDensityCurve.Evaluate(time)) * 6);
        Shader.SetGlobalFloat("_FogHeightFallOut", FogHeightFallOutCurve.Evaluate(time) * 0.1f);
        Shader.SetGlobalFloat("_FogStartDistance", FogStartDistanceCurve.Evaluate(time));
        Shader.SetGlobalFloat("_FogHeightFallOutPos", FogHeightFallOutPosCurve.Evaluate(time));
        Shader.SetGlobalFloat("_FalloutModif", Sky_FalloutModifCurve.Evaluate(time));
        Shader.SetGlobalFloat("_FalloutModifHeight", Sky_FalloutModifHeightCurve.Evaluate(time));
        
        Shader.SetGlobalFloat("_VerticalFogDensity", VerticalFogDensityCurve.Evaluate(time));
        Shader.SetGlobalFloat("_VerticalFogHeight", VerticalFogHeightCurve.Evaluate(time));

        Shader.SetGlobalFloat("_LightmapForce", LightmapForceCurve.Evaluate(time));
        Shader.SetGlobalFloat("_GlobalEmissionLevel", Mathf.Max(GlobalEmissionLevelCurve.Evaluate(time), 0.3f));
        Shader.SetGlobalColor("_LightmapColor", (LightmapColorGradient.Evaluate(time)));

        if (!IsUIChara)
        {
            Shader.SetGlobalColor("_CharaLight", RainedColor(CharaLightGradient.Evaluate(time), true));
            Shader.SetGlobalColor("_CharaDark", RainedColor(CharaDarkGradient.Evaluate(time), true));
            Shader.SetGlobalColor("_CharaSkinDark", RainedColor(CharaSkinDarkGradient.Evaluate(time), true));
        }
        else
        {
            Shader.SetGlobalColor("_CharaLight", UICharaLight);
            Shader.SetGlobalColor("_CharaDark", UICharaDark);
            Shader.SetGlobalColor("_CharaSkinDark", UICharaSkinDark);
        }
        Shader.SetGlobalColor("_GrassColor", (GrassColorGradient.Evaluate(time)));

        CloudyColor = CloudyColorGradient.Evaluate(time);
        CloudySaturation = CloudySaturationCurve.Evaluate(time);
        if (mRainSky != null)
        {
            CloudySky = CloudySkyGradient.Evaluate(time);
            Shader.SetGlobalColor("_RainSkyColor", CloudySky);
        }

        Material skyMat = RenderSettings.skybox;
        if (skyMat != null)
        {
            float timeSkyBox24 = SkyBox24Curve.Evaluate(time);
            if (time < DuskStart / 24f && timeSkyBox24 < 3)
            {
                if (timeSkyBox24 < 1)
                {
                    skyMat.SetTexture("_Tex", NightSkyBox);
                    skyMat.SetTexture("_Tex2", MorningSkyBox);
                    LerpSky(timeSkyBox24);
                }
                else if (timeSkyBox24 < 2)
                {
                    skyMat.SetTexture("_Tex", MorningSkyBox);
                    skyMat.SetTexture("_Tex2", DaySkyBox);
                    LerpSky(timeSkyBox24 - 1);
                }
                else if (timeSkyBox24 < 3)
                {
                    skyMat.SetTexture("_Tex", DaySkyBox);
                    skyMat.SetTexture("_Tex2", DuskSkyBox);
                    LerpSky(timeSkyBox24 - 2);
                }
            }
            else
            {
                skyMat.SetTexture("_Tex", NightSkyBox);
                skyMat.SetTexture("_Tex2", DuskSkyBox);
                LerpSky(timeSkyBox24 / 3);
            }

            CharaSunSetter();
        }

        if (TestSun())
        {
            SunColorForMixer = RenderSettings.sun.color = RainedColor(SunColorGradient.Evaluate(time));
            SunIntensityForMixer = RenderSettings.sun.intensity = SunIntensityCurve.Evaluate(time);
        }

        SkyColorForMixer = RenderSettings.ambientSkyColor = RainedColor(SkyEnvColorGradient.Evaluate(time));
        EquatorColorForMixer = RenderSettings.ambientEquatorColor = RainedColor(EquatorEnvColorGradient.Evaluate(time));
        GroundColorForMixer = RenderSettings.ambientGroundColor = RainedColor(GroundEnvColorGradient.Evaluate(time));

        Shader.SetGlobalFloat("_CloudyLevel", CloudyLevel);


        //Add By Zzc LV01 WaterFallCurve according to 24H
        Shader.SetGlobalFloat(waterFallShaderParamsId1, waterFallCurveSaturation.Evaluate(time));
        Shader.SetGlobalFloat(waterFallShaderParamsId2, waterFallCurveStrength.Evaluate(time));
    }

    private void ReadTimePoint()
    {
        Material skyMat = RenderSettings.skybox;

        if (skyMat != null && !Is24Hours)
        {
            switch (DayTime)
            {
                case DayInterval.Night:
                    mCube1 = NightSkyBox;
                    break;
                case DayInterval.Morning:
                    mCube1 = MorningSkyBox;
                    break;
                case DayInterval.Day:
                    mCube1 = DaySkyBox;
                    break;
                case DayInterval.Dusk:
                    mCube1 = DuskSkyBox;
                    break;
            }
        }
        if (mCube1 != null)
        {
            skyMat.SetTexture("_Tex", mCube1);
        }
        if (mCube2 != null)
        {
            skyMat.SetTexture("_Tex2", mCube2);
        }

        mDayTimeBuffer = DayTime;

        FogColor = ReadTimePoint(FogColorGradient, FogColor);
        FogLightColor = ReadTimePoint(FogLightColorGradient, FogLightColor);
        FogDensity = ReadTimePoint(FogDensityCurve, FogDensity);
        FogHeightFallOut = ReadTimePoint(FogHeightFallOutCurve, FogHeightFallOut);
        FogStartDistance = ReadTimePoint(FogStartDistanceCurve, FogStartDistance);
        FogHeightFallOutPos = ReadTimePoint(FogHeightFallOutPosCurve, FogHeightFallOutPos);
        Sky_FalloutModif = ReadTimePoint(Sky_FalloutModifCurve, Sky_FalloutModif);
        Sky_FalloutModifHeight = ReadTimePoint(Sky_FalloutModifHeightCurve, Sky_FalloutModifHeight);
        VerticalFogDensity = ReadTimePoint(VerticalFogDensityCurve, VerticalFogDensity);
        VerticalFogHeight = ReadTimePoint(VerticalFogHeightCurve, VerticalFogHeight);

        LightmapForce = ReadTimePoint(LightmapForceCurve, LightmapForce);
        GlobalEmissionLevel = ReadTimePoint(GlobalEmissionLevelCurve, GlobalEmissionLevel);
        LightmapColor = ReadTimePoint(LightmapColorGradient, LightmapColor);

        CharaLight = ReadTimePoint(CharaLightGradient, CharaLight);
        CharaDark = ReadTimePoint(CharaDarkGradient, CharaDark);
        CharaSkinDark = ReadTimePoint(CharaSkinDarkGradient, CharaSkinDark);

        GrassColor = ReadTimePoint(GrassColorGradient, GrassColor);

        CloudyColor = ReadTimePoint(CloudyColorGradient, CloudyColor);
        CloudySaturation = ReadTimePoint(CloudySaturationCurve, CloudySaturation);

        if (mRainSky != null)
        {
            CloudySky = ReadTimePoint(CloudySkyGradient, CloudySky);
            Shader.SetGlobalColor("_RainSkyColor", CloudySky);
        }

        if (TestSun())
        {
            SunColorForMixer = RenderSettings.sun.color = RainedColor(ReadTimePoint(SunColorGradient, RenderSettings.sun.color));
            SunIntensityForMixer = RenderSettings.sun.intensity = ReadTimePoint(SunIntensityCurve, RenderSettings.sun.intensity);
        }

        SkyColorForMixer = RenderSettings.ambientSkyColor = RainedColor(ReadTimePoint(SkyEnvColorGradient, RenderSettings.ambientSkyColor));
        EquatorColorForMixer = RenderSettings.ambientEquatorColor = RainedColor(ReadTimePoint(EquatorEnvColorGradient, RenderSettings.ambientEquatorColor));
        GroundColorForMixer = RenderSettings.ambientGroundColor = RainedColor(ReadTimePoint(GroundEnvColorGradient, RenderSettings.ambientGroundColor));
    }

    private void BakingHelper()
    {

#if UNITY_EDITOR

        Shader.SetGlobalFloat("_NoLightmap", LightmapSettings.lightmaps.Length == 0 ? 1 : 0);

        if (!Application.isPlaying)
        {
            if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Trilight)
            {

                if (Lightmapping.isRunning && !Switch)
                {
                    Switch = true;
                    EquatorColorMemo = RenderSettings.ambientEquatorColor;
                    SkyColorMemo = RenderSettings.ambientSkyColor;
                    GroundColorMemo = RenderSettings.ambientGroundColor;

                    mrs = FindObjectsOfType<MeshRenderer>();
                    mrShadows = new ShadowCastingMode[mrs.Length];

                    for (int i = 0; i < mrs.Length; i++)
                    {
                        if (mrs[i] != null)
                        {
                            mrShadows[i] = mrs[i].shadowCastingMode;
                            if (mrs[i].shadowCastingMode == ShadowCastingMode.Off)
                            {
                                mrs[i].shadowCastingMode = ShadowCastingMode.On;
                            }
                        }
                        else
                        {
                            mrShadows[i] = ShadowCastingMode.Off;
                        }
                    }

                    if (TestSun())
                    {
                        SunColorMemo = RenderSettings.sun.color;
                        SunIntensityMemo = RenderSettings.sun.intensity;
                        RenderSettings.sun.color = Color.white;
                        RenderSettings.sun.intensity = 1;
                        if (InDoor)
                        {
                            RenderSettings.sun.intensity = 0;
                            RenderSettings.sun.shadows = LightShadows.None;
                        }
                        //RenderSettings.sun.bounceIntensity = 1;
                    }
                    if (InDoor)
                    {
                        ChangeShadowCastingMode(UnityEngine.Rendering.ShadowCastingMode.On);
                        Shader.SetGlobalFloat("_SunOutDoor", 1);
                    }

                    RenderSettings.ambientEquatorColor = Color.black;
                    RenderSettings.ambientSkyColor = Color.black;
                    RenderSettings.ambientGroundColor = Color.black;
                }
                else if (!Lightmapping.isRunning && Switch)
                {
                    Switch = false;
                    EquatorColorForMixer = RenderSettings.ambientEquatorColor = EquatorColorMemo;
                    SkyColorForMixer = RenderSettings.ambientSkyColor = SkyColorMemo;
                    GroundColorForMixer = RenderSettings.ambientGroundColor = GroundColorMemo;

                    for (int i = 0; i < mrs.Length; i++)
                    {
                        if (mrs[i] != null)
                        {
                            mrs[i].shadowCastingMode = mrShadows[i];
                        }
                    }

                    if (TestSun())
                    {
                        SunColorForMixer = RenderSettings.sun.color = SunColorMemo;
                        SunIntensityForMixer = RenderSettings.sun.intensity = SunIntensityMemo;
                        RenderSettings.sun.shadows = LightShadows.Soft;
                    }
                    if (InDoor)
                    {
                        ChangeShadowCastingMode(UnityEngine.Rendering.ShadowCastingMode.Off);
                        Shader.SetGlobalFloat("_SunOutDoor", 0);
                    }
                }
            }
            else if (Lightmapping.isRunning)
            {
                Debug.LogError("错误的环境光模式，请设置为Gradient");
                Lightmapping.ForceStop();
            }
        }
#endif
    }

    private bool TestSun()
    {
        if (RenderSettings.sun != null)
        {
            return true;
        }
        else
        {
            Debug.LogError("请在Lighting中设置主光源，否则无法保存光源颜色");
            return false;
        }
    }

    public void SaveTimePoint()
    {
        Material skyMat = RenderSettings.skybox;

        if (skyMat != null && skyMat.shader == UnityUtils.FindShader("SAO_TJia_V3/TjiaNewSky"))
        {
            if (skyMat.GetTexture("_Tex") && skyMat.GetTexture("_Tex2"))
            {
                mCube1 = (Cubemap)skyMat.GetTexture("_Tex");
                mCube2 = (Cubemap)skyMat.GetTexture("_Tex2");
                switch (DayTime)
                {
                    case DayInterval.Night:
                        NightSkyBox = mCube1;
                        break;
                    case DayInterval.Morning:
                        MorningSkyBox = mCube1;
                        break;
                    case DayInterval.Day:
                        DaySkyBox = mCube1;
                        break;
                    case DayInterval.Dusk:
                        DuskSkyBox = mCube1;
                        break;
                }
            }
        }

        SaveTo24Sys(FogColorGradient, FogColor);
        SaveTo24Sys(FogLightColorGradient, FogLightColor);
        SaveTo24Sys(FogDensityCurve, FogDensity);
        SaveTo24Sys(FogHeightFallOutCurve, FogHeightFallOut);
        SaveTo24Sys(FogStartDistanceCurve, FogStartDistance);
        SaveTo24Sys(FogHeightFallOutPosCurve, FogHeightFallOutPos);
        SaveTo24Sys(Sky_FalloutModifCurve, Sky_FalloutModif);
        SaveTo24Sys(Sky_FalloutModifHeightCurve, Sky_FalloutModifHeight);
        SaveTo24Sys(VerticalFogHeightCurve, VerticalFogHeight);
        SaveTo24Sys(VerticalFogDensityCurve, VerticalFogDensity);

        SaveTo24Sys(SkyBox24Curve, mSkyBox24);

        SaveTo24Sys(LightmapForceCurve, LightmapForce);
        SaveTo24Sys(GlobalEmissionLevelCurve, GlobalEmissionLevel);
        SaveTo24Sys(LightmapColorGradient, LightmapColor);

        SaveTo24Sys(CharaLightGradient, CharaLight);
        SaveTo24Sys(CharaDarkGradient, CharaDark);
        SaveTo24Sys(CharaSkinDarkGradient, CharaSkinDark);

        SaveTo24Sys(GrassColorGradient, GrassColor);

        if (TestSun())
        {
            SaveTo24Sys(SunColorGradient, RenderSettings.sun.color);
            SaveTo24Sys(SunIntensityCurve, RenderSettings.sun.intensity);
        }

        SaveTo24Sys(SkyEnvColorGradient, RenderSettings.ambientSkyColor);
        SaveTo24Sys(EquatorEnvColorGradient, RenderSettings.ambientEquatorColor);
        SaveTo24Sys(GroundEnvColorGradient, RenderSettings.ambientGroundColor);
    }

    private void SaveTo24Sys(AnimationCurve curve, float value)
    {
        if (curve.keys.Length != 8)
        {
            Keyframe[] keyFrames = new Keyframe[8];

            keyFrames[0].time = NightEnd / 24f;
            keyFrames[1].time = MorningStart / 24f;
            keyFrames[2].time = MorningEnd / 24f;
            keyFrames[3].time = DayStart / 24f;
            keyFrames[4].time = DayEnd / 24f;
            keyFrames[5].time = DuskStart / 24f;
            keyFrames[6].time = DuskEnd / 24f;
            keyFrames[7].time = NightStart / 24f;
            for (int i = 0; i < 8; i++)
            {
                keyFrames[i].value = 0.5f;
            }

            curve.keys = keyFrames;
        }
        if (curve.keys.Length == 8)
        {
            Keyframe[] keyFrames = curve.keys;

            keyFrames[0].time = NightEnd / 24f;
            keyFrames[1].time = MorningStart / 24f;
            keyFrames[2].time = MorningEnd / 24f;
            keyFrames[3].time = DayStart / 24f;
            keyFrames[4].time = DayEnd / 24f;
            keyFrames[5].time = DuskStart / 24f;
            keyFrames[6].time = DuskEnd / 24f;
            keyFrames[7].time = NightStart / 24f;

            switch (DayTime)
            {
                case DayInterval.Day:
                    keyFrames[3].value = value;
                    keyFrames[4].value = value;
                    break;
                case DayInterval.Dusk:
                    keyFrames[5].value = value;
                    keyFrames[6].value = value;
                    break;
                case DayInterval.Night:
                    keyFrames[7].value = value;
                    keyFrames[0].value = value;
                    break;
                case DayInterval.Morning:
                    keyFrames[1].value = value;
                    keyFrames[2].value = value;
                    break;
            }

            curve.keys = keyFrames;
        }
    }

    public float ReadTimePoint(AnimationCurve curve, float value)
    {
        float res = value;
        if (curve.keys.Length == 8 || curve.keys.Length == 8)
        {
            Keyframe[] keyFrames = curve.keys;

            switch (DayTime)
            {
                case DayInterval.Day:
                    res = keyFrames[3].value;
                    break;
                case DayInterval.Dusk:
                    res = keyFrames[5].value;
                    break;
                case DayInterval.Night:
                    res = keyFrames[0].value;
                    break;
                case DayInterval.Morning:
                    res = keyFrames[1].value;
                    break;
            }
        }
        else if (Is24Hours)
        {
            float time = TimeOf24 / 24f;
            res = curve.Evaluate(time);
        }
        return res;
    }

    public Color ReadTimePoint(Gradient colorGradient, Color theColor)
    {
        Color res = theColor;
        if (colorGradient.colorKeys.Length == 8 || colorGradient.alphaKeys.Length == 8)
        {
            GradientColorKey[] colorKeys = colorGradient.colorKeys;
            GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;

            switch (DayTime)
            {
                case DayInterval.Day:
                    res = colorKeys[3].color;
                    res.a = alphaKeys[3].alpha;
                    break;
                case DayInterval.Dusk:
                    res = colorKeys[5].color;
                    res.a = alphaKeys[5].alpha;
                    break;
                case DayInterval.Night:
                    res = colorKeys[0].color;
                    res.a = alphaKeys[0].alpha;
                    break;
                case DayInterval.Morning:
                    res = colorKeys[1].color;
                    res.a = alphaKeys[1].alpha;

                    break;
            }
        }
        else if (Is24Hours)
        {
            float time = TimeOf24 / 24f;
            res = colorGradient.Evaluate(time);
        }
        return res;
    }

    private void SaveTo24Sys(Gradient colorGradient, Color theColor)
    {
        if (colorGradient.colorKeys.Length != 8 || colorGradient.alphaKeys.Length != 8)
        {
            GradientColorKey[] colorKeys = new GradientColorKey[8];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[8];

            for (int i = 0; i < 8; i++)
            {
                colorKeys[i].color = Color.black;
                alphaKeys[i].alpha = 1;
            }

            colorGradient.SetKeys(colorKeys, alphaKeys);
        }
        if (colorGradient.colorKeys.Length == 8 || colorGradient.alphaKeys.Length == 8)
        {
            GradientColorKey[] colorKeys = colorGradient.colorKeys;
            GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;

            colorKeys[0].time = NightEnd / 24f;
            alphaKeys[0].time = NightEnd / 24f;
            colorKeys[1].time = MorningStart / 24f;
            alphaKeys[1].time = MorningStart / 24f;
            colorKeys[2].time = MorningEnd / 24f;
            alphaKeys[2].time = MorningEnd / 24f;
            colorKeys[3].time = DayStart / 24f;
            alphaKeys[3].time = DayStart / 24f;
            colorKeys[4].time = DayEnd / 24f;
            alphaKeys[4].time = DayEnd / 24f;
            colorKeys[5].time = DuskStart / 24f;
            alphaKeys[5].time = DuskStart / 24f;
            colorKeys[6].time = DuskEnd / 24f;
            alphaKeys[6].time = DuskEnd / 24f;
            colorKeys[7].time = NightStart / 24f;
            alphaKeys[7].time = NightStart / 24f;

            switch (DayTime)
            {
                case DayInterval.Day:
                    colorKeys[3].color = theColor;
                    alphaKeys[3].alpha = theColor.a;
                    colorKeys[4].color = theColor;
                    alphaKeys[4].alpha = theColor.a;
                    break;
                case DayInterval.Dusk:
                    colorKeys[5].color = theColor;
                    alphaKeys[5].alpha = theColor.a;
                    colorKeys[6].color = theColor;
                    alphaKeys[6].alpha = theColor.a;
                    break;
                case DayInterval.Night:
                    colorKeys[0].color = theColor;
                    alphaKeys[0].alpha = theColor.a;
                    colorKeys[7].color = theColor;
                    alphaKeys[7].alpha = theColor.a;
                    break;
                case DayInterval.Morning:
                    colorKeys[1].color = theColor;
                    alphaKeys[1].alpha = theColor.a;
                    colorKeys[2].color = theColor;
                    alphaKeys[2].alpha = theColor.a;
                    break;
            }


            colorGradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    public Color GetColor(Gradient g, Color c)
    {
        float time = TimeOf24 / 24f;
        return Is24Hours ? g.Evaluate(time) : c;
    }

    public float GetValue(AnimationCurve a, float v)
    {
        float time = TimeOf24 / 24f;
        return Is24Hours ? a.Evaluate(time) : v;
    }
}
