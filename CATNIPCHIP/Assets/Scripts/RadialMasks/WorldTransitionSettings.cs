using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldTransitionSettings
{
    [ColorUsage(true, true)]
    public Color transitionColor = new Color(0,2,4);
    public float fadeOffset = .55f;
    public float noiseFadeOffset = .17f;
    public float noiseScale = .12f;
    public Texture2D noiseTexture;

    public void SetGlobalShaderProperties()
    {
        Shader.SetGlobalColor("_GlobalTransitionColor", transitionColor);
        Shader.SetGlobalFloat("_GlobalFadeOffset", fadeOffset);
        Shader.SetGlobalFloat("_GlobalNoiseFadeOffset", noiseFadeOffset);
        Shader.SetGlobalFloat("_GlobalNoiseScale", noiseScale);
        Shader.SetGlobalTexture("_GlobalNoiseTexture", noiseTexture);
    } 
}
