inline float4 ApplyShadowNormalBias(float4 vertexWS, float3 normalWS){
	if (unity_LightShadowBias . z != 0.0)
	{
		float3 wLight = normalize (UnityWorldSpaceLightDir (vertexWS.xyz));
		float shadowCos = dot (normalWS, wLight);
		float shadowSine = sqrt (1 - shadowCos * shadowCos);
		float normalBias = unity_LightShadowBias . z * shadowSine;
		vertexWS.xyz -= normalWS * normalBias;
	}
    return vertexWS;
}