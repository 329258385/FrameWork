// Upgrade NOTE: replaced 'defined CHROMATIC_ABERRATION' with 'defined (CHROMATIC_ABERRATION)'

Shader "Hidden/CameraFXShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_LUTTex("_LUTTex", 3D) = "" {}
		_LUTTex2("_LUTTex2", 3D) = "" {}
		_LerpLUT("LerpLut", Range(0,1)) = 0
	}
		SubShader
		{
			Cull Off ZWrite Off ZTest Always
			Fog { Mode Off } Blend Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile _ USE_LUT
				//#pragma multi_compile __ OBJECT_SPACE_FX
				#pragma multi_compile __ AA 
				#pragma multi_compile __ RADIUS_BLUR
				#pragma multi_compile __ AUTO_EXPOSURE
				#pragma multi_compile __ CHROMATIC_ABERRATION
				#pragma multi_compile __ VIGNETTE
				#pragma multi_compile __ ROTATE_BLUR
				#pragma glsl

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;

				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					#ifdef AA
						float4 interpolatorA : TEXCOORD1;
						float4 interpolatorB : TEXCOORD2;
						float4 interpolatorC : TEXCOORD3;
					#endif
				};

				sampler2D _MainTex;
#ifdef AUTO_EXPOSURE
				sampler2D _AutoExplosureTex;
#endif
/*#ifdef RADIUS_BLUR
				sampler2D _BlurRT;
#endif*/
				float3 _CameraForward;
				fixed _RainForce;
				sampler3D _LUTTex, _LUTTex2;
				fixed _LerpLUT;
				half _ScreenForce, _OverlayForce, _FinalSaturation;
				uniform float4 _MainTex_TexelSize;
				uniform float4 _rcpFrame;
				uniform float4 _rcpFrameOpt;
				half4 _MainTex_ST;
				uniform half _EdgeThresholdMin;
				uniform half _EdgeThreshold;
				uniform half _EdgeSharpness;

				half lum(half3 c)
				{
					return dot(c, half3(0.22, 0.707, 0.071));
				}

				fixed3 PS_Blending_Screen(fixed3 a, fixed3 b)
				{
					a = pow(a, 0.4545);
					b = a;
					fixed3 res = 1 - (1 - a)*(1 - b);
					res = pow(res, 2.2);
					return res;
				}

				fixed3 linearSpaceTex3D(sampler3D tex_3D, fixed3 col)
				{
					return pow(tex3D(tex_3D, pow(col.rgb, 0.4545)), 2.2);
				}

