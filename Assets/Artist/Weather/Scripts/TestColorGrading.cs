using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    public Color MainColor = new Color(1, 1, 1, 1);

    [Range(0, 255)] public float InBlack = 0;
    [Range(0, 255)] public float InWhite = 255.0f;
    [Range(0, 10)] public float InGamma = 1.61f;
    [Range(0, 255)] public float OutBlack = 0;
    [Range(0, 255)] public float OutWhite = 255.0f;

    public AnimationCurve RedChannel;
    public AnimationCurve GreenChannel;
    public AnimationCurve BlueChannel;
    
    private Material m_Material = null;
    private Texture2D m_CurveTex = null;
    private int m_Width = 1024;

    private Material material
    {
        get
        {
            if (m_Material == null)
            {
                Shader testColorShader = UnityUtils.FindShader("PostEffect/TestColor");
                m_Material = new Material(testColorShader);
            }
            return m_Material;
        }
    }


    private void InitCurveTexture()
    {
        if (m_CurveTex == null)
        {
            m_CurveTex = new Texture2D(m_Width, 1, TextureFormat.ARGB32, false);
            m_CurveTex.filterMode = FilterMode.Bilinear;
            m_CurveTex.wrapMode = TextureWrapMode.Clamp;
        }

        for (int x = 0; x < m_Width; x++)
        {
            Color piexColor = new Color(0, 0, 0, 1);
            float rate = (float)x / (float)m_Width;
            float redValue = RedChannel.Evaluate(rate);
            float greenValue = GreenChannel.Evaluate(rate);
            float blueValue = BlueChannel.Evaluate(rate);

            piexColor.r = redValue;
            piexColor.g = greenValue;
            piexColor.b = blueValue;

            m_CurveTex.SetPixel(x, 0, piexColor);
        }

        m_CurveTex.Apply();

    }


    void OnEnable()
    {
        //
    }

    void OnDisable()
    {
        if (m_CurveTex != null) m_CurveTex = null;
    }

    //属性有变化时触发
    void OnValidate()
    {
        this.InitCurveTexture();
    }

    // Use this for initialization
    void Start () {
        this.InitCurveTexture();
    }

    // Update is called once per frame
    void Update () {
		//
	}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetColor("_MainColor",MainColor);

        material.SetFloat("_InBlack", InBlack);
        material.SetFloat("_InWhite", InWhite);
        material.SetFloat("_InGamma", InGamma);
        material.SetFloat("_OutBlack", OutBlack);
        material.SetFloat("_OutWhite", OutWhite);

        material.SetTexture("_CurveTex", m_CurveTex);
        Graphics.Blit(source, destination, material);

    }






}
