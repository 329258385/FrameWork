using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using LuaInterface;

public class RenderTargetScaler : MonoBehaviour {

    [Header("使用动态屏幕分辨率")]
    public bool UseScreenScaler = false;
    [Header("自动分辨率")]
    public bool AutoScale = true;

    [Range(0.1f, 1)]public float Scale = 0.7f;
    private float mCachedScale = -1;
    private Camera mCam;
    private Camera mBackBufferCam;
    internal RenderTexture mFrameBuffer;

    [NoToLua]
    public Material OneToOneMat;

    private bool mUseScreenScaler;

    private bool Initialized = false;
    private FrameTiming mFT;
    private float mTiming;

    public float autoMinValue;
    public float autoMaxValue;

    void Start ()
    {
        if (UseScreenScaler)
        {
            Initialized = true;
            Init();
            Setup();
        }
    }

    void OnDisable()
    {
        Stop();
    }

    private void Stop()
    {
        if (mCam != null)
        {
            mCam.targetTexture = null;
            if (mFrameBuffer != null)
            {
                mFrameBuffer.Release();
                Destroy(mFrameBuffer);
                //RenderTexture.ReleaseTemporary(mFrameBuffer);
                mFrameBuffer = null;
            }
            mBackBufferCam.gameObject.SetActive(false);
            mCachedScale = Scale;
        }
    }

    void OnEnable()
    {
        Restart();
    }

    private void Restart()
    {
        if (mCam != null)
        {
            mBackBufferCam.gameObject.SetActive(true);
        }
        mCachedScale = -1;
    }

    private void Setup()
    {
        mCachedScale = Scale;
        if (mFrameBuffer != null)
        {
            mFrameBuffer.Release();
            Destroy(mFrameBuffer);
            //RenderTexture.ReleaseTemporary(mFrameBuffer);
            mFrameBuffer = null;
        }
        var width = (int)((float)Screen.width * Scale);
        var height = (int)((float)Screen.height * Scale);

        mFrameBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        //mFrameBuffer = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        mFrameBuffer.name = "Resize RT";
        mFrameBuffer.useMipMap = false;
        mFrameBuffer.filterMode = FilterMode.Bilinear;
        mFrameBuffer.useDynamicScale = false;
        mFrameBuffer.Create();
        mCam.targetTexture = mFrameBuffer;
        OneToOneMat.SetTexture("_GameTex", mFrameBuffer);
        Debug.Log(">> RenderTargetScaler.Setup: " + width + ", " + height + ", sw=" + Screen.width + ", sh=" + Screen.height + ", scale=" + Scale + ", currentrw=" + Screen.currentResolution.width + ", currentrh=" + Screen.currentResolution.height);
    }

    void Update()
    {
        if (UseScreenScaler && !Initialized)
        {
            Initialized = true;
            Init();
            Setup();
        }
        if (mUseScreenScaler != UseScreenScaler)
        {
            mUseScreenScaler = UseScreenScaler;
            if (UseScreenScaler)
            {
                Restart();
            }
            else
            {
                Stop();
            }
        }
        if (UseScreenScaler && AutoScale)
        {
            AutoResizer();
        }
        if (mCachedScale != Scale && UseScreenScaler)
        {
            if (Scale == 1.0)
            {
                Stop();
            }
            else
            {
                if (mCachedScale == 1)
                {
                    Restart();
                }
                Setup();
            }
        }
    }

    private void AutoResizer()
    {
        mTiming = mTiming * 0.99f + Time.deltaTime * 1000f * 0.01f;// 移动平均
        if (mTiming > 1000) // FPS01
        {
            Scale = 0.3f;
        }
        else if (mTiming > 200) // FPS05
        {
            Scale = 0.4f;
        }
        else if (mTiming > 100) // FPS10
        {
            Scale = 0.5f;
        }
        else if (mTiming > 66.66) // FPS15
        {
            Scale = 0.6f;
        }
        else if (mTiming > 50.00) // FPS20
        {
            Scale = 0.7f;
        }
        else if (mTiming > 40.00) // FPS25
        {
            Scale = 0.8f;
        }
        else if (mTiming > 34.48) // FPS29
        {
            Scale = 0.9f;
        }
        else
        {
            Scale = 1;
        }
        if (Scale < autoMinValue)
            Scale = autoMinValue;
        else if (Scale > autoMaxValue)
            Scale = autoMaxValue;
        //if (!GlobalGameDefine.mIsNeedAutoScaleScreenRT)
        //    Scale = GlobalGameDefine.fAdapteScaleScreenRTValue;// 直接使用一个合理的值, 避免rt动态创建无法释放的开销
        //Debug.Log(mTiming);
    }

    //
    // Set RenderTarget Scaler Manually
    //
    public void SetRenderTargetScaleManual(float pscale)
    {
        AutoScale = false;
        Scale = pscale;
    }

    public float GetRenderTargetScale()
    {
        return this.mCachedScale;
    }

    private void Init()
    {
        mCam = GetComponent<Camera>();
        mCam.allowDynamicResolution = true;
        var backCamGO = new GameObject("Back Buffer Camera");
        mBackBufferCam = backCamGO.AddComponent<Camera>();
        mBackBufferCam.cullingMask = 0;
        mBackBufferCam.depth = mCam.depth;// copy depth config
        mBackBufferCam.transform.parent = transform;
        mBackBufferCam.clearFlags = CameraClearFlags.Nothing;
        mBackBufferCam.useOcclusionCulling = false;
        mBackBufferCam.allowHDR = false;
        mBackBufferCam.allowMSAA = false;
        mBackBufferCam.allowDynamicResolution = false;
        mBackBufferCam.depth = mCam.depth;

        var oto = mBackBufferCam.gameObject.AddComponent<OneToOne>();
        if (OneToOneMat == null)
        {
            OneToOneMat = new Material(UnityUtils.FindShader("Hidden/OneToOne"));
        }
        oto.OneToOneMat = OneToOneMat;
    }
}
