Shader "Custom/ReactionDiffusion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AliveColor ("AliveColor", Color) = (0,1,0,1)
        _DeadColor ("DeadColor", Color) = (0,0,1,1)

        _HorizontalVertivcalDifusionRate ("HorizontalVertivcalDifusionRate", float) = 0.2
        _DiagonalDifusionRate ("DiagonalDifusionRate", float) = 0.05

        _AliveDiffusion ("AliveDiffusion", float) = 1.0
        _FeedRate ("FeedRate", float) = 0.035

        _DeadDiffusion ("DeadDiffusion", float) = 0.5
        _KillFactor("KillFactor", float) = 0.058
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

            float _HorizontalVertivcalDifusionRate;
            float _DiagonalDifusionRate;

            float _AliveDiffusion;
            float _FeedRate;

            float _DeadDiffusion;
            float _KillFactor;

            float GetAmountAliveAt(v2f i, float xOffset, float yOffset)
            {
                return tex2D(_MainTex, i.uv + float2(xOffset * _MainTex_TexelSize.x, yOffset * _MainTex_TexelSize.y)).y;
            }

            float GetAmountDeadAt(v2f i, float xOffset, float yOffset)
            {
                return tex2D(_MainTex, i.uv + float2(xOffset * _MainTex_TexelSize.x, yOffset * _MainTex_TexelSize.y)).z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 currentColor= tex2D(_MainTex, i.uv);


                float amountAlive = currentColor.y;
                // add what was diffused from diagonal cells
                amountAlive+= GetAmountAliveAt(i, -1, -1) * _AliveDiffusion * _DiagonalDifusionRate;
                amountAlive+= GetAmountAliveAt(i, -1, 1) * _AliveDiffusion * _DiagonalDifusionRate;
                amountAlive+= GetAmountAliveAt(i, 1, -1) * _AliveDiffusion * _DiagonalDifusionRate;
                amountAlive+= GetAmountAliveAt(i, 1, 1) * _AliveDiffusion * _DiagonalDifusionRate;
                
                // add what was diffused from horizontal/vertical cells
                amountAlive+= GetAmountAliveAt(i, 0, -1) * _AliveDiffusion * _HorizontalVertivcalDifusionRate;
                amountAlive+= GetAmountAliveAt(i, 0, 1) * _AliveDiffusion * _HorizontalVertivcalDifusionRate;
                amountAlive+= GetAmountAliveAt(i, -1, 0) * _AliveDiffusion * _HorizontalVertivcalDifusionRate;
                amountAlive+= GetAmountAliveAt(i, 1, 0) * _AliveDiffusion * _HorizontalVertivcalDifusionRate;
                
                // remove what diffuse to neighbour cells
                // equals to amountAlive-= GetAmountAliveAt(i, 0, 0) * _AliveDiffusion;
                // but since gat amount calls to tex2D it is more optimised to call directly the current texture.
                amountAlive-= currentColor.y * _AliveDiffusion;

                // Remove what reacted with dead amount
                // equals to amountAlive-= GetAmountAliveAt(i, 0, 0) * GetAmountDeadAt(i, 0, 0) * GetAmountDeadAt(i, 0, 0);
                // but since gat amount calls to tex2D it is more optimised to call directly the current texture.
                amountAlive-= currentColor.y * currentColor.z * currentColor.z;

                // add the feed of aliveness
                // equals to amountAlive+= _FeedRate * (1-GetAmountAliveAt(i, 0, 0));
                // but since gat amount calls to tex2D it is more optimised to call directly the current texture.
                amountAlive+= _FeedRate * (1-currentColor.y);

                if(amountAlive < 0.0)
                {
                    amountAlive = 0.0;
                }
                if(amountAlive > 1.0)
                {
                    amountAlive = 1.0;
                }


                float amountDead = currentColor.z;
                // add what was diffused from diagonal cells
                amountDead+= GetAmountDeadAt(i, -1, -1) * _DeadDiffusion * _DiagonalDifusionRate;
                amountDead+= GetAmountDeadAt(i, -1, 1) * _DeadDiffusion * _DiagonalDifusionRate;
                amountDead+= GetAmountDeadAt(i, 1, -1) * _DeadDiffusion * _DiagonalDifusionRate;
                amountDead+= GetAmountDeadAt(i, 1, 1) * _DeadDiffusion * _DiagonalDifusionRate;
                
                // add what was diffused from horizontal/vertical cells
                amountDead+= GetAmountDeadAt(i, 0, -1) * _DeadDiffusion * _HorizontalVertivcalDifusionRate;
                amountDead+= GetAmountDeadAt(i, 0, 1) * _DeadDiffusion * _HorizontalVertivcalDifusionRate;
                amountDead+= GetAmountDeadAt(i, -1, 0) * _DeadDiffusion * _HorizontalVertivcalDifusionRate;
                amountDead+= GetAmountDeadAt(i, 1, 0) * _DeadDiffusion * _HorizontalVertivcalDifusionRate;
                
                // remove what diffuse to neighbour cells
                // equals to amountDead-= GetAmountDeadAt(i, 0, 0) * _DeadDiffusion;
                // but since gat amount calls to tex2D it is more optimised to call directly the current texture.
                amountDead-= currentColor.z * _DeadDiffusion;

                // add what reacted with alive amount
                // equals to amountDead+= GetAmountAliveAt(i, 0, 0) * GetAmountDeadAt(i, 0, 0) * GetAmountDeadAt(i, 0, 0);
                // but since gat amount calls to tex2D it is more optimised to call directly the current texture.
                amountDead+= currentColor.y * currentColor.z * currentColor.z;

                // add the kill of deathness
                // equals to amountDead+= (_FeedRate + _KillFactor) * GetAmountDeadAt(i, 0, 0);
                // but since gat amount calls to tex2D it is more optimised to call directly the current texture.
                amountDead-= (_FeedRate + _KillFactor) * currentColor.z;

                if(amountDead < 0.0)
                {
                    amountDead = 0.0;
                }
                if(amountDead > 1.0)
                {
                    amountDead = 1.0;
                }


                return float4(0.0, amountAlive,amountDead, 1.0);
            }
            ENDCG
        }
    }
}
