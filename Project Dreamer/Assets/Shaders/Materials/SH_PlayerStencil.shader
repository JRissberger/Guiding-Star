Shader "Custom/PlayerStencil"
{
    Properties
    {
        _MainTex ("noiseTexture", 2D) = "white" {}
        _ViewSpaceNoiseScale ("Noise Scale (view space)", Float) = .02 
        _UVSpaceNoiseScale ("Noise Scale (uv space)", Float) = .8
        _StencilUVRadius ("Radius Min,Max", Vector, 2) = (0.5,0.9,0,0)
        _ScrollRates ("Scroll Rate, view space, model space", Vector, 2) = (.8,1,0,0)
    }
    SubShader
    {
        // render directly after geometry queue, before effect that checks stencil
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
        LOD 100

        Pass
        {  
            // draw 1 to stencil buffer, but never draw to actual screen
            Stencil
            {
                Ref 1
                Comp Never
                Fail Replace
            }            

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 viewuv: TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _UVSpaceNoiseScale;
            float _ViewSpaceNoiseScale;
            float2 _ScrollRates;
            float2 _StencilUVRadius;

            v2f vert (appdata v)
            {
                v2f o;

                //section here rotates the points to make a default plane mesh billboard
                //towards the camera, regardless of in world rotation
                float3 worldRotationVertex = v.vertex.xzy;
                
                //last column of the transform matrix (translation), apply other transforms to only this.
                float4 worldCoord = float4(
                    unity_ObjectToWorld._m03,
                    unity_ObjectToWorld._m13,
                    unity_ObjectToWorld._m23,
                    1
                );
                //split up operation here, since I want to use the worldpos
                float4 view = mul(unity_MatrixV, worldCoord) + float4(worldRotationVertex, 0);
                o.vertex = mul(unity_CameraProjection, view);

                //macro which transforms uv coords from original model to offset
                //accounting for whatever in-engine transforms were applied to texture
                //if I understand correctly, unity is weird and this is a note mostly
                //for myself to remember what each macro means.
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                //preemptively transform the viewspace positions to the view based UV coords
                o.viewuv = view.xy * _ViewSpaceNoiseScale + float2(0,frac(-_Time.x * _ScrollRates.x));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // [0, 1] difference between the texture sampled in world space
                // and texture sampled in uv space
                fixed col = (
                    tex2D(_MainTex, i.viewuv).x - 
                    tex2D(_MainTex, i.uv * _UVSpaceNoiseScale + frac(float2(0,_Time.x) * _ScrollRates.y)).x
                ) * .5 + .5;

                fixed threashold = clamp(
                    (length(i.uv * 2. - 1.) - _StencilUVRadius.x) / 
                    (_StencilUVRadius.y - _StencilUVRadius.x),
                    0, 1
                );
                
                // discard fragment, this will prevent this from ending up on the 
                // stencil buffer if the difference in normalized [0, 1] range is 
                // less than threashold.
                if(col < threashold){
                    discard;
                }

                // need to return something, this will never get drawn.
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
