using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using VolumeMixer;

[ExecuteInEditMode]
public partial class ShaderSetter : VMValue {

    

    [System.Serializable]
    public class SetterUnit
    {
        [NoToLua] public string Name;
        [NoToLua] public Texture2D ColorSelectorTex;
        [Space(10)]
        [SerializeField]
        private float DFogDensityValue;
        [NoToLua] public List<float> DFogDensityList;
        [Space(10)]
        [SerializeField]
        private float VFogDensityValue;
        [NoToLua] public List<float> VFogDensityList;
        [Space(10)]
        [SerializeField]
        private float LightMapSliderValue;
        [NoToLua] public List<float> LightMapSliderList;
        [Space(10)]
        [SerializeField]
        private float BrightnessValue;
        [NoToLua] public List<float> BrightnessList;
        [Space(10)]
        [SerializeField]
        private float FogShowSkyValue;
        [NoToLua] public List<float> FogShowSkyList;

        public float RotationSun = 0f;
        public float RotationMoonX = 0f;
        public float RotationMoonY = 0f;
    }

    [Header("Weather Lock")]
    [NoToLua] public bool IsLock = false;   //是否锁定(触发天气变化时,停止时间走动)
    [Space(10)]

    //[NoToLua] public Color Tint;
    [Header("ColorSelector")]
    [NoToLua] public bool IsColorSetter = false;
    [Range(0, 24)]
    [NoToLua] public float TimeHour;

    [NoToLua] public int FirstIndex = 0;
    [NoToLua] public int SecondIndex = 0;
    [Range(0,1)]
    [NoToLua] public float LerpValue = 0;

    [Space(10)]

    [SerializeField] private float DFogDensityValue;
    [SerializeField] private float VFogDensityValue;
    [SerializeField] private float LightMapSliderValue;
    [SerializeField] private float BrightnessValue;
    [SerializeField] private float FogShowSkyValue;

    [Space(20)]
    [Header("注意检查！！！")]
    [Header("查找图必须开启读写")]
    [Header("不然会有打包错误")]
    [NoToLua] public SetterUnit[] Setters;
    [Space(20)]

    [HideInInspector][NoToLua] public Texture2D ColorSelectorTex;    
    [HideInInspector] [NoToLua] public List<float> DFogDensityList;    
    [HideInInspector] [NoToLua] public List<float> VFogDensityList;    
    [HideInInspector] [NoToLua] public List<float> LightMapSliderList;    
    [HideInInspector] [NoToLua] public List<float> BrightnessList;   
    [HideInInspector] [NoToLua] public List<float> FogShowSkyList;

    [Space(20)]
    [Header("GlobalSettings")]
    
    [Range(0, 1)]
    [NoToLua] public float Wetness = 0;
    [Range(0, 0.6f)]
    [NoToLua] public float RainForce = 0;

    [NoToLua] public Texture2D BRDFTex;

    [NoToLua] public Texture2D NoiseTex;
    [NoToLua] public Material SkyBoxMat;
    //[Range(0, 100)]
    //public float Shininess = 1;
    public Transform Player;
    [Space(20)]
    /*[HDR]_FresnelLightColor("_FresnelLightColor", color) = (1,1,1,1)
    [HDR]_FresnelDarkColor("_FresnelDarkColor", color) = (1,1,1,1)
    _SOLightFresnel("_SOLightFresnel", Range(0,1)) = 0
	_SODarkFresnel("_SODarkFresnel", Range(0,1)) = 0
    _SOLightFresnelFar("_SOLightFresnelFar", Range(0,10)) = 0.1
    _SODarkFresnelFar("_SODarkFresnelFar", Range(0,10)) = 0.1
    _SOLightFresnelFarForce("_SOLightFresnelFarForce", Range(0.001,5)) = 0.1
    _SODarkFresnelFarForce("_SODarkFresnelFarForce", Range(0.001,5)) = 0.1*/
    [NoToLua] [ColorUsage(false, true)] public Color _FresnelLightColor = Color.black;
    [NoToLua] [ColorUsage(false, true)] public Color _FresnelDarkColor = Color.black;
    [Range(0, 1)] public float _SOLightFresnel = 0;
	[Range(0, 1)] public float _SODarkFresnel = 0;
    [Range(0, 100)] public float _SOLightFresnelFar = 0;
    [Range(0, 100)] public float _SODarkFresnelFar = 0;
    [Range(0.001f, 5)] public float _SOLightFresnelFarForce = 0.001f;
    [Range(0.001f, 5)] public float _SODarkFresnelFarForce = 0.001f;

