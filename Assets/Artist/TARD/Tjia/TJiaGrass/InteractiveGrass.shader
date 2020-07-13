Shader "Unlit/InteractiveGrass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WalkTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass //0
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
			sampler2D _WalkTex;
			float4 _MainTex_ST;

			float3 _PlayerPosition;
			float3 _LastPlayerPos;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 dUv = (_PlayerPosition.xz - _LastPlayerPos.xz) * 0.0125;
				float4 col = tex2D(_MainTex, i.uv + dUv);
				float4 walk = tex2D(_WalkTex, (i.uv - 0.5) * 20 + 0.5);

				//col = walk;
				float clear = step(0.245, dot(i.uv - 0.5, i.uv - 0.5));
				float coef = 0.025 + clear * 0.975;
				col.rgb = lerp(col.rgb, walk.rgb, step(0.1, walk.b));
				col.rgb = lerp(col.rgb, float3(0.5, 0.5, 0), coef);
				
				col.a *= (1 - clear);

				return col;
			}
			ENDCG
		}

		Pass //Rond Skill
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
			sampler2D _WalkTex;
			float4 _MainTex_ST;

			float3 _PlayerPosition;
			float3 _LastPlayerPos;
			float _SkillRange;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float2 dUv = (_PlayerPosition.xz - _LastPlayerPos.xz) * 0.0125;
				float4 col = tex2D(_MainTex, i.uv + dUv);

				float4 walk = tex2D(_WalkTex, (i.uv - 0.5) * 10 / _SkillRange + 0.5);

				col.rgb = lerp(col.rgb, walk.rgb, step(0.1, walk.b));
				col.a = max(col.a, walk.a);
				return col;
			}
			ENDCG
		}

		Pass //Sector Skill
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
			sampler2D _WalkTex;
			float4 _MainTex_ST;

			float3 _PlayerPosition;
			float3 _PlayerDir;
			float3 _LastPlayerPos;
			float _SkillAngle;
			float _SkillRange;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 dUv = (_PlayerPosition.xz - _LastPlayerPos.xz) * 0.0125;
				float4 col = tex2D(_MainTex, i.uv + dUv);

				float2 playerDir = normalize(float2(_PlayerDir.x, _PlayerDir.z));
				float2 skillDir = normalize(i.uv - 0.5);
				float cosAngle = dot(playerDir, skillDir);
				float cond = step(cosAngle, cos(_SkillAngle * 0.5));

				float4 walk = tex2D(_WalkTex, (i.uv - 0.5) * 10 / _SkillRange + 0.5);
				walk = lerp(walk, float4(0.5, 0.5, 0, 0), cond); // Rond


				col.rgb = lerp(col.rgb, walk.rgb, step(0.1, walk.b));
				col.a = max(col.a, walk.a);

				//col = condCarre;

				return col;
			}
			ENDCG
		}

		Pass //Square Skill
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
			sampler2D _WalkTex;
			float4 _MainTex_ST;

			float3 _PlayerPosition;
			float3 _PlayerDir;
			float3 _LastPlayerPos;
			float _SkillLength;
			float _SkillRange;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 dUv = (_PlayerPosition.xz - _LastPlayerPos.xz) * 0.0125;
				float4 col = tex2D(_MainTex, i.uv + dUv);

				float2 playerDir = normalize(float2(_PlayerDir.x, _PlayerDir.z));
				float2 biPlayerDir = float2(playerDir.y, -playerDir.x);
				float2 uvCarre = dot(biPlayerDir, i.uv - 0.5) * biPlayerDir;
				float projectedUv = dot(playerDir, i.uv - 0.5);
				float condCarre = step(0, dot(playerDir, i.uv - 0.5)) * step(projectedUv, _SkillRange * 0.04 * _SkillLength);

				float4 walk = tex2D(_WalkTex, uvCarre * 10 / _SkillRange + 0.5);
				walk = lerp(walk, float4(0.5, 0.5, 0, 0), 1 - condCarre); // Carre

				col.rgb = lerp(col.rgb, walk.rgb, step(0.1, walk.b));
				col.a = max(col.a, walk.a);

				//col = condCarre;

				return col;
			}
			ENDCG
		}
	}
}
