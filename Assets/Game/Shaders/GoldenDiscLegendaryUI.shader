Shader "Custom/UI/GoldenLegendary"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _Alpha("Global Alpha", Range(0,1)) = 1

        [Header(Glow)]
        _GlowSize("Glow Size", Range(0,20)) = 5
        _GlowIntensity("Glow Intensity", Range(0,20)) = 5
        _GlowAlpha("Glow Alpha", Range(0,1)) = 1

        [Header(Gold)]
        _GoldColor("Gold Color", Color) = (1.0,0.75,0.15,1)
        _HighlightColor("Highlight Color", Color) = (1,1,1,1)

        _ShineSpeed("Shine Speed", Range(0,10)) = 2
        _ShineIntensity("Shine Intensity", Range(0,10)) = 4

        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness("Outline Thickness", Range(0,10)) = 2
        _OutlineAlpha("Outline Alpha", Range(0,1)) = 1

        [Header(UI Masking do not touch set by Unity)]
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UIGoldenLegendary"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local UNITY_UI_CLIP_RECT
            #pragma shader_feature_local UNITY_UI_ALPHACLIP

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
                float4 worldPosition : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            float4 _Color;
            float _Alpha;

            float _GlowSize;
            float _GlowIntensity;
            float _GlowAlpha;

            float4 _GoldColor;
            float4 _HighlightColor;

            float _ShineSpeed;
            float _ShineIntensity;

            float4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineAlpha;

            float4 _ClipRect;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.worldPosition = IN.positionOS;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;

                return OUT;
            }

            float SampleAlpha(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            // Standard UGUI rect-clip helper (equivalent to UnityUI's UnityGet2DClipping)
            float UIClip(float2 position, float4 clipRect)
            {
                float2 inside = step(clipRect.xy, position) * step(position, clipRect.zw);
                return inside.x * inside.y;
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
                // GOLD SHINE
                //----------------------------------------

                float2 centerUV = IN.uv - 0.5;

                float angle = atan2(centerUV.y, centerUV.x);

                float shine =
                    sin(
                        angle * 6.0 +
                        _Time.y * _ShineSpeed
                    ) * 0.5 + 0.5;

                shine = pow(shine, 8.0);

                shine *= _ShineIntensity;

                //----------------------------------------
                // GOLD COLOR
                //----------------------------------------

                float3 legendaryGold =
                    lerp(
                        _GoldColor.rgb,
                        _HighlightColor.rgb,
                        saturate(shine)
                    );

                //----------------------------------------
                // BASE
                //----------------------------------------

                float4 baseCol = sprite * IN.color;

                //----------------------------------------
                // OUTLINE
                //----------------------------------------

                float4 outlineCol = _OutlineColor;

                outlineCol.rgb *= IN.color.rgb;
                outlineCol.a *= outlineMask * _OutlineAlpha;

                //----------------------------------------
                // GLOW
                //----------------------------------------

                float4 glowCol;

                glowCol.rgb = legendaryGold;
                glowCol.rgb *= _GlowIntensity;

                glowCol.a = glowMask * _GlowAlpha;

                //----------------------------------------
                // EXTRA HOTSPOTS
                //----------------------------------------

                float sparkle =
                    sin(_Time.y * 7.0 + angle * 14.0) * 0.5 + 0.5;

                sparkle = pow(sparkle, 16.0);

                glowCol.rgb += sparkle * shine * 2.0;

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

                //----------------------------------------
                // UI RECT CLIPPING (Mask / RectMask2D support)
                //----------------------------------------

                #ifdef UNITY_UI_CLIP_RECT
                result.a *= UIClip(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(result.a - 0.001);
                #endif

                return result;
            }

            ENDHLSL
        }
    }
}
