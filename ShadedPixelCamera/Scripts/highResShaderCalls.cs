using UnityEngine;
using System.Collections;

public class highResShaderCalls : MonoBehaviour {

    //public float scaleOfBending = 0.1f;
   
    
    private Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Hidden/bendScreen"));
    }

   

    

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("HighResShader: Source width and height: (" + source.width + ","+source.height+")");





        Graphics.Blit(source, destination);
        return;


        //Here we bend the screen like an old CRT Tube Monitor
        //Right now we are using Unity fisheye shader so we dont need this code.
        //I leave it here in case it is usefull.

        //if (scaleOfBending == 0)
        //{
        //    Graphics.Blit(source, destination);

        //}else{
        //    material.SetFloat("_Scale", scaleOfBending);
        //    Graphics.Blit(source, destination, material);
        //}

        //if (destination != null)
        //{
        //    Debug.Log(destination.height);
        //    Debug.Log(destination.width);
        //}

        //return;
    }


}
