Shader "Custom/Outline Fill"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)]
        _ZTest("ZTest", Float) = 0

        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Range(0,10)) = 2

        _HatchTex("Hatch Texture", 2D) = "white" {}
        _HatchScale("Hatch Scale", Float) = 50

        _NoiseTex("Noise Texture", 2D) = "gray" {}
        _NoiseStrength("Noise Strength", Range(0,0.1)) = 0.01

        _UpdateRate("Update Rate", Range(1,60)) = 12

        _ScrollX("Scroll X", Float) = 0.03
        _ScrollY("Scroll Y", Float) = 0.02
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+110"
            "RenderType" = "Transparent"
            "DisableBatching" = "True"
        }

        Pass
        {
            Name "Fill"

            Cull Off
            ZTest [_ZTest]
            ZWrite Off

            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            Stencil
            {
                Ref 1
                Comp NotEqual
            }

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 smoothNormal : TEXCOORD3;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 screenPos : TEXCOORD0;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _HatchTex;
            sampler2D _NoiseTex;

            float4 _OutlineColor;

            float _OutlineWidth;

            float _HatchScale;

            float _NoiseStrength;

            float _UpdateRate;

            float _ScrollX;
            float _ScrollY;

            v2f vert(appdata input)
            {
                v2f output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 normal =
                    any(input.smoothNormal)
                    ? input.smoothNormal
                    : input.normal;

                float3 viewPosition =
                    UnityObjectToViewPos(input.vertex);

                float3 viewNormal =
                    normalize(
                        mul(
                            (float3x3)UNITY_MATRIX_IT_MV,
                            normal
                        )
                    );

                output.position =
                    UnityViewToClipPos(
                        viewPosition +
                        viewNormal *
                        -viewPosition.z *
                        _OutlineWidth /
                        1000.0
                    );

                output.screenPos =
                    ComputeScreenPos(output.position);

                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float2 uv =
                    input.screenPos.xy /
                    input.screenPos.w;

                float steppedTime =
                    floor(_Time.y * _UpdateRate) /
                    _UpdateRate;

                float2 noiseUV =
                    uv +
                    float2(
                        steppedTime * _ScrollX,
                        steppedTime * _ScrollY
                    );

                float2 distortion =
                    (tex2D(_NoiseTex, noiseUV).rg - 0.5)
                    * _NoiseStrength;

                float hatch =
                    tex2D(
                        _HatchTex,
                        uv * _HatchScale + distortion
                    ).r;

                fixed4 col = _OutlineColor;

                col.a *= hatch;

                return col;
            }

            ENDCG
        }
    }
}