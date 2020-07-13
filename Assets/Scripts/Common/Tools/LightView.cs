using UnityEngine;

    [ExecuteInEditMode]
    public class LightView : MonoBehaviour
    {
        public Cubemap globalReflectionCube;
        public float globalReflectionRotation = 0;
        public float globalReflectionIntensity = 1;
        public Color ambientColorAdd = Color.white;
        public Color characterAmbientColor = Color.white;
        public Color characterSkyColor = Color.white;
        public Color characterEquatorColor = Color.white;
        public Color characterGroundColor = Color.white;
        public float characterAmbientScale = 1;
        public float aoIntensity = 0;
        public float shadowIntensity = 0.5f;
        public float globalLightmapIntensity = 3;
        public Color heightFogColor;
        public float heightFogMax;
        public float heightFogMin;
        public float characterHeight = 0;
        public int shaderLOD = 600;
        private float darkworldScale = 1;
        public float fogFactor = 30;
        public float fogFactor2 = 0.2f;
        public float reflectionScale = 1;
        public float scenePBRReflectionScale = 1f;
        public float specularScale = 1;
        public float shadowAmbientScale = 1;
        public bool enableHeightFog = true;
        public bool enableEmissiveFog = true;
        public float[] lodLevelOne;
        public float[] lodLevelTwo;
        public float[] lodLevelThree;
        private Vector4 heightFogParam;
        public bool disableLightmapSpecular = false;
        public bool basePassDiffuse = false;

        public float[] CurrentLodLevel { get; set; }

        public bool update = false;

        public void ApplyCharacterColor()
        {
            Shader.SetGlobalColor("_GlobalCharacterSkyColor", characterSkyColor);
            Shader.SetGlobalColor("_GlobalCharacterEquatorColor", characterEquatorColor);
            Shader.SetGlobalColor("_GlobalCharacterGroundColor", characterGroundColor);
        }

        protected void Apply()
        {
            Shader.SetGlobalTexture("_GlobalReflectionCube", globalReflectionCube);
            Shader.SetGlobalFloat("_GlobalReflectionRotation", globalReflectionRotation);
            Shader.SetGlobalFloat("_GlobalReflectionIntensity", globalReflectionIntensity);
            Shader.SetGlobalColor("_GlobalAmbientScale", ambientColorAdd);
            Shader.SetGlobalColor("_GlobalCharacterAmbientColor", characterAmbientColor);
            Shader.SetGlobalColor("_GlobalCharacterSkyColor", characterSkyColor);
            Shader.SetGlobalColor("_GlobalCharacterEquatorColor", characterEquatorColor);
            Shader.SetGlobalColor("_GlobalCharacterGroundColor", characterGroundColor);
            Shader.SetGlobalFloat("_GlobalCharacterAmbientScale", characterAmbientScale);
            ProcessHeightFogParam(heightFogMax, heightFogMin);
            //Shader.globalMaximumLOD = shaderLOD;
            Shader.SetGlobalFloat("_DarkWorldScale", darkworldScale);
            Shader.SetGlobalFloat("_FogFactor", fogFactor);
            Shader.SetGlobalFloat("_FogFactor2", fogFactor2);
            Shader.SetGlobalFloat("_GlobalReflectionScale", reflectionScale);
            Shader.SetGlobalFloat("_GlobalScenePBRReflectionScale", scenePBRReflectionScale);
            Shader.SetGlobalFloat("_GlobalSpecularScale", specularScale);
            Shader.SetGlobalFloat("_GlobalAOIntensity", aoIntensity);
            Shader.SetGlobalFloat("_GlobalShadowIntensity", shadowIntensity);
            Shader.SetGlobalFloat("_GlobalLightmapIntensity", globalLightmapIntensity);
            if (enableHeightFog)
                Shader.EnableKeyword("HEIGHT_FOG");
            else
                Shader.DisableKeyword("HEIGHT_FOG");
            if (enableEmissiveFog)
                Shader.EnableKeyword("EMISSIVE_FOG");
            else
                Shader.DisableKeyword("EMISSIVE_FOG");
            if (disableLightmapSpecular)
                Shader.EnableKeyword("DISABLE_LIGHTMAP_SPECULAR");
            else
                Shader.DisableKeyword("DISABLE_LIGHTMAP_SPECULAR");
            if (basePassDiffuse)
                Shader.EnableKeyword("BASE_PASS_DIFFUSE");
            else
                Shader.DisableKeyword("BASE_PASS_DIFFUSE");

            ProcessShadowAmbientScale();
        }
        protected void Awake()
        {
            Apply();
        }

        public void Update()
        {
            if (update)
            {
                update = false;
                //Shader.globalMaximumLOD = shaderLOD;
                Shader.SetGlobalFloat("_GlobalCharacterHeight", characterHeight);
                Apply();
            }
        }

        public void OnValidate()
        {
            update = true;
        }

        void ProcessHeightFogParam(float max, float min)
        {

            Vector4 v = Vector4.zero;
            v.x = min;
            v.y = max;
            if (v.y - v.x < 0.01f)
            {
                v.x = v.y - 0.01f;
            }
            v.z = 1f / (v.y - v.x);
            v.w = 1f - v.y * v.z;
            heightFogParam = v;
            heightFogMax = heightFogParam.y;
            heightFogMin = heightFogParam.x;
            Shader.SetGlobalVector("_GlobalHeightFogParams", heightFogParam);
            Shader.SetGlobalColor("_GlobalHeightFogColor", heightFogColor);
        }

        void ProcessShadowAmbientScale()
        {
            if (shadowAmbientScale < 1)
                shadowAmbientScale = 1;
            Shader.SetGlobalFloat("_GlobalShadowAmbientScale", shadowAmbientScale);
            Shader.SetGlobalColor("_GlobalAmbientColor", ambientColorAdd / shadowAmbientScale);
        }

        void InitLOD(float[] lodLevel)
        {
            if (lodLevel == null || lodLevel.Length == 0) return;
            LODGroup[] groups = gameObject.GetComponentsInChildren<LODGroup>();
            foreach (LODGroup g in groups)
            {
                LOD[] lods = g.GetLODs();
                for (int i = 0; i < lods.Length; i++)
                {
                    if (i < lodLevel.Length)
                    {
                        lods[i].screenRelativeTransitionHeight = lodLevel[i];
                        lods[i].fadeTransitionWidth = lodLevel[i];
                    }
                }
                g.SetLODs(lods);
            }
        }
    }
