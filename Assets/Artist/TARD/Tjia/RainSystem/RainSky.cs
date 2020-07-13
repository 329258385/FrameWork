using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RainSky : MonoBehaviour {

    public Transform CameraTrans;
    [Range(0, 1)]
    public float CloudyLevel = 0;
    private Vector3 mPosCache;
    private float mTransparencyCache = -1;
    private MeshRenderer mMeshRenderer;
    private Material mRainSkyMat;
    private bool mRendererActivation = false;
    //[Range(0, 1)] public float TimeParam;
    //public Gradient RainSkyColor;
    //internal Color CloudySkyColor;
    internal bool Is24Hours;

    private Transform RainBG;
    private bool Init = false;

    [Range(0, 1)]public float RainLevel = 0;
    private float bRainLevel = 0;
    private bool mRainBGActive = false;

    private Transform RainDrop;
    private ParticleSystem mPS;

    [Range(0, 1)] public float Wetness = 0;

    [Space(40)]
    //[Range(0, 1)] public float GlobalMetallic = 0;
    [Range(0, 1)] public float GlobalSmoothness = 0;
    [Range(0, 2)] public float GlobalSpecForce = 0;
    [Range(0, 1)] public float GlobalAlbedoReduce = 1;

    private float mValueBuffer = -1;

    void OnEnable()
    {
        mMeshRenderer = GetComponent<MeshRenderer>();
        mRainSkyMat = mMeshRenderer.sharedMaterial;
        transform.parent = null;
        if (CameraTrans != null)
        {
            transform.localScale = Vector3.one * CameraTrans.GetComponent<Camera>().farClipPlane * 1.99f;
        }

        foreach(RenderingController rc in FindObjectsOfType<RenderingController>())
        {
            if (rc.mRainSky == null)
            {
                rc.mRainSky = this;
            }
        }

        RainBG = transform.Find("RainBackground");
        RainDrop = RainBG.Find("RainDrop");
        mPS = RainDrop.GetComponent<ParticleSystem>();

        RainBG.parent = null;
        RainBG.localScale = new Vector3(20, 50, 20);
        RainBG.parent = transform;
        Init = false;

        Shader.EnableKeyword("_RAIN_RIPPLE_ON");
    }

    private void OnDestroy()
    {
        Shader.DisableKeyword("_RAIN_RIPPLE_ON");
    }


    void Update () {

        if (mValueBuffer != GlobalSpecForce + GlobalSmoothness + GlobalAlbedoReduce + Wetness)
        {
            mValueBuffer = GlobalSpecForce + GlobalSmoothness + GlobalAlbedoReduce + Wetness;
            Shader.SetGlobalVector("_SSR", new Vector3(
                Mathf.Lerp(1f, GlobalSpecForce, Wetness), 
                Mathf.Lerp(0f, GlobalSmoothness, Wetness), 
                Mathf.Lerp(1f, GlobalAlbedoReduce, Wetness)));
        }

        if (!Init)
        {
            Init = true;
            RainBG.localPosition = transform.InverseTransformVector(new Vector3(0, 45, 0));
            RainDrop.localPosition = new Vector3(0, -0.8f, 0);
        }

        if (CameraTrans != null)
        {
            Vector3 camPos = CameraTrans.position;
            if (mPosCache != camPos)
            {
                mPosCache = camPos;
                transform.position = camPos;
                transform.rotation = Quaternion.identity;
            }
        }
        if (CloudyLevel == 0)
        {
            if (mRendererActivation)
            {
                mRendererActivation = false;
                mMeshRenderer.enabled = false;
            }
        }
        else if (mTransparencyCache != CloudyLevel)
        {
            if (!mRendererActivation)
            {
                mRendererActivation = true;
                mMeshRenderer.enabled = true;
            }
#if !UNITY_EDITOR
            //mTransparencyCache = Transparency;
#endif
            mRainSkyMat.SetFloat("_CloudyLevel", CloudyLevel);
        }
        if (bRainLevel != RainLevel)
        {
            bRainLevel = RainLevel;
            if (RainLevel == 0)
            {
                mRainBGActive = false;
                RainBG.gameObject.SetActive(false);
            }
            else
            {
                if (!mRainBGActive)
                {
                    mRainBGActive = true;
                    RainBG.gameObject.SetActive(true);
                }
                float presentativeRainLevel = Mathf.Sqrt(RainLevel);
                Shader.SetGlobalFloat("_RainLevel", presentativeRainLevel);

                var emission = mPS.emission;
                emission.rateOverTime = 800 * RainLevel;
            }
        }


        Shader.SetGlobalFloat("_RainRippleNormal", RainLevel*3f);
    }
}
