using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceBlender : MonoBehaviour
{
    public bool isMainPlayer = false;
    public bool UpdateFace = false;
    public bool ContinusUpdate = false;
    [Space(20)]
    public Texture2D FaceTex;
    public Texture2D FaceMask;
    [Space(20)]
    public Texture2D EyeTex;
    public Texture2D EyeMask;
    [ColorUsage(true, true)] public Color EyeColor = Color.white;
    public Vector4 EyeZone = new Vector4(0, 0.5f, 0.75f, 1);
    public Vector4 EyeTarget = new Vector4(0, 1, 0, 1);
    [Space(20)]
    public Texture2D MouthTex;
    public Texture2D MouthMask;
    [ColorUsage(true, true)] public Color MouthColor = Color.white;
    public Vector4 MouthZone = new Vector4(0, 0.25f, 0.75f, 1);
    public Vector4 MouthTarget = new Vector4(0, 1, 0, 1);
    [Space(20)]
    public Texture2D MakeupTex;
    [ColorUsage(true, true)] public Color MakeupColor = Color.white;
    public Vector4 MakeupZone = new Vector4(0, 1, 0, 1);
    public Vector4 MakeupTarget = new Vector4(0, 1, 0, 1);
    [Space(20)]
    public RenderTexture FaceOutTex;
    public RenderTexture FaceOutMask;
 public Material BlenderMat;

    private bool hasRenderOnce = false;
    private float notRenderOnceTime = 0f;


    private bool matIsNil = false;

    private float time = 0;
    // Use this for initialization
    private void SetTexture()
    {
     
        if (FaceOutTex == null)
        {
            FaceOutTex = FaceRenderTextureObjectPoolManager.GetInstance().GetRenderTexture(this, true, isMainPlayer);//RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.ARGB32);
            FaceOutTex.name = "FaceOutTex" + transform.name;
   
            if (!Application.isPlaying)
            {
                GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", FaceOutTex);         
            }
            else
            {
                GetComponent<Renderer>().material.SetTexture("_MainTex", FaceOutTex);          
            }
        }

        if (FaceOutMask == null)
        {
            FaceOutMask = FaceRenderTextureObjectPoolManager.GetInstance().GetRenderTexture(this, false, isMainPlayer);
            FaceOutMask.name = "FaceOutMask" + transform.name;
            if (!Application.isPlaying)
            {             
                GetComponent<Renderer>().sharedMaterial.SetTexture("_LightMap", FaceOutMask);
            }
            else
            {                
                GetComponent<Renderer>().material.SetTexture("_LightMap", FaceOutMask);
            }
        }
    }

    [ContextMenu("RenderFace")]
    public void RenderFace()
    {
        SetTexture();
        GenerateNewImage();
        UpdateFace = true;
        hasRenderOnce = true;
    }
    //void OnBacamVisible()
    //{
    //    if (FaceOutTex == null)
    //    {
    //        FaceOutTex = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.ARGB32);
    //        FaceOutMask = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.RGB565);
    //        GetComponent<Renderer>().material.SetTexture("_MainTex", FaceOutTex);
    //        GetComponent<Renderer>().material.SetTexture("_LightMap", FaceOutMask);
    //    }
    //}
    [ContextMenu("release")]
    public void Release()
    {
        if (FaceOutTex != null)
        {
            FaceRenderTextureObjectPoolManager.GetInstance().ReleaseRenderTexture(FaceOutTex, this);
            //RenderTexture.ReleaseTemporary(FaceOutTex);           
            FaceOutTex = null;          
        }

        if (FaceOutMask != null)
        {
            FaceRenderTextureObjectPoolManager.GetInstance().ReleaseRenderTexture(FaceOutMask, this);
            //RenderTexture.ReleaseTemporary(FaceOutMask);
            FaceOutMask = null;
        }
    }

    public void OnEnable()
    {
        if (UpdateFace && hasRenderOnce)
        {
            RenderFace();
        }
    }

    public void OnDisable()
    {
        Release();
    }

    public void OnDestroy()
    {
        Release();
    }

    //void OnBecameInvisible()
    //{
    //    if (FaceOutTex != null)
    //    {
    //        RenderTexture.ReleaseTemporary(FaceOutTex);
    //        RenderTexture.ReleaseTemporary(FaceOutMask);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (FaceOutTex != null && FaceOutMask != null)
        {
            if (UpdateFace)
            {
                GenerateNewImage();
                UpdateFace = false;
                ContinusUpdate = false;
            }
            else if (ContinusUpdate)
            {
                GenerateNewImage();
            }
        }
        else
        {
            if (hasRenderOnce)
            {
                RenderFace();
            }
            else
            {
                notRenderOnceTime += Time.deltaTime;
                if (notRenderOnceTime > 5f)
                {
                    hasRenderOnce = true;
                }
            }
        }

        if (matIsNil)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                time = 3;
                if (FaceOutTex == null || FaceOutMask == null)
                {
                    RenderFace();
                }
                else
                {
                    GenerateNewImage();
                }
            }
        }
    }

    private void GenerateNewImage()
    {
        if (FaceOutTex != null && FaceOutMask != null)
        {
            if (BlenderMat != null)
            {
                if (FaceMask != null)
                {
                    BlenderMat.SetTexture("_LightMap", FaceMask);
                }
                else
                {
                    //Debug.LogError("FaceMask is nil");
                }
                if (EyeTex != null)
                {
                    BlenderMat.SetTexture("_EyeTex", EyeTex);
                }
                else
                {
                    //Debug.LogError("EyeTex is nil");
                }

                if (EyeMask != null)
                {
                    BlenderMat.SetTexture("_LightMapEye", EyeMask);
                }
                else
                {
                    //Debug.LogError("EyeMask is nil");
                }

                if (MouthTex != null)
                {
                    BlenderMat.SetTexture("_MouthTex", MouthTex);
                }
                else
                {
                    //Debug.LogError("MouthTex is nil");
                }

                if (MouthMask != null)
                {
                    BlenderMat.SetTexture("_LightMapMouth", MouthMask);
                }
                else
                {
                    //Debug.LogError("MouthMask is nil");
                }

                if (MakeupTex != null)
                {
                    BlenderMat.SetTexture("_MakeupTex", MakeupTex);
                }
                else
                {
                    //Debug.LogError("MakeupTex is nil");
                }


                BlenderMat.SetVector("_EyeOri", EyeZone);
                BlenderMat.SetVector("_MouthOri", MouthZone);
                BlenderMat.SetVector("_MakeupOri", MakeupZone);
                BlenderMat.SetVector("_EyeTar", EyeTarget);
                BlenderMat.SetVector("_MouthTar", MouthTarget);
                BlenderMat.SetVector("_MakeupTar", MakeupTarget);

                BlenderMat.SetColor("_EyeColor", EyeColor);
                BlenderMat.SetColor("_MouthColor", MouthColor);
                BlenderMat.SetColor("_MakeupColor", MakeupColor);



                Graphics.Blit(FaceTex, FaceOutTex, BlenderMat, 0);
                Graphics.Blit(FaceTex, FaceOutMask, BlenderMat, 1);
                matIsNil = false;
            }
            else
            {
                Debug.LogWarning("BlenderMat is nil");
                matIsNil = true;
            }
        }
        else
        {
            //Debug.LogError("FaceOutTex or FaceOutMask is nil");
        }
    }
}