    [Space(20)]
    [Space(10)]
    public bool CameraFogOn = true;
    [NoToLua] public bool CameraFogStart = false;
    [NoToLua] public float FogStartY = 0;
    [NoToLua] public float FogHeight = 0;
    [NoToLua] [Range(0,1)]public float VFogVariation1 = 0.1f;
    [NoToLua] public float FogMass = 24;
    [Range(-100, 100)] public float DFogHeight = 20;
    [Range(0, 10)] public float DFogMass = 1;

    [Space(10)]
    [NoToLua] public Texture2D CloudNoiseTex;
    [Space(10)]
    public bool IsUiChara = false;
    [NoToLua] public Color UiCharaLight = new Color(1, 1, 1, 1);
    [NoToLua] public Color UiCharaDark = new Color(0.66f, 0.66f, 0.66f, 1);
    [NoToLua] public Color UiCharaSkinDark = new Color(0.77f, 0.70f, 0.66f, 1);

    [Range(0, 1)]
    [NoToLua] public float CharaBackLight = 0;
    [Range(0, 6.28318f)]
    [NoToLua] public float CharaLightRot = 0;
    [NoToLua] [Range(0, 1)] public float CharaLmShadowForce = 0.6f;
    [NoToLua] [Range(0.1f, 1)] public float LmLightForceInShadow = 1f;
    [NoToLua] [Range(0, 1)] public float LmLightForceInLight = 0f;
    


    [Space(10)]
    [NoToLua] public Texture2D CharaLightModelDiff;
    [NoToLua] public Texture2D CharaLightModel;
    //[NoToLua] public Texture2D CharaLightModelSpec;
    [Space(10)]
    [Range(0, 2)]
    [NoToLua] public float Saturation = 1;
    public float FarFogDistance = 350;



    [Space(20)]
    [Header("Debugger")]
    //[ColorUsageAttribute(true, true)]
    [ColorUsage(true, true)] public Color LightColor = Color.white;
    [NoToLua] [ColorUsage(true, true)] public Color ShadowColor = Color.black;
    [NoToLua] public bool Bake = false;
    [Space(20)]
    [Range(0, 1)]
    [NoToLua] public float LightmapSlider = 0;
    [Range(0, 5)]
    [NoToLua] public float Brightness = 0.58f;
    [Range(0, 1)]
    [NoToLua] public float RealTimeLightOnLightmap = 0;
    [Range(0, 1)]
    [NoToLua] public float ShadowOnLightmap = 0;
    [Space(20)]
    [NoToLua][Range(0,1)] public float CloudShadow = 0.5f;


    [Space(10)]
    public Color FogFar = Color.white;
    public Color FogNear = Color.white;
    [NoToLua] public Color FogLow = Color.white;
    [NoToLua] public Color FogHigh = Color.white;
    [Range(0, 100)]
    public float DFogDensity = 0;
    [Range(0,100)]
    [NoToLua] public float VFogDensity = 0;
    public Color SencondFogFar = Color.white;
    [NoToLua] [Range(0,10)] public float SecondLevelFogDistance;
    [NoToLua] [Range(0,1)] public float SecondLevelFogForce;
    [Range(0, 1)]
    public float FogShowSky = 0;

    [Space(10)]
    public Color SkyLowColor = Color.yellow;
    public Color SkyHighColor = Color.blue;

    [Range(0, 360)] public float RotationSun = 0f;
    [Range(0, 360)] public float RotationMoonX = 0f;
    [Range(0, 360)] public float RotationMoonY = 0f;

    [Space(10)]
    public Color CloudLight = Color.yellow;
    public Color CloudDark = Color.blue;
    public Color CloudDarkControl = Color.gray;
    [Range(0,100)]public float CloudCoverage = 2.6f;
    
    //[Range(0,100)]public float Coverage = 2.5f;

