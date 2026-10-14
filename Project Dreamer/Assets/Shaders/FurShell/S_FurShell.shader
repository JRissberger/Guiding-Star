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
            #include "WindDistort.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                half3 diff : COLOR0;
                half3 ambient : COLOR1;
            };

            sampler2D _MainTex;
            
            v2f vert (appdata_tan v)
            {
                v2f o;
                o.pos = ProjectWindDistortVertex(v.vertex.xyz, 1);

                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                
                TRANSFER_SHADOW(o);
                return o;
            }

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

        // shadow caster rendering pass, implemented manually to allow for
        // deformation shadowcasting mesh's shape
        // using macros from UnityCG.cginc
        Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            #include "ManualShadows.cginc"
            #include "WindDistort.cginc"

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = ProjectWindDistortVertex(v.vertex.xyz, 1);

		        float3 wNormal = UnityObjectToWorldNormal (v.normal);
                // technically, this is wrong, since we are using model source normals
                // to compute the normal bias, however, unless you have enough turbulance
                // to kill a person, nobody else noticed.
                o.pos = ApplyShadowNormalBias(o.pos,wNormal);
                o.pos = UnityApplyLinearShadowBias(o.pos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDHLSL
        }
    }
}
