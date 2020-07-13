Shader "SAO_TJia_V3/Obj/TJiaNewObj(Opaque)" //BreathLight //Covered //RockMask //Terrain
//AlphaCutOff  Tree InteractiveTree
//AlphaBlend
{
	Properties
	{
		[KeywordEnum(Normal, Flag)]_Type("静动态",float) = 0
		[Header(Main)]
		[HDR]_Color("♘颜色",Color) = (1,1,1,1)
		_MainTex("♘主贴图", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpTex("♘法线贴图", 2D) = "bump" {}
		_BumpScale("♘法线强度", Range(0,10)) = 1

		_Metallic("♘金属度", Range(0, 1)) = 0
		_Smoothness("♘光滑度", Range(0,1)) = 0.2
		_SpecForec("♘高光强度", Range(0,2)) = 1

		[Space(10)][Header(Emission)]
		[KeywordEnum(Off, On, Breath, MSE)]_Emission("☭自发光类型(默认关闭)",float) = 0
		[NoScaleOffset]_EmissionTex("☭自发光贴图", 2D) = "black" {}
		[HDR] _Color1("☭自发光颜色", color) = (1,1,1,1)
		_BreathSpeed("☭呼吸速度", Range(0,2)) = 1
		[HDR] _Color2("☭呼吸变化色", color) = (1,1,1,1)
		[Enum(On,1,Off,0)]_GEmissionOn("☭全局自发光与Lightmap", int) = 1

		[Space(10)][Header(Animation)]
		
		_XYZForce("☥旗子幅度XYZ", vector) = (1,1,1,1)
		_WindForce("☥总体幅度", Range(0, 20)) = 1
		_WindSpeed("☥摇动频率", Range(0, 10)) = 1
		_WindLengh("☥摇动变化强度", Range(0, 4)) = 1

		[Space(10)]
		[Enum(Off,1,On,0)]_CameraHide("░▒相机渐隐", int) = 0
		_FadeOutTransparency("░▒渐隐透明度", Range(0,1)) = 1

		[Space(10)]
		[Enum(Off,0,Ice,1)]_IceOn("冰", int) = 0

		[Space(10)]
		_EnableDecal("是否屏蔽Decal(圆片阴影)",Range(0,1))= 1

		[Space(10)]
		[Toggle]_DontUseSceneLight("不使用场景灯光,用自定义灯光",Float)=0
		[HDR]_CustomLightColor("自定义灯光颜色",COLOR)=(1,1,1,1)
		[HDR]_CustomCubeMapColor("自定义Skybox CubeMap颜色",COLOR)=(0,0,0,0)
		[HDR]_CustomEnvironmentSkyColor("自定义环境光中的Sky颜色",COLOR)=(1,1,1,1)
		[HDR]_CustomEnvironmentEquatorColor("自定义环境光中的Equator颜色",COLOR) = (1,1,1,1)
		[HDR]_CustomEnvironmentGroundColor("自定义环境光中的Ground颜色",COLOR) = (1,1,1,1)
		_CustomSunOutDoor("自定义_SunOutDoor值",Float) = 1
	}

		CGINCLUDE
#pragma target 3.0
#include "UnityCG.cginc"
#include "AutoLight.cginc"

#pragma skip_variants POINT_COOKIE DIRECTIONAL_COOKIE LIGHTMAP_SHADOW_MIXING  //SHADOWS_DEPTH SHADOWS_CUBE VERTEXLIGHT_ON LIGHTPROBE_SH DYNAMICLIGHTMAP_ON DIRLIGHTMAP_COMBINED SPOT POINT


		half4 _FogColor;
		half _FogDensity;
		half _FogHeightFallOut;
		half _FogHeightFallOutPos;
		half _FogStartDistance;
		half4 _FogLightColor;
		half _VerticalFogDensity;
		half _VerticalFogHeight;
		fixed _NoLightmap;

		half _WindSpeed;
		half _WindLengh;
		half3 _XYZForce;
		half _WindForce;
		half _GWindForce;
		half _GWindSpeed;

		half windNoise(float time)
		{
			float res = cos(time) * cos(3 * time) * cos(5 * time) * cos(7 * time) + sin(25 * time) * 0.1;
			res = saturate((res + 0.2) / 1.2);//(-0.2 -- 1)
			return res;
		}

		half windNoise2(float time)
		{
			float res = pow(cos(time), 2) * cos(5 * time) * cos(3 * time) * 0.5 + sin(25 * time) * 0.02;
			res = saturate((res + 0.08) / 0.58);//(-0.08 -- 0.5)
			return res;
		}

		half windNoise3(float time)
		{
			float res = cos(time) * cos(3 * time) + sin(7 * time) * 0.1;
			res = saturate((res + 0.6) / 1.6);//--(-0.6 -- 1)
			return res;
		}

		half windNoise4(float time)
		{
			float res = cos(time) + sin(3 * time) * 0.1;
			res = saturate((res +1)/2);
			return res;
		}

		float4 vertAnime(float4 v, float time, float force)
		{
			float4 wv = mul(unity_ObjectToWorld, v);
			float n0 = windNoise3(time * 5 + /*wv.x * */0.01) * 2 + 1;
			force *= n0;
			time *= _WindSpeed * 5 * _GWindSpeed;
			//time += (_CosTime.x + 1);
				
			float n1 = windNoise4(wv.y + time * 15 + wv.z * _WindLengh * 0.5) - 0.5;
			float n2 = windNoise4(wv.y + time * 17 + /*wv.x **/ _WindLengh * 0.3) - 0.5;
			v.xyz += float3(n2,n1*n2,n1) * force * _XYZForce * _WindForce * _GWindForce;
			//v.xyz += sin(_Time.y)*float3(1, 1, 1);
			return v;
		}

		float4 vertAnime2(float4 v, float time, float force)
		{
			float4 wv = mul(unity_ObjectToWorld, v);
			float n0 = windNoise3(time * 5 +/* wv.x **/ 0.01) * 2 + 1;
			force *= n0;
			time *= _WindSpeed * 5 * _GWindSpeed;
			//time += (_CosTime.x + 1);

			float n1 = windNoise4(wv.y + time * 15 + wv.z * _WindLengh * 0.5) - 0.5;
			float n2 = windNoise4(wv.y + time * 17 + /*wv.x **/ _WindLengh * 0.3) - 0.5;
			v.xyz += float3(n2, n1*n2, n1) * force * _XYZForce * _WindForce * _GWindForce;
			//v.xyz += sin(_Time.y)*float3(1, 1, 1);
			return v;
		}

		float avoid0(float num)
		{
			return saturate(max(num, 0.000001));
		}

		float DistributionGGX(float nh, float roughness)
		{
			float r4 = pow(max(roughness, 0.000001), 4);
			float nh2 = nh * nh;
			float num = r4;
			float denum = pow(nh2 * (r4 - 1) + 1, 2) * UNITY_PI;
			float res = num / denum;
			res = step(1, res) * pow(res, 0.25) + step(res, 1) * res;
			return res;
		}

		inline half3 SafeHDR(half3 c)
		{
			return min(c, 65504);
		}

		half mixShadowMask(half atten, float2 uv, float3 worldPos)
		{
			half condX = step(0.5, uv.x);
			half condY = step(0.5, uv.y);
			uv.x = uv.x * 2 - condX;
			uv.y = uv.y * 2 - condY;
			half4 shadowMask4 = UNITY_SAMPLE_TEX2D(unity_ShadowMask, uv);
			half shadowMask =   shadowMask4.r * (1 - condX) * (    condY) + 
								shadowMask4.g * (    condX) * (    condY) + 
								shadowMask4.b * (1 - condX) * (1 - condY) + 
								shadowMask4.a * (    condX) * (1 - condY);
			float zDist = dot(_WorldSpaceCameraPos - worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(worldPos, zDist);
			atten = lerp(min(atten, shadowMask), shadowMask, UnityComputeShadowFade(fadeDist) * (0.8 + 0.2 * _NoLightmap) + 0.2 * (1 - _NoLightmap));
			return atten;
		}

		half mixShadowMask(half atten, float3 worldPos, float lms)
		{
			half shadowMask = lms;
			float zDist = dot(_WorldSpaceCameraPos - worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(worldPos, zDist);
			atten = lerp(min(atten, shadowMask), shadowMask, UnityComputeShadowFade(fadeDist) * (0.8 + 0.2 * _NoLightmap) + 0.2 * (1 - _NoLightmap));
			return atten;
		}

		half3 ApplyFog(half3 c, float3 lm, float3 viewVec, float vl)
		{
			float3 viewVecVertical = viewVec;
			viewVecVertical.y += _VerticalFogHeight;
			float verticalCond = step(0, viewVecVertical.y);
			viewVecVertical.y += viewVecVertical.y * verticalCond * _VerticalFogDensity;
			float viewDis = length(viewVecVertical);
			float dis = max(0, viewDis - _FogStartDistance) / 2000;
			float height = -(viewVec.y + _FogHeightFallOutPos) / 100;
			float fogDensity = saturate(1 - exp(-pow(_FogDensity * dis, 2)));
			float fogHeightFallOut = saturate(exp(-_FogHeightFallOut * height));
			//float vfogDensity = saturate(1 - exp(-pow(_FogDensity * 10 * height, 2)));
			float sunAmount = pow(vl, 4);
			float3 res = lerp(c, lerp(_FogColor, _FogLightColor.rgb, sunAmount * _FogLightColor.a) + lm * 0.005, fogDensity * fogHeightFallOut * _FogColor.a);
			//res = pow(vl, 4.4);
			return res;
		}

		half3 ApplyFog(half3 c, float3 viewVec, float vl)
		{
			float3 viewVecVertical = viewVec;
			viewVecVertical.y += _VerticalFogHeight;
			float verticalCond = step(0, viewVecVertical.y);
			viewVecVertical.y += viewVecVertical.y * verticalCond * _VerticalFogDensity;
			float viewDis = length(viewVecVertical);
			float dis = max(0, viewDis - _FogStartDistance) / 2000;
			float height = -(viewVec.y + _FogHeightFallOutPos) / 100;
			float fogDensity = saturate(1 - exp(-pow(_FogDensity * dis, 2)));
			float fogHeightFallOut = saturate(exp(-_FogHeightFallOut * height));
			//float vfogDensity = saturate(1 - exp(-pow(_FogDensity * 10 * height, 2)));
			float sunAmount = pow(vl, 4);
			float3 res = lerp(c, lerp(_FogColor, _FogLightColor.rgb, sunAmount * _FogLightColor.a), fogDensity * fogHeightFallOut * _FogColor.a);
			//res = pow(vl, 4.4);
			return res;
		}		
		
		/*half3 ApplyVerticalFog(half3 c, float3 viewVec)
		{
			float viewDis = viewVec.y;
			float dis = max(0, viewDis - _FogStartDistance) / 2000;
			float height = -(viewVec.y + _FogHeightFallOutPos) / 100;
			float fogDensity = saturate(1 - exp(-pow(_FogDensity * dis, 2)));
			float fogHeightFallOut = saturate(exp(-_FogHeightFallOut * height));

			float3 res = lerp(c, lerp(_FogColor, _FogLightColor.rgb, _FogLightColor.a), fogDensity * fogHeightFallOut * _FogColor.a);
			return res;
		}*/

		half Max3(half3 c)
		{
			return max(c.r, max(c.g, c.b));
		}

		half lum(half3 c)
		{
			return dot(c, half3(0.22, 0.707, 0.071));
		}

		half posNoise(float3 pos)
		{
			pos = pos * 0.01;
			float res = dot(float3(cos(pos.x), cos(pos.y), cos(pos.z)), pos);
			//res *= 0.01;
			return res;
		}

		float4x4 _DecalProjectionMatrix;
		sampler2D _DecalTexture;
		//sampler2D _DecalTexture2;
		float _EnableDecal;

		half3 decalTexCol(half3 col, float3 worldPos, float worldNormalY)
		{
			float4 decalPos = mul(_DecalProjectionMatrix, float4(worldPos, 1));
			float3 decalProj = (decalPos.xyz / decalPos.w);
			decalProj = decalProj * 0.5 + 0.5;

			//modify by Zzc --- decal rt optimize
			/*fixed4 decalColor;

			if (decalProj.y > 0.25)
			{
				float2 uv = float2(decalProj.x, (decalProj.y - 0.25)*1.3333333333);
				decalColor = tex2D(_DecalTexture, uv);
			}
			else
			{
				float2 uv = float2(decalProj.x, decalProj.y * 4);
				decalColor = tex2D(_DecalTexture2, uv);
			}*/

			fixed4 decalColor = tex2D(_DecalTexture, decalProj.xy);
			decalColor *= float4(1, 1, 1, 1)*smoothstep(1, 0.7, abs((decalProj.x - 0.5)*2.0))*smoothstep(1, 0.7, abs((decalProj.y - 0.5)*2.0));

			decalColor = decalColor * step(abs(decalProj.x - 0.5), 0.499) * step(abs(decalProj.y - 0.5), 0.499)*_EnableDecal;// *step(decalColor, 0.0000001);				
			fixed normalCoef = pow(saturate(worldNormalY),2);
			half4 res = decalColor * normalCoef;
			half decalAlpha = Max3(res.rgb);
			res.rgb = lerp(col.rgb, res.rgb * (decalAlpha * 2 + 1), saturate(res.a * (decalAlpha * 4 + 1))) + res.rgb;
			return res.rgb;
		}

half _SpecForec;

#ifdef TJIA_POINT_LIGHT

		float4 _PointLightPos1[4];
		float4 _LightProperties1[4];
		fixed _PointLightNumber;

		float4 _VPointLightPos1[16];
		float4 _VLightProperties1[16];
		fixed _VPointLightNumber;

		fixed3 AddPointLight(float3 worldPos, float3 worldNormal, float3 viewDir, float2 sr, int i)
		{
			fixed3 res = 0;
			_VPointLightPos1[i].w += 0.001;
			float3 ray = _VPointLightPos1[i].xyz - worldPos;
			float3 lightDir = normalize(ray);
			float3 halfVector = normalize(lightDir + viewDir);
			float nh = avoid0(dot(halfVector, worldNormal));
			float dis = length(ray);
			float range = (1 - dis / _VPointLightPos1[i].w);
			float lambert = saturate(dot(lightDir, worldNormal));
			res = range * range * lambert * lambert;
			fixed cond = step(dis, _VPointLightPos1[i].w);
			res.rgb *= cond * _VLightProperties1[i].w * _VLightProperties1[i].rgb;
			return res;
		}

		fixed3 AddPointLightWithSpec(float3 worldPos, float3 worldNormal, float3 viewDir, float2 sr, int i)
		{
			fixed3 res = 0;
			_PointLightPos1[i].w += 0.001;
			float3 ray = _PointLightPos1[i].xyz - worldPos;
			float3 lightDir = normalize(ray);
			float3 halfVector = normalize(lightDir + viewDir);
			float nh = avoid0(dot(halfVector, worldNormal));
			float dis = length(ray);
			fixed cond = step(dis, _PointLightPos1[i].w);
			float range = (1 - dis / _PointLightPos1[i].w);
			float lambert = saturate(dot(lightDir, worldNormal));
			float spec = DistributionGGX(nh, sr.y) * (sr.x * sr.x);
			res = range * range * (lambert * lambert + spec * _SpecForec);
			res.rgb *= cond * _LightProperties1[i].w * _LightProperties1[i].rgb;
			return res;
		}

#endif

		ENDCG


		SubShader
		{
			Tags{"RenderType" = "Opaque"}
			ZTest LEqual
			ZWrite On
			Pass
			{
			Tags{"RenderType" = "Opaque" "LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing 
			//#pragma multi_compile_fwdbase 

			//origin
			//--------------------------------------------------------------------------			
			//#pragma multi_compile _ LIGHTMAP_ON
			//#pragma multi_compile _ SHADOWS_SCREEN
			//#pragma multi_compile _ SHADOWS_SHADOWMASK

			//#pragma multi_compile _EMISSION_OFF _EMISSION_ON _EMISSION_BREATH _EMISSION_MSE
			//#pragma multi_compile _TYPE_NORMAL _TYPE_FLAG
			//#pragma multi_compile __ TJIA_POINT_LIGHT
			//#pragma multi_compile _ LOD_FADE_CROSSFADE
			//#pragma multi_compile __ SCENE_INIT
			//----------------------------------------------------------------------------

			//SVC Optimize
			//--------------------------------------------------------------------------			
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _EMISSION_OFF _EMISSION_ON _EMISSION_BREATH _EMISSION_MSE
			#pragma multi_compile _TYPE_NORMAL _TYPE_FLAG
			#pragma multi_compile __ TJIA_POINT_LIGHT
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile __ SCENE_INIT

			//#pragma instancing_options lodfade

			//#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				#ifdef _TYPE_FLAG
					half4 color : COLOR;
				#endif
			};

			struct v2f
			{
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;				
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 tangent : TEXCOORD4;
				float3 bitangent : TEXCOORD5;
				#ifdef TJIA_POINT_LIGHT
					float3 pointColor : TEXCOORD7;
				#endif
				SHADOW_COORDS(6)
			};

			struct v2fLod
			{
				UNITY_VERTEX_INPUT_INSTANCE_ID
				#ifdef LOD_FADE_CROSSFADE
					UNITY_VPOS_TYPE vpos : VPOS;
				#else
					float4 pos : SV_POSITION;
				#endif
				float4 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 tangent : TEXCOORD4;
				float3 bitangent : TEXCOORD5;
				#ifdef TJIA_POINT_LIGHT
					float3 pointColor : TEXCOORD7;
				#endif
				SHADOW_COORDS(6)
			};

			sampler2D _MainTex;
			sampler2D _BumpTex;
#ifndef _EMISSION_OFF
			sampler2D _EmissionTex;
			float3 _Color1;
			float3 _Color2;			
#endif
			float _GlobalEmissionLevel;
			float _GEmissionOn;
#ifndef LIGHTMAP_ON
			sampler2D _LmColor;
			//sampler2D _LmShadowTex;
#endif
			float4 _MainTex_ST;
			float _Smoothness;
			float4 _Color;
			float4 _LightmapColor;
			float _BumpScale;
			float _Metallic;
			//half _SpecForec;
			half _LightmapForce;
			half _BreathSpeed;
			float3 _LmCamPos;
			half _SunOutDoor;
			int _IceOn;

			int _CrossfadeOn;
			float _DontUseSceneLight;
			float4 _CustomLightColor;
			float4 _CustomCubeMapColor;
			float4 _CustomEnvironmentSkyColor;
			float4 _CustomEnvironmentEquatorColor;
			float4 _CustomEnvironmentGroundColor;
			float _CustomSunOutDoor;

			v2f vert(appdata v)
			{
				v2f o;

				//UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o); 
	
#ifdef _TYPE_FLAG
				float force = v.color.r;
				v.vertex = vertAnime(v.vertex, _Time.x, force);
#endif

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
				o.bitangent = normalize(cross(o.normal, o.tangent) * v.tangent.w * unity_WorldTransformParams.w);
#ifdef TJIA_POINT_LIGHT
				o.pointColor = 0;
				float smoothness = _Smoothness * 0.95;
				float roughness = 1 - smoothness;
				for (int j = 0; j < _VPointLightNumber; j++)
				{
					o.pointColor += AddPointLight(o.worldPos, o.normal, normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz), float2(smoothness, roughness), j);
				}
#endif
				o.screenPos = ComputeScreenPos(o.pos);
				TRANSFER_SHADOW(o);
				return o;
			}

			float _GridRange;
			float _WhiteModelRange;
			float _Colorfy;
			//sampler2D _GridTex;

			static float4x4 thresholdMatrix = 
			{
				1.0/17.0, 9.0/17.0, 3.0/17.0, 11.0/17.0,
				13.0/17.0, 5.0/17.0, 15.0/17.0, 7.0/17.0, 
				4.0/17.0, 12.0/17.0, 2.0/17.0, 10.0/17.0, 
				16.0/17.0, 8.0/17.0, 14.0/17.0, 6.0/17.0
			};

			static float4x4 _RowAccess = 
			{
				1, 0, 0, 0, 
				0, 1, 0, 0, 
				0, 0, 1, 0, 
				0, 0, 0, 1
			};

			float _CameraHide;
			float2 _FlagFadeOut;
			float2 _OpaqueFadeOut;
			float _FadeOutTransparency;
			float _LmScale;
			float _CloudyLevel;

			float3 _SSR;

			fixed4 frag(v2fLod i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				#ifdef LOD_FADE_CROSSFADE
						i.vpos *= _CrossfadeOn;
						UnityApplyDitherCrossFade(i.vpos);					
				#endif

				float3 viewVec = _WorldSpaceCameraPos.xyz - i.worldPos.xyz;

				float2 screenPos = i.screenPos.xy / i.screenPos.w * _ScreenParams.xy;

				#ifdef _TYPE_FLAG
					float clipThreshold = smoothstep(_FlagFadeOut.x, _FlagFadeOut.y, length(viewVec));
				#endif
				#ifdef _TYPE_NORMAL
					float clipThreshold = smoothstep(_OpaqueFadeOut.x, _OpaqueFadeOut.y, length(viewVec));
				#endif

				clip(_CameraHide + clipThreshold * _FadeOutTransparency - thresholdMatrix[fmod(screenPos.x, 4)] * _RowAccess[fmod(screenPos.y, 4)]);

				i.normal = normalize(i.normal);
				float3 oriNormal = i.normal;
				float3x3 tangentTransform = float3x3(i.tangent, i.bitangent, i.normal);

				float3 normalLocal = UnpackNormal(tex2D(_BumpTex, i.uv.xy));
				normalLocal.xy *= _BumpScale;
				i.normal = mul(normalize(normalLocal), tangentTransform);

				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				
				float3 viewDir = normalize(viewVec);

				float attenuation = SHADOW_ATTENUATION(i);//SHADOW_ATTENUATION(i);//UNITY_SHADOW_ATTENUATION(i, i.worldPos);  

				
	//阴影SSAA by Zzc-------------------------------------------------------------------------------

//#ifdef SHADOWS_SCREEN
//				float shadowOffset = 0.0003;
//				float4 oriShadowCoord = i._ShadowCoord;
//				i._ShadowCoord = oriShadowCoord + float4(-shadowOffset, -shadowOffset, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				i._ShadowCoord = oriShadowCoord + float4(-shadowOffset, 0, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				i._ShadowCoord = oriShadowCoord + float4(-shadowOffset, shadowOffset, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				i._ShadowCoord = oriShadowCoord + float4(0, -shadowOffset, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				i._ShadowCoord = oriShadowCoord + float4(0, shadowOffset, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				i._ShadowCoord = oriShadowCoord + float4(shadowOffset, -shadowOffset, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				i._ShadowCoord = oriShadowCoord + float4(shadowOffset, 0, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				i._ShadowCoord = oriShadowCoord + float4(shadowOffset, shadowOffset, 0, 0);
//				attenuation += SHADOW_ATTENUATION(i);
//				attenuation /= 9;
//#endif

				//ui拍摄时不受场景光影响
				float sunOutDoor = _SunOutDoor * (1 - _DontUseSceneLight) + _CustomSunOutDoor * _DontUseSceneLight;
				float3 lightColor0 = _LightColor0.rgb*(1 - _DontUseSceneLight) + _CustomLightColor.rgb * _DontUseSceneLight;
				float4 ambientEquator = unity_AmbientEquator * (1 - _DontUseSceneLight) + _CustomEnvironmentEquatorColor * _DontUseSceneLight;
				float4 ambientSky = unity_AmbientSky * (1 - _DontUseSceneLight) + _CustomEnvironmentSkyColor * _DontUseSceneLight;
				float4 ambientGround = unity_AmbientGround * (1 - _DontUseSceneLight) + _CustomEnvironmentGroundColor * _DontUseSceneLight;

				//return float4(lightColor0, 1);

				#ifdef LOD_FADE_CROSSFADE
					attenuation = 1;
				#endif
#ifdef LIGHTMAP_ON
				float coef = sign(dot(oriNormal, float3(1, 0.01, 1)));
				float normalAO = dot(i.normal, float3(coef, oriNormal.y + 1 - abs(oriNormal.y), coef)) * 0.58;
				normalAO = saturate(normalAO);
				attenuation = mixShadowMask(attenuation, i.uv.zw, i.worldPos) * lerp(normalAO * 0.75 + 0.25, 1, sunOutDoor);
#else
				float2 lmUv = (i.worldPos.xz - _LmCamPos.xz) / _LmScale + 0.5;
				//fixed4 lmc = tex2D(_LmColor, lmUv) * 4;
				half4 lmca = tex2D(_LmColor, lmUv); half3 lmc = lmca.rgb * (1 - _NoLightmap);
				half lms =lmca.a * (1 - _NoLightmap) + _NoLightmap;
				attenuation = mixShadowMask(attenuation, i.worldPos, lms);
				//fixed lms = lum(lmsTex.rgb);
#endif				
				attenuation = lerp(attenuation, 1, _CloudyLevel * 0.5);

				//ui相机物体无attenuation
				attenuation = attenuation * (1 - _DontUseSceneLight) + _DontUseSceneLight;

				float3 lightColor = lightColor0 * attenuation * sunOutDoor;
				float3 halfVector = normalize(lightDir + viewDir);

				float nl = dot(i.normal, lightDir);
				float sssCoef = avoid0(1 - nl);
				//sssCoef *= sssCoef;
				nl = avoid0(nl);
				float nv = avoid0(dot(i.normal, viewDir));
				float vl = avoid0(dot(lightDir, -viewDir));
				//float vh = avoid0(dot(viewDir, halfVector));
				//float lh = avoid0(dot(lightDir, halfVector));
				float nh = avoid0(dot(i.normal, halfVector));
				//float nv = avoid0(dot(i.normal, viewDir));

				fixed4 Albedo = tex2D(_MainTex, i.uv.xy) * _Color;

				float plantLevel = pow(saturate(Albedo.g - max(Albedo.r, Albedo.b)),0.625);
				Albedo.rgb *= lerp(_SSR.z, 1, plantLevel);
				_Smoothness = saturate(_SSR.y + _Smoothness);
				_SpecForec *= lerp(_SSR.x, 0, plantLevel);

#ifndef _EMISSION_OFF
				float3 emission = tex2D(_EmissionTex, i.uv.xy);
				float3 ecol = _Color1;
#ifdef _EMISSION_MSE
				_Metallic *= emission.r;
				_Smoothness *=  emission.g;
				emission = emission.b;
#endif
#endif

#ifndef _EMISSION_OFF
				_GlobalEmissionLevel = _GlobalEmissionLevel * _GEmissionOn + (1 - _GEmissionOn);
#ifndef _EMISSION_ON
				ecol = lerp(ecol, _Color2, windNoise(_Time.y * _BreathSpeed + posNoise(i.worldPos)));
#endif
				//emission = emission * ecol * lerp(max(0.3, _GlobalEmissionLevel), _GlobalEmissionLevel, saturate(1 - _GlobalEmissionLevel) * attenuation);
				emission = emission * ecol * _GlobalEmissionLevel;
#endif

				float smoothness = _Smoothness * 0.95;
				float roughness = 1 - smoothness;

				float3 SpecularResult = DistributionGGX(nh, roughness) * (smoothness * smoothness + 0.04);

				float3 diffColor = lightColor * nl * 1.6;// / PI;

#ifdef TJIA_POINT_LIGHT
				for (int j = 0; j < _PointLightNumber; j++)
				{
					diffColor += AddPointLightWithSpec(i.worldPos, i.normal, viewDir, float2(smoothness, roughness), j) * saturate(_GlobalEmissionLevel);
				}

				diffColor += i.pointColor * saturate(_GlobalEmissionLevel);
#endif

				float3 specColor = SpecularResult * nl * lightColor * UNITY_PI;

				float3 iblDiffuseResult = lerp(ambientEquator, ambientSky, saturate(i.normal.y) );
				iblDiffuseResult = lerp(iblDiffuseResult, ambientGround, saturate(-i.normal.y) ) * 1;

#ifdef LIGHTMAP_ON
				float3 lightMapDiff = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw));
#else
				float3 lightMapDiff = lmc;
#endif

#ifndef _EMISSION_OFF
				lightMapDiff *= 1 - saturate(lum(emission));
#endif
				
				_LightmapForce = _LightmapForce * _GEmissionOn + (1 - _GEmissionOn);
				lightMapDiff *= saturate(attenuation + dot(lightMapDiff, float3(0.22,0.707,0.071)) * 2 * (sunOutDoor * 0.8 + 0.2));
				//lightMapDiff *= _LightmapColor * lerp(max(1, _LightmapForce), _LightmapForce, saturate(1 - _LightmapForce) * attenuation) * 2;
				lightMapDiff *= _LightmapColor * _LightmapForce * 2;
				iblDiffuseResult = iblDiffuseResult * saturate(1 - lightMapDiff) + lightMapDiff;
					//iblDiffuseResult = 1 - (1 - iblDiffuseResult)*(1 - lightMapDiff);
					//* (1-nl);

				float mip_roughness = roughness * (1.7 - 0.7 * roughness);
				float3 reflectVec = reflect(-viewDir, i.normal);
				half mip = mip_roughness * UNITY_SPECCUBE_LOD_STEPS;
				half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectVec, mip * 1.5);

				//ui拍摄时不受场景光影响
				rgbm = rgbm * (1 - _DontUseSceneLight) + _CustomCubeMapColor * _DontUseSceneLight;

				float fres = 1 - nv;
				float3 iblSpecCoef = pow(fres, 4 * (smoothness + 0.00001)) * smoothness * smoothness + lerp(_Metallic * (0.5 + fres), _Metallic, smoothness);

				float3 iblSpecularResult = DecodeHDR(rgbm, unity_SpecCube0_HDR) * iblSpecCoef;
				//float3 IndirectResult = (iblDiffuseResult + iblSpecularResult);

				Albedo.rgb = lerp(lum(Albedo.rgb), Albedo.rgb, attenuation * 0.3 + 0.7);

				float3 DiffResult = (diffColor + iblDiffuseResult)  * pow(1 - _Metallic, 3)  * Albedo.rgb;
				float3 SpecResult = SafeHDR(specColor + iblSpecularResult) * lerp(1, Albedo.rgb, _Metallic) * _SpecForec;

				float4 result;
				result.rgb = DiffResult + SpecResult;

#ifndef _EMISSION_OFF
				result.rgb += emission * lerp(1, (fres + 0.6) * sssCoef * vl * lum(lightColor0) + 0.2, _IceOn);
				result.rgb = lerp(result.rgb, saturate(result.rgb), _IceOn * 0.75);
#endif

				result.rgb = decalTexCol(result.rgb, i.worldPos, i.normal.y);



//#ifdef OBJECT_SPACE_FX
#ifdef LIGHTMAP_ON
				result.rgb = ApplyFog(result.rgb, lightMapDiff, viewVec, vl);
				//result.rgb = normalAO * 0.8 + 0.3;
#else
				result.rgb = ApplyFog(result.rgb, viewVec, vl);
#endif

			/*float viewDis = length(viewVecVertical);
			float dis = max(0, viewDis - _FogStartDistance) / 2000;
			float height = -(viewVec.y + _FogHeightFallOutPos) / 100;
			float fogDensity = saturate(1 - exp(-pow(_FogDensity * dis, 2)));
			float fogHeightFallOut = saturate(exp(-_FogHeightFallOut * height));
			//float vfogDensity = saturate(1 - exp(-pow(_FogDensity * 10 * height, 2)));
			float sunAmount = pow(vl, 4);
			float3 res = lerp(c, lerp(_FogColor, _FogLightColor.rgb, sunAmount * _FogLightColor.a), fogDensity * fogHeightFallOut * _FogColor.a);
			//res = pow(vl, 4.4);
			return res;*/

				//result.rgb = ApplyVerticalFog(result.rgb, viewVec);
//#else
//#endif
				
				result.a = 1;

				//result.rgb = abs(dot(i.normal, 0.58) + 0.1);
				//result.rgb = abs(dot(i.normal, normalize(float3(1,1,1))));

				//
				#ifdef SCENE_INIT
				float vecDis = length(viewVec.xz);
				_WhiteModelRange -= vecDis;
				_GridRange -= vecDis;
				_Colorfy -= vecDis;

				float wave = sin(i.worldPos.x + i.worldPos.z + i.worldPos.y) * 0.4;
				float wmRange = _WhiteModelRange + wave;
				float gRange = _GridRange + wave;
				float cRange = _Colorfy + wave;

				oriNormal = abs(oriNormal);
				float3 gridPos = abs(frac(i.worldPos * 2) - 0.5) * 2;
				
				float zGrid = max(gridPos.x, gridPos.y) * oriNormal.z;
				float xGrid = max(gridPos.y, gridPos.z) * oriNormal.x;
				float yGrid = max(gridPos.z, gridPos.x) * oriNormal.y;
				float lines = max(zGrid, max(xGrid, yGrid));

				lines = saturate(step(vecDis, gRange) * step(gRange, vecDis + 0.2 + vecDis * 0.01) + lines);

				half3 oriColor = result.rgb;

				//修改颜色参数by王健宁
				result.rgb = lerp(result.rgb, fixed3(0.1, 0.1, 8), lines);

				float gridCoef = step(vecDis, wmRange);
				result.rgb = lerp(lerp(fixed3(0, 0.1, 8), fixed3(0, 0.45, 8), pow(lines * 2 - 1, 4)), (((dot(oriNormal, lightDir) * 0.5 + 0.5) * (attenuation * 0.6 + 0.4)) * 0.9 + 0.1) * fixed3(0.6, 0.6, 0.6), gridCoef);
				float gridCoef2 = step(0.2 + vecDis * 0.01, abs(vecDis - wmRange));
				result.rgb = lerp(fixed3(0, 0.3, 7), result.rgb, gridCoef2);

				clip((gridCoef + lines) * step(vecDis, gRange) - 0.8);
				result.rgb = saturate(result.rgb) * 1.9;

				gridCoef2 = step(0.2 + vecDis * 0.01, abs(vecDis - cRange));

				result.rgb = lerp(result.rgb, oriColor, step(vecDis, cRange));
				result.rgb = lerp(fixed3(2.5, 1.5, 0.5), result.rgb, gridCoef2);
				//修改颜色参数by王健宁
				//result.rgb = normalize(_WorldSpaceLightPos0.xyz);
				#endif
				//

				
				return result;
			}
			ENDCG
		}
		Pass // NOPASS-BEGIN
		{
			Tags{"RenderType" = "Opaque" "LightMode" = "ForwardAdd"}
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd
			#pragma multi_compile_instancing 
			#pragma multi_compile _TYPE_NORMAL _TYPE_FLAG
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile _EMISSION_MSE

				//#include "UnityCG.cginc"
				#include "UnityPBSLighting.cginc"
				#include "AutoLight.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 uv : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
					#ifdef _TYPE_FLAG
						half4 color : COLOR;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 pos : SV_POSITION;
					float3 normal : TEXCOORD1;
					float3 worldPos : TEXCOORD2;
					float3 tangent : TEXCOORD4;
					float3 bitangent : TEXCOORD5;
					LIGHTING_COORDS(6,7)
				};

				sampler2D _MainTex;
				sampler2D _BumpTex;
#ifdef _EMISSION_MSE
				sampler2D _EmissionTex;
#endif
				float4 _MainTex_ST;
				float _Smoothness;
				float4 _Color;
				float _BumpScale;
				float _Metallic;
				//half _SpecForec;

				v2f vert(appdata v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
#ifdef _TYPE_FLAG
				float force = v.color.r;
				v.vertex = vertAnime(v.vertex, _Time.x, force);
#endif
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.normal = UnityObjectToWorldNormal(v.normal);
					o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
					o.bitangent = normalize(cross(o.normal, o.tangent) * v.tangent.w * unity_WorldTransformParams.w);
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

#ifdef _EMISSION_MSE
					float2 emission = tex2D(_EmissionTex, i.uv).rg;
					//_Metallic *= emission.r;
					//_Smoothness *=  emission.g;
#endif

					i.normal = normalize(i.normal);
					float3x3 tangentTransform = float3x3(i.tangent, i.bitangent, i.normal);

					float3 normalLocal = UnpackNormal(tex2D(_BumpTex, i.uv));
					normalLocal.xy *= _BumpScale;
					i.normal = mul(normalize(normalLocal), tangentTransform);

					float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
					float3 viewVec = _WorldSpaceCameraPos.xyz - i.worldPos.xyz;
					float3 viewDir = normalize(viewVec);

					float attenuation = LIGHT_ATTENUATION(i);

					//UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);

					float3 lightColor = _LightColor0.rgb * attenuation;
					float3 halfVector = normalize(lightDir + viewDir);


					float nl = avoid0(dot(i.normal, lightDir));
					//float nv = avoid0(dot(i.normal, viewDir));
					//float vl = avoid0(dot(lightDir, -viewDir));
					//float vh = avoid0(dot(viewDir, halfVector));
					//float lh = avoid0(dot(lightDir, halfVector));
					float nh = avoid0(dot(i.normal, halfVector));
					//float nv = avoid0(dot(i.normal, viewDir));

					fixed4 Albedo = tex2D(_MainTex, i.uv) * _Color;

					float smoothness = _Smoothness * 0.95;
					float roughness = 1 - smoothness;

					float3 SpecularResult = DistributionGGX(nh, roughness) * (smoothness * smoothness + 0.04);

					float3 diffColor = lightColor * nl * 1.6;// / PI;
					float3 specColor = SpecularResult * nl * lightColor * UNITY_PI * _SpecForec;

					float3 DiffResult = (diffColor) * pow(1 - _Metallic, 3)  * Albedo.rgb;
					float3 SpecResult = SafeHDR(specColor) * lerp(1, Albedo.rgb, _Metallic);

					float4 result;
					result.rgb = DiffResult + SpecResult;

					result.a = 1;

					//result.rgb = nl;

					return result;
				}
				ENDCG
			} // NOPASS-END
				Pass
				{
					Tags { "LightMode" = "ShadowCaster" }

					CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#pragma multi_compile_instancing 
					#pragma multi_compile_shadowcaster
					#pragma multi_compile _TYPE_NORMAL _TYPE_FLAG

					#include "UnityCG.cginc"
					struct appdata
					{
						float4 vertex : POSITION;
						float3 normal : NORMAL;
						#ifdef _TYPE_FLAG
							half4 color : COLOR;
						#endif
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						V2F_SHADOW_CASTER;
					};

					v2f vert(appdata v) {
						v2f o;

						UNITY_SETUP_INSTANCE_ID(v);

#ifdef _TYPE_FLAG
						float force = v.color.r;
						v.vertex = vertAnime(v.vertex, _Time.x, force);
						//v.vertex.xyz += sin(_Time.y)*float3(1, 1, 1);
#endif


						

						TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

						return o;
					}

					fixed4 frag(v2f i) : SV_Target {
						SHADOW_CASTER_FRAGMENT(i)
					}
					ENDCG
				}
		}
			//Fallback "VertexLit"
}
