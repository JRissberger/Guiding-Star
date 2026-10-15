#include "Perlin3D.cginc"

uniform float _TurbulanceDisplace;
uniform float _TurbulanceRate; 
uniform float3 _WindDir;
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
inline float4 ProjectWindDistortVertex(float3 vertex, float height){
    float4 worldPosition = mul (unity_ObjectToWorld, float4 (vertex, 1.0));
    float3 worldNormal = UnityObjectToWorldNormal(float3(0,0,1));
    
    float relaxedLength = _FurLength * height;
    float influence = max(height,0);
    influence *= influence;

    float3 originWS = worldPosition.xyz;

    worldPosition.xyz += worldNormal * relaxedLength;

    float3 samplePos = (worldPosition.xyz - _TurbulanceRate * _WindDir * _Time.y) * _InvTurbulanceNoiseScale;
    worldPosition.xyz += influence * (Perlin3DVec(samplePos) * _TurbulanceDisplace + _WindDir * _WindPower);
    
    worldPosition.xyz -= originWS;
    
    worldPosition.xyz = SafeNormalize(worldPosition.xyz); 
    worldPosition.xyz *= relaxedLength;
    worldPosition.xyz += originWS;

    return mul(unity_MatrixVP, worldPosition);
}