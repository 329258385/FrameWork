using UnityEngine;
using UnityEditor;
using System;

public class ScenePBRGUI : ShaderGUI
{
    public enum BlendMode
    {
        Opaque,
        Cutout
    }

    public enum CullMode
    {
        //
        // 摘要:
        //     Disable culling.
        Off = 0,
        //
        // 摘要:
        //     Cull front-facing geometry.
        Front = 1,
        //
        // 摘要:
        //     Cull back-facing geometry.
        Back = 2
    }

    private static class Styles
    {
        public static GUIContent albedoText = new GUIContent("Albedo(RGB)", "Albdeo Map for Diffuse");
        public static GUIContent cutoutText = new GUIContent("Cut Out", "Alpha Test Cutout");
        public static GUIContent normalText = new GUIContent("Normal Map", "Normal Map in Tangent Space");
        public static GUIContent bumpscaleText = new GUIContent("Bump Scale", "Bump Map Scale");
        public static GUIContent pbrText = new GUIContent("Smooth(R), Metallic(G), AO(B)", "PBR Texture");
        public static GUIContent occlusionText = new GUIContent("Occlusion", "Ambient Occlusion");
        public static GUIContent lightmapSpecText = new GUIContent("Lightmap Specular", "Calculate Specular For Lightmap");
        public static GUIContent emissionText = new GUIContent("Emission Map", "Self Emission Map");

        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
        public static readonly string[] cullNames = Enum.GetNames(typeof(CullMode));
    }

    MaterialProperty _BlendMode;
    MaterialProperty _MainTex;
	MaterialProperty _Color;
    MaterialProperty _Cutout;
    MaterialProperty _NormalTex;
    MaterialProperty _NormalScale;
    MaterialProperty _PBRTex;
    //MaterialProperty _Reflection;
    MaterialProperty _LightmapSpec;
    MaterialProperty _EmissionTex;
    MaterialProperty _EmissionColor;
    MaterialProperty _Cull;
    MaterialProperty _Occlusion;
    MaterialEditor _materialEditor;

    public void FindProperties(MaterialProperty[] props)
    {
        _BlendMode = FindProperty("_BlendMode", props);
        _MainTex = FindProperty("_MainTex", props);
		_Color = FindProperty("_Color", props, false);
        _Cutout = FindProperty("_Cutout", props);
        _NormalTex = FindProperty("_NormalTex", props);
        _NormalScale = FindProperty("_NormalScale", props);
        _PBRTex = FindProperty("_PBRTex", props);
        //_Reflection = FindProperty("_Reflection", props, false);
        _LightmapSpec = FindProperty("_LightmapSpec", props);
        _EmissionTex = FindProperty("_EmissionTex", props);
        _EmissionColor = FindProperty("_EmissionColor", props);
        _Occlusion = FindProperty("_Occlusion", props);
        _Cull = FindProperty("_Cull", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);
        _materialEditor = materialEditor;
        Material material = _materialEditor.target as Material;
        ShaderPropertiesGUI(material);
        if (EditorGUI.EndChangeCheck())
        {
            MaterialChanged(material);
        }
    }

    static void MaterialChanged(Material material)
    {
        if (material.GetTexture("_EmissionTex") != null)
            material.EnableKeyword("EMISSION_ON");
        else
            material.DisableKeyword("EMISSION_ON");

        BlendMode mode = (BlendMode)material.GetFloat("_BlendMode");
        if (mode == BlendMode.Cutout)
        {
            material.EnableKeyword("ALPHA_TEST_ON");
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest + 1;
        }
        else if (mode == BlendMode.Opaque)
        {
            material.DisableKeyword("ALPHA_TEST_ON");
            material.SetOverrideTag("RenderType", "Opaque");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest + 1;
        }

        bool useLightmapSpec = material.GetFloat("_LightmapSpec") == 1.0f;
        if (useLightmapSpec)
            material.EnableKeyword("LIGHTMAP_SPEC");
        else
            material.DisableKeyword("LIGHTMAP_SPEC");

    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUI.BeginChangeCheck();
        {
            BlendModePopup();
            EditorGUILayout.Space();
            DoAlbedo(material);
            EditorGUILayout.Space();
            DoNormal(material);
            EditorGUILayout.Space();
            DoPBR(material);
            EditorGUILayout.Space();
            DoEmission(material);
            EditorGUILayout.Space();
            DoCullMode(material);
            EditorGUILayout.Space();
            _materialEditor.RenderQueueField();
            _materialEditor.DoubleSidedGIField();
        }
    }

    void BlendModePopup()
    {
        EditorGUI.showMixedValue = _BlendMode.hasMixedValue;
        var mode = (BlendMode)_BlendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup("Rendering Mode", (int)mode, Styles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            _materialEditor.RegisterPropertyChangeUndo("Rendering Mode");
            _BlendMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }

    void DoAlbedo(Material material)
    {
		_materialEditor.TexturePropertySingleLine(Styles.albedoText, _MainTex, _Color);
        _materialEditor.TextureScaleOffsetProperty(_MainTex);
        if (material.IsKeywordEnabled("ALPHA_TEST_ON"))
            _materialEditor.ShaderProperty(_Cutout, Styles.cutoutText);
    }

    void DoNormal(Material material)
    {
        _materialEditor.TexturePropertySingleLine(Styles.normalText, _NormalTex, _NormalScale);
    }

    void DoPBR(Material material)
    {
        _materialEditor.TexturePropertySingleLine(Styles.pbrText, _PBRTex);
        _materialEditor.ShaderProperty(_Occlusion, Styles.occlusionText);
        _materialEditor.ShaderProperty(_LightmapSpec, Styles.lightmapSpecText);
    }

    void DoEmission(Material material)
    {
        _materialEditor.TexturePropertySingleLine(Styles.emissionText, _EmissionTex, _EmissionColor);
        _materialEditor.LightmapEmissionFlagsProperty(1, true);
    }

    void DoCullMode(Material material)
    {
        EditorGUI.showMixedValue = _Cull.hasMixedValue;
        var mode = (CullMode)_Cull.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (CullMode)EditorGUILayout.Popup("Cull Mode", (int)mode, Styles.cullNames);
        if (EditorGUI.EndChangeCheck())
        {
            _materialEditor.RegisterPropertyChangeUndo("Cull Mode");
            _Cull.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }
}
