Shader "Shader Graphs/sky winstreak gen"
{
    Properties
    {
        [NoScaleOffset]_Texture2D("Texture2D", 2D) = "white" {}
        _tiling("tiling", Vector) = (0, 0, 0, 0)
        _speed("speed", Float) = 0
        _sub_speed("sub speed", Float) = 0
        _per("per", Range(0, 1)) = 0
        _per_convert("per convert", Float) = 0
        _sub_alpha("sub alpha", Range(0, 1)) = 1
        _sub_offset_vertical("sub offset vertical", Float) = 0
        _offset_time("offset time", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
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
        ZTest LEqual
        ZWrite Off
        
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
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
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
             float3 positionWS;
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
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
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
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
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
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
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
        float4 _Texture2D_TexelSize;
        float2 _tiling;
        float _sub_speed;
        float _per;
        float _speed;
        float _per_convert;
        float _sub_alpha;
        float _sub_offset_vertical;
        float _offset_time;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
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
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture2D);
            float2 _Property_a0bfd0861fb549a8a29cae2a03fd553c_Out_0_Vector2 = _tiling;
            float _Property_79e89e3587524f19b8725034a4a06b25_Out_0_Float = _speed;
            float _Property_bcb8c4c5f8d940e98ae7d86d6f11ad3a_Out_0_Float = _offset_time;
            float _Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float;
            Unity_Add_float(IN.TimeParameters.x, _Property_bcb8c4c5f8d940e98ae7d86d6f11ad3a_Out_0_Float, _Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float);
            float _Sine_8bb29ac1b7cb4d77a7568ce210f0f35f_Out_1_Float;
            Unity_Sine_float(_Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float, _Sine_8bb29ac1b7cb4d77a7568ce210f0f35f_Out_1_Float);
            float _Property_7fbd5f05fe1b4fada351c0eef97d8fdc_Out_0_Float = _sub_speed;
            float _Multiply_0e0e344bae9b4ff2b0ce92ee74450442_Out_2_Float;
            Unity_Multiply_float_float(_Sine_8bb29ac1b7cb4d77a7568ce210f0f35f_Out_1_Float, _Property_7fbd5f05fe1b4fada351c0eef97d8fdc_Out_0_Float, _Multiply_0e0e344bae9b4ff2b0ce92ee74450442_Out_2_Float);
            float _Add_dde30242def742be859a98b53f88cceb_Out_2_Float;
            Unity_Add_float(_Multiply_0e0e344bae9b4ff2b0ce92ee74450442_Out_2_Float, _Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float, _Add_dde30242def742be859a98b53f88cceb_Out_2_Float);
            float _Multiply_edb01659d1734d1faf11f3615fc4e950_Out_2_Float;
            Unity_Multiply_float_float(_Property_79e89e3587524f19b8725034a4a06b25_Out_0_Float, _Add_dde30242def742be859a98b53f88cceb_Out_2_Float, _Multiply_edb01659d1734d1faf11f3615fc4e950_Out_2_Float);
            float _Property_72062bcf9a6e4b0891bbbf7b9ae676c5_Out_0_Float = _per;
            float _Property_587bfb4295ac41e396125825e8ead931_Out_0_Float = _per_convert;
            float _Multiply_1dec6782a6e0404ca5fc11848ab087b7_Out_2_Float;
            Unity_Multiply_float_float(_Property_72062bcf9a6e4b0891bbbf7b9ae676c5_Out_0_Float, _Property_587bfb4295ac41e396125825e8ead931_Out_0_Float, _Multiply_1dec6782a6e0404ca5fc11848ab087b7_Out_2_Float);
            float _Cosine_371c800365294a85b601ffe1f2f7ccb8_Out_1_Float;
            Unity_Cosine_float(_Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float, _Cosine_371c800365294a85b601ffe1f2f7ccb8_Out_1_Float);
            float _Property_a716539e3d08462c838846238fcc468c_Out_0_Float = _sub_offset_vertical;
            float _Multiply_f9ccb32bd3cc4fcc8f04731560d24242_Out_2_Float;
            Unity_Multiply_float_float(_Cosine_371c800365294a85b601ffe1f2f7ccb8_Out_1_Float, _Property_a716539e3d08462c838846238fcc468c_Out_0_Float, _Multiply_f9ccb32bd3cc4fcc8f04731560d24242_Out_2_Float);
            float _Add_dbc83b4a55f64028a28e5688a9c13017_Out_2_Float;
            Unity_Add_float(_Multiply_1dec6782a6e0404ca5fc11848ab087b7_Out_2_Float, _Multiply_f9ccb32bd3cc4fcc8f04731560d24242_Out_2_Float, _Add_dbc83b4a55f64028a28e5688a9c13017_Out_2_Float);
            float2 _Vector2_0a3aca61048b46c1823e282c7fdd4bc8_Out_0_Vector2 = float2(_Multiply_edb01659d1734d1faf11f3615fc4e950_Out_2_Float, _Add_dbc83b4a55f64028a28e5688a9c13017_Out_2_Float);
            float2 _TilingAndOffset_964605a5148e4410953364af630f7548_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_a0bfd0861fb549a8a29cae2a03fd553c_Out_0_Vector2, _Vector2_0a3aca61048b46c1823e282c7fdd4bc8_Out_0_Vector2, _TilingAndOffset_964605a5148e4410953364af630f7548_Out_3_Vector2);
            float4 _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D.tex, _Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D.samplerstate, _Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_964605a5148e4410953364af630f7548_Out_3_Vector2) );
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_R_4_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.r;
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_G_5_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.g;
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_B_6_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.b;
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_A_7_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.a;
            float _Property_c95acf0ef38243f4866927a3adcbe3d0_Out_0_Float = _sub_alpha;
            float _Multiply_9c6963bd2bc243be9c7f5bc9d2daef37_Out_2_Float;
            Unity_Multiply_float_float(_Property_c95acf0ef38243f4866927a3adcbe3d0_Out_0_Float, _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_A_7_Float, _Multiply_9c6963bd2bc243be9c7f5bc9d2daef37_Out_2_Float);
            surface.BaseColor = (_SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.xyz);
            surface.Alpha = _Multiply_9c6963bd2bc243be9c7f5bc9d2daef37_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
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
        ZTest LEqual
        ZWrite Off
        
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
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
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
             float3 positionWS;
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
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
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
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
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
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
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
        float4 _Texture2D_TexelSize;
        float2 _tiling;
        float _sub_speed;
        float _per;
        float _speed;
        float _per_convert;
        float _sub_alpha;
        float _sub_offset_vertical;
        float _offset_time;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
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
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture2D);
            float2 _Property_a0bfd0861fb549a8a29cae2a03fd553c_Out_0_Vector2 = _tiling;
            float _Property_79e89e3587524f19b8725034a4a06b25_Out_0_Float = _speed;
            float _Property_bcb8c4c5f8d940e98ae7d86d6f11ad3a_Out_0_Float = _offset_time;
            float _Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float;
            Unity_Add_float(IN.TimeParameters.x, _Property_bcb8c4c5f8d940e98ae7d86d6f11ad3a_Out_0_Float, _Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float);
            float _Sine_8bb29ac1b7cb4d77a7568ce210f0f35f_Out_1_Float;
            Unity_Sine_float(_Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float, _Sine_8bb29ac1b7cb4d77a7568ce210f0f35f_Out_1_Float);
            float _Property_7fbd5f05fe1b4fada351c0eef97d8fdc_Out_0_Float = _sub_speed;
            float _Multiply_0e0e344bae9b4ff2b0ce92ee74450442_Out_2_Float;
            Unity_Multiply_float_float(_Sine_8bb29ac1b7cb4d77a7568ce210f0f35f_Out_1_Float, _Property_7fbd5f05fe1b4fada351c0eef97d8fdc_Out_0_Float, _Multiply_0e0e344bae9b4ff2b0ce92ee74450442_Out_2_Float);
            float _Add_dde30242def742be859a98b53f88cceb_Out_2_Float;
            Unity_Add_float(_Multiply_0e0e344bae9b4ff2b0ce92ee74450442_Out_2_Float, _Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float, _Add_dde30242def742be859a98b53f88cceb_Out_2_Float);
            float _Multiply_edb01659d1734d1faf11f3615fc4e950_Out_2_Float;
            Unity_Multiply_float_float(_Property_79e89e3587524f19b8725034a4a06b25_Out_0_Float, _Add_dde30242def742be859a98b53f88cceb_Out_2_Float, _Multiply_edb01659d1734d1faf11f3615fc4e950_Out_2_Float);
            float _Property_72062bcf9a6e4b0891bbbf7b9ae676c5_Out_0_Float = _per;
            float _Property_587bfb4295ac41e396125825e8ead931_Out_0_Float = _per_convert;
            float _Multiply_1dec6782a6e0404ca5fc11848ab087b7_Out_2_Float;
            Unity_Multiply_float_float(_Property_72062bcf9a6e4b0891bbbf7b9ae676c5_Out_0_Float, _Property_587bfb4295ac41e396125825e8ead931_Out_0_Float, _Multiply_1dec6782a6e0404ca5fc11848ab087b7_Out_2_Float);
            float _Cosine_371c800365294a85b601ffe1f2f7ccb8_Out_1_Float;
            Unity_Cosine_float(_Add_a7ecea558b364c77acbd60256af0f6f2_Out_2_Float, _Cosine_371c800365294a85b601ffe1f2f7ccb8_Out_1_Float);
            float _Property_a716539e3d08462c838846238fcc468c_Out_0_Float = _sub_offset_vertical;
            float _Multiply_f9ccb32bd3cc4fcc8f04731560d24242_Out_2_Float;
            Unity_Multiply_float_float(_Cosine_371c800365294a85b601ffe1f2f7ccb8_Out_1_Float, _Property_a716539e3d08462c838846238fcc468c_Out_0_Float, _Multiply_f9ccb32bd3cc4fcc8f04731560d24242_Out_2_Float);
            float _Add_dbc83b4a55f64028a28e5688a9c13017_Out_2_Float;
            Unity_Add_float(_Multiply_1dec6782a6e0404ca5fc11848ab087b7_Out_2_Float, _Multiply_f9ccb32bd3cc4fcc8f04731560d24242_Out_2_Float, _Add_dbc83b4a55f64028a28e5688a9c13017_Out_2_Float);
            float2 _Vector2_0a3aca61048b46c1823e282c7fdd4bc8_Out_0_Vector2 = float2(_Multiply_edb01659d1734d1faf11f3615fc4e950_Out_2_Float, _Add_dbc83b4a55f64028a28e5688a9c13017_Out_2_Float);
            float2 _TilingAndOffset_964605a5148e4410953364af630f7548_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_a0bfd0861fb549a8a29cae2a03fd553c_Out_0_Vector2, _Vector2_0a3aca61048b46c1823e282c7fdd4bc8_Out_0_Vector2, _TilingAndOffset_964605a5148e4410953364af630f7548_Out_3_Vector2);
            float4 _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D.tex, _Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D.samplerstate, _Property_f52805aafd0b495cb713dae59767bd89_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_964605a5148e4410953364af630f7548_Out_3_Vector2) );
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_R_4_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.r;
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_G_5_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.g;
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_B_6_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.b;
            float _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_A_7_Float = _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.a;
            float _Property_c95acf0ef38243f4866927a3adcbe3d0_Out_0_Float = _sub_alpha;
            float _Multiply_9c6963bd2bc243be9c7f5bc9d2daef37_Out_2_Float;
            Unity_Multiply_float_float(_Property_c95acf0ef38243f4866927a3adcbe3d0_Out_0_Float, _SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_A_7_Float, _Multiply_9c6963bd2bc243be9c7f5bc9d2daef37_Out_2_Float);
            surface.BaseColor = (_SampleTexture2D_3ae7e429a45346318cf235fe2f4fc968_RGBA_0_Vector4.xyz);
            surface.Alpha = _Multiply_9c6963bd2bc243be9c7f5bc9d2daef37_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}