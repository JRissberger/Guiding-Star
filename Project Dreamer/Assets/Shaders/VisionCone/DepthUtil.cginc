#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

inline float3 depthToWS(float2 vertex){
    float2 UV = vertex / _ScaledScreenParams.xy;

    #if UNITY_REVERSED_Z
        float depth = SampleSceneDepth(UV);
    #else
        // Adjust z to match NDC for OpenGL
        float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(UV));
    #endif

    return ComputeWorldSpacePosition(UV, depth, UNITY_MATRIX_I_VP);
}
