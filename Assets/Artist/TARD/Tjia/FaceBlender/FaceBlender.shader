Shader "SAO_TJia_V3/FaceBlender"
{
	Properties
	{
		_MainTex ("脸", 2D) = "white" {}
		_LightMap ("脸Mask", 2D) = "white" {}

		_EyeTex ("眼", 2D) = "white" {}
		_LightMapEye ("眼Mask", 2D) = "white" {}
		_EyeOri("眼原始区域", vector) = (0,1,0,1)
		_EyeTar("眼目标区域", vector) = (0,1,0,1)
		[HDR]_EyeColor("眼颜色", color) = (1,1,1,1)

		_MouthTex ("嘴", 2D) = "white" {}
		_LightMapMouth ("嘴Mask", 2D) = "white" {}
		_MouthOri("嘴原始区域", vector) = (0,1,0,1)
		_MouthTar("嘴目标区域", vector) = (0,1,0,1)
		[HDR]_MouthColor("嘴颜色", color) = (1,1,1,1)


		_MakeupTex ("妆容", 2D) = "white" {}
		[KeywordEnum(R, G, B, A)]_MakeupChanel("妆容通道", float) = 0
		_MakeupOri("妆容原始区域", vector) = (0,1,0,1)
		_MakeupTar("妆容目标区域", vector) = (0,1,0,1)
		_MakeupColor("妆容颜色", Color) = (1,1,1,1)
		
		
		//_LightMapMakeup ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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

			sampler2D _EyeTex, _LightMapEye;
			float4 _EyeOri, _EyeTar;
			fixed4 _EyeColor;

			sampler2D _MouthTex;
			float4 _MouthOri, _MouthTar;
			fixed4 _MouthColor;

			sampler2D _MakeupTex;
			float4 _MakeupOri, _MakeupTar;
			fixed4 _MakeupColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col		= tex2D(_MainTex, i.uv);

				float2 makeupUv = saturate((_MakeupOri.yw - _MakeupOri.xz) / (_MakeupTar.yw - _MakeupTar.xz) * (i.uv - _MakeupTar.xz) + _MakeupOri.xz);
				half  cond		= step(_MakeupOri.x, makeupUv.x) * step(_MakeupOri.z, makeupUv.y) * step(makeupUv.x, _MakeupOri.y) * step(makeupUv.y, _MakeupOri.w);
				fixed4 makeup	= tex2D(_MakeupTex, makeupUv) * _MakeupColor;
				col.rgb			= lerp(col.rgb, makeup.rgb, makeup.a * cond);

				float2 eyeUv	= saturate((_EyeOri.yw - _EyeOri.xz) / (_EyeTar.yw - _EyeTar.xz) * (i.uv - _EyeTar.xz) + _EyeOri.xz);
				cond			= step(_EyeOri.x, eyeUv.x) * step(_EyeOri.z, eyeUv.y) * step(eyeUv.x, _EyeOri.y) * step(eyeUv.y, _EyeOri.w);
				fixed4 eye		= tex2D(_EyeTex, eyeUv);
				fixed4 eyeMask	= tex2D(_LightMapEye, eyeUv);
				eye.rgb			*= lerp(1, _EyeColor.rgb, eyeMask.a * (1 - eye.r));
				col.rgb			= lerp(col.rgb, eye, eye.a * _EyeColor.a * cond);
				col.a			= 1 - (eye.a * cond - eyeMask.a);

				float2 mouthUv	= saturate((_MouthOri.yw - _MouthOri.xz) / (_MouthTar.yw - _MouthTar.xz) * (i.uv - _MouthTar.xz) + _MouthOri.xz);
				cond			= step(_MouthOri.x, mouthUv.x) * step(_MouthOri.z, mouthUv.y) * step(mouthUv.x, _MouthOri.y) * step(mouthUv.y, _MouthOri.w);
				fixed4 mouth	= tex2D(_MouthTex, mouthUv);
				col.rgb			= lerp(col.rgb, mouth * _MouthColor.rgb, mouth.a * _MouthColor.a * cond);

				return col;
			}
			ENDCG
		}

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

			sampler2D _MainTex, _LightMap;

			sampler2D _EyeTex, _LightMapEye;
			float4 _EyeOri, _EyeTar;
			fixed4 _EyeColor;

			sampler2D _MouthTex, _LightMapMouth;
			float4 _MouthOri, _MouthTar;
			fixed4 _MouthColor;

			sampler2D _MakeupTex;
			float4 _MakeupOri, _MakeupTar;
			fixed4 _MakeupColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 mask = tex2D(_LightMap, i.uv);

				float2 eyeUv = saturate((_EyeOri.yw - _EyeOri.xz) / (_EyeTar.yw - _EyeTar.xz) * (i.uv - _EyeTar.xz) + _EyeOri.xz);
				half cond = step(_EyeOri.x, eyeUv.x) * step(_EyeOri.z, eyeUv.y) * step(eyeUv.x, _EyeOri.y) * step(eyeUv.y, _EyeOri.w);
				fixed4 eye = tex2D(_EyeTex, eyeUv) * _EyeColor;
				fixed4 eyeMask = tex2D(_LightMapEye, eyeUv);
				mask.rgb = lerp(mask.rgb, eyeMask.rgb, eye.a * cond);


				float2 mouthUv = saturate((_MouthOri.yw - _MouthOri.xz) / (_MouthTar.yw - _MouthTar.xz) * (i.uv - _MouthTar.xz) + _MouthOri.xz);
				cond = step(_MouthOri.x, mouthUv.x) * step(_MouthOri.z, mouthUv.y) * step(mouthUv.x, _MouthOri.y) * step(mouthUv.y, _MouthOri.w);
				fixed4 mouth = tex2D(_MouthTex, mouthUv) * _MouthColor;
				fixed4 mouthMask = tex2D(_LightMapMouth, mouthUv);
				mask.rgb = lerp(mask.rgb, mouthMask.rgb, mouth.a * cond);

				return mask;
			}
			ENDCG
		}
	}
}
