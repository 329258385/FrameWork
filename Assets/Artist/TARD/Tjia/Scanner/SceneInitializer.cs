using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneInitializer : MonoBehaviour {

    //public Texture2D GridTex;
    public float GridRange = 0;
    public float WhiteModelRange = 0;
    public float Colorfy = 0;

    [Range(0, 1)] public float SkyGrid = 0;
    [Range(0, 1)] public float SkyColor = 0;

    public bool Begin = false;
    public bool DestroyAfterRunning = true;
    enum State {Run, Stop};

    State mSceneState = State.Stop;

    public Vector2 GridStartDuration = new Vector2(0, 4);
    public Vector2 WhiteStartDuration = new Vector2(2, 4);
    public Vector2 ColorfyStartDuration = new Vector2(4, 4);
    public Vector2 SkyGridStartDuration = new Vector2(7, 2);
    public Vector2 SkyColorStartDuration = new Vector2(8, 2);
    private float Timer = 0;

    private float ViewDis = 520;

    private Camera mCam;

    //private List<GameObject> Particles = new List<GameObject>();
    //private List<int> ParticleLayers = new List<int>();


    void Start () {
        GridRange = 0;
        WhiteModelRange = 0;
        Colorfy = 0;
        SkyGrid = 0;
        SkyColor = 0;

        mCam = GetComponent<Camera>();

        if (Application.isPlaying)
        {
            Shader.DisableKeyword("SCENE_INIT");
        }
        else
        {
            Shader.EnableKeyword("SCENE_INIT");
        }

        //foreach (ParticleSystem ps in FindObjectsOfType<ParticleSystem>())
        //{
        //    if (!Particles.Contains(ps.gameObject))
        //    {
        //        Particles.Add(ps.gameObject);
        //    }
        //}
        
    }

    void OnDisable()
    {
        Shader.DisableKeyword("SCENE_INIT");
        if(mCam != null) mCam.useOcclusionCulling = true;
    }

    void OnEnable()
    {
        if (Application.isPlaying)
        {
            Shader.DisableKeyword("SCENE_INIT");
        }
        else
        {
            Shader.EnableKeyword("SCENE_INIT");
        }
    }
	
	// Update is called once per frame
	void Update() {

        float dTime;

        if (SkyGrid < 0.999f || SkyColor < 0.999f || GridRange < ViewDis * 1.4985f || WhiteModelRange < ViewDis * 1.4985f || Colorfy < ViewDis * 1.4985f)
        {
            Shader.EnableKeyword("SCENE_INIT");
            Shader.EnableKeyword("TJIA_POINT_LIGHT");
            if (GridRange < ViewDis * 1.4985f)
            {
                mCam.useOcclusionCulling = false;
            }
            else
            {
                mCam.useOcclusionCulling = true;
            }
        }

        if (!Begin && mSceneState == State.Stop)
        {
            Shader.SetGlobalFloat("_WhiteModelRange", WhiteModelRange);
            Shader.SetGlobalFloat("_Colorfy", Colorfy);
            Shader.SetGlobalFloat("_GridRange", GridRange);
            Shader.SetGlobalFloat("_SkyGrid", SkyGrid);
            Shader.SetGlobalFloat("_SkyColor", SkyColor);
            //Shader.SetGlobalTexture("_GridTex", GridTex);
        }

        switch (mSceneState)
        {
            case State.Stop :
                if (Begin)
                {
                    Shader.EnableKeyword("SCENE_INIT");
                    Begin = false;
                    mSceneState = State.Run;
                    GridRange = 0;
                    WhiteModelRange = 0;
                    Colorfy = 0;
                    SkyGrid = 0;
                    SkyColor = 0;
                    Timer = Time.time;
                    Shader.SetGlobalFloat("_WhiteModelRange", WhiteModelRange);
                    Shader.SetGlobalFloat("_Colorfy", Colorfy);
                    Shader.SetGlobalFloat("_GridRange", GridRange);
                    Shader.SetGlobalFloat("_SkyGrid", SkyGrid);
                    Shader.SetGlobalFloat("_SkyColor", SkyColor);
                    mCam.useOcclusionCulling = false;
                    ViewDis = mCam.farClipPlane * 2f;
                    mCam.cullingMask &= ~(1 << 1); // Cull Transparent FX
                    //for (int i = 0; i < Particles.Count; i++)
                    //{
                    //    if (Particles[i] != null)
                    //    {
                    //        ParticleLayers.Add(Particles[i].layer);
                    //        Particles[i].layer = LayerMask.NameToLayer("DoNotRenderer");
                    //    }
                    //    else
                    //    {
                    //        ParticleLayers.Add(999);
                    //    }
                    //}
                }
                break;
            case State.Run:
                if (Begin)
                {
                    mSceneState = State.Stop;
                }
                dTime = Time.time - Timer;
                if (dTime > GridStartDuration.x)
                {
                    float gDTime = dTime - GridStartDuration.x;
                    if (gDTime < GridStartDuration.y)
                    {
                        GridRange = ViewDis * Mathf.Pow(gDTime / GridStartDuration.y, 2);
                        Shader.SetGlobalFloat("_GridRange", GridRange);
                    }
                    else
                    {
                        GridRange = ViewDis * 1.5f;
                        Shader.SetGlobalFloat("_GridRange", GridRange);
                        mCam.useOcclusionCulling = true;
                    }
                }
                if (dTime > WhiteStartDuration.x)
                {
                    float gDTime = dTime - WhiteStartDuration.x;
                    if (gDTime < WhiteStartDuration.y)
                    {
                        WhiteModelRange = ViewDis * Mathf.Pow(gDTime / WhiteStartDuration.y, 2);
                        Shader.SetGlobalFloat("_WhiteModelRange", WhiteModelRange);
                    }
                    else
                    {
                        WhiteModelRange = ViewDis * 1.5f;
                        Shader.SetGlobalFloat("_WhiteModelRange", WhiteModelRange);
                    }
                }
                if (dTime > ColorfyStartDuration.x)
                {
                    float gDTime = dTime - ColorfyStartDuration.x;
                    if (gDTime < ColorfyStartDuration.y)
                    {
                        Colorfy = ViewDis * Mathf.Pow(gDTime / ColorfyStartDuration.y, 2);
                        Shader.SetGlobalFloat("_Colorfy", Colorfy);
                    }
                    else
                    {
                        Colorfy = ViewDis * 1.5f;
                        Shader.SetGlobalFloat("_Colorfy", Colorfy);
                    }
                }
                if (dTime > SkyGridStartDuration.x)
                {
                    float gDTime = dTime - SkyGridStartDuration.x;
                    if (gDTime < SkyGridStartDuration.y)
                    {
                        SkyGrid = 1f * Mathf.Pow(gDTime / SkyGridStartDuration.y, 2);
                        Shader.SetGlobalFloat("_SkyGrid", SkyGrid);
                    }
                    else
                    {
                        SkyGrid = 1f;
                        Shader.SetGlobalFloat("_SkyGrid", SkyGrid);
                    }
                }
                if (dTime > SkyColorStartDuration.x)
                {
                    float gDTime = dTime - SkyColorStartDuration.x;
                    if (gDTime < SkyColorStartDuration.y)
                    {
                        SkyColor = 1f * Mathf.Pow(gDTime / SkyColorStartDuration.y, 2);
                        Shader.SetGlobalFloat("_SkyColor", SkyColor);
                    }
                    else
                    {
                        SkyColor = 1f;
                        Shader.SetGlobalFloat("_SkyColor", SkyColor);
                    }
                }
                if (dTime > ColorfyStartDuration.x + ColorfyStartDuration.y && dTime > SkyColorStartDuration.x + SkyColorStartDuration.y)
                {
                    //for (int i = 0; i < Particles.Count; i++)
                    //{
                    //    if (Particles[i] != null)
                    //    {
                    //        Particles[i].layer = ParticleLayers[i];
                    //    }
                    //}
                    mCam.cullingMask |= (1 << 1); //Open Transparent FX
                    mSceneState = State.Stop;
                    Begin = false;
                    Shader.DisableKeyword("SCENE_INIT");
                    if (DestroyAfterRunning && Application.isPlaying)
                    {
                        Destroy(this);
                    }
                }

                break;
        }
    }
}
