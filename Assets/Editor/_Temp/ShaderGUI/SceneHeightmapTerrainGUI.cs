using UnityEngine;
using UnityEditor;
using System;

public class SceneHeightmapTerrainGUI : ShaderGUI
{
    public static OnChange onChange;

    private static class Styles
    {
        public static GUIContent controllText = new GUIContent("Controll Map", "Controll Map Using RGBA");
        public static GUIContent Albedo1Text = new GUIContent("Albedo1 Map", "Albedo1 Map");
        public static GUIContent Albedo2Text = new GUIContent("Albedo2 Map", "Albedo1 Map");
        public static GUIContent Albedo3Text = new GUIContent("Albedo3 Map", "Albedo1 Map");

        public static GUIContent heightText = new GUIContent("Height Map", "Height Map");
    }

    MaterialProperty _ControllTex;
    MaterialProperty _HeightWeight;
    MaterialProperty _AlbedoTex0;
    MaterialProperty _AlbedoTex1;
    MaterialProperty _AlbedoTex2;
    MaterialProperty _HeightTex;
	MaterialProperty _WaterHeight;
	MaterialProperty _WaterHighLight;
	MaterialProperty _WaterColor;
    MaterialProperty _Water;
    MaterialProperty _NormalAtten;
    MaterialProperty _NormalScale0;
    MaterialProperty _NormalScale1;
    MaterialProperty _NormalScale2;

    MaterialEditor _materialEditor;

    public void FindProperties(MaterialProperty[] props)
    {
        _ControllTex = FindProperty("_ControllTex", props);
        _HeightWeight = FindProperty("_HeightWeight", props);
        _AlbedoTex0 = FindProperty("_AlbedoTex0", props);
        _AlbedoTex1 = FindProperty("_AlbedoTex1", props);
        _AlbedoTex2 = FindProperty("_AlbedoTex2", props);
        //        _AlbedoTex3 = FindProperty("_AlbedoTex3", props);
        _HeightTex = FindProperty("_HeightTex", props);
        _Water = FindProperty("_WATER", props);
        _NormalAtten = FindProperty("_NormalAtten", props);
        _WaterHeight = FindProperty("_WaterHeight", props);
		_WaterHighLight = FindProperty("_WaterHighLight", props);
		_WaterColor = FindProperty("_WaterColor", props);
        _NormalScale0 = FindProperty("_NormalScale0", props);
        _NormalScale1 = FindProperty("_NormalScale1", props);
        _NormalScale2 = FindProperty("_NormalScale2", props);
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
            if (onChange != null)
                onChange();
        }
    }

    static void MaterialChanged(Material material)
    {
        if (material.GetTexture("_ControllTex") != null)
        {
            if ((material.GetTexture("_ControllTex") as Texture2D).alphaIsTransparency)
            {
                Debug.LogError("controll map alpha used");
            }
            material.EnableKeyword("MAP_CONTROLL");
        }
        else
        {
            material.DisableKeyword("MAP_CONTROLL");
        }
        if (material.GetTexture("_AlbedoTex0") != null)
            material.EnableKeyword("MAP_ONE");
        else
            material.DisableKeyword("MAP_ONE");

        if (material.GetTexture("_AlbedoTex1") != null)
            material.EnableKeyword("MAP_TWO");
        else
            material.DisableKeyword("MAP_TWO");

        if (material.GetTexture("_AlbedoTex2") != null)
            material.EnableKeyword("MAP_THREE");
        else
            material.DisableKeyword("MAP_THREE");
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUI.BeginChangeCheck();
        {
            DoControll(material);
            EditorGUILayout.Space();
            DoMaps(material);
        }
    }

    void DoControll(Material material)
    {
        _materialEditor.TexturePropertySingleLine(Styles.controllText, _ControllTex);
        _materialEditor.TextureScaleOffsetProperty(_ControllTex);
        _materialEditor.ShaderProperty(_HeightWeight, "高度图权重");

        if (material.GetTexture("_ControllTex") != null)
        {
            _materialEditor.ShaderProperty(_Water, "启用积水");
            if (material.GetFloat("_WATER") != 0)
            {
                EditorGUILayout.Space();
                _materialEditor.ShaderProperty(_WaterHeight, "积水高度");
                _materialEditor.ShaderProperty(_WaterHighLight, "积水高光");
                _materialEditor.ShaderProperty(_WaterColor, "积水颜色");
                _materialEditor.ShaderProperty(_NormalAtten, "法线削弱");
            }
        }
    }

    void DoMaps(Material material)
    {
        _materialEditor.TexturePropertySingleLine(Styles.heightText, _HeightTex);
        EditorGUILayout.Space();

        _materialEditor.TexturePropertySingleLine(Styles.Albedo1Text, _AlbedoTex0, _NormalScale0);
        _materialEditor.TextureScaleOffsetProperty(_AlbedoTex0);

        EditorGUILayout.Space();

        _materialEditor.TexturePropertySingleLine(Styles.Albedo2Text, _AlbedoTex1, _NormalScale1);
        //_materialEditor.TextureScaleOffsetProperty(_AlbedoTex1);

        EditorGUILayout.Space();

        _materialEditor.TexturePropertySingleLine(Styles.Albedo3Text, _AlbedoTex2, _NormalScale2);
        //_materialEditor.TextureScaleOffsetProperty(_AlbedoTex2);

        EditorGUILayout.Space();
    }
}
