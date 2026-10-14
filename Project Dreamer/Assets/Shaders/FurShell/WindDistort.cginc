#include "Perlin3D.cginc"

uniform float _TurbulanceDisplace;
uniform float _TurbulanceRate; 
uniform float3 _WindDir;
uniform float _WindPower;

inline float4 ProjectWindDistortVertex(float3 vertex,float influence){
    float4 worldPosition = mul (unity_ObjectToWorld, float4 (vertex, 1.0));
    float3 samplePos = worldPosition.xyz - _TurbulanceRate * _WindDir * _Time.y;
    worldPosition.xyz += Perlin3DVec(samplePos) * _TurbulanceDisplace + _WindDir * _WindPower;
    return mul(unity_MatrixVP, worldPosition);
}