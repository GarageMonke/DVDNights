Shader "UI/TMPChalk"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {}

        _NoiseStrength ("UV Distortion", Range(0,0.01)) = 0.002
        _AlphaVariation ("Alpha Variation", Range(0,1)) = 0.15

        _ScrollX ("Scroll X", Float) = 0.01
        _ScrollY ("Scroll Y", Float) = 0.00

        _FaceColor ("Color", Color) = (1,1,1,1)
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
            float4 _NoiseTex_ST;

            float _NoiseStrength;
            float _AlphaVariation;

            float _ScrollX;
            float _ScrollY;

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
                float2 noiseUV =
                    i.uv +
                    float2(
                        _Time.y * _ScrollX,
                        _Time.y * _ScrollY
                    );

                float2 distortion =
                    (tex2D(_NoiseTex, noiseUV).rg - 0.5)
                    * _NoiseStrength;

                float4 font =
                    tex2D(_MainTex, i.uv + distortion);

                float noise =
                    tex2D(_NoiseTex, noiseUV).r;

                float alphaMod =
                    lerp(
                        1.0 - _AlphaVariation,
                        1.0,
                        noise
                    );

                float alpha =
                    font.a * alphaMod;

                return float4(
                    _FaceColor.rgb * i.color.rgb,
                    alpha * _FaceColor.a * i.color.a
                );
            }

            ENDHLSL
        }
    }
}