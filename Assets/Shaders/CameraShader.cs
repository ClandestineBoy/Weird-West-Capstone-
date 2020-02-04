using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CameraShader : MonoBehaviour
{
    public Material mat;

    void OnEnable()
    {
        GetComponent<Camera>().farClipPlane = 30;
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
    private void OnDisable()
    {
        GetComponent<Camera>().farClipPlane = 1000;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}

