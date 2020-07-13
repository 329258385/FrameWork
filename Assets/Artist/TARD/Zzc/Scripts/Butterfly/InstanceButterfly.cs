using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceButterfly : MonoBehaviour
{
    [Tooltip("蝴蝶Mesh")]
    public Mesh butterflyMesh;
    [Tooltip("蝴蝶材质")]
    public Material butterflyMaterial;
    [Tooltip("(ChunkPos中心点)离相机多远(这群物件)不显示")]
    public float cullDistance = 50f;
    [Tooltip("玩家Transform,运行游戏时自动获取,测试场景需自己指定,不然物体不显示")]
    public Transform player;
    public Color col1;
    public Color col2;
    public Color col3;
    public List<DrawInstancing.ButterFlyData> butterflyData;



    private List<Matrix4x4> mats;
    private List<float> randomIdleWingSpeed;
    private List<float> randomFlyingWingSpeed;
    private List<float> randomWingSinStart;
    private List<float> idleWingSpeeds;
    private List<float> gpuBirdStates;
    private List<float> sinFactor1s;
    private List<float> sinFactor2s;
    private List<float> randomFlyingSinStarts;
    private List<Vector4> butterflyColors;

    private List<float> butterflyColors1;
    private List<float> butterflyColors2;
    private List<float> butterflyColors3;
    private List<float> butterflyColors4;

    private int randomFlyingSinStartIndex;
    private int sinFactor1Index;
    private int sinFactor2Index;
    private int randomWingSinStartIndex;
    private int idleWingSpeedIndex;
    private int gpuBirdStateIndex;
    private int tilingOffsetIndex;

    private int tilingOffsetIndex1;
    private int tilingOffsetIndex2;
    private int tilingOffsetIndex3;
    private int tilingOffsetIndex4;

    private MaterialPropertyBlock mpb;

    private Vector3 position;
    private float t;
    private float tt;
    private Vector3 forward;
    private Vector3 euler;
    private List<Vector3> bezierPoints;

    private Vector4[] tilingOffset;
     

    private Transform camTrans;

    private Vector3 tempVec;

    void Awake()
    {
        if (GlobalGameDefine.mIsForceCloseInstanceBF)
        {
            //Destroy(this);
        }
    }

    // Use this for initialization
    void Start()
    {
        tilingOffset = new Vector4[3];
        //tilingOffset[0] = new Vector4(0.5f, 0.5f, 0f, 0f);
        //tilingOffset[1] = new Vector4(0.5f, 0.5f, 0.5f, 0f);
        tilingOffset[0] = new Vector4(0.333f, 0.5f, 0f, 0.5f);
        tilingOffset[1] = new Vector4(0.333f, 0.5f, 0.325f, 0.5f);
        tilingOffset[2] = new Vector4(0.333f, 0.5f, 0.665f, 0.5f);

        //tilingOffset[0] = new Vector4(1f, 0f, 0f, 1f);
        //tilingOffset[1] = new Vector4(0f, 1f, 0f, 1f);
        //tilingOffset[2] = new Vector4(0f, 0f, 1f, 1f);

        mats = new List<Matrix4x4>();
        randomWingSinStart = new List<float>();
        randomFlyingSinStarts = new List<float>();
        idleWingSpeeds = new List<float>();
        sinFactor1s = new List<float>();
        sinFactor2s = new List<float>();
        gpuBirdStates = new List<float>();
        //butterflyColors = new List<Vector4>();

        butterflyColors1 = new List<float>();
        butterflyColors2 = new List<float>();
        butterflyColors3 = new List<float>();
        butterflyColors4 = new List<float>();

        mpb = new MaterialPropertyBlock();
        if (!player)
        {
            player = LsyCommon.FindPlayer();
        }
        randomWingSinStartIndex = Shader.PropertyToID("_RandomWingSinStart");
        idleWingSpeedIndex = Shader.PropertyToID("_IdleWingSpeed");
        randomFlyingSinStartIndex = Shader.PropertyToID("_RandomFlyingSinStart");
        sinFactor1Index = Shader.PropertyToID("_SinFactor1");
        sinFactor2Index = Shader.PropertyToID("_SinFactor2");
        gpuBirdStateIndex = Shader.PropertyToID("_GPUBirdState");
        tilingOffsetIndex = Shader.PropertyToID("_ZzcButterflyMainTex_ST1111");

        tilingOffsetIndex1 = Shader.PropertyToID("_ZzcButterflyMainTex_ST1");
        tilingOffsetIndex2 = Shader.PropertyToID("_ZzcButterflyMainTex_ST2");
        tilingOffsetIndex3 = Shader.PropertyToID("_ZzcButterflyMainTex_ST3");
        tilingOffsetIndex4 = Shader.PropertyToID("_ZzcButterflyMainTex_ST4");

        for (int i = 0; i < butterflyData.Count; i++)
        {
            butterflyData[i].positions = new List<Vector3>();
            butterflyData[i].startPos = new List<Vector3>();
            butterflyData[i].endPos = new List<Vector3>();

            butterflyData[i].cullDistance = cullDistance;
            butterflyData[i].mainCamera = Camera.main;

            butterflyData[i].butterFlyState = DrawInstancing.ButterFlyData.ButterFlyState.idle;


            butterflyData[i].randomDirections = new List<Vector3>();
            butterflyData[i].randomSinStart = new List<float>();
            butterflyData[i].lastPositions = new List<Vector3>();
            butterflyData[i].quaternions = new List<Quaternion>();
            butterflyData[i].randomScale = new List<float>();
            butterflyData[i].tilingOffsets = new List<Vector4>();

            for (int j = 0; j < butterflyData[i].chunkLength; j++)
            {
                butterflyData[i].randomDirections.Add(new Vector3(Random.Range(-1f, 1f), butterflyData[i].bezierYFactor, Random.Range(-1f, 1f)));
                butterflyData[i].randomSinStart.Add(Random.Range(0f, 360f));

                tempVec = new Vector3(Random.Range(-butterflyData[i].startArea.x, butterflyData[i].startArea.x), Random.Range(-butterflyData[i].startArea.y, butterflyData[i].startArea.y), Random.Range(-butterflyData[i].startArea.z, butterflyData[i].startArea.z));
                tempVec = Quaternion.AngleAxis(butterflyData[i].startRotate, Vector3.up)*tempVec;
                butterflyData[i].startPos.Add( butterflyData[i].chunkPos + tempVec);
                tempVec = new Vector3(Random.Range(-butterflyData[i].endArea.x, butterflyData[i].endArea.x), Random.Range(-butterflyData[i].endArea.y, butterflyData[i].endArea.y), Random.Range(-butterflyData[i].endArea.z, butterflyData[i].endArea.z));
                tempVec = Quaternion.AngleAxis(butterflyData[i].endRotate, Vector3.up) * tempVec;
                butterflyData[i].endPos.Add( butterflyData[i].endAreaPos + tempVec);

                butterflyData[i].lastPositions.Add(butterflyData[i].startPos[j]);
                butterflyData[i].quaternions.Add(Quaternion.LookRotation(butterflyData[i].randomDirections[j]));
                butterflyData[i].positions.Add(butterflyData[i].startPos[j]);
                randomWingSinStart.Add(Random.Range(0f, 360f));
                randomFlyingSinStarts.Add(Random.Range(0f, 360f));
                idleWingSpeeds.Add(1f);
                gpuBirdStates.Add(0);
                sinFactor1s.Add(butterflyData[i].sinYFactor1);
                sinFactor2s.Add(butterflyData[i].sinYFactor2);
                butterflyData[i].randomScale.Add(Random.Range(0.7f, 1.1f));
                butterflyData[i].tilingOffsets.Add(tilingOffset[Random.Range(0,3)]);
                butterflyData[i].randomArriveTimes.Add(Random.Range(0f, 3f));
                //butterflyColors.Add(tilingOffset[Random.Range(0, 3)]);

                butterflyColors1.Add(1f);
                butterflyColors2.Add(1f);
                butterflyColors3.Add(1f);
                butterflyColors4.Add(1f);
            }
        }

        mpb.SetFloatArray(sinFactor1Index, sinFactor1s);
        mpb.SetFloatArray(sinFactor2Index, sinFactor2s);
        mpb.SetFloatArray(randomFlyingSinStartIndex, randomFlyingSinStarts);
        mpb.SetFloatArray(randomWingSinStartIndex, randomWingSinStart);

        camTrans = Camera.main.transform;
    }



    private void OnValidate()
    {
        if (sinFactor1s!=null&&sinFactor2s!=null)
        {
            sinFactor1s.Clear();
            sinFactor2s.Clear();
            for (int i = 0; i < butterflyData.Count; i++)
            {
                for (int j = 0; j < butterflyData[i].chunkLength; j++)
                {
                    sinFactor1s.Add(butterflyData[i].sinYFactor1);
                    sinFactor2s.Add(butterflyData[i].sinYFactor2);
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            t += Time.deltaTime;
            if (t > 1f)
            {
                player = LsyCommon.FindPlayer();
                t = 0f;
            }
        }

        if (!player)
        {
            return;
        }

        if (!camTrans)
        {
            camTrans = Camera.main.transform;
        }

        if (!camTrans)
        {
            return;
        }

        for (int i = 0; i < butterflyData.Count; i++)
        {
            if (Vector3.Distance(butterflyData[i].chunkPos, camTrans.position) >= cullDistance && Vector3.Distance(butterflyData[i].endAreaPos, camTrans.position) >= cullDistance)
            {
                continue;
            }

            Vector3 cam2Chunk = butterflyData[i].chunkPos - camTrans.position;
            Vector3 cam2End = butterflyData[i].endAreaPos - camTrans.position;
            if (Vector3.Dot(cam2Chunk, camTrans.forward) < 0 && Vector3.Dot(cam2End, camTrans.forward) < 0)
            {
                continue;
            }

            //待机状态
            if (butterflyData[i].butterFlyState == DrawInstancing.ButterFlyData.ButterFlyState.idle)
            {
                if (Vector3.Distance(player.position, butterflyData[i].chunkPos) <= butterflyData[i].fleeDistance)
                {
                    butterflyData[i].butterFlyState = DrawInstancing.ButterFlyData.ButterFlyState.flyingTo;
                }
                for (int j = 0; j < butterflyData[i].chunkLength; j++)
                {
                    mats.Add(Matrix4x4.TRS(butterflyData[i].positions[j], butterflyData[i].quaternions[j], butterflyData[i].randomScale[j]*Vector3.one * butterflyData[i].scale));
                    gpuBirdStates.Add(0);
                    idleWingSpeeds.Add(0.2f);
                    //butterflyColors.Add(butterflyData[i].tilingOffsets[j]);

                    butterflyColors1.Add(butterflyData[i].tilingOffsets[j].x);
                    butterflyColors2.Add(butterflyData[i].tilingOffsets[j].y);
                    butterflyColors3.Add(butterflyData[i].tilingOffsets[j].z);
                    butterflyColors4.Add(butterflyData[i].tilingOffsets[j].w);
                }
            }else

            //在终点待机
            if (butterflyData[i].butterFlyState == DrawInstancing.ButterFlyData.ButterFlyState.endIdle)
            {
                if (Vector3.Distance(player.position, butterflyData[i].endAreaPos) <= butterflyData[i].fleeDistance)
                {
                    butterflyData[i].butterFlyState = DrawInstancing.ButterFlyData.ButterFlyState.flyingBack;
                }
                for (int j = 0; j < butterflyData[i].chunkLength; j++)
                {
                    mats.Add(Matrix4x4.TRS(butterflyData[i].positions[j], butterflyData[i].quaternions[j], butterflyData[i].randomScale[j] * Vector3.one * butterflyData[i].scale));
                    gpuBirdStates.Add(0);
                    idleWingSpeeds.Add(0.2f);
                    //butterflyColors.Add(butterflyData[i].tilingOffsets[j]);

                    butterflyColors1.Add(butterflyData[i].tilingOffsets[j].x);
                    butterflyColors2.Add(butterflyData[i].tilingOffsets[j].y);
                    butterflyColors3.Add(butterflyData[i].tilingOffsets[j].z);
                    butterflyColors4.Add(butterflyData[i].tilingOffsets[j].w);
                }
            }else

            //起点飞往终点
            if (butterflyData[i].butterFlyState == DrawInstancing.ButterFlyData.ButterFlyState.flyingTo)
            {
                butterflyData[i].currentTime += Time.deltaTime;

                if (butterflyData[i].currentTime >= butterflyData[i].flyTime)
                {
                    butterflyData[i].currentTime = 0f;
                    butterflyData[i].butterFlyState = DrawInstancing.ButterFlyData.ButterFlyState.endIdle;
                    for (int j = 0; j < butterflyData[i].chunkLength; j++)
                    {
                        mats.Add(Matrix4x4.TRS(butterflyData[i].positions[j], butterflyData[i].quaternions[j], butterflyData[i].randomScale[j] * Vector3.one * butterflyData[i].scale));
                        gpuBirdStates.Add(0);
                        idleWingSpeeds.Add(0.2f);
                        //butterflyColors.Add(butterflyData[i].tilingOffsets[j]);

                        butterflyColors1.Add(butterflyData[i].tilingOffsets[j].x);
                        butterflyColors2.Add(butterflyData[i].tilingOffsets[j].y);
                        butterflyColors3.Add(butterflyData[i].tilingOffsets[j].z);
                        butterflyColors4.Add(butterflyData[i].tilingOffsets[j].w);
                    }
                    continue;
                }
                //tt = butterflyData[i].currentTime / (butterflyData[i].flyTime);
                //t = Mathf.Pow(tt, 0.7f);

                for (int j = 0; j < butterflyData[i].chunkLength; j++)
                {
                    tt = butterflyData[i].currentTime / (butterflyData[i].flyTime-butterflyData[i].randomArriveTimes[j]);
                    tt = Mathf.Clamp01(tt);
                    t = Mathf.Pow(tt, 0.7f);

                    butterflyData[i].positions[j] = (1f - t) * (1f - t) * butterflyData[i].startPos[j] + 2f * t * (1f - t) * (butterflyData[i].startPos[j] + butterflyData[i].randomDirections[j].normalized * butterflyData[i].bezierFactor) + t * t * butterflyData[i].endPos[j];
                    if (t >= 0.8f)
                    {
                        butterflyData[i].positions[j] += butterflyData[i].finalCurve.Evaluate((t - 0.8f) * 5f) * Vector3.up;
                    }

                    if (t<0.8f)
                    {
                        forward = butterflyData[i].positions[j] - butterflyData[i].lastPositions[j];
                        butterflyData[i].quaternions[j] = Quaternion.LookRotation(forward);
                        euler = butterflyData[i].quaternions[j].eulerAngles;
                        euler -= Vector3.right * 45f;
                        butterflyData[i].quaternions[j] = Quaternion.Euler(euler);
                    }
                    

                    butterflyData[i].lastPositions[j] = butterflyData[i].positions[j];

                    mats.Add(Matrix4x4.TRS(butterflyData[i].positions[j], butterflyData[i].quaternions[j], butterflyData[i].randomScale[j] * Vector3.one * butterflyData[i].scale));                 
                    gpuBirdStates.Add(Mathf.Sqrt(1f - Mathf.Abs((Mathf.Clamp01(tt) - 0.5f) * 2f)));
                    if (tt > 0.95f)
                    {
                        idleWingSpeeds.Add(0.3f);
                    }
                    else
                    {
                        idleWingSpeeds.Add(1f);
                    }
                    
                    //butterflyColors.Add(butterflyData[i].tilingOffsets[j]);

                    butterflyColors1.Add(butterflyData[i].tilingOffsets[j].x);
                    butterflyColors2.Add(butterflyData[i].tilingOffsets[j].y);
                    butterflyColors3.Add(butterflyData[i].tilingOffsets[j].z);
                    butterflyColors4.Add(butterflyData[i].tilingOffsets[j].w);
                }
            }else


            //终点飞往起点
            if (butterflyData[i].butterFlyState == DrawInstancing.ButterFlyData.ButterFlyState.flyingBack)
            {
                butterflyData[i].currentTime += Time.deltaTime;

                if (butterflyData[i].currentTime >= butterflyData[i].flyTime)
                {
                    butterflyData[i].currentTime = 0f;
                    butterflyData[i].butterFlyState = DrawInstancing.ButterFlyData.ButterFlyState.idle;
                    for (int j = 0; j < butterflyData[i].chunkLength; j++)
                    {
                        mats.Add(Matrix4x4.TRS(butterflyData[i].positions[j], butterflyData[i].quaternions[j], butterflyData[i].randomScale[j] * Vector3.one * butterflyData[i].scale));
                        gpuBirdStates.Add(0);
                        idleWingSpeeds.Add(0.2f);
                        //butterflyColors.Add(butterflyData[i].tilingOffsets[j]);

                        butterflyColors1.Add(butterflyData[i].tilingOffsets[j].x);
                        butterflyColors2.Add(butterflyData[i].tilingOffsets[j].y);
                        butterflyColors3.Add(butterflyData[i].tilingOffsets[j].z);
                        butterflyColors4.Add(butterflyData[i].tilingOffsets[j].w);
                    }
                    continue;
                }
                //tt = butterflyData[i].currentTime / butterflyData[i].flyTime;
                //t = Mathf.Pow(tt, 0.7f);

                for (int j = 0; j < butterflyData[i].chunkLength; j++)
                {
                    tt = butterflyData[i].currentTime / (butterflyData[i].flyTime - butterflyData[i].randomArriveTimes[j]);
                    tt = Mathf.Clamp01(tt);
                    t = Mathf.Pow(tt, 0.7f);

                    butterflyData[i].positions[j] = (1f - t) * (1f - t) * butterflyData[i].endPos[j] + 2f * t * (1f - t) * (butterflyData[i].endPos[j] + butterflyData[i].randomDirections[j].normalized * butterflyData[i].bezierFactor) + t * t * butterflyData[i].startPos[j];
                    if (t >= 0.8f)
                    {
                        butterflyData[i].positions[j] += butterflyData[i].finalCurve.Evaluate((t - 0.8f) * 5f) * Vector3.up;
                    }

                    if (t < 0.8f)
                    {
                        forward = butterflyData[i].positions[j] - butterflyData[i].lastPositions[j];
                        butterflyData[i].quaternions[j] = Quaternion.LookRotation(forward);
                        euler = butterflyData[i].quaternions[j].eulerAngles;
                        euler -= Vector3.right * 45f;
                        butterflyData[i].quaternions[j] = Quaternion.Euler(euler);
                    }

                    butterflyData[i].lastPositions[j] = butterflyData[i].positions[j];

                    mats.Add(Matrix4x4.TRS(butterflyData[i].positions[j], butterflyData[i].quaternions[j], butterflyData[i].randomScale[j] * Vector3.one* butterflyData[i].scale));
                    gpuBirdStates.Add(Mathf.Sqrt(1f - Mathf.Abs((Mathf.Clamp01(tt) - 0.5f) * 2f)) );
                    if (tt > 0.95f)
                    {
                        idleWingSpeeds.Add(0.3f);
                    }
                    else
                    {
                        idleWingSpeeds.Add(1f);
                    }
                    //butterflyColors.Add(butterflyData[i].tilingOffsets[j]);

                    butterflyColors1.Add(butterflyData[i].tilingOffsets[j].x);
                    butterflyColors2.Add(butterflyData[i].tilingOffsets[j].y);
                    butterflyColors3.Add(butterflyData[i].tilingOffsets[j].z);
                    butterflyColors4.Add(butterflyData[i].tilingOffsets[j].w);
                }
            }           
        }


        mpb.SetFloatArray(idleWingSpeedIndex, idleWingSpeeds);
        mpb.SetFloatArray(gpuBirdStateIndex, gpuBirdStates);
        //mpb.SetVectorArray(tilingOffsetIndex, butterflyColors);

        mpb.SetFloatArray(tilingOffsetIndex1, butterflyColors1);
        mpb.SetFloatArray(tilingOffsetIndex2, butterflyColors2);
        mpb.SetFloatArray(tilingOffsetIndex3, butterflyColors3);
        mpb.SetFloatArray(tilingOffsetIndex4, butterflyColors4);

        //Debug.Log(butterflyColors.Count);
        Graphics.DrawMeshInstanced(butterflyMesh, 0, butterflyMaterial, mats, mpb);
        mats.Clear();
        idleWingSpeeds.Clear();
        gpuBirdStates.Clear();
        //butterflyColors.Clear();

        butterflyColors1.Clear();
        butterflyColors2.Clear();
        butterflyColors3.Clear();
        butterflyColors4.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (butterflyData==null)
        {
            return;
        }
        for (int a = 0; a < butterflyData.Count; a++)
        {
            Gizmos.matrix = Matrix4x4.TRS(butterflyData[a].chunkPos, Quaternion.Euler(0, butterflyData[a].startRotate, 0), Vector3.one);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(Vector3.zero, 0.2f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(butterflyData[a].startArea.x * 2, butterflyData[a].startArea.y * 2, butterflyData[a].startArea.z * 2));
            Gizmos.matrix = Matrix4x4.TRS(butterflyData[a].endAreaPos, Quaternion.Euler(0, butterflyData[a].endRotate, 0), Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Vector3.zero, 0.2f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(butterflyData[a].endArea.x * 2, butterflyData[a].endArea.y * 2, butterflyData[a].endArea.z * 2));
            Gizmos.matrix = Matrix4x4.identity;

            Vector3 one = new Vector3(0, butterflyData[a].bezierYFactor, 1).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;
            Vector3 two = new Vector3(1, butterflyData[a].bezierYFactor, 1).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;
            Vector3 three = new Vector3(1, butterflyData[a].bezierYFactor, 0).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;
            Vector3 four = new Vector3(1, butterflyData[a].bezierYFactor, -1).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;
            Vector3 five = new Vector3(-1, butterflyData[a].bezierYFactor, -1).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;
            Vector3 six = new Vector3(-1, butterflyData[a].bezierYFactor, 0).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;
            Vector3 seven = new Vector3(-1, butterflyData[a].bezierYFactor, 1).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;
            Vector3 eight = new Vector3(0, butterflyData[a].bezierYFactor, -1).normalized * butterflyData[a].bezierFactor + butterflyData[a].chunkPos;

            bezierPoints = new List<Vector3>();

            bezierPoints.Add(one);
            bezierPoints.Add(two);
            bezierPoints.Add(three);
            bezierPoints.Add(four);
            bezierPoints.Add(five);
            bezierPoints.Add(six);
            bezierPoints.Add(seven);
            bezierPoints.Add(eight);

            for (int j = 0; j < bezierPoints.Count; j++)
            {
                for (float i = 0f; i < 1f; i += 0.01f)
                {
                    Vector3 pos = (1f - i) * (1f - i) * butterflyData[a].chunkPos + 2f * i * (1f - i) * bezierPoints[j] + i * i * butterflyData[a].endAreaPos;
                    Gizmos.DrawSphere(pos, 0.1f);                    
                }
            }
        }
    }
#endif
}
