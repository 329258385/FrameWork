Shader "SAO_TJia_V3/TjiaNewSky"
{
	Properties{
		
		//[HideInInspector]
		[KeywordEnum(Lerp, One, Two)]_Texnumtype("☭贴图选择",float) = 0
		[KeywordEnum(None, Rotate)]_Motion("☭运动状态",float) = 0
		_Tint("颜色", Color) = (.5, .5, .5, .5)
		[Gamma] _Exposure("曝光", Range(0, 8)) = 1.0
		_Rotation("旋转（针对Cubemap2）", Range(0, 360)) = 0
		_RotateSpeed("旋转速度（针对Cubemap2）", Range(-60,60)) = 0

		[NoScaleOffset] _Tex("Cubemap", Cube) = "grey" {}
		[NoScaleOffset] _Tex2("Cubemap2", Cube) = "black" {}
		_TexLerp("切换Cubemap", Range(0,1)) = 1

		//_FalloutModif("高度消散调整", float) = 0
		//_FalloutModifHeight("高度消散位置调整", float) = 0

		/*[KeywordEnum(Off, On)]_MColor("☭调色",half) = 0
		_Color1("ColorR", Color) = (1,0,0,1)
		_Color2("ColorG", Color) = (0,1,0,1)
		_Color3("ColorB", Color) = (0,0,1,1)
		_HSV("HSV", vector) = (0,0,0,1)*/
		
	}

		SubShader{
			Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
			Cull Off ZWrite Off

			Pass {

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#pragma multi_compile _TEXNUMTYPE_LERP _TEXNUMTYPE_ONE _TEXNUMTYPE_TWO
				#pragma multi_compile _MOTION_NONE _MOTION_ROTATE
				#pragma multi_compile __ SCENE_INIT
				//#pragma multi_compile _MCOLOR_OFF _MCOLOR_ON 

				#ifdef _TEXNUMTYPE_ONE
					samplerCUBE _Tex;
				#endif
				#ifdef _TEXNUMTYPE_TWO
					samplerCUBE _Tex2;
				#endif
				#ifdef _TEXNUMTYPE_LERP
					samplerCUBE _Tex, _Tex2;
				#endif
				half4 _Tex_HDR;
				half4 _Tex2_HDR;
				half4 _Tint;
				half _Exposure, _RotateSpeed;
				half _Rotation;
				fixed _CubeMapTransparency;
				fixed4 _SkyLowColor, _SkyHighColor, _CloudLightColor, _CloudShadowColor;
				fixed _Pos, _ShowSky, _DFogDensity;
				fixed _FogHeight, _TexLerp;
				half _FogHeightFallOut;
				half _FogDensity;
				half _FogHeightFallOutPos;
				half4 _FogColor;
				half4 _FogLightColor;
				half3 _Color1;
				half3 _Color2;
				half3 _Color3;
				half3 _HSV;
				half _FalloutModif;
				half _FalloutModifHeight;

				half3 RotateAroundYInDegrees(half3 vertex, half degrees)
				{
					half alpha = degrees * UNITY_PI / 180.0;
					half sina, cosa;
					sincos(alpha, sina, cosa);
					half2x2 m = half2x2(cosa, -sina, sina, cosa);
					return half3(mul(m, vertex.xz), vertex.y).xzy;
				}

				struct appdata_t {
					half4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f {
					half4 vertex : SV_POSITION;
					half3 texcoord : TEXCOORD0;
					half3 texcoordRot : TEXCOORD1;
					half3 lightDir : TEXCOORD2;
					half3 worldPos : TEXCOORD3;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				half3 HSVToRGB(half3 c)
				{
					half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
					half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
					return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
				}

				half3 RGBToHSV(half3 c)
				{
					half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
					half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
					half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));
					half d = q.x - min(q.w, q.y);
					half e = 1.0e-10;
					return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
				}

				half lum(half3 c)
				{
					return dot(c, half3(0.22, 0.707, 0.071));
				}

				v2f vert(appdata_t v)
				{
					v2f o;
					//UNITY_SETUP_INSTANCE_ID(v);
					//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					//half3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation + _RotateSpeed * _Time.x);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = v.vertex;
					#ifdef _MOTION_ROTATE
					o.texcoordRot = RotateAroundYInDegrees(v.vertex, _Rotation + _RotateSpeed * _Time.x);
					#else
					o.texcoordRot = v.vertex;
					#endif
					o.lightDir = ObjSpaceLightDir(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					//o.texcoordRot.y = 1-pow(1-o.texcoordRot.y, 1.1);
					return o;
				}

				float _SkyGrid;
				//float _WhiteModelRange;
				float _SkyColor;

				float correctiveRange(float r)
				{
					return (1 - pow(1 - saturate(r), 4));
				}

				fixed4 frag(v2f i) : SV_Target
				{
					#ifdef _TEXNUMTYPE_LERP
						half4 tex = texCUBE(_Tex, i.texcoordRot);
						half4 tex2 = texCUBE(_Tex2, i.texcoordRot);
						tex = lerp(tex, tex2, _TexLerp);
						half3 c = DecodeHDR(tex, lerp(_Tex_HDR, _Tex2_HDR, _TexLerp));
					#endif

					#ifdef _TEXNUMTYPE_ONE
						half4 tex = texCUBE(_Tex, i.texcoordRot);
						half3 c = DecodeHDR(tex, _Tex_HDR);
					#endif

					#ifdef _TEXNUMTYPE_TWO
						half4 tex = texCUBE(_Tex2, i.texcoordRot);
						half3 c = DecodeHDR(tex, _Tex2_HDR);
					#endif

					c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
					c *= _Exposure;

					/*#ifdef _MCOLOR_ON


						c = RGBToHSV(c);
						c += _HSV * 0.1;
						c = saturate(c);
						c = HSVToRGB(c);

						c = c.r * _Color1 + c.g * _Color2 + c.b * _Color3;
					#endif*/

					half3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
					half3 RayDir = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
					half vl = saturate(dot(RayDir, worldLightDir));
					half sunAmount = pow(vl, 4);

					_FogHeightFallOut += _FalloutModif;
					_FogHeightFallOutPos += _FalloutModifHeight;

					half3 oriColor = c;

					



					half fogDensity = exp(-_FogHeightFallOut * (i.texcoord.y * 2 - _FogHeightFallOutPos / 100 - 0.075)) * saturate(_FogDensity * 0.11);
					half3 fog = lerp(c, lerp(_FogColor, _FogLightColor.rgb, sunAmount * _FogLightColor.a), saturate(fogDensity) * _FogColor.a);
					half4 res = 1;

					#ifdef SCENE_INIT
					c.rgb = fixed3(0,0.0125,0.0125) * 2;

					float lines = step(0.9, max(frac(i.texcoordRot.x * 75), frac(i.texcoordRot.y * 75))) + step(length(i.texcoordRot.xz), correctiveRange(_SkyGrid)) * step(correctiveRange(_SkyGrid - 0.005), length(i.texcoordRot.xz));
					c.rgb = lerp(c.rgb, fixed3(0, 1.4, 1.4), saturate(lines) * step(length(i.texcoordRot.xz), correctiveRange(_SkyGrid)) * step(0, i.texcoordRot.y));

					//c.rgb = lerp(c.rgb, 0.6, step(length(i.texcoordRot.xz), correctiveRange(_WhiteModelRange/260)) * step(0, i.texcoordRot.y));
					_SkyColor += sin((i.texcoordRot.x) * 20 + i.texcoordRot.y + _Time.y) * 0.01;
					c.rgb = lerp(c.rgb, fixed3(1.4, 1.4, 0), step(correctiveRange(saturate(1 - _SkyColor - 0.005)), length(i.texcoordRot.xz)));
					fog = lerp(c.rgb, fog, step(correctiveRange(1 - _SkyColor), length(i.texcoordRot.xz)));
					#endif
					//c.rgb = lines;
					res.rgb = fog;

					return res;
				}
				ENDCG
			}
	}


		Fallback Off

}
