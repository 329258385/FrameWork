using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LV01BirdFlock : MonoBehaviour {
    [Tooltip("鸟Mesh")]
    public MeshFilter meshFilter;
    [Tooltip("鸟材质")]
    public MeshRenderer meshRenderer;
    
    public List<DrawInstancing.BirdData> birdData;

    private List<Matrix4x4> matrixs;
    private List<float> alphas;
    private MaterialPropertyBlock mpb;
    private Rect screenRect;
    private Vector3 invertZ;
    private Vector3 generatePos;

    private int birdAlphaIndex;

    void Awake()
    {
        if (GlobalGameDefine.mIsForceCloseInstanceBF)
        {
            //Destroy(this);
        }
    }

    void Start () {
        birdAlphaIndex = Shader.PropertyToID("_BirdAlpha");
        matrixs = new List<Matrix4x4>();
        alphas = new List<float>();
        mpb = new MaterialPropertyBlock();
        screenRect = new Rect(-200, -200, Screen.width + 400, Screen.height + 400);
        invertZ = new Vector3(1, 1, -1);
        for (int i = 0; i < birdData.Count; i++)
        {
            birdData[i].screenRect = screenRect;
            birdData[i].chunk2End = birdData[i].endPos - birdData[i].chunkPos;
            birdData[i].mScale = invertZ * 3;
            birdData[i].mRot = Quaternion.LookRotation(birdData[i].chunk2End);
       
            for (int j = 0; j < birdData[i].chunkLength; j++)
            {
                generatePos = birdData[i].RandomPos();
                birdData[i].positions.Add(generatePos);
                birdData[i].birdAlpha = 1f;
            }
        }
	}
	
	void Update () {
        for (int i = 0; i < birdData.Count; i++)
        {
            if (birdData[i].curTime>=birdData[i].totalTime)
            {                
                birdData[i].curTime = 0f;
                birdData[i].positions.Clear();
                for (int j = 0; j < birdData[i].chunkLength; j++)
                {
                    birdData[i].positions.Add(birdData[i].RandomPos());
                }
            }
            birdData[i].curTime += Time.deltaTime;
            birdData[i].birdAlpha = Mathf.Clamp01(birdData[i].curTime);
            if (birdData[i].curTime>=birdData[i].totalTime-1f)
            {
                birdData[i].birdAlpha = Mathf.Clamp01(Mathf.Abs(birdData[i].totalTime- birdData[i].curTime));
            }

            for (int j = 0; j < birdData[i].chunkLength; j++)
            {
                birdData[i].curPos = Vector3.Lerp(birdData[i].positions[j], birdData[i].positions[j] + birdData[i].chunk2End, birdData[i].curTime / birdData[i].totalTime);
                birdData[i].mPos = birdData[i].curPos;
                matrixs.Add(Matrix4x4.TRS(birdData[i].mPos, birdData[i].mRot, birdData[i].mScale));
                alphas.Add(birdData[i].birdAlpha);
            }
        }
        mpb.SetFloatArray(birdAlphaIndex, alphas);
        Graphics.DrawMeshInstanced(meshFilter.sharedMesh, 0, meshRenderer.sharedMaterial, matrixs,mpb);        
        matrixs.Clear();
        alphas.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (birdData==null)
        {
            return;
        }
        for (int i = 0; i < birdData.Count; i++)
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawSphere(birdData[i].chunkPos, 1);
            Gizmos.color = new Color(1, 0, 0, 1);
            Gizmos.DrawSphere(birdData[i].endPos, 1);
            Gizmos.color = new Color(0, 1, 0, 1);
            Gizmos.DrawWireCube(birdData[i].chunkPos, new Vector3(birdData[i].flyingAreaSize.x, birdData[i].flyingAreaSize.y, birdData[i].flyingAreaSize.z));
        }       
    }
#endif
}
