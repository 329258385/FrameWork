using AmplifyImpostors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TexDrawer : MonoBehaviour
{

#if UNITY_EDITOR
    [System.Serializable]
    public class ObjAndMask
    {
        public Transform Obj;
        public Texture2D Mask;
        internal Material ShowMat;
        internal Material DrawMat;
        [HideInInspector] public Material OriMat;
        [HideInInspector] public RenderTexture RTMask;
        [HideInInspector] public RenderTexture BrushMask;
        [HideInInspector] public RenderTexture TempRT;
        [HideInInspector] public RenderTexture LastStep;
        [HideInInspector] public bool Readable;
        internal int mOriLayer = -1;
        //internal Vector2 BrushPos;
        internal float BrushSize = 1;

        /*ObjAndMask()
        {
            Setup();
        }*/

        internal void Setup()
        {
            ShowMat = new Material(UnityUtils.FindShader("TJia/TexDrawer"));
            DrawMat = new Material(UnityUtils.FindShader("TJia/TexBlender"));
            RTMask = RenderTexture.GetTemporary(Mask.width, Mask.height, 0, RenderTextureFormat.ARGB32);
            TempRT = RenderTexture.GetTemporary(RTMask.descriptor);
            LastStep = RenderTexture.GetTemporary(RTMask.descriptor);
            BrushMask = RenderTexture.GetTemporary(RTMask.descriptor);
            RTMask.name = Obj.name + "_Draw_Mask";
            TempRT.name = Obj.name + "_Temp_RT";
            LastStep.name = Obj.name + "_Last_Step";
            BrushMask.name = Obj.name + "_Brush_Mask";
            Graphics.Blit(Mask, RTMask, DrawMat, 3);
            Graphics.Blit(RTMask, BrushMask, DrawMat, 0);
            ShowMat.SetTexture("_MainTex", RTMask);
            ShowMat.SetTexture("_BrushMask", BrushMask);
            OriMat = Obj.GetComponent<Renderer>().sharedMaterial;
            Obj.GetComponent<Renderer>().material = ShowMat;
            BrushSize = 1;
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(Mask));
            Readable = ti.isReadable;
        }

        internal void EndStep()
        {
            if (OriMat != null)
            {
                Obj.GetComponent<Renderer>().sharedMaterial = OriMat;
            }
            if (RTMask != null)
            {
                RenderTexture.ReleaseTemporary(RTMask);
                RenderTexture.ReleaseTemporary(TempRT);
                RenderTexture.ReleaseTemporary(LastStep);
                RenderTexture.ReleaseTemporary(BrushMask);
                RTMask = null;
                TempRT = null;
                ShowMat = null;
                DrawMat = null;
            }
        }

        public void SaveLastStep()
        {
            Graphics.Blit(RTMask, LastStep);
        }

        internal void ResetOneStep()
        {
            Graphics.Blit(LastStep, RTMask);
        }

        internal void Draw(Vector2 uv, float brushSize)
        {
            Graphics.Blit(BrushMask, TempRT, DrawMat, 1);
            Graphics.Blit(TempRT, BrushMask);
            //Debug.Log("Drawing");
        }

        internal void FinishDraw()
        {
            DrawMat.SetTexture("_RTMask", RTMask);
            Graphics.Blit(BrushMask, TempRT, DrawMat, 2);
            Graphics.Blit(TempRT, RTMask);
            Graphics.Blit(RTMask, BrushMask, DrawMat, 0);
        }
        internal void Save()
        {
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(Mask));
            ti.isReadable = true;
            ti.SaveAndReimport();

            string path = AssetDatabase.GetAssetPath(Mask);
            string extension = Path.GetExtension(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            string location = Path.GetDirectoryName(path);
            //Debug.Log(location + "\\" + filename + ".jpg");
            //Debug.Log(extension);

            int type = 0; //no type
            if (extension.Contains("png"))
            {
                type = 1;
            }
            else if (extension.Contains("jpg"))
            {
                type = 2;
            }
            else if (extension.Contains("tga"))
            {
                type = 3;
            }

            byte[] texdata;
            FileStream ss;

            RenderTexture.active = RTMask;
            Texture2D ResTex = new Texture2D(RTMask.width, RTMask.height);
            ResTex.ReadPixels(new Rect(0, 0, RTMask.width, RTMask.height), 0, 0);
            ResTex.Apply();
            RenderTexture.active = null;

            switch (type)
            {
                case 1:
                    {
                        texdata = ResTex.EncodeToPNG();
                        ss = File.OpenWrite(path);
                        ss.Write(texdata, 0, texdata.Length);
                        ss.Close();
                        break;
                    }
                case 2:
                    {
                        texdata = ResTex.EncodeToJPG();
                        ss = File.OpenWrite(path);
                        ss.Write(texdata, 0, texdata.Length);
                        ss.Close();
                        break;
                    }
                case 3:
                    {
                        texdata = ResTex.EncodeToTGA();
                        ss = File.OpenWrite(path);
                        ss.Write(texdata, 0, texdata.Length);
                        ss.Close();
                        break;
                    }
            }

            ti.isReadable = Readable;
            ti.SaveAndReimport();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////


    internal Vector2 Uv;

    [Header("编辑")]
    [Space(20)]

    public LayerMask DrawLayer;

    public bool Edit = false;

    public bool Reset = false;

    public enum PaintChanel
    { RGB, R, G, B };

    public PaintChanel Chanel = PaintChanel.RGB;

    [Header("笔刷")]
    [Space(20)]

    public float BrushSize = 1;

    [Range(1, 50)] public float BrushSoftness = 1;

    [Range(1, 50)] public int Flow = 1;

    [Header("绘制列表")]
    [Space(20)]

    public List<ObjAndMask> ObjAndMaskList = new List<ObjAndMask>();

    [HideInInspector] public ObjAndMask Target;

    [HideInInspector] public bool Clean = false;

    public Color BrushColor = Color.black;

    internal bool PaintState = false;

    private int Cpt = 0;

    /*void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 1);
        Gizmos.DrawSphere(HitPos, 0.1f);
    }*/

    public void BrushSizeModifer(float value)
    {
        BrushSize = BrushSize * value;
        //Shader.SetGlobalFloat("_BrushSize", BrushSize);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        for (int i = 0; i < ObjAndMaskList.Count; i++)
        {
            //ObjAndMaskList[i].Save();
            ObjAndMaskList[i].EndStep();
        }
        Reset = true;
        Clean = true;
        Edit = false;
        Target = null;
    }

    private void Init()
    {
        Shader.SetGlobalFloat("_BrushSize", 0);
    }

    public void UpdateHitInfo(Transform hitTrans, Vector2 uv, bool paint, bool finish)
    {
        if (Edit)
        {
            Uv = uv;
            if (Target != null && Target.Obj == hitTrans && Target.ShowMat != null)
            {
                Target.ShowMat.SetVector("_BrushPos", Uv);
                Target.ShowMat.SetFloat("_BrushSize", BrushSize);
                Target.DrawMat.SetVector("_BrushPos", Uv);
                Target.DrawMat.SetFloat("_BrushSize", BrushSize);
                Target.DrawMat.SetColor("_BrushColor", BrushColor);

                if (paint)
                {
                    /*if (PaintState == false)
                    {
                        Target.SaveLastStep();
                    }*/
                    Cpt++;
                    PaintState = true;
                    if (Cpt % Flow == 0)
                    {
                        Target.Draw(Uv, BrushSize);
                    }
                }
                else if (PaintState == true && finish)
                {
                    PaintState = false;
                    Target.FinishDraw();
                }
            }
            else
            {
                if (Target != null && Target.ShowMat != null)
                {
                    Target.ShowMat.SetFloat("_BrushSize", 0);
                    Target = null;
                }
                for (int i = 0; i < ObjAndMaskList.Count; i++)
                {
                    if (ObjAndMaskList[i].Obj == hitTrans)
                    {
                        Target = ObjAndMaskList[i];
                        break;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (Selection.activeGameObject != gameObject)
        {
            Edit = false;
        }
        /*else
        {
            Edit = true;
        }*/
        if (Edit)
        {
            Shader.SetGlobalFloat("_BrushSoftness", BrushSoftness);

            switch (Chanel)
            {
                case PaintChanel.RGB:
                    Shader.SetGlobalVector("_RGB", Vector3.one);
                    break;
                case PaintChanel.R:
                    Shader.SetGlobalVector("_RGB", Vector3.right);
                    break;
                case PaintChanel.G:
                    Shader.SetGlobalVector("_RGB", Vector3.up);
                    break;
                case PaintChanel.B:
                    Shader.SetGlobalVector("_RGB", Vector3.forward);
                    break;
            }

            if (Reset)
            {
                Reset = false;
                for (int i = 0; i < ObjAndMaskList.Count; i++)
                {
                    if (!Clean)
                    {
                        ObjAndMaskList[i].EndStep();
                    }
                    ObjAndMaskList[i].Setup();
                }
                Clean = false;
            }
        }
        else if (Reset == false)
        {
            for (int i = 0; i < ObjAndMaskList.Count; i++)
            {
                //ObjAndMaskList[i].Save();
                ObjAndMaskList[i].EndStep();
            }
            Reset = true;
            Clean = true;
        }
    }

    public void SavePic()
    {
        Chanel = PaintChanel.RGB;
        Shader.SetGlobalVector("_RGB", Vector3.one);
        for (int i = 0; i < ObjAndMaskList.Count; i++)
        {
            ObjAndMaskList[i].Save();
        }
    }

    public void ResetOneStep()
    {
        if (Target != null)
        {
            Target.ResetOneStep();
        }
    }
#endif
}
