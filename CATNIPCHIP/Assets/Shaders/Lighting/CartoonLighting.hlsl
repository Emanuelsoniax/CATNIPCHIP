#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
    #if (SHADERPASS != SHADERPASS_FORWARD)
        #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    #endif
#endif

struct CartoonLightingData
{
    float3 positionWS;
    float3 normalWS;
    float3 viewDirectionWS;
    float3 albedo;
    float4 shadowCoord;
    float smoothness;
    float metallic;
    float sheen;
    float ambientOcclusion;
    float3 bakedGI;
    float fogFactor;
};

float sheenIntensity = 1;

float GetSmoothnessPower(float rawSmoothness)
{
    return exp2(10 * rawSmoothness + 1);
}


#ifndef SHADERGRAPH_PREVIEW

float3 CartoonGlobalIllumination(CartoonLightingData d)
{
    float3 indirectDiffuse = d.albedo * d.bakedGI * d.ambientOcclusion;

    float3 reflectVector = reflect(-d.viewDirectionWS, d.normalWS);
    
    float fresnel = Pow4(1 - saturate(dot(d.viewDirectionWS, d.normalWS)));
    
    float3 indirectSpecular = GlossyEnvironmentReflection(reflectVector,
        RoughnessToPerceptualRoughness(1 - d.smoothness),
        d.ambientOcclusion) * fresnel;
    
    return indirectDiffuse + indirectSpecular * d.smoothness;
}

float3 CartoonLightHandling(CartoonLightingData d, Light light)
{
    float atten = light.distanceAttenuation * light.shadowAttenuation;
    
    
    float3 radiance = light.color * step(.1, light.shadowAttenuation) * light.distanceAttenuation;
    
    float raw_diffuse = saturate(dot(d.normalWS, light.direction));
    float diffuse = step(.01f, raw_diffuse);
    
    float3 reflectionDir = reflect(d.viewDirectionWS, d.normalWS);
    float specularDot = saturate(dot(reflectionDir, -light.direction));
    float specular = step(.1, pow(specularDot, GetSmoothnessPower(d.smoothness)));
    
    float3 diffuse_color = diffuse * atten * light.color * d.albedo * (1 - d.metallic);
    float3 specular_color = specular * atten * light.color * (d.metallic * d.albedo + (1 - d.metallic) * light.color) * d.smoothness;

    float fresnel = step(1 - d.sheen, Pow4(1 - saturate(dot(d.viewDirectionWS, d.normalWS)))) * raw_diffuse * atten;
    
    float3 color = diffuse_color + specular_color + fresnel * light.color;

    return color;
}
#endif

float3 CalculateCartoonLighting(CartoonLightingData d)
{
    #ifdef SHADERGRAPH_PREVIEW
    float3 lightDir = float3(0.5, 0.5, 0);
    float intensity = saturate(dot(d.normalWS, lightDir));
    return d.albedo * intensity;
    #else
    Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, 1);
    
    MixRealtimeAndBakedGI(mainLight, d.normalWS, d.bakedGI);
    float3 color = CartoonGlobalIllumination(d);
    
    color += CartoonLightHandling(d, mainLight);
    
    #ifdef _ADDITIONAL_LIGHTS

        uint numAdditionalLights = GetAdditionalLightsCount();
        for (uint iLight = 0; iLight < numAdditionalLights; iLight++)
        {
            Light light = GetAdditionalLight(iLight, d.positionWS, 1);
            color += CartoonLightHandling(d, light);
        }
    #endif
    
    color = MixFog(color, d.fogFactor);

    return color;
    #endif
}

void CalculateCartoonLighting_float(float3 Position, float3 Normal, float3 ViewDirection, float3 Albedo, float Smoothness, float Metallic, float Sheen, float AmbientOcclusion, float2 LightmapUV, out float3 Color)
{
    CartoonLightingData d;
    d.normalWS = Normal;
    d.albedo = Albedo;
    d.viewDirectionWS = ViewDirection;
    d.smoothness = Smoothness;
    d.ambientOcclusion = AmbientOcclusion;
    d.smoothness = Smoothness;
    d.metallic = Metallic;
    d.sheen = Sheen;
    
    d.positionWS = Position;
    
    #ifdef SHADERGRAPH_PREVIEW
        d.shadowCoord = 0;
        d.bakedGI = 0;
        d.fogFactor = 0;
    #else
    float4 positionCS = TransformWorldToHClip(Position);
        #if SHADOWS_SCREEN
            d.shadowCoord = ComputeScreenPos(positionCS);
        #else
            d.shadowCoord = TransformWorldToShadowCoord(Position);
        #endif
    
    float2 lightmapUV;
    OUTPUT_LIGHTMAP_UV(LightmapUV, unity_LightmapST, lightmapUV);
    float3 vertexSH;
    OUTPUT_SH(Normal, vertexSH);
    d.bakedGI = SAMPLE_GI(lightmapUV, vertexSH, Normal);
    
    d.fogFactor = ComputeFogFactor(positionCS.z);
    #endif
    
    Color = CalculateCartoonLighting(d);

}

void CalculateCartoonLighting_half(float3 Position, float3 Normal, float3 ViewDirection, float3 Albedo, float Smoothness, float Metallic, float Sheen, float AmbientOcclusion, float2 LightmapUV, out float3 Color)
{
    CalculateCartoonLighting_float(Position, Normal, ViewDirection
    , Albedo, Smoothness, Metallic, Sheen, AmbientOcclusion, LightmapUV, Color);
}

#endif