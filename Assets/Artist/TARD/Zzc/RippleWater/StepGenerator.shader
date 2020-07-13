Shader "Zzc/StepGenerator"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_StepBump("StepBump",2D)="bump"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
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

			sampler2D _MainTex;
			sampler2D _StepBump;
			float4 _MainTex_ST;

			float3 _DeltaPos;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float delta = saturate(length(_DeltaPos.xz)*5);
				float4 col = tex2D(_MainTex,(i.uv-0.5)*(0.91+0.01*(3-delta*3))+0.5 + _DeltaPos.xz*0.08);
				float4 stepCol = tex2D(_StepBump, (i.uv - 0.5) * 5 + 0.5);					
				col.rgb -= 0.015;
				col = saturate(col);
				col.rgb += stepCol.rgb*stepCol.a*0.4*(delta*0.8+0.2)*(sin(_Time.y % 360 * 50)*0.2+0.8)*saturate(1-col.rgb*2);
				col.rgb = saturate(col.rgb);
				float cond = step(abs(i.uv.x - 0.5), 0.499)*step(abs(i.uv.y - 0.5), 0.499);
				col = lerp(float4(0.5, 0.5, 1, 0), col, cond);
				float temp = distance(i.uv , float2(0.5,0.5));
				col.rgb *= smoothstep(0.5, 0.4, temp);
				return float4(col);
			}
			ENDCG
		}

		Pass{
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

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col;
				col = float4(0, 0, 0, 0);
				return col;
			}
			ENDCG
		}
	}
}
