Shader "Unlit/DrawPen_Drawablew"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DirtWidth("DirtWidth", Float) = 1
        _DirtHeight("DirtHeight", Float) = 1
        _BrushWidth("BrushWidth", Float) = 1
        _BrushHeight("BrushHeight", Float) = 1
        _Pos("Pos", Vector) = (0.1, 0.1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue" = "Transparent"}
        LOD 100
        
ZWrite Off
        Blend SrcAlpha DstAlpha  
       BlendOp Min

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
            };

            sampler2D _MainTex;
          //  float4 _MainTex_ST;
                    float _DirtWidth;
        float _DirtHeight;
        float _BrushWidth;
        float _BrushHeight;
        float2 _Pos;


            float2 GetUvPos(float2 pos, float dirtWidth, float dirtHeight,float brushWidth, float brushHeight,float2 ObjectSize, float2 vertPos)
            {
            #if UNITY_UV_STARTS_AT_TOP
                float2   uvPos = float2(pos.x/dirtWidth*2-1 + (vertPos.x/ObjectSize.x)*brushWidth/dirtWidth,
               (1-  pos.y/dirtHeight)*2-1 + (vertPos.y/ObjectSize.y)*brushHeight/dirtHeight);
                #else
                float2   uvPos = float2(pos.x/dirtWidth*2-1 + (vertPos.x/ObjectSize.x)*brushWidth/dirtWidth,
                pos.y/dirtHeight*2-1 + (vertPos.y/ObjectSize.y)*brushHeight/dirtHeight);
                #endif
                return uvPos;
            }


            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.vertex = v.vertex;
                // o.vertex.x = o.vertex.x*-1;
              o.vertex.z = 1;
             o.vertex.xy = GetUvPos(_Pos,_DirtWidth,_DirtHeight,_BrushWidth,_BrushHeight,float2(1,1),v.vertex.xy);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
            fixed4 o = tex2D(_MainTex,i.uv);
               o.a =o.r; //temp for check alpha with r channel
                return o;
            }
            ENDCG
        }
    }
}
