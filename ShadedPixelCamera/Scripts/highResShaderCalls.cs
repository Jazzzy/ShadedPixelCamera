using UnityEngine;
using System.Collections;

public class highResShaderCalls : MonoBehaviour {

    public float intensity;
    private Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Hidden/ntsc"));
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("HighResShader: Source width and height: (" + source.width + ","+source.height+")");
        

        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, material);


        if (destination != null)
        {
            Debug.Log(destination.height);
            Debug.Log(destination.width);
        }
    }


}
