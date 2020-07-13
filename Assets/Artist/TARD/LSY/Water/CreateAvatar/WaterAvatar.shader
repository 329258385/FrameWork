Shader "SAO_TLsy/Water/WaterAvatar"
{
	Properties {

		_Fresnel("Fresnel",Range(0,1))=0.5
		_FresnelPow("FresnelPow",Range(0,32)) = 1
		//Lighting Model Params
		_Shine ("_Shine", Range(0,100)) = 8
		_LightWrap ("Light Wrap", Range(0,1)) = 0.0

		//Water Color
		_ShallowColor ("Shallow Color", Color) = (1,1,1,1)
		_DeepColor ("Deep Color", Color) = (1,1,1,1)
		[NoScaleOffset]
		_ShoreRippleTex("Shore Ripple Tex ", 2D) = "white" {}
		[NoScaleOffset]
		_ReflectionTex ("Reflection Tex", 2D) = "white" {}
		_BelowInvisible("Below Invisible", Range(0,1)) = 0.2

		//Waves
		_FlowSpeed("Flow Speed", Vector) = (1,1,0,0)
		_WaveScale("Wave Scale", Range(0,100)) = 1.0

        [Normal]
        _WaveTex1("Wave Tex Large", 2D) = "bump" {}
        [Normal]
        [NoScaleOffset]
        _WaveTex2("Wave Tex Med", 2D) = "bump" {}
        [Normal]
        [NoScaleOffset]
        _WaveTex3("Wave Tex Small", 2D) = "bump" {}
        _MediumTilingDistance ("Medium Tiling Distance", Float ) = 500
        _LongTilingDistance ("Long Tiling Distance", Float ) = 1500
        _DistanceTilingFade ("Distance Tiling Fade", Float ) = 1
	}
	SubShader {
		Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite On
        //GrabPass{"_GrabTexture1"}

		CGPROGRAM
		#pragma surface surf lsyLightModel vertex:vert fullforwardshadows alpha:blend
		#pragma target 3.0
	    #define iRefract 1
	    #include "UnityPBSLighting.cginc"


		struct Input {
			float2 uv_WaveTex1;
			float3 worldPos ;
			float3 worldNormal;INTERNAL_DATA
			float4 grabPos;
			float4 screenPo;
		};

		sampler2D _CameraDepthTexture;
		//sampler2D _GrabTexture1;
		sampler2D _ShoreRippleTex;
		sampler2D _ReflectionTex;
		float4x4 refMA;

		half _Shine;
		half _LightWrap;
		fixed4 _ShallowColor;
		fixed4 _DeepColor;
		float _BelowInvisible;

		float _WaveScale;
		sampler2D _WaveTex1;
		sampler2D _WaveTex2;
		sampler2D _WaveTex3;
		float4 _FlowSpeed;
	    uniform float _MediumTilingDistance;
        uniform float _DistanceTilingFade;
        uniform float _LongTilingDistance;
       
		float _Fresnel;
		float _FresnelPow;

        float4 LightinglsyLightModel(SurfaceOutput s, float3 lightDir,half3 viewDir, half atten)
		{
			float4 c;
			float diffuseF = max(0,dot(s.Normal,lightDir));
			diffuseF = _LightWrap + diffuseF * (1-_LightWrap);
			float specF;
			float3 H = normalize(lightDir+viewDir);
			float specBase = max(0,dot(s.Normal,H));
			specF = pow(specBase,_Shine);
			c.rgb = s.Albedo * _LightColor0 * diffuseF *atten + _LightColor0*specF; 
			c.a = 1;
			return c;
		}


		inline float3 GetWaveNormal(float dis1,float dis2,float2 pannerBase,float dis,sampler2D waveTex,float spd)
		{
			float2 pannerAdd = float2(_Time.x*spd,0);
			float2 panner = pannerBase+ pannerAdd;
			float3 wave1 = UnpackNormal(tex2D(waveTex,panner));
			//Biger Wave
			float3 wave2 = UnpackNormal(tex2D(waveTex,panner/20));
			float3 wave3 = UnpackNormal(tex2D(waveTex,panner/60));
			float3 waveResult = lerp(lerp(wave1,wave2,dis1),wave3,dis2);
			return waveResult;
		}

   		void vert (inout appdata_full v, out Input o) {
   			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.worldPos = mul(unity_ObjectToWorld,v.vertex);
			o.worldNormal = float3(0,0,1);
			float4 ppos =  UnityObjectToClipPos(v.vertex);
			o.grabPos = ComputeGrabScreenPos(ppos);
			o.screenPo =  ComputeScreenPos(ppos);
			COMPUTE_EYEDEPTH(o.screenPo.z);
        }

		void surf (Input IN, inout SurfaceOutput o) {
			float3 wViewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
			float3 wNormal = WorldNormalVector (IN, o.Normal);

			float3 worldNormal = float3(0, 1, 0);
			float fresnel = _Fresnel + (1 - _Fresnel)*pow(1 - dot(wViewDir, worldNormal), _FresnelPow);
		
			/////////////////////////////
			//////////// Water //////////
			/////////////////////////////

			//////////// 1 Water Depth //////////
			float z = IN.screenPo.z;
			float z_scene = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPo))); 
			//This depth diff is approximate water depth
			//float waterDepth = z_scene-z;
			float waterDepth = saturate((z_scene-z)/2);
			float waterDepthInvert = saturate(1 - (z_scene-z)/2);
			float waterDepth_Color = saturate((z_scene-z)/30);


			//////////// 2 Waves //////////
			float dis = length(IN.worldPos-_WorldSpaceCameraPos);
			float dis1 = saturate(pow((dis/_MediumTilingDistance),_DistanceTilingFade));
		    float dis2 = saturate(pow((dis/_LongTilingDistance),_DistanceTilingFade));

			float3 wave1 = GetWaveNormal(dis1,dis2,IN.uv_WaveTex1*100,dis,_WaveTex1,_FlowSpeed.x);
			float3 wave2 = GetWaveNormal(dis1,dis2,IN.uv_WaveTex1*100,dis,_WaveTex2,_FlowSpeed.y);
			float3 wave3 = GetWaveNormal(dis1,dis2,IN.uv_WaveTex1*100,dis,_WaveTex3,_FlowSpeed.z);
			float3 finalWave = normalize(wave1+wave2+wave3);
			o.Normal = lerp(float3(0,0,1),finalWave,_WaveScale);

			//////////// 3 Reflection //////////
			float4 projPos = mul(refMA,float4(IN.worldPos,1));
	     	projPos.xyz = projPos.xyz/projPos.w;
	     	float2 refuv = projPos*0.5+0.5;
	     	float3 colorReflection = tex2D (_ReflectionTex, refuv).rgb;
	     	float3 waterSurface = colorReflection;

