Shader "Unlit/TJiaPlanarReflection"
{
	Properties
	{
		_MainTex ("扰动(法线)", 2D) = "bump" {}
		_Force ("扰动强度", Range(0,4)) = 1
		_RefAlpha ("反射强度", Range(0,10)) = 1
		[Toggle(_DITHER_ON)] _Dither ("使用Cubemap", float) = 0
		_CubeMap("Cubemap", Cube) = "black"{}
		[HDR]_CubeColor("Cube反射颜色", color) = (1,1,1,1)
		_CubeDistortion ("Cube扰动", Range(0, 0.2)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+1"}
		//Tags {"RenderType"="Opaque"}
		LOD 100
		//Blend SrcAlpha One
		Blend SrcAlpha One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _DITHER_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 normal : TEXCOORD3;
			};

			sampler2D _MainTex;
			sampler2D _ReflectionTex;
			float2 _ReflectionTex_TexelSize;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(UNITY_MATRIX_M, v.vertex).xyz;
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			float lum(float3 c)
			{
				return dot(c, float3(0.22, 0.707, 0.071));
			}

			float _Force;
			float _RefAlpha;
			#ifdef _DITHER_ON
				samplerCUBE _CubeMap;
				float3 _CubeColor;
				float _CubeDistortion;
			#endif

		half Max3(half3 c)
		{
			return max(c.r, max(c.g, c.b));
		}

		float4x4 _DecalProjectionMatrix;
		sampler2D _DecalTexture;
		//sampler2D _DecalTexture2;
		//float _EnableDecal;

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

			decalColor = decalColor * step(abs(decalProj.x - 0.5), 0.499) * step(abs(decalProj.y - 0.5), 0.499)*1;// *step(decalColor, 0.0000001);				
			fixed normalCoef = pow(saturate(worldNormalY),2);
			half4 res = decalColor * normalCoef;
			half decalAlpha = Max3(res.rgb);
			res.rgb = lerp(col.rgb, res.rgb * (decalAlpha * 2 + 1), saturate(res.a * (decalAlpha * 4 + 1))) + res.rgb;
			return res.rgb;
		}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 distortion = tex2D(_MainTex, i.uv).xy * 2 - 1;
				float2 refUv = i.screenPos.xy / i.screenPos.w;
				float3 viewVec = _WorldSpaceCameraPos - i.worldPos;
				float3 viewDir = normalize(viewVec);
				float3 normal = normalize(i.normal);				
				float nv = dot(normal, viewDir);

				refUv += distortion * viewDir.y * 0.025 * _Force;
				
				float4 col = tex2D(_ReflectionTex, refUv);

				#ifdef _DITHER_ON
					normal.xz += distortion * _CubeDistortion;
					float3 worldRef = reflect(-viewDir, normalize(normal));
					col.rgb += texCUBE(_CubeMap, worldRef).rgb * _CubeColor;
				#endif

				col.a = lum(col.rgb);

				/*float3 dUv = float3(_ReflectionTex_TexelSize, 0);
				col.rgb += tex2D(_ReflectionTex, refUv + dUv.xy);
				col.rgb += tex2D(_ReflectionTex, refUv + dUv.xz);
				col.rgb += tex2D(_ReflectionTex, refUv + dUv.zy);
				col.rgb *= 0.25;*/

				col.rgb = decalTexCol(col.rgb, i.worldPos, normal.y);

				col.a = lum(col.rgb) * 0.5 + col.a * 0.5;

				col.a = saturate((col.a * 0.25 + pow(1 - nv, 16) + 0.1) * _RefAlpha) * saturate(normal.y * 0.875 + 0.125);

				//col.rgb = 0.5;

				//col.a = 1;

				return col;
			}
			ENDCG
		}
	}
}
