using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFlock : MonoBehaviour
{
    public MeshFilter[] BirdObjMeshFilter;
    public MeshRenderer[] BirdObjMeshRenderer;
    [Range(0, 1023)]
    public int BirdsNumber = 12;

    //[Range(0, 60)]
    //public float TargetChangeDuration = 3;


    /// <summary>
    /// 飞行最大范围
    /// </summary>
    public Vector3 FlyingAreaSize = new Vector3(10, 10, 10);
    /// <summary>
    /// 小目标范围
    /// </summary>
    public Vector3 smallTargetAreaSize = new Vector3(2, 2, 2);
    /// <summary>
    /// 目标最大范围
    /// </summary>
    //public float RandomTargetSize = 3;
    //private Vector3 mRandomTargetSize = Vector3.one;
    private Vector3 mRandomTargetPos = Vector3.zero;
    //private float mTimer = 0;
    //private float checkTimer = 0;
    //private float speed = 0f;
    //public Transform target;
    /// <summary>
    /// 相机位置
    /// </summary>
    public Transform cameraPos;
    /// <summary>
    /// 相机
    /// </summary>
    public Camera mainCamera;
    /// <summary>
    /// 是否使用快速排序
    /// </summary>
    public bool useQuickSort = false;
    /// <summary>
    /// 最远距离
    /// </summary>
    public float maxDistance = 100f;
    /// <summary>
    /// LOD距离
    /// </summary>
    public float LODDistance = 25f;
    /// <summary>
    /// 鸟旋转速度系数
    /// </summary>
    [Range(0,20)]
    public float birdRotateFactor = 1f;
    /// <summary>
    /// 飞行速度系数
    /// </summary>
    [Range(0,20)]
    public float speedFactor = 1f;
    /// <summary>
    /// 目标中心
    /// </summary>
    public Transform bigTargetPos;
    /// <summary>
    /// 目标数组
    /// </summary>
    private List<Vector3> myPos;
    /// <summary>
    /// 鸟矩阵
    /// </summary>
    private List<Matrix4x4> birdMatrixs;
    private List<Vector3> birdPos;
    private List<Vector3> tempDir;
    private List<float> distance;
    //private List<Transform> mBirds;
    private List<float> birdSpeeds;
    private List<float> birdScale;
    //private List<Vector3> pos;
    private Vector3 invertZScale;

    private Vector3 mPos;
    private Vector3 mScale;
    private Quaternion mRot;

    private Vector3 targetPos;
    private Vector3 dir;
    private Quaternion q;
    private float x, y, z = 0;
    private Vector3 res;
    private bool disCheck;
    private bool fovCheck;
    private Rect screenRect;
    private Vector3 screenPos;
    private MeshFilter mf;
    private MeshRenderer mr;
    private float disCamera2Bird;
    private Vector3 center2Birds;
    private Vector3 horizontalDir;
    public bool showGizmos;
    // Use this for initialization
    void Start()
    {
        //z轴反向
        invertZScale = new Vector3(1, 1, -1);
        birdScale = new List<float>();
        birdMatrixs = new List<Matrix4x4>();
        birdPos = new List<Vector3>();
        myPos = new List<Vector3>();
        tempDir = new List<Vector3>();
        distance = new List<float>();
        birdSpeeds = new List<float>();
        Vector3 generationPos;


        int i = 0;
        for (i = 0; i < BirdsNumber; i++)
        {
            birdSpeeds.Add(Random.Range(3f, 4.5f));
            birdScale.Add(Random.Range(0.8f, 1.2f));
            generationPos = RandomPos();
            while (Vector3.Distance(generationPos, bigTargetPos.position) < smallTargetAreaSize.x*0.707f)
            {
                generationPos = RandomPos();
            }
            birdPos.Add(generationPos);

            center2Birds = birdPos[i] - bigTargetPos.position;
            tempDir.Add(Vector3.Cross(center2Birds, Vector3.up));

        }
        mRandomTargetPos = transform.position;
        RandomPosInSmallBox();

        mf = BirdObjMeshFilter[0];
        mr = BirdObjMeshRenderer[0];
    }

    private Vector3 RandomPos()
    {
        res.x = Random.Range(transform.position.x - FlyingAreaSize.x * 0.5f, transform.position.x + FlyingAreaSize.x * 0.5f);
        res.y = Random.Range(transform.position.y - FlyingAreaSize.y * 0.5f, transform.position.y + FlyingAreaSize.y * 0.5f);
        res.z = Random.Range(transform.position.z - FlyingAreaSize.z * 0.5f, transform.position.z + FlyingAreaSize.z * 0.5f);
        return res;
    }

    private void RandomPosInSmallBox()
    {
        myPos.Clear();
        for (int i = 0; i < BirdsNumber; i++)
        {
            
            x = Random.Range(mRandomTargetPos.x - smallTargetAreaSize.x * 0.5f, mRandomTargetPos.x + smallTargetAreaSize.x * 0.5f);            
            y = Random.Range(mRandomTargetPos.y - smallTargetAreaSize.y * 0.5f, mRandomTargetPos.y + smallTargetAreaSize.y * 0.5f);
            z = Random.Range(mRandomTargetPos.z - smallTargetAreaSize.z * 0.5f, mRandomTargetPos.z + smallTargetAreaSize.z * 0.5f);
            myPos.Add(new Vector3(x, y, z));
        }
    }

    private void BirdsStartPosInit()
    {
        for (int i = 0; i < BirdsNumber; i++)
        {
            center2Birds = birdPos[i] - bigTargetPos.position;
            tempDir.Add(Vector3.Cross(center2Birds, Vector3.up));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (showGizmos)
            {
                showGizmos = false;
            }
            else
            {
                showGizmos = true;
            }
        }
        //mTimer += Time.deltaTime;
        //checkTimer += Time.deltaTime;

        //mRandomTargetPos = RandomPos();
        //RandomPosInSmallBox();

        //if (mTimer > TargetChangeDuration)
        //{
        //    mRandomTargetPos = RandomPos();
        //    RandomPosInSmallBox();
        //    mTimer = 0;
        //}

        //if (checkTimer >= 5)
        //{
        //    for (int i = 0; i < BirdsNumber; i++)
        //    {
        //        birdSpeeds[BirdsNumber + i] = Random.Range(3f, 6f);
        //    }
        //    checkTimer = 0f;
        //}

        for (int i = 0; i < BirdsNumber; i++)
        {
            //目标方向
            targetPos = Vector3.Lerp(birdPos[i] + tempDir[i], myPos[i], 0.000001f * birdSpeeds[i]* birdRotateFactor);
            dir = targetPos - birdPos[i];
            if (Vector3.Angle(targetPos, dir) > 150)
            {
                //dir = Vector3.Normalize(targetPos) + Vector3.Normalize((myPos[i] - birdPos[i]));
            }
            //当前面向方向
            tempDir[i] = Vector3.Normalize(dir);
            horizontalDir = Vector3.right * tempDir[i].x + Vector3.forward * tempDir[i].z;
            if (Vector3.Angle(horizontalDir, tempDir[i]) >= 15)
            {
                tempDir[i] = horizontalDir;
            }
            q = Quaternion.LookRotation(dir);
            //更新位置
            birdPos[i] += Vector3.Normalize(dir) * 0.01f * birdSpeeds[i] * speedFactor;

            //速度渐变
            //birdSpeeds[i] = Mathf.Lerp(birdSpeeds[i], birdSpeeds[i + BirdsNumber], checkTimer * 0.2f);

            //根据距离和是否在视锥体内进行裁剪
            disCamera2Bird = Vector3.Distance(birdPos[i], cameraPos.position);
            disCheck = disCamera2Bird < maxDistance;
            screenPos = mainCamera.WorldToScreenPoint(birdPos[i]);
            screenRect = new Rect(-10, -10, Screen.width + 10, Screen.height + 10);
            fovCheck = screenRect.Contains(screenPos);
            //if (!disCheck||!fovCheck)
            //{
            //    continue;
            //}
            //LOD
            if (disCamera2Bird< LODDistance)
            {
                //Debug.Log("1");
                mf = BirdObjMeshFilter[0];
                mr = BirdObjMeshRenderer[0];
            }
            else
            {
                //Debug.Log("2");
                mf = BirdObjMeshFilter[1];
                mr = BirdObjMeshRenderer[1];
                
            }

            mPos = birdPos[i];
            mScale = invertZScale * birdScale[i];
            mRot = q;
            //构建矩阵列表
            birdMatrixs.Add(Matrix4x4.TRS(mPos, mRot, mScale));
            //距离列表
            distance.Add(Vector3.Distance(cameraPos.position, birdPos[i]));
            
        }
        if (useQuickSort)
        {
            QuickSort(distance, 0, distance.Count - 1);
        }
        
        Graphics.DrawMeshInstanced(mf.sharedMesh, 0, mr.sharedMaterial, birdMatrixs);
        //清空列表留给下一帧
        birdMatrixs.Clear();
        distance.Clear();
    }

    //绘制区域
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, smallTargetAreaSize);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, FlyingAreaSize);
        }
        
    }

    #region QuickSort
    private int Division(List<float> distance, int left, int right)
    {
        while (left < right)
        {
            float num = distance[left];
            Matrix4x4 matrix = birdMatrixs[left];

            if (num > distance[left + 1])
            {
                distance[left] = distance[left + 1];
                birdMatrixs[left] = birdMatrixs[left + 1];

                distance[left + 1] = num;
                birdMatrixs[left + 1] = matrix;

                left++;
            }
            else
            {
                float temp = distance[right];
                Matrix4x4 matrixTemp = birdMatrixs[right];

                distance[right] = distance[left + 1];
                birdMatrixs[right] = birdMatrixs[left + 1];

                distance[left + 1] = temp;
                birdMatrixs[left + 1] = matrixTemp;

                right--;
            }
        }
        return left;
    }

    private void QuickSort(List<float> list, int left, int right)
    {
        if (left < right)
        {
            int i = Division(list, left, right);
            QuickSort(list, i + 1, right);
            QuickSort(list, left, i - 1);
        }
    }
    #endregion
}



