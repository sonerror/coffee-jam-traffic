Shader "URP/Button Idle"
{
    Properties
    {
        Vector1_2713f090f3064018affc5274030ff792("str", Range(0, 5)) = 0
        [NoScaleOffset]Texture2D_0ae3ff0bb764425ca85d0ce1932ffd5e("Texture2D (1)", 2D) = "white" {}
        Vector1_ca470111a100494b95feebccb7eff930("rot speed", Float) = 0
        [NoScaleOffset]Texture2D_86d4ef24c2d34bd385f9e41d2bb12702("BG light MAsk", 2D) = "white" {}
        Vector1_5918ca520b11405bb4de940a39ec4b0a("light speed", Float) = 0
        Vector1_58a6d70653af4b9cbad4a13be22195ba("light strenthg", Float) = 0
        Color_173e509dd48c42b884076fa2ddca203f("light COlor", Color) = (0, 0, 0, 0)
        Vector1_8c2a51773b5c4d879c17e7cc628951af("float speed", Float) = 0
        Vector1_dde646ad75374a2e95f7641dfcd7529c("float str", Float) = 0
        [NoScaleOffset]Texture2D_f817e98ebd4c44f88f8fe8d049e15743("bubble Tex", 2D) = "white" {}
        Vector1_d158349fd5b341cc92404b987953e8a2("float ofset", Float) = 0
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
        float Vector1_2713f090f3064018affc5274030ff792;
        float4 Texture2D_0ae3ff0bb764425ca85d0ce1932ffd5e_TexelSize;
        float Vector1_ca470111a100494b95feebccb7eff930;
        float4 Texture2D_86d4ef24c2d34bd385f9e41d2bb12702_TexelSize;
        float Vector1_5918ca520b11405bb4de940a39ec4b0a;
        float Vector1_58a6d70653af4b9cbad4a13be22195ba;
        float4 Color_173e509dd48c42b884076fa2ddca203f;
        float Vector1_8c2a51773b5c4d879c17e7cc628951af;
        float Vector1_dde646ad75374a2e95f7641dfcd7529c;
        float4 Texture2D_f817e98ebd4c44f88f8fe8d049e15743_TexelSize;
        float Vector1_d158349fd5b341cc92404b987953e8a2;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(Texture2D_0ae3ff0bb764425ca85d0ce1932ffd5e);
        SAMPLER(samplerTexture2D_0ae3ff0bb764425ca85d0ce1932ffd5e);
        TEXTURE2D(Texture2D_86d4ef24c2d34bd385f9e41d2bb12702);
        SAMPLER(samplerTexture2D_86d4ef24c2d34bd385f9e41d2bb12702);
        TEXTURE2D(Texture2D_f817e98ebd4c44f88f8fe8d049e15743);
        SAMPLER(samplerTexture2D_f817e98ebd4c44f88f8fe8d049e15743);

            // Graph Functions
            
        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
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

        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
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
            UnityTexture2D _Property_9143f5ebb78b49a498ebcd5664fe7960_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_f817e98ebd4c44f88f8fe8d049e15743);
            float _Property_248946c7974e4190ae92ebd0ec195888_Out_0 = Vector1_8c2a51773b5c4d879c17e7cc628951af;
            float _Multiply_2e86e95879ce4012b105b9204f9795c3_Out_2;
            Unity_Multiply_float(_Property_248946c7974e4190ae92ebd0ec195888_Out_0, IN.TimeParameters.x, _Multiply_2e86e95879ce4012b105b9204f9795c3_Out_2);
            float _Sine_2cf82ba34b8f4d5a9f963be5b5a81168_Out_1;
            Unity_Sine_float(_Multiply_2e86e95879ce4012b105b9204f9795c3_Out_2, _Sine_2cf82ba34b8f4d5a9f963be5b5a81168_Out_1);
            float _Property_8220dc7bb57c4a22afa27b508358a243_Out_0 = Vector1_dde646ad75374a2e95f7641dfcd7529c;
            float _Multiply_7bc1faefdde848149d3b538e73e86d0b_Out_2;
            Unity_Multiply_float(_Sine_2cf82ba34b8f4d5a9f963be5b5a81168_Out_1, _Property_8220dc7bb57c4a22afa27b508358a243_Out_0, _Multiply_7bc1faefdde848149d3b538e73e86d0b_Out_2);
            float _Property_7108f39c57d44892b851ce93c7cffe28_Out_0 = Vector1_d158349fd5b341cc92404b987953e8a2;
            float _Add_0c84fce14ff649f4b625748f30bd2d9d_Out_2;
            Unity_Add_float(_Multiply_7bc1faefdde848149d3b538e73e86d0b_Out_2, _Property_7108f39c57d44892b851ce93c7cffe28_Out_0, _Add_0c84fce14ff649f4b625748f30bd2d9d_Out_2);
            float2 _Vector2_2cbd5586d28b46c6973534b6fcf1f5cd_Out_0 = float2(0, _Add_0c84fce14ff649f4b625748f30bd2d9d_Out_2);
            float2 _TilingAndOffset_28437b06de48470f9eb537818897d347_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_2cbd5586d28b46c6973534b6fcf1f5cd_Out_0, _TilingAndOffset_28437b06de48470f9eb537818897d347_Out_3);
            float4 _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9143f5ebb78b49a498ebcd5664fe7960_Out_0.tex, _Property_9143f5ebb78b49a498ebcd5664fe7960_Out_0.samplerstate, _TilingAndOffset_28437b06de48470f9eb537818897d347_Out_3);
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_R_4 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.r;
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_G_5 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.g;
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_B_6 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.b;
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_A_7 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.a;
            UnityTexture2D _Property_50c345d101864bec89ec36f626b68647_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_86d4ef24c2d34bd385f9e41d2bb12702);
            float4 _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_50c345d101864bec89ec36f626b68647_Out_0.tex, _Property_50c345d101864bec89ec36f626b68647_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_R_4 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.r;
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_G_5 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.g;
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_B_6 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.b;
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_A_7 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.a;
            float _Property_bce971dcc70d438ca5b4f7fd629eb6c5_Out_0 = Vector1_58a6d70653af4b9cbad4a13be22195ba;
            float _Property_6802fd5722e443f69d854fcf5c5869aa_Out_0 = Vector1_5918ca520b11405bb4de940a39ec4b0a;
            float _Multiply_251ae6d5c1a34e78bbc3a736bae091a6_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_6802fd5722e443f69d854fcf5c5869aa_Out_0, _Multiply_251ae6d5c1a34e78bbc3a736bae091a6_Out_2);
            float _Sine_fac684727c2742b5997942629e5ee240_Out_1;
            Unity_Sine_float(_Multiply_251ae6d5c1a34e78bbc3a736bae091a6_Out_2, _Sine_fac684727c2742b5997942629e5ee240_Out_1);
            float _Multiply_7517b0d5a9cf4f2496003a10a04c0ede_Out_2;
            Unity_Multiply_float(_Property_bce971dcc70d438ca5b4f7fd629eb6c5_Out_0, _Sine_fac684727c2742b5997942629e5ee240_Out_1, _Multiply_7517b0d5a9cf4f2496003a10a04c0ede_Out_2);
            float _Saturate_83a61912b5cb4e5cb7caef3aa2b81262_Out_1;
            Unity_Saturate_float(_Multiply_7517b0d5a9cf4f2496003a10a04c0ede_Out_2, _Saturate_83a61912b5cb4e5cb7caef3aa2b81262_Out_1);
            float4 _Multiply_e1f0c6fe901e4bbfbab0d2c4205e2b6a_Out_2;
            Unity_Multiply_float(_SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0, (_Saturate_83a61912b5cb4e5cb7caef3aa2b81262_Out_1.xxxx), _Multiply_e1f0c6fe901e4bbfbab0d2c4205e2b6a_Out_2);
            float4 _Property_5379430f18774410a10b2b9f5321b39a_Out_0 = Color_173e509dd48c42b884076fa2ddca203f;
            float4 _Multiply_888b1467adda4954929daabfa3e6cb09_Out_2;
            Unity_Multiply_float(_Multiply_e1f0c6fe901e4bbfbab0d2c4205e2b6a_Out_2, _Property_5379430f18774410a10b2b9f5321b39a_Out_0, _Multiply_888b1467adda4954929daabfa3e6cb09_Out_2);
            UnityTexture2D _Property_6a9a826df40a48b7963b0e08bbcff55e_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0ae3ff0bb764425ca85d0ce1932ffd5e);
            float4 _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6a9a826df40a48b7963b0e08bbcff55e_Out_0.tex, _Property_6a9a826df40a48b7963b0e08bbcff55e_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_R_4 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.r;
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_G_5 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.g;
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_B_6 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.b;
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_A_7 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.a;
            float _Property_686b42305a584ab98d7d2ceb9f69379b_Out_0 = Vector1_2713f090f3064018affc5274030ff792;
            float _Property_f336cc430f6744e48ce945586dd4cf91_Out_0 = Vector1_ca470111a100494b95feebccb7eff930;
            float _Multiply_1613626b750e4443a3ebe21c424cb35e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_f336cc430f6744e48ce945586dd4cf91_Out_0, _Multiply_1613626b750e4443a3ebe21c424cb35e_Out_2);
            float2 _Rotate_0f8e905ead2f479bb6226c8f6d0e2296_Out_3;
            Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1613626b750e4443a3ebe21c424cb35e_Out_2, _Rotate_0f8e905ead2f479bb6226c8f6d0e2296_Out_3);
            float _SimpleNoise_248df111e75f454e87c06ed1c5d6ec34_Out_2;
            Unity_SimpleNoise_float(_Rotate_0f8e905ead2f479bb6226c8f6d0e2296_Out_3, 20, _SimpleNoise_248df111e75f454e87c06ed1c5d6ec34_Out_2);
            float _Multiply_02c15b457b6043779be1bc9dac618a12_Out_2;
            Unity_Multiply_float(_Property_686b42305a584ab98d7d2ceb9f69379b_Out_0, _SimpleNoise_248df111e75f454e87c06ed1c5d6ec34_Out_2, _Multiply_02c15b457b6043779be1bc9dac618a12_Out_2);
            float4 _Multiply_70df71f22b634294a00019956ac8759e_Out_2;
            Unity_Multiply_float(_SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0, (_Multiply_02c15b457b6043779be1bc9dac618a12_Out_2.xxxx), _Multiply_70df71f22b634294a00019956ac8759e_Out_2);
            UnityTexture2D _Property_ec839c8b3ef9405b9b2bf4858a73ba1a_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ec839c8b3ef9405b9b2bf4858a73ba1a_Out_0.tex, _Property_ec839c8b3ef9405b9b2bf4858a73ba1a_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_R_4 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.r;
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_G_5 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.g;
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_B_6 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.b;
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_A_7 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.a;
            float4 _Add_7573fa5e42d944a3ace4920e27fad63c_Out_2;
            Unity_Add_float4(_Multiply_70df71f22b634294a00019956ac8759e_Out_2, _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0, _Add_7573fa5e42d944a3ace4920e27fad63c_Out_2);
            float4 _Add_bd267390ca20438196a7db5bb3aaf878_Out_2;
            Unity_Add_float4(_Multiply_888b1467adda4954929daabfa3e6cb09_Out_2, _Add_7573fa5e42d944a3ace4920e27fad63c_Out_2, _Add_bd267390ca20438196a7db5bb3aaf878_Out_2);
            float4 _Add_9daa87b921644b859b4c79c3851cf809_Out_2;
            Unity_Add_float4((_SampleTexture2D_f9fb53164c504f6791269ace2792a54b_A_7.xxxx), _Add_bd267390ca20438196a7db5bb3aaf878_Out_2, _Add_9daa87b921644b859b4c79c3851cf809_Out_2);
            surface.BaseColor = (_Add_9daa87b921644b859b4c79c3851cf809_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_A_7;
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
        float Vector1_2713f090f3064018affc5274030ff792;
        float4 Texture2D_0ae3ff0bb764425ca85d0ce1932ffd5e_TexelSize;
        float Vector1_ca470111a100494b95feebccb7eff930;
        float4 Texture2D_86d4ef24c2d34bd385f9e41d2bb12702_TexelSize;
        float Vector1_5918ca520b11405bb4de940a39ec4b0a;
        float Vector1_58a6d70653af4b9cbad4a13be22195ba;
        float4 Color_173e509dd48c42b884076fa2ddca203f;
        float Vector1_8c2a51773b5c4d879c17e7cc628951af;
        float Vector1_dde646ad75374a2e95f7641dfcd7529c;
        float4 Texture2D_f817e98ebd4c44f88f8fe8d049e15743_TexelSize;
        float Vector1_d158349fd5b341cc92404b987953e8a2;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(Texture2D_0ae3ff0bb764425ca85d0ce1932ffd5e);
        SAMPLER(samplerTexture2D_0ae3ff0bb764425ca85d0ce1932ffd5e);
        TEXTURE2D(Texture2D_86d4ef24c2d34bd385f9e41d2bb12702);
        SAMPLER(samplerTexture2D_86d4ef24c2d34bd385f9e41d2bb12702);
        TEXTURE2D(Texture2D_f817e98ebd4c44f88f8fe8d049e15743);
        SAMPLER(samplerTexture2D_f817e98ebd4c44f88f8fe8d049e15743);

            // Graph Functions
            
        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
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

        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
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
            UnityTexture2D _Property_9143f5ebb78b49a498ebcd5664fe7960_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_f817e98ebd4c44f88f8fe8d049e15743);
            float _Property_248946c7974e4190ae92ebd0ec195888_Out_0 = Vector1_8c2a51773b5c4d879c17e7cc628951af;
            float _Multiply_2e86e95879ce4012b105b9204f9795c3_Out_2;
            Unity_Multiply_float(_Property_248946c7974e4190ae92ebd0ec195888_Out_0, IN.TimeParameters.x, _Multiply_2e86e95879ce4012b105b9204f9795c3_Out_2);
            float _Sine_2cf82ba34b8f4d5a9f963be5b5a81168_Out_1;
            Unity_Sine_float(_Multiply_2e86e95879ce4012b105b9204f9795c3_Out_2, _Sine_2cf82ba34b8f4d5a9f963be5b5a81168_Out_1);
            float _Property_8220dc7bb57c4a22afa27b508358a243_Out_0 = Vector1_dde646ad75374a2e95f7641dfcd7529c;
            float _Multiply_7bc1faefdde848149d3b538e73e86d0b_Out_2;
            Unity_Multiply_float(_Sine_2cf82ba34b8f4d5a9f963be5b5a81168_Out_1, _Property_8220dc7bb57c4a22afa27b508358a243_Out_0, _Multiply_7bc1faefdde848149d3b538e73e86d0b_Out_2);
            float _Property_7108f39c57d44892b851ce93c7cffe28_Out_0 = Vector1_d158349fd5b341cc92404b987953e8a2;
            float _Add_0c84fce14ff649f4b625748f30bd2d9d_Out_2;
            Unity_Add_float(_Multiply_7bc1faefdde848149d3b538e73e86d0b_Out_2, _Property_7108f39c57d44892b851ce93c7cffe28_Out_0, _Add_0c84fce14ff649f4b625748f30bd2d9d_Out_2);
            float2 _Vector2_2cbd5586d28b46c6973534b6fcf1f5cd_Out_0 = float2(0, _Add_0c84fce14ff649f4b625748f30bd2d9d_Out_2);
            float2 _TilingAndOffset_28437b06de48470f9eb537818897d347_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_2cbd5586d28b46c6973534b6fcf1f5cd_Out_0, _TilingAndOffset_28437b06de48470f9eb537818897d347_Out_3);
            float4 _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9143f5ebb78b49a498ebcd5664fe7960_Out_0.tex, _Property_9143f5ebb78b49a498ebcd5664fe7960_Out_0.samplerstate, _TilingAndOffset_28437b06de48470f9eb537818897d347_Out_3);
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_R_4 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.r;
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_G_5 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.g;
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_B_6 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.b;
            float _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_A_7 = _SampleTexture2D_f9fb53164c504f6791269ace2792a54b_RGBA_0.a;
            UnityTexture2D _Property_50c345d101864bec89ec36f626b68647_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_86d4ef24c2d34bd385f9e41d2bb12702);
            float4 _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_50c345d101864bec89ec36f626b68647_Out_0.tex, _Property_50c345d101864bec89ec36f626b68647_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_R_4 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.r;
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_G_5 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.g;
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_B_6 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.b;
            float _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_A_7 = _SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0.a;
            float _Property_bce971dcc70d438ca5b4f7fd629eb6c5_Out_0 = Vector1_58a6d70653af4b9cbad4a13be22195ba;
            float _Property_6802fd5722e443f69d854fcf5c5869aa_Out_0 = Vector1_5918ca520b11405bb4de940a39ec4b0a;
            float _Multiply_251ae6d5c1a34e78bbc3a736bae091a6_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_6802fd5722e443f69d854fcf5c5869aa_Out_0, _Multiply_251ae6d5c1a34e78bbc3a736bae091a6_Out_2);
            float _Sine_fac684727c2742b5997942629e5ee240_Out_1;
            Unity_Sine_float(_Multiply_251ae6d5c1a34e78bbc3a736bae091a6_Out_2, _Sine_fac684727c2742b5997942629e5ee240_Out_1);
            float _Multiply_7517b0d5a9cf4f2496003a10a04c0ede_Out_2;
            Unity_Multiply_float(_Property_bce971dcc70d438ca5b4f7fd629eb6c5_Out_0, _Sine_fac684727c2742b5997942629e5ee240_Out_1, _Multiply_7517b0d5a9cf4f2496003a10a04c0ede_Out_2);
            float _Saturate_83a61912b5cb4e5cb7caef3aa2b81262_Out_1;
            Unity_Saturate_float(_Multiply_7517b0d5a9cf4f2496003a10a04c0ede_Out_2, _Saturate_83a61912b5cb4e5cb7caef3aa2b81262_Out_1);
            float4 _Multiply_e1f0c6fe901e4bbfbab0d2c4205e2b6a_Out_2;
            Unity_Multiply_float(_SampleTexture2D_845ddd220f4c49f79bba17cda435c87e_RGBA_0, (_Saturate_83a61912b5cb4e5cb7caef3aa2b81262_Out_1.xxxx), _Multiply_e1f0c6fe901e4bbfbab0d2c4205e2b6a_Out_2);
            float4 _Property_5379430f18774410a10b2b9f5321b39a_Out_0 = Color_173e509dd48c42b884076fa2ddca203f;
            float4 _Multiply_888b1467adda4954929daabfa3e6cb09_Out_2;
            Unity_Multiply_float(_Multiply_e1f0c6fe901e4bbfbab0d2c4205e2b6a_Out_2, _Property_5379430f18774410a10b2b9f5321b39a_Out_0, _Multiply_888b1467adda4954929daabfa3e6cb09_Out_2);
            UnityTexture2D _Property_6a9a826df40a48b7963b0e08bbcff55e_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0ae3ff0bb764425ca85d0ce1932ffd5e);
            float4 _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6a9a826df40a48b7963b0e08bbcff55e_Out_0.tex, _Property_6a9a826df40a48b7963b0e08bbcff55e_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_R_4 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.r;
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_G_5 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.g;
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_B_6 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.b;
            float _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_A_7 = _SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0.a;
            float _Property_686b42305a584ab98d7d2ceb9f69379b_Out_0 = Vector1_2713f090f3064018affc5274030ff792;
            float _Property_f336cc430f6744e48ce945586dd4cf91_Out_0 = Vector1_ca470111a100494b95feebccb7eff930;
            float _Multiply_1613626b750e4443a3ebe21c424cb35e_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_f336cc430f6744e48ce945586dd4cf91_Out_0, _Multiply_1613626b750e4443a3ebe21c424cb35e_Out_2);
            float2 _Rotate_0f8e905ead2f479bb6226c8f6d0e2296_Out_3;
            Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1613626b750e4443a3ebe21c424cb35e_Out_2, _Rotate_0f8e905ead2f479bb6226c8f6d0e2296_Out_3);
            float _SimpleNoise_248df111e75f454e87c06ed1c5d6ec34_Out_2;
            Unity_SimpleNoise_float(_Rotate_0f8e905ead2f479bb6226c8f6d0e2296_Out_3, 20, _SimpleNoise_248df111e75f454e87c06ed1c5d6ec34_Out_2);
            float _Multiply_02c15b457b6043779be1bc9dac618a12_Out_2;
            Unity_Multiply_float(_Property_686b42305a584ab98d7d2ceb9f69379b_Out_0, _SimpleNoise_248df111e75f454e87c06ed1c5d6ec34_Out_2, _Multiply_02c15b457b6043779be1bc9dac618a12_Out_2);
            float4 _Multiply_70df71f22b634294a00019956ac8759e_Out_2;
            Unity_Multiply_float(_SampleTexture2D_e510b6d3e1ab4c9db449ef907f091e61_RGBA_0, (_Multiply_02c15b457b6043779be1bc9dac618a12_Out_2.xxxx), _Multiply_70df71f22b634294a00019956ac8759e_Out_2);
            UnityTexture2D _Property_ec839c8b3ef9405b9b2bf4858a73ba1a_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ec839c8b3ef9405b9b2bf4858a73ba1a_Out_0.tex, _Property_ec839c8b3ef9405b9b2bf4858a73ba1a_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_R_4 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.r;
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_G_5 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.g;
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_B_6 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.b;
            float _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_A_7 = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0.a;
            float4 _Add_7573fa5e42d944a3ace4920e27fad63c_Out_2;
            Unity_Add_float4(_Multiply_70df71f22b634294a00019956ac8759e_Out_2, _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_RGBA_0, _Add_7573fa5e42d944a3ace4920e27fad63c_Out_2);
            float4 _Add_bd267390ca20438196a7db5bb3aaf878_Out_2;
            Unity_Add_float4(_Multiply_888b1467adda4954929daabfa3e6cb09_Out_2, _Add_7573fa5e42d944a3ace4920e27fad63c_Out_2, _Add_bd267390ca20438196a7db5bb3aaf878_Out_2);
            float4 _Add_9daa87b921644b859b4c79c3851cf809_Out_2;
            Unity_Add_float4((_SampleTexture2D_f9fb53164c504f6791269ace2792a54b_A_7.xxxx), _Add_bd267390ca20438196a7db5bb3aaf878_Out_2, _Add_9daa87b921644b859b4c79c3851cf809_Out_2);
            surface.BaseColor = (_Add_9daa87b921644b859b4c79c3851cf809_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_36d156b327ab40a0815149fa5d9aefa0_A_7;
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