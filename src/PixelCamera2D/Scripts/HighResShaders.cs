using UnityEngine;
using System.Collections;

public class HighResShaders : MonoBehaviour {


    public Texture2D scanLinesTexture;


    //public Vector4 UVScalarAUX;
    //public Vector4 UVOffsetAUX;
    //public Vector4 CRTMask_ScaleAUX;
    //public Vector4 CRTMask_OffsetAUX;

    //[Range(0f, 1f)]
    //public float Tuning_Overscan;
    //[Range(0f, 1f)]
    //public float Tuning_Dimming;
    //[Range(0f, 1f)]
    //public float Tuning_Satur;
    //[Range(0f, 1f)]
    //public float Tuning_ReflScalar;
    //[Range(0f, 1f)]
    //public float Tuning_Barrel;
    //[Range(0f, 1f)]
    //public float Tuning_Scanline_Brightness;
    //[Range(0f, 1f)]
    //public float Tuning_Scanline_Opacity;
    //[Range(0f, 1f)]
    //public float Tuning_Diff_Brightness;
    //[Range(0f, 1f)]
    //public float Tuning_Spec_Brightness;
    //[Range(0f, 1f)]
    //public float Tuning_Spec_Power;
    //[Range(0f, 1f)]
    //public float Tuning_Fres_Brightness;


    //public Vector4 Tuning_LightPos;


    private Material ScreenShader;

    [Range(0f, 1f)]
    public float Tuning_Scanlines;

    
    public float width;
    

    public float heigth;


    void Start()
    {
        //ScreenShader = new Material(Shader.Find("Hidden/screen"));
        ScreenShader = new Material(Shader.Find("Hidden/Screen"));

    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {




        //scanLinesTexture.wrapMode = TextureWrapMode.Repeat;

        ScreenShader.SetTexture("ScanLinesSampler", scanLinesTexture);
        ScreenShader.SetFloat("Tuning_Scanlines", Tuning_Scanlines);

        width = Screen.width;
        heigth = Screen.height;

        ScreenShader.SetFloat("width", width);
        ScreenShader.SetFloat("heigth", heigth);

        Graphics.Blit(src, dst, ScreenShader);


        //ScreenShader.SetTexture("compFrameSampler", src);


        ////src.SetBorderColor(borderColor);

        //scanLinesTexture.wrapMode = TextureWrapMode.Repeat;
        //ScreenShader.SetTexture("scanlinesSampler", scanLinesTexture);


        //ScreenShader.SetVector("UVScalarAUX", UVScalarAUX);
        //ScreenShader.SetVector("UVOffsetAUX", UVOffsetAUX);
        //ScreenShader.SetVector("CRTMask_ScaleAUX", CRTMask_ScaleAUX);
        //ScreenShader.SetVector("CRTMask_OffsetAUX", CRTMask_OffsetAUX);



        //ScreenShader.SetFloat("Tuning_Overscan", Tuning_Overscan);
        //ScreenShader.SetFloat("Tuning_Dimming", Tuning_Dimming);
        //ScreenShader.SetFloat("Tuning_Satur", Tuning_Satur);
        //ScreenShader.SetFloat("Tuning_ReflScalar", Tuning_ReflScalar);
        //ScreenShader.SetFloat("Tuning_Barrel", Tuning_Barrel);
        //ScreenShader.SetFloat("Tuning_Scanline_Brightness", Tuning_Scanline_Brightness);
        //ScreenShader.SetFloat("Tuning_Scanline_Opacity", Tuning_Scanline_Opacity);
        //ScreenShader.SetFloat("Tuning_Diff_Brightness", Tuning_Diff_Brightness);
        //ScreenShader.SetFloat("Tuning_Spec_Brightness", Tuning_Spec_Brightness);
        //ScreenShader.SetFloat("Tuning_Spec_Power", Tuning_Spec_Power);
        //ScreenShader.SetFloat("Tuning_Fres_Brightness", Tuning_Fres_Brightness);


        //ScreenShader.SetVector("Tuning_LightPos", Tuning_LightPos);


        //Graphics.Blit(src, dst, ScreenShader);



    }
}
