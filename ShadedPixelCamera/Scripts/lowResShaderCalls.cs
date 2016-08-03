using UnityEngine;
using System.Collections;

public class lowResShaderCalls : MonoBehaviour {

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
        Debug.Log("LowResShader: Source width and height: (" + source.width + "," + source.height + ")");


        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }
        Debug.Log(material);

        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, material);


        if (destination != null)
        {
            Debug.Log("LowResShader: Destination width and height: (" + destination.width + "," + destination.height + ")");
        }
    }
}