    [Space(10)]
    [NoToLua] public Color CharaLight = new Color(1, 1, 1, 1);
    [NoToLua] public Color CharaDark = new Color(0.2f, 0.2f, 0.2f, 1);
    [NoToLua] public Color CharaSkinDark = new Color(0.76f, 0.4082f, 0.2128f, 1);
    [Space(10)]
    [NoToLua] [ColorUsage(false, true)] public Color GrassColor = Color.white;
    [NoToLua] [ColorUsage(false, true)] public Color GrassShadowColor = Color.white;
    [NoToLua] public Color CloudyColor = Color.white;
    [NoToLua] [ColorUsage(false, true)] public Color LmMultiply = Color.white;

    private Color mTmpColor;
    private Color mCloudyColor;
    private Color mLightColorMemo;
    internal enum BakeState
    {
        BakeStart,
        Baking,
        BakeStop
    }
    private BakeState mBakeState = BakeState.BakeStop;


    //[HDR]_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
    //[HDR]_Shading ("Shading Color", Color) = (0, 0, 0, 1)
    //[HDR]_SkyLightAttenuation ("Sky Light Attenuation", Color) = (0, 0, 0, 1)



    //public List<float> FogDensity;

    private static ShaderSetter mInstance = null;
    [NoToLua]
    public bool mIsDestroyed = false;



    public int GetSetterCount()
    {
        return Setters.Length;
    }

    void Awake()
    {
        mInstance = this;
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
    }

    // Use this for initialization
    void Start () {


        //IsLock = false;
        //InvokeRepeating("ShaderUpdate", 0, 0.05f);
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
    }

    // Update is called once per frame
    void Update () {
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
        //if (IsLock == true) return;
        if (mIsDestroyed) return;

        //根据TimeHour和LerpValue参数改变天气
        this.ChangeWeather();
		SetSkybox ();
    }

    private void OnValidate()
    {
        //根据TimeHour和LerpValue参数改变天气
        this.ChangeWeather();
    }

