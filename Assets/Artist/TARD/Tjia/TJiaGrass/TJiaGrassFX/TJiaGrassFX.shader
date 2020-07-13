// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "SAO_TJia_V3/LightedParticles/TJiaGrassFX" {
Properties {
    //_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
    _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "LightMode" = "ForwardBase"}
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles

            #include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"

            sampler2D _MainTex;
            fixed4 _GrassColor;

            struct appdata_t {
                float4 vertex : POSITION;
				float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 worldPositon : TEXCOORD3;
                float4 texcoord : TEXCOORD0;
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;

			sampler2D _GrassInfoTex;
			sampler2D _LmColor;
			float3 _LmCamPos;
			half3 _LightmapColor;
			half _LightmapForce;
			fixed _NoLightmap;
			float _SunOutDoor;
			float _LmScale;

            v2f vert (appdata_t v)
            {
                v2f o;
				o.worldPositon.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldPositon.w = v.color.a;
				float2 lmUv = (o.worldPositon.xz - _LmCamPos.xz) / _LmScale + 0.5;
				float2 grassInfo = tex2Dlod(_GrassInfoTex, float4(lmUv, 0, 0)).ba;
				o.texcoord.zw = grassInfo;

				//v.vertex *= grassInfo.x;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
                
                o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);


                return o;
            }

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            float _InvFade;

			half lum(half3 c)
			{
				return saturate(dot(c, half3(0.22, 0.707, 0.071)));
			}



            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate (_InvFade * (sceneZ-partZ));
                i.color.a *= fade;
                #endif

				fixed4 tex = tex2D(_MainTex, i.texcoord.xy);
                fixed4 col = 1;
				

				float2 lmUv = (i.worldPositon.xz - _LmCamPos.xz) / _LmScale + 0.5;
				
				half4 lmc = tex2D(_LmColor, lmUv);
				lmc.rgb = lmc.rgb * _LightmapColor * _LightmapForce * 2 * (1 - _NoLightmap);
				float2 grassInfo = i.texcoord.zw;
				float diffuse = grassInfo.y * lmc.a;
				lmc.rgb *= saturate(diffuse + dot( lmc.rgb, float3(0.22,0.707,0.071)) * 2 * (_SunOutDoor * 0.8 + 0.2));
				float3 ambient = unity_AmbientSky.rgb * saturate(1 - lmc.rgb) + lmc.rgb;
				float3 lighting = ambient + _LightColor0.rgb * 1.6 * diffuse;

				col.rgb = lighting * _GrassColor.rgb * (tex * 0.4 + 1.0) * float3(0.9, 0.9, 1);

				col.a *= tex.a * grassInfo.x * grassInfo.x * i.worldPositon.w;

                col.a = saturate(col.a); // alpha should not have double-brightness applied to it, but we can't fix that legacy behaior without breaking everyone's effects, so instead clamp the output to get sensible HDR behavior (case 967476)

                return col;
            }
            ENDCG
        }
    }
}
}
