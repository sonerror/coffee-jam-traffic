Shader "URP/Shine button"
{
    Properties
    {
        [NoScaleOffset]Texture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343("Shine TEx\", 2D) = "white" {}
        Vector1_c51fd93d61da46cfb5857f486141eb9b("speed", Float) = -5.12
        [HDR]Color_e506ccf8374d45b685658cd0cfe3f181("Color", Color) = (8.693878, 8.693878, 8.693878, 0)
        Vector1_87c5fd85b8f5402691f5bae7693baec9("offset", Int) = 5
        Vector1_44ffefa5251e4e12be819b1e97711424("delay", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

//
        _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15
        //
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
        //
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
        //

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
        float4 Texture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343_TexelSize;
        float Vector1_c51fd93d61da46cfb5857f486141eb9b;
        float4 Color_e506ccf8374d45b685658cd0cfe3f181;
        float Vector1_87c5fd85b8f5402691f5bae7693baec9;
        float Vector1_44ffefa5251e4e12be819b1e97711424;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Clamp);
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(Texture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343);
        SAMPLER(samplerTexture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343);

            // Graph Functions
            
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
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
            UnityTexture2D _Property_f5688476b2984e5987728a4f42e0ebe6_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343);
            float4 _UV_90a1f0d6355048ad92659b95148b9f4f_Out_0 = IN.uv0;
            float _Property_4a97b85b09a0489284d222e835c5cd22_Out_0 = Vector1_44ffefa5251e4e12be819b1e97711424;
            float _Add_53c3ddedfb664cb68d366eb52551544c_Out_2;
            Unity_Add_float(_Property_4a97b85b09a0489284d222e835c5cd22_Out_0, IN.TimeParameters.x, _Add_53c3ddedfb664cb68d366eb52551544c_Out_2);
            float _Property_30f47af16a1a41c4aea1ded9af76746a_Out_0 = Vector1_87c5fd85b8f5402691f5bae7693baec9;
            float _Modulo_93880cce1af54fd3a3bfe56b294afc52_Out_2;
            Unity_Modulo_float(_Add_53c3ddedfb664cb68d366eb52551544c_Out_2, _Property_30f47af16a1a41c4aea1ded9af76746a_Out_0, _Modulo_93880cce1af54fd3a3bfe56b294afc52_Out_2);
            float _Property_4f255ab4ef654c46858265818174308d_Out_0 = Vector1_c51fd93d61da46cfb5857f486141eb9b;
            float _Multiply_4e907834d2e54d48968d5bb5a9fdd09b_Out_2;
            Unity_Multiply_float(_Modulo_93880cce1af54fd3a3bfe56b294afc52_Out_2, _Property_4f255ab4ef654c46858265818174308d_Out_0, _Multiply_4e907834d2e54d48968d5bb5a9fdd09b_Out_2);
            float2 _Vector2_2b42bc323c454e758daaf7512d6f562b_Out_0 = float2(_Multiply_4e907834d2e54d48968d5bb5a9fdd09b_Out_2, 0);
            float2 _Add_cc1091cbad5f4496a075a7cf1d2b04e8_Out_2;
            Unity_Add_float2(_Vector2_2b42bc323c454e758daaf7512d6f562b_Out_0, float2(1, 0), _Add_cc1091cbad5f4496a075a7cf1d2b04e8_Out_2);
            float2 _Add_a52ed581cb6a426493d0ce81215f3341_Out_2;
            Unity_Add_float2((_UV_90a1f0d6355048ad92659b95148b9f4f_Out_0.xy), _Add_cc1091cbad5f4496a075a7cf1d2b04e8_Out_2, _Add_a52ed581cb6a426493d0ce81215f3341_Out_2);
            float4 _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0 = SAMPLE_TEXTURE2D(_Property_f5688476b2984e5987728a4f42e0ebe6_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Linear_Clamp).samplerstate, _Add_a52ed581cb6a426493d0ce81215f3341_Out_2);
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_R_4 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.r;
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_G_5 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.g;
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_B_6 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.b;
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_A_7 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.a;
            float4 _Property_81b2f31b269f446087dcd70784e724ff_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_e506ccf8374d45b685658cd0cfe3f181) : Color_e506ccf8374d45b685658cd0cfe3f181;
            float4 _Multiply_3b13734c1e2d43b3aa8228345a246f43_Out_2;
            Unity_Multiply_float((_SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_A_7.xxxx), _Property_81b2f31b269f446087dcd70784e724ff_Out_0, _Multiply_3b13734c1e2d43b3aa8228345a246f43_Out_2);
            UnityTexture2D _Property_4882a15220084894a9d3d7961574734c_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4882a15220084894a9d3d7961574734c_Out_0.tex, _Property_4882a15220084894a9d3d7961574734c_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_R_4 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.r;
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_G_5 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.g;
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_B_6 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.b;
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_A_7 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.a;
            float4 _Add_11dc79116391475981c0de99e73602b0_Out_2;
            Unity_Add_float4(_Multiply_3b13734c1e2d43b3aa8228345a246f43_Out_2, _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0, _Add_11dc79116391475981c0de99e73602b0_Out_2);
            surface.BaseColor = (_Add_11dc79116391475981c0de99e73602b0_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_A_7;
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
        float4 Texture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343_TexelSize;
        float Vector1_c51fd93d61da46cfb5857f486141eb9b;
        float4 Color_e506ccf8374d45b685658cd0cfe3f181;
        float Vector1_87c5fd85b8f5402691f5bae7693baec9;
        float Vector1_44ffefa5251e4e12be819b1e97711424;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Clamp);
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(Texture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343);
        SAMPLER(samplerTexture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343);

            // Graph Functions
            
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
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
            UnityTexture2D _Property_f5688476b2984e5987728a4f42e0ebe6_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_2d2b5fa8dd374cdfa2a2cbcbf2545343);
            float4 _UV_90a1f0d6355048ad92659b95148b9f4f_Out_0 = IN.uv0;
            float _Property_4a97b85b09a0489284d222e835c5cd22_Out_0 = Vector1_44ffefa5251e4e12be819b1e97711424;
            float _Add_53c3ddedfb664cb68d366eb52551544c_Out_2;
            Unity_Add_float(_Property_4a97b85b09a0489284d222e835c5cd22_Out_0, IN.TimeParameters.x, _Add_53c3ddedfb664cb68d366eb52551544c_Out_2);
            float _Property_30f47af16a1a41c4aea1ded9af76746a_Out_0 = Vector1_87c5fd85b8f5402691f5bae7693baec9;
            float _Modulo_93880cce1af54fd3a3bfe56b294afc52_Out_2;
            Unity_Modulo_float(_Add_53c3ddedfb664cb68d366eb52551544c_Out_2, _Property_30f47af16a1a41c4aea1ded9af76746a_Out_0, _Modulo_93880cce1af54fd3a3bfe56b294afc52_Out_2);
            float _Property_4f255ab4ef654c46858265818174308d_Out_0 = Vector1_c51fd93d61da46cfb5857f486141eb9b;
            float _Multiply_4e907834d2e54d48968d5bb5a9fdd09b_Out_2;
            Unity_Multiply_float(_Modulo_93880cce1af54fd3a3bfe56b294afc52_Out_2, _Property_4f255ab4ef654c46858265818174308d_Out_0, _Multiply_4e907834d2e54d48968d5bb5a9fdd09b_Out_2);
            float2 _Vector2_2b42bc323c454e758daaf7512d6f562b_Out_0 = float2(_Multiply_4e907834d2e54d48968d5bb5a9fdd09b_Out_2, 0);
            float2 _Add_cc1091cbad5f4496a075a7cf1d2b04e8_Out_2;
            Unity_Add_float2(_Vector2_2b42bc323c454e758daaf7512d6f562b_Out_0, float2(1, 0), _Add_cc1091cbad5f4496a075a7cf1d2b04e8_Out_2);
            float2 _Add_a52ed581cb6a426493d0ce81215f3341_Out_2;
            Unity_Add_float2((_UV_90a1f0d6355048ad92659b95148b9f4f_Out_0.xy), _Add_cc1091cbad5f4496a075a7cf1d2b04e8_Out_2, _Add_a52ed581cb6a426493d0ce81215f3341_Out_2);
            float4 _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0 = SAMPLE_TEXTURE2D(_Property_f5688476b2984e5987728a4f42e0ebe6_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Linear_Clamp).samplerstate, _Add_a52ed581cb6a426493d0ce81215f3341_Out_2);
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_R_4 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.r;
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_G_5 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.g;
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_B_6 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.b;
            float _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_A_7 = _SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_RGBA_0.a;
            float4 _Property_81b2f31b269f446087dcd70784e724ff_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_e506ccf8374d45b685658cd0cfe3f181) : Color_e506ccf8374d45b685658cd0cfe3f181;
            float4 _Multiply_3b13734c1e2d43b3aa8228345a246f43_Out_2;
            Unity_Multiply_float((_SampleTexture2D_83aeb0692d004e1784b7a4f23f6261ae_A_7.xxxx), _Property_81b2f31b269f446087dcd70784e724ff_Out_0, _Multiply_3b13734c1e2d43b3aa8228345a246f43_Out_2);
            UnityTexture2D _Property_4882a15220084894a9d3d7961574734c_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4882a15220084894a9d3d7961574734c_Out_0.tex, _Property_4882a15220084894a9d3d7961574734c_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_R_4 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.r;
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_G_5 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.g;
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_B_6 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.b;
            float _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_A_7 = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0.a;
            float4 _Add_11dc79116391475981c0de99e73602b0_Out_2;
            Unity_Add_float4(_Multiply_3b13734c1e2d43b3aa8228345a246f43_Out_2, _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_RGBA_0, _Add_11dc79116391475981c0de99e73602b0_Out_2);
            surface.BaseColor = (_Add_11dc79116391475981c0de99e73602b0_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_3759dc4a48b7489f88e64ef29d16a5d2_A_7;
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