    public void ChangeWeather()
    {
#if UNITY_EDITOR
        if (Bake)
        {
            switch (mBakeState)
            {
                case BakeState.BakeStop:
                    if (Lightmapping.isRunning != true)
                    {
                        mLightColorMemo = RenderSettings.sun.color;
                        RenderSettings.sun.color = LightColor.gamma;
                        Lightmapping.BakeAsync();
                        mBakeState = BakeState.Baking;
                    }
                    break;
                case BakeState.Baking:
                    if (Lightmapping.isRunning == true)
                    {
                        Bake = true;
                    }
                    else
                    {
                        Bake = false;
                        mBakeState = BakeState.BakeStop;
                        RenderSettings.sun.color = mLightColorMemo;
                    }
                    break;
                default:
                    break;

            }
        }
#endif
        //if (Set == true)
        //GlobalSetting
        {
            if (BRDFTex != null)
                Shader.SetGlobalTexture("_BRDFTex", BRDFTex);
            if (NoiseTex != null)
                Shader.SetGlobalTexture("_NoiseTex", NoiseTex);
            //Shader.SetGlobalFloat("_Shininess", Shininess);

            if (CharaLightModelDiff != null)
                Shader.SetGlobalTexture("_MatCap", CharaLightModelDiff);

            if (CharaLightModel != null)
                Shader.SetGlobalTexture("_LightModel", CharaLightModel);
            /*if (CharaLightModelSpec != null)
                Shader.SetGlobalTexture("_MatCapSpec", CharaLightModelSpec);*/
            if (CameraFogStart == false)
            {
                Shader.SetGlobalFloat("_StartY", FogStartY);
            }
            else
            {
                Shader.SetGlobalFloat("_StartY", transform.position.y);
            }
            Shader.SetGlobalFloat("_FogMass", FogMass);
            Shader.SetGlobalFloat("_VFogVariation", VFogVariation1);
            
            Shader.SetGlobalFloat("_DFogHeight", DFogHeight);
            Shader.SetGlobalFloat("_DFogMass", DFogMass);
            Shader.SetGlobalFloat("_Wetness", Wetness);
            Shader.SetGlobalFloat("_RainForce", RainForce);
            Shader.SetGlobalFloat("_EndY", FogHeight);
#if UNITY_IOS
                        Shader.SetGlobalFloat("_FinalSaturation", Saturation); 
#else
                        Shader.SetGlobalFloat("_FinalSaturation", Saturation);
#endif
            Shader.SetGlobalFloat("_FarFogDistance", FarFogDistance);

            Shader.SetGlobalFloat("_SODarkFresnel", _SODarkFresnel);
            Shader.SetGlobalFloat("_SODarkFresnelFar", _SODarkFresnelFar);
            Shader.SetGlobalFloat("_SODarkFresnelFarForce", _SODarkFresnelFarForce);
            Shader.SetGlobalFloat("_SOLightFresnel", _SOLightFresnel);
            Shader.SetGlobalFloat("_SOLightFresnelFar", _SOLightFresnelFar);
            Shader.SetGlobalFloat("_SOLightFresnelFarForce", _SOLightFresnelFarForce);
            Shader.SetGlobalColor("_FresnelDarkColor", _FresnelDarkColor);
            Shader.SetGlobalColor("_FresnelLightColor", _FresnelLightColor);


            if (Player != null)
            {
                Shader.SetGlobalVector("_PlayerPos", Player.position);
            }
            if (CloudNoiseTex != null)
            {
                Shader.SetGlobalTexture("_CloudNoiseTex", CloudNoiseTex);
            }
        }

        if (IsColorSetter)
        {
            int timePixel = (int)(42.22f * TimeHour);
            if (Setters.Length > FirstIndex && Setters.Length > SecondIndex)
            {
                mCloudyColor = Color.Lerp(Color.white, LerpColor(timePixel, 362), RainForce * 1.667f);

                mTmpColor = LerpColor(timePixel, 533);
                Shader.SetGlobalColor("_LightColorC", mTmpColor * mCloudyColor);
                mTmpColor = LerpColor(timePixel, 576);
                Shader.SetGlobalColor("_ShadowColor", mTmpColor * mCloudyColor);

                mTmpColor = LerpColor(timePixel, 619);
                Shader.SetGlobalColor("_DFogColorFar", mTmpColor * mCloudyColor);
                mTmpColor = LerpColor(timePixel, 662);
                Shader.SetGlobalColor("_DFogColorNear", mTmpColor * mCloudyColor);
                mTmpColor = LerpColor(timePixel, 705);
                Shader.SetGlobalColor("_VFogColorLow", mTmpColor * mCloudyColor);
                mTmpColor = LerpColor(timePixel, 748);
                Shader.SetGlobalColor("_VFogColorHigh", mTmpColor * mCloudyColor);

                mTmpColor = LerpColor(timePixel, 791);
                Shader.SetGlobalColor("_CloudColor", mTmpColor * mCloudyColor);

                mTmpColor = LerpColor(timePixel, 834);
                Shader.SetGlobalColor("_BodyColor", mTmpColor * mCloudyColor);
                mTmpColor = LerpColor(timePixel, 877);
                Shader.SetGlobalColor("_ShadowColor1", mTmpColor * mCloudyColor);
                mTmpColor = LerpColor(timePixel, 920);
                Shader.SetGlobalColor("_ShadowColor2", mTmpColor * mCloudyColor);

                if (SkyBoxMat != null)
                {
                    mTmpColor = LerpColor(timePixel, 963);
                    SkyBoxMat.SetColor("_SkyLowColor", mTmpColor * mCloudyColor);
                    mTmpColor = LerpColor(timePixel, 1006);
                    SkyBoxMat.SetColor("_SkyHighColor", mTmpColor * mCloudyColor);
                }
                LerpFloat();
                Shader.SetGlobalFloat("_Rotationbbb", RotationSun);
                Shader.SetGlobalFloat("_RotationeeeX", RotationMoonX);
                Shader.SetGlobalFloat("_RotationeeeY", RotationMoonY);


    mTmpColor = IsUiChara ? UiCharaLight : LerpColor(timePixel, 491) * mCloudyColor;
                Shader.SetGlobalColor("_LightColorMapping", mTmpColor);
                mTmpColor = IsUiChara ? UiCharaDark : LerpColor(timePixel, 448) * mCloudyColor;
                Shader.SetGlobalColor("_DarkColorMapping", mTmpColor);
                mTmpColor = IsUiChara ? UiCharaSkinDark : LerpColor(timePixel, 405) * mCloudyColor;
                Shader.SetGlobalColor("_SkinDarkColorMapping", mTmpColor);


                mTmpColor = LerpColor(timePixel, 362);
                Shader.SetGlobalColor("_GrassColor", mTmpColor * mCloudyColor);

                mTmpColor = LerpColor(timePixel, 319);
                Shader.SetGlobalColor("_SMShadowColor", mTmpColor * mCloudyColor);


                /*Debug Show Color*/

                /*LightColor =  ColorSelectorTex.GetPixel(timePixel, 533);
                ShadowColor =  ColorSelectorTex.GetPixel(timePixel, 576);

                FogFar =  ColorSelectorTex.GetPixel(timePixel, 619);
                FogNear = ColorSelectorTex.GetPixel(timePixel, 662);
                FogLow = ColorSelectorTex.GetPixel(timePixel, 705);
                FogHigh = ColorSelectorTex.GetPixel(timePixel, 748);

                CloudShadowColor = ColorSelectorTex.GetPixel(timePixel, 791);

                CloudLight = ColorSelectorTex.GetPixel(timePixel, 834);
                CloudDark = ColorSelectorTex.GetPixel(timePixel, 877);
                CloudDarkControl = ColorSelectorTex.GetPixel(timePixel, 920);

                if (SkyBoxMat != null)
                {
                    SkyLowColor = ColorSelectorTex.GetPixel(timePixel, 963);
                    SkyHighColor = ColorSelectorTex.GetPixel(timePixel, 1006);
                }

                //Shader.SetGlobalColor("_SkyLowColor", ColorSelectorTex.GetPixel(timePixel, 533);
                //Shader.SetGlobalColor("_SkyHighColor", ColorSelectorTex.GetPixel(timePixel, 576);

                CharaLight = ColorSelectorTex.GetPixel(timePixel, 491);
                CharaDark = ColorSelectorTex.GetPixel(timePixel, 448);
                CharaSkinDark = ColorSelectorTex.GetPixel(timePixel, 405);*/

                if (!CameraFogOn)
                {
                    Shader.SetGlobalFloat("_DFogDensity", 0);
                    Shader.SetGlobalFloat("_VFogDensity", 0);
                }
                else
                {
                    DFogDensityValue = LerpList(Setters[FirstIndex].DFogDensityList, Setters[SecondIndex].DFogDensityList);
                    Shader.SetGlobalFloat("_DFogDensity", DFogDensityValue * 0.1f);

                    VFogDensityValue = LerpList(Setters[FirstIndex].VFogDensityList, Setters[SecondIndex].VFogDensityList);
                    Shader.SetGlobalFloat("_VFogDensity", VFogDensityValue * 0.1f);
                }



                LightMapSliderValue = LerpList(Setters[FirstIndex].LightMapSliderList, Setters[SecondIndex].LightMapSliderList);
                Shader.SetGlobalFloat("_HemiSlider", LightMapSliderValue * (1 + Wetness * 0.5f));

                BrightnessValue = LerpList(Setters[FirstIndex].BrightnessList, Setters[SecondIndex].BrightnessList);
                Shader.SetGlobalFloat("_Brightness", BrightnessValue);

                FogShowSkyValue = LerpList(Setters[FirstIndex].FogShowSkyList, Setters[SecondIndex].FogShowSkyList);
                Shader.SetGlobalFloat("_ShowSky", FogShowSkyValue);
            }
        }
        else
        {
            mCloudyColor = Color.Lerp(Color.white, CloudyColor, RainForce * 1.667f);

            Shader.SetGlobalColor("_LightColorC", LightColor * mCloudyColor);
            Shader.SetGlobalColor("_ShadowColor", ShadowColor * mCloudyColor);
            RenderSettings.ambientLight = ShadowColor;

            Shader.SetGlobalFloat("_HemiSlider", LightmapSlider * (1 + Wetness * 0.5f));

            Shader.SetGlobalColor("_DFogColorNear", FogNear * mCloudyColor);
            Shader.SetGlobalColor("_DFogColorFar", FogFar * mCloudyColor);
            Shader.SetGlobalColor("_VFogColorLow", FogLow * mCloudyColor);
            Shader.SetGlobalColor("_VFogColorHigh", FogHigh * mCloudyColor);
            Shader.SetGlobalColor("_SDFogColorFar", SencondFogFar * mCloudyColor);

            if (float.IsNaN(DFogDensity) || DFogDensity < 0) DFogDensity = 0;
            if (!CameraFogOn)
            {
                Shader.SetGlobalFloat("_DFogDensity", 0);
                Shader.SetGlobalFloat("_VFogDensity", 0);
                Shader.SetGlobalFloat("_SecondLevelFogForce", 0);
            }
            else
            {
                Shader.SetGlobalFloat("_DFogDensity", DFogDensity * 0.1f);
                Shader.SetGlobalFloat("_VFogDensity", VFogDensity * 0.1f);
                Shader.SetGlobalFloat("_SecondLevelFogDistance", SecondLevelFogDistance);
                Shader.SetGlobalFloat("_SecondLevelFogForce", SecondLevelFogForce);
            }

            Shader.SetGlobalFloat("_CloudShadow", CloudShadow);

            Shader.SetGlobalColor("_BodyColor", CloudLight * mCloudyColor);
            Shader.SetGlobalColor("_RimColor", CloudLight * mCloudyColor);
            Shader.SetGlobalColor("_ShadowColor1", CloudDark * mCloudyColor);
            Shader.SetGlobalColor("_ShadowColor2", CloudDarkControl * mCloudyColor);
            Shader.SetGlobalFloat("_Coverage", CloudCoverage);

            Shader.SetGlobalColor("_LightColorMapping", IsUiChara ? UiCharaLight : (CharaLight * mCloudyColor));
            Shader.SetGlobalColor("_DarkColorMapping", IsUiChara ? UiCharaDark : (CharaDark * mCloudyColor));
            Shader.SetGlobalColor("_SkinDarkColorMapping", IsUiChara ? UiCharaSkinDark : (CharaSkinDark * mCloudyColor));

            Shader.SetGlobalColor("_GrassColor", GrassColor * mCloudyColor);
            Shader.SetGlobalColor("_SMShadowColor", GrassShadowColor * mCloudyColor);
            Shader.SetGlobalColor("_LmMultiply", LmMultiply * mCloudyColor);


            Shader.SetGlobalFloat("_BackLight", CharaBackLight);
            Shader.SetGlobalFloat("_RotDiff", CharaLightRot);
            if (IsUiChara)
            {
                Shader.SetGlobalFloat("_LmShadowForce", 0);
                Shader.SetGlobalFloat("_LmLightForceInShadow", 0);
                Shader.SetGlobalFloat("_LmLightForceInLight", 0);
            }
            else
            {
                Shader.SetGlobalFloat("_LmShadowForce", CharaLmShadowForce);
                Shader.SetGlobalFloat("_LmLightForceInShadow", LmLightForceInShadow);
                Shader.SetGlobalFloat("_LmLightForceInLight", LmLightForceInLight);
            }

            if (SkyBoxMat != null)
            {
                SkyBoxMat.SetColor("_SkyLowColor", SkyLowColor * mCloudyColor);
                SkyBoxMat.SetColor("_SkyHighColor", SkyHighColor * mCloudyColor);
            }

            Shader.SetGlobalFloat("_Rotationbbb", RotationSun);
            Shader.SetGlobalFloat("_RotationeeeX", RotationMoonX);
            Shader.SetGlobalFloat("_RotationeeeY", RotationMoonY);

            Shader.SetGlobalColor("_SkyLowColor", SkyLowColor * mCloudyColor);
            Shader.SetGlobalColor("_SkyHighColor", SkyHighColor * mCloudyColor);
            Shader.SetGlobalFloat("_Brightness", Brightness);
            Shader.SetGlobalFloat("_RtLightOnLm", RealTimeLightOnLightmap);
            Shader.SetGlobalFloat("_LmShadow", ShadowOnLightmap);
            Shader.SetGlobalFloat("_ShowSky", FogShowSky);
        }
    }

    private float LerpList(List<float> list1, List<float> list2)
    {
        float res = 0;
        if (list1.Count > 0)
        {
            int i = (int)(list1.Count * TimeHour / 24);
            if (i == list1.Count)
            {
                i = i - 1;
            }
            int i2 = i + 1;
            if (i2 == list1.Count)
            {
                i2 = 0;
            }
            float tmp = Mathf.Lerp( list1[i],
                                    list1[i2],
                                    (TimeHour - i * 24 / list1.Count) / (24 / list1.Count));
            res = tmp;
        }
        if (list2.Count > 0)
        {
            int i = (int)(list2.Count * TimeHour / 24);
            if (i == list2.Count)
            {
                i = i - 1;
            }
            int i2 = i + 1;
            if (i2 == list2.Count)
            {
                i2 = 0;
            }
            float tmp = Mathf.Lerp(list2[i],
                                    list2[i2],
                                    (TimeHour - i * 24 / list2.Count) / (24 / list2.Count));
            res = Mathf.Lerp(res, tmp, LerpValue);
        }
        return res;
    }

    private Color LerpColor(int timePixel, int v)
    {
        Color c1 = Color.black;
        if (Setters[FirstIndex].ColorSelectorTex != null)
        {
            c1 = Setters[FirstIndex].ColorSelectorTex.GetPixel(timePixel, v);
        }
        Color c2 = Color.black;
        if (Setters[SecondIndex].ColorSelectorTex != null)
        {
            c2 = Setters[SecondIndex].ColorSelectorTex.GetPixel(timePixel, v);
        }
        c1 = Color.Lerp(c1, c2, LerpValue);
        return c1;
    }

