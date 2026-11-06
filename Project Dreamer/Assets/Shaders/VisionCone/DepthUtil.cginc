
inline float3 depthToWS(float2 vertex, sampler2D depthTex){
    float3 ndc;
    ndc.z = DecodeFloatRG(tex2D(depthTex, vertex / _ScreenParams.xy).xy);
    
    ndc.xy = vertex / _ScreenParams.xy;
    ndc.xy = ndc.xy * 2. - 1.;
    return ndc;

    float4 baseWS = mul(unity_CameraInvProjection,float4(ndc, 1.));
    baseWS.xyz /= baseWS.w;
    return mul(unity_MatrixInvV, float4(baseWS.xyz, 1.)).xyz;
}
