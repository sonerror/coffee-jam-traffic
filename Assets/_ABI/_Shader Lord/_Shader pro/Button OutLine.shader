Shader "URP/Button Outline"
{
    Properties
    {
        Vector1_b918494f881b490784b1d6aae334953f("str", Float) = 0.95
        [HDR]Color_b86217c8fdca4d79a0f4d6838bb2ec9d("Color", Color) = (1, 0, 0, 0)
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
        float Vector1_b918494f881b490784b1d6aae334953f;
        float4 Color_b86217c8fdca4d79a0f4d6838bb2ec9d;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

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
            UnityTexture2D _Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_4c72475767f5450aafd2fef32428774c_Out_0 = Vector1_b918494f881b490784b1d6aae334953f;
            float _OneMinus_3bd2c71958fa4fd496d497d94350b72d_Out_1;
            Unity_OneMinus_float(_Property_4c72475767f5450aafd2fef32428774c_Out_0, _OneMinus_3bd2c71958fa4fd496d497d94350b72d_Out_1);
            float _Multiply_22ffad8020c04b9795aa6136e778ce49_Out_2;
            Unity_Multiply_float(0.5, _OneMinus_3bd2c71958fa4fd496d497d94350b72d_Out_1, _Multiply_22ffad8020c04b9795aa6136e778ce49_Out_2);
            float2 _TilingAndOffset_84c6b501271342d5bfae1d87781ee668_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_4c72475767f5450aafd2fef32428774c_Out_0.xx), (_Multiply_22ffad8020c04b9795aa6136e778ce49_Out_2.xx), _TilingAndOffset_84c6b501271342d5bfae1d87781ee668_Out_3);
            float4 _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.tex, _Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.samplerstate, _TilingAndOffset_84c6b501271342d5bfae1d87781ee668_Out_3);
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_R_4 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.r;
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_G_5 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.g;
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_B_6 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.b;
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_A_7 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.a;
            float4 _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.tex, _Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_R_4 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.r;
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_G_5 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.g;
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_B_6 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.b;
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_A_7 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.a;
            float _Subtract_52261767f37e4fb2acfaa41561586df1_Out_2;
            Unity_Subtract_float(_SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_A_7, _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_A_7, _Subtract_52261767f37e4fb2acfaa41561586df1_Out_2);
            float _Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1;
            Unity_Saturate_float(_Subtract_52261767f37e4fb2acfaa41561586df1_Out_2, _Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1);
            float4 _Property_9f10938ce1634ad39c0b87f25fc542f9_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_b86217c8fdca4d79a0f4d6838bb2ec9d) : Color_b86217c8fdca4d79a0f4d6838bb2ec9d;
            float4 _Multiply_4d674f69eb4044d7868c11057cdab032_Out_2;
            Unity_Multiply_float((_Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1.xxxx), _Property_9f10938ce1634ad39c0b87f25fc542f9_Out_0, _Multiply_4d674f69eb4044d7868c11057cdab032_Out_2);
            float4 _Add_ff6123c0e81047dbae9c997734e66bf8_Out_2;
            Unity_Add_float4(_Multiply_4d674f69eb4044d7868c11057cdab032_Out_2, _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0, _Add_ff6123c0e81047dbae9c997734e66bf8_Out_2);
            float _Add_0beba5fa984c4f0288e1a431d58b2ce8_Out_2;
            Unity_Add_float(_Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1, _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_A_7, _Add_0beba5fa984c4f0288e1a431d58b2ce8_Out_2);
            surface.BaseColor = (_Add_ff6123c0e81047dbae9c997734e66bf8_Out_2.xyz);
            surface.Alpha = _Add_0beba5fa984c4f0288e1a431d58b2ce8_Out_2;
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
        float Vector1_b918494f881b490784b1d6aae334953f;
        float4 Color_b86217c8fdca4d79a0f4d6838bb2ec9d;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;

            // Graph Functions
            
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

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
            UnityTexture2D _Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_4c72475767f5450aafd2fef32428774c_Out_0 = Vector1_b918494f881b490784b1d6aae334953f;
            float _OneMinus_3bd2c71958fa4fd496d497d94350b72d_Out_1;
            Unity_OneMinus_float(_Property_4c72475767f5450aafd2fef32428774c_Out_0, _OneMinus_3bd2c71958fa4fd496d497d94350b72d_Out_1);
            float _Multiply_22ffad8020c04b9795aa6136e778ce49_Out_2;
            Unity_Multiply_float(0.5, _OneMinus_3bd2c71958fa4fd496d497d94350b72d_Out_1, _Multiply_22ffad8020c04b9795aa6136e778ce49_Out_2);
            float2 _TilingAndOffset_84c6b501271342d5bfae1d87781ee668_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, (_Property_4c72475767f5450aafd2fef32428774c_Out_0.xx), (_Multiply_22ffad8020c04b9795aa6136e778ce49_Out_2.xx), _TilingAndOffset_84c6b501271342d5bfae1d87781ee668_Out_3);
            float4 _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.tex, _Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.samplerstate, _TilingAndOffset_84c6b501271342d5bfae1d87781ee668_Out_3);
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_R_4 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.r;
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_G_5 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.g;
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_B_6 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.b;
            float _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_A_7 = _SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_RGBA_0.a;
            float4 _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.tex, _Property_ac5bf7a42b8c40beb55caa048fb42c22_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_R_4 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.r;
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_G_5 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.g;
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_B_6 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.b;
            float _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_A_7 = _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0.a;
            float _Subtract_52261767f37e4fb2acfaa41561586df1_Out_2;
            Unity_Subtract_float(_SampleTexture2D_7a9f52a7d9894fd994c61eadf538554d_A_7, _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_A_7, _Subtract_52261767f37e4fb2acfaa41561586df1_Out_2);
            float _Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1;
            Unity_Saturate_float(_Subtract_52261767f37e4fb2acfaa41561586df1_Out_2, _Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1);
            float4 _Property_9f10938ce1634ad39c0b87f25fc542f9_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_b86217c8fdca4d79a0f4d6838bb2ec9d) : Color_b86217c8fdca4d79a0f4d6838bb2ec9d;
            float4 _Multiply_4d674f69eb4044d7868c11057cdab032_Out_2;
            Unity_Multiply_float((_Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1.xxxx), _Property_9f10938ce1634ad39c0b87f25fc542f9_Out_0, _Multiply_4d674f69eb4044d7868c11057cdab032_Out_2);
            float4 _Add_ff6123c0e81047dbae9c997734e66bf8_Out_2;
            Unity_Add_float4(_Multiply_4d674f69eb4044d7868c11057cdab032_Out_2, _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_RGBA_0, _Add_ff6123c0e81047dbae9c997734e66bf8_Out_2);
            float _Add_0beba5fa984c4f0288e1a431d58b2ce8_Out_2;
            Unity_Add_float(_Saturate_f7aa7139b4a04a159deeaade3fdca603_Out_1, _SampleTexture2D_b0ace295467f4f08a83dd17ed6af902b_A_7, _Add_0beba5fa984c4f0288e1a431d58b2ce8_Out_2);
            surface.BaseColor = (_Add_ff6123c0e81047dbae9c997734e66bf8_Out_2.xyz);
            surface.Alpha = _Add_0beba5fa984c4f0288e1a431d58b2ce8_Out_2;
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