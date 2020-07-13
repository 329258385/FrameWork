Shader "Hidden/MangaScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainTex2 ("Texture2", 2D) = "white" {}
		_Angle("角度", Range(0, 3.14159)) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _MainTex2;
			float _Angle;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_MainTex2, i.uv);

				float tanAngle = tan(_Angle);
				float vValue = tanAngle * (i.uv.x - 0.5) + 0.5;

				float Coef = step(vValue, i.uv.y);
				float cc = step(_Angle, 1.570795);
				Coef = Coef * cc + (1 - Coef) * (1 - cc);

				col = lerp(col, col2, Coef);
				return col;
			}
			ENDCG
		}
	}
}
