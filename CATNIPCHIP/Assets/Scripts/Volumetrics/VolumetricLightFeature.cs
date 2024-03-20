using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

public enum DownSample { off = 1, half = 2, third = 3, quarter = 4 };

public class VolumetricLightFeature : ScriptableRendererFeature
{
    [Serializable]
    public class Settings
    {
        public DownSample downsampling = DownSample.off;
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public float intensity = 1;
        public float scattering = -0.4f;
        public float steps = 25;
        public float maxDistance = 75;
        public float jitter;
        public GaussBlur gaussBlur;
    }

    [System.Serializable]
    public class GaussBlur
    {
        public float amount;
        public float samples;
    }

    public Settings settings = new Settings();

    class Pass : ScriptableRenderPass
    {
        public Settings settings;
        private RTHandle _source;
        private RTHandle _tempTexture;
        private RTHandle _tempTexture2;
        private RTHandle _tempTexture3;

        private string _profilerTag;

        public void Setup(RTHandle source)
        {
            _source = source;
        }

        public Pass(string profilerTag)
        {
            _profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor original = cameraTextureDescriptor;
            int divider = (int)settings.downsampling;

            // Applies down sampling.
            if (Camera.current != null)
            {
                cameraTextureDescriptor.width = (int)Camera.current.pixelRect.width / divider;
                cameraTextureDescriptor.height = (int)Camera.current.pixelRect.height / divider;
            }
            else
            {
                cameraTextureDescriptor.width /= divider;
                cameraTextureDescriptor.height /= divider;
            }

            cameraTextureDescriptor.colorFormat = RenderTextureFormat.R16;
            cameraTextureDescriptor.msaaSamples = 1;

            //cmd.GetTemporaryRT(_tempTexture., cameraTextureDescriptor);
            //ConfigureTarget(_tempTexture.Identifier());

            RenderingUtils.ReAllocateIfNeeded(ref _tempTexture, Vector2.one / divider, cameraTextureDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempTexture");
            ConfigureTarget(_tempTexture);

            RenderingUtils.ReAllocateIfNeeded(ref _tempTexture2, Vector2.one / divider, cameraTextureDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempTexture2");
            ConfigureTarget(_tempTexture2);

            RenderingUtils.ReAllocateIfNeeded(ref _tempTexture3, Vector2.one, original, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempTexture3");
            ConfigureTarget(_tempTexture3);



            //_tempTexture2.id = 1;
            //cmd.GetTemporaryRT(_tempTexture2.id, cameraTextureDescriptor);
            //ConfigureTarget(_tempTexture2.Identifier());

            //_tempTexture3.id = 2;
            //cmd.GetTemporaryRT(_tempTexture3.id, original);
            //ConfigureTarget(_tempTexture3.Identifier());

            //colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;


            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);
            cmd.Clear();

            try
            {
                // Sets properties.
                settings.material.SetFloat("_Scattering", settings.scattering);
                settings.material.SetFloat("_Steps", settings.steps);
                settings.material.SetFloat("_JitterVolumetric", settings.jitter);
                settings.material.SetFloat("_MaxDistance", settings.maxDistance);
                settings.material.SetFloat("_Intensity", settings.intensity);
                settings.material.SetFloat("_GaussSamples", settings.gaussBlur.samples);
                settings.material.SetFloat("_GaussAmount", settings.gaussBlur.amount);

                // Raymarch.
                cmd.Blit(_source, _tempTexture, settings.material, 0);

                // Bilateral blur x.
                cmd.Blit(_tempTexture, _tempTexture2, settings.material, 1);

                // Bilateral blur y.
                cmd.Blit(_tempTexture2, _tempTexture, settings.material, 2);
                cmd.SetGlobalTexture("_VolumetricTexture", _tempTexture);

                // Downsample depth.
                cmd.Blit(_source, _tempTexture2, settings.material, 4);
                cmd.SetGlobalTexture("_LowResDepth", _tempTexture2);

                // Upsample and composite
                cmd.Blit(_source, _tempTexture3, settings.material, 3);
                cmd.Blit(_tempTexture3, _source);

                context.ExecuteCommandBuffer(cmd);
            }
            catch
            {
                Debug.LogError($"{nameof(VolumetricLightFeature)} Error");
            }

            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    private Pass _pass;

    public override void Create()
    {
        _pass = new Pass("Volumetric Light");
        name = "Volumetric Light";
        _pass.settings = settings;
        _pass.renderPassEvent = settings.renderPassEvent;
    }

    protected override void Dispose(bool disposing)
    {
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        //base.SetupRenderPasses(renderer, renderingData);
        RTHandle cameraColorTargetIdent = renderer.cameraColorTargetHandle;
        _pass.Setup(cameraColorTargetIdent);

    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
}
