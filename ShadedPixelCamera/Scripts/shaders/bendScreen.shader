Shader "Hidden/bendScreen"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Scale("Scale", Range(0.0, 2.0)) = 0.0
	}
	
	SubShader{
		Pass
		{
			CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag


		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

		fixed _Scale;

		fixed4 frag(v2f_img i) : COLOR
			{
			float zero_p = 0.5;
			float2 fragment;
			fixed4 finalColor;
			float p_x = i.uv.x - zero_p;
			float p_y = i.uv.y - zero_p;
			float k = _Scale;
			float radius = sqrt(p_x*p_x + p_y*p_y);
			fragment = float2(zero_p + (1 - k / 5)*(1 + k*radius)*p_x, zero_p + (1 - k / 5)*(1 + k*radius)*p_y);
			if (fragment[0] < 0) {
				fragment[0] = 0;
			}if (fragment[0] > 1) {
				fragment[0] = 1;
			}if (fragment[1] < 0) {
				fragment[1] = 0;
			}if (fragment[1] > 1) {
				fragment[1] = 1;
			}

			finalColor = tex2D(_MainTex, fragment);
			return finalColor;

			}

			ENDCG
		}
	}
		FallBack "Diffuse"
}
