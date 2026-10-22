Shader "Unlit/Spectre Fresnel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FresnelColor ("Fresnel Color", Color) = (0,0,0,0)
        _FresnelPower ("Fresnel Power", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 view: COLOR0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _FresnelColor;
            float _FresnelPower;

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
                fixed4 col = 
                    tex2D(_MainTex, i.uv) * 
                    _FresnelColor * 
                    pow((1. - dot(i.normal, i.view)), _FresnelPower);
                return col;
            }
            ENDCG
        }
    }
}
