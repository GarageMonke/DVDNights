Shader "UI/VHS_FastForward"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ScanlineTex ("Scanline Texture (Wrap: Repeat)", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 15.0
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _ScanlineOpacity ("Scanline Opacity", Range(0, 1)) = 0.8
        
        // UI Required
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _ScanlineTex;
            float _ScrollSpeed;
            float _DistortionStrength;
            float _ScanlineOpacity;

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // 1. Calculate the scrolling UV for the scanlines
                float2 scanUV = i.texcoord;
                scanUV.y += _Time.y * _ScrollSpeed;

                // 2. Sample the scanline texture
                // We use the 'R' channel of your asset to drive the distortion
                fixed4 scanSample = tex2D(_ScanlineTex, scanUV);

                // 3. DISTORTION: Offset the main texture UVs horizontally
                // This creates the jagged "tearing" look seen in the video
                float distortion = (scanSample.r - 0.5) * _DistortionStrength;
                float2 mainUV = i.texcoord + float2(distortion, 0);

                // 4. Sample the main image with the distorted UV
                fixed4 col = tex2D(_MainTex, mainUV) * i.color;

                // 5. Blend the white scanlines over the image
                // We multiply the scanline brightness by opacity
                fixed3 finalRGB = lerp(col.rgb, scanSample.rgb, scanSample.r * _ScanlineOpacity);

                return fixed4(finalRGB, col.a);
            }
            ENDCG
        }
    }
}