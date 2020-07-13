Shader "Hidden/TJia/PFX/TjiaNewBloom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_GaussianScale ("高斯大小", Range(0,1)) = 1
	}
	CGINCLUDE

			
			
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

			float max3(float3 c)
			{
				return max(max(c.r,c.g),c.b);
			}
			
			sampler2D _MainTex;

			float2 Gaussian(float2 v)
			{
				float2 res;
				res.x = frac(sin(dot(v, float2(1134.5621, 1134.5242)) + _Time.x)  * 19880.803) - 0.5;
				res.y = frac(sin(dot(1 - v, float2(1236.5423, 1432.5651)) + _Time.x)  * 1988.0803) - 0.5;

				//res = exp(- res * res * 16) * sign(res);
				//float dis = dot(res, res);
				//res *= (step(dis, 0.25) * 0.7 + 0.3);

				return res;
			}

			float _GaussianScale;
			float2 _MainTex_TexelSize;

			float4 fragOne (v2f i) : SV_Target 
			{
				float2 delta = float2(1, 2);
				float2 duv0 = Gaussian(i.uv + delta.xx + _GaussianScale) * _GaussianScale * 200 + 0.5; 
				float2 duv1 = Gaussian(i.uv + delta.yy + _GaussianScale) * _GaussianScale * 200 + 0.5; 
				float2 duv2 = Gaussian(i.uv + delta.xy + _GaussianScale) * _GaussianScale * 200 + 0.5; 
				float2 duv3 = Gaussian(i.uv + delta.yx + _GaussianScale) * _GaussianScale * 200 + 0.5; 

				float4 col = 1;
				float2 delta2 = float2(1, -1);
				col.rgb  = tex2D(_MainTex, i.uv + duv0 * _MainTex_TexelSize * delta2.xx).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv1 * _MainTex_TexelSize * delta2.yy).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv2 * _MainTex_TexelSize * delta2.xy).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv3 * _MainTex_TexelSize * delta2.yx).rgb;
				col.rgb *= 0.25;
				return col;
			}			

			float _Threshold;
			float _Intensity;
			
			float4 fragPre (v2f i) : SV_Target
			{
				float2 delta = float2(2, 3);
				float2 duv0 = Gaussian(i.uv + delta.xx) * _GaussianScale * 200 + 0.5; 
				float2 duv1 = Gaussian(i.uv + delta.yy) * _GaussianScale * 200 + 0.5; 
				float2 duv2 = Gaussian(i.uv + delta.xy) * _GaussianScale * 200 + 0.5; 
				float2 duv3 = Gaussian(i.uv + delta.yx) * _GaussianScale * 200 + 0.5; 

				float4 col = 1;
				col.rgb  = saturate(tex2D(_MainTex, i.uv + duv0 * _MainTex_TexelSize).rgb - _Threshold);
				col.rgb += saturate(tex2D(_MainTex, i.uv + duv1 * _MainTex_TexelSize).rgb - _Threshold);
				col.rgb += saturate(tex2D(_MainTex, i.uv + duv2 * _MainTex_TexelSize).rgb - _Threshold);
				col.rgb += saturate(tex2D(_MainTex, i.uv + duv3 * _MainTex_TexelSize).rgb - _Threshold);
				col.rgb *= 0.25; 
				return col;
			}


			float4 fragTwo (v2f i) : SV_Target 
			{
				float2 delta = float2(3, 4);
				float2 duv0 = Gaussian(i.uv + delta.xx) * _GaussianScale * 200 + 0.5; 
				float2 duv1 = Gaussian(i.uv + delta.yy) * _GaussianScale * 200 + 0.5; 
				float2 duv2 = Gaussian(i.uv + delta.xy) * _GaussianScale * 200 + 0.5; 
				float2 duv3 = Gaussian(i.uv + delta.yx) * _GaussianScale * 200 + 0.5; 

				float4 col = 1;
				float3 delta2 = float3(1, -1, 0);
				col.rgb  = tex2D(_MainTex, i.uv + duv0 * _MainTex_TexelSize * delta2.xz);
				col.rgb += tex2D(_MainTex, i.uv + duv1 * _MainTex_TexelSize * delta2.yz).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv2 * _MainTex_TexelSize * delta2.zx).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv3 * _MainTex_TexelSize * delta2.zy).rgb;
				col.rgb *= 0.25;
				return col;
			}	

			sampler2D _OriginalTex;

			float4 fragFinal (v2f i) : SV_Target
			{
				float2 delta = float2(1, 2);
				float2 duv0 = Gaussian(i.uv + delta.xx + _GaussianScale) * _GaussianScale * 200 + 0.5; 
				float2 duv1 = Gaussian(i.uv + delta.yy + _GaussianScale) * _GaussianScale * 200 + 0.5; 
				float2 duv2 = Gaussian(i.uv + delta.xy + _GaussianScale) * _GaussianScale * 200 + 0.5; 
				float2 duv3 = Gaussian(i.uv + delta.yx + _GaussianScale) * _GaussianScale * 200 + 0.5; 
				// just invert the colors

				float4 col = 1;
				float2 delta2 = float2(1, -1);
				col.rgb  = tex2D(_MainTex, i.uv + duv0 * _MainTex_TexelSize * delta2.xx).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv1 * _MainTex_TexelSize * delta2.yy).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv2 * _MainTex_TexelSize * delta2.xy).rgb;
				col.rgb += tex2D(_MainTex, i.uv + duv3 * _MainTex_TexelSize * delta2.yx).rgb;
				col.rgb *= 0.25;

				col *= _Intensity;

				float4 ori = tex2D(_OriginalTex, i.uv);

				col.rgb += ori;
				//col.rg = duv;
				//col = duv;
				return col;
			}

	ENDCG
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragPre
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragOne
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragTwo
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragFinal
			ENDCG
		}
	}
}
