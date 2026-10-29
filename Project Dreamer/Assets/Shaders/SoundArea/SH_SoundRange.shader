Shader "Custom/SoundRange"
{
    Properties
    {
        _WaveColor ("Wave Color", Color) = (1,1,1,.8)
        _WaveHeight ("Wave Height", Float) = 10
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,.25)
        _FresnelPower ("Fresnel Power", Float) = 10
        _CameraPositionResponse("CameraSpaceResponse", Float) = 0.2
        _WaveTexture ("Wave Texture", 2D) = "white" {}
        _FrenzelTexture ("Wave Texture", 2D) = "white" {}
    }
    SubShader
    { 
        Tags { "RenderType"="Transparent" "Queue" ="Transparent+1" }
        LOD 100
        Cull Off
        Blend One OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                half4 normal : NORMAL;
                fixed2 uv: TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed2 uv: TEXCOORD0;
                fixed3 norm : NORMAL;
                fixed3 view : COLOR0;
            };

            sampler2D _CameraDepthTexture;
            float _CameraPositionResponse;
            float _FresnelPower;
            fixed4 _FresnelColor;
            sampler2D _FrenzelTexture;
            float _WaveHeight;
            fixed4 _WaveColor;
            sampler2D _WaveTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos= UnityObjectToClipPos(v.vertex);
                o.norm = normalize(mul((float3x3) unity_ObjectToWorld, v.normal));
                float4 worldPosition = mul (unity_ObjectToWorld, v.vertex);
                o.view = normalize(_WorldSpaceCameraPos - worldPosition);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float original_depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.pos.xy / _ScreenParams.xy).x);
                float new_depth = LinearEyeDepth(i.pos.z);
                fixed aligned = abs(dot(normalize(i.norm), normalize(i.view)));
                float depth_diff = aligned * (original_depth - new_depth);
                
                if (depth_diff < 0){
                    discard;
                }
                float2 screenCenteredUV = i.pos.xy / _ScreenParams.xy;
                float2 samplePosition = i.uv + screenCenteredUV * _CameraPositionResponse;

                fixed waveTexture = tex2D(_WaveTexture, samplePosition).x;
                fixed freznelTexture = tex2D(_FrenzelTexture, samplePosition).x;

                float waveHeightMax = _WaveHeight + waveTexture;

                float waveDepth = max((waveHeightMax - depth_diff) / waveHeightMax, 0);

                fixed4 wave = _WaveColor * waveDepth;
                fixed4 freznel = _FresnelColor * pow(1. - aligned, _FresnelPower + freznelTexture);

                return max(wave, freznel);
            }
            ENDCG
        }
    }
}
