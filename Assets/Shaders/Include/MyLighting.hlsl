
void GetMyLight_half(float3 WorldPos, out float3 Direction, out float3 Color, out half DistanceAttenuation,
	out half ShadowAttenuation) {
	#ifdef SHADERGRAPH_PREVIEW
		Direction = half3(0.5, 0.5, 0);
		Color = 1;
		DistanceAttenuation = 1;
		ShadowAttenuation = 1;
	#else
		#if SHADOWS_SCREEN
			half4 clipPos = TransformWorldToHClip(WorldPos);
			half4 shadowCoord = ComputeScreenPos(clipPos);
		#else
			half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
		#endif
		Light light = GetMainLight();
		Direction = light.direction;
		Color = light.color;
		DistanceAttenuation = light.distanceAttenuation;

		#if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
			ShadowAttenuation = 1;
		#endif	

		#if SHADOWS_SCREEN
			ShadowAttenuation = SampleScreenSpaceShadowmap(shadowCoord);
		#else
			ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
			half shadowStrength = GetMainLightShadowStrength();
			ShadowAttenuation = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
				sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);
		#endif
	#endif
}

void GetMySpecular_half(half3 Specular, half Smoothness, half3 Direction, half3 Color, half3 WorldNormal, half3 WorldView, out half3 Out) {
	#ifdef SHADERGRAPH_PREVIEW
		Out = 0;
	#else
		Smoothness = exp2(10 * Smoothness + 1);
		WorldNormal = normalize(WorldNormal);
		WorldView = normalize(WorldView);
		Out = LightingSpecular(Color, Direction, WorldNormal, WorldView, half4(Specular, 0), Smoothness);
	#endif
}