#include "Perlin3D.cginc"

uniform float _TurbulanceDisplace;
uniform float _TurbulanceRate; 
uniform float3 _WindDir;
uniform float2 _SwayFactors;
uniform float _WindPower;
uniform float _FurLength;
uniform float _InvTurbulanceNoiseScale;

//todo: change normalization const based on 
// #pragma multi_compile QUALITY_LOW QUALITY_MED QUALITY_HIGH
inline half3 SafeNormalize(half3 inVec){
	float dp3 = max (0.005, dot (inVec, inVec));
	return inVec * rsqrt (dp3);
}

// performs full transformation of a vertex based on wind
inline float4 ProjectWindDistortVertex(float3 vertex, float height, float3 normOS, float3 tangentOS){
    float4 worldPosition = mul (unity_ObjectToWorld, float4 (vertex, 1.0));

    float3 worldNormal = SafeNormalize(mul((float3x3) unity_ObjectToWorld, normOS));
    float3 worldTangent = SafeNormalize(mul((float3x3) unity_ObjectToWorld, tangentOS));
    //pride flag, wait no I'm getting distracted by something that I thought was
    //funnier than it was, guess you could say I went on a *tangent* haha ._.
    float3 worldBiTangent = cross(worldNormal, worldTangent);

    float relaxedLength = _FurLength * height;
    float influence = max(height,0);
    influence *= influence;

    // float3 originWS = worldPosition.xyz;

    float3 samplePos = ((worldPosition.xyz + worldNormal * relaxedLength) - _TurbulanceRate * _WindDir * _Time.y) * _InvTurbulanceNoiseScale;
    float3 offsetPos = influence * (Perlin3DVec(samplePos) * _TurbulanceDisplace + _WindDir * _WindPower);

    offsetPos = 
        _FurLength * worldNormal * height +
        _SwayFactors.x * worldTangent * dot(offsetPos, worldTangent) / dot(worldTangent, worldTangent) + 
        _SwayFactors.y * worldBiTangent * dot(offsetPos, worldBiTangent) / dot(worldBiTangent, worldBiTangent);
    
    worldPosition.xyz += SafeNormalize(offsetPos) * relaxedLength; 
    // worldPosition.xyz ;

    return mul(unity_MatrixVP, worldPosition);
}