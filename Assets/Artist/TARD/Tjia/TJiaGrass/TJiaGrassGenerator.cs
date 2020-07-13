using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TJiaGrassGenerator : MonoBehaviour
{

    [System.Serializable]
    public class InstanceModule
    {
        private float mRadiasRonde = 0.5f;
        private float mHideAngle = 0.51f;
        public int Density = 1;
        public int StartRadias = 0;
        public int Thickness = 30;
        [SerializeField] private int InstanceCount;
        public Mesh InstanceMesh;
        public Material InstanceMaterial;
        private int SubMeshIndex = 0;

        private int mCachedDensity = -1;
        private int mCachedRadias = -1;
        private float mCachedHideAngle = -1;

        private ComputeBuffer mPositionBuffer;
        private ComputeBuffer mArgsBuffer;
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        internal float RepeatRate = 0.7f;

        private DitherMode Dither = DitherMode.Off;

        internal bool Indirect = true;
        private List<Matrix4x4[]> mTransMatrixList;
        public int MatrixLength = 0;

        public enum DitherMode
        {
            Off,
            ZeroToOne,
            OneToZero
        };

        public InstanceModule()
        {
            Density = 1;
            StartRadias = 0;
            Thickness = 30;
        }

        public void Initialization()
        {
            mCachedDensity = -1;
            mCachedRadias = -1;
            Dither = DitherMode.Off;
            InstanceMaterial.enableInstancing = true;
            if (Indirect)
            {
                InstanceMaterial.shader = UnityUtils.FindShader("SAO_TJia_V3/Grass/TjiaInstancedGrassShader");
                mArgsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                UpdateBuffers();
            }
            else
            {
                InstanceMaterial.shader = UnityUtils.FindShader("SAO_TJia_V3/Grass/TjiaDirectInstancedGrassShader");
                mTransMatrixList = new List<Matrix4x4[]>();
                UpdateBuffersDirect();
            }
        }

        private void UpdateBuffersDirect()
        {
            int endRadias = Thickness + StartRadias;

            if (mCachedDensity != Density || mCachedRadias != endRadias || mCachedHideAngle != mHideAngle)
            {
                InstanceMaterial.SetFloat("_GrassDensity", 1f / (float)Density);
                InstanceMaterial.SetFloat("_RepeatRate", RepeatRate);

                if (Dither == DitherMode.Off)
                {
                    InstanceMaterial.DisableKeyword("_DITHER_ON");
                }
                else
                {
                    InstanceMaterial.EnableKeyword("_DITHER_ON");
                }

                if (mTransMatrixList != null)
                {
                    mTransMatrixList.Clear();
                    mTransMatrixList = null;
                    mTransMatrixList = new List<Matrix4x4[]>();
                }

                int squareNum = (int)((endRadias) * Density);

                List<Vector4> posBuffer = new List<Vector4>();

                for (int i = 0; i < squareNum; i++)
                {
                    float posX = ((float)i / (float)(squareNum) - 0.5f) * 2f;
                    for (int j = 0; j < squareNum; j++)
                    {
                        float posZ = ((float)j / (float)(squareNum) - 0.5f) * 2f;

                        float squaredDis = posX * posX + posZ * posZ;
                        float sinThetaCaree = posZ / Mathf.Sqrt(squaredDis + 1e-6f);

                        if ((sinThetaCaree > mHideAngle && squaredDis < 1f && squaredDis > (float)(StartRadias * StartRadias) / (float)(endRadias * endRadias)) || (StartRadias == 0 && squaredDis < mRadiasRonde * mRadiasRonde))
                        {
                            float startProportion;
                            float dither = 0;
                            switch (Dither)
                            {
                                case DitherMode.Off:
                                    break;
                                case DitherMode.ZeroToOne:
                                    startProportion = (float)StartRadias / (float)(endRadias);
                                    dither = 1 - (Mathf.Sqrt(squaredDis) - startProportion) / (1 - startProportion);
                                    //dither *= dither;
                                    break;
                                case DitherMode.OneToZero:
                                    startProportion = (float)StartRadias / (float)(endRadias);
                                    dither = (Mathf.Sqrt(squaredDis) - startProportion) / (1 - startProportion);
                                    //dither *= dither;
                                    break;
                            }
                            posBuffer.Add(new Vector4(i - squareNum / 2, 0, j - squareNum / 2, dither));
                        }
                    }
                }

                InstanceCount = posBuffer.Count;

                if (InstanceCount > 0)
                {
                    int num = InstanceCount;
                    int posCpt = 0;
                    var rotation = Quaternion.identity;
                    var scale = Vector3.one;
                    while (num > 1021)
                    {
                        var matrixArry = new Matrix4x4[1023];
                        for (int i = 0; i < matrixArry.Length; i++)
                        {
                            if (i < 1021)
                            {
                                Vector3 pos = posBuffer[posCpt];
                                var matrix = Matrix4x4.TRS(pos, rotation, scale);
                                matrixArry[i] = matrix;
                                posCpt++;
                                num--;
                            }
                            else
                            {
                                Vector3 pos = Vector3.one * 10000 * ((i % 2) * 2 - 1);
                                var matrix = Matrix4x4.TRS(pos, rotation, scale);
                                matrixArry[i] = matrix;
                            }
                        }
                        mTransMatrixList.Add(matrixArry);
                    }
                    var matrixArryF = new Matrix4x4[num + 2];
                    for (int i = 0; i < matrixArryF.Length; i++)
                    {
                        if (i < matrixArryF.Length - 2)
                        {
                            Vector3 pos = posBuffer[posCpt];
                            var matrix = Matrix4x4.TRS(pos, rotation, scale);
                            matrixArryF[i] = matrix;
                            posCpt++;
                            num--;
                        }
                        else
                        {
                            Vector3 pos = Vector3.one * 10000 * ((i % 2) * 2 - 1);
                            var matrix = Matrix4x4.TRS(pos, rotation, scale);
                            matrixArryF[i] = matrix;
                        }
                    }
                    mTransMatrixList.Add(matrixArryF);

                    posBuffer.Clear();
                    posBuffer = null;
                }

                mCachedDensity = Density;
                mCachedRadias = endRadias;
                mCachedHideAngle = mHideAngle;
            }
        }

        public void Update()
        {
            return;
            if (Indirect)
            {
                UpdateBuffers();

                if (InstanceCount > 0 && InstanceMesh != null && InstanceMaterial != null)
                {
                    Graphics.DrawMeshInstancedIndirect(InstanceMesh, SubMeshIndex, InstanceMaterial, new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000)), mArgsBuffer);
                }
                else if (InstanceMesh == null || InstanceMaterial == null)
                {
                    Debug.LogError("草原模型或材质丢失");
                }
            }
            else
            {
                UpdateBuffersDirect();

                MatrixLength = mTransMatrixList.Count;

                if (InstanceCount > 0 && InstanceMesh != null && InstanceMaterial != null)
                {
                    for (int i = 0; i < mTransMatrixList.Count; i++)
                    {
                        Graphics.DrawMeshInstanced(InstanceMesh, SubMeshIndex, InstanceMaterial, mTransMatrixList[i]);
                        //Debug.Log(Time.time);
                    }
                }
                else if (InstanceMesh == null || InstanceMaterial == null)
                {
                    Debug.LogError("草原模型或材质丢失");
                }
            }
        }

        public void UpdateBuffers()
        {
            int endRadias = Thickness + StartRadias;

            if (mCachedDensity != Density || mCachedRadias != endRadias || mCachedHideAngle != mHideAngle)
            {
                InstanceMaterial.SetFloat("_GrassDensity", 1f / (float)Density);
                InstanceMaterial.SetFloat("_RepeatRate", RepeatRate);

                if (Dither == DitherMode.Off)
                {
                    InstanceMaterial.DisableKeyword("_DITHER_ON");
                }
                else
                {
                    InstanceMaterial.EnableKeyword("_DITHER_ON");
                }

                if (mPositionBuffer != null)
                {
                    mPositionBuffer.Release();
                }

                int squareNum = (int)((endRadias) * Density);

                List<Vector4> posBuffer = new List<Vector4>();

                for (int i = 0; i < squareNum; i++)
                {
                    float posX = ((float)i / (float)(squareNum) - 0.5f) * 2f;
                    for (int j = 0; j < squareNum; j++)
                    {
                        float posZ = ((float)j / (float)(squareNum) - 0.5f) * 2f;

                        float squaredDis = posX * posX + posZ * posZ;
                        float sinThetaCaree = posZ / Mathf.Sqrt(squaredDis + 1e-6f);

                        if ((sinThetaCaree > mHideAngle && squaredDis < 1f && squaredDis > (float)(StartRadias * StartRadias) / (float)(endRadias * endRadias)) || (StartRadias == 0 && squaredDis < mRadiasRonde * mRadiasRonde))
                        {
                            float startProportion;
                            float dither = 0;
                            switch (Dither)
                            {
                                case DitherMode.Off:
                                    break;
                                case DitherMode.ZeroToOne:
                                    startProportion = (float)StartRadias / (float)(endRadias);
                                    dither = 1 - (Mathf.Sqrt(squaredDis) - startProportion) / (1 - startProportion);
                                    //dither *= dither;
                                    break;
                                case DitherMode.OneToZero:
                                    startProportion = (float)StartRadias / (float)(endRadias);
                                    dither = (Mathf.Sqrt(squaredDis) - startProportion) / (1 - startProportion);
                                    //dither *= dither;
                                    break;
                            }
                            posBuffer.Add(new Vector4(i - squareNum / 2, 0, j - squareNum / 2, dither));
                        }
                    }
                }

                InstanceCount = posBuffer.Count;

                if (InstanceCount > 0)
                {

                    mPositionBuffer = new ComputeBuffer(InstanceCount, 16);
                    Vector4[] positions = new Vector4[InstanceCount];

                    for (int i = 0; i < InstanceCount; i++)
                    {
                        positions[i] = posBuffer[i];
                    }

                    posBuffer.Clear();
                    posBuffer = null;

                    mPositionBuffer.SetData(positions);
                    InstanceMaterial.SetBuffer("_PositionBuffer", mPositionBuffer);

                    if (InstanceMesh != null)
                    {
                        args[0] = (uint)InstanceMesh.GetIndexCount(SubMeshIndex);
                        args[1] = (uint)InstanceCount;
                        args[2] = (uint)InstanceMesh.GetIndexStart(SubMeshIndex);
                        args[3] = (uint)InstanceMesh.GetBaseVertex(SubMeshIndex);
                    }
                    else
                    {
                        args[0] = args[1] = args[2] = args[3] = 0;
                    }

                    mArgsBuffer.SetData(args);
                }

                mCachedDensity = Density;
                mCachedRadias = endRadias;
                mCachedHideAngle = mHideAngle;
            }
        }

        public void Destroy()
        {
            if (Indirect)
            {
                if (mPositionBuffer != null)
                {
                    mPositionBuffer.Release();
                }
                mPositionBuffer = null;
                if (mArgsBuffer != null)
                {
                    mArgsBuffer.Release();
                }
                mArgsBuffer = null;
            }
            else
            {
                mTransMatrixList.Clear();
                mTransMatrixList = null;
            }
        }
    }
    [NoToLua]
    public RenderTexture InteractiveGrassTexture;
    [NoToLua]
    public Material InteractiveGrassMat;
    [NoToLua]
    public InstanceModule[] IMs;

    private RenderTexture mTmpRT;
    private bool mCpt;
    private Transform GrassFXTr;
    private ParticleSystem GrassFX;
    internal Transform PlayerPos;

    public bool AutoMethodSelection = true;
    public bool Indirect = true;
    
    private static TJiaGrassGenerator _instance;
    public static TJiaGrassGenerator Instance
    {
        get
        {
            return _instance;
        }
    }

    void OnEnable()
    {
        // 添加强制关闭，以便于模块化测试
        if (GlobalGameDefine.mIsForceCloseGPUGrass)
        {
            this.enabled = false;
            return;
        }

        if (AutoMethodSelection)
        {
            Indirect = UnityUtils.FindShader("SAO_TJia_V3/Grass/TjiaInstancedGrassShader").isSupported;
        }

        //Indirect = false;
        Debug.Log("TJiaGrassGenerator.OnEnable: " + UnityEngine.SystemInfo.deviceModel);
        if (UnityEngine.SystemInfo.deviceModel.Contains("HUAWEI"))
        {
            Indirect = false;
            Debug.Log("TJiaGrassGenerator.OnEnable: " + Indirect);
        }

        mTmpRT = RenderTexture.GetTemporary(InteractiveGrassTexture.descriptor);

        _instance = this;

        mTmpRT.name = "TJia_Grass_Interactive";
        Shader.SetGlobalTexture("_InteractiveGrass", InteractiveGrassTexture);

        if (GameObject.Find("TJiaGrassFX"))
        {
            GrassFXTr = GameObject.Find("TJiaGrassFX").transform;
            GrassFX = GrassFXTr.GetComponent<ParticleSystem>();
        }

        for (int i = 0; i < IMs.Length; i++)
        {
            IMs[i].Indirect = Indirect;
            IMs[i].Initialization();
            IMs[i].RepeatRate = 0.7f + 1.5f * (float)i;
        }
    }

    void OnDisable()
    {
        if (_instance == this) { _instance = null; }
        RenderTexture.ReleaseTemporary(mTmpRT);
        if (Indirect)
        {
            for (int i = 0; i < IMs.Length; i++)
            {
                IMs[i].Destroy();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            CircleSkill(2);
        }
        else if (Input.GetKeyDown(KeyCode.Period))
        {
            SectorSkill(2, 90);
        }
        else if (Input.GetKeyDown(KeyCode.Slash))
        {
            SquareSkill(2, 4);
        }


        if (mCpt)
        {
            Graphics.Blit(InteractiveGrassTexture, mTmpRT, InteractiveGrassMat, 0);
        }
        else
        {
            Graphics.Blit(mTmpRT, InteractiveGrassTexture, InteractiveGrassMat, 0);
        }

        mCpt = !mCpt;

        for (int i = 0; i < IMs.Length; i++)
        {
            IMs[i].Update();
        }
    }

    public void CircleSkill(float range)
    {
        InteractiveGrassMat.SetFloat("_SkillRange", range);
        Graphics.Blit(mTmpRT, InteractiveGrassTexture, InteractiveGrassMat, 1);
        if (GrassFXTr != null && PlayerPos != null)
        {
            GrassFXTr.position = PlayerPos.position;
            GrassFXTr.rotation = PlayerPos.rotation;
            var fxShape = GrassFX.shape;
            fxShape.shapeType = ParticleSystemShapeType.Cone;
            fxShape.arc = 360f;
            fxShape.position = Vector3.zero;
            fxShape.scale = Vector3.one * range;

            GrassFX.Emit((int)(16f * range));
        }
    }

    public void SectorSkill(float range, float angle)
    {
        InteractiveGrassMat.SetFloat("_SkillRange", range);
        InteractiveGrassMat.SetFloat("_SkillAngle", angle * Mathf.Deg2Rad);
        Graphics.Blit(mTmpRT, InteractiveGrassTexture, InteractiveGrassMat, 2);
        if (GrassFXTr != null && PlayerPos != null)
        {
            GrassFXTr.position = PlayerPos.position;
            GrassFXTr.rotation = PlayerPos.rotation;
            var fxShape = GrassFX.shape;
            fxShape.shapeType = ParticleSystemShapeType.Cone;
            fxShape.arc = 180f;
            fxShape.position = Vector3.zero;
            fxShape.scale = Vector3.one * range;

            GrassFX.Emit((int)(8f * range));
        }
    }

    public void SquareSkill(float width, float length)
    {
        InteractiveGrassMat.SetFloat("_SkillRange", width * 0.5f);
        InteractiveGrassMat.SetFloat("_SkillLength", length);
        Graphics.Blit(mTmpRT, InteractiveGrassTexture, InteractiveGrassMat, 3);
        if (GrassFXTr != null && PlayerPos != null)
        {
            GrassFXTr.position = PlayerPos.position;
            GrassFXTr.rotation = PlayerPos.rotation;
            var fxShape = GrassFX.shape;
            fxShape.shapeType = ParticleSystemShapeType.Box;
            Vector3 pos = Vector3.zero;
            length *= 1.5f;
            pos.z += length * 0.5f;
            fxShape.position = pos;
            Vector3 zone = Vector3.one;
            zone.x *= width;
            zone.y *= length;
            zone.z *= 0.01f;
            fxShape.scale = zone;

            GrassFX.Emit((int)(4f * length));
        }
    }

    /*public int GrassDensity = 1;
    public int Radias = 80;

    private int InstanceCount = 100000;
    public Mesh InstanceMesh;
    public Material InstanceMaterial;
    private int SubMeshIndex = 0;

    private int mCachedInstanceCount = -1;
    private ComputeBuffer mPositionBuffer;
    private ComputeBuffer mArgsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };*/

    // Use this for initialization
    /*void Start () {
        mArgsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
	}*/
    /*void Update () {
        if (mCachedInstanceCount != InstanceCount)
        {
            UpdateBuffers();
        }

        if (Input.GetAxisRaw("Horizontal") != 0f)
        {
            InstanceCount = (int)Mathf.Clamp(InstanceCount + Input.GetAxis("Horizontal") * 40000, 1f, 5000000f);
        }

        InstanceMaterial.SetFloat("_GrassDensity", 1f / (float)GrassDensity);


        Graphics.DrawMeshInstancedIndirect(InstanceMesh, SubMeshIndex, InstanceMaterial, new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000)), mArgsBuffer);
	}*/

    /*void OnGUI()
    {
        GUI.Label(new Rect(265, 25, 200, 30), "Instance Count : " + InstanceCount.ToString());
        InstanceCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)InstanceCount, 1.0f, 5000000.0f);
    }*/

    /*void UpdateBuffers()
    {
        if (InstanceMesh != null)
        {
            SubMeshIndex = Mathf.Clamp(SubMeshIndex, 0, InstanceMesh.subMeshCount - 1);
        }

        if (mPositionBuffer != null)
        {
            mPositionBuffer.Release();
        }

        int squareNum = Radias * GrassDensity;

        List<Vector4> posBuffer = new List<Vector4>();

        for (int i = 0; i < squareNum; i++)
        {
            float posX = ((float)i / (float)(squareNum) - 0.5f) * 2f;
            for (int j = 0; j < squareNum; j++)
            {
                float posZ = ((float)j / (float)(squareNum) - 0.5f) * 2f;
                if ((posZ > 0 && posX * posX + posZ * posZ < 1f) || posX * posX + posZ * posZ < 0.04f)
                {
                    posBuffer.Add(new Vector4(i - squareNum / 2, 0, j - squareNum / 2, 0));
                }
            }
        }

        InstanceCount = posBuffer.Count;

        mPositionBuffer = new ComputeBuffer(InstanceCount, 16);
        Vector4[] positions = new Vector4[InstanceCount];

        for (int i = 0; i < InstanceCount; i++)
        {
            positions[i] = posBuffer[i];
        }

        posBuffer.Clear();
        posBuffer = null;

        mPositionBuffer.SetData(positions);
        InstanceMaterial.SetBuffer("_PositionBuffer", mPositionBuffer);

        if (InstanceMesh != null)
        {
            args[0] = (uint)InstanceMesh.GetIndexCount(SubMeshIndex);
            args[1] = (uint)InstanceCount;
            args[2] = (uint)InstanceMesh.GetIndexStart(SubMeshIndex);
            args[3] = (uint)InstanceMesh.GetBaseVertex(SubMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }

        mArgsBuffer.SetData(args);

        mCachedInstanceCount = InstanceCount;
    }*/

    /*void OnDisable()
    {
        if (mPositionBuffer != null)
        {
            mPositionBuffer.Release();
        }
        mPositionBuffer = null;
        if (mArgsBuffer != null)
        {
            mArgsBuffer.Release();
        }
        mArgsBuffer = null;
    }*/
}
