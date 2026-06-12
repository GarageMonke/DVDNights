Shader "Custom/CRTDiscSprite"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color("Tint", Color) = (1,1,1,1)

        _GlowIntensity("Glow Intensity", Range(0,10)) = 2
        _GlowAlpha("Glow Alpha", Range(0,1)) = 1

        _ScanlineDensity("Scanline Density", Range(10,500)) = 150
        _ScanlineStrength("Scanline Strength", Range(0,1)) = 0.15

        _DistortionStrength("Distortion", Range(0,0.05)) = 0.005

        _FresnelPower("Fresnel Power", Range(0.1,8)) = 3
        _FresnelIntensity("Fresnel Intensity", Range(0,5)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Name "Forward"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;
            float4 _Color;

            float _GlowIntensity;
            float _GlowAlpha;

            float _ScanlineDensity;
            float _ScanlineStrength;

            float _DistortionStrength;

            float _FresnelPower;
            float _FresnelIntensity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs pos =
                    GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionCS = pos.positionCS;

                OUT.uv =
                    TRANSFORM_TEX(IN.uv, _MainTex);

                OUT.normalWS =
                    TransformObjectToWorldNormal(IN.normalOS);

                float3 worldPos =
                    TransformObjectToWorld(IN.positionOS.xyz);

                OUT.viewDirWS =
                    GetCameraPositionWS() - worldPos;

                OUT.color = IN.color;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                float t = _Time.y;

                // CRT wobble
                uv.x += sin(uv.y * 40.0 + t * 8.0)
                      * _DistortionStrength;

                float4 tex =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        uv
                    );

                clip(tex.a - 0.001);

                float alpha =
                    tex.a *
                    _Color.a *
                    IN.color.a;

                // Scanlines
                float scan =
                    sin(uv.y * _ScanlineDensity);

                scan =
                    lerp(
                        1.0,
                        scan * 0.5 + 0.5,
                        _ScanlineStrength
                    );

                float3 baseColor =
                    tex.rgb *
                    _Color.rgb *
                    IN.color.rgb *
                    scan;

                // Fresnel
                float3 N =
                    normalize(IN.normalWS);

                float3 V =
                    normalize(IN.viewDirWS);

                float fresnel =
                    pow(
                        1.0 -
                        saturate(dot(N, V)),
                        _FresnelPower
                    );

                // HDR emissive glow
                float luminance =
                    dot(
                        tex.rgb,
                        float3(
                            0.299,
                            0.587,
                            0.114
                        )
                    );

                float3 glow =
                    tex.rgb *
                    _Color.rgb *
                    luminance *
                    (_GlowIntensity * 10.0) *
                    _GlowAlpha;

                float3 fresnelGlow =
                    _Color.rgb *
                    fresnel *
                    _FresnelIntensity *
                    _GlowAlpha;

                float3 finalColor =
                    (baseColor + fresnelGlow) * alpha +
                    glow;

                return float4(finalColor, alpha);
            }

            ENDHLSL
        }
    }

    FallBack Off
}