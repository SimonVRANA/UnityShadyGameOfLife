Shader "Custom/GameOfLife"
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

            float4 GetColorAt(sampler2D tex, v2f i, float xOffset, float yOffset)
            {
                return tex2D(tex, i.uv + float2(xOffset * _MainTex_TexelSize.x, yOffset * _MainTex_TexelSize.y));
            }

            bool IsAlive(float4 color)
            {
                return distance(color, _AliveColor) < 0.01;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = 1.0 / _ScreenParams.xy;

                float4 topLeftColor = GetColorAt(_MainTex, i, -1, -1);
                float4 topColor = GetColorAt(_MainTex, i, 0, -1);
                float4 topRightColor = GetColorAt(_MainTex, i, 1, -1);
                float4 leftColor = GetColorAt(_MainTex, i, -1, 0);
                float4 currentColor= tex2D(_MainTex, i.uv);
                float4 rightColor = GetColorAt(_MainTex, i, 1, 0);
                float4 bottomLeftColor = GetColorAt(_MainTex, i, -1, 1);
                float4 bottomColor = GetColorAt(_MainTex, i, 0, 1);
                float4 bottomRightColor = GetColorAt(_MainTex, i, 1, 1);

                int numberOfAliveNeighbours=0;
                if(IsAlive(topLeftColor)){numberOfAliveNeighbours++;}
                if(IsAlive(topColor)){numberOfAliveNeighbours++;}
                if(IsAlive(topRightColor)){numberOfAliveNeighbours++;}
                if(IsAlive(leftColor)){numberOfAliveNeighbours++;}
                if(IsAlive(rightColor)){numberOfAliveNeighbours++;}
                if(IsAlive(bottomLeftColor)){numberOfAliveNeighbours++;}
                if(IsAlive(bottomColor)){numberOfAliveNeighbours++;}
                if(IsAlive(bottomRightColor)){numberOfAliveNeighbours++;}

                if(numberOfAliveNeighbours == 3
                   || (IsAlive(currentColor) && numberOfAliveNeighbours == 2))
                {
                    return _AliveColor;
                }

                return _DeadColor;
            }
            ENDCG
        }
    }
}
