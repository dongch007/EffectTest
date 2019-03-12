Shader "Effect/OrderedDither"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DitherTex("Texture", 2D) = "white" {}
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
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _DitherTex;
			float4 _DitherTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float lum = Luminance(col);
			
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float2 screenPos = screenUV * _ScreenParams.xy;

				float threshold = tex2D(_DitherTex, screenPos*_DitherTex_TexelSize.x).r;
				if(lum <= threshold)
					col = 0;
				else
					col = 1;

				//float dither[16] = { 0, 8, 2, 10,
				//	12,4,14, 6,
				//	3, 11, 1, 9,
				//	15, 7, 13, 5 };
				//int2 index = screenPos % 4;
				//float d = dither[index.x * 4 + index.y];

				//if (lum < d / 15.0f)
				//	col = 0;
				//else
				//	col = 1;
				//if (lum < 0.5)
				//	col = 0;
				//else
				//	col = 1;


				//col = tex2D(_MainTex, screenUV);
				//col.rgb = Luminance(col);
				return col;
			}
			ENDCG
		}
	}
}
