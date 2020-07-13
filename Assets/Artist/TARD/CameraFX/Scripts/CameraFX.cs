using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class CameraFX : MonoBehaviour
{
    public bool ShowOverdraw = false;
    private bool bShowOverdraw = false;
    private bool mBloomState = true;
    public Shader OverdrawShader;
    [Space(20)]
    public Transform PlayerPos;
    private Vector3 mLastPlayerPos;
    public bool FirstScene_CBSystemOn { get { return CB_Manager.Instance != null; } }

    [Space(30)]
    public bool OpenWater = false;
    public bool OpenShadow = true;
    [HideInInspector] public float MaxHeight = 1024f;
    private Camera mDepthCam;
    private RenderTexture mRT;

    //private RenderTexture mBlurRT;
    //private RenderTexture mBlurRT2;
    private RenderTexture[] mAutoExplosureRTs = new RenderTexture[4];
    private RenderTexture mAERTMemo, mAERTMemo1;

    public void DisableCFX()
    {
        Invoke("DisableCFXAfter", 0.3f);
    }

    void DisableCFXAfter()
    {
        this.enabled = false;
    }

    /*[Space(30)]
    public bool VolumetricLight = false;
    public Transform MainLight;
    public Color ColorThreshold = Color.gray;
    [Range(1.0f, 4.0f)]
    public float LightPowFactor = 3.0f;
    [Range(0.0f, 5.0f)]
    public float LightRadius = 2.0f;
    [Range(0, 3)]
    public int DownSample = 1;
    [Range(0.0f, 10.0f)]
    public float SamplerScale = 1;
    [Range(0.0f, 20.0f)]
    public float LightFactor = 0.5f;
    [Range(1, 3)]
    public int BlurIteration = 2;
    [Range(0, 1)]
    public float LightSaturation = 1;
    public Color LightColor = Color.white;
    public bool LightDebug = false;
    [Space(30)]*/

    [HideInInspector] public static bool ObjSpaceFx = true;
    private static bool mObjSpaceFx = false;

    public bool EnableRadiusBlur = false;


    [NoToLua]
    public Material Mat;

    private Camera _mCam = null;

    
    private Camera mCam
    {
        get
        {
            if (_mCam == null)
                _mCam = GetComponent<Camera>();
            return _mCam;
        }
        set
        {
            _mCam = value;
        }
    }

    [NoToLua]
    [Space(20)]
    public bool UseLut = false;
    [NoToLua]
    public Texture3D lutTexture;
    [NoToLua]
    public Texture3D lutTexture2;
    [NoToLua]
    [Range(0, 1)]
    public float LerpLut;
    //[NoToLua]
    //[Tooltip("Select this if input to postprocess has color values bigger than 1, like in HDR rendering mode enabled")]
    //public bool clampColorRangeTo01;


    [Space(20)]
    public bool AA = false;
    [HideInInspector] [Range(0.01f, 4)] public float AASample = 1;
    [HideInInspector] public float EdgeThresholdMin = 0.05f;
    [HideInInspector] public float EdgeThreshold = 0.2f;
    [HideInInspector] public float EdgeSharpness = 4f;

    private Texture2D PreviewRT;

    private float Timer;

    private Light mSun;
    private float mRBForce;

    [Space(20)]
    public bool AutoExplosure = false;
    //[Range(0, 1)] public float A = 0.25f;
    [Range(0, 1)] public float Light = 0.15f;
    [Range(0, 1)] public float Dark = 0.15f;
    //[Range(0, 1)] public float D = 0.15f;

    [Space(20)]
    public bool Vignette = false;

    [Space(20)]
    public bool ChromaticAberration = false;

    [Space(20)]
    public bool RotateBlur = false;
    public float RotateBlurSpeedThreshold = 60f;
    private float LastRotateAngle;
    private float ASpeedBuffer;

    private TJiaGrassGenerator mTGG;
    private bool mTGGState = true;

    [Space(20)]
    public bool IsMangaScreen = false;
    public Material MangaScreenMat;
    public float MangaScreenAngle = 75;
    private float bMangaScreenAngle = -1;
    private MangaScreen mMangaScreen = null;
    private RenderTexture mStoryBoardCameraRt = null;
    private Camera mStoryBoardCameraRef = null;


//Zzc HorizontalBlur        
[Space(50)]
    public bool horizontalBlurActive = false;
    [Range(0f, 1f)]
    public float size = 0f;
    [Range(0, 8)]
    public int horizontalDownSample = 8;
    [Range(0, 4)]
    public int verticalDownSample = 4;
    private Material horizontalMat;
    private RenderTexture horizontalBlurRT;
    private int id = Shader.PropertyToID("_Size");

    //Zzc playerRipple
    [Space(20)]
    private RippleFollowPlayer rfp;

    public bool DepthOn = false;

#if BUILD_SINGLESCNE_MODE
    private Camera singleCamera;
#endif

    private void OpenDepth()
    {
        if (OpenShadow)
        {
            if (QualitySettings.shadows != ShadowQuality.Disable)
            {
                //if (mDepthCam != null)
                {
                    //Destroy(GameObject.Find("DepthCam"));
                }
                //return;
            }
            else
            {
                QualitySettings.shadows = ShadowQuality.All;
            }
        }
        else
        {
            if (QualitySettings.shadows != ShadowQuality.Disable)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
        }
        OpenDepthCam();
    }

    private void OpenDepthCam()
    {
        if (DepthOn)
        {
            if (mCam.depthTextureMode != DepthTextureMode.Depth)
            {
                mCam.depthTextureMode = DepthTextureMode.Depth;
            }
            return;
        }
		//if (FirstScene_CBSystemOn && SceneManager.sceneCount == 1) {
		if (FirstScene_CBSystemOn) {
			return;
		}
        if (mCam == null)
        {
#if BUILD_SINGLESCNE_MODE
            mCam = singleCamera;
#else
            _mCam = GetComponent<Camera>();
#endif
        }

        if (mCam.depthTextureMode != DepthTextureMode.None)
        {
            mCam.depthTextureMode = DepthTextureMode.None;
        }

        if (OpenWater && mDepthCam == null)
        {
            if (mCam != null)
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying)
                {
                    mCam.depthTextureMode = DepthTextureMode.Depth;
                }
                else
#endif
                {
                    if (mCam.depthTextureMode != DepthTextureMode.None)
                    {
                        mCam.depthTextureMode = DepthTextureMode.None;
                    }

                    if (mDepthCam == null)
                    {
                        if (GameObject.Find("DepthCam"))
                        {
                            mDepthCam = GameObject.Find("DepthCam").GetComponent<Camera>();
                        }
                        else
                        {
                            mDepthCam = new GameObject().AddComponent<Camera>();
                            mDepthCam.gameObject.name = "DepthCam";
                        }
                    }
                    if (mDepthCam != null)
                    {
                        mDepthCam.depthTextureMode = DepthTextureMode.Depth;
                        mDepthCam.clearFlags = CameraClearFlags.SolidColor;
                        mDepthCam.depth = -20;
                        mDepthCam.transform.parent = mCam.transform;
                        mDepthCam.transform.localPosition = Vector3.zero;
                        mDepthCam.transform.localEulerAngles = Vector3.zero;
                        mDepthCam.nearClipPlane = mCam.nearClipPlane;
                        mDepthCam.farClipPlane = mCam.farClipPlane;
                        mDepthCam.useOcclusionCulling = false;
                        //mDepthCam.fieldOfView = mCam.fieldOfView;
                        GameHelper.SetCameraFOV(mDepthCam, mCam.fieldOfView);
                        mDepthCam.cullingMask = 1 << 4 | 1 << 17 | 1 << 28;
                        if(mDepthCam.gameObject.GetComponent<CameraAvoidShadow>() == null)
                            mDepthCam.gameObject.AddComponent<CameraAvoidShadow>();// 避免重复添加导致设置shadowdistance错误

                        float coef = MaxHeight / Screen.height;
                        coef = coef < 1 ? coef : 1;
                        coef = 0.7f;
                        mRT = new RenderTexture((int)((float)Screen.width * coef), (int)((float)Screen.height * coef), 0);
                        mRT.name = "DepthRT";
                        mRT.format = RenderTextureFormat.R8;
                        mDepthCam.targetTexture = mRT;
                        mDepthCam.SetReplacementShader(UnityUtils.FindShader("Legacy Shaders/VertexLit"), "RenderType");
                        mDepthCam.allowMSAA = false;
                        mDepthCam.allowHDR = false;
                        //mDepthCam.enabled = false;
                    }
                }
            }
        }
        else if (!OpenWater && mDepthCam != null)
        {
            if (GameObject.Find("DepthCam"))
            {
                Destroy(GameObject.Find("DepthCam"));
            }
        }
    }

    // Use this for initialization
    [NoToLua]
    void Start()
    {        
        Shader.SetGlobalInt("_CrossfadeOn", 1);

        PlayerPos = null;
        OpenShadow = true;
        OverdrawShader = UnityUtils.FindShader("SAO_TJia_V3/Debugger/Overdraw");

#if BUILD_SINGLESCNE_MODE
        singleCamera = GameObject.Find("客户端测试节点_请勿删除/saoUIJoyPadCom/Camera").GetComponent<Camera>();
#endif
        mSun = RenderSettings.sun;

#if BUILD_SINGLESCNE_MODE
        mCam = singleCamera;
        mCam = singleCamera;
#else
        _mCam = GetComponent<Camera>();
#endif
        OpenDepth();

        if (GetComponent<DBP_Controler>())
        {

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                DestroyImmediate(GetComponent<DBP_Controler>());
            }
            else
            {
                Destroy(GetComponent<DBP_Controler>());
            }
#else
            Destroy(GetComponent<DBP_Controler>());
#endif
        }
        if (!GetComponent<QuadtreeManager>() && GetComponent<CameraController>())
        {
            QuadtreeManager qtm = gameObject.AddComponent<QuadtreeManager>();
            //qtm.HideAngle = 30;
        }
        else if(GetComponent<QuadtreeManager>())
        {
            GetComponent<QuadtreeManager>().enabled = true;
        }

        mTGG = GetComponent<TJiaGrassGenerator>();

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.tag == "MainPlayer")
            {
                PlayerPos = go.transform;
                if (mTGG != null)
                {
                    mTGG.PlayerPos = PlayerPos;
                }
                break;
            }
        }

        //------------------------------------------------------------
        //Add by Zzc
        //------------------------------------------------------------
        PlayerAddRipple();

        int pSize = 8;
        for (int i = 0; i < mAutoExplosureRTs.Length; i++)
        {
            mAutoExplosureRTs[i] = new RenderTexture(pSize, pSize, 0);
            pSize /= 2;
        }
        mAERTMemo = new RenderTexture(1, 1, 0);
        mAERTMemo1 = new RenderTexture(1, 1, 0);
    }

    protected void OnEnable()
    {
        //QualitySettings.shadowDistance = 20;
        PreviewRT = new Texture2D(1, 1);
        Color color = Color.black;
        color.a = 0;
        PreviewRT.SetPixel(0, 0, color);
        PreviewRT.SetPixel(1, 1, color);
        PreviewRT.Apply();
        //PreviewRT.name = "PreviewRT";
        if (SystemInfo.supports3DTextures == false)
        {
            Debug.LogWarning("Device does not support 3D textures");
            enabled = false;
            return;
        }
        if (lutTexture == null)
        {
            Debug.LogWarning("Missing lut texture");
        }
        InitializeLUTMat();
        /*if (FindObjectOfType<RoleShadowmapCamera>() == null)
        {
            Shader.SetGlobalTexture("_DecalTexture", PreviewRT);
            Shader.SetGlobalMatrix("shadowProjectionMatrix", new Matrix4x4());
        }*/
        if (FindObjectOfType<DecalEffect>() == null)
        {
            Shader.SetGlobalTexture("_DecalTexture", PreviewRT);
            Shader.SetGlobalMatrix("_DecalProjectionMatrix", new Matrix4x4());
        }
        if (FindObjectOfType<RoleShadowmapCamera>() && FindObjectOfType<DecalEffect>())
        {
            //RenderTexture.ReleaseTemporary(PreviewRT);
        }
#if UNITY_EDITOR
        if (Lightmapping.lightingDataAsset == null)
        {
            Shader.SetGlobalFloat("_InGame", 0);
        }
        else// if (EditorApplication.isPlaying)
        {
            Shader.SetGlobalFloat("_InGame", 1);
        }
#else
        Shader.SetGlobalFloat("_InGame", 1);
#endif
        //if (EnableRadiusBlur && mBlurRT == null)
        //{
        //    mBlurRT = RenderTexture.GetTemporary((int)(Screen.width), (int)(Screen.height));
        //    mBlurRT.name = "BlurRT";
        //    mBlurRT2 = RenderTexture.GetTemporary((int)(Screen.width), (int)(Screen.height));
        //    mBlurRT2.name = "BlurRT2";
        //}

    }

    private void OnDisable()
    {
        //if (mBlurRT != null)
        //{
        //    RenderTexture.ReleaseTemporary(mBlurRT);
        //    RenderTexture.ReleaseTemporary(mBlurRT2);
        //}
    }

    private void InitializeLUTMat()
    {
        if (lutTexture == null)
        {
            return;
        }
        Mat.SetTexture("_LUTTex", lutTexture);
        Mat.SetTexture("_LUTTex2", lutTexture2);
        Mat.SetFloat("_LerpLUT", LerpLut);
        /*if (clampColorRangeTo01)
            Mat.EnableKeyword("HIGH_COLOR_RANGE");
        else
            Mat.DisableKeyword("HIGH_COLOR_RANGE");*/
    }

    private void Update()
    {
        //if (EnableRadiusBlur && mBlurRT == null)
        //{
        //    mBlurRT = RenderTexture.GetTemporary((int)(Screen.width), (int)(Screen.height));
        //    mBlurRT.name = "BlurRT";
        //    mBlurRT2 = RenderTexture.GetTemporary((int)(Screen.width), (int)(Screen.height));
        //    mBlurRT2.name = "BlurRT2";
        //}
        //else if (mBlurRT != null)
        //{
        //    RenderTexture.ReleaseTemporary(mBlurRT);
        //    RenderTexture.ReleaseTemporary(mBlurRT2);
        //}

        if (IsMangaScreen && MangaScreenMat != null && mMangaScreen == null)
        {
            if (gameObject.GetComponent<MangaScreen>())
            {
                mMangaScreen = gameObject.GetComponent<MangaScreen>();
            }
            else
            {
                mMangaScreen = gameObject.AddComponent<MangaScreen>();
            }
            mMangaScreen.MangaMat = MangaScreenMat;

            // 绑定camera dest rt, 以及mat的texture
            GameObject tGo = GameObject.FindGameObjectWithTag("StoryBoardCamera");// if active=false, will find nothing!
            if (tGo != null)
            {
                Camera cameraRt2 = tGo.GetComponent<Camera>();
                mStoryBoardCameraRef = cameraRt2;
                if (cameraRt2.targetTexture == null)
                {
                    mStoryBoardCameraRt = RenderTexture.GetTemporary(1366, 768, 24, RenderTextureFormat.ARGB32);
                    cameraRt2.targetTexture = mStoryBoardCameraRt;
                    mMangaScreen.MangaMat.SetTexture("_MainTex2", mStoryBoardCameraRt);
                    //Debug.Log("CameraFX: turn on, rt isnot exist in camera, dynamic create ref!");
                }
                else
                {
                    mStoryBoardCameraRt = cameraRt2.targetTexture;
                    mMangaScreen.MangaMat.SetTexture("_MainTex2", mStoryBoardCameraRt);
                    //Debug.Log("CameraFX: turn on, rt is exist in camera, get ref!");
                }
            }
        }
        else if (!IsMangaScreen && mMangaScreen != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(mMangaScreen);
            }
            else
#endif
            {
                Destroy(mMangaScreen);
            }

            // release in ondisable function
            /*if (mStoryBoardCameraRt != null)
            {
                if (mStoryBoardCameraRef != null) mStoryBoardCameraRef.targetTexture = null;
                mStoryBoardCameraRef = null;

                if(mStoryBoardCameraRt != null) RenderTexture.ReleaseTemporary(mStoryBoardCameraRt);
                mStoryBoardCameraRt = null;
                Debug.Log("CameraFX: turn off, release rt!");
            }*/
        }
        else if (IsMangaScreen && MangaScreenMat != null && bMangaScreenAngle != MangaScreenAngle)
        {
            bMangaScreenAngle = MangaScreenAngle;
            MangaScreenMat.SetFloat("_Angle", MangaScreenAngle * 0.01745f);
        }

        Mat.SetVector("_CameraForward", mCam.transform.forward);

        if (mTGG != null)
        {
            if (TARDSwitches.GrassSwitch != mTGGState)
            {
                mTGGState = TARDSwitches.GrassSwitch;
                mTGG.enabled = mTGGState;
            }
        }

        OpenDepth();

        if (mObjSpaceFx != ObjSpaceFx)
        {
            mObjSpaceFx = ObjSpaceFx;
        }

        Shader.SetGlobalVector("_CamPos", transform.position);

        if (Mat != null)
        {
           

            Mat.SetTexture("_LUTTex", lutTexture);
            Mat.SetTexture("_LUTTex2", lutTexture2);
            Mat.SetFloat("_LerpLUT", LerpLut);

            if (UseLut)
                Mat.EnableKeyword("USE_LUT");
            else
                Mat.DisableKeyword("USE_LUT");
            /*if (clampColorRangeTo01)
                Mat.EnableKeyword("HIGH_COLOR_RANGE");
            else
                Mat.DisableKeyword("HIGH_COLOR_RANGE");*/

            if (ObjSpaceFx)
            {
                Shader.EnableKeyword("OBJECT_SPACE_FX");
            }
            else
            {
                Shader.DisableKeyword("OBJECT_SPACE_FX");

            }
            {
                Mat.SetFloat("_LightFactor", 0);
            }

            //AA = false;
            if (AA)
            {
                Mat.EnableKeyword("AA");
                float rcpWidth = 1.0f / Screen.width;
                float rcpHeight = 1.0f / Screen.height;

                Mat.SetVector("_rcpFrame", new Vector4(rcpWidth, rcpHeight, 0, 0));
                Mat.SetVector("_rcpFrameOpt", new Vector4(rcpWidth * 2 * AASample, rcpHeight * 2 * AASample, rcpWidth * 0.5f / AASample, rcpHeight * 0.5f / AASample));
                Mat.SetFloat("_EdgeThresholdMin", EdgeThresholdMin);
                Mat.SetFloat("_EdgeThreshold", EdgeThreshold);
                Mat.SetFloat("_EdgeSharpness", EdgeSharpness);
            }
            else
            {
                Mat.DisableKeyword("AA");
            }

            if (AutoExplosure)
            {
                Mat.EnableKeyword("AUTO_EXPOSURE");
                Mat.SetTexture("_AutoExplosureTex", mAERTMemo);
                Mat.SetTexture("_AERTMemo", mAERTMemo1);
                Mat.SetFloat("_B", Light);
                Mat.SetFloat("_C", Dark);
            }
            else
            {
                Mat.DisableKeyword("AUTO_EXPOSURE");
            }

            if (Vignette)
            {
                Mat.EnableKeyword("VIGNETTE");
            }
            else
            {
                Mat.DisableKeyword("VIGNETTE");
            }

            if (Vignette)
            {
                Mat.EnableKeyword("VIGNETTE");
            }
            else
            {
                Mat.DisableKeyword("VIGNETTE");
            }

            if (ChromaticAberration)
            {
                Mat.EnableKeyword("CHROMATIC_ABERRATION");
            }
            else
            {
                Mat.DisableKeyword("CHROMATIC_ABERRATION");
            }

            if (RotateBlur)
            {
                float camAngle = mCam.transform.eulerAngles.y;
                float dAngle = Mathf.Abs(camAngle - LastRotateAngle);

                if (dAngle > 360)
                {
                    dAngle %= 360;
                }
                if (dAngle > 270)
                {
                    dAngle = 360 - dAngle;
                }

                float aSpeed = dAngle / Time.deltaTime;
                ASpeedBuffer = Mathf.Lerp(aSpeed, ASpeedBuffer, 0.4f);
                float dSpeed = ASpeedBuffer - RotateBlurSpeedThreshold;
                if (dSpeed > 0)
                {
                    Mat.EnableKeyword("ROTATE_BLUR");
                    Mat.SetFloat("_RotSpeed", dSpeed * 0.00278f);
                    //Debug.Log("Speed : " + dSpeed + " DAngle : " + dAngle);
                }
                else
                {
                    Mat.DisableKeyword("ROTATE_BLUR");
                }
                LastRotateAngle = camAngle;
            }
            else
            {
                Mat.DisableKeyword("ROTATE_BLUR");
            }
        }
        if (PlayerPos != null)
        {
            Vector3 playerPos = PlayerPos.position;
            Shader.SetGlobalVector("_LastPlayerPos", mLastPlayerPos);
            Shader.SetGlobalVector("_PlayerPosition", playerPos);
            Shader.SetGlobalVector("_PlayerDir", PlayerPos.forward);
            mLastPlayerPos = playerPos;
            //Debug.Log(PlayerPos.position);
        }
        else if (Time.time > Timer + 5)
        {
            
            Timer = Time.time;

            foreach (GameObject go in FindObjectsOfType<GameObject>())
            {
                if (go.tag == "MainPlayer")
                {
                    PlayerPos = go.transform;
                    if (mTGG != null)
                    {
                        mTGG.PlayerPos = PlayerPos;
                    }
                    break;
                }
            }


            //------------------------------------------------------------
            //Add by Zzc
            //------------------------------------------------------------
            PlayerAddRipple();

        }
        if (ShowOverdraw != bShowOverdraw)
        {
            bShowOverdraw = ShowOverdraw;
            if (ShowOverdraw)
            {
                if (GetComponent<TJiaBloom>())
                {
                    mBloomState = GetComponent<TJiaBloom>().enabled;
                    GetComponent<TJiaBloom>().enabled = false;
                }
                mCam.SetReplacementShader(OverdrawShader, "");
                GameObject uiCam = GameObject.Find("UGUICamera");
                if (uiCam != null)
                {
                    uiCam.GetComponent<Camera>().SetReplacementShader(OverdrawShader, "");
                }
                mCam.clearFlags = CameraClearFlags.Color;
                mCam.backgroundColor = Color.black;
            }
            else
            {
                if (GetComponent<TJiaBloom>())
                {
                    GetComponent<TJiaBloom>().enabled = mBloomState;
                }
                GameObject uiCam = GameObject.Find("UGUICamera");
                if (uiCam != null)
                {
                    uiCam.GetComponent<Camera>().ResetReplacementShader();
                }
                mCam.ResetReplacementShader();
                mCam.clearFlags = CameraClearFlags.Skybox;
            }
        }
    }

    // Update is called once per frame
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!horizontalMat)
        {
            horizontalMat = new Material(UnityUtils.FindShader("Zzc/ZzcHorizontalBlur"));
        }



        //Shader.EnableKeyword("LIGHTMAP_SHADOW_MIXING");
        //Shader.EnableKeyword("SHADOWS_SHADOWMASK");
        //Shader.EnableKeyword("SHADOWS_SCREEN");
        /*if (mObjSpaceFx != ObjSpaceFx)
        {
            mObjSpaceFx = ObjSpaceFx;

			if (!FirstScene_CBSystemOn) {
				if (!ObjSpaceFx)
					mCam.depthTextureMode = DepthTextureMode.Depth;
#if !UNITY_EDITOR
			else
				mCam.depthTextureMode = DepthTextureMode.None;
#else
                else if (EditorApplication.isPlaying)
                {
                    mCam.depthTextureMode = DepthTextureMode.None;
                }
#endif
            }
        }
		if (!FirstScene_CBSystemOn) {
#if !UNITY_EDITOR
            mCam.depthTextureMode = DepthTextureMode.None;
#endif
        }
        Shader.SetGlobalVector("_CamPos", transform.position);
#if UNITY_EDITOR
        if (ShowCamInfo)
        {
            CamPos = transform.position.x.ToString("f1") + ","
                    + transform.position.y.ToString("f1") + ","
                    + transform.position.z.ToString("f1");
            CamRot = transform.rotation.eulerAngles.x.ToString("f1") + ","
                    + transform.rotation.eulerAngles.y.ToString("f1") + ","
                    + transform.rotation.eulerAngles.z.ToString("f1");
        }
#endif*/

        if (Mat != null && (AA || EnableRadiusBlur || UseLut || AutoExplosure || Vignette || ChromaticAberration || RotateBlur) && !ShowOverdraw)
        {

            /*float tanHalfFOV = Mathf.Tan(0.5f * mCam.fieldOfView * Mathf.Deg2Rad);
            float halfHeight = tanHalfFOV * mCam.nearClipPlane;
            float halfWidth = halfHeight * mCam.aspect;
            Vector3 toTop = mCam.transform.up * halfHeight;
            Vector3 toRight = mCam.transform.right * halfWidth;
            Vector3 forward = mCam.transform.forward * mCam.nearClipPlane;
            Vector3 toTopLeft = forward + toTop - toRight;
            Vector3 toBottomLeft = forward - toTop - toRight;
            Vector3 toTopRight = forward + toTop + toRight;
            Vector3 toBottomRight = forward - toTop + toRight;

            toTopLeft /= mCam.nearClipPlane;
            toBottomLeft /= mCam.nearClipPlane;
            toTopRight /= mCam.nearClipPlane;
            toBottomRight /= mCam.nearClipPlane;

            Matrix4x4 frustumDir = Matrix4x4.identity;
            frustumDir.SetRow(0, toBottomLeft);
            frustumDir.SetRow(1, toBottomRight);
            frustumDir.SetRow(2, toTopLeft);
            frustumDir.SetRow(3, toTopRight);


            Mat.SetMatrix("_FrustumDir", frustumDir);*/
            /*Mat.SetVector("_CameraForward", mCam.transform.forward);

            Mat.SetTexture("_LUTTex", lutTexture);
            Mat.SetTexture("_LUTTex2", lutTexture2);
            Mat.SetFloat("_LerpLUT", LerpLut);
            Mat.SetFloat("_ScreenForce", ScreenForce);
            Mat.SetFloat("_OverlayForce", OverlayForce);
            if (UseLut)
                Mat.EnableKeyword("USE_LUT");
            else
                Mat.DisableKeyword("USE_LUT");
            if (clampColorRangeTo01)
                Mat.EnableKeyword("HIGH_COLOR_RANGE");
            else
                Mat.DisableKeyword("HIGH_COLOR_RANGE");

            if (ObjSpaceFx)
            {
                Shader.EnableKeyword("OBJECT_SPACE_FX");
                Shader.SetGlobalFloat("_CloudSpeed", CloudSpeed);
                Shader.SetGlobalFloat("_CloudUVCoef", CloudUVCoef);
                Shader.SetGlobalTexture("_Rain", Rain);
                //Graphics.Blit(source, destination);
            }
            else
            {
                Shader.DisableKeyword("OBJECT_SPACE_FX");

            }*/
            //if ( VolumetricLight && MainLight != null)
            /*if(false)
            {
                Vector3 viewPortLightPos = MainLight == null ? new Vector3(.5f, .5f, 0) : mCam.WorldToViewportPoint(MainLight.position);

                if (viewPortLightPos.z > 0)
                {

                    Mat.SetColor("_ColorThreshold", ColorThreshold);
                    Mat.SetVector("_ViewPortLightPos", new Vector4(viewPortLightPos.x, viewPortLightPos.y, viewPortLightPos.z, 0));
                    Mat.SetFloat("_LightRadius", LightRadius);
                    Mat.SetFloat("_PowFactor", LightPowFactor);

                    Mat.SetFloat("_LightSaturation", LightSaturation);
                    Mat.SetColor("_LightColor", LightColor);

                    int rtWidth = source.width >> DownSample;
                    int rtHeight = source.height >> DownSample;

                    RenderTexture temp1 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
					temp1.name = "CameraFX temp1";
                    RenderTexture temp2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
					temp2.name = "CameraFX temp2";
                    //Graphics.Blit(source, temp1, Mat, 0);
                    Graphics.Blit(source, temp1, Mat, 1);

                    float samplerOffset = SamplerScale / source.width;
                    for (int i = 0; i < BlurIteration; i++)
                    {

                        float offset = samplerOffset * (i * 2 + 1);
                        Mat.SetVector("_offsets", new Vector4(offset, offset, 0, 0));
                        Graphics.Blit(temp1, temp2, Mat, 2);

                        offset = samplerOffset * (i * 2 + 2);
                        Mat.SetVector("_offsets", new Vector4(offset, offset, 0, 0));
                        Graphics.Blit(temp2, temp1, Mat, 2);

                    }
                    Mat.SetTexture("_BlurTex", temp1);
                    //Mat.SetVector("_LightColor", lightColor);
                    Mat.SetFloat("_LightFactor", LightFactor);
                    if (LightDebug)
                    {
                        Graphics.Blit(temp1, destination);
                    }
                    else
                    {
                        Graphics.Blit(source, destination, Mat, 0);
                    }
                    RenderTexture.ReleaseTemporary(temp1);
                    RenderTexture.ReleaseTemporary(temp2);
                }
                else
                {
                    Mat.SetFloat("_LightFactor", 0);
                    Graphics.Blit(source, destination, Mat, 0);
                }
            }
            else*/
            {
                Mat.SetFloat("_LightFactor", 0);
                if (EnableRadiusBlur)
                {

                    //Graphics.Blit(source, mBlurRT, Mat, 0);
                    //Graphics.Blit(mBlurRT, mBlurRT2);
                    //Graphics.Blit(mBlurRT, destination);
                    //Mat.SetTexture("_BlurRT", mBlurRT2);
                    Mat.EnableKeyword("RADIUS_BLUR");
                    if (mRBForce < 1)
                    {
                        mRBForce += Time.deltaTime * 10f;
                    }
                    else
                    {
                        mRBForce = 1;
                    }
                    Mat.SetFloat("_RBForce", mRBForce);
                }
                else
                {
                    if (mRBForce < 0)
                    {
                        Mat.DisableKeyword("RADIUS_BLUR");
                    }
                    else
                    {
                        mRBForce -= Time.deltaTime * 2.5f;
                        Mat.SetFloat("_RBForce", mRBForce);
                    }
                    //Graphics.Blit(source, destination, Mat, 0);
                }
            }

            if (AutoExplosure)
            {
                Graphics.Blit(source, mAutoExplosureRTs[0]);
                for (int i = 1; i < mAutoExplosureRTs.Length; i++)
                {
                    Graphics.Blit(mAutoExplosureRTs[i - 1], mAutoExplosureRTs[i]);
                }
                Graphics.Blit(mAERTMemo, mAERTMemo1);
                Graphics.Blit(mAutoExplosureRTs[mAutoExplosureRTs.Length - 1], mAERTMemo, Mat, 1);                


            }

            //Zzc HorizontalBlur
            if (horizontalMat != null&&horizontalBlurActive&& size>0)
            {
                RenderTexture horizontalBlurRT = RenderTexture.GetTemporary(Screen.width >> horizontalDownSample, Screen.height >> verticalDownSample, 0, source.format);
                Graphics.Blit(source, horizontalBlurRT, Mat, 0);
                horizontalMat.SetFloat(id, size);
                Graphics.Blit(horizontalBlurRT, destination, horizontalMat, 0);
                RenderTexture.ReleaseTemporary(horizontalBlurRT);
            }
            else
            {
                Graphics.Blit(source, destination, Mat, 0);
            }

            //Graphics.Blit(source, destination, Mat, 0);

        }
        else
        {
            //Zzc HorizontalBlur
            if (horizontalMat != null && horizontalBlurActive&&size>0)
            {
                RenderTexture horizontalBlurRT = RenderTexture.GetTemporary(Screen.width >> horizontalDownSample, Screen.height >> verticalDownSample, 0, source.format);
                Graphics.Blit(source, horizontalBlurRT, Mat, 0);
                horizontalMat.SetFloat(id, size);
                Graphics.Blit(horizontalBlurRT, destination, horizontalMat, 0);
                RenderTexture.ReleaseTemporary(horizontalBlurRT);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
            //Graphics.Blit(source, destination);
        }
    }

    /// <summary>
    /// 互动水波
    /// </summary>
    private void PlayerAddRipple()
    {        
        if (PlayerPos)
        {
            if (!rfp)
            {
                rfp = PlayerPos.gameObject.AddComponent<RippleFollowPlayer>();
                if (rfp)
                {
                    if (ZzcFind.Instance.playerRippleRT && ZzcFind.Instance.playerRippleMat)
                    {
                        rfp.stepRT = ZzcFind.Instance.playerRippleRT;
                        rfp.stepMat = ZzcFind.Instance.playerRippleMat;
                    }
                }
            }
        }
    }
}
