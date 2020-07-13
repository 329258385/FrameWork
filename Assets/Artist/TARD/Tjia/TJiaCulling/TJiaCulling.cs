using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
struct ParticleBuffer
{
    public Vector3 pos;
    public int playMode;//0 play 1 pause 2 stop
};

struct ObjectBuffer
{
    public Vector3 pos;
    public float size;
    public int hideMode;//0 show 1 hide
};

[ExecuteInEditMode]
public class TJiaCulling : MonoBehaviour
{
    public bool RunInEditor = true;
    private bool mInitialized = false;
    private List<ParticleSystem> mPSL = new List<ParticleSystem>();
    private List<MeshRenderer> mMRL = new List<MeshRenderer>();
    public ComputeShader ParticleCullingShader;
    public ComputeShader ObjectCullingShader;
    public bool DistanceBasedParticleCull = true;
    public float ParticleMinDis = 20;
    public float ParticlePauseDis = 60;
    public float ParticleStopDis = 80;
    public bool ViewAngleBasedObjCull = true;
    [Range(0, 179)] public float HideAngle = 2.5f;

    private ComputeBuffer mParticleCBuffer;
    private ComputeBuffer mObjectCBuffer;

    private int ParticleQuantityPerFrame = 300;
    private int ObjectQuantityPerFrame = 600;

    private int mPCpt = 0;
    private int mOCpt = 0;
    //private Transform mMainCamPos;

    //public float Timer;

