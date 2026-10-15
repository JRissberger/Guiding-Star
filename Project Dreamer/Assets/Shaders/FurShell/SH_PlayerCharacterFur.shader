Shader "Lit/Diffuse With Shadows"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        // _DisplaceAmount ("Displacement Amount", Float) = .5
        _TurbulanceDisplace ("Turbulance", Float) = .5
        _TurbulanceRate ("Turbulance Rate", Float) = 2
        _WindDir ("Wind Direction", Vector, 3) = (0,0,0,0)
        _WindPower ("Wind Amount", Float) = 2
        _ScrollVelocity ("Scroll Velocity", Float) = 2
        _InvTurbulanceNoiseScale ("Turbulance Scale", Float) = .2
        _FurLength ("Fur length", Float) = 2
        // [NoScaleOffset] _DisplaceTex ("Displace", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            Cull Off
            //Blend One OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile_instancing
            // shadow helper functions and macros
            #include "AutoLight.cginc"
            #include "WindDistort.cginc"

            struct appdata {
                float4 vertex : POSITION;
                // float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                half3 diff : COLOR0;
                half3 ambient : COLOR1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            uniform float _ScrollVelocity;

            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.uv = v.texcoord;
                //FIXME: replace this with another piece of input data.
                // float r = max(v.vertex.y, 0);
                o.pos = ProjectWindDistortVertex(v.vertex.xyz, v.texcoord.y);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                
                TRANSFER_SHADOW(o);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float2 scrolledUV;
                half value;
                scrolledUV = i.uv.xy * 2.;
                scrolledUV.y += frac(_ScrollVelocity * -_Time.y);
                value = tex2D(_MainTex, scrolledUV).x;
                
                scrolledUV = i.uv.xy;
                scrolledUV.x += .2;
                scrolledUV.y += frac(_ScrollVelocity * -_Time.y);
                value -= tex2D(_MainTex, scrolledUV).x;

                value = value * .8 + .5;
                value -= i.uv.y;
                // value +=
                if(value < 0.){
                    discard;
                }
                
                half4 col = half4(0,0,0,0);
                // compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                half shadow = SHADOW_ATTENUATION(i);
                // darken light's illumination with shadow, keep ambient intact
                half3 lighting = i.diff * shadow + i.ambient;
                col.rgb *= lighting;
                return col;
            }
            ENDHLSL
        }

        // shadow caster rendering pass, implemented manually to allow for
        // deformation shadowcasting mesh's shape
        // using macros from UnityCG.cginc
        // Pass
        // {
        //     Tags {"LightMode"="ShadowCaster"}
        //     Cull Off

        //     HLSLPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment frag
        //     #pragma multi_compile_shadowcaster
        //     #pragma multi_compile_instancing
        //     #include "UnityCG.cginc"
        //     #include "ManualShadows.cginc"
        //     #include "WindDistort.cginc"
        
        //     UNITY_INSTANCING_BUFFER_START(Props)
        //         // put more per-instance properties here
        //     UNITY_INSTANCING_BUFFER_END(Props)
            
        //     struct appdata {
        //         float4 vertex : POSITION;
        //         // float4 tangent : TANGENT;
        //         float3 normal : NORMAL;
        //         float4 texcoord : TEXCOORD0;
        //         UNITY_VERTEX_INPUT_INSTANCE_ID
        //     };

        //     struct v2f { 
        //         float2 uv: TEXCOORD0;
        //         V2F_SHADOW_CASTER;
        //         UNITY_VERTEX_INPUT_INSTANCE_ID
        //     };

        //     sampler2D _MainTex;
        //     uniform float _ScrollVelocity;

        //     v2f vert(appdata v)
        //     {
        //         v2f o;
        //         UNITY_SETUP_INSTANCE_ID(v);
        //         UNITY_TRANSFER_INSTANCE_ID(v, o);
        //         o.pos = ProjectWindDistortVertex(v.vertex.xyz, v.texcoord.y);
        //         o.uv = v.texcoord;

		//         float3 wNormal = UnityObjectToWorldNormal (v.normal);
        //         // technically, this is wrong, since we are using model source normals
        //         // to compute the normal bias, however, unless you have enough turbulance
        //         // to kill a person, nobody else noticed.
        //         o.pos = ApplyShadowNormalBias(o.pos,wNormal);
        //         o.pos = UnityApplyLinearShadowBias(o.pos);
        //         return o;
        //     }

        //     float4 frag(v2f i) : SV_Target
        //     {
        //         UNITY_SETUP_INSTANCE_ID(i);
        //         float2 scrolledUV;
        //         half value;
        //         scrolledUV = i.uv.xy * 2.;
        //         scrolledUV.y += frac(_ScrollVelocity * -_Time.y);
        //         value = tex2D(_MainTex, scrolledUV).x;
                
        //         scrolledUV = i.uv.xy;
        //         scrolledUV.x += .2;
        //         scrolledUV.y += frac(_ScrollVelocity * -_Time.y);
        //         value -= tex2D(_MainTex, scrolledUV).x;

        //         value = value * .8 + .5;
        //         value -= i.uv.y;
        //         // value +=
        //         if(value < 0.){
        //             discard;
        //         }
        //         SHADOW_CASTER_FRAGMENT(i)
        //     }
        //     ENDHLSL
        // }
    }
}