#if defined(SHADER_API_GLES) && defined(SHADER_API_DESKTOP)
#define FxaaTexTop(t, p) tex2D(t, UnityStereoScreenSpaceUVAdjust(p, _MainTex_ST)) 
#else
#define FxaaTexTop(t, p) tex2Dlod(t, float4(UnityStereoScreenSpaceUVAdjust(p, _MainTex_ST), 0.0, 0.0))
#endif 

        inline half TexLuminance( float2 uv )
        {
            return Luminance(FxaaTexTop(_MainTex, uv).rgb);
        }

        half3 FxaaPixelShader(float2 pos, float4 extents, float4 rcpSize, float4 rcpSize2)
        {
            half lumaNw = TexLuminance(extents.xy);
            half lumaSw = TexLuminance(extents.xw);
            half lumaNe = TexLuminance(extents.zy);
            half lumaSe = TexLuminance(extents.zw);
            
            half3 centre = FxaaTexTop(_MainTex, pos).rgb;
            half lumaCentre = Luminance(centre);
            
            half lumaMaxNwSw = max( lumaNw , lumaSw );
            lumaNe += 1.0/384.0;
            half lumaMinNwSw = min( lumaNw , lumaSw );
            
            half lumaMaxNeSe = max( lumaNe , lumaSe );
            half lumaMinNeSe = min( lumaNe , lumaSe );
            
            half lumaMax = max( lumaMaxNeSe, lumaMaxNwSw );
            half lumaMin = min( lumaMinNeSe, lumaMinNwSw );
            
            half lumaMaxScaled = lumaMax * _EdgeThreshold;
            
            half lumaMinCentre = min( lumaMin , lumaCentre );
            half lumaMaxScaledClamped = max( _EdgeThresholdMin , lumaMaxScaled );
            half lumaMaxCentre = max( lumaMax , lumaCentre );
            half dirSWMinusNE = lumaSw - lumaNe;
            half lumaMaxCMinusMinC = lumaMaxCentre - lumaMinCentre;
            half dirSEMinusNW = lumaSe - lumaNw;
            
            if(lumaMaxCMinusMinC < lumaMaxScaledClamped)
                return centre;
            
            half2 dir;
            dir.x = dirSWMinusNE + dirSEMinusNW;
            dir.y = dirSWMinusNE - dirSEMinusNW;
            
            dir = normalize(dir);           
            half3 col1 = FxaaTexTop(_MainTex, pos.xy - dir * rcpSize.zw).rgb;
            half3 col2 = FxaaTexTop(_MainTex, pos.xy + dir * rcpSize.zw).rgb;
            
            half dirAbsMinTimesC = min( abs( dir.x ) , abs( dir.y ) ) * _EdgeSharpness;
            dir = clamp(dir.xy/dirAbsMinTimesC, -2.0, 2.0);
            
            half3 col3 = FxaaTexTop(_MainTex, pos.xy - dir * rcpSize2.zw).rgb;
            half3 col4 = FxaaTexTop(_MainTex, pos.xy + dir * rcpSize2.zw).rgb;
            
            half3 rgbyA = col1 + col2;
            half3 rgbyB = ((col3 + col4) * 0.25) + (rgbyA * 0.25);
            
            if((Luminance(rgbyA) < lumaMin) || (Luminance(rgbyB) > lumaMax))
                return rgbyA * 0.5;
            else
                return rgbyB;
        }

				fixed3 PS_Blending_Overlay(fixed3 a, fixed3 b)
				{
					a = pow(a, 0.4545);
					b = a;
					fixed3 res = step(b, 0.5) * 2 * a * b + step(0.5, b) * (1 - 2 * (1 - a)*(1 - b)); 
					res = pow(res, 2.2);
					return res;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					float2 uv = v.uv;

					int ix = (int)uv.x;
					int iy = (int)uv.y;
					//o.frustumDir = _FrustumDir[ix + 2 * iy];

					o.uv = uv;
					#ifdef AA
					float4 extents;
					float2 offset = (_MainTex_TexelSize.xy) * 0.5f;
					extents.xy = v.uv - offset;
					extents.zw = v.uv + offset;

					float4 rcpSize;
					rcpSize.xy = -_MainTex_TexelSize.xy * 0.5f;
					rcpSize.zw = _MainTex_TexelSize.xy * 0.5f;
#if defined (SHADER_API_PSP2)
					//cg compiler linker bug workaround
					float almostzero = v.uv.x*0.000001f;
					rcpSize.x += almostzero;
#endif
					o.interpolatorA = extents;
					o.interpolatorB = rcpSize;
					o.interpolatorC = rcpSize;

					o.interpolatorC.xy *= 4.0;
					o.interpolatorC.zw *= 4.0;
					#endif
					return o;
				}

				float Noise(float t)
				{
					float res = cos(t) * cos(3 * t) * cos(5 * t) + sin(7 * t) * 0.3;
					return res;
				}

				half _RBForce;
				float _A, _B, _C, _D;
				half _RotSpeed;

				fixed4 frag(v2f i) : SV_Target
				{

					//i.uv.x += Noise(_Time.y + i.uv.y * 12) * 0.002 * i.uv.y;
					//i.uv.y += Noise(_Time.y + i.uv.x * 6) * 0.001 * i.uv.y;

#if defined (AA)

					fixed4 col = 1;
					
					col.rgb = FxaaPixelShader(i.uv, i.interpolatorA, i.interpolatorB, i.interpolatorC);

#else
					fixed4 col = tex2D(_MainTex, i.uv);
#endif

#if defined (CHROMATIC_ABERRATION)					
					i.uv.x += (i.uv.x - 0.5) * 0.005;
					col.b = tex2D(_MainTex, i.uv).b;
					i.uv.x -= (i.uv.x - 0.5) * 0.010;
					col.r = tex2D(_MainTex, i.uv).r;
#endif

#ifdef ROTATE_BLUR
					half3 rotBlur = col.rgb * 2;
					float coef = i.uv.y * pow(length(i.uv - 0.5), 2) * 5 *_RotSpeed;
					i.uv.x += 0.0030 * coef;
					rotBlur += tex2D(_MainTex, i.uv).rgb;
					i.uv.x -= 0.0060 * coef;
					rotBlur += tex2D(_MainTex, i.uv).rgb;
					rotBlur *= 0.25;
					col.rgb = rotBlur;
					//col.rgb = lerp(col.rgb, rotBlur, i.uv.y * (pow(length(i.uv - 0.5), 1)));
#endif

#ifdef RADIUS_BLUR
					float2 uvBlur = i.uv - 0.5;
					uvBlur.x *= 2;
					float centerCoef = length(uvBlur);
					centerCoef = (1 - pow(1 - centerCoef, 4)) * _RBForce;
					col.rgb = lerp(col.rgb, tex2D(_MainTex, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.012).rgb, lerp(0, 0.5, centerCoef));
					col.rgb = lerp(col.rgb, tex2D(_MainTex, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.023).rgb, lerp(0, 0.25, centerCoef));
					col.rgb = lerp(col.rgb, tex2D(_MainTex, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.035).rgb, lerp(0, 0.125, centerCoef));
					col.rgb = lerp(col.rgb, tex2D(_MainTex, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.047).rgb, lerp(0, 0.0625, centerCoef));
					//col.rgb = lerp(col.rgb, tex2D(_MainTex, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.017).rgb, lerp(0, 0.5, centerCoef));
#endif

					//col.rgb += tex2D(_BlurRT, i.uv - (i.uv - 0.5) * 0.01).rgb;
					//col.rgb += tex2D(_BlurRT, i.uv - (i.uv - 0.5) * 0.02).rgb;
					//col.rgb += tex2D(_BlurRT, i.uv - (i.uv - 0.5) * 0.03).rgb;

					//col *= 0.5;

					col.rgb = saturate(col.rgb);

	#if USE_LUT
	//#if HIGH_COLOR_RANGE
					//col.rgb = lerp(linearSpaceTex3D(_LUTTex, col.rgb), linearSpaceTex3D(_LUTTex2, col.rgb), _LerpLUT).rgb;
					//col.rgb = lerp(linearSpaceTex3D(_LUTTex, saturate(col.rgb)), linearSpaceTex3D(_LUTTex2, saturate(col.rgb)), _LerpLUT).rgb;
	//#else
					col.rgb = lerp(linearSpaceTex3D(_LUTTex, col.rgb), linearSpaceTex3D(_LUTTex2, col.rgb), _LerpLUT).rgb;
	//#endif
	#endif 

#ifdef RADIUS_BLUR
					/*float2 uvBlur = i.uv - 0.5;
					uvBlur.x *= 2;
					float centerCoef = length(uvBlur);
					centerCoef = 1 - pow(1 - centerCoef, 8);*/
					//col.rgb = lerp(col.rgb, tex2D(_BlurRT, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.02).rgb, lerp(0, 0.1, centerCoef));
					//col.rgb = lerp(col.rgb, tex2D(_BlurRT, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.03).rgb, lerp(0, 0.1, centerCoef));
					//col.rgb = lerp(col.rgb, tex2D(_BlurRT, i.uv - lerp(0, i.uv - 0.5, centerCoef) * 0.023).rgb, lerp(0, 0.125, centerCoef));
					//i.uv -= 0.5;
					//col.rgb = centerCoef;
#endif

					//col.rgb = saturate(col.rgb);

#ifdef VIGNETTE
					half2 darkCorner = saturate(1 - pow((0.5 - i.uv) * 2, 2) + float2(0.7, 0.5)) * 0.2 + 0.8;
					col.rgb *= saturate(1 - pow((0.5 - i.uv.x) * 2, 4) + 0.5);
					col.rgb *= (darkCorner.x * darkCorner.y * 0.5 + 0.5);
#endif			

					//return pow(centerCoef, 2.2);
					//col.rgb *= 1.1;
#ifdef AUTO_EXPOSURE
					float ae = lum(tex2D(_AutoExplosureTex,0).rgb);
					ae = _C / (clamp(ae, 0, _B) + 0);
					col.rgb = col.rgb * ae;
#endif
//col = 1 - pow(length(i.uv - 0.5), 1);
					//col.rgb = lerp(0.5, col.rgb, 1.025);
					//col.rgb = lerp(lum(col.rgb), col.rgb, float3(0.95, 0.95, 1.00));					
					//col.rgb *= 1.1;
					return col;
				}
				ENDCG
			}

