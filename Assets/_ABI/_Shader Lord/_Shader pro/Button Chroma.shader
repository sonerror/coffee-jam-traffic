Shader "URP/Button Chroma"
{
    Properties
    {
        Vector1_2a57fa8ef8cd40b98a239a9739fe142e("left", Float) = -0.1
        Vector1_fb37676100a74dc28992c96f455638f3("right", Float) = 0.1
        [HDR]Color_45116af49c84427292bdd72ac0180c2a("Color", Color) = (0, 1, 0.1098039, 0)
        [HDR]Color_f4190c21519c44d69981e643913ccbc4("Color right", Color) = (1, 0, 0, 0)
        Vector1_46de468335bd468d9639df3ae81e6b0d("speed", Float) = -0.5
        Vector1_6afabf2a7f1f4c98be96f83cc5943815("str", Float) = 0.2
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

                _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
              ZTest [unity_GUIZTestMode]
        ZWrite Off

        Stencil
        {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp]
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
            float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_2a57fa8ef8cd40b98a239a9739fe142e;
        float Vector1_fb37676100a74dc28992c96f455638f3;
        float4 Color_45116af49c84427292bdd72ac0180c2a;
        float4 Color_f4190c21519c44d69981e643913ccbc4;
        float Vector1_46de468335bd468d9639df3ae81e6b0d;
        float Vector1_6afabf2a7f1f4c98be96f83cc5943815;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;

            // Graph Functions
            
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }


        inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
        }

        inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
        {
            return (1.0-t)*a + (t*b);
        }


        inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);

            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
        {
            float t = 0.0;

            float freq = pow(2.0, float(0));
            float amp = pow(0.5, float(3-0));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            Out = t;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_d87630b382b049bba748b50be9a31f0f_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d87630b382b049bba748b50be9a31f0f_Out_0.tex, _Property_d87630b382b049bba748b50be9a31f0f_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_R_4 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.r;
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_G_5 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.g;
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_B_6 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.b;
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.a;
            float _Property_86e8d2e8d85d4951a266e48e4f6e4480_Out_0 = Vector1_fb37676100a74dc28992c96f455638f3;
            float2 _TilingAndOffset_f6c82d018e954b6fae4ccf28be28d00c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), (_Property_86e8d2e8d85d4951a266e48e4f6e4480_Out_0.xx), _TilingAndOffset_f6c82d018e954b6fae4ccf28be28d00c_Out_3);
            float4 _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d87630b382b049bba748b50be9a31f0f_Out_0.tex, _Property_d87630b382b049bba748b50be9a31f0f_Out_0.samplerstate, _TilingAndOffset_f6c82d018e954b6fae4ccf28be28d00c_Out_3);
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_R_4 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.r;
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_G_5 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.g;
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_B_6 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.b;
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.a;
            float _Subtract_dfc811fa17eb4d849b2f5e5be5e35819_Out_2;
            Unity_Subtract_float(_SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7, _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7, _Subtract_dfc811fa17eb4d849b2f5e5be5e35819_Out_2);
            float _Property_a0ed7f6f0c624f5dad569b99f4aab120_Out_0 = Vector1_2a57fa8ef8cd40b98a239a9739fe142e;
            float2 _TilingAndOffset_e776162256bb4b7c8f8f1eadcbb46edc_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), (_Property_a0ed7f6f0c624f5dad569b99f4aab120_Out_0.xx), _TilingAndOffset_e776162256bb4b7c8f8f1eadcbb46edc_Out_3);
            float4 _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d87630b382b049bba748b50be9a31f0f_Out_0.tex, _Property_d87630b382b049bba748b50be9a31f0f_Out_0.samplerstate, _TilingAndOffset_e776162256bb4b7c8f8f1eadcbb46edc_Out_3);
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_R_4 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.r;
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_G_5 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.g;
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_B_6 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.b;
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.a;
            float _Subtract_8afd06b36d64417782a81b22fcb3e6c4_Out_2;
            Unity_Subtract_float(_Subtract_dfc811fa17eb4d849b2f5e5be5e35819_Out_2, _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7, _Subtract_8afd06b36d64417782a81b22fcb3e6c4_Out_2);
            float _Saturate_82774f7a91e743729e7651cca27679d7_Out_1;
            Unity_Saturate_float(_Subtract_8afd06b36d64417782a81b22fcb3e6c4_Out_2, _Saturate_82774f7a91e743729e7651cca27679d7_Out_1);
            float _Property_2e3ffc39a25a44f38b4badea4bb6c1cf_Out_0 = Vector1_46de468335bd468d9639df3ae81e6b0d;
            float _Multiply_a61a333f722b40d4ae34613cd3cc7a6f_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_2e3ffc39a25a44f38b4badea4bb6c1cf_Out_0, _Multiply_a61a333f722b40d4ae34613cd3cc7a6f_Out_2);
            float2 _Vector2_3121547898c64c74b91679baee7768e7_Out_0 = float2(0, _Multiply_a61a333f722b40d4ae34613cd3cc7a6f_Out_2);
            float2 _TilingAndOffset_022a280b0bde4a89a2bc017dd8696e07_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_3121547898c64c74b91679baee7768e7_Out_0, _TilingAndOffset_022a280b0bde4a89a2bc017dd8696e07_Out_3);
            float _SimpleNoise_f49e2a6d35bd4577b1fa25aaf53c5243_Out_2;
            Unity_SimpleNoise_float(_TilingAndOffset_022a280b0bde4a89a2bc017dd8696e07_Out_3, 12, _SimpleNoise_f49e2a6d35bd4577b1fa25aaf53c5243_Out_2);
            float _Remap_7a1281ab1f7f419aa01b3e12e8a4c291_Out_3;
            Unity_Remap_float(_SimpleNoise_f49e2a6d35bd4577b1fa25aaf53c5243_Out_2, float2 (0, 1), float2 (-0.5, 1), _Remap_7a1281ab1f7f419aa01b3e12e8a4c291_Out_3);
            float _Property_1eb9ad8e746e499eb2d712324eccfba4_Out_0 = Vector1_6afabf2a7f1f4c98be96f83cc5943815;
            float _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2;
            Unity_Multiply_float(_Remap_7a1281ab1f7f419aa01b3e12e8a4c291_Out_3, _Property_1eb9ad8e746e499eb2d712324eccfba4_Out_0, _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2);
            float _Multiply_68ecd1af82b64531905bb78a07e9cf17_Out_2;
            Unity_Multiply_float(_Saturate_82774f7a91e743729e7651cca27679d7_Out_1, _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2, _Multiply_68ecd1af82b64531905bb78a07e9cf17_Out_2);
            float4 _Property_f2cde1731b754954b8bf668a35f09b3f_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_f4190c21519c44d69981e643913ccbc4) : Color_f4190c21519c44d69981e643913ccbc4;
            float4 _Multiply_ffdfdfcb93764e50b329404c670acef5_Out_2;
            Unity_Multiply_float(_Property_f2cde1731b754954b8bf668a35f09b3f_Out_0, (_Saturate_82774f7a91e743729e7651cca27679d7_Out_1.xxxx), _Multiply_ffdfdfcb93764e50b329404c670acef5_Out_2);
            float4 _Add_3192b56a880342a1814ee65bf60a0f53_Out_2;
            Unity_Add_float4((_Multiply_68ecd1af82b64531905bb78a07e9cf17_Out_2.xxxx), _Multiply_ffdfdfcb93764e50b329404c670acef5_Out_2, _Add_3192b56a880342a1814ee65bf60a0f53_Out_2);
            float4 _Add_5b28ae9eab664415ae53b4c1c25c1a56_Out_2;
            Unity_Add_float4(_SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0, _Add_3192b56a880342a1814ee65bf60a0f53_Out_2, _Add_5b28ae9eab664415ae53b4c1c25c1a56_Out_2);
            float _Subtract_90e822b7106d49c792437c4632c6599f_Out_2;
            Unity_Subtract_float(_SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7, _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7, _Subtract_90e822b7106d49c792437c4632c6599f_Out_2);
            float _Subtract_cb9a55b5e08e4271845e51541d39b29b_Out_2;
            Unity_Subtract_float(_Subtract_90e822b7106d49c792437c4632c6599f_Out_2, _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7, _Subtract_cb9a55b5e08e4271845e51541d39b29b_Out_2);
            float _Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1;
            Unity_Saturate_float(_Subtract_cb9a55b5e08e4271845e51541d39b29b_Out_2, _Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1);
            float _Multiply_7af1751a88ad48b0a905488001077de3_Out_2;
            Unity_Multiply_float(_Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1, _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2, _Multiply_7af1751a88ad48b0a905488001077de3_Out_2);
            float4 _Property_d97055f37f5b4dbfada2bdf4f48976b7_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_45116af49c84427292bdd72ac0180c2a) : Color_45116af49c84427292bdd72ac0180c2a;
            float4 _Multiply_37ae88a9b6704fbdaf8464c7036038c4_Out_2;
            Unity_Multiply_float(_Property_d97055f37f5b4dbfada2bdf4f48976b7_Out_0, (_Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1.xxxx), _Multiply_37ae88a9b6704fbdaf8464c7036038c4_Out_2);
            float4 _Add_4dbfa9950595459c8d8f972274ec343a_Out_2;
            Unity_Add_float4((_Multiply_7af1751a88ad48b0a905488001077de3_Out_2.xxxx), _Multiply_37ae88a9b6704fbdaf8464c7036038c4_Out_2, _Add_4dbfa9950595459c8d8f972274ec343a_Out_2);
            float4 _Add_bddfb7f105354be8b9bc3b5accce33d7_Out_2;
            Unity_Add_float4(_Add_5b28ae9eab664415ae53b4c1c25c1a56_Out_2, _Add_4dbfa9950595459c8d8f972274ec343a_Out_2, _Add_bddfb7f105354be8b9bc3b5accce33d7_Out_2);
            float _Add_30789788668a4b1483d33aa40179069b_Out_2;
            Unity_Add_float(_SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7, _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7, _Add_30789788668a4b1483d33aa40179069b_Out_2);
            float _Add_28850c2974f04286b3bf67a15fed034a_Out_2;
            Unity_Add_float(_Add_30789788668a4b1483d33aa40179069b_Out_2, _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7, _Add_28850c2974f04286b3bf67a15fed034a_Out_2);
            float _Saturate_6dfbd6225f3d45e08d272d312dbbade1_Out_1;
            Unity_Saturate_float(_Add_28850c2974f04286b3bf67a15fed034a_Out_2, _Saturate_6dfbd6225f3d45e08d272d312dbbade1_Out_1);
            float _Saturate_bb0f7eb8bb884ad2b1b9ca988050a821_Out_1;
            Unity_Saturate_float(_Saturate_6dfbd6225f3d45e08d272d312dbbade1_Out_1, _Saturate_bb0f7eb8bb884ad2b1b9ca988050a821_Out_1);
            surface.BaseColor = (_Add_bddfb7f105354be8b9bc3b5accce33d7_Out_2.xyz);
            surface.Alpha = _Saturate_bb0f7eb8bb884ad2b1b9ca988050a821_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
           ZTest [unity_GUIZTestMode]
        ZWrite Off

        Stencil
        {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp]
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
            float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_2a57fa8ef8cd40b98a239a9739fe142e;
        float Vector1_fb37676100a74dc28992c96f455638f3;
        float4 Color_45116af49c84427292bdd72ac0180c2a;
        float4 Color_f4190c21519c44d69981e643913ccbc4;
        float Vector1_46de468335bd468d9639df3ae81e6b0d;
        float Vector1_6afabf2a7f1f4c98be96f83cc5943815;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;

            // Graph Functions
            
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }


        inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
        }

        inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
        {
            return (1.0-t)*a + (t*b);
        }


        inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);

            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
        {
            float t = 0.0;

            float freq = pow(2.0, float(0));
            float amp = pow(0.5, float(3-0));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            Out = t;
        }

        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_d87630b382b049bba748b50be9a31f0f_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d87630b382b049bba748b50be9a31f0f_Out_0.tex, _Property_d87630b382b049bba748b50be9a31f0f_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_R_4 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.r;
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_G_5 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.g;
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_B_6 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.b;
            float _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7 = _SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0.a;
            float _Property_86e8d2e8d85d4951a266e48e4f6e4480_Out_0 = Vector1_fb37676100a74dc28992c96f455638f3;
            float2 _TilingAndOffset_f6c82d018e954b6fae4ccf28be28d00c_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), (_Property_86e8d2e8d85d4951a266e48e4f6e4480_Out_0.xx), _TilingAndOffset_f6c82d018e954b6fae4ccf28be28d00c_Out_3);
            float4 _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d87630b382b049bba748b50be9a31f0f_Out_0.tex, _Property_d87630b382b049bba748b50be9a31f0f_Out_0.samplerstate, _TilingAndOffset_f6c82d018e954b6fae4ccf28be28d00c_Out_3);
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_R_4 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.r;
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_G_5 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.g;
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_B_6 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.b;
            float _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7 = _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_RGBA_0.a;
            float _Subtract_dfc811fa17eb4d849b2f5e5be5e35819_Out_2;
            Unity_Subtract_float(_SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7, _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7, _Subtract_dfc811fa17eb4d849b2f5e5be5e35819_Out_2);
            float _Property_a0ed7f6f0c624f5dad569b99f4aab120_Out_0 = Vector1_2a57fa8ef8cd40b98a239a9739fe142e;
            float2 _TilingAndOffset_e776162256bb4b7c8f8f1eadcbb46edc_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), (_Property_a0ed7f6f0c624f5dad569b99f4aab120_Out_0.xx), _TilingAndOffset_e776162256bb4b7c8f8f1eadcbb46edc_Out_3);
            float4 _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d87630b382b049bba748b50be9a31f0f_Out_0.tex, _Property_d87630b382b049bba748b50be9a31f0f_Out_0.samplerstate, _TilingAndOffset_e776162256bb4b7c8f8f1eadcbb46edc_Out_3);
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_R_4 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.r;
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_G_5 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.g;
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_B_6 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.b;
            float _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7 = _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_RGBA_0.a;
            float _Subtract_8afd06b36d64417782a81b22fcb3e6c4_Out_2;
            Unity_Subtract_float(_Subtract_dfc811fa17eb4d849b2f5e5be5e35819_Out_2, _SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7, _Subtract_8afd06b36d64417782a81b22fcb3e6c4_Out_2);
            float _Saturate_82774f7a91e743729e7651cca27679d7_Out_1;
            Unity_Saturate_float(_Subtract_8afd06b36d64417782a81b22fcb3e6c4_Out_2, _Saturate_82774f7a91e743729e7651cca27679d7_Out_1);
            float _Property_2e3ffc39a25a44f38b4badea4bb6c1cf_Out_0 = Vector1_46de468335bd468d9639df3ae81e6b0d;
            float _Multiply_a61a333f722b40d4ae34613cd3cc7a6f_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_2e3ffc39a25a44f38b4badea4bb6c1cf_Out_0, _Multiply_a61a333f722b40d4ae34613cd3cc7a6f_Out_2);
            float2 _Vector2_3121547898c64c74b91679baee7768e7_Out_0 = float2(0, _Multiply_a61a333f722b40d4ae34613cd3cc7a6f_Out_2);
            float2 _TilingAndOffset_022a280b0bde4a89a2bc017dd8696e07_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_3121547898c64c74b91679baee7768e7_Out_0, _TilingAndOffset_022a280b0bde4a89a2bc017dd8696e07_Out_3);
            float _SimpleNoise_f49e2a6d35bd4577b1fa25aaf53c5243_Out_2;
            Unity_SimpleNoise_float(_TilingAndOffset_022a280b0bde4a89a2bc017dd8696e07_Out_3, 12, _SimpleNoise_f49e2a6d35bd4577b1fa25aaf53c5243_Out_2);
            float _Remap_7a1281ab1f7f419aa01b3e12e8a4c291_Out_3;
            Unity_Remap_float(_SimpleNoise_f49e2a6d35bd4577b1fa25aaf53c5243_Out_2, float2 (0, 1), float2 (-0.5, 1), _Remap_7a1281ab1f7f419aa01b3e12e8a4c291_Out_3);
            float _Property_1eb9ad8e746e499eb2d712324eccfba4_Out_0 = Vector1_6afabf2a7f1f4c98be96f83cc5943815;
            float _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2;
            Unity_Multiply_float(_Remap_7a1281ab1f7f419aa01b3e12e8a4c291_Out_3, _Property_1eb9ad8e746e499eb2d712324eccfba4_Out_0, _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2);
            float _Multiply_68ecd1af82b64531905bb78a07e9cf17_Out_2;
            Unity_Multiply_float(_Saturate_82774f7a91e743729e7651cca27679d7_Out_1, _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2, _Multiply_68ecd1af82b64531905bb78a07e9cf17_Out_2);
            float4 _Property_f2cde1731b754954b8bf668a35f09b3f_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_f4190c21519c44d69981e643913ccbc4) : Color_f4190c21519c44d69981e643913ccbc4;
            float4 _Multiply_ffdfdfcb93764e50b329404c670acef5_Out_2;
            Unity_Multiply_float(_Property_f2cde1731b754954b8bf668a35f09b3f_Out_0, (_Saturate_82774f7a91e743729e7651cca27679d7_Out_1.xxxx), _Multiply_ffdfdfcb93764e50b329404c670acef5_Out_2);
            float4 _Add_3192b56a880342a1814ee65bf60a0f53_Out_2;
            Unity_Add_float4((_Multiply_68ecd1af82b64531905bb78a07e9cf17_Out_2.xxxx), _Multiply_ffdfdfcb93764e50b329404c670acef5_Out_2, _Add_3192b56a880342a1814ee65bf60a0f53_Out_2);
            float4 _Add_5b28ae9eab664415ae53b4c1c25c1a56_Out_2;
            Unity_Add_float4(_SampleTexture2D_11025b520378442faf7a96f309730f1a_RGBA_0, _Add_3192b56a880342a1814ee65bf60a0f53_Out_2, _Add_5b28ae9eab664415ae53b4c1c25c1a56_Out_2);
            float _Subtract_90e822b7106d49c792437c4632c6599f_Out_2;
            Unity_Subtract_float(_SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7, _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7, _Subtract_90e822b7106d49c792437c4632c6599f_Out_2);
            float _Subtract_cb9a55b5e08e4271845e51541d39b29b_Out_2;
            Unity_Subtract_float(_Subtract_90e822b7106d49c792437c4632c6599f_Out_2, _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7, _Subtract_cb9a55b5e08e4271845e51541d39b29b_Out_2);
            float _Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1;
            Unity_Saturate_float(_Subtract_cb9a55b5e08e4271845e51541d39b29b_Out_2, _Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1);
            float _Multiply_7af1751a88ad48b0a905488001077de3_Out_2;
            Unity_Multiply_float(_Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1, _Multiply_8e8fbfc39bc94d8f8793af4dd6014c81_Out_2, _Multiply_7af1751a88ad48b0a905488001077de3_Out_2);
            float4 _Property_d97055f37f5b4dbfada2bdf4f48976b7_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_45116af49c84427292bdd72ac0180c2a) : Color_45116af49c84427292bdd72ac0180c2a;
            float4 _Multiply_37ae88a9b6704fbdaf8464c7036038c4_Out_2;
            Unity_Multiply_float(_Property_d97055f37f5b4dbfada2bdf4f48976b7_Out_0, (_Saturate_fa34e4dd3d3842af97abef724d5c115a_Out_1.xxxx), _Multiply_37ae88a9b6704fbdaf8464c7036038c4_Out_2);
            float4 _Add_4dbfa9950595459c8d8f972274ec343a_Out_2;
            Unity_Add_float4((_Multiply_7af1751a88ad48b0a905488001077de3_Out_2.xxxx), _Multiply_37ae88a9b6704fbdaf8464c7036038c4_Out_2, _Add_4dbfa9950595459c8d8f972274ec343a_Out_2);
            float4 _Add_bddfb7f105354be8b9bc3b5accce33d7_Out_2;
            Unity_Add_float4(_Add_5b28ae9eab664415ae53b4c1c25c1a56_Out_2, _Add_4dbfa9950595459c8d8f972274ec343a_Out_2, _Add_bddfb7f105354be8b9bc3b5accce33d7_Out_2);
            float _Add_30789788668a4b1483d33aa40179069b_Out_2;
            Unity_Add_float(_SampleTexture2D_e7d4fee7d8434159814dee72e3439ff2_A_7, _SampleTexture2D_0efb5a53c95a4a5cb0568c1549badbca_A_7, _Add_30789788668a4b1483d33aa40179069b_Out_2);
            float _Add_28850c2974f04286b3bf67a15fed034a_Out_2;
            Unity_Add_float(_Add_30789788668a4b1483d33aa40179069b_Out_2, _SampleTexture2D_11025b520378442faf7a96f309730f1a_A_7, _Add_28850c2974f04286b3bf67a15fed034a_Out_2);
            float _Saturate_6dfbd6225f3d45e08d272d312dbbade1_Out_1;
            Unity_Saturate_float(_Add_28850c2974f04286b3bf67a15fed034a_Out_2, _Saturate_6dfbd6225f3d45e08d272d312dbbade1_Out_1);
            float _Saturate_bb0f7eb8bb884ad2b1b9ca988050a821_Out_1;
            Unity_Saturate_float(_Saturate_6dfbd6225f3d45e08d272d312dbbade1_Out_1, _Saturate_bb0f7eb8bb884ad2b1b9ca988050a821_Out_1);
            surface.BaseColor = (_Add_bddfb7f105354be8b9bc3b5accce33d7_Out_2.xyz);
            surface.Alpha = _Saturate_bb0f7eb8bb884ad2b1b9ca988050a821_Out_1;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}