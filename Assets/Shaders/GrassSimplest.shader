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

	Varyings CreateBladeVertex(float3 basePosition, int index, float rotation, float tilt, float3 windAxis, float3 windStrength) {
		float3 position = bladeVertices[index];
		float2 uv = float2(position.x + uvShiftX, position.y);

		position.x *= _Width;
		position.y *= _Height;
		position.z *= _Width;

		float3x3 tiltM = RotationFromAxisAngle(float3(1,0,0), sin(tilt * uv.y), cos(tilt * uv.y));
		float3x3 rotationM = RotationFromAxisAngle(float3(0,1,0), sin(rotation), cos(rotation));

		//position = Rotate(float3(0,0,0), position, float3(1,0,0), tilt * uv.y);
		//position = Rotate(float3(0,0,0), position, float3(0,1,0), rotation);
		//position = Rotate(float3(0,0,0), position, windAxis, windStrength * uv.y);

		position = mul(mul(rotationM, tiltM), position);

		Varyings output;
		output.position = TransformWorldToHClip(basePosition + position);
		output.uv = uv;
		return output;
	}

	Varyings CreateVertex(float3 positionWS, float2 uv) {
		Varyings output;
		output.position = TransformWorldToHClip(positionWS);
		output.uv = uv;
		return output;
	}

	struct SegmentInfo {
		float3 position;
		float3x3 orientation;
		float positionOSy;
	};

	SegmentInfo segment;

	float degToRad(float deg) {
		return ((2 * 3.14159265) / 360) * deg;
	}

	struct VertexInfo {
		float3 position;
		float2 uv;
		float3x3 rotationOS;
	};

	

	float _WindStrength;
	float _WindSpeed;

	VertexInfo ProcessVertex(float3 position, float tilt, float3 windDir, float3 windStrength) {
		float2 uv = float2(position.x + uvShiftX, position.y);

		position.y -= segment.positionOSy;

		position.x *= _Width;
		position.y *= _Height;

		float3x3 tiltRotation = RotationFromAxisAngle(float3(1,0,0), sin(tilt), cos(tilt));
		float3 positionWS = mul(mul(segment.orientation, tiltRotation), position);
		// place for potential limiter

		float3 windAxis = cross(windDir, positionWS);
		
		//windAxis = windAxis / length(windAxis);
		//float3 windAxis = windDir;

		//float3x3 windRotation = RotationFromAxisAngle(windAxis, sin(windStrength), cos(windStrength));
		position = mul(mul(segment.orientation, tiltRotation), position);

		position += segment.position;

		VertexInfo output;
		output.position = position;
		output.uv = uv;
		//output.rotationOS = mul(windRotation, tiltRotation);
		output.rotationOS = tiltRotation;

		return output;
	}

	void CreateGrassSegment(int indexFrom, int indexTo, float tilt, inout TriangleStream<Varyings> triStream, float3 windDir, float windStrength) {
		for (int i = indexFrom; i <= indexTo; i++) {
			VertexInfo vertex = ProcessVertex(bladeVertices[i], tilt, windDir, windStrength);
			triStream.Append(CreateVertex(vertex.position, vertex.uv));
		}

		SegmentInfo newInfo;

		float3 position = (bladeVertices[indexFrom] + bladeVertices[indexTo]) / 2;
		newInfo.positionOSy = position.y;
		VertexInfo vertex = ProcessVertex(position, tilt, windDir, windStrength);

		newInfo.position = vertex.position;
		newInfo.orientation = mul(segment.orientation, vertex.rotationOS);
		
		segment = newInfo;
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
	float _TiltModifier;

	[maxvertexcount(MAX_BLADES * VERTICES_PER_BLADE)]
	void geom(triangle Varyings input[3], inout TriangleStream<Varyings> triStream) {
		float3 middle = (input[0].position + input[1].position + input[2].position) / 3;
		float3 a = input[0].position - middle;
		float3 b = input[1].position - middle;
		float3 c = input[2].position - middle;
		
		for (int i = 0; i < _BladesPerTriangle; i++) {
			float3 position = GetPositionForBlade(middle, a, b, c, i);
			float tilt = (rand(position.xyz) * 2 - 1) * degToRad(30) * _TiltModifier;
			float rotation = (rand(position.yzz) * 2 - 1) * degToRad(90);

			//float2 windUV = position.xz * _WindTexture_ST.xy + _WindTexture_ST.zw + float2(_WindSpeed * _Time.y, _WindSpeed * _Time.y);
			//float2 windUV = TRANSFORM_TEX(position.xz, _WindTexture) + float2(_WindSpeed * _Time.y, _WindSpeed * _Time.y);
			//float2 windUV = position.xz + float2(_WindSpeed * _Time.y, _WindSpeed * _Time.y);
			float4 windSample = SAMPLE_TEXTURE2D_LOD(_WindTexture, sampler_WindTexture, position.xz + float2(_Time.y, _Time.y), 0) * 2 - 1;
			//float4 windSample = float4(0,0,0,0);
			//windSample = float2(1, 1);
			float3 windDir = float3(windSample.x, 0, windSample.y) * _WindStrength;
			//float3 windDir = float3(1, 0, 1) * _WindStrength;
			float windStrength = 0;//length(windDir);

			//segment.position = position + windDir;
			//segment.orientation = RotationFromAxisAngle(float3(0,1,0), sin(rotation), cos(rotation));
			//segment.positionOSy = 0;

			for (int j = 0; j < VERTICES_PER_BLADE - 2; j += 2) {
				//CreateGrassSegment(j, j + 1, tilt, triStream, windDir, windStrength);
				triStream.Append(CreateVertex(position + bladeVertices[j] + windDir, float2(0, bladeVertices[j].y)));
			}
			//CreateGrassSegment(VERTICES_PER_BLADE - 1, VERTICES_PER_BLADE - 1, tilt, triStream, windDir, windStrength);

			/*float3 windAxis = float3(0.7, 0, -0.5);
			float windStrength = length(windAxis) * rand(position.xzy) * (sin(_Time * _WindSpeed) + 1) * _WindStrength;

			for (int j = 0; j < VERTICES_PER_BLADE; j++) {
				triStream.Append(CreateBladeVertex(position, j, rotation, tilt, windAxis, windStrength));
			}*/
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
