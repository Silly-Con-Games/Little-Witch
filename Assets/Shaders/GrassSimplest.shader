Shader "Custom/GrassSimplest"
{
	Properties
	{
		_WindTex ("Wind texture", 2D) = "white" {}
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (0,0,0,1)
		_BladesPerTriangle("Blades per Triangle", Float) = 5
		_Height("Height", Float) = 0.5
		_Width("Width", Float) = 0.5
		_WindStrength("Wind Strength", Float) = 1
		_WindSpeed("Wind Speed", Float) = 1
		_TiltModifier("Tilt mod.", Float) = 1
	}

	HLSLINCLUDE

	#define MAX_BLADES 20
	#define VERTICES_PER_BLADE 7

	#pragma vertex vert
	#pragma fragment frag
	#pragma require geometry
	#pragma geometry geom

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"

	struct Attributes {
		float4 positionOS : POSITION;
	};

	struct Varyings {
		float4 position : SV_POSITION;
		float2 uv : TEXTCOORD0;
	};

	TEXTURE2D(_WindTexture);
	SAMPLER(sampler_WindTexture);

	CBUFFER_START(UnityPerMaterial)
		float4 _BaseMap_ST;
	CBUFFER_END

	Varyings vert(Attributes input) {
		Varyings output;
		float3 position = TransformObjectToWorld(input.positionOS.xyz);
		float4 windSample = SAMPLE_TEXTURE2D_LOD(_WindTexture, sampler_WindTexture, position.xz + float2(_Time.y, _Time.y), 0) * 2 - 1;
		float3 windDir = float3(windSample.x, 0, windSample.y);
		output.position = float4(position, 1);
		
		return output;
	}

	float _Height;
	float _Width;

	static const float3 bladeVertices[] = {
			float3(0.15, 0, 0),
			float3(-0.15, 0, 0),
			float3(0.2, 0.3, 0),
			float3(-0.2, 0.3, 0),
			float3(0.1, 0.7, 0),
			float3(-0.1, 0.7, 0),
			float3(0, 1, 0)
	};
	static const float uvShiftX = 0.5;


	Varyings CreateVertex(float3 base, int index) {
		float3 position = bladeVertices[index];
		float2 uv = float2(0, position.y);

		Varyings output;
		position.x *= _Width;
		position.y *= _Height;
		output.position = TransformWorldToHClip(base + position);
		output.uv = uv;
		return output;
	}


	float rand(float3 seed) {
		return frac(sin(dot(seed.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
	}

	float3 GetPositionForBlade(float3 middle, float3 a, float3 b, float3 c, int index) {
		float p1 = rand(middle.xxy + index);
		float p2 = rand(middle.xyz + index) * (1 - p1);
		float p3 = 1 - p1 - p2;

		return middle + float3(a * p1 + b * p2 + c * p3);
	}

	int _BladesPerTriangle;

	[maxvertexcount(MAX_BLADES * VERTICES_PER_BLADE)]
	void geom(triangle Varyings input[3], inout TriangleStream<Varyings> triStream) {
		float3 middle = (input[0].position + input[1].position + input[2].position) / 3;
		float3 a = input[0].position - middle;
		float3 b = input[1].position - middle;
		float3 c = input[2].position - middle;
		
		for (int i = 0; i < _BladesPerTriangle; i++) {
			float3 position = GetPositionForBlade(middle, a, b, c, i);

			for (int j = 0; j < VERTICES_PER_BLADE; j ++) {
				triStream.Append(CreateVertex(position, j));
			}
			triStream.RestartStrip();
		}
	}

	ENDHLSL

		SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

		Cull Off

		Pass
		{
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM

			float4 _TopColor;
			float4 _BottomColor;

			half4 frag(Varyings input) : SV_Target{
				return lerp(_BottomColor, _TopColor, input.uv.y);
			}

			ENDHLSL
		}
	}
}
