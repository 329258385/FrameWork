using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class LightmapColorPicker : MonoBehaviour {

    private Shader LmShader;
    //private Shader LmShadowShader;
    private Shader LmGrassInfoShader;
    private Camera mCamera;
    private Transform mMainCam;
    public float UpdatePeriod = 0.1f;
    private Ray mRay;
    private float mTimer = 0;
    private RaycastHit mRayInfo;
    private Transform mLmObj;
    private List<Material> mMats;
    private Vector2 mPoint;
    private int mXSize, mYSize;
    public RenderTexture LmColorRT;
    //public RenderTexture LmShadowRT;
    public RenderTexture GrassInfoRT;
    private Texture2D mTex;
    private Vector3 mPosMemo;
    private Vector3 mTmpVec;
    Rect mRect;

    internal bool GlobalView = false;
    private float mDistance = 150f;

    // Use this for initialization
    void Start () {
        Init();
    }

    void Init()
    {

#if BUILD_SINGLESCNE_MODE
        // test
        mMainCam = GameObject.Find("客户端测试节点_请勿删除/saoUIJoyPadCom/Camera").transform;          
#else
        mMainCam = Camera.main.transform;
#endif
        mCamera = GetComponent<Camera>();

        if (GlobalView)
        {
            mDistance = 750;
        }

        mCamera.orthographicSize = mDistance;
        Shader.SetGlobalFloat("_LmScale", mDistance * 2);
        LmShader = UnityUtils.FindShader("Hidden/LmColorShader");
        //LmShadowShader = UnityUtils.FindShader("Hidden/LmShadowShader");
        LmGrassInfoShader = UnityUtils.FindShader("Hidden/TJiaGrassInfoCollector");
        Shader.SetGlobalTexture("_LmColor", LmColorRT);
        //Shader.SetGlobalTexture("_LmShadowTex", LmShadowRT);
        Shader.SetGlobalTexture("_GrassInfoTex", GrassInfoRT);
        mTmpVec = mMainCam.position;
        mTmpVec.y = 0;
        if (mMainCam != null)
        {
            float lodBiasMemo = QualitySettings.lodBias;
            QualitySettings.lodBias = 10000;
            mPosMemo = mMainCam.position;
            mPosMemo.y = 0;
            transform.position = mMainCam.position + Vector3.up * 5;
            mTimer += UpdatePeriod;

            Shader.SetGlobalVector("_LmCamPos", mMainCam.position);

            mCamera.targetTexture = LmColorRT;
            mCamera.RenderWithShader(LmShader, "");
            //mCamera.targetTexture = LmShadowRT;
            //mCamera.RenderWithShader(LmShadowShader, "RenderType");
            mCamera.targetTexture = GrassInfoRT;
            mCamera.RenderWithShader(LmGrassInfoShader, "GrassTaker");

            QualitySettings.lodBias = lodBiasMemo;

        }
    }
	
	// Update is called once per frame
	void Update() {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            if (mMainCam != null)
            {
                float lodBiasMemo = QualitySettings.lodBias;
                QualitySettings.lodBias = 10000;
                mPosMemo = mMainCam.position;
                mPosMemo.y = 0;
                transform.position = mMainCam.position + Vector3.up * 20;
                mTimer += UpdatePeriod;

                Shader.SetGlobalVector("_LmCamPos", mMainCam.position);

                mCamera.targetTexture = LmColorRT;
                mCamera.RenderWithShader(LmShader, "");
                //mCamera.targetTexture = LmShadowRT;
                //mCamera.RenderWithShader(LmShadowShader, "RenderType");
                mCamera.targetTexture = GrassInfoRT;
                mCamera.RenderWithShader(LmGrassInfoShader, "GrassTaker");

                QualitySettings.lodBias = lodBiasMemo;
            }
            return;
        }
#endif
        mTmpVec = mMainCam.position;
        mTmpVec.y = 0;
        if (mMainCam == null)
        {
            Init();
        }        
        else if (mMainCam != null && Time.time > mTimer && Vector3.Distance(mPosMemo, mTmpVec) > mDistance * 0.2f)
        {
            float lodBiasMemo = QualitySettings.lodBias;
            QualitySettings.lodBias = 10000;
            mPosMemo = mMainCam.position;
            mPosMemo.y = 0;
            transform.position = mMainCam.position + Vector3.up * 20;
            mTimer += UpdatePeriod;

            Shader.SetGlobalVector("_LmCamPos", mMainCam.position);

            mCamera.targetTexture = LmColorRT;
            mCamera.RenderWithShader(LmShader, "");
            //mCamera.targetTexture = LmShadowRT;
            //mCamera.RenderWithShader(LmShadowShader, "RenderType");
            mCamera.targetTexture = GrassInfoRT;
            mCamera.RenderWithShader(LmGrassInfoShader, "GrassTaker");

            QualitySettings.lodBias = lodBiasMemo;
        }
    }
}
