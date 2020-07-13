using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBP_Controler : MonoBehaviour
{
    public bool RunInEditor = false;
    public bool OpenAllPSs = false;
    private List<ParticleSystem> mPSs;
    private int mIndex = 0;
    private Vector3 mPsPos;
    private Vector3 mCamPos;
    public float ParticleSysPauseDis = 60;
    public float ParticleSysStopDis = 80;
    public float ParticleSysVisibleViewAngle = 80;
    private int ParticleCalPerFrame = 4;
    private float ParticleSysMiniDis = 20;
    internal Transform mCam;

    private int MeshCalPerFrame = 25;
    private List<MeshRenderer> Meshes;
    [Range(0, 100)] public float HideAnglePercentage = 5;
    private float mHideAnglePercentage;
    private float mHideAngle;
    private int mMeshIndex = 0;
    private Dictionary<GameObject, int> MesheLayers;
    private GameObject mGOBuffer;

    // Use this for initialization
    void Start()
    {
        Destroy(this);
#if !UNITY_EDITOR
        //RunInEditor = true;
#endif
        mPSs = new List<ParticleSystem>();
        Meshes = new List<MeshRenderer>();
        MesheLayers = new Dictionary<GameObject, int>();
        InitPSs();
        InitMeshes();
        Invoke("InitPSs", 8);
        Invoke("InitMeshes", 12);
        //Shader.SetGlobalFloat("_InGame", 1);
        //HideAnglePercentage = 10;
        //StartCoroutine(RunParticleController());
    }

    private void InitMeshes()
    {
        if (RunInEditor)
        {
            foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
            {
                if (mr.gameObject.isStatic && mr.enabled == true && Meshes != null && !Meshes.Contains(mr))
                {
                    Meshes.Add(mr);
                    MesheLayers.Add(mr.gameObject, mr.gameObject.layer);
                }
            }
            foreach (VisualCullDisable vcd in FindObjectsOfType<VisualCullDisable>())
            {
                foreach (MeshRenderer mr in vcd.GetComponentsInChildren<MeshRenderer>())
                {
                    if (Meshes.Contains(mr))
                    {
                        Meshes.Remove(mr);
                        MesheLayers.Remove(mr.gameObject);
                    }
                }
            }
            mHideAngle = Mathf.Tan(Mathf.Deg2Rad * (HideAnglePercentage * 0.5f * 60f / 100f));
        }
    }

    private void InitPSs()
    {
        if (RunInEditor)
        {
            if (mCam == null)
            {
                mCam = Camera.main.transform;
            }
            mCam.GetComponent<Camera>().cullingMask &= ~(1 << 29);
            foreach (ParticleSystem ps in FindObjectsOfType<ParticleSystem>())
            {
                if (ps.GetComponent<Renderer>().enabled == false)
                {
                    Destroy(ps);
                }
                else
                {
                    var main = ps.main;
                    if (main.loop == true && mPSs != null && !mPSs.Contains(ps))
                    {
                        mPSs.Add(ps);
                    }
                }
            }
        }
    }
    private void OnEnable()
    {
        if (RunInEditor)
        {
            InitPSs();
            InitMeshes();
            mIndex = 0;
            mMeshIndex = 0;
        }
    }

    private void OnDisable()
    {
        if (RunInEditor)
        {
            for (int i = 0; i < Meshes.Count; i++)
            {
                if (Meshes[i] != null)
                {
                    mGOBuffer = Meshes[i].gameObject;
                    mGOBuffer.layer = MesheLayers[mGOBuffer];
                }
            }
            for (int i = 0; i < mPSs.Count; i++)
            {
                if (mPSs[i] != null)
                {
                    mPSs[i].Play();
                }
            }
        }
    }

    // Update is called once per frame
    void OnPreCull()
    //private IEnumerator RunParticleController()
    {
        //while (true)
        {
            if (RunInEditor)
            {
                for (int i = 0; i < MeshCalPerFrame; i++)
                {
                    int count = Meshes.Count;
                    if (count > 0)
                    {
                        if (mMeshIndex >= count - 1)
                        {
                            mMeshIndex = 0;
                        }
                        if (Meshes[mMeshIndex] == null)
                        {
                            Meshes.Remove(Meshes[mMeshIndex]);
                            mMeshIndex = (mMeshIndex + 1) % count;
                            break;
                        }
                        else
                        {
                            float dis = Vector3.Distance(mCam.position, Meshes[mMeshIndex].bounds.center);
                            float angle = Vector3.Magnitude(Meshes[mMeshIndex].bounds.extents) / dis;
                            if (mHideAnglePercentage != HideAnglePercentage)
                            {
                                mHideAnglePercentage = HideAnglePercentage;
                                mHideAngle = Mathf.Tan(Mathf.Deg2Rad * (HideAnglePercentage * 0.5f * 60f * 0.01f));
                            }
                            //Debug.Log(i + Meshes[i].name + "" + angle);
                            float coef = dis * 0.02f;
                            if (coef > 1)
                            {
                                coef = 1;
                            }
                            mGOBuffer = Meshes[mMeshIndex].gameObject;
                            if (dis > 20 && angle < mHideAngle * coef)
                            {
                                mGOBuffer.layer = 29;
                            }
                            else
                            {
                                mGOBuffer.layer = MesheLayers[mGOBuffer];
                            }
                        }
                        mMeshIndex = (mMeshIndex + 1) % count;
                    }
                }
                //yield return 0;
                for (int i = 0; i < ParticleCalPerFrame; i++)
                {
                    if (mPSs != null && mPSs.Count > 0)
                    {
                        if (mPSs[mIndex] != null)
                        {
                            mPsPos = mPSs[mIndex].transform.position;
                            mCamPos = mCam.position;
                            float dis = Vector3.Distance(mPsPos, mCamPos);
                            if (OpenAllPSs || dis < ParticleSysMiniDis)
                            {
                                mPSs[mIndex].Play();
                            }
                            else if (dis > ParticleSysStopDis)
                            {
                                if (mPSs[mIndex].isPaused == true)
                                {
                                    mPSs[mIndex].Play();
                                }
                                mPSs[mIndex].Stop();
                            }
                            else if (dis > ParticleSysPauseDis
                                || Vector3.Angle(mPsPos - mCamPos, mCam.forward) > ParticleSysVisibleViewAngle * 0.5)
                            {
                                mPSs[mIndex].Pause();
                            }
                            else
                            {
                                mPSs[mIndex].Play();
                            }
                        }
                        else
                        {
                            mPSs.Remove(mPSs[mIndex]);
                            mIndex = (mIndex + 1) % mPSs.Count;
                            //return;
                            break;
                        }
                        mIndex = (mIndex + 1) % mPSs.Count;
                    }
                }
                //yield return 5;
            }
            //yield return new WaitForEndOfFrame();
        }
    }
}
