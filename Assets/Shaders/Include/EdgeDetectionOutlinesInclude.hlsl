#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

static float2 sobelSamplePoints[9] = {
	float2(-1, 1), float2(0, 1), float2(1, 1),
	float2(-1, 0), float2(0, 0), float2(1, 1),
	float2(-1, -1), float2(0, -1), float2(1, -1)
};

static float sobelMatrixX[9] = {
	1, 0, -1,
	2, 0, -2,
	1, 0, -1
};

static float sobelMatrixY[9] = {
	 1,  2,  1,
	 0,  0,  0,
	-1, -2, -1
};

void DepthSobel_float(float2 UV, float Thickness, out float Out) {
	float2 sobel = 0;

	[unroll] for (int i = 0; i < 9; ++i) {
		float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
		sobel += depth * float2(sobelMatrixX[i], sobelMatrixY[i]);
	}

	Out = length(sobel);
}

void ColorSobel_float(float2 UV, float Thickness, out float Out) {
	float2 sobelR = 0;
	float2 sobelG = 0;
	float2 sobelB = 0;

	[unroll] for (int i = 0; i < 9; ++i) {
		float3 color = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV + sobelSamplePoints[i] * Thickness);
		float2 sobel = float2(sobelMatrixX[i], sobelMatrixY[i]);

		sobelR += color.r * sobel;
		sobelG += color.g * sobel;
		sobelB += color.b * sobel;
	}

	Out = max(length(sobelR), max(length(sobelG), length(sobelB)));
}

#endif