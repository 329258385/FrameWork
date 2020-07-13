using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TJiaPointLightManager : MonoBehaviour {

    public List<TJiaPointLight> PointLights;
    public bool UpdateLights = false;
    private Vector4[] mPosAndRange;
    private Vector4[] mColorAndIntendity;
    private List<Transform> mPointTrans;
    private Vector4 mVectorBuffer;

    private TJiaPointLight[] mPixelLights = new TJiaPointLight[4];
    private TJiaPointLight[] mVertexLights = new TJiaPointLight[16];
    private Vector4[] mPPosAndRange;
    private Vector4[] mPColorAndIntendity;
    private Vector4[] mVPosAndRange;
    private Vector4[] mVColorAndIntendity;

    private Transform mCam;

    void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {

        Init();
    }

    private void Init()
    {
        Shader.SetGlobalFloat("_HeroLight", 0);
        mPosAndRange = new Vector4[16];
        mColorAndIntendity = new Vector4[16];
        mPointTrans = new List<Transform>();
        if (PointLights != null && PointLights.Count != 0)
        {
            foreach (TJiaPointLight lit in PointLights)
            {
                if (lit != null)
                {
                    mPointTrans.Add(lit.transform);
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (UpdateLights)
        {
            UpdateLights = false;
            Init();
        }
        if (PointLights != null)
        {
            SortLights();

            int lightNum = PointLights.Count;
            if (lightNum != 0)
            {
                Shader.EnableKeyword("TJIA_POINT_LIGHT");
                if (mPosAndRange == null)
                {
                    Init();
                }
                if (lightNum < 5)
                {
                    SetPixelLights(lightNum);
                }
                else
                {
                    SetPixelLights(4);

                    lightNum -= 4;
                    if (lightNum > mVertexLights.Length)
                    {
                        lightNum = mVertexLights.Length;
                    }

                    SetVertexLights(lightNum);
                }
            }
            else
            {
                Shader.SetGlobalInt("_PointLightNumber", 0);
                Shader.DisableKeyword("TJIA_POINT_LIGHT");
            }
        }
    }

    private void SetPixelLights(int lightNum)
    {
        float heroLight = 0;
        for (int i = 0; i < lightNum; i++)
        {
            if (mPixelLights[i] != null)
            {
                mVectorBuffer = mPixelLights[i].transform.position;
                mVectorBuffer.w = mPixelLights[i].range;
                mPosAndRange[i] = mVectorBuffer;

                mVectorBuffer = mPixelLights[i].color;
                mVectorBuffer.w = mPixelLights[i].intensity;
                mColorAndIntendity[i] = mVectorBuffer;

                if(mPixelLights[i].HeroLight)
                {
                    heroLight += 1;
                }
            }
        }
        Shader.SetGlobalFloat("_HeroLight", heroLight);
        Shader.SetGlobalVectorArray("_PointLightPos1", mPosAndRange);
        Shader.SetGlobalVectorArray("_LightProperties1", mColorAndIntendity);
        Shader.SetGlobalInt("_PointLightNumber", lightNum < 4 ? lightNum : 4);
        Shader.SetGlobalInt("_VPointLightNumber", 0);
    }
    private void SetVertexLights(int lightNum)
    {
        for (int i = 0; i < lightNum; i++)
        {
            if (mVertexLights[i] != null)
            {
                mVectorBuffer = mVertexLights[i].transform.position;
                mVectorBuffer.w = mVertexLights[i].range;
                mPosAndRange[i] = mVectorBuffer;

                mVectorBuffer = mVertexLights[i].color;
                mVectorBuffer.w = mVertexLights[i].intensity;
                mColorAndIntendity[i] = mVectorBuffer;
            }
        }
        Shader.SetGlobalVectorArray("_VPointLightPos1", mPosAndRange);
        Shader.SetGlobalVectorArray("_VLightProperties1", mColorAndIntendity);
        Shader.SetGlobalInt("_VPointLightNumber", lightNum < 16 ? lightNum : 16);
    }

    private void SortLights()
    {
        float minDis = 99999;
        float minLimit = 0;
        if (Camera.main)
        {
            mCam = Camera.main.transform;
        }        
        if (!mCam)
        {
            mCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        for (int j = 0; j < mPixelLights.Length; j++)
        {
            mPixelLights[j] = null;
            for (int i = 0; i < PointLights.Count; i++)
            {
                float dis = Vector3.Distance(mPointTrans[i].position, mCam.position);
                if (dis < minDis && dis > minLimit)
                {
                    minDis = dis;
                    mPixelLights[j] = PointLights[i];
                }
            }
            minLimit = minDis;
            minDis = 99999;
        }
        for (int j = 0; j < mVertexLights.Length; j++)
        {
            mVertexLights[j] = null;
            for (int i = 0; i < PointLights.Count; i++)
            {
                float dis = Vector3.Distance(mPointTrans[i].position, mCam.position);
                if (dis < minDis && dis > minLimit)
                {
                    minDis = dis;
                    mVertexLights[j] = PointLights[i];
                }
            }
            minLimit = minDis;
            minDis = 99999;
        }
    }

    private void OnEnable()
    {
        Shader.EnableKeyword("TJIA_POINT_LIGHT");
    }

    private void OnDisable()
    {
        Shader.DisableKeyword("TJIA_POINT_LIGHT");
    }
}
