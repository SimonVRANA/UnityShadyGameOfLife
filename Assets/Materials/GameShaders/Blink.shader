Shader "Custom/Blink"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AliveColor ("AliveColor", Color) = (1,1,1,1)
        _DeadColor ("DeadColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            float4 _MainTex_TexelSize;
            float4 _AliveColor;
            float4 _DeadColor;


            bool IsAlive(float4 color)
            {
                return distance(color, _AliveColor) < 0.01;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = 1.0 / _ScreenParams.xy;

                
                float4 currentColor= tex2D(_MainTex, i.uv);

                if(IsAlive(currentColor))
                {
                    return _DeadColor;
                }

                return _AliveColor;
            }
            ENDCG
        }
    }
}
