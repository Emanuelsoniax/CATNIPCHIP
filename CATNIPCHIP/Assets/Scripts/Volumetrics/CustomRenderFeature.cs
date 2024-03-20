using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderFeature : ScriptableRendererFeature
{
    [Serializable]
    public class Settings
    {
        public DownSample downsampling = DownSample.off;
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public Settings settings = new Settings();

    class Pass : ScriptableRenderPass
    {
        public Settings settings;
        private RenderTargetIdentifier _source;
        private RenderTargetHandle _tempTexture;

        private string _profilerTag;

        public Pass(string profilerTag)
        {
            _profilerTag = profilerTag;
        }

        public void Setup(RenderTargetIdentifier source)
        {
            _source = source;
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

            cameraTextureDescriptor.colorFormat = RenderTextureFormat.ARGB64;
            cameraTextureDescriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(_tempTexture.id, cameraTextureDescriptor);
            ConfigureTarget(_tempTexture.Identifier());

            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);
            cmd.Clear();

            try
            {
                cmd.Blit(_source, _tempTexture.Identifier());
                cmd.Blit(_tempTexture.Identifier(), _source, settings.material, 0);

                context.ExecuteCommandBuffer(cmd);
            }
            catch
            {
                Debug.LogError($"{nameof(CustomRenderFeature)} Error");
            }

            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    private Pass _pass;

    public override void Create()
    {
        _pass = new Pass("Custom Renderer");
        name = "Custom Renderer";
        _pass.settings = settings;
        _pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        RenderTargetIdentifier cameraColorTargetIdent = renderer.cameraColorTarget;
        _pass.Setup(cameraColorTargetIdent);
        renderer.EnqueuePass(_pass);
    }
}
