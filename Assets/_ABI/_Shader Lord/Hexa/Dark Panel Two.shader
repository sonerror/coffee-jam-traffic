Shader "Custom/Dark Panel Two"
{
    Properties
    {
        _MainTex("Tex",2D) = "white"{}
   
                _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15
    }
    SubShader
    {
        Tags{"RenderType" = "Transparent"  "RenderQueue" = "Transparent"}
        
//               Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
              ZTest [unity_GUIZTestMode]
//        ZWrite Off

        Stencil
        {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp]
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                        fixed4 color : COLOR;
    };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;

            
            fixed4 frag (v2f i) : SV_Target
            {
fixed4 col = i.color;
                return col;
            }
            ENDCG
        }
    }
}
