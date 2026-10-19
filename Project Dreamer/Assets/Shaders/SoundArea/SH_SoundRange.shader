Shader "Custom/SoundRange"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _WaveColor ("Wave Color", Color) = (1,1,1,.8)
        _WaveHeight ("Wave Height", Float) = 10
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,.25)
        _FresnelPower ("Fresnel Power", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+20" }
        LOD 100
        Cull Off
        Blend One OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                half4 normal : NORMAL;
                // float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed3 norm : NORMAL;
                fixed3 view : COLOR0;
                // UNITY_FOG_COORDS(1)
                // float2 uv : TEXCOORD0;
            };

            sampler2D _CameraDepthTexture;
            float _FresnelPower;
            fixed4 _FresnelColor;
            float _WaveHeight;
            fixed4 _WaveColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos= UnityObjectToClipPos(v.vertex);
                o.norm = normalize(mul((float3x3) unity_ObjectToWorld, v.normal));
                float4 worldPosition = mul (unity_ObjectToWorld, v.vertex);
                o.view = normalize(_WorldSpaceCameraPos - worldPosition);
                // // o.uv = v.uv;
                // o.pos = UnityObjectToClipPos(v.vertex);
                // o.norm = normalize(mul((float3x3)unity_MatrixMVP, v.normal));
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float original_depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.pos.xy / _ScreenParams.xy).x);
                float new_depth = LinearEyeDepth(i.pos.z);
                fixed aligned = abs(dot(normalize(i.norm), normalize(i.view)));
                float depth_diff = aligned * (original_depth - new_depth);
                
                float waveDepth = max((_WaveHeight - depth_diff) / _WaveHeight, 0);

                fixed4 wave = _WaveColor * waveDepth;

                fixed4 freznel = _FresnelColor * pow(1. - aligned, _FresnelPower);
                return max(wave, freznel);
            }
            ENDCG
        }
    }
}
