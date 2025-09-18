Shader "PicoDebugger/CubeOutline"
{
    Properties
    {
		_Color("Color", color) = (1,1,1,1)
		_BorderWidth("BorderWidth", range(0,1)) = 0.1
    }
    SubShader
    {
		Tags { "Queue"="Transparent" }
		Pass {

			//If you want to display the wireframe on the back, just uncomment the following two comments
			cull off
			ZWrite off
			blend srcalpha oneminussrcalpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			float4 _Color;
			float _BorderWidth;

			struct a2v {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v v) {
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			float4 frag(v2f i) : SV_Target {

				float4 col = float4(0,0,0,0);
				// Cube's base color
				_BorderWidth /= 100;
				col += saturate(step(i.uv.x, _BorderWidth) + step(1 - _BorderWidth, i.uv.x) + step(i.uv.y, _BorderWidth) + step(1 - _BorderWidth, i.uv.y)) * _Color;

				// if (i.uv.x < _BorderWidth || i.uv.x > 1 - _BorderWidth || i.uv.y < _BorderWidth || i.uv.y > 1 - _BorderWidth) 
				// {
				// 	col = _Color;
				// }

				return  col;
			}

			ENDCG
		}
    }
}