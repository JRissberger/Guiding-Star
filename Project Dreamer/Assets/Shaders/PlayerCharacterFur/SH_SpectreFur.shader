Shader "Custom/Spectre Fur"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _SwayFactors ("Sway factor (tangent, bitangent)", Vector, 2) = (1,1,0,0)
        _TurbulanceDisplace ("Turbulance", Float) = .5
        _TurbulanceRate ("Turbulance Rate", Float) = 2
        _WindDir ("Wind Direction", Vector, 3) = (0,0,0,0)
        _WindPower ("Wind Amount", Float) = 2
        _ScrollVelocity ("Scroll Velocity", Float) = 2
        _InvTurbulanceNoiseScale ("Turbulance Scale", Float) = .2
        _FurLength ("Fur length", Float) = 2
        _DefaultTangentDisplace ("Default Tangent Displace", Vector, 2) = (1,0,0,0)
        _GlimmerColor("Glimmer Color", Color) = (0,0,1,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" ="Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            ZWrite On
            Cull Off
            Blend One OneMinusSrcAlpha
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
 // // compile shader into multiple variants, with and without shadows
            // // (we don't care about any lightmaps yet, so skip these variants)
            // #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile_instancing
            // // shadow helper functions and macros
            // #include "AutoLight.cginc"
            // #include "UnityCG.cginc"
            // #include "Lighting.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #include "../CGTypes.hlsl"

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                half3 lambert : COLOR0;
                // half3 ambient : COLOR1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _TurbulanceDisplace;
                float _TurbulanceRate;
                float3 _WindDir;
                float2 _SwayFactors;
                float _WindPower;
                float _FurLength;
                float _InvTurbulanceNoiseScale;
                float2 _DefaultTangentDisplace;
                float _ScrollVelocity;
                fixed4 _GlimmerColor;
            CBUFFER_END

            #include "WindDistort.cginc"

            v2f vert(appdata v)
            {
                v2f OUT;
                OUT.uv = v.texcoord;
                OUT.pos = ProjectWindDistortVertex(v.vertex.xyz, v.texcoord.y, v.normal.xyz, v.tangent.xyz);

                // the lighting model is now using lambertian diffuse, not realistic but
                // it's good enough.
                // Get the VertexNormalInputs of the vertex, which contains the normal in world space
                VertexNormalInputs positions = GetVertexNormalInputs(v.vertex.xyz);

                // Get the properties of the main light
                Light light = GetMainLight();

                // Calculate the amount of light the vertex receives
                OUT.lambert = LightingLambert(light.color, light.direction, positions.normalWS.xyz);
                
                return OUT;
            }

            half4 frag(v2f i) : SV_Target
            {
                fixed2 uvOffset1 = float2(i.uv * .5);
                uvOffset1.y += frac(_ScrollVelocity * -_Time.y);
                fixed2 uvOffset2 = float2(i.uv * .4);
                uvOffset2.y += frac(2.* _ScrollVelocity * -_Time.y);

                fixed noise1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvOffset1).x;
                fixed noise2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvOffset2).x;

                fixed value = (noise1 - noise2) * .8 + .5;
                value -= i.uv.y;
                
                if(value < 0.){
                    discard;
                }
                
                fixed3 col = (1. - smoothstep(.0,.5,noise1)) * _GlimmerColor.xyz;
                
                fixed3 lighting = i.lambert;
                col.rgb *= lighting;
                return fixed4(col, 1.) * smoothstep(.0,.3,value);
            }
            ENDHLSL
        }
    }
}
