Shader "Lit/Diffuse With Shadows"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        // _DisplaceAmount ("Displacement Amount", Float) = .5
        _TurbulanceDisplace ("Turbulance", Float) = .5
        _TurbulanceRate ("Turbulance Rate", Float) = 2
        _WindDir ("WindDirection", Vector, 3) = (0,0,0,0)
        _WindPower ("WindAmount", Float) = 2
        [NoScaleOffset] _DisplaceTex ("Displace", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"
            #include "Perlin3D.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                // float3 tangent: TANGENT;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                half3 diff : COLOR0;
                half3 ambient : COLOR1;
            };

            // inline float3 TransformToTangentSpace(float3 viewDir, float3 normal, float3 tangent)
            // {
            //     return float3(
            //         dot(tangent, viewDir),
            //         dot(cross(normal, tangent), viewDir),
            //         dot(normal, viewDir)
            //     );
            // }

            uniform float _TurbulanceDisplace;
            uniform float _TurbulanceRate; 
            uniform float3 _WindDir;
            uniform float _WindPower;
            sampler2D _MainTex;
            // sampler2D _DisplaceTex;

            v2f vert (appdata_tan v)
            {
                v2f o;

                float4 worldPosition = mul (unity_ObjectToWorld, float4 (v.vertex.xyz, 1.0));
                
                float3 samplePos = worldPosition.xyz - _TurbulanceRate * _WindDir * _Time.y;
                worldPosition.xyz += Perlin3DVec(samplePos) * _TurbulanceDisplace + _WindDir * _WindPower;

                o.pos = mul(unity_MatrixVP, worldPosition);
                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                
                TRANSFER_SHADOW(o);
                return o;
            }

            // uniform float2 _ImageSize;

            // float noiseFunction(float3 position){
            //     return position ; 
            // // }

            // float testSDF(float3 current){
            //     return length(current) - 1.;//max(depth - dot(srcWS - current, normalWS), 0);
            // }

            half4 frag (v2f i) : SV_Target
            {
                float2 scrolledUV = i.uv.xy;
                scrolledUV.y += _Time;
                half4 col = half4(1,1,1,1);
                // compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                half shadow = SHADOW_ATTENUATION(i);
                // darken light's illumination with shadow, keep ambient intact
                half3 lighting = i.diff * shadow + i.ambient;
                col.rgb *= lighting;
                return col;
            }
            ENDHLSL
        }

        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
