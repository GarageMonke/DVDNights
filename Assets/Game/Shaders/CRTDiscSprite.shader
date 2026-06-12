Shader "Custom/URP/SpriteGlowOutline"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color("Tint", Color) = (1,1,1,1)

        _Alpha("Global Alpha", Range(0,1)) = 1

        [Header(Glow)]
        _GlowColor("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity("Glow Intensity", Range(0,10)) = 3
        _GlowSize("Glow Size", Range(0,20)) = 5
        _GlowAlpha("Glow Alpha", Range(0,1)) = 1

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
            Name "SpriteGlowOutline"

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

            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowSize;
            float _GlowAlpha;

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
                // BASE SPRITE
                //----------------------------------------

                float4 baseCol = sprite * _Color * IN.color;

                //----------------------------------------
                // OUTLINE (inherits SpriteRenderer tint)
                //----------------------------------------

                float4 outlineCol = _OutlineColor;

                outlineCol.rgb *= (_Color.rgb * IN.color.rgb);
                outlineCol.a *= outlineMask * _OutlineAlpha;

                //----------------------------------------
                // GLOW (inherits SpriteRenderer tint)
                //----------------------------------------

                float4 glowCol = _GlowColor;

                glowCol.rgb *= (_Color.rgb * IN.color.rgb);
                glowCol.rgb *= _GlowIntensity;
                glowCol.a *= glowMask * _GlowAlpha;

                //----------------------------------------
                // COMPOSITE
                //----------------------------------------

                float4 result = glowCol;

                result.rgb = lerp(result.rgb, outlineCol.rgb, outlineCol.a);
                result.a = max(result.a, outlineCol.a);

                result.rgb = lerp(result.rgb, baseCol.rgb, baseCol.a);
                result.a = max(result.a, baseCol.a);

                // Global fade
                result.rgb *= _Alpha;
                result.a *= _Alpha;

                return result;
            }

            ENDHLSL
        }
    }
}