Shader "Hidden/Border"
{
	Properties{
		_MainTex("Screen Blended", 2D) = "" {}
		_Overlay("Color", 2D) = "grey" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment fragAlphaBlend
			
			#include "UnityCG.cginc"

			struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};

	sampler2D _Overlay;
	half4 _Overlay_ST;

	sampler2D _MainTex;
	half4 _MainTex_ST;

	half _Intensity=60;
	half4 _MainTex_TexelSize;
	half4 _UV_Transform = half4(1, 0, 0, 1);

	
	half4 fragAlphaBlend(v2f i) : SV_Target{
		half4 toAdd = tex2D(_Overlay, i.uv[0]);
		return lerp(tex2D(_MainTex, i.uv[1]), toAdd, toAdd.a * 60);
	}


	
			ENDCG
		}
	}
}
