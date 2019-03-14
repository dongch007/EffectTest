Shader "Hidden/ASCII Art Fx"
{
    Properties 
    {
        _MainTex("Source Image", 2D) = "" {}
		_AsciiTex("AscII Image", 2D) = "" {}
    }

CGINCLUDE

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _MainTex_TexelSize;
sampler2D _AsciiTex;
float _FontPixel;
float _TileX;
float _TileY;

struct v2f
{
    float4 position : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};   

float4 frag(v2f i) : SV_Target
{
	float2 texel = _MainTex_TexelSize.xy * _FontPixel;
	float2 uv = i.texcoord.xy;
	float4 c = tex2D(_MainTex, floor(uv/ texel)*texel);

	float gray = Luminance(c);

	float tileNum = _TileX * _TileY;
	float index = floor(gray * tileNum);

	float2 asciiUV = frac(uv / texel);
	asciiUV.x = index/_TileX + asciiUV.x/_TileX;
	asciiUV.y = index%_TileX + asciiUV.y/_TileY;

	//float4 col = tex2D(_AsciiTex, asciiUV) * gray;
	float4 col = tex2D(_AsciiTex, asciiUV) * c;

	return col;
}

ENDCG

SubShader
{
	Pass
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert_img
		#pragma fragment frag
		ENDCG
	}
}
}