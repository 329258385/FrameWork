using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceFish : MonoBehaviour
{
    [Tooltip("鱼Mesh")]
    public Mesh fishMesh;
    [Tooltip("鱼材质")]
    public Material fishMaterial;
    [Tooltip("是否Scene显示辅助线框")]
    public bool showGizmo = true;
    [Tooltip("(ChunkPos中心点)离相机多远(这群物件)不显示")]
    public float cullDistance = 50f;
    [Tooltip("玩家Transform,运行游戏时自动获取,测试场景需自己指定,不然物体不显示")]
    public Transform player;

    public List<DrawInstancing.FishData> fishData;

    private List<Matrix4x4> mats;
    private MaterialPropertyBlock mpb;
    private List<Vector4> colors1;
    private List<Vector4> colors2;
    private List<Vector4> colors3;
    private List<Vector4> colors4;
    private List<float> randomNum;
    private List<Vector3> scales;
    private List<float> speed;
    private List<float> fishAlphas;

    private int fishAlphaIndex;
    private int fishColorIndex1;
    private int fishColorIndex2;
    //private int fishColorIndex3;
    //private int fishColorIndex4;
    private int randomNumIndex;

    private Transform camTrans;

    private float t;

    void Awake()
    {
        if (GlobalGameDefine.mIsForceCloseInstanceBF)
        {
            //Destroy(this);
        }
    }

    private void Start()
    {
        if (!player)
        {
            player = LsyCommon.FindPlayer();
        }

        fishAlphaIndex = Shader.PropertyToID("_FishAlpha");
        fishColorIndex1 = Shader.PropertyToID("_FishColor1Main");
        fishColorIndex2 = Shader.PropertyToID("_FishColor1Sub");
        //fishColorIndex3 = Shader.PropertyToID("_FishColor2Main");
        //fishColorIndex4 = Shader.PropertyToID("_FishColor2Sub");
        randomNumIndex = Shader.PropertyToID("_RandomNum");

        mats = new List<Matrix4x4>();
        colors1 = new List<Vector4>();
        colors2 = new List<Vector4>();
        //colors3 = new List<Vector4>();
        //colors4 = new List<Vector4>();
        randomNum = new List<float>();
        scales = new List<Vector3>();
        speed = new List<float>();
        fishAlphas = new List<float>();
        mpb = new MaterialPropertyBlock();        

        for (int i = 0; i < fishData.Count; i++)
        {
            fishData[i].positions = new List<Vector3>();

            fishData[i].fishColors1 = new List<Vector4>();
            fishData[i].fishColors2 = new List<Vector4>();

            fishData[i].cullDistance = cullDistance;
            fishData[i].mainCamera = Camera.main;

            fishData[i].fishState = DrawInstancing.FishData.FishState.normal;
            fishData[i].fishAlpha = 1f;

            for (int j = 0; j < fishData[i].chunkLength; j++)
            {
                //if (j % 2 == 0)
                //{
                //    colors1.Add(fishData[i].color1Main);
                //    colors2.Add(fishData[i].color1Sub);
                //}
                    
                //else if (j % 2 == 1)
                //{
                //    colors1.Add(fishData[i].color2Main);
                //    colors2.Add(fishData[i].color2Sub);
                //}
                    
                //顶点运动随机时间偏移
                randomNum.Add(Random.Range(0, 10));
                //生成鱼以及鱼中心的位置
                Vector3 spawnCenterPos = new Vector3(Random.Range(-fishData[i].centerRect.x , fishData[i].centerRect.x), 0, Random.Range(-fishData[i].centerRect.y, fishData[i].centerRect.y));
                Vector3 spawnPos = new Vector3(Random.Range(-fishData[i].fishMaxRadius / 1.414f, fishData[i].fishMaxRadius / 1.414f), Random.Range(-fishData[i].height, fishData[i].height), Random.Range(-fishData[i].fishMaxRadius / 1.414f, fishData[i].fishMaxRadius / 1.414f));               
                while (spawnPos.x * spawnPos.x + spawnPos.z * spawnPos.z < fishData[i].fishMinRadius * fishData[i].fishMinRadius)
                    spawnPos = new Vector3(Random.Range(-fishData[i].fishMaxRadius / 1.414f, fishData[i].fishMaxRadius / 1.414f), Random.Range(-fishData[i].height, fishData[i].height), Random.Range(-fishData[i].fishMaxRadius / 1.414f, fishData[i].fishMaxRadius / 1.414f));                
                fishData[i].positions.Add(fishData[i].chunkPos + spawnCenterPos + spawnPos);
                fishData[i].oriPositions.Add(fishData[i].chunkPos + spawnCenterPos + spawnPos);
                fishData[i].forwards.Add(Vector3.one);
                fishData[i].startPositions.Add(fishData[i].chunkPos+spawnCenterPos);

                if (j % 2 == 0)
                {
                    fishData[i].fishColors1.Add(fishData[i].color1Main);
                    fishData[i].fishColors2.Add(fishData[i].color1Sub);
                }

                else if (j % 2 == 1)
                {
                    fishData[i].fishColors1.Add(fishData[i].color2Main);
                    fishData[i].fishColors2.Add(fishData[i].color2Sub);
                }
                colors1.Add(new Vector4(1, 1, 1, 1));
                colors2.Add(new Vector4(1, 1, 1, 1));

                //每条鱼速度随机系数
                speed.Add(Random.Range(0.8f, 1.2f));

                fishData[i].fleeFactor = 1f;
                fishAlphas.Add(1f);
            }
        }

        camTrans = Camera.main.transform;
    }

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        if (colors1!=null&&colors2!=null)
//        {
//            colors1.Clear();
//            colors2.Clear();
//            //colors3.Clear();
//            //colors4.Clear();
//            for (int i = 0; i < fishData.Count; i++)
//            {
//                for (int j = 0; j < fishData[i].chunkLength; j++)
//                {
//                    if (j % 2 == 0)
//                    {
//                        colors1.Add(fishData[i].color1Main);
//                        colors2.Add(fishData[i].color1Sub);
//                    }

//                    else if (j % 2 == 1)
//                    {
//                        colors1.Add(fishData[i].color2Main);
//                        colors2.Add(fishData[i].color2Sub);
//                    }
//                }
//            }
//        }        
//    }
//#endif

    void Update()
    {
        if (!player)
        {
            t += Time.deltaTime;
            if (t>1f)
            {
                player = LsyCommon.FindPlayer();
                t = 0f;
            }
            
        }

        if (!camTrans)
        {
            camTrans = Camera.main.transform;
        }

        if (!camTrans)
        {
            return;
        }

        if (!player)
        {
            return;
        }

        for (int i = 0; i < fishData.Count; i++)
        {
            bool shouldDo=true;
            //摄像机裁剪
            float dis = Vector3.Distance(fishData[i].chunkPos, camTrans.position);
            Vector3 cam2Chunk = fishData[i].chunkPos - camTrans.position;
            if (Vector3.Dot(cam2Chunk, camTrans.forward)<0)
            {
                shouldDo = dis >= cullDistance*0.3f ? false : true;
            }
            else
            {
                shouldDo = dis >= cullDistance ? false : true;
            }

            //循环状态
            if (fishData[i].fishState==DrawInstancing.FishData.FishState.normal)
            {
                if (Vector2.Distance(new Vector2(player.position.x,player.position.z),new Vector2(fishData[i].chunkPos.x,fishData[i].chunkPos.z)) <= fishData[i].fleeDistance)
                {
                    fishData[i].fishState = DrawInstancing.FishData.FishState.fleeing;
                }
            }
            //逃跑状态
            else if(fishData[i].fishState == DrawInstancing.FishData.FishState.fleeing)
            {
                fishData[i].chunkTime += Time.deltaTime;
                fishData[i].fleeFactor -= fishData[i].fadeSpeed * 0.01f * Time.deltaTime;
                fishData[i].fleeFactor = Mathf.Clamp01(fishData[i].fleeFactor);
                if (fishData[i].fishAlpha>0&&fishData[i].chunkTime>=fishData[i].startFadeTime)
                {
                    fishData[i].fishAlpha -= fishData[i].fadeSpeed * 0.01f * Time.deltaTime;
                    fishData[i].fishAlpha = Mathf.Clamp01(fishData[i].fishAlpha);                   
                }                
                if (Vector3.Distance(player.position, fishData[i].chunkPos)>=fishData[i].resetDistance)
                {
                    for (int j = 0; j < fishData[i].chunkLength; j++)
                    {
                        fishData[i].positions[j] = fishData[i].oriPositions[j];                 
                    }
                    fishData[i].fishState = DrawInstancing.FishData.FishState.normal;
                    fishData[i].fishAlpha = 1.0f;
                    fishData[i].fleeFactor = 1.0f;
                    fishData[i].chunkTime = 0f;
                }
            }

            for (int j = 0; j < fishData[i].chunkLength; j++)
            {
                Vector3 position=fishData[i].positions[j];
                Quaternion quaternion=Quaternion.identity;
                Vector3 forward = fishData[i].forwards[j];

                if (shouldDo&&fishData[i].fishAlpha>0f)
                {
                    //循环状态
                    if (fishData[i].fishState== DrawInstancing.FishData.FishState.normal)
                    {
                        Vector3 center2Fish = fishData[i].positions[j] - fishData[i].startPositions[j];
                        Vector3 newCenter2Fish=Vector3.zero;
                        if (fishData[i].direction== DrawInstancing.FishData.Direction.clockwise)
                        {
                            forward = Vector3.Cross(Vector3.up, new Vector3(center2Fish.x, 0, center2Fish.z));
                            newCenter2Fish = Quaternion.AngleAxis(Time.deltaTime * speed[j] * fishData[i].swimSpeed, Vector3.up) * center2Fish;
                        }
                        else
                        {
                            forward = -Vector3.Cross(Vector3.up, new Vector3(center2Fish.x, 0, center2Fish.z));
                            newCenter2Fish = Quaternion.AngleAxis(Time.deltaTime * speed[j] * -fishData[i].swimSpeed, Vector3.up) * center2Fish;
                        }                       
                        quaternion = Quaternion.LookRotation(forward);
                        position = fishData[i].startPositions[j] + newCenter2Fish;
                        fishData[i].positions[j] = position;
                        fishData[i].forwards[j] = forward;
                        fishAlphas.Add(1.0f);

                        colors1.Add(fishData[i].fishColors1[j]);
                        colors2.Add(fishData[i].fishColors2[j]);
                    }
                    //逃跑状态
                    else if (fishData[i].fishState==DrawInstancing.FishData.FishState.fleeing)
                    {
                        //Vector3 player2Center = fishData[i].chunkPos - player.position;
                        Vector3 player2Fish = fishData[i].positions[j] - player.position;
                        //float angle = AngleSigned(player.forward, player2Fish, Vector3.up);
                        //angle = Mathf.Clamp(angle, -45f, 45f);
                        //player2Fish = Quaternion.AngleAxis(angle * 2.5f, Vector3.up) * player2Center;
                        player2Fish = new Vector3(player2Fish.x, 0, player2Fish.z);
                        forward = Vector3.Lerp(fishData[i].forwards[j], player2Fish, Mathf.Clamp01((1 - fishData[i].fleeFactor))*0.5f);
                        Vector3 right = Quaternion.AngleAxis(90f, Vector3.up) * forward;
                        quaternion = Quaternion.LookRotation(forward);
                        position = fishData[i].positions[j] + forward.normalized * fishData[i].fleeSpeed*speed[j] * Time.deltaTime + right.normalized * Time.deltaTime * Mathf.Sin(Time.timeSinceLevelLoad * 10f + speed[j] * 100f) * 0.5f;                        
                        fishData[i].positions[j] = position;
                        fishData[i].forwards[j] = forward;
                        fishAlphas.Add(fishData[i].fishAlpha);

                        colors1.Add(fishData[i].fishColors1[j]);
                        colors2.Add(fishData[i].fishColors2[j]);

                    }
                    
                    if (mats.Count < 1023)
                    {
                        mats.Add(Matrix4x4.TRS(position, quaternion, Vector3.one * fishData[i].fishSize));
                    }
                }
                else
                {
                    position = fishData[i].positions[j];
                    quaternion = Quaternion.identity;
                }      
            }
        }

        mpb.SetVectorArray(fishColorIndex1, colors1);
        mpb.SetVectorArray(fishColorIndex2, colors2);
        //mpb.SetVectorArray(fishColorIndex3, colors3);
        //mpb.SetVectorArray(fishColorIndex4, colors4);
        mpb.SetFloatArray(randomNumIndex, randomNum);
        mpb.SetFloatArray(fishAlphaIndex, fishAlphas);

        Graphics.DrawMeshInstanced(fishMesh, 0, fishMaterial, mats, mpb);
        mats.Clear();
        fishAlphas.Clear();

        colors1.Clear();
        colors2.Clear();
    }

    private static float AngleSigned(Vector3 v1,Vector3 v2,Vector3 n)
    {
        return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            if (fishData == null)
            {
                return;
            }
            for (int i = 0; i < fishData.Count; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(fishData[i].chunkPos, 1);
                Gizmos.DrawWireCube(fishData[i].chunkPos, new Vector3(fishData[i].centerRect.x * 2, fishData[i].height * 2, fishData[i].centerRect.y * 2));
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(fishData[i].chunkPos, new Vector3((fishData[i].centerRect.x  + fishData[i].fishMaxRadius) * 2, fishData[i].height * 2, (fishData[i].centerRect.y + fishData[i].fishMaxRadius) * 2));
            }
        }
    }
#endif
}