    private void LerpFloat()
    {
        RotationSun = Mathf.Lerp(Setters[FirstIndex].RotationSun, Setters[SecondIndex].RotationSun, LerpValue);
        RotationMoonX = Mathf.Lerp(Setters[FirstIndex].RotationMoonX, Setters[SecondIndex].RotationMoonX, LerpValue);
        RotationMoonY = Mathf.Lerp(Setters[FirstIndex].RotationMoonY, Setters[SecondIndex].RotationMoonY, LerpValue);
    }

    [NoToLua]
    public void ExcuteDestroy()
    {
        // flags
        mIsDestroyed = true;

        // release texture
        if (this.Setters != null)
        {
            int count = this.Setters.Length;
            for (int i = 0; i < count; i++)
            {
                Texture2D colorTex = this.Setters[i].ColorSelectorTex;
                if (colorTex != null)
                {
                    Resources.UnloadAsset(colorTex);
                    colorTex = null;
                }
            }
        }
    }

    //
    // Clear memory when scene unload
    //
    public static void ClearTextureMemory()
    {
        if (mInstance != null)
            mInstance.ExcuteDestroy();

        mInstance = null;
    }


    //插值辅助函数
    private Color LerpColor2(ShaderSetter.SetterUnit setterUnit, int v, float startTime, float endTime, float rate)
    {
        if (setterUnit.ColorSelectorTex == null) return Color.black;
        //int timePixel = (int)(42.22f * TimeHour);
        float scale = 42.22f;   //每小时占用42.22像素;
        int timePixelStart = (int)(startTime * scale);
        int timePixelEnd = (int)(endTime * scale);

        Color color1 = setterUnit.ColorSelectorTex.GetPixel(timePixelStart, v);
        Color color2 = setterUnit.ColorSelectorTex.GetPixel(timePixelEnd, v);
        Color lerpColor = Color.Lerp(color1, color2, rate);
        return lerpColor;
    }

