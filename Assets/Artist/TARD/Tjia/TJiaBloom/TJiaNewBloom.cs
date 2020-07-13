using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

[ExecuteInEditMode]
public class TJiaNewBloom : MonoBehaviour {

    [NoToLua]
    public Material BloomMat;

    [NoToLua]
    [SerializeField] public float _Threshold = 1.2f;
    [NoToLua]
    [SerializeField] public float _Intensity = 1.0f;

    //[SerializeField, Range(0, 1)] public float _SoftKnee = 0.5f;

    private List<RenderTexture> TRTs = new List<RenderTexture>();

    private RenderTexture mTmpRT1;
    private RenderTexture mTmpRT2;
    //public RenderTexture mTmpRT3;

    [NoToLua]
    [Range(0,1)]public float GussianForce = 0.2f;

    private bool mBlock = false;

    // Use this for initialization
    void OnEnable () {
        mBlock = false;

    }

    void OnDisable()
    {
        mBlock = true;
        for (int i = 0; i < TRTs.Count; i++)
        {
            if (TRTs[i] != null)
            {
                RenderTexture.ReleaseTemporary(TRTs[i]);
            }
        }
        TRTs.Clear();
    }

    int mTw, mTh;

    private RenderTexture GetRT(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "TJia Bloom RT")
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw);
        rt.filterMode = filterMode;
        rt.wrapMode = wrapMode;
        rt.name = name;
        TRTs.Add(rt);
        return rt;
    }

    void Update () {
        if (mTmpRT1 == null && mTw * mTh != 0)
        {
            bool useRGBM = Application.isMobilePlatform;
            RenderTextureFormat mRTFormat = useRGBM ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
            mTmpRT1 = GetRT(mTw / 02, mTh / 02, 0, mRTFormat);
            mTmpRT2 = GetRT(mTw / 04, mTh / 04, 0, mRTFormat);
            //mTmpRT3 = GetRT(mTw / 08, mTh / 08, 0, mRTFormat);
        }
        BloomMat.SetFloat("_Threshold", _Threshold);
        BloomMat.SetFloat("_Intensity", _Intensity);
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!mBlock)
        {
            mTw = source.width;
            mTh = source.height;

            //float knee = _Threshold * _SoftKnee + 1e-5f;
            //Vector3 curve = new Vector3(_Threshold - knee, knee * 2, 0.25f / knee);

            BloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
            Graphics.Blit(source, mTmpRT1, BloomMat, 0);
            //BloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
            BloomMat.SetTexture("_OriginalTex", mTmpRT1);
            Graphics.Blit(mTmpRT1, mTmpRT2, BloomMat, 3);
            //BloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
            /*Graphics.Blit(mTmpRT2, mTmpRT3, BloomMat, 1);
            BloomMat.SetFloat("_GaussianScale", GussianForce * 0.0625f);
            BloomMat.SetTexture("_OriginalTex", mTmpRT3);
            Graphics.Blit(mTmpRT3, mTmpRT2, BloomMat, 3);*/
            //BloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
            BloomMat.SetTexture("_OriginalTex", mTmpRT2);
            Graphics.Blit(mTmpRT2, mTmpRT1, BloomMat, 3);
            //BloomMat.SetFloat("_GaussianScale", GussianForce * 1.0f);
            BloomMat.SetTexture("_OriginalTex", source);
            Graphics.Blit(mTmpRT1, destination, BloomMat, 3);
        }
    }
}
