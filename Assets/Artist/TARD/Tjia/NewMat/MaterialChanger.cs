using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class MaterialChanger : MonoBehaviour {

    public bool Change = false;

#if !UNITY_EDITOR
    void Start()
    {
        Destroy(this);
    }
#endif

#if UNITY_EDITOR

    void Start()
    {
        Change = false;
    }
    //private RenderingController mRC;

    void Update()
    {
        if (Application.isPlaying)
        {
            Change = false;
            Destroy(this);
        }
        if (Change)
        {
            Debug.Log(Change);
            Change = false;

            //mRC = Camera.main.GetComponent<RenderingController>();
            //if (mRC == null)
            //{
            //    mRC = Camera.main.gameObject.AddComponent<RenderingController>();
            //}

            if (RenderSettings.sun != null)
            {
                RenderSettings.sun.shadows = LightShadows.Soft;
                QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                QualitySettings.shadowDistance = 50;

                RenderSettings.ambientMode = AmbientMode.Trilight;
            }
            else
            {
                Debug.LogError("Lighting中没有设置主光源，请设置后重试");
                return;
            }

            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;

            if (RenderSettings.customReflection == null)
            {
                Debug.LogError("Lighting中没有设置反射Cubemap(需要开启mipmap)，请设置后重试");
                return;
            }

            Debug.LogError("请在GraphicsSettings中开启HDR，并选择FP16模式，如已开启请忽略");

            foreach (Renderer rd in FindObjectsOfType<Renderer>())
            {

                foreach (Material mat in rd.sharedMaterials)
                {
                    if (mat != null && mat.shader != null)
                    {
                        if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_Original"))
                        {
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Opaque)");
                            if (mat.GetTexture("_EmissionTex") != null)
                            {
                                mat.DisableKeyword("_EMISSION_OFF");
                                mat.EnableKeyword("_EMISSION_ON");
                                mat.SetFloat("_Emission", 1);
                            }
                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_AlphaBlend"))
                        {
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(AlphaBlend)");
                            if (mat.GetTexture("_EmissionTex") != null)
                            {
                                mat.DisableKeyword("_EMISSION_OFF");
                                mat.EnableKeyword("_EMISSION_ON");
                                mat.SetFloat("_Emission", 1);
                            }
                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_BreathLight"))
                        {
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Opaque)");
                            if (mat.GetTexture("_EmissionTex") != null)
                            {
                                mat.DisableKeyword("_EMISSION_OFF");
                                mat.EnableKeyword("_EMISSION_BREATH");
                                mat.SetFloat("_Emission", 2);
                            }
                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_Covered"))
                        {
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Terrain)");
                            if (mat.GetTexture("_EmissionTex") != null)
                            {
                                mat.DisableKeyword("_EMISSION_OFF");
                                mat.EnableKeyword("_EMISSION_ON");
                                mat.SetFloat("_Emission", 1);
                            }
                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_Terrain"))
                        {
                            Texture tex = mat.GetTexture("_SplatBump1");
                            float bump = mat.GetFloat("_NormalForce");

                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Terrain)");
                            mat.SetTexture("_BumpTex", tex);
                            mat.SetFloat("_BumpScale", bump);

                            mat.DisableKeyword("_TYPE_COVER");
                            mat.EnableKeyword("_TYPE_TERRAIN");
                            mat.SetFloat("_Type", 1);

                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_RockMask"))
                        {
                            Texture tex1 = mat.GetTexture("_MixTex1");
                            Texture tex2 = mat.GetTexture("_MixTex2");
                            Texture tex3 = mat.GetTexture("_MixTex3");
                            float uv1 = mat.GetFloat("_MixUV1");
                            float uv2 = mat.GetFloat("_MixUV2");
                            float uv3 = mat.GetFloat("_MixUV3");

                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Terrain)");
                            mat.SetTexture("_SplatTex1", tex1);
                            mat.SetTexture("_SplatTex2", tex2);
                            mat.SetTexture("_SplatTex3", tex3);
                            mat.SetFloat("_UvScale1", uv1);
                            mat.SetFloat("_UvScale2", uv2);
                            mat.SetFloat("_UvScale3", uv3);

                            mat.DisableKeyword("_TYPE_COVER");
                            mat.EnableKeyword("_TYPE_MASKEDROCK");
                            mat.SetFloat("_Type", 1);

                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_AlphaCutoff"))
                        {
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Cutoff)");
                            if (mat.GetTexture("_EmissionTex") != null)
                            {
                                mat.DisableKeyword("_EMISSION_OFF");
                                mat.EnableKeyword("_EMISSION_ON");
                                mat.SetFloat("_Emission", 1);
                            }
                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_Tree"))
                        {
                            float type = mat.GetFloat("_AnimationType");
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Cutoff)");
                            if (mat.GetTexture("_EmissionTex") != null)
                            {
                                mat.DisableKeyword("_EMISSION_OFF");
                                mat.EnableKeyword("_EMISSION_ON");
                                mat.SetFloat("_Emission", 1);
                            }
                            if (type == 0)
                            {
                                mat.DisableKeyword("_TYPE_NORMAL");
                                mat.EnableKeyword("_TYPE_TREE");
                                mat.SetFloat("_Type", 1);
                            }
                            else if (type == 1)
                            {
                                mat.DisableKeyword("_TYPE_NORMAL");
                                mat.EnableKeyword("_TYPE_FLOWER");
                                mat.SetFloat("_Type", 2);
                            }
                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                            if (rd.isPartOfStaticBatch)
                            {
                                Debug.LogError("请关闭 " + rd.gameObject.name + " 的Batching Static选项！");
                            }
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/BRDF_Tree_Interactive"))
                        {
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Cutoff)");
                            if (mat.GetTexture("_EmissionTex") != null)
                            {
                                mat.DisableKeyword("_EMISSION_OFF");
                                mat.EnableKeyword("_EMISSION_ON");
                                mat.SetFloat("_Emission", 1);
                            }

                            mat.DisableKeyword("_TYPE_NORMAL");
                            mat.EnableKeyword("_TYPE_INTERACTIVEFLOWER");
                            mat.SetFloat("_Type", 3);

                            mat.enableInstancing = true;
                            mat.SetFloat("_Smoothness", 0);
                            mat.SetFloat("_Metallic", 0);
                            if (rd.isPartOfStaticBatch)
                            {
                                Debug.LogError("请关闭 " + rd.gameObject.name + " 的Batching Static选项！");
                            }
                        }
                        else if (mat.shader == UnityUtils.FindShader("SAO_TJia/Matcap_Toon"))
                        {
                            mat.shader = UnityUtils.FindShader("SAO_TJia_V3/NewChara/NewCharaMain");
                        }
                    }
                }
            }

            foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
            {
                foreach (Material mat in mr.sharedMaterials)
                {
                    if (mat!= null 
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/NewChara/NewCharaMain")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Cutoff)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Terrain)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Opaque)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(AlphaBlend)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia/BRDF_Billboard")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia/BRDF_BillboardFade")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia/BRDF_DepthWater"))
                    {
                        if (!mr.name.ToLower().Contains("col")
                            && !mr.name.ToLower().Contains("cube"))
                        {
                            Debug.LogError("警告:" + mr.name + " 的材质不属于新框架");
                        }
                    }
                }
            }

            foreach (SkinnedMeshRenderer mr in FindObjectsOfType<SkinnedMeshRenderer>())
            {
                foreach (Material mat in mr.sharedMaterials)
                {
                    if (mat != null
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/NewChara/NewCharaMain")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Cutoff)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Terrain)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(Opaque)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia_V3/Obj/TJiaNewObj(AlphaBlend)")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia/BRDF_Billboard")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia/BRDF_BillboardFade")
                        && mat.shader != UnityUtils.FindShader("SAO_TJia/BRDF_DepthWater"))
                    {
                        if (!mr.name.ToLower().Contains("col")
                            && !mr.name.ToLower().Contains("cube"))
                        {
                            Debug.LogError("警告:" + mr.name + " 的材质不属于新框架");
                        }
                    }
                }
            }

        }
    }
#endif
}