
Shader "UI/ComputerGlitchImage"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {}

        _JitterStrength ("Jitter Strength", Range(0,0.05)) = 0.01
        _LineStrength ("Horizontal Tear", Range(0,0.1)) = 0.02
        _Chromatic ("RGB Split", Range(0,0.02)) = 0.003

        _ScanlineIntensity ("Scanlines", Range(0,1)) = 0.35

        _ScrollX ("Noise Scroll X", Float) = 0.15
        _ScrollY ("Noise Scroll Y", Float) = 1.0

        _UpdateRate ("Update Rate", Range(1,60)) = 18
    }

    SubShader
    {
         Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
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

                uv += (noise - 0.5) * _JitterStrength * _MainTex_ST.xy;

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

                float4 rTex =
                    tex2D(
                        _MainTex,
                        uv + float2(_Chromatic,0)
                    );

                float4 gTex =
                    tex2D(
                        _MainTex,
                        uv
                    );

                float4 bTex =
                    tex2D(
                        _MainTex,
                        uv - float2(_Chromatic,0)
                    );

                float3 color =
                    float3(
                        rTex.r,
                        gTex.g,
                        bTex.b
                    );

                float alpha = gTex.a;

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

                return float4(
                    color * i.color.rgb,
                    alpha * i.color.a
                );
            }

            ENDHLSL
        }
    }
}