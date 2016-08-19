Shader "Hidden/Composite"
{
	Properties
	{
		curFrameSampler("Input Texture", 2D) = "white" {}
		prevFrameSampler("Previous Frame Map", 2D) = "white" {}
		NTSCArtifactSampler("Artifact Texture", 2D) = "black" {}

		width("width", Range(0,1000)) = 640
		heigth("heigth", Range(0,1000)) = 480


		Tuning_Sharp("Tuning_Sharp", Range(0,1)) = 0.5
		Tuning_Bleed("Tuning_Bleed", Range(0,1)) = 0.5
		Tuning_NTSC("Tuning_NTSC", Range(0,1)) = 0.5
		NTSCLerp("NTSCLerp", Range(0,1)) = 0.5

		Tuning_Persistence("Tuning_Persistence", Vector) = (0.8,0.8,0.8,0.8)

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

			// This is the second step of the CRT simulation process,
			// after the ntsc.fx shader has transformed the RGB values with a lookup table.
			// This is where we apply effects "inside the screen," including spatial and temporal bleeding,
			// an unsharp mask to simulate overshoot/undershoot, NTSC artifacts, and so on.

			uniform float width;
			uniform float heigth;


			uniform float Tuning_Sharp;				// typically [0,1], defines the weighting of the sharpness taps
			uniform float4 Tuning_Persistence;		// typically [0,1] per channel, defines the total blending of previous frame values
			uniform float Tuning_Bleed;				// typically [0,1], defines the blending of L/R values with center value from prevous frame
			uniform float Tuning_NTSC;				// typically [0,1], defines the weighting of NTSC scanline artifacts (not physically accurate by any means)
			uniform float NTSCLerp;					// Defines an interpolation between the two NTSC filter states. Typically would be 0 or 1 for vsynced 60 fps gameplay or 0.5 for unsynced, but can be whatever.



													// These are render target textures at the game scene resolution (256x224)
													// representing the current scene prior to any compositing and the previous frame after compositing.
													// Once we process this frame, we'll swap these and the final image will become our next frame's "previous."
			uniform sampler2D curFrameSampler;

			uniform sampler2D prevFrameSampler;

			uniform sampler2D NTSCArtifactSampler;

			// Weight for applying an unsharp mask at a distance of 1, 2, or 3 pixels from changes in luma.
			// The sign of each weight changes in order to alternately simulate overshooting and undershooting.
			// (Note: The "autogen=false" markup is not a part of HLSL; my engine uses this markup to determine whether the game needs to provide a value for this variable.)
			/*float SharpWeight[3] =
			{
				1.0, -0.3162277, 0.1
			};*/

			//// Reciprocals of screen dimensions, precomputed for perf.
			//const float2 RcpScrWidth = float2(1.0f / 640.0f, 0.0f);
			//const float2 RcpScrHeight = float2(0.0f, 1.0f / 480.0f);

		
			// Calculate luma for an RGB value.
			half Brightness(half4 InVal)
			{
				return dot(InVal, half4(0.299, 0.587, 0.114, 0.0));
			}

			half4 frag(v2f_img frag) : COLOR
			{

				 // This values should be introduced from outside if this shader is going to be used with verious resolutions 
				 half2 RcpScrWidth = half2(1.0f / width, 0.0f);
				 half2 RcpScrHeight = half2(0.0f, 1.0f / heigth);

				 float SharpWeight[3] =
				 {
					 1.0, -0.3162277, 0.1
				 };

				half4 NTSCArtifact1 = tex2D(NTSCArtifactSampler, frag.uv);
				half4 NTSCArtifact2 = tex2D(NTSCArtifactSampler, frag.uv + RcpScrHeight);
				half4 NTSCArtifact = lerp(NTSCArtifact1, NTSCArtifact2, NTSCLerp);

				half2 LeftUV = frag.uv - RcpScrWidth;
				half2 RightUV = frag.uv + RcpScrWidth;

				half4 Cur_Left = tex2D(curFrameSampler, LeftUV);
				half4 Cur_Local = tex2D(curFrameSampler, frag.uv);
				half4 Cur_Right = tex2D(curFrameSampler, RightUV);

				half4 TunedNTSC = NTSCArtifact * Tuning_NTSC;

				// Note: The "persistence" and "bleed" parameters have some overlap, but they are not redundant.
				// "Persistence" affects bleeding AND trails. (Scales the sum of the previous value and its scaled neighbors.)
				// "Bleed" only affects bleeding. (Scaling of neighboring previous values.)

				half4 Prev_Left = tex2D(prevFrameSampler, LeftUV);
				half4 Prev_Local = tex2D(prevFrameSampler, frag.uv);
				half4 Prev_Right = tex2D(prevFrameSampler, RightUV);

				// Apply NTSC artifacts based on differences in luma between local pixel and neighbors..
				Cur_Local =
					saturate(Cur_Local +
					(((Cur_Left - Cur_Local) + (Cur_Right - Cur_Local)) * TunedNTSC));

				half curBrt = Brightness(Cur_Local);
				half offset = 0;

				// Step left and right looking for changes in luma that would produce a ring or halo on this pixel due to undershooting/overshooting.
				// (Note: It would probably be more accurate to look at changes in luma between pixels at a distance of N and N+1,
				// as opposed to 0 and N as done here, but this works pretty well and is a little cheaper.)
				for (int i = 0; i < 3; ++i)
				{
					half2 StepSize = (half2(1.0 / width,0) * (float(i + 1)));
					half4 neighborleft = tex2D(curFrameSampler, frag.uv - StepSize);
					half4 neighborright = tex2D(curFrameSampler, frag.uv + StepSize);

					half NBrtL = Brightness(neighborleft);
					half NBrtR = Brightness(neighborright);
					offset += ((((curBrt - NBrtL) + (curBrt - NBrtR))) * SharpWeight[i]);
				}

				// Apply the NTSC artifacts to the unsharp offset as well.
				Cur_Local = saturate(Cur_Local + (offset * Tuning_Sharp * lerp(half4(1,1,1,1), NTSCArtifact, Tuning_NTSC)));

				// Take the max here because adding is overkill; bleeding should only brighten up dark areas, not blow out the whole screen.
				Cur_Local = saturate(max(Cur_Local, Tuning_Persistence * (1.0 / (1.0 + (2.0 * Tuning_Bleed))) * (Prev_Local + ((Prev_Left + Prev_Right) * Tuning_Bleed))));

				//Cur_Local = saturate(Cur_Local + (Tuning_Persistence * (1.0 / (1.0 + (2.0 * Tuning_Bleed))) * (Prev_Local + ((Prev_Left + Prev_Right) * Tuning_Bleed))));


				return Cur_Local;
			}


				ENDCG
		}
	}
}
