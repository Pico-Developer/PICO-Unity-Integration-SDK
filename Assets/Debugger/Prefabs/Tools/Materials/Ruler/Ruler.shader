Shader "PXR_Debugger/Tool/Ruler"
{
    Properties
    {
        _Width ("Tick Width", Float) = 1
        _Height ("Tick Height", Float) = 0.5
        _MeshLength ("Mesh Length", Float) = 1.0  // 新增属性，表示网格长度
        _Divisor ("Tick Thickness Divisor", Float) = 5.0
        _BackgroundColor ("Background Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            float _Width;
            float _Height;
            float _MeshLength;  // 新增变量，表示网格长度
            float _Divisor;
            float4 _BackgroundColor;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算刻度数量
                float _NumOfTicks = 100.0;
                float numOfTicks = _MeshLength * _NumOfTicks;
                float maxTickWidth = 0.001;

                // 其余代码保持不变
                float tickWidth = _MeshLength*_Width;
                float tickHeight = _Height;

                float tickIndex = round(i.uv.x * _MeshLength / maxTickWidth);
                float remainder = fmod(tickIndex, _Divisor);
                bool shouldBeThick = remainder == 0.0;
                
                if (shouldBeThick) {
                    tickWidth *= 2.0;
                    tickHeight = _ScreenParams.y;
                }
                 
                float tickHalfWidth = tickWidth * 0.5;
                float tickCoordX = (tickIndex * maxTickWidth);
                
                fixed4 color = _BackgroundColor;
                
                if (i.uv.x * _MeshLength > tickCoordX - tickHalfWidth && 
                    i.uv.x * _MeshLength < tickCoordX + tickHalfWidth && 
                    i.uv.y * _ScreenParams.y > _ScreenParams.y - tickHeight)
                {
                    color = fixed4(0, 0, 0, 1);
                }
                
                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
