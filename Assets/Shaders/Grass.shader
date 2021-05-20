Shader "Custom/Grass"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (0,0,0,1)
		_BladesPerTriangle("Blades per Triangle", Float) = 5
		_Height("Height", Float) = 0.5
		_Width("Width", Float) = 0.5
		 
	}

	HLSLINCLUDE

	#define MAX_BLADES 20
	#define VERTICES_PER_BLADE 5

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

	Varyings vert(Attributes input) {
		Varyings output;
		output.position = float4(TransformObjectToWorld(input.positionOS.xyz), 1);
		
		return output;
	}

	/*Varyings getpos(float4 middle, float4 offset) {
		Varyings output;
		float4 tmp = TransformWorldToHClip(middle + offset);

		#if UNITY_REVERSED_Z
		tmp.z = min(tmp.z, UNITY_NEAR_CLIP_VALUE);
		#else
		tmp.z = max(tmp.z, UNITY_NEAR_CLIP_VALUE);
		#endif


		output.position = tmp;

		return output;
	}*/

	float _Height;
	float _Width;

	static const float3 bladeVertices[] = {
			float3(0.15, 0, 0),
			float3(-0.15, 0, 0),
			float3(0.2, 0.3, 0),
			float3(-0.2, 0.3, 0),
			float3(0, 1, 0)
	};
	static const float uvShiftX = 0.5;

	Varyings CreateBladeVertex(float3 basePosition, int index, float rotation, float tilt) {
		float3 position = bladeVertices[index];
		float2 uv = float2(position.x + uvShiftX, position.y);

		position.x *= _Width;
		position.y *= _Height;
		position.z *= _Width;

		position = Rotate(float3(0,0,0), position, float3(1,0,0), tilt);
		position = Rotate(float3(0,0,0), position, float3(0,1,0), rotation);

		Varyings output;
		output.position = TransformWorldToHClip(basePosition + position);
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

		/*float3x4 permutations[6];
		permutations[0] = float3x4(a, b, c);
		permutations[1] = float3x4(a, c, b);
		permutations[2] = float3x4(b, a, c);
		permutations[3] = float3x4(b, c, b);
		permutations[4] = float3x4(c, a, b);
		permutations[5] = float3x4(c, b, a);


		int index = (int)round(rand(middle.xzz) * 5);
		float3x4 perm = permutations[index];*/

		return middle + float3(a * p1 + b * p2 + c * p3);
	}

	int _BladesPerTriangle;

	float degToRad(float deg) {
		return ((2 * 3.14159265) / 360) * deg;
	}

	[maxvertexcount(MAX_BLADES * VERTICES_PER_BLADE)]
	//[maxvertexcount(30)]
	void geom(triangle Varyings input[3], inout TriangleStream<Varyings> triStream) {
		float3 middle = (input[0].position + input[1].position + input[2].position) / 3;
		float3 a = input[0].position - middle;
		float3 b = input[1].position - middle;
		float3 c = input[2].position - middle;
		
		for (int i = 0; i < _BladesPerTriangle; i++) {
			float3 position = GetPositionForBlade(middle, a, b, c, i);
			float tilt = (rand(position.xyz) * 2 - 1) * degToRad(30);
			float rotation = (rand(position.yzz) * 2 - 1) * degToRad(90);

			for (int j = 0; j < VERTICES_PER_BLADE; j++) {
				float tiltStrength = floor((float)j / 2) / floor(VERTICES_PER_BLADE / 2);
				triStream.Append(CreateBladeVertex(position, j, rotation, tilt * tiltStrength));
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
