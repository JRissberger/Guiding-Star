// the mighto'kondreyeah(mitochondria but I spelled it wrong.) is the 
// powerhouse of the cell.

// the comment above is a silly thing I wrote because one of my friends 
// said it.

// personally I'm doing it to see if anyone actually reads this.
// please appreciate my code. :3 thx! -Stephanie Y.

Shader "Unlit/Spectre Fresnel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FresnelTint ("Fresnel Color", Color) = (0,0,0,0)
        _FresnelPower ("Fresnel Power", Float) = 2

        // This is the way unity allows enabling/disabling shader variants.
        [Toggle(_FresnelOn)] _FresnelOn ("Fresnel", Float) = 1
        // this needs a comment since unity *did not* document it *PROPERLY*
        // graphics docs can sometimes be lacking, unity is like really bad
        // about this.

        // also, these are effectively booleans, but unity prefers to say they
        // are floats anyways because why not.
        [Toggle(_DitherFade)] _DitherFade ("Dithered Fading", Float) = 1 
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #pragma shader_feature_local_fragment _ _FresnelOn
            #pragma shader_feature_local_fragment _ _DitherFade 

            #include "UnityCG.cginc"
            #include "Bayers.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : NORMAL;
                half3 view: COLOR0;
            };

            sampler2D _MainTex;
            
            #if defined(_FresnelOn)
            half4 _FresnelTint;
            float _FresnelPower;
            #endif

            #if defined(_DitherFade)
            half _Alpha;
            #endif

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(mul((float3x3) unity_ObjectToWorld, v.normal));
                float4 worldPosition = mul (unity_ObjectToWorld, v.vertex);
                o.view = normalize(_WorldSpaceCameraPos - worldPosition);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 albedo = tex2D(_MainTex, i.uv); 
                fixed4 light = half4(0,0,0,0);

                #if defined(_FresnelOn)
                light += _FresnelTint * pow((1. - dot(i.normal, i.view)), _FresnelPower);
                #endif

                #if defined(_DitherFade)
                float2 pixelPosition = frac(i.vertex.xy * .25);
                half ditherOffset = bayer4x4(pixelPosition); 
                if (_Alpha < ditherOffset){
                    discard;
                }
                #endif
                
                return albedo * light;
            }
            ENDHLSL
        }
    }
}
