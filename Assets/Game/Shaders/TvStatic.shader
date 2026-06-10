Shader "Custom/TVStatic"
{
    Properties
    {
        _Brightness ("Brightness", Range(0,5)) = 1

        _NoiseScale ("Noise Scale", Float) = 512
        _NoiseSpeed ("Noise Speed", Float) = 30
        _Threshold ("Noise Density", Range(0,1)) = 0.75

        _ScanlineCount ("Scanline Count", Float) = 300
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.2

        _BandCount ("Band Count", Float) = 6
        _BandSpeed ("Band Speed", Float) = 0.5
        _BandStrength ("Band Strength", Range(0,2)) = 0.8

        _JitterAmount ("Horizontal Jitter", Range(0,0.05)) = 0.005
        _JitterSpeed ("Jitter Speed", Float) = 50
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Forward"

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

            float _Brightness;

            float _NoiseScale;
            float _NoiseSpeed;
            float _Threshold;

            float _ScanlineCount;
            float _ScanlineStrength;

            float _BandCount;
            float _BandSpeed;
            float _BandStrength;

            float _JitterAmount;
            float _JitterSpeed;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                return OUT;
            }

            float Hash(float2 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * (p.x + p.y));
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                //----------------------------------
                // Horizontal jitter
                //----------------------------------

                float jitter =
                    sin(uv.y * 400.0 + _Time.y * _JitterSpeed)
                    * _JitterAmount;

                uv.x += jitter;

                //----------------------------------
                // Static noise
                //----------------------------------

                float2 pixelCoord =
                    floor(uv * _NoiseScale);

                float noise =
                    Hash(pixelCoord + floor(_Time.y * _NoiseSpeed));

                noise = step(_Threshold, noise);

                //----------------------------------
                // Scanlines
                //----------------------------------

                float scan =
                    sin(uv.y * _ScanlineCount * 6.283185);

                scan = scan * 0.5 + 0.5;

                scan *= _ScanlineStrength;

                //----------------------------------
                // Horizontal interference bands
                //----------------------------------

                float band =
                    sin(
                        uv.y * _BandCount * 6.283185 +
                        _Time.y * _BandSpeed
                    );

                band = pow(abs(band), 8.0);
                band *= _BandStrength;

                //----------------------------------
                // Final
                //----------------------------------

                float value =
                    noise +
                    band +
                    scan;

                value *= _Brightness;

                return half4(value, value, value, 1);
            }

            ENDHLSL
        }
    }
}