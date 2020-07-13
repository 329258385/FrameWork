using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Fog")]
[RequireComponent(typeof(Camera))]
public class FogEffect : UnityStandardAssets.ImageEffects.PostEffectsBase
{
    public enum AttenuationType { Linear, Exponent, ExponentSquare }

    [HideInInspector] public Material material = null;
    [Header("Fog")]
    public Shader fogShader = null;
    public Gradient fogColorRamp = new Gradient();
    public bool fogColorMultiplyLight = false;
    [Range(0, 1)]
    public float fogColorIntensityFactor = 1;
    [HideInInspector] public Gradient gradientSample = new Gradient();
    public float fogDensity;
    [Space(10)]
    public bool useSkyDensity = false;
    [Range(0, 1)] public float skyDensity = 1;
    public AttenuationType fogAttenuationType = AttenuationType.Exponent;
    [Range(0, 1)]
    public float fogStartDepth;
    [Space(10)]
    public bool useFogHeight;
    public float fogHeight;
    public float fogHeightScale;

    [Header("Mie Scatter")]
    [Range(0, 1)]
    public float miePhaseAnisotropy = 0.9f;
    public Gradient worldMieColorRamp = new Gradient();
    public float worldMieColorIntensity = 1.0f;
    public float worldMieDensity;
    public float fogMieDensity;
    public float heightMieDistanceScale;
    public Transform sunSource;
    // Use this for initialization
    public override bool CheckResources()
    {
        CheckSupport(true);

        if (!fogShader)
            fogShader = UnityUtils.FindShader("Hidden/DepthHeightFog2");

        material = CheckShaderAndCreateMaterial(fogShader, material);

        if (!isSupported)
            ReportAutoDisable();

        return isSupported;
    }
    //private void OnDisable()
    //{
    //    Debug.Log("Disable");
    //}
    //private void OnEnable()
    //{
    //    Debug.Log("OnEnable");
    //}
    //[ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Camera cam = GetComponent<Camera>();
        //cam.depthTextureMode |= DepthTextureMode.Depth;
        var shouldRender =
            CheckResources()
            && ((cam && cam.actualRenderingPath == RenderingPath.DeferredShading));

        if (!shouldRender)
        {
            Graphics.Blit(source, destination);
            return;
        }
        if (material)
        {
            //Camera cam = Camera.main;
            //Transform CamTrans = cam.transform;
            //float near = cam.nearClipPlane;
            //float far = cam.farClipPlane;
            //float halfHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            //Vector3 toRight = CamTrans.right * halfHeight * cam.aspect;
            //Vector3 upVector = CamTrans.up * halfHeight;
            //Vector3 topVector = CamTrans.forward * near + upVector;
            //Vector3 bottomVector = CamTrans.forward * near - upVector;

            //Vector3 bottomLeft = bottomVector - toRight; //左下
            //Vector3 bottomRight = bottomVector + toRight; //右下
            //Vector3 topLeft = topVector - toRight; //左上
            //Vector3 topRight = topVector + toRight; //右上
            //List<Vector4> l = new List<Vector4>
            //{
            //    bottomLeft,bottomRight,topLeft,topRight
            //};
            //material.SetVectorArray("_CameraMat", l);
            int num = 7;
            List<Vector4> l = new List<Vector4>();
            for (float i = 0; i <= 1.0f; i += 1.0f / num)
            {
                var c = fogColorRamp.Evaluate(i);
                if (fogColorMultiplyLight)
                {

                    var light = GameObject.FindGameObjectWithTag("MainLight").GetComponent<Light>();
                    c *= light.intensity * fogColorIntensityFactor;
                }
                l.Add(c);
            }
            material.SetVectorArray("_ColorGradient", l);
            material.SetInt("_ColorGradient_Num", l.Count - 1);
            material.SetFloat("_FogDensity", fogDensity);
            material.SetFloat("_FogStartDepth", fogStartDepth);
            material.SetFloat("_SkyFogDensity", skyDensity);
            if (useSkyDensity)
                material.EnableKeyword("FOG_SKY");
            else
                material.DisableKeyword("FOG_SKY");
            if (useFogHeight)
                material.EnableKeyword("FOG_USE_HEIGHT");
            else
                material.DisableKeyword("FOG_USE_HEIGHT");
            material.DisableKeyword("FOG_ATTENUATION_LINEAR");
            material.DisableKeyword("FOG_ATTENUATION_EXPONENT");
            material.DisableKeyword("FOG_ATTENUATION_EXPONENT_SQUARE");
            switch (fogAttenuationType)
            {
                case AttenuationType.Linear:
                    material.EnableKeyword("FOG_ATTENUATION_LINEAR");
                    break;
                case AttenuationType.Exponent:
                    material.EnableKeyword("FOG_ATTENUATION_EXPONENT");
                    break;
                case AttenuationType.ExponentSquare:
                    material.EnableKeyword("FOG_ATTENUATION_EXPONENT_SQUARE");
                    break;
            }
            material.SetFloat("_FogHeight", fogHeight);
            material.SetFloat("_FogHeightScale", fogHeightScale);



            var mieColorM20 = worldMieColorRamp.Evaluate(0.00f);
            var mieColorO00 = worldMieColorRamp.Evaluate(0.50f);
            var mieColorP20 = worldMieColorRamp.Evaluate(1.00f);
            material.SetFloat("u_MiePhaseAnisotropy", miePhaseAnisotropy);
            material.SetFloat("u_WorldMieDensity", worldMieDensity);
            material.SetFloat("u_HeightMieDensity", fogMieDensity);
            material.SetVector("u_MieColorM20", (Vector4)mieColorM20 * worldMieColorIntensity);
            material.SetVector("u_MieColorO00", (Vector4)mieColorO00 * worldMieColorIntensity);
            material.SetVector("u_MieColorP20", (Vector4)mieColorP20 * worldMieColorIntensity);
            material.SetFloat("u_HeightMieDistanceScale", heightMieDistanceScale);

            var d = -sunSource.forward; d.Normalize();
            material.SetVector("u_SunDirection", new Vector4(d.x, d.y, d.z, 1));
            Graphics.Blit(source, destination, material);
            GradientColorKey[] gck = new GradientColorKey[l.Count];
            GradientAlphaKey[] gak = new GradientAlphaKey[l.Count];
            for (int i = 0; i < l.Count; ++i)
            {
                gak[i].alpha = l[i].w;
                gak[i].time = 1.0f / num * i;
                gck[i].color = new Color(l[i].x, l[i].y, l[i].z);
                gck[i].time = 1.0f / num * i;
            }
            gradientSample.SetKeys(gck, gak);
        }
    }
}