//	     	//////////// 4 Refraction //////////
//	     	float dNV = dot(wNormal,wViewDir);
//			float iBlend = saturate(1 - (dNV-_BelowInvisible)/(1-_BelowInvisible));
//	     	#if iRefract
//	     		float3 grab = tex2Dproj(_GrabTexture,UNITY_PROJ_COORD(IN.grabPos+float4(finalWave*0.05,0))).rgb;
//	     		float3 waterColor = grab;
//	     		o.Alpha  = 1;
//	     	#else
//	     		float3 waterColor = float3(1,1,1);
//	     		o.Alpha  = iBlend;
//	     	#endif


//	     	//////////// 5 Blending //////////
//	     	o.Albedo = lerp(waterColor,waterSurface,iBlend);
//	     	o.Albedo *= lerp(_ShallowColor,_DeepColor,waterDepth_Color);
//
//	     	//////////// 6 Shore ripple//////////
//			fixed shoreWave = 1 - tex2D (_ShoreRippleTex, IN.uv_WaveTex1*1000 + float2(_Time.x*_FlowSpeed.w,0)).b;
//			fixed shoreWave2 = 1 - tex2D (_ShoreRippleTex, IN.uv_WaveTex1*500 + float2(-_Time.x*_FlowSpeed.w,0)).b;
//			shoreWave =  saturate(shoreWave+shoreWave2);
//			shoreWave *= waterDepthInvert;
//			float4 shoreColor = float4(1,1,1,1)*3;
//			o.Albedo = lerp(o.Albedo ,shoreColor,shoreWave);
//			o.Alpha  *= waterDepth;

			o.Albedo = colorReflection*_ShallowColor*fresnel*1.5;
	     	o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}


