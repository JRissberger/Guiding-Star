Shader "Unlit/LightConeVolume"
{
    Properties
    {
        _Density ("Fog Density", Float) = 2
        _Color ("Fog Color", Color) = (1,1,1,0)
        _ConeSourceOS ("Cone Source Object Space", Vector, 3) = (0,0,1,0)
        _ConeExitPlaneOS ("Cone Exit", Float) = 1
        _Theta ("Cone Angle", Float) = 1
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
            // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
            #pragma exclude_renderers gles

            #pragma vertex vert
            #pragma fragment frag
            // // make fog work
            // #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "DepthUtil.cginc"
            #include "../PlayerCharacterFur/Perlin3D.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 view: COLOR0;
            };

            sampler2D _CameraDepthTexture;
            // sampler2D _LastCameraDepthTexture;
            fixed4 _Color;
            float3 _ConeSourceOS;
            float _Density;
            float _ConeExitPlaneOS;
            float _Theta;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.view = mul(unity_ObjectToWorld,v.vertex).xyz - _WorldSpaceCameraPos; 
                return o;
            }

            #define STEP_PERCENT 0.1

            inline float3 intersectCone(float3 rayOrigin, float3 rayDir, float theta){
                float tanTheta = tan(theta);
                float3 rayVecT = rayDir * float3(1,1,tanTheta);
                float3 rayOriginT = rayOrigin * float3(1,1,tanTheta);
                float a = dot(rayVecT * rayVecT, float3(1,1,-1));
                float b = 2. * dot(rayVecT * rayOriginT, float3(1,1,-1));
                float c = dot(rayOriginT * rayOriginT, float3(1,1,-1));
                float det = b * b - 4. * a * c;
                float t1 = (-b - sqrt(det)) / (2. * a);
                float t2 = (-b + sqrt(det)) / (2. * a);
                return float3(t1, t2, det);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 fragWS = i.view + _WorldSpaceCameraPos;
                float3 enterPointOS = mul(unity_WorldToObject, float4(fragWS,1.)).xyz - _ConeSourceOS;
                float3 viewVecOS = normalize(mul((float3x3)unity_WorldToObject,i.view));

                float3 points = intersectCone(enterPointOS, viewVecOS, _Theta);
                if(points.z < 0){
                    discard;
                }
                
                float3 coneEnterOS = enterPointOS + viewVecOS * points.x;
                float3 coneExitOS = enterPointOS + viewVecOS * points.y;
                float planeTravelDistance = (_ConeExitPlaneOS - enterPointOS.z) / viewVecOS.z;
                float3 planeIntersectOS = enterPointOS + viewVecOS * planeTravelDistance; 

                bool exitInRange  = (coneExitOS.   z >= 0 && coneExitOS.   z <= _ConeExitPlaneOS);
                bool enterInRange = (coneEnterOS.  z >= 0 && coneEnterOS.  z <= _ConeExitPlaneOS);
                
                if(!enterInRange && !exitInRange){
                    discard;
                }
                if(!enterInRange){
                    coneEnterOS = planeIntersectOS;
                }
                if(!exitInRange){
                    coneExitOS = planeIntersectOS;
                }

                float3 enterPoint = mul(unity_ObjectToWorld, float4(coneEnterOS + _ConeSourceOS,1.));
                float3 exitPoint = mul(unity_ObjectToWorld, float4(coneExitOS + _ConeSourceOS, 1.));
                float3 baseWorldPoint = depthToWS(i.vertex.xy, _CameraDepthTexture);

                float exitPointDepth  = dot(unity_WorldToCamera._m20_m21_m22_m23, float4(exitPoint,      1.)); 
                float enterPointDepth = dot(unity_WorldToCamera._m20_m21_m22_m23, float4(enterPoint,     1.)); 
                float worldPointDepth = dot(unity_WorldToCamera._m20_m21_m22_m23, float4(baseWorldPoint, 1.)); 

                if(enterPointDepth > worldPointDepth){
                    discard;
                }
                if(exitPointDepth > worldPointDepth){
                    exitPoint = baseWorldPoint;
                }

                fixed4 sum = fixed4(0,0,0,0);

                float totalTravel = length(enterPoint - exitPoint);
                
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
