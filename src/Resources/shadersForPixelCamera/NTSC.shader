Shader "Hidden/NTSC"
{
	Properties
	{
		_MainTex("Input Texture", 2D) = "black" {}
		lutSampler("Input Lut", 2D) = "black" {}
		Tuning_Strength("Tuning_Strength", Range(0,1)) = 0.5
		GradingRes("GradingRes", Range(0,64)) = 32
	}
		SubShader
	{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

		Pass
		{ 
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma enable_d3d11_debug_symbols


			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D lutSampler;
			uniform float Tuning_Strength;		
			uniform float GradingRes;


			// Takes in a color value and returns a transformed color value from the lookup table.
			float4 DoPost
			(
				half4 InColor
			)
			{
				half4 GameScene = InColor;

				// Some of this stuff could be precomputed offline for perf.
				half Res = GradingRes;
				half RcpRes = 1.0f / Res;
				half ResSq = Res*Res;
				half RcpResSq = 1.0f / ResSq;
				half HalfRcpResSq = 0.5f * RcpResSq;
				half HalfRcpRes = 0.5f * RcpRes;
				
				half ResMinusOne = Res - 1.0f;
				half ResMinusOneOverRes = ResMinusOne / Res;
				half ResMinusOneOverResSq = ResMinusOne / ResSq;

				half2 graduv_lo;
				half2 graduv_hi;

				half b_lo = floor(InColor.b * ResMinusOne);
				half b_hi = ceil(InColor.b * ResMinusOne);
				half b_alpha = ((InColor.b * ResMinusOne) - b_lo);

				graduv_lo.x = (b_lo * RcpRes) + InColor.r * ResMinusOneOverResSq + HalfRcpResSq;
				graduv_lo.y = InColor.g * ResMinusOneOverRes + HalfRcpRes;

				graduv_hi.x = (b_hi * RcpRes) + InColor.r * ResMinusOneOverResSq + HalfRcpResSq;
				graduv_hi.y = InColor.g * ResMinusOneOverRes + HalfRcpRes;

				half4 postgrad_lo = tex2D(lutSampler, graduv_lo);
				half4 postgrad_hi = tex2D(lutSampler, graduv_hi);

				half4 postgrad = lerp(postgrad_lo, postgrad_hi, b_alpha);

				return lerp(InColor, postgrad, Tuning_Strength);
			}



			half4 frag(v2f_img frag) : COLOR
			{
				return DoPost(tex2D(_MainTex, frag.uv));
			}


			ENDCG
		}
	}
}
