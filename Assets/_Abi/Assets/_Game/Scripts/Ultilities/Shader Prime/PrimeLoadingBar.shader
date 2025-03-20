Shader "Custom/ PrimeLaodingBAr"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Float ("Float",Range(0,1)) = 0
        _Offset ("Offset",Range(0,.2)) = .052
        _Stretch("Stretch",Range(0.5,1.5)) = 0
    }
    SubShader
    {
      Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
//        ZTest Always
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Float;
            float _Offset;
            float _Stretch;

            fixed4 frag (v2f i) : SV_Target
            {
                float f  = _Offset*_Stretch + _Float * (1-_Offset*_Stretch *2);

                    if(i.uv.x<_Offset * _Stretch)
                    {
                        i.uv.x = i.uv.x / _Stretch;
                    }
                    else if(i.uv.x<f)
                    {
                         // i.uv.x = .5f;
                    }
                    else if(i.uv.x<(f+_Offset*_Stretch))
                    {
                        i.uv.x =  (i.uv.x  + (1-_Offset*_Stretch - f)-1)/_Stretch + 1;
                    }
                    else
                    {
                        return fixed4(0,0,0,0);
                    }
                
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                return col;
            }
            ENDCG
        }
    }
}
