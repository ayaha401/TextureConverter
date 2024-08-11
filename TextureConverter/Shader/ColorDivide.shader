//RGBAとAllで分離して表示するShader
Shader "Unlit/ColorDived"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(All,0, R,1, G,2, B,3, A,4)] _DivedMode ("DivedMode", int) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            int _DivedMode;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                float3 col = (float3)0.0;
                float3 allChannelCol = (float3)0.0;
                float3 redChannelCol = (float3)0.0;
                float3 greenChannelCol = (float3)0.0;
                float3 blueChannelCol = (float3)0.0;
                float3 alphaChannelCol = (float3)0.0;

                if(_DivedMode == 0)
                {
                    allChannelCol = texColor.rgb;
                }

                if(_DivedMode == 1)
                {
                    redChannelCol = float3(texColor.r, 0.0, 0.0);
                }

                if(_DivedMode == 2)
                {
                    greenChannelCol = float3(0.0, texColor.g, 0.0);
                }

                if(_DivedMode == 3)
                {
                    blueChannelCol = float3(0.0, 0.0, texColor.b);
                }

                if(_DivedMode == 4)
                {
                    alphaChannelCol = float3(texColor.aaa);
                }
                
                float3 resultCol = redChannelCol + greenChannelCol + blueChannelCol + alphaChannelCol + allChannelCol;

                return float4(resultCol, 1.0);
            }
            ENDHLSL
        }
    }
}
