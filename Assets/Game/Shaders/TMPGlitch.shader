Shader "UI/TMPComputerGlitch"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {}

        _JitterStrength ("Jitter Strength", Range(0,0.05)) = 0.01
        _LineStrength ("Horizontal Tear", Range(0,0.1)) = 0.02
        _Chromatic ("RGB Split", Range(0,0.02)) = 0.003

        _ScanlineIntensity ("Scanlines", Range(0,1)) = 0.35

        _ScrollX ("Noise Scroll X", Float) = 0.15
        _ScrollY ("Noise Scroll Y", Float) = 1.0

        _UpdateRate ("Update Rate", Range(1,60)) = 18

        _FaceColor ("Color", Color) = (1,1,1,1)

        [Enum(UnityEngine.Rendering.CullMode)]
        _CullMode ("Cull Mode", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;

            float4 _MainTex_ST;

            float _JitterStrength;
            float _LineStrength;
            float _Chromatic;
            float _ScanlineIntensity;

            float _ScrollX;
            float _ScrollY;
            float _UpdateRate;

            float4 _FaceColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float steppedTime =
                    floor(_Time.y * _UpdateRate) / _UpdateRate;

                //---------------------------------
                // Animated noise
                //---------------------------------

                float2 noiseUV =
                    i.uv +
                    float2(
                        steppedTime * _ScrollX,
                        steppedTime * _ScrollY
                    );

                float2 noise =
                    tex2D(_NoiseTex, noiseUV).rg;

                //---------------------------------
                // Global jitter
                //---------------------------------

                float2 uv = i.uv;

                uv += (noise - 0.5) * _JitterStrength;

                //---------------------------------
                // Horizontal tearing
                //---------------------------------

                float lineNoise =
                    tex2D(
                        _NoiseTex,
                        float2(0.0, uv.y * 8 + steppedTime)
                    ).r;

                uv.x +=
                    (lineNoise - 0.5)
                    * _LineStrength;

                //---------------------------------
                // RGB Split
                //---------------------------------

                float r =
                    tex2D(
                        _MainTex,
                        uv + float2(_Chromatic,0)
                    ).a;

                float g =
                    tex2D(
                        _MainTex,
                        uv
                    ).a;

                float b =
                    tex2D(
                        _MainTex,
                        uv - float2(_Chromatic,0)
                    ).a;

                float alpha = max(r,max(g,b));

                float3 color;

                color.r = r;
                color.g = g;
                color.b = b;

                //---------------------------------
                // Scanlines
                //---------------------------------

                float scan =
                    sin(i.uv.y * 900);

                scan =
                    lerp(
                        1,
                        scan * 0.5 + 0.5,
                        _ScanlineIntensity
                    );

                color *= scan;

                //---------------------------------
                // Final
                //---------------------------------

                color *= _FaceColor.rgb;
                color *= i.color.rgb;

                alpha *= _FaceColor.a;
                alpha *= i.color.a;

                return float4(color, alpha);
            }

            ENDHLSL
        }
    }
}