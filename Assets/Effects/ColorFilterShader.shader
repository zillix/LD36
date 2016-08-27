// Upgrade NOTE: replaced 'texRECT' with 'tex2D'

Shader "zillix/Color Filter Shader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_FilterColor("Filter Color", Color) = (1, 1, 1, 1)
	}

		SubShader{
		Pass{
		ZTest Always Cull Off ZWrite Off

		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
	uniform sampler2D _RampTex;
	uniform float3 _FilterColor;
	uniform half _RampOffset;


	fixed4 frag(v2f_img i) : COLOR
	{
		fixed4 original = tex2D(_MainTex, i.uv);
	float luminance = (original.rgb.r + original.rgb.b + original.rgb.g) / 3;
	float rDelta = max(0, (original.rgb.r - luminance)) * _FilterColor.r;
	float gDelta = max(0, (original.rgb.g - luminance)) * _FilterColor.g;
	float bDelta = max(0, (original.rgb.b - luminance)) * _FilterColor.b;


	original.rgb.x = luminance + rDelta - gDelta / 2 - bDelta / 2;
	original.rgb.y = luminance - rDelta / 2 + gDelta - gDelta / 2;
	original.rgb.z = luminance - rDelta / 2 - gDelta / 2 + bDelta;

	return original;
	}
		ENDCG

	}
	}

		Fallback off

}
