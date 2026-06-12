Shader "Custom/URP/SpriteRainbowGlowOutline"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color("Tint", Color) = (1,1,1,1)

        _Alpha("Global Alpha", Range(0,1)) = 1

        [Header(Glow)]
        _GlowIntensity("Glow Intensity", Range(0,10)) = 3
        _GlowSize("Glow Size", Range(0,20)) = 5
        _GlowAlpha("Glow Alpha", Range(0,1)) = 1

        [Header(Rainbow)]
        _RainbowColor1("Rainbow Color 1", Color) = (1,0,0,1)
        _RainbowColor2("Rainbow Color 2", Color) = (1,1,0,1)
        _RainbowColor3("Rainbow Color 3", Color) = (0,1,0,1)
        _RainbowColor4("Rainbow Color 4", Color) = (0,1,1,1)
        _RainbowColor5("Rainbow Color 5", Color) = (0,0,1,1)
        _RainbowColor6("Rainbow Color 6", Color) = (1,0,1,1)

        _RainbowSpeed("Rainbow Speed", Range(0,10)) = 1
        _RainbowScale("Rainbow Scale", Range(0.1,20)) = 5

        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness("Outline Thickness", Range(0,10)) = 2
        _OutlineAlpha("Outline Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "SpriteRainbowGlowOutline"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;

            float4 _Color;
            float _Alpha;

            float _GlowIntensity;
            float _GlowSize;
            float _GlowAlpha;

            float4 _RainbowColor1;
            float4 _RainbowColor2;
            float4 _RainbowColor3;
            float4 _RainbowColor4;
            float4 _RainbowColor5;
            float4 _RainbowColor6;

            float _RainbowSpeed;
            float _RainbowScale;

            float4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineAlpha;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;

                return OUT;
            }

            float SampleAlpha(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            float3 RainbowGradient(float t)
            {
                if (t < 0.2)
                    return lerp(_RainbowColor1.rgb, _RainbowColor2.rgb, t / 0.2);

                if (t < 0.4)
                    return lerp(_RainbowColor2.rgb, _RainbowColor3.rgb, (t - 0.2) / 0.2);

                if (t < 0.6)
                    return lerp(_RainbowColor3.rgb, _RainbowColor4.rgb, (t - 0.4) / 0.2);

                if (t < 0.8)
                    return lerp(_RainbowColor4.rgb, _RainbowColor5.rgb, (t - 0.6) / 0.2);

                return lerp(_RainbowColor5.rgb, _RainbowColor6.rgb, (t - 0.8) / 0.2);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                float spriteAlpha = sprite.a;
                float2 texel = _MainTex_TexelSize.xy;

                //----------------------------------------
                // OUTLINE
                //----------------------------------------

                float outlineRadius = max(_OutlineThickness, 0.001);

                float outlineSample =
                    SampleAlpha(IN.uv + float2( texel.x * outlineRadius, 0)) +
                    SampleAlpha(IN.uv + float2(-texel.x * outlineRadius, 0)) +
                    SampleAlpha(IN.uv + float2(0,  texel.y * outlineRadius)) +
                    SampleAlpha(IN.uv + float2(0, -texel.y * outlineRadius)) +
                    SampleAlpha(IN.uv + float2( texel.x * outlineRadius,  texel.y * outlineRadius)) +
                    SampleAlpha(IN.uv + float2(-texel.x * outlineRadius,  texel.y * outlineRadius)) +
                    SampleAlpha(IN.uv + float2( texel.x * outlineRadius, -texel.y * outlineRadius)) +
                    SampleAlpha(IN.uv + float2(-texel.x * outlineRadius, -texel.y * outlineRadius));

                outlineSample /= 8.0;

                float outlineMask = saturate(outlineSample - spriteAlpha);

                //----------------------------------------
                // GLOW
                //----------------------------------------

                float glowRadius = max(_GlowSize, 0.001);

                float glow =
                    SampleAlpha(IN.uv + float2( texel.x * glowRadius, 0)) +
                    SampleAlpha(IN.uv + float2(-texel.x * glowRadius, 0)) +
                    SampleAlpha(IN.uv + float2(0,  texel.y * glowRadius)) +
                    SampleAlpha(IN.uv + float2(0, -texel.y * glowRadius)) +
                    SampleAlpha(IN.uv + float2( texel.x * glowRadius * 1.5,  texel.y * glowRadius * 1.5)) +
                    SampleAlpha(IN.uv + float2(-texel.x * glowRadius * 1.5,  texel.y * glowRadius * 1.5)) +
                    SampleAlpha(IN.uv + float2( texel.x * glowRadius * 1.5, -texel.y * glowRadius * 1.5)) +
                    SampleAlpha(IN.uv + float2(-texel.x * glowRadius * 1.5, -texel.y * glowRadius * 1.5));

                glow /= 8.0;

                float glowMask = saturate(glow - spriteAlpha);

                //----------------------------------------
                // RAINBOW
                //----------------------------------------

                float2 centeredUV = (IN.uv - 0.5) * _RainbowScale;

                float rainbowT =
                    atan2(centeredUV.y, centeredUV.x) / 6.2831853 +
                    0.5;

                rainbowT += _Time.y * _RainbowSpeed;
                rainbowT = frac(rainbowT);

                float3 rainbowColor = RainbowGradient(rainbowT);

                //----------------------------------------
                // BASE
                //----------------------------------------

                float4 baseCol = sprite * _Color * IN.color;

                //----------------------------------------
                // OUTLINE
                //----------------------------------------

                float4 outlineCol = _OutlineColor;

                outlineCol.rgb *= (_Color.rgb * IN.color.rgb);
                outlineCol.a *= outlineMask * _OutlineAlpha;

                //----------------------------------------
                // GLOW
                //----------------------------------------

                float4 glowCol;

                glowCol.rgb = rainbowColor;
                glowCol.rgb *= (_Color.rgb * IN.color.rgb);
                glowCol.rgb *= _GlowIntensity;

                glowCol.a = glowMask * _GlowAlpha;

                //----------------------------------------
                // COMPOSITE
                //----------------------------------------

                float4 result = glowCol;

                result.rgb = lerp(result.rgb, outlineCol.rgb, outlineCol.a);
                result.a = max(result.a, outlineCol.a);

                result.rgb = lerp(result.rgb, baseCol.rgb, baseCol.a);
                result.a = max(result.a, baseCol.a);

                result.rgb *= _Alpha;
                result.a *= _Alpha;

                return result;
            }

            ENDHLSL
        }
    }
}