    // Use this for initialization
    void Awake()
    {
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
        RunInEditor = false;
#if UNITY_EDITOR
        //RunInEditor = false;
#endif
        if (!SystemInfo.supportsComputeShaders || !RunInEditor)
        {
            DistanceBasedParticleCull = false;
            ViewAngleBasedObjCull = false;
            Destroy(this);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
    }

    void OnDisable()
    {
        mInitialized = false;
        if (DistanceBasedParticleCull)
        {
            ParticleCullOff();
        }
        if (ViewAngleBasedObjCull)
        {
            ObjectCullOff();
        }
    }

    private void ParticleCullOff()
    {
        if (mParticleCBuffer != null)
        {
            mParticleCBuffer.Release();
        }
        for (int i = 0; i < mPSL.Count; i++)
        {
            if (mPSL[i] != null)
            {
                mPSL[i].Play();
            }
        }
    }

    private void ObjectCullOff()
    {
        if (mObjectCBuffer != null)
        {
            mObjectCBuffer.Release();
        }
        for (int i = 0; i < mMRL.Count; i++)
        {
            if (mMRL[i] != null)
            {
                mMRL[i].enabled = true;
            }
        }
    }

    // Update is called once per frame
    void OnPreCull()
    {
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
        if (Time.time > 1 && !mInitialized)
        {
            if (DistanceBasedParticleCull)
            {
                InitPSL();
            }
            if (ViewAngleBasedObjCull)
            {
                InitMRL();
            }
            //Debug.LogError(mPSL.Count);
            //Debug.LogError(mMRL.Count);
        }
        if (mInitialized)
        {
            if (DistanceBasedParticleCull)
            {
                DispatchPCB();
                CullPCB();
            }
            if (ViewAngleBasedObjCull)
            {
                DispatchOCB();
                CullOCB();
            }
        }
    }

    private void CullPCB()
    {
        int count = mPSL.Count;
        ParticleBuffer[] pba = new ParticleBuffer[count];
        mParticleCBuffer.GetData(pba);
        int endNum = mPCpt + ParticleQuantityPerFrame > count ? count : mPCpt + ParticleQuantityPerFrame;
        for (int i = mPCpt; i < endNum; i++)
        {
            if (mPSL[i] != null)
            {
                if (pba[i].playMode == 0)
                {
                    mPSL[i].Play();
                }
                else if (pba[i].playMode == 1)
                {
                    mPSL[i].Pause();
                }
                else if (pba[i].playMode == 2)
                {
                    if (mPSL[i].isPaused)
                    {
                        mPSL[i].Play();
                    }
                    mPSL[i].Stop();
                }
            }
        }
        mPCpt = mPCpt + ParticleQuantityPerFrame > count ? 0 : mPCpt + ParticleQuantityPerFrame;
    }

    private void CullOCB()
    {
        int count = mMRL.Count;
        ObjectBuffer[] oba = new ObjectBuffer[count];
        mObjectCBuffer.GetData(oba);
        int endNum = mOCpt + ObjectQuantityPerFrame > count ? count : mOCpt + ObjectQuantityPerFrame;
        for (int i = mOCpt; i < endNum; i++)
        {
            if (mMRL[i] != null)
            {
                if (oba[i].hideMode == 0)
                {
                    if (!mMRL[i].enabled)
                    {
                        mMRL[i].enabled = true;
                    }
                }
                else if (oba[i].hideMode == 1)
                {
                    if (mMRL[i].enabled)
                    {
                        mMRL[i].enabled = false;
                    }
                }
            }
        }
        mOCpt = mOCpt + ObjectQuantityPerFrame > count ? 0 : mOCpt + ObjectQuantityPerFrame;
    }

    private void DispatchPCB()
    {
        //Debug.Log(2);
        ParticleCullingShader.SetFloat("_MinDis", ParticleMinDis);
        ParticleCullingShader.SetFloat("_PauseDis", ParticlePauseDis);
        ParticleCullingShader.SetFloat("_StopDis", ParticleStopDis);
        ParticleCullingShader.SetVector("_MainCamPos", transform.position);
        ParticleCullingShader.SetVector("_MainCamDir", transform.forward);
        int kid = ParticleCullingShader.FindKernel("CSMain");
        ParticleCullingShader.SetBuffer(kid, "buffer", mParticleCBuffer);
        ParticleCullingShader.Dispatch(kid, 16, 16, 1);
    }

    private void DispatchOCB()
    {
        ObjectCullingShader.SetVector("_MainCamPos", transform.position);
        ObjectCullingShader.SetVector("_MainCamDir", transform.forward);
        ObjectCullingShader.SetFloat("_TanHideAngle", Mathf.Tan(HideAngle * 0.5f * Mathf.Deg2Rad));
        int kid = ObjectCullingShader.FindKernel("CSMain");
        ObjectCullingShader.SetBuffer(kid, "buffer", mObjectCBuffer);
        ObjectCullingShader.Dispatch(kid, 16, 16, 1);
    }

    private void InitPSL()
    {
        mPSL.Clear();
        ParticleSystem[] psa = FindObjectsOfType<ParticleSystem>();
        if (psa.Length == 0)
        {
            DistanceBasedParticleCull = false;
            return;
        }
        for (int i = 0; i < psa.Length; i++)
        {
            if (psa[i].isPlaying)
            {
                mPSL.Add(psa[i]);
            }
        }
        CreateParticleBuffer(mPSL.Count);
        mInitialized = true;
    }

    private void InitMRL()
    {
        mMRL.Clear();
        MeshRenderer[] mra = FindObjectsOfType<MeshRenderer>();
        if (mra.Length == 0)
        {
            ViewAngleBasedObjCull = false;
            return;
        }
        for (int i = 0; i < mra.Length; i++)
        {
            if (mra[i].enabled && Vector3.Magnitude(mra[i].bounds.extents) < 8.33f)
            {
                mMRL.Add(mra[i]);
            }
        }
        CreateObjectBuffer(mMRL.Count);
        mInitialized = true;
    }

    private void CreateParticleBuffer(int count)
    {
        if (mParticleCBuffer != null)
        {
            mParticleCBuffer.Release();
        }
        mParticleCBuffer = new ComputeBuffer(count, sizeof(float) * 3 + sizeof(int));
        ParticleBuffer[] buffers = new ParticleBuffer[count];
        for (int i = 0; i < count; i++)
        {
            InitParticleBuffer(ref buffers[i], i);
        }
        mParticleCBuffer.SetData(buffers);
    }

    private void CreateObjectBuffer(int count)
    {
        if (mObjectCBuffer != null)
        {
            mObjectCBuffer.Release();
        }
        mObjectCBuffer = new ComputeBuffer(count, sizeof(float) * 3 + sizeof(float) + sizeof(int));
        ObjectBuffer[] buffers = new ObjectBuffer[count];
        for (int i = 0; i < count; i++)
        {
            InitObjectBuffer(ref buffers[i], i);
        }
        mObjectCBuffer.SetData(buffers);
    }

    private void InitParticleBuffer(ref ParticleBuffer pBuffer, int index)
    {
        pBuffer = new ParticleBuffer();
        pBuffer.pos = mPSL[index].transform.position;
        pBuffer.playMode = 0;
    }

    private void InitObjectBuffer(ref ObjectBuffer pBuffer, int index)
    {
        pBuffer = new ObjectBuffer();
        pBuffer.pos = mMRL[index].transform.position;
        pBuffer.size = Vector3.Magnitude(mMRL[index].bounds.extents);
        pBuffer.hideMode = 0;
    }
}
