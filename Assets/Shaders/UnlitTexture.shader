Shader "Custom/UnlitTexture"
{
	Properties
	{
		[MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
		[MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
	}

		// Universal Render Pipeline subshader. If URP is installed this will be used.
		SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

		Pass
		{
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma require geometry
			#pragma geometry geom

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float2 uv           : TEXCOORD0;
			};

			struct Varyings
			{
				float2 uv           : TEXCOORD0;
				float4 position  : SV_POSITION;
			};

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST;
			half4 _BaseColor;
			CBUFFER_END

			Varyings vert(Attributes IN)
			{
				Varyings OUT;
				float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);// + off;
				OUT.position = float4(positionWS, 1);
				OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
				return OUT;
			}

			[maxvertexcount(3)]
			void geom(triangle Varyings input[3], inout TriangleStream<Varyings> triStream) {
				Varyings v;
				v.uv = float2(0, 0);
				v.position = TransformWorldToHClip(input[0].position.xyz + float3(-0.1, 0, 0));
				triStream.Append(v);

				v.uv = float2(0, 0);
				float3 pos = input[0].position.xyz + float3(0.1, 0, 0);
				float4 smp = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, pos.xz + float2(_Time.x, _Time.x), 0) * 2 - 1;
				float3 off = float3(smp.x, 0, smp.y);
				v.position = TransformWorldToHClip(pos + off);
				triStream.Append(v);

				v.uv = float2(0, 0);
				pos = input[0].position.xyz + float3(0, 0.3, 0);
				smp = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, pos.xz + float2(_Time.x, _Time.x), 0) * 2 - 1;
				off = float3(smp.x, 0, smp.y);
				v.position = TransformWorldToHClip(pos + off);
				triStream.Append(v);
			}

			half4 frag(Varyings IN) : SV_Target
			{
				return SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, IN.uv + float2(sin(_Time.y), sin(_Time.y)), 0) * _BaseColor;
			}
			ENDHLSL
		}
	}
}