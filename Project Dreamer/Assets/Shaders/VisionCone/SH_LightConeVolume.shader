Shader "Unlit/LightConeVolume"
{
    Properties
    {
        _Radius ("Radius", Float) = 1.0
        _Density ("Fog Density", Float) = 2
        _Color ("Fog Color", Color) = (1,1,1,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" ="Transparent" }
        LOD 100
        Cull Back
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "../PlayerCharacterFur/Perlin3D.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                // float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                // float2 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed3 view: COLOR0;
                float distance: COLOR1;
            };

            // sampler2D _MainTex;
            float _Density;
            fixed4 _Color;
            float4 _MainTex_ST;
            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.view = normalize(mul(unity_ObjectToWorld,v.vertex).xyz - _WorldSpaceCameraPos); 
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            #define STEP_PERCENT 0.125

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                fixed4 sum = fixed4(0,0,0,0);

                // float4 center = ;
                // float4 camera = float4(0,0,0,0);

                float3 o_minus_c = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz - _WorldSpaceCameraPos;

                float b = (dot(normalize(i.view), o_minus_c));
                float c = dot(o_minus_c, o_minus_c) - _Radius * _Radius;

                float3 enterPoint = (-b + sqrt(b * b - c)) * normalize(i.view) - _WorldSpaceCameraPos;
                float3 exitPoint = (-b - sqrt(b * b - c)) * normalize(i.view) - _WorldSpaceCameraPos;

                if(b * b - c <= 0){
                    discard;
                }

                float totalTravel = 2 * sqrt(b * b - c);
                
                for (float i = 0; i < 1.; i += STEP_PERCENT){
                    float3 currentPoint = lerp(enterPoint, exitPoint, i);
                    float noise = Perlin3Dln(currentPoint * .55) + Perlin3Dln(currentPoint * 1.2) + Perlin3Dln(currentPoint * 3.1415926);
                    float lerpFac = exp(-_Density * max(noise * .333 - .2, 0.) * totalTravel * STEP_PERCENT);
                    sum.xyz = lerp(_Color.xyz,sum.xyz,lerpFac);
                    sum.w = lerp(1, sum.w, lerpFac);
                }
                
                return sum;
            }
            ENDCG
        }
    }
}
