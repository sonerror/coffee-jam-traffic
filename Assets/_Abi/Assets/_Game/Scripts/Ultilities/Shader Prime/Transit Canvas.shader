Shader "Custom/Transit Canvas"
{
    Properties
    {
 _Color ("COlor",Color) = (0,0,0,0)
 _MainTex("Tex", 2D) = "" {}
        _MaskTex("Mask Tex",2D) = ""{}
        _Float ("FLOAT", Range(0,1.78)) = 0
        _Float2("NEGAte",Float)  =1 // 0||1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
Blend SrcAlpha OneMinusSrcAlpha

Tags{"Queue" = "Transparent" "RenderType" = "Transparent" }
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 subUv : TEXCOORD1;
            };
            sampler2D _MainTex;
            sampler2D _MaskTex;
               float4 _MaskTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MaskTex);
                o.subUv = v.uv;
                return o;
            }

            fixed4 _Color;
fixed _Float;
            fixed _Float2;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MaskTex,i.uv);
                fixed uvy = lerp(i.subUv.y,1-i.subUv.y,_Float2);
                fixed a = smoothstep(_Float-.78,_Float,floor(uvy/(1/_MaskTex_ST.y))*(1/_MaskTex_ST.y));
                a = step(a,col.r);
                // col =col* _Color;
                col = tex2D(_MainTex,i.subUv);
                col.a = a;
                return col;
            }
            ENDCG
        }
    }
}
