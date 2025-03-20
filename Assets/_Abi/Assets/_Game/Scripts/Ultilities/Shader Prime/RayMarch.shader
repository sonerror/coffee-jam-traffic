Shader "Custom/RayMarch"
{
    Properties
    {
        _DepthMayBe("DepthMaybe",Range(-.5,1.5)) = .5 
        _Color ("Color " , Color) = (1,1,1,1)
        _Sphere("Sphere",Vector) = (0,0,0,0)
        _Box ("Box",Vector) = (0,0,0,0)
        _MaxStep("Step",Float) = 60
        _MaxDis("MaxDis",Float) = 100
        _Smooth("Smooth",Float) = .5
        _Absorb("Absorb",Float) = .5
        _BigIncre("BigIncre",Float) = .2
        _SmallIncre("SmallIncre",Float) = .02
        _NoiseScale("NoiseScale", Float) = 1
        _TimeSpeed("TimeSpeed",Float) = .5
        _StencilRef("Stecil ref",Int) = 0
        _MaxIrre("MmaxIrre",Float) = 27
        _PowBase("PowBase",Float) = 5
    }
    SubShader
    {
        
        // No culling or depth
//        Cull Back
//         ZWrite Off 
//         ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
        Stencil
        {
            Ref [_StencilRef]
            Comp Equal
            Pass Keep
//            Fail Replace
        }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // #include "UnityCG.cginc"
            #include  "DistanceFunctions.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
           

            uniform  float4x4 CamFrustum,C2WMatrix;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ray:TEXCOORD1;
            };
            
            float _DepthMayBe;

            v2f vert (appdata v)
            {
                v2f o;
                #if UNITY_UV_STARTS_AT_TOP
                o.vertex = (v.vertex * float4(2,-2,1,1)) - float4(0,0,0,0);
                #else
                o.vertex = (v.vertex * float4(2,2,1,1)) - float4(0,0,0,0);
                #endif
                
                o.vertex.z = _DepthMayBe;
                ////////////////////////////////////////////////////////////////////
                half index = v.vertex.z;
                v.vertex.z = 0;
                // o.vertex = TransformObjectToHClip(v.vertex); // if = v.vertex , quad will be drawn at top right conner -> screen is quad -1 -1     -1 1   1 -1    1 1 with gl.quads
                // o.vertex = v.vertex;
                o.ray = CamFrustum[(int)index].xyz;
                o.ray /= abs(o.ray.z);
                o.ray = mul(C2WMatrix,o.ray);
                
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            half4 _Color;
            float4 _Sphere;
            float4 _Box;
            half _MaxStep;
            float _MaxDis;
            float _Smooth;
            float _Absorb;
            float _BigIncre;
            float _SmallIncre;
            float _NoiseScale;
            float _TimeSpeed;
            float _MaxIrre;
            float _PowBase;

            float beersLaw(float d)
            {
                return pow(_PowBase,-d*_Absorb);
            }
            

            float sdf_Sphere_Cus(float3 dis, float radius)
            {
                return length(dis) - radius;
            }

            float distanceField(float3 p)
            {
                float res  =0;
                float sphere = sdf_Sphere_Cus(p - _Sphere.xyz,_Sphere.w);
                // float box = sdBox(p-_Box.xyz,_Box.www);
                //
                // res = opIS(-sphere,box,_Smooth);
                
                return sphere;
            }
            
            float3 getNormal(float3 p)
            {
                const float2 offset = float2(.001,0);
                float3 n  =float3(
                         distanceField(p+offset.xyy) - distanceField(p-offset.xyy),
                         distanceField(p+offset.yxy) - distanceField(p-offset.yxy),
                         distanceField(p+offset.yyx) - distanceField(p-offset.yyx)
                    );

                return normalize(n);
            }

            // scalar bug mean not fittable type like try to input float4 for float comparaytion
            
            half4 raymarching(float3 ro, float3 rd)
            {
                half4 result = half4(0,0,0,1);

                float dis = 0;

               for(int i=0; i<_MaxStep;i++)
               {
                   if(dis>_MaxDis)
                   {
                       result = half4(0,0,0,0);
                       break;
                   }

                   float3 p = ro + rd * dis;

                   float d = distanceField(p);
                   if(d<.01f)
                   {
                       result = _Color + _Color * dot(getNormal(p),GetMainLight().direction);
                        result.a = _Color.a;
                       break;
                   }

                   dis += d;
               }

                return result;
            }

            float2 unity_gradientNoise_dir(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float unity_gradientNoise(float2 p)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(unity_gradientNoise_dir(ip), fp);
    float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

            float getNoise(float3 p)
            {
                return unity_gradientNoise((p.xy + p.z)*_NoiseScale) + 0.5;
            }

            float getAlpha(float3 ro, float3 rd)
            {
                float dens = 0;
                float dis = 0;
                float step;
                float irre = 0;

               for(int i=0; i<_MaxStep;i++)
               {
                   if(dis>_MaxDis)
                   {
                       break;
                   }

                   float3 p = ro + rd*dis;
                   float d = distanceField(p);

                   if(d<0)
                   {
                       // step = max(_SmallIncre,-d);
                       step  = _SmallIncre;
                       irre  += _SmallIncre;
                       dens += step *getNoise(irre* p *_NoiseScale + _Time*_TimeSpeed);
                        // dens += step;
                   }
                   else
                   {
                       step = max(d,_SmallIncre);
                   }

                   dis += step;
                   
               }

                // return (1-beersLaw(dens))*getNoise(dens*_NoiseScale + _Time*_TimeSpeed);
                return (1-beersLaw(dens));
                // return irre;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 rayDir = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;
                half4 col =half4(_Color.xyz,getAlpha(rayOrigin,rayDir));
                // half c  = getAlpha(rayOrigin,rayDir)/_MaxIrre;
                // half4 col = c;
                // half c = getAlpha(rayOrigin,rayDir);
                // half4 col = half4(c,c,c,1);
                return col;
                // return half4(0,0,0,1);
            }
            ENDHLSL
        }
    }
}
