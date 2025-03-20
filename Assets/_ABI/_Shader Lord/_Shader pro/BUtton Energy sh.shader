Shader "URP/Button Energy"
{
    Properties
    {
        Vector1_d767694738d14c4ca3a4f6a4a6a56440("speed", Float) = 0
        [NoScaleOffset]Texture2D_838adeddf92a4f2ab1ccb7fe2284f737("Ener TEx", 2D) = "white" {}
        [HDR]Color_65262b63e15c43a08d9b42a634126ca8("Color", Color) = (2, 2, 2, 0)
        Vector1_6b962cf308254be7ba14c2d1c434a855("step", Range(0, 1)) = 0
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
        float Vector1_d767694738d14c4ca3a4f6a4a6a56440;
        float4 Texture2D_838adeddf92a4f2ab1ccb7fe2284f737_TexelSize;
        float4 Color_65262b63e15c43a08d9b42a634126ca8;
        float Vector1_6b962cf308254be7ba14c2d1c434a855;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(Texture2D_838adeddf92a4f2ab1ccb7fe2284f737);
        SAMPLER(samplerTexture2D_838adeddf92a4f2ab1ccb7fe2284f737);

            // Graph Functions
            
        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Step_float4(float4 Edge, float4 In, out float4 Out)
        {
            Out = step(Edge, In);
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
            UnityTexture2D _Property_bcf4dc61132d4ad79c11697286229a3d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0 = SAMPLE_TEXTURE2D(_Property_bcf4dc61132d4ad79c11697286229a3d_Out_0.tex, _Property_bcf4dc61132d4ad79c11697286229a3d_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_R_4 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.r;
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_G_5 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.g;
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_B_6 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.b;
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_A_7 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.a;
            float _Property_bdd28ff7031648bf9ec8b9348894d5fd_Out_0 = Vector1_6b962cf308254be7ba14c2d1c434a855;
            UnityTexture2D _Property_c0dcdaf028cc47cf9a7ff94d6eef8594_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_838adeddf92a4f2ab1ccb7fe2284f737);
            float _Property_bb6188da96fd40bb8a0d1cec62df9604_Out_0 = Vector1_d767694738d14c4ca3a4f6a4a6a56440;
            float _Multiply_03331c3679694b40a6dd8c89d061f358_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_bb6188da96fd40bb8a0d1cec62df9604_Out_0, _Multiply_03331c3679694b40a6dd8c89d061f358_Out_2);
            float2 _TilingAndOffset_8e3d6ee0b6194ac0a4b51dcc6690919f_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 0.2), (_Multiply_03331c3679694b40a6dd8c89d061f358_Out_2.xx), _TilingAndOffset_8e3d6ee0b6194ac0a4b51dcc6690919f_Out_3);
            float4 _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c0dcdaf028cc47cf9a7ff94d6eef8594_Out_0.tex, _Property_c0dcdaf028cc47cf9a7ff94d6eef8594_Out_0.samplerstate, _TilingAndOffset_8e3d6ee0b6194ac0a4b51dcc6690919f_Out_3);
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_R_4 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.r;
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_G_5 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.g;
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_B_6 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.b;
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_A_7 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.a;
            float4 _Step_5cf35293262547d089cae7b571f28b85_Out_2;
            Unity_Step_float4((_Property_bdd28ff7031648bf9ec8b9348894d5fd_Out_0.xxxx), _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0, _Step_5cf35293262547d089cae7b571f28b85_Out_2);
            float4 _Property_71c917ba76f040c28fa757c1a62b006a_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_65262b63e15c43a08d9b42a634126ca8) : Color_65262b63e15c43a08d9b42a634126ca8;
            float4 _Multiply_6c6ade0b85fb467a9b0fdea6bc122c52_Out_2;
            Unity_Multiply_float(_Step_5cf35293262547d089cae7b571f28b85_Out_2, _Property_71c917ba76f040c28fa757c1a62b006a_Out_0, _Multiply_6c6ade0b85fb467a9b0fdea6bc122c52_Out_2);
            float4 _Add_d9e9be35571d4677b70eb89107a8088a_Out_2;
            Unity_Add_float4(_SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0, _Multiply_6c6ade0b85fb467a9b0fdea6bc122c52_Out_2, _Add_d9e9be35571d4677b70eb89107a8088a_Out_2);
            surface.BaseColor = (_Add_d9e9be35571d4677b70eb89107a8088a_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_A_7;
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
        float Vector1_d767694738d14c4ca3a4f6a4a6a56440;
        float4 Texture2D_838adeddf92a4f2ab1ccb7fe2284f737_TexelSize;
        float4 Color_65262b63e15c43a08d9b42a634126ca8;
        float Vector1_6b962cf308254be7ba14c2d1c434a855;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(Texture2D_838adeddf92a4f2ab1ccb7fe2284f737);
        SAMPLER(samplerTexture2D_838adeddf92a4f2ab1ccb7fe2284f737);

            // Graph Functions
            
        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Step_float4(float4 Edge, float4 In, out float4 Out)
        {
            Out = step(Edge, In);
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
            UnityTexture2D _Property_bcf4dc61132d4ad79c11697286229a3d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0 = SAMPLE_TEXTURE2D(_Property_bcf4dc61132d4ad79c11697286229a3d_Out_0.tex, _Property_bcf4dc61132d4ad79c11697286229a3d_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_R_4 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.r;
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_G_5 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.g;
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_B_6 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.b;
            float _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_A_7 = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0.a;
            float _Property_bdd28ff7031648bf9ec8b9348894d5fd_Out_0 = Vector1_6b962cf308254be7ba14c2d1c434a855;
            UnityTexture2D _Property_c0dcdaf028cc47cf9a7ff94d6eef8594_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_838adeddf92a4f2ab1ccb7fe2284f737);
            float _Property_bb6188da96fd40bb8a0d1cec62df9604_Out_0 = Vector1_d767694738d14c4ca3a4f6a4a6a56440;
            float _Multiply_03331c3679694b40a6dd8c89d061f358_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_bb6188da96fd40bb8a0d1cec62df9604_Out_0, _Multiply_03331c3679694b40a6dd8c89d061f358_Out_2);
            float2 _TilingAndOffset_8e3d6ee0b6194ac0a4b51dcc6690919f_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 0.2), (_Multiply_03331c3679694b40a6dd8c89d061f358_Out_2.xx), _TilingAndOffset_8e3d6ee0b6194ac0a4b51dcc6690919f_Out_3);
            float4 _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c0dcdaf028cc47cf9a7ff94d6eef8594_Out_0.tex, _Property_c0dcdaf028cc47cf9a7ff94d6eef8594_Out_0.samplerstate, _TilingAndOffset_8e3d6ee0b6194ac0a4b51dcc6690919f_Out_3);
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_R_4 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.r;
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_G_5 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.g;
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_B_6 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.b;
            float _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_A_7 = _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0.a;
            float4 _Step_5cf35293262547d089cae7b571f28b85_Out_2;
            Unity_Step_float4((_Property_bdd28ff7031648bf9ec8b9348894d5fd_Out_0.xxxx), _SampleTexture2D_a2cf9ea597664ccaa3571cf6e28fa425_RGBA_0, _Step_5cf35293262547d089cae7b571f28b85_Out_2);
            float4 _Property_71c917ba76f040c28fa757c1a62b006a_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_65262b63e15c43a08d9b42a634126ca8) : Color_65262b63e15c43a08d9b42a634126ca8;
            float4 _Multiply_6c6ade0b85fb467a9b0fdea6bc122c52_Out_2;
            Unity_Multiply_float(_Step_5cf35293262547d089cae7b571f28b85_Out_2, _Property_71c917ba76f040c28fa757c1a62b006a_Out_0, _Multiply_6c6ade0b85fb467a9b0fdea6bc122c52_Out_2);
            float4 _Add_d9e9be35571d4677b70eb89107a8088a_Out_2;
            Unity_Add_float4(_SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_RGBA_0, _Multiply_6c6ade0b85fb467a9b0fdea6bc122c52_Out_2, _Add_d9e9be35571d4677b70eb89107a8088a_Out_2);
            surface.BaseColor = (_Add_d9e9be35571d4677b70eb89107a8088a_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_9f80b9aacb0d48919f481e6703ccf2aa_A_7;
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