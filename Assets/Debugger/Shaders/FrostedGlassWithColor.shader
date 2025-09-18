Shader "PicoDebugger/FrostedGlassWithColor"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _FrostedStrength ("Frosted Strength", Range(0, 1)) = 0.5
        _BlurStrength ("Blur Strength", Range(0, 1)) = 0.5
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            fixed4 _Color;
            float _FrostedStrength;
            float _BlurStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample main texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // Sampling noise texture
                fixed4 noise = tex2D(_NoiseTex, i.uv );

                // Applied color
                col *= _Color;

                // Applied blur effect
                float2 uvOffsets[4] = {
                    float2(1.0, 0.0) * _BlurStrength,
                    float2(-1.0, 0.0) * _BlurStrength,
                    float2(0.0, 1.0) * _BlurStrength,
                    float2(0.0, -1.0) * _BlurStrength
                };

                fixed4 blurredColor = fixed4(0, 0, 0, 0);
                for (int j = 0; j < 4; j++) {
                    blurredColor += tex2D(_MainTex, i.uv + uvOffsets[j]) * 0.25;
                }

                col.rgb = lerp(col.rgb, blurredColor.rgb, _BlurStrength-noise.r);

                // Apply raw edge strength
                col.a *= (1 - _FrostedStrength);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

