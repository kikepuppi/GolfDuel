Shader "Custom/GrassSprite"
{
    Properties
    {
        _MainTex        ("Sprite Texture",   2D) = "white" {}
        _GrassTex       ("Grass Leaf",       2D) = "white" {}
        _BorderTex      ("Border Leaf",      2D) = "white" {}
        _AccentTex      ("Accent Leaf 1",    2D) = "white" {}
        _AccentTex2     ("Accent Leaf 2",    2D) = "white" {}

        [Header(Cores)]
        _Color1         ("Cor Base",         Color) = (0.50, 0.72, 0.22, 1)
        _Color2         ("Cor Patch 2",      Color) = (0.35, 0.58, 0.12, 1)
        _Color3         ("Cor Patch 3",      Color) = (0.28, 0.48, 0.10, 1)
        _AccentTint     ("Tint Accent 1",    Color) = (0.60, 0.85, 0.25, 1)
        _AccentTint2    ("Tint Accent 2",    Color) = (0.55, 0.80, 0.20, 1)

        [Header(Chunks)]
        _ChunkScale     ("Tamanho Chunks",    Range(0.01, 1.0)) = 0.12
        _Threshold2     ("Threshold Patch 2", Range(0.0, 1.0))  = 0.5
        _Threshold3     ("Threshold Patch 3", Range(0.0, 1.0))  = 0.6

        [Header(Border Leaves)]
        _BorderScale    ("Densidade Border",  Range(0.5, 20.0)) = 6.0
        _BorderLeafSize ("Tamanho Leaf",      Range(0.1, 3.0))  = 1.4
        _BorderWindMult ("Vento Borda Mult",  Range(1.0, 5.0))  = 2.0

        [Header(Sprites)]
        _SpriteDarken   ("Escurecer Sprites", Range(0.3, 1.0))  = 0.7
        _Density        ("Densidade",         Range(1, 15))     = 3
        _BladeScale     ("Tamanho Blade",     Range(0.1, 2.0))  = 0.5
        _BladeScaleMin  ("Tamanho Blade Min", Range(0.1, 2.0))  = 0.2
        _AccentScaleMin ("Accent 1 Min",      Range(0.1, 2.0))  = 0.3
        _AccentScaleMax ("Accent 1 Max",      Range(0.1, 2.0))  = 0.7
        _AccentScaleMin2("Accent 2 Min",      Range(0.1, 2.0))  = 0.3
        _AccentScaleMax2("Accent 2 Max",      Range(0.1, 2.0))  = 0.7
        _WindSpeed      ("Vel. Vento",        Range(0, 5))      = 1.2
        _WindStrength   ("Força Vento",       Range(0, 0.3))    = 0.08
        _AccentChance   ("Chance Accent 1",   Range(0, 1))      = 0.12
        _AccentChance2  ("Chance Accent 2",   Range(0, 1))      = 0.08
        _GrassChance    ("Chance Grama",      Range(0, 1))      = 0.4
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                float3 worldPos    : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);
            TEXTURE2D(_GrassTex);   SAMPLER(sampler_GrassTex);
            TEXTURE2D(_BorderTex);  SAMPLER(sampler_BorderTex);
            TEXTURE2D(_AccentTex);  SAMPLER(sampler_AccentTex);
            TEXTURE2D(_AccentTex2); SAMPLER(sampler_AccentTex2);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4  _Color1, _Color2, _Color3;
                half4  _AccentTint, _AccentTint2;
                float  _ChunkScale, _Threshold2, _Threshold3;
                float  _BorderScale, _BorderLeafSize, _BorderWindMult;
                float  _SpriteDarken;
                float  _Density, _BladeScale, _BladeScaleMin;
                float  _AccentScaleMin, _AccentScaleMax;
                float  _AccentScaleMin2, _AccentScaleMax2;
                float  _WindSpeed, _WindStrength;
                float  _AccentChance, _AccentChance2, _GrassChance;
            CBUFFER_END

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float rand(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float sampleSpillLeaf(float2 wp, float scale, float leafSize,
                                  float chunkScale, float threshold,
                                  out half4 spillColor, half4 darkColor,
                                  float windStrength)
            {
                spillColor = darkColor;
                float2 baseCell = floor(wp * scale);

                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    float2 cell = baseCell + float2(dx, dy);
                    float2 cellWorldCenter = (cell + 0.5) / scale;

                    float cellNoise = valueNoise(cellWorldCenter * chunkScale);
                    if (cellNoise < threshold) continue;

                    float2 local = wp * scale - cell;

                    float r1 = rand(cell);
                    float r2 = rand(cell + 7.3);
                    float2 center = float2(0.2 + r1 * 0.6, 0.2 + r2 * 0.6);

                    // Vento por celula com fase unica
                    float cellWind = sin(_Time.y * _WindSpeed + r1 * 6.28) * windStrength;

                    float2 uv = local - center;
                    uv.x -= cellWind * uv.y;
                    uv /= leafSize;
                    uv += 0.5;

                    if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1) continue;

                    float alpha = SAMPLE_TEXTURE2D(_BorderTex, sampler_BorderTex, uv).a;
                    if (alpha > 0.5) return 1;
                }
                return 0;
            }

            half4 sampleSprite(TEXTURE2D_PARAM(tex, samp), float2 local,
                               float2 offset, float scale, float wind)
            {
                float2 uv = local - offset;
                uv.x -= wind * uv.y;
                uv /= scale;
                uv += 0.5;
                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                    return half4(0, 0, 0, 0);
                return SAMPLE_TEXTURE2D(tex, samp, uv);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color       = IN.color;
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 spriteColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;
                clip(spriteColor.a - 0.01);

                float2 wp = IN.worldPos.xy;

                // --- CHUNKS ---
                float n2 = valueNoise(wp * _ChunkScale);
                float n3 = valueNoise(wp * _ChunkScale * 0.7 + float2(3.7, 1.3));

                half4 chunkColor = _Color1;
                if (n2 > _Threshold2) chunkColor = _Color2;
                if (n3 > _Threshold3) chunkColor = _Color3;

                half4 col = chunkColor;

                // --- BORDER LEAVES COM SPILL + VENTO ---
                float borderWind = _WindStrength * _BorderWindMult;
                half4 spillColor2, spillColor3;

                float spill2 = sampleSpillLeaf(wp, _BorderScale, _BorderLeafSize,
                                               _ChunkScale, _Threshold2,
                                               spillColor2, _Color2, borderWind);

                float spill3 = sampleSpillLeaf(wp, _BorderScale, _BorderLeafSize,
                                               _ChunkScale * 0.7, _Threshold3,
                                               spillColor3, _Color3, borderWind);

                if (spill2 > 0.5) col = _Color2;
                if (spill3 > 0.5) col = _Color3;

                // --- SPRITES ---
                float2 cell  = floor(wp * _Density);
                float2 local = frac(wp * _Density);

                float r  = rand(cell);
                float r2 = rand(cell + 3.7);
                float r3 = rand(cell + 9.1);
                float r4 = rand(cell + 15.3);
                float r5 = rand(cell + 21.6);

                float2 bladePos = float2(0.2 + r * 0.6, 0.2 + r2 * 0.6);
                float  wind     = sin(_Time.y * _WindSpeed + r * 6.28) * _WindStrength;

                if (r3 < _AccentChance)
                {
                    float scale1 = lerp(_AccentScaleMin, _AccentScaleMax, r5);
                    half4 accent = sampleSprite(TEXTURE2D_ARGS(_AccentTex, sampler_AccentTex),
                                                local, bladePos, scale1, wind);
                    accent.rgb *= _AccentTint.rgb;
                    col = lerp(col, accent, accent.a);
                }
                else if (r3 < _AccentChance + _AccentChance2)
                {
                    float scale2 = lerp(_AccentScaleMin2, _AccentScaleMax2, r5);
                    half4 accent2 = sampleSprite(TEXTURE2D_ARGS(_AccentTex2, sampler_AccentTex2),
                                                 local, bladePos, scale2, wind);
                    accent2.rgb *= _AccentTint2.rgb;
                    col = lerp(col, accent2, accent2.a);
                }
                else if (r4 < _GrassChance)
                {
                    float scaleB = lerp(_BladeScaleMin, _BladeScale, r5);
                    half4 blade = sampleSprite(TEXTURE2D_ARGS(_GrassTex, sampler_GrassTex),
                                               local, bladePos, scaleB, wind);
                    blade.rgb *= chunkColor.rgb * _SpriteDarken;
                    col = lerp(col, blade, blade.a);
                }

                col.a = spriteColor.a;
                return col;
            }
            ENDHLSL
        }
    }
}