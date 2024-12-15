Shader "Custom/ForestFire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AliveColor ("AliveColor", Color) = (1,1,1,1)
        _DeadColor ("DeadColor", Color) = (1,1,1,1)
        _NumberOfRegrowSteps ("NumberOfRegrowSteps", int) = 8
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
            int _NumberOfRegrowSteps;

            float4 StageToColor(int stage)
            {
                float4 firstRegrowthStepColor = _AliveColor/_NumberOfRegrowSteps;
                float4 result = stage * firstRegrowthStepColor;
                result.w= 1;
                return result;
            }

            int GrowthStage(v2f i)
            {
                float4 firstRegrowthStepColor = _AliveColor/_NumberOfRegrowSteps;
                float4 currentColor= tex2D(_MainTex, i.uv);

                for(int j=0; j<_NumberOfRegrowSteps; j++)
                {
                    if(distance(currentColor, StageToColor(j)) < 0.01)
                    return j;
                }
                return _NumberOfRegrowSteps;
            }
            
            float4 GetColorAt(sampler2D tex, v2f i, float xOffset, float yOffset)
            {
                return tex2D(tex, i.uv + float2(xOffset * _MainTex_TexelSize.x, yOffset * _MainTex_TexelSize.y));
            }

            bool AnyNeighbourIsDead(v2f i)
            {
                float4 currentColor= tex2D(_MainTex, i.uv);

                float4 stepZeroColor= StageToColor(0);

                return distance(GetColorAt(_MainTex, i, -1, -1), stepZeroColor) < 0.01
                       || distance(GetColorAt(_MainTex, i, 0, -1), stepZeroColor) < 0.01
                       || distance(GetColorAt(_MainTex, i, 1, -1), stepZeroColor) < 0.01
                       || distance(GetColorAt(_MainTex, i, -1, 0), stepZeroColor) < 0.01
                       || distance(GetColorAt(_MainTex, i, 1, 0), stepZeroColor) < 0.01
                       || distance(GetColorAt(_MainTex, i, -1, 1), stepZeroColor) < 0.01
                       || distance(GetColorAt(_MainTex, i, 0, 1), stepZeroColor) < 0.01
                       || distance(GetColorAt(_MainTex, i, 1, 1), stepZeroColor) < 0.01;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int currentStage= GrowthStage(i);
                if(currentStage < _NumberOfRegrowSteps)
                {
                    return StageToColor(currentStage+1);
                }
                else
                {
                    if(AnyNeighbourIsDead(i))
                    {
                        return StageToColor(0);
                    }
                    else
                    {
                        return _AliveColor;
                    }
                }
            }
            ENDCG
        }
    }
}
