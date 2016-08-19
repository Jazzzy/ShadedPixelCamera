Shader "Hidden/Screen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		ScanLinesSampler("Artifact Texture", 2D) = "black" {}

		Tuning_Scanlines("Tuning_Scanlines", Range(0,1)) = 0.5

			width("width", Range(0,1000)) = 640
			heigth("heigth", Range(0,1000)) = 480


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
			
			#include "UnityCG.cginc"

			
			
			sampler2D _MainTex;
			sampler2D ScanLinesSampler;
			uniform float Tuning_Scanlines;

			uniform float width;
			uniform float heigth;

			half4 frag (v2f_img i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);

				half2 scanUV = half2(width / 6 , heigth / 3 );

				half4 scan = tex2D(ScanLinesSampler, i.uv * scanUV);

				//if (col.w > 0) {

				if(col.x<0.01 && col.y<0.01 && col.z<0.01){

					return col;

				}
				else {

					half4 returnVal = lerp(col, scan, Tuning_Scanlines);
					return returnVal;
				}

				
			}
			ENDCG
		}
	}
}
