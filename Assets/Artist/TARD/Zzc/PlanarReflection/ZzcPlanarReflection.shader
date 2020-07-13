Shader "Zzc/ZzcPlanarReflection"
{
	Properties{
		_Alpha("Alpha", Range(0, 1)) = 0.8
		_Fresnel("Fresnel",Range(0,1))=0.5
		_FresnelPow("FresnelPow",Range(0,32)) = 5
		_NoiseTex("NoiseTex",2D)="white"{}
		_Distort("Distort",Range(0,1))=0.5
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Pass
		{
			ZWrite Off
			Blend SrcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;		
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 screenPos:TEXCOORD0;
				float3 worldViewDir:TEXCOORD2;
				float3 worldPos:TEXCOORD1;
				float4 vertex : SV_POSITION;
				float2 uv:TEXCOORD3;
			};

			sampler2D _ReflectionTex;
			float _Alpha;
			float _Fresnel;
			sampler2D _NoiseTex;
			float _Distort;
			float _FresnelPow;

			float lum(float3 c)
			{
				return dot(c, float3(0.22, 0.707, 0.071));
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 worldNormal = float3(0,1,0);
				float3 worldViewDir = normalize(i.worldViewDir);

				float distance2Center = length(i.uv - float2(0.5, 0.5));
				float centerReduce = smoothstep(0.01, 0.05, distance2Center);

				float fresnel = _Fresnel + (1 - _Fresnel)*pow(1 - dot(worldViewDir, worldNormal), _FresnelPow);
				float2 noiseUV = (tex2D(_NoiseTex, i.uv + float2(frac(_Time.x), frac(_Time.x))).xx-0.25)*0.02*_Distort*centerReduce;

				float4 col = tex2D(_ReflectionTex,i.screenPos.xy / i.screenPos.w+ noiseUV);
				col.rgb *= _Alpha;		
				col.rgb = lerp(float3(0,0,0), col.rgb, fresnel);
				col.a = 1-lum(col.rgb)*0.3;
				return col;
			}
			ENDCG
		}
	}
}
