Shader "PicoDebugger/UIGradationOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1("Color 1", color) = (0.376, 0.12, 0.95)
        _Color2("Color 2", color) = (.6, 0.35, 0.75)
        _Color3("Color 3", color) = (0.17, 0.17, 0.51)
        _Radius("radius", Range(0, 1)) = 0.5
        _Alpha("Alpha", Range(0, 1)) = 0.0
        [HideInInspector]_TouchPos ("TouchPos", Vector) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-10" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #define NUM_POINTS 4

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
            float3 _Color1;
            float3 _Color2;
            float3 _Color3;
            float _Alpha;
            float _Radius;
            float4 _TouchPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 st = i.uv - 0.5; 
                float radio = _TouchPos.z / _TouchPos.w;
                st = float2(st.x * max(radio, 1.0), st.y / min(radio, 1.0));
                _TouchPos.xy = float2(_TouchPos.x * max(radio, 1.0), _TouchPos.y / min(radio, 1.0));
                float d = distance(st, _TouchPos.xy);
                float attenuation = smoothstep(0.33+_Radius*radio, -0.83, d);

                uv.x *= max(radio, 1.0);
                uv.y /= min(radio, 1.0);

                float t = _Time.y * 0.1+100;
                float e = 2.0;

                float2 pointPos[NUM_POINTS];
                pointPos[0] = (0.5 + 0.5 * float2(cos(-t), sin(-t))) * e;
                pointPos[1] = (0.5 + 0.5 * float2(cos(t * 1.7856), sin(t * 1.234))) * e;
                pointPos[2] = (0.5 + 0.5 * float2(cos(-t * 2.78633), sin(-t * 3.564))) * e;
                pointPos[3] = (0.5 + 0.5 * float2(cos(t * 4.567), sin(t * 3.124))) * e;

                float3 pointCol[NUM_POINTS];
                pointCol[0] = _Color1+float3(0.0, sin(t * 0.230) * 0.1, cos(t * 0.268 + 0.34) * 0.1);
                pointCol[1] = _Color1;
                pointCol[2] = _Color2;
                pointCol[3] = _Color3;

                float blend = 4.0;

                float3 col = float3(0.0,0.0,0.0);
                float totalWeight = 0.0;
                for (int i = 0; i < NUM_POINTS; ++i) {
                    float dist = distance(uv, pointPos[i]);
                    float weight = 1.0 / (pow(dist, blend) + 0.01);

                    col += pointCol[i] * weight;
                    totalWeight += weight;
                }

                col /= totalWeight;
                return float4(col+float3(attenuation,attenuation,attenuation)*0.2, _Alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
