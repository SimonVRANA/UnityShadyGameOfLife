Shader "Custom/BetterForestFire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [Header(Colors)]
        _DeadColor ("DeadColor", Color) = (1, 0, 0, 1)
        _AliveColor ("AliveColor", Color) = (0, 1, 0, 1)

        [Header(Speeds)]
        _FireSpeed ("FireSpeed", float) = 0.2
        _RegrowSpeed ("RegrowSpeed", float) = 0.05

        [Header(WoodProperties)]
        _WoodToFireRatio ("WoodToFireRatio", float) = 2
        [Range(0.0, 1.0)]_ImuneThreshold ("ImuneThreshold", float) = 1

        [Header(Diffusion)]
        _HorizontalVertivcalDifusionRate ("HorizontalVertivcalDifusionRate", float) = 0.2
        _DiagonalDifusionRate ("DiagonalDifusionRate", float) = 0.05
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
            
            float4 _DeadColor;
            float4 _AliveColor;
            
            float _FireSpeed;
            float _RegrowSpeed;

            float _WoodToFireRatio;
            float _ImuneThreshold;

            float _HorizontalVertivcalDifusionRate;
            float _DiagonalDifusionRate;

            bool IsBurning(v2f i)
            {
                float4 currentColor= tex2D(_MainTex, i.uv);
                return currentColor.x > 0.0;
            }
            
            float AmountOfFireAt(v2f i, float xOffset, float yOffset)
            {
                return tex2D(_MainTex, i.uv + float2(xOffset * _MainTex_TexelSize.x, yOffset * _MainTex_TexelSize.y)).x;
            }

            float4 AmountOfFireRecievedFromNeighbours(v2f i)
            {
                float amountOfFire= 0.0;
                
                // add what was diffused from diagonal cells
                amountOfFire+= AmountOfFireAt(i, -1, -1) * _DiagonalDifusionRate;
                amountOfFire+= AmountOfFireAt(i, -1, 1) * _DiagonalDifusionRate;
                amountOfFire+= AmountOfFireAt(i, 1, -1) * _DiagonalDifusionRate;
                amountOfFire+= AmountOfFireAt(i, 1, 1) * _DiagonalDifusionRate;
                
                // add what was diffused from horizontal/vertical cells
                amountOfFire+= AmountOfFireAt(i, 0, -1) * _HorizontalVertivcalDifusionRate;
                amountOfFire+= AmountOfFireAt(i, 0, 1) * _HorizontalVertivcalDifusionRate;
                amountOfFire+= AmountOfFireAt(i, -1, 0) * _HorizontalVertivcalDifusionRate;
                amountOfFire+= AmountOfFireAt(i, 1, 0) * _HorizontalVertivcalDifusionRate;
                
                return amountOfFire;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 currentColor= tex2D(_MainTex, i.uv);
                float fireAmount = currentColor.x;
                float woodAmount = currentColor.y;

                if(IsBurning(i))
                {
                    // Less fire => Fire is extinguishing naturally
                    fireAmount -= _FireSpeed;

                    // How much fire can still be produced
                    float availableFireAmount = 1.0 - fireAmount;

                    // More Fire => fire eats wood to keep living
                    if(woodAmount < availableFireAmount / _WoodToFireRatio)
                    {
                        fireAmount+= woodAmount * _WoodToFireRatio;
                        woodAmount = 0.0;
                    }
                    else
                    {
                        fireAmount= 1.0;
                        woodAmount-= availableFireAmount / _WoodToFireRatio;
                    }

                    return float4(fireAmount, woodAmount, 0.0, 1.0);
                }

                if(woodAmount < _ImuneThreshold)
                {
                    woodAmount += _RegrowSpeed;
                    if(woodAmount > 1.0)
                    {
                        woodAmount = 1.0;
                    }
                    fireAmount= 0.0;
                }
                else
                {
                    fireAmount= AmountOfFireRecievedFromNeighbours(i);
                    if(fireAmount <= 0.0)
                    {
                        woodAmount += _RegrowSpeed;
                        if(woodAmount > 1.0)
                        {
                            woodAmount = 1.0;
                        }
                    }

                }
                return float4(fireAmount, woodAmount, 0.0, 1.0);
            }
            ENDCG
        }
    }
}
