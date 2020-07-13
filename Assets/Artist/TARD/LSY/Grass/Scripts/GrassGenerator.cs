//using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassGenerator : MonoBehaviour
{
    //public Channels Channel = Channels.R;
    //public Texture2D ControlTex;
    //[NoToLua]
    public Transform PlayerCamera;
    //[NoToLua]
    public Vector3 MapCenter;
    //[NoToLua]
    public float Radiance = 80;
    [Range(0.1f,5f)]
    //[NoToLua]
    public float Density = 5;
    [HideInInspector]
    //[NoToLua]
    public bool Regenerate = false;
    //[NoToLua]
    public GameObject Grass;
    [HideInInspector]
    //[NoToLua]
    public GameObject ShadowGrass;
    //[NoToLua]
    public bool Clean = false;
    //[NoToLua]
    public float Maxdis = 80;
    //[NoToLua]
    public float Mindis = 20;

    private Ray mRay;
    private Vector3 mOrigon;
    private RaycastHit mHitInfo;
    private int mGrassQuantity = 1000;
    private MeshRenderer mMR;
    private MeshFilter mMF;
    private Mesh mSharedMesh;
    private Material mSharedMaterial;
    private Matrix4x4[] TransInfos;
    [HideInInspector]
    //[NoToLua]
    public Dictionary<Vector3, Matrix4x4[]> TransInfosMatrix;

    private Material mTerrainMat;
    private Transform mCurrentTerrain;
    public Texture2D TerrainTex;
    private Texture2D mTerrainTex;
    private int WPixels, HPixels;
    private Dictionary<Vector3, Matrix4x4[]> TransInfosMatrixPool;
    private List<Vector3> KeyGroup;

    private float SquareLength;
    private float mHalfLength;

    Vector3 mPos;
    Vector3 mScale;
    Quaternion mRot;
    Matrix4x4 TransMatrix;

    //[NoToLua]
    public float yDeltaCoef = 0;
    Vector3 yDelta;

    [Header("Only Terrain")]
    [Space(20)]
    //[NoToLua]
    public Transform TerrainModel;

    void Awake()
    {
        PlayerCamera = Camera.main.transform;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
		if (!TARDSwitches.GrassSwitch)
			return;
		
        if (Clean)
        {
            CleanAll();
        }
        Init();

        if (Regenerate)
        {
            Regenerate = false;
            GrassGeneration();
            Debug.Log("Scanning finished, GrassCount : " + TransInfosMatrix.Count * 1023);
        }
        RenderUnderCondition();
    }

    private void Init()
    {

        if (mMR == null)
        {
            mMR = Grass.GetComponentInChildren<MeshRenderer>();
            mMF = Grass.GetComponentInChildren<MeshFilter>();
            mSharedMaterial = mMR.sharedMaterial;
            mSharedMesh = mMF.sharedMesh;
            mRot = Grass.transform.rotation;
        }
        if (TransInfosMatrix == null)
        {
            TransInfosMatrix = new Dictionary<Vector3, Matrix4x4[]>();
        }
        if (KeyGroup == null)
        {
            KeyGroupGeneration();
        }
        yDelta = Vector3.up * 0.49f * Grass.transform.localScale.y * yDeltaCoef;
    }

    private void RenderUnderCondition()
    {
        if (TransInfosMatrix != null)
        {
            for (int i = 0; i < KeyGroup.Count; i++)
            {
                Vector3 onePoint = KeyGroup[i];
                onePoint.y = PlayerCamera.position.y;
                float dis = Vector3.Distance(onePoint, PlayerCamera.position);
                if (dis < Maxdis && (dis < Mindis || Vector3.Dot(onePoint - PlayerCamera.position, PlayerCamera.forward) > 0))
                {
                    if (TransInfosMatrix.ContainsKey(KeyGroup[i]))
                    {
                        Graphics.DrawMeshInstanced(mSharedMesh, 0, mSharedMaterial, TransInfosMatrix[KeyGroup[i]]);
                    }
                    else
                    {
                        SquareGeneration(KeyGroup[i]);
                    }
                }
            }
        }
    }

    private void KeyGroupGeneration()
    {
        KeyGroup = new List<Vector3>();
        mGrassQuantity = (int)(Radiance * Radiance * 4 * Density);
        //Debug.Log(mGrassQuantity);
        //mGrassQuantity = mGrassQuantity;
        TransInfos = new Matrix4x4[mGrassQuantity];

        int SquareNumber = mGrassQuantity / 1023;
        SquareLength = Radiance * 2 / Mathf.Sqrt((float)SquareNumber);
        mHalfLength = SquareLength * 0.5f;

        for (float ix = MapCenter.x - Radiance; ix < MapCenter.x + Radiance; ix = ix + SquareLength)
        {
            for (float iz = MapCenter.z - Radiance; iz < MapCenter.z + Radiance; iz = iz + SquareLength)
            {
                KeyGroup.Add(new Vector3(ix + mHalfLength, 0, iz + mHalfLength));
            }
        }
    }

    private void GrassGeneration()
    {
        CleanAll();

        KeyGroupGeneration();

        //int SquareNumber = mGrassQuantity / 1023;
        //float SquareLength = Radiance * 2 / Mathf.Sqrt((float)SquareNumber);

        for (int v = 0; v < KeyGroup.Count; v++)
        {

            Vector3 centerVector = KeyGroup[v];

            SquareGeneration(centerVector);
        }
    }

    private void SquareGeneration(Vector3 centerVector)
    {
        Matrix4x4[] tmpMatrix = new Matrix4x4[1023];
        Vector2 coord;
        Color col;

        for (int i = 0; i < 1023; i++)
        {
            mOrigon.x = Random.Range(centerVector.x - mHalfLength, centerVector.x + mHalfLength);
            mOrigon.z = Random.Range(centerVector.z - mHalfLength, centerVector.z + mHalfLength);
            {
                mOrigon.y = mRay.origin.y;
                mRay.origin = mOrigon;

                bool isRightTerrain = true;

                if (Physics.Raycast(mRay, out mHitInfo, 2000, 1 << 28 | 1 << 17))
                {
                    if (TerrainModel != null || mCurrentTerrain != mHitInfo.transform)
                    {
                        if (TerrainModel != null)
                        {
                            mCurrentTerrain = TerrainModel;
                            isRightTerrain = mHitInfo.transform == TerrainModel;
                        }
                        else
                        {
                            mCurrentTerrain = mHitInfo.transform;
                        }

                        MeshRenderer mr = mCurrentTerrain.GetComponent<MeshRenderer>();
                        mTerrainMat = mr.sharedMaterial;
                        /*if (TerrainTex == null)
                        {
                            mTerrainTex = (Texture2D)mTerrainMat.GetTexture("_MainTex");
                        }*/
                        if (TerrainTex != null && mTerrainTex == null)
                        {
                            mTerrainTex = TerrainTex;
                        }
                        else if (TerrainTex == null)
                        {
                            return;
                        }
                        WPixels = mTerrainTex.width;
                        HPixels = mTerrainTex.height;
                    }
                    mPos = mHitInfo.point;
                    if (mTerrainTex != null && isRightTerrain)
                    {
                        coord = mHitInfo.textureCoord;
                        col = mTerrainTex.GetPixel((int)(coord.x * (float)WPixels), (int)(coord.y * (float)HPixels));
                        if (Random.Range(0f, 1f) < col.r)// || Random.Range(0f, 1f) < col.a)
                        {
                            //float scaleModi = Mathf.Clamp01(col.r + col.a);
                            float scaleModi = Mathf.Clamp01(col.r);
                            float scaleModi2 = scaleModi * scaleModi;
                            mPos += yDelta * scaleModi;
                            //mScale = Random.Range(0.9f, 1.2f) * Grass.transform.localScale * scaleModi2;
                            mScale = Random.Range(0.9f, 1.2f) * Grass.transform.localScale;
                            //mRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                            mRot = Quaternion.identity;
                            TransMatrix = Matrix4x4.TRS(mPos, mRot, mScale);
                            tmpMatrix[i] = TransMatrix;

                        }
                        else
                        {
                            OutOfSpace(tmpMatrix, i);
                        }
                    }
                    else
                    {
                        OutOfSpace(tmpMatrix, i);
                    }

                }
                else
                {
                    mRay = new Ray(PlayerCamera.position + Vector3.up * 500, -Vector3.up);
                    OutOfSpace(tmpMatrix, i);
                }
            }
        }
        TransInfosMatrix.Add(centerVector, tmpMatrix);
    }

    private void OutOfSpace(Matrix4x4[] tmpMatrix, int i)
    {
        mPos = Vector3.zero;
        mScale = Vector3.zero;
        TransMatrix = Matrix4x4.TRS(mPos, mRot, mScale);
        tmpMatrix[i] = TransMatrix;
    }

    private void CleanAll()
    {

        Clean = false;
        KeyGroupGeneration();
        yDelta = Vector3.up * 0.45f * Grass.transform.localScale.y;
        mTerrainTex = null;
        mMR = Grass.GetComponentInChildren<MeshRenderer>();
        mMF = Grass.GetComponentInChildren<MeshFilter>();
        mSharedMaterial = mMR.sharedMaterial;
        mSharedMesh = mMF.sharedMesh;
        mRot = Grass.transform.rotation;
        TransInfosMatrix = new Dictionary<Vector3, Matrix4x4[]>();
        Matrix4x4[] tmpMatrix = new Matrix4x4[1];
        tmpMatrix[0] = new Matrix4x4();
        Graphics.DrawMeshInstanced(mSharedMesh, 0, mSharedMaterial, tmpMatrix);
    }
}