    private float LerpList(List<float> listValue,int start,int end,float rate)
    {
        if (listValue == null) return 0;
        if (start < 0 || start >= listValue.Count) return 0;
        if (end < 0 || end >= listValue.Count) return 0;

        float value1 = listValue[start];
        float value2 = listValue[end];
        float result = Mathf.Lerp(value1, value2, rate);

        return result;
    }


    public void WeatherRainLerp(int setterIndex, float start, float end, float rate)
    {

    }

    //同一区域,不同时间段的颜色和参数插值(added by thjie 2019-01-16)
    public void WeatherColorLerp(int setterIndex, float start, float end, float rate)
    {
        if (setterIndex < 0 || setterIndex >= Setters.Length) return;
        ShaderSetter.SetterUnit setterUnit = Setters[setterIndex];
        if (setterUnit == null) return;

        //颜色变化
        if (IsColorSetter)
        {
            mCloudyColor = Color.Lerp(Color.white, LerpColor2(setterUnit, 362, start, end, rate), RainForce * 1.667f);
            mTmpColor = LerpColor2(setterUnit, 533, start, end, rate);
            Shader.SetGlobalColor("_LightColorC", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 576, start, end, rate);
            Shader.SetGlobalColor("_ShadowColor", mTmpColor * mCloudyColor);
            RenderSettings.ambientLight = mTmpColor * mCloudyColor;
            mTmpColor = LerpColor2(setterUnit, 619, start, end, rate);
            Shader.SetGlobalColor("_DFogColorFar", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 662, start, end, rate);
            Shader.SetGlobalColor("_DFogColorNear", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 705, start, end, rate);
            Shader.SetGlobalColor("_VFogColorLow", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 748, start, end, rate);
            Shader.SetGlobalColor("_VFogColorHigh", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 791, start, end, rate);
            Shader.SetGlobalColor("_CloudColor", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 834, start, end, rate);
            Shader.SetGlobalColor("_BodyColor", mTmpColor * mCloudyColor);
            Shader.SetGlobalColor("_RimColor", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 877, start, end, rate);
            Shader.SetGlobalColor("_ShadowColor1", mTmpColor * mCloudyColor);
            mTmpColor = LerpColor2(setterUnit, 920, start, end, rate);
            Shader.SetGlobalColor("_ShadowColor2", mTmpColor * mCloudyColor);
            if (SkyBoxMat != null)
            {
                mTmpColor = LerpColor2(setterUnit, 963, start, end, rate);
                SkyBoxMat.SetColor("_SkyLowColor", mTmpColor * mCloudyColor);
                mTmpColor = LerpColor2(setterUnit, 1006, start, end, rate);
                SkyBoxMat.SetColor("_SkyHighColor", mTmpColor * mCloudyColor);
            }

            Shader.SetGlobalFloat("_Rotationbbb", RotationSun);
            Shader.SetGlobalFloat("_RotationeeeX", RotationMoonX);
            Shader.SetGlobalFloat("_RotationeeeY", RotationMoonY);

            mTmpColor = IsUiChara ? UiCharaLight : LerpColor2(setterUnit, 491, start, end, rate) * mCloudyColor;
            Shader.SetGlobalColor("_LightColorMapping", mTmpColor);
            mTmpColor = IsUiChara ? UiCharaDark : LerpColor2(setterUnit, 448, start, end, rate) * mCloudyColor;
            Shader.SetGlobalColor("_DarkColorMapping", mTmpColor);
            mTmpColor = IsUiChara ? UiCharaSkinDark : LerpColor2(setterUnit, 405, start, end, rate) * mCloudyColor;
            Shader.SetGlobalColor("_SkinDarkColorMapping", mTmpColor);

        }


        //公用变量
        if (BRDFTex != null)
            Shader.SetGlobalTexture("_BRDFTex", BRDFTex);
        if (NoiseTex != null)
            Shader.SetGlobalTexture("_NoiseTex", NoiseTex);
        //Shader.SetGlobalFloat("_Shininess", Shininess);

        if (CharaLightModelDiff != null)
            Shader.SetGlobalTexture("_MatCap", CharaLightModelDiff);
        /*if (CharaLightModelSpec != null)
            Shader.SetGlobalTexture("_MatCapSpec", CharaLightModelSpec);*/

        Shader.SetGlobalFloat("_StartY", FogStartY);
        Shader.SetGlobalFloat("_Wetness", Wetness);
        Shader.SetGlobalFloat("_RainForce", RainForce);
        Shader.SetGlobalFloat("_EndY", FogHeight);
#if UNITY_IOS
                Shader.SetGlobalFloat("_FinalSaturation", Saturation); 
#else
                Shader.SetGlobalFloat("_FinalSaturation", Saturation);
#endif
        Shader.SetGlobalFloat("_FarFogDistance", FarFogDistance);

        if (Player != null)
        {
            Shader.SetGlobalVector("_PlayerPos", Player.position);
        }
        if (CloudNoiseTex != null)
        {
            Shader.SetGlobalTexture("_CloudNoiseTex", CloudNoiseTex);
        }


    }



}
