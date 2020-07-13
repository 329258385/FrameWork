using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TJiaBloom : MonoBehaviour
{
    [SerializeField] public bool FastBloom = false;
    private bool bFastBloom = false;

    [SerializeField] public float _Threshold = 1.2f;
    [SerializeField, Range(0, 1)] public float _SoftKnee = 0.5f;
    [SerializeField] public float _Intensity = 0.35f;
    [SerializeField, Range(0, 7)] public float _Radius = 4f;
    [SerializeField] public bool _AntiFlicker = true;
    [SerializeField, Range(0, 1)] public float _SoftFocus = 0.2f;
    [SerializeField, Range(0.0625f, 1)] public float _Resolution = 1f;

    public float ThresholdGamma
    {
        get { return Mathf.Max(0, _Threshold); }
        set { _Threshold = value; }
    }

    public float ThresholdLinear
    {
        get { return Mathf.GammaToLinearSpace(ThresholdGamma); }
        set { _Threshold = value; }
    }

    private const int cMaxPyramidBlurLevel = 16;
    readonly RenderTexture[] mBlurBuffer1 = new RenderTexture[cMaxPyramidBlurLevel];
    readonly RenderTexture[] mBlurBuffer2 = new RenderTexture[cMaxPyramidBlurLevel];
    private Material mBloomMat;
    private bool Initialized = false;
    private int mTw;
    private int mTh;
    private RenderTextureFormat mRTFormat;
    private int mIterations;
    private float mRadiusBuffer = -1;
    private bool mEnableUpdate = false;
    private float mThresholdBuffer = -1 - 1e-5f;
    private float mKneeBuffer = -1;
    private bool mAntiFlickerBuffer = true;
    private RenderTexture mPrefiltered;
    private float mSoftFocusBuffer = -1;
    private List<RenderTexture> TRTs = new List<RenderTexture>();
    private float mIntensityBuffer = -1;
    private float mResolutionBuffer = -1;
    private bool mBlock = false;

    private RenderTexture mTmpRT1;
    private RenderTexture mTmpRT2;

    void Awake()
    {
        if(GlobalGameDefine.mIsForceDisableAnyBloom)
        {
            this.enabled = false;
        }
        else
        {
            if (GlobalGameDefine.mIsHWDevice && GlobalGameDefine.mIsForceCloseBloomForHW) this.enabled = false;
        }
    }

    private void OnEnable()
    {
        //FastBloom = false;
        if (FastBloom)
        {
            EnableFastBloom();
        }
        else
        {
            EnableBloom();
        }
    }

    private void OnDisable()
    {
        DisableBloom();
        DisableFastBloom();
    }

    private void Initialization()
    {
        Initialized = true;
        bool useRGBM = Application.isMobilePlatform;
        mRTFormat = useRGBM ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
        mEnableUpdate = true;
        mRadiusBuffer = -1 + 1e-5f;
        mThresholdBuffer = -1 + 1e-5f;
        UpdateProperties();
    }

    private RenderTexture GetRT(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "TJia Bloom RT")
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw);
        rt.filterMode = filterMode;
        rt.wrapMode = wrapMode;
        rt.name = name;
        TRTs.Add(rt);
        return rt;
    }

    private void Update()
    {
        if (!FastBloom)
        {
            if (!mBlock && mEnableUpdate)
            {
                UpdateProperties();
            }
        }
        else
        {
            UpdateFastBloom();
        }
        if(FastBloom != bFastBloom)
        {
            bFastBloom = FastBloom;
            if (FastBloom)
            {
                DisableBloom();
                EnableFastBloom();
            }
            else
            {
                DisableFastBloom();
                EnableBloom();
            }
        }
    }

    private void UpdateFastBloom()
    {
        if (mTmpRT1 == null && mTw * mTh != 0)
        {
            bool useRGBM = Application.isMobilePlatform;
            RenderTextureFormat mRTFormat = useRGBM ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
            mTmpRT1 = GetRT(mTw / 02, mTh / 02, 0, mRTFormat);
            mTmpRT2 = GetRT(mTw / 04, mTh / 04, 0, mRTFormat);
            //mTmpRT3 = GetRT(mTw / 08, mTh / 08, 0, mRTFormat);
        }
        mBloomMat.SetFloat("_Threshold", _Threshold * 0.96f);
        mBloomMat.SetFloat("_Intensity", _Intensity * 2.143f);
    }

    private void EnableBloom()
    {
        // read it from cfg, when has value
        if (GlobalGameDefine.fBloomResolutionScale > 0) _Resolution = GlobalGameDefine.fBloomResolutionScale;// bloom resolution

        if (mBloomMat == null)
        {
            mBloomMat = new Material(UnityUtils.FindShader("Hidden/TJia/PFX/TjiaBloom"))
            {
                hideFlags = HideFlags.DontSave,
                shaderKeywords = null
            };
        }
        mBlock = false;
    }

    private void DisableFastBloom()
    {
        mBlock = true;
        if (mBloomMat != null)
        {
            DestroyImmediate(mBloomMat);
        }
        for (int i = 0; i < TRTs.Count; i++)
        {
            if (TRTs[i] != null)
            {
                RenderTexture.ReleaseTemporary(TRTs[i]);
            }
        }
        TRTs.Clear();
    }

    private void EnableFastBloom()
    {
        mBlock = false;
        if (mBloomMat == null)
        {
            mBloomMat = new Material(UnityUtils.FindShader("Hidden/TJia/PFX/TjiaNewBloom"))
            {
                hideFlags = HideFlags.DontSave,
                shaderKeywords = null
            };
        }
    }

    private void DisableBloom()
    {
        if (mBloomMat != null)
        {
            DestroyImmediate(mBloomMat);
        }
        Initialized = false;
        mEnableUpdate = false;
        mBlock = true;
        //Debug.Log("AAA " + Time.time);
        for (int i = 0; i < TRTs.Count; i++)
        {
            if (TRTs[i] != null)
            {
                RenderTexture.ReleaseTemporary(TRTs[i]);
                //Debug.Log(TRTs[i].name);
            }
        }
        TRTs.Clear();
        //Debug.Log("BBB " + Time.time);
    }

    private void UpdateProperties()
    {
        if (_Radius != mRadiusBuffer
            || mThresholdBuffer != _Threshold
            || mKneeBuffer != _SoftKnee
            || mAntiFlickerBuffer != _AntiFlicker
            || mSoftFocusBuffer != _SoftFocus
            || mIntensityBuffer != _Intensity
            || mResolutionBuffer != _Resolution)
        {
            for (int i = 0; i < TRTs.Count; i++)
            {
                if (TRTs[i] != null)
                {
                    RenderTexture.ReleaseTemporary(TRTs[i]);
                }
            }
            TRTs.Clear();

            mRadiusBuffer = _Radius;
            //Determine le compte de l'iteration
            float logh = Mathf.Log(mTh, 2f) + _Radius - 8;
            int logh_i = (int)logh;
            mIterations = Mathf.Clamp(logh_i, 1, cMaxPyramidBlurLevel);
            float sampleScale = 0.5f + logh - logh_i;
            mBloomMat.SetFloat("_SampleScale", sampleScale);

            mThresholdBuffer = _Threshold;
            mKneeBuffer = _SoftKnee;
            mAntiFlickerBuffer = _AntiFlicker;
            mSoftFocusBuffer = _SoftFocus;
            mIntensityBuffer = _Intensity;
            mResolutionBuffer = _Resolution;

            mBloomMat.SetFloat("_Intensity", Mathf.Max(_Intensity, 0));
            mBloomMat.SetFloat("_SoftFocus", _SoftFocus / (float)mIterations);
            mBloomMat.SetFloat("_Threshold", _Threshold);
            float knee = _Threshold * _SoftKnee + 1e-5f;
            Vector3 curve = new Vector3(_Threshold - knee, knee * 2, 0.25f / knee);
            mBloomMat.SetVector("_Curve", curve);
            mBloomMat.SetFloat("_PrefilterOffs", _AntiFlicker ? -0.5f : 0);
            if (_AntiFlicker)
            {
                mBloomMat.EnableKeyword("ANTI_FLICKER");
            }
            else
            {
                mBloomMat.DisableKeyword("ANTI_FLICKER");
            }
            mPrefiltered = GetRT(mTw, mTh, 0, mRTFormat, name: "TJia_Bloom_RT_Pre " + Time.time);
            RenderTexture last = mPrefiltered;
            for (int level = 0; level < mIterations; level++)
            {
                mBlurBuffer1[level] = GetRT(last.width / 2, last.height / 2, 0, mRTFormat, name: "TJia_Bloom_RT_Down " + level + " " + Time.time);
                last = mBlurBuffer1[level];
            }
            for (int level = mIterations - 2; level >= 0; level--)
            {
                RenderTexture baseTex = mBlurBuffer1[level];
                mBlurBuffer2[level] = GetRT(baseTex.width, baseTex.height, 0, mRTFormat, name: "TJia_Bloom_RT__Up " + level + " " + Time.time);
            }
            //Debug.Log(mIterations);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (bFastBloom != FastBloom)
        {
            return;
        }
        if (!mBlock)
        {
            if (!FastBloom)
            {
                //Commance par une resolution divisee par 2, car la resolution originale ne prends pas grande chose
                //mais donne une limitation a la performance
                mTw = (int)((float)source.width / 2 * _Resolution);
                mTh = (int)((float)source.height / 2 * _Resolution);

                if (!Initialized)
                {
                    Initialization();
                }

                Graphics.Blit(source, mPrefiltered, mBloomMat, 0);
                //Graphics.Blit(mPrefiltered, destination);
                RenderTexture last = mPrefiltered;
                //DownSample
                for (int level = 0; level < mIterations; level++)
                {
                    int pass = (level == 0) ? 1 : 2;
                    Graphics.Blit(last, mBlurBuffer1[level], mBloomMat, pass);
                    last = mBlurBuffer1[level];

                }
                //Graphics.Blit(last, destination);

                //Upsample and combime
                for (int level = mIterations - 2; level >= 0; level--)
                {
                    RenderTexture baseTex = mBlurBuffer1[level];
                    //Graphics.Blit(last, mBlurBuffer2[level]); //Progressive upsample is better than upsample directly
                    mBloomMat.SetTexture("_BaseTex", mBlurBuffer1[level]);
                    Graphics.Blit(last, mBlurBuffer2[level], mBloomMat, 3);
                    last = mBlurBuffer2[level];
                }
                //Graphics.Blit(last, destination);
                mBloomMat.SetTexture("_BaseTex", source);
                Graphics.Blit(last, destination, mBloomMat, 4);
            }
            else
            {
                mTw = source.width;
                mTh = source.height;

                //float knee = _Threshold * _SoftKnee + 1e-5f;
                //Vector3 curve = new Vector3(_Threshold - knee, knee * 2, 0.25f / knee);

                float gaussianForce = _Radius * 0.05f;

                mBloomMat.SetFloat("_GaussianScale", gaussianForce * 1.0f);
                Graphics.Blit(source, mTmpRT1, mBloomMat, 0);
                //mBloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
                mBloomMat.SetTexture("_OriginalTex", mTmpRT1);
                Graphics.Blit(mTmpRT1, mTmpRT2, mBloomMat, 3);
                //mBloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
                /*Graphics.Blit(mTmpRT2, mTmpRT3, mBloomMat, 1);
                mBloomMat.SetFloat("_GaussianScale", GussianForce * 0.0625f);
                mBloomMat.SetTexture("_OriginalTex", mTmpRT3);
                Graphics.Blit(mTmpRT3, mTmpRT2, mBloomMat, 3);*/
                //mBloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
                mBloomMat.SetTexture("_OriginalTex", mTmpRT2);
                Graphics.Blit(mTmpRT2, mTmpRT1, mBloomMat, 3);
                //mBloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
                mBloomMat.SetTexture("_OriginalTex", source);
                Graphics.Blit(mTmpRT1, destination, mBloomMat, 3);
            }
        }
    }
}
