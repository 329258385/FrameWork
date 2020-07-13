using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostShadow : MonoBehaviour

{
    private bool _ghostActive = false;
    public bool GhostActive
    {
        set {
            _ghostActive = value;
            lastTime = 0;
        }
        get { return _ghostActive; }
    }

    //持续时间
    //public float duration = 2f;
    //创建新残影间隔
    public float interval = 0.1f;
    //内外发光颜色
    public Color RimColorInSide = Color.cyan;
    public Color RimColorOutSide = Color.yellow;
    
    //边缘颜色强度
    [Range(0, 3)]
    public float Intension = 1;
    [Range(0, 10)]
    public float Power = 1;

    public Color coDissolveCol = Color.white;

    //网格数据
    SkinnedMeshRenderer[] meshRender;

    //X-ray--已废除，启用TJiaNewCharaTrans
    Shader ghostShader;

    //------------------------------------------------------------------------------------------------------------

    //阴影透明度
    [Range(0, 1)]
    public float mDarkAlpha = 0;
    //亮部范围
    [Range(0, 1)]
    public float mLightRange = 0;
    //边缘发光颜色
    public Color mRimColorHDR = Color.cyan;
    //边缘光范围
    [Range(0, 10)]
    public float mRimPower = 0;
    //消散颜色
    public Color mDisPercentageColorHDR = Color.yellow;
    //消散尺寸
    [Range(0.1f, 10)]
    public float mDisScale = 0;
    //消散时间
    public float mfDissolveTime = 1;
    //单个残影维持时间
    public float mKeepTime = 0;



    void Start()
    {
        //获取身上所有的Mesh
        meshRender = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        ghostShader = ShaderBundleManager.FindBundleShader("SAO_TJia_V3/NewChara/NewCharaTransparent");//"SAO_TJia/Xray_Distortion");//UnityUtils.FindShader("SAO_TJia/Xray_Distortion");
    }

    private float lastTime = 0;
    private Vector3 lastPos = Vector3.zero;
    //private float curDissolveVal = 0;


    void CreateGhost()
    {
        if (meshRender == null)
            return;
        for (int i = 0; i < meshRender.Length; i++)
        {
            Mesh mesh = new Mesh();
            if (meshRender[i] == null)
                continue;
            meshRender[i].BakeMesh(mesh);

            GameObject go = new GameObject("Ghost_"+ this.gameObject.name);
            go.layer = 1;//transFX
            go.hideFlags = HideFlags.HideAndDontSave;

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshRenderer meshRen = go.AddComponent<MeshRenderer>();
            meshRen.material = meshRender[i].material;
            InitMaterial(meshRen.material);
/*
            meshRen.material = meshRender[i].material;
            meshRen.material.shader = ghostShader;//设置xray效果
            meshRen.material.SetFloat("_Intension", Intension);//颜色强度传入shader中
            meshRen.material.SetVector("_RimColor", RimColorOutSide * Mathf.Pow(2f,2.8f));//颜色强度传入shader中
            //meshRen.material.SetVector("_EmissionColor", Color.white * 4.2354f);
            meshRen.material.SetVector("_RimColor2", RimColorInSide * Mathf.Pow(2f,1.7169f));//颜色强度传入shader中
            //meshRen.material.SetVector("_EmissionColor", Color.white * 1.7169f);
            meshRen.material.SetFloat("_Power", Power);//颜色强度传入shader中
            meshRen.material.SetVector("_DisColor", coDissolveCol * Mathf.Pow(2f, 1.5f));
            //固有值(取值请见xray_distortion的prefab)
            meshRen.material.SetFloat("_DistortionForce", 0.02f);
            meshRen.material.SetFloat("_DistortionScale", 0.18f);
            meshRen.material.SetFloat("_RefractionForce", 0.05f);
            meshRen.material.SetFloat("_DisTortionSpeed", 0.1f);
            meshRen.material.SetFloat("_InnerDarkness", 1f);
*/
            go.transform.localScale = meshRender[i].transform.localScale;
            go.transform.position = meshRender[i].transform.position;
            go.transform.rotation = meshRender[i].transform.rotation;

            GhostItem item = go.AddComponent<GhostItem>();//控制残影消失
            item.SetDataAndRun(mfDissolveTime, Time.time + mfDissolveTime + mKeepTime, mKeepTime, meshRen);
        }
    }

    void InitMaterial(Material mat)
    {
        //固有值
        mat.shader = ghostShader;
        mat.SetFloat("_Type", 0);//全部Original
        mat.SetColor("_Color", new Color(0, 0, 0.05f, 0));//默认颜色为透明
        mat.SetTexture("_MainTex", null);   //贴图去除
        mat.SetTexture("_LightMap", null);
        mat.SetFloat("_OutlineWidth",0);    //描边设0
        mat.SetColor("_Color2", new Color(0, 0, 0.05f, 0));//这是什么颜色？-- 这是挑染发色的颜色 所以也要统一这只和_Color一致 --by 王健宁

        //动态值
        mat.SetFloat("_DarkAlpha", mDarkAlpha);//阴影透明度
        mat.SetFloat("_LightRange", mLightRange);//亮部范围
        mat.SetColor("_RimShieldColor", mRimColorHDR);//边缘发光颜色
        mat.SetFloat("_RimPower", mRimPower);//边缘发光范围
        mat.SetColor("_DisColor", mDisPercentageColorHDR);//消散颜色
        mat.SetFloat("_DisScale", mDisScale);//消散尺寸
    }

    void Update()
    {
        //人物有位移才创建残影
        if (!GhostActive)// && lastPos == this.transform.position)
        {
            return;
        }
        lastPos = this.transform.position;
        if (Time.time - lastTime < interval)
        {//残影间隔时间
            return;
        }
        lastTime = Time.time;
        CreateGhost();
    }
}