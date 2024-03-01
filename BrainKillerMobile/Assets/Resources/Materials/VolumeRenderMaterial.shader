Shader "Unlit/VolumeRenderMaterial"
{
    Properties
    {
        _dataTex ("Data Texture (Generated)", 3D) = "" {}
        _MinVal ("Min Value", Range(0.0, 1.0)) = 0.0
        _MaxVal ("Max Value", Range(0.0, 1.0)) = 1.0
        _StepCount ("Step Count", Int) = 128
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Cull Front
        ZTest LEqual
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION; // clip space
                float3 vertexLocal : TEXCOORD1; // object space
            };

            sampler3D _dataTex;
            // sampler2D _NoiseTex;
            float _MinVal;
            float _MaxVal;
            int _StepCount;

            struct Ray
            {
                float3 startPos;
                float3 endPos;
                float3 Dir;
                float2 aabbInters;
            };

            struct RayMarch
            {
                Ray ray;
                int numOfStepCount;
                float stepSize;
                float numStepsRecip;
            };

            float3 getViewRayDir(float3 vertexLocal) // return a ray in the object space (object->camera)
            {
                if(unity_OrthoParams.w == 0)
                {
                    // Perspective
                    return normalize(ObjSpaceViewDir(float4(vertexLocal, 0.0f)));
                }
                else
                {
                    // Orthographic
                    float3 camfwd = mul((float3x3)unity_CameraToWorld, float3(0,0,-1));
                    float4 camfwdobjspace = mul(unity_WorldToObject, camfwd);
                    return normalize(camfwdobjspace);
                }
            }

            // Find ray intersection points with axis aligned bounding box
            float2 intersectAABB(float3 rayOrigin, float3 rayDir, float3 boxMin, float3 boxMax)
            {
                float3 tMin = (boxMin - rayOrigin) / rayDir;
                float3 tMax = (boxMax - rayOrigin) / rayDir;
                float3 t1 = min(tMin, tMax);
                float3 t2 = max(tMin, tMax);
                float tNear = max(max(t1.x, t1.y), t1.z);
                float tFar = min(min(t2.x, t2.y), t2.z);
                return float2(tNear, tFar); // tnear 和 tfar 表示在光线方向上走的距离，标记出了 ray 和 aabb box 的两个交点
            };

            Ray getRayBack2Front(float3 vertexLocal)
            // vertexLocal is the in the coordinate of a cube(obecjt sapce), center is (0.0,0.0,0.0)
            {
                Ray ray;
                ray.Dir = getViewRayDir(vertexLocal);
                ray.startPos = vertexLocal + float3(0.5f, 0.5f, 0.5f); // transfer to a (0.0,1.0) cube texture space
                // Find intersections with axis aligned boundinng box (the volume)
                // float3(0.0, 0.0, 0.0), float3(1.0f, 1.0f, 1.0) is the bounding box of a texture3D
                ray.aabbInters = intersectAABB(ray.startPos, ray.Dir, float3(0.0, 0.0, 0.0), float3(1.0f, 1.0f, 1.0));

                // Check if camera is inside AABB
                const float3 farPos = ray.startPos + ray.Dir * ray.aabbInters.y - float3(0.5f, 0.5f, 0.5f);
                float4 clipPos = UnityObjectToClipPos(float4(farPos, 1.0f));
                ray.aabbInters += min(clipPos.w, 0.0);

                ray.endPos = ray.startPos + ray.Dir * ray.aabbInters.y;
                return ray;
            }

            RayMarch initRaymarch(Ray ray, int maxNumSteps)
            {
                RayMarch raymarchInfo;
                raymarchInfo.ray = ray;
                raymarchInfo.stepSize = 1.732f/*greatest distance in box*/ / maxNumSteps;
                raymarchInfo.numOfStepCount = (int)clamp(abs(ray.aabbInters.x - ray.aabbInters.y) / raymarchInfo.stepSize, 1, maxNumSteps);
                raymarchInfo.numStepsRecip = 1.0 / raymarchInfo.numOfStepCount;
                return raymarchInfo;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertexLocal = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                Ray ray = getRayBack2Front(i.vertexLocal);
                RayMarch raymarchInfo = initRaymarch(ray, _StepCount);

                float maxDensity = 0.0f;
                for (int iStep = 0; iStep < raymarchInfo.numOfStepCount; iStep++)
                {
                    const float t = iStep * raymarchInfo.numStepsRecip;
                    const float3 currPos = lerp(ray.startPos, ray.endPos, t);

                    const float density = tex3Dlod(_dataTex, float4(currPos.x, currPos.y, currPos.z, 0.0f));
                    if (density > maxDensity)
                    {
                        maxDensity = density;
                    }
                }

                // Write fragment output
                return float4(1.0f, 1.0f, 1.0f, maxDensity); // maximum intensity
            }
            
            ENDCG
        }
    }
}
