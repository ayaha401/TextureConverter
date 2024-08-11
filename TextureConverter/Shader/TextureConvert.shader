// 4枚のTexuterから各チャネルで合成する
Shader "Unlit/TextureConvert"
{
    Properties
    {
        _SourceTexture1 ("SourceTexture1", 2D) = "white" {}
        [Enum(R,1, G,2, B,3, A,4)] _DivedMode1 ("DivedMode", int) = 1

        _SourceTexture2 ("SourceTexture2", 2D) = "white" {}
        [Enum(R,1, G,2, B,3, A,4)] _DivedMode2 ("DivedMode", int) = 1

        _SourceTexture3 ("SourceTexture3", 2D) = "white" {}
        [Enum(R,1, G,2, B,3, A,4)] _DivedMode3 ("DivedMode", int) = 1

        _SourceTexture4 ("SourceTexture4", 2D) = "white" {}
        [Enum(R,1, G,2, B,3, A,4)] _DivedMode4 ("DivedMode", int) = 1

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

            sampler2D _SourceTexture1;
            sampler2D _SourceTexture2;
            sampler2D _SourceTexture3;
            sampler2D _SourceTexture4;
            
            CBUFFER_START(UnityPerMaterial)
            int _DivedMode1;
            int _DivedMode2;
            int _DivedMode3;
            int _DivedMode4;
            CBUFFER_END

            float getDivedChannelColor(float4 textureColor, int divedMode)
            {
                float col = 0.0;

                if(divedMode == 1)
                {
                    col = textureColor.r;
                }

                if(divedMode == 2)
                {
                    col = textureColor.g;
                }

                if(divedMode == 3)
                {
                    col = textureColor.b;
                }

                if(divedMode == 4)
                {
                    col = textureColor.a;
                }

                return col;
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float4 col = float4(0.0, 0.0, 0.0, 1.0);
                
                col.r = getDivedChannelColor(tex2D(_SourceTexture1, i.uv), _DivedMode1);
                col.g = getDivedChannelColor(tex2D(_SourceTexture2, i.uv), _DivedMode2);
                col.b = getDivedChannelColor(tex2D(_SourceTexture3, i.uv), _DivedMode3);
                col.a = getDivedChannelColor(tex2D(_SourceTexture4, i.uv), _DivedMode4);

                return col;
            }
            ENDHLSL
        }
    }
}
