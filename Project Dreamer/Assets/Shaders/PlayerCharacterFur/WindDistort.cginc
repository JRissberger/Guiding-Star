#include "Perlin3D.cginc"

//copy this to the top of the file or it won't work
// float _TurbulanceDisplace;
// float _TurbulanceRate; 
// float3 _WindDir;
// float2 _SwayFactors;
// float _WindPower;
// float _FurLength;
// float _InvTurbulanceNoiseScale;
// float2 _DefaultTangentDisplace;

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

    float3 samplePos = ((worldPosition.xyz + worldNormal * relaxedLength) - _TurbulanceRate * _WindDir * _Time.y) * _InvTurbulanceNoiseScale;
    float3 offsetPos = influence * (Perlin3DVec(samplePos) * _TurbulanceDisplace + _WindDir * _WindPower);

    float tangentDisplace = dot(offsetPos, worldTangent) / dot(worldTangent, worldTangent);
    float bitangentDisplace = dot(offsetPos, worldBiTangent) / dot(worldBiTangent, worldBiTangent);

    offsetPos = 
        worldNormal * relaxedLength +
        worldTangent * (_DefaultTangentDisplace.x + _SwayFactors.x * tangentDisplace) + 
        worldBiTangent * (_DefaultTangentDisplace.y + _SwayFactors.y * bitangentDisplace);
    
    worldPosition.xyz += SafeNormalize(offsetPos) * relaxedLength; 

    return mul(unity_MatrixVP, worldPosition);
}