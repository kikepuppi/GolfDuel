Shader "Custom/CloudShadow"
{
    Properties
    {
        _BaseMap       ("Base Map (ignorado)", 2D)       = "white" {}
        _CloudTex      ("Cloud Noise Texture", 2D)       = "white" {}
        _CloudTex2     ("Cloud Noise Texture 2", 2D)     = "white" {}
        _ShadowColor   ("Shadow Color", Color)            = (0.1, 0.15, 0.2, 1)
        _ShadowAlpha   ("Shadow Intensity", Range(0,1))   = 0.45
        _Speed         ("Cloud Speed", Vector)            = (0.015, 0.008, 0, 0)
        _Speed2        ("Cloud Layer 2 Speed", Vector)    = (-0.01, 0.005, 0, 0)
        _Tiling        ("Cloud Tiling", Float)            = 1.5
        _Tiling2       ("Cloud Layer 2 Tiling", Float)   = 2.2
        _Softness      ("Edge Softness", Range(0.01, 1))  = 0.4
        _Threshold     ("Cloud Threshold", Range(0, 1))   = 0.45
    }

    SubShader
    {
        Tags
        {
            "RenderType"      = "Transparent"
            "Queue"           = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "CloudShadow"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            TEXTURE2D(_CloudTex);   SAMPLER(sampler_CloudTex);
            TEXTURE2D(_CloudTex2);  SAMPLER(sampler_CloudTex2);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _CloudTex_ST;
                float4 _CloudTex2_ST;
                float4 _ShadowColor;
                float  _ShadowAlpha;
                float4 _Speed;
                float4 _Speed2;
                float  _Tiling;
                float  _Tiling2;
                float  _Softness;
                float  _Threshold;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv1 = IN.uv * _Tiling + _Time.y * _Speed.xy;
                half cloud1 = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, uv1).r;

                float2 uv2 = IN.uv * _Tiling2 + _Time.y * _Speed2.xy;
                half cloud2 = SAMPLE_TEXTURE2D(_CloudTex2, sampler_CloudTex2, uv2).r;

                half cloudMask = cloud1 * 0.6 + cloud2 * 0.4;
                half shadow = smoothstep(_Threshold, _Threshold + _Softness, cloudMask);

                half4 col;
                col.rgb = _ShadowColor.rgb;
                col.a   = shadow * _ShadowAlpha;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
