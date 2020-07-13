using UnityEngine;
using UnityEditor;
using System;

public delegate void OnChange();

public class SceneTerrainGUI : ShaderGUI
{
    public static OnChange onChange;

    private static class Styles
    {
        public static GUIContent controllText = new GUIContent("Controll Map", "Controll Map Using RGBA");
        public static GUIContent Albedo1Text = new GUIContent("Albedo1 Map", "Albedo1 Map");
        public static GUIContent normal1Text = new GUIContent("Normal1 Map", "Normal1 Map");
        public static GUIContent Albedo2Text = new GUIContent("Albedo2 Map", "Albedo1 Map");
        public static GUIContent normal2Text = new GUIContent("Normal2 Map", "Normal1 Map");
        public static GUIContent Albedo3Text = new GUIContent("Albedo3 Map", "Albedo1 Map");
        public static GUIContent normal3Text = new GUIContent("Normal3 Map", "Normal1 Map");
        public static GUIContent Albedo4Text = new GUIContent("Albedo4 Map", "Albedo1 Map");
        public static GUIContent normal4Text = new GUIContent("Normal4 Map", "Normal1 Map");

        //public static GUIContent occlusionText = new GUIContent("Occlusion", "Ambient Occlusion");
    }

    MaterialProperty _Control;
    MaterialProperty _HeightWeight;
    MaterialProperty _MainTex0;
    MaterialProperty _NormalTex0;
    MaterialProperty _MainTex1;
    MaterialProperty _NormalTex1;
    MaterialProperty _MainTex2;
    MaterialProperty _NormalTex2;
    MaterialProperty _MainTex3;
    MaterialProperty _NormalTex3;
    //MaterialProperty _Occlusion;
    //MaterialProperty _WaterHeight;
    //MaterialProperty _WaterHighLight;
    //MaterialProperty _WaterColor;
    //   MaterialProperty _Water;
    //MaterialProperty _NormalAtten;
    MaterialProperty _NormalScale0;
    MaterialProperty _NormalScale1;
    MaterialProperty _NormalScale2;
    MaterialProperty _NormalScale3;
    MaterialEditor _materialEditor;

    public void FindProperties(MaterialProperty[] props)
    {
        _Control = FindProperty("_Control", props);
        _HeightWeight = FindProperty("_HeightWeight", props);
        _MainTex0 = FindProperty("_MainTex0", props);
        _NormalTex0 = FindProperty("_NormalTex0", props);
        _MainTex1 = FindProperty("_MainTex1", props);
        _NormalTex1 = FindProperty("_NormalTex1", props);
        _MainTex2 = FindProperty("_MainTex2", props);
        _NormalTex2 = FindProperty("_NormalTex2", props);
        _MainTex3 = FindProperty("_MainTex3", props);
        _NormalTex3 = FindProperty("_NormalTex3", props);
        //        _MainTex3 = FindProperty("_MainTex3", props);
        //_Reflect = FindProperty("_Reflect", props);
        //_Water = FindProperty("_WATER", props);
        //_NormalAtten = FindProperty("_NormalAtten", props);
       // _WaterHeight = FindProperty("_WaterHeight", props);
		//_WaterHighLight = FindProperty("_WaterHighLight", props);
		//_WaterColor = FindProperty("_WaterColor", props);
        _NormalScale0 = FindProperty("_NormalScale0", props);
        _NormalScale1 = FindProperty("_NormalScale1", props);
        _NormalScale2 = FindProperty("_NormalScale2", props);
        _NormalScale2 = FindProperty("_NormalScale3", props);
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
        if (material.GetTexture("_Control") != null)
        {
            if ((material.GetTexture("_Control") as Texture2D).alphaIsTransparency)
            {
                Debug.LogError("controll map alpha used");
            }
            material.EnableKeyword("MAP_CONTROLL");
        }
        else
        {
            material.DisableKeyword("MAP_CONTROLL");
        }
        if (material.GetTexture("_MainTex0") != null)
            material.EnableKeyword("MAP_ONE");
        else
            material.DisableKeyword("MAP_ONE");

        if (material.GetTexture("_MainTex1") != null)
            material.EnableKeyword("MAP_TWO");
        else
            material.DisableKeyword("MAP_TWO");

        if (material.GetTexture("_MainTex2") != null)
            material.EnableKeyword("MAP_THREE");
        else
            material.DisableKeyword("MAP_THREE");

        if (material.GetTexture("_MainTex3") != null)
            material.EnableKeyword("MAP_FOUR");
        else
            material.DisableKeyword("MAP_FOUR");
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
        _materialEditor.TexturePropertySingleLine(Styles.controllText, _Control);
        _materialEditor.TextureScaleOffsetProperty(_Control);
        _materialEditor.ShaderProperty(_HeightWeight, "高度图权重");

        if (material.GetTexture("_Control") != null)
        {
            //_materialEditor.ShaderProperty(_Water, "启用积水");
            //if (material.GetFloat("_WATER") != 0)
            //{
            //    EditorGUILayout.Space();
            //    _materialEditor.ShaderProperty(_WaterHeight, "积水高度");
            //    _materialEditor.ShaderProperty(_WaterHighLight, "积水高光");
            //    _materialEditor.ShaderProperty(_WaterColor, "积水颜色");
            //    _materialEditor.ShaderProperty(_NormalAtten, "法线削弱");
            //}
        }
    }

    void DoMaps(Material material)
    {
        _materialEditor.TexturePropertySingleLine(Styles.Albedo1Text, _MainTex0);
        _materialEditor.TextureScaleOffsetProperty(_MainTex0);
        if (material.GetTexture("_MainTex0") != null)
            _materialEditor.TexturePropertySingleLine(Styles.normal1Text, _NormalTex0, _NormalScale0);

        EditorGUILayout.Space();

        _materialEditor.TexturePropertySingleLine(Styles.Albedo2Text, _MainTex1);
        _materialEditor.TextureScaleOffsetProperty(_MainTex1);
        if (material.GetTexture("_MainTex1") != null)
            _materialEditor.TexturePropertySingleLine(Styles.normal2Text, _NormalTex1, _NormalScale1);

        EditorGUILayout.Space();

        _materialEditor.TexturePropertySingleLine(Styles.Albedo3Text, _MainTex2);
        _materialEditor.TextureScaleOffsetProperty(_MainTex2);
        if (material.GetTexture("_MainTex2") != null)
            _materialEditor.TexturePropertySingleLine(Styles.normal3Text, _NormalTex2, _NormalScale2);

        EditorGUILayout.Space();

        _materialEditor.TexturePropertySingleLine(Styles.Albedo4Text, _MainTex3);
        _materialEditor.TextureScaleOffsetProperty(_MainTex3);
        if (material.GetTexture("_MainTex3") != null)
            _materialEditor.TexturePropertySingleLine(Styles.normal4Text, _NormalTex3);

        EditorGUILayout.Space();
        //_materialEditor.ShaderProperty(_Occlusion, Styles.occlusionText);
    }
}