/*			CGINCLUDE

	#define RADIAL_SAMPLE_COUNT 6
	#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_TexelSize;
			sampler2D _BlurTex;
			fixed4 _BlurTex_TexelSize;
			fixed4 _ViewPortLightPos;

			fixed4 _offsets;
			fixed4 _ColorThreshold;
			fixed4 _LightColor;
			float _LightFactor;
			float _PowFactor;
			float _LightRadius;
			float _LightSaturation;
			//fixed4 _VolumetricLightColor;

				struct v2f_threshold
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f_threshold vert_threshold(appdata_img v)
			{
				v2f_threshold o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;

	#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
	#endif	
				return o;
			}

			fixed4 frag_threshold(v2f_threshold i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				float distFromLight = length(_ViewPortLightPos.xy - i.uv);
				float distanceControl = saturate(_LightRadius - distFromLight);
				float4 thresholdColor = saturate(color - _ColorThreshold) * distanceControl;
				//float lumColor = lum(thresholdColor.rgb);
				//lumColor = pow(lumColor, _PowFactor);
				thresholdColor.rgb = pow(thresholdColor, _PowFactor);
				thresholdColor.a = 1;
				return thresholdColor;
				//return fixed4(lumColor, lumColor, lumColor, 1);
			}

				struct v2f_blur
			{
				float4 pos : SV_POSITION;
				float2 uv  : TEXCOORD0;
				float2 blurOffset : TEXCOORD1;
			};

			v2f_blur vert_blur(appdata_img v)
			{
				v2f_blur o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				o.blurOffset = _offsets * (_ViewPortLightPos.xy - o.uv);
				return o;
			}

			fixed3 lum(fixed3 c)
			{
				return dot(c, fixed3(0.22, 0.707, 0.071));
			}

			fixed4 frag_blur(v2f_blur i) : SV_Target
			{
				half4 color = half4(0,0,0,0);
				for (int j = 0; j < RADIAL_SAMPLE_COUNT; j++)
				{
					color += tex2D(_MainTex, i.uv.xy);
					i.uv.xy += i.blurOffset;
				}
				color /= RADIAL_SAMPLE_COUNT;

				color.rgb = lerp(lum(color.rgb), color.rgb, _LightSaturation);
				color += _LightFactor * color * _LightColor;
				color.a = 1;

				return color;
			}

				ENDCG

			Pass
			{
				ZTest Off
				Cull Off
				ZWrite Off
				Fog{ Mode Off }

				CGPROGRAM
				#pragma vertex vert_threshold
				#pragma fragment frag_threshold
				ENDCG
			}

			Pass
			{
				ZTest Off
				Cull Off
				ZWrite Off
				Fog{ Mode Off }

				CGPROGRAM
				#pragma vertex vert_blur
				#pragma fragment frag_blur
				ENDCG
			}*/

			Pass
			{
				ZTest Off
				Cull Off
				ZWrite Off
				Fog{ Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _AERTMemo;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 memo = tex2D(_AERTMemo, i.uv).rgb;
				col.rgb = lerp(col.rgb, memo, 0.966);
				// just invert the colors
				//col.rgb = 1 - col.rgb;
				return col;
			}
			ENDCG
			}
		}
}
