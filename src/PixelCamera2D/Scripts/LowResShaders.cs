using UnityEngine;
using System.Collections;

[AddComponentMenu("Ghosting")]
public class LowResShaders : MonoBehaviour {

    [Range(0, 0.99f)]
    public float Ghosting_Intensity;

    public int lowResWidth = 640;
    public int lowResHeigth = 480;

    //public float Tuning_Strength = 0;

    //public float GradingRes = 32;

    //public Texture LUTTexture;

    [Range(0f, 1f)]
    public float Tuning_Sharp;

    [Range(0f, 1f)]
    public float Tuning_Bleed;

    [Range(0f, 1f)]
    public float Tuning_NTSC;

    [Range(0f, 1f)]
    public float NTSCLerp;

    public Vector4 Tuning_Persistence;

    public Texture2D NTSCArtifactTexture;

    private Material GhostingShader;
    private Material CompositeShader;
    private float alting=0;

    private RenderTexture pastFrame;

    void Start()
    {
        GhostingShader = new Material(Shader.Find("Hidden/Ghosting"));
        
        CompositeShader = new Material(Shader.Find("Hidden/Composite"));
        
        pastFrame = new RenderTexture(Screen.width, Screen.height, 24);

       
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {

        RenderTexture temp = RenderTexture.GetTemporary(lowResWidth, lowResHeigth);
        


        //Composite effect
        CompositeShader.SetTexture("curFrameSampler", src);
        CompositeShader.SetTexture("prevFrameSampler", pastFrame);

        NTSCArtifactTexture.wrapMode = TextureWrapMode.Repeat;
        CompositeShader.SetTexture("NTSCArtifactSampler", NTSCArtifactTexture);

        CompositeShader.SetFloat("Tuning_Sharp", Tuning_Sharp);
        CompositeShader.SetFloat("Tuning_Bleed", Tuning_Bleed);
        CompositeShader.SetFloat("Tuning_NTSC", Tuning_NTSC);
        if (NTSCLerp != 0 && NTSCLerp != 1)
        {
            if (alting == 1)
            {
                alting = 0;
            }
            else {
                alting = 1;
            }
            CompositeShader.SetFloat("NTSCLerp", alting);
            Debug.Log(alting);
        }
        else
        {
            CompositeShader.SetFloat("NTSCLerp", NTSCLerp);
        }
        
        CompositeShader.SetVector("Tuning_Persistence", Tuning_Persistence);
        
        Graphics.Blit(src, dst, CompositeShader);

        
        //Ghosting effect
        GhostingShader.SetFloat("_Intensity", Ghosting_Intensity);
        GhostingShader.SetTexture("_BTex", pastFrame);
        
        Graphics.Blit(dst, temp, GhostingShader);
        Graphics.Blit(temp, dst);
        Graphics.Blit(temp, pastFrame);
        RenderTexture.ReleaseTemporary(temp);
        
    }

}
