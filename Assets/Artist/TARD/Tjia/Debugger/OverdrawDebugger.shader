Shader "SAO_TJia_V3/Debugger/Overdraw"
{
	Properties
	{
	}
	SubShader
	{
		//Tags { "RenderType"="Tranparent" "Queue"="Transparent"}
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		LOD 100
		ZTest Always
		ZWrite Off
		Blend One One

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.1, 0.04, 0.02, 1);// 
				//col.rgb *= tex2D(_MainTex, i.uv).a;
				return col;
			}
			ENDCG
		}
	}
}
