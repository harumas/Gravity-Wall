using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Contents.Shaders
{
    public class EdgeBlurRendererPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(EdgeBlurRendererPass);
        private readonly Material material;
        private RTHandle targetHandle;

        private static readonly int strengthId = Shader.PropertyToID("_Strength"); //プロパティIDを取得
        private static readonly int backgroundColorId = Shader.PropertyToID("_BackgroundColor"); //プロパティIDを取得
        private static readonly int depthMapId = Shader.PropertyToID("_DepthMap"); //プロパティIDを取得

        private float strength;
        private Color backgroundColor;

        public EdgeBlurRendererPass(Shader shader)
        {
            material = CoreUtils.CreateEngineMaterial(shader);
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void Setup(float strength, Color backgroundColor)
        {
            this.strength = strength;
            this.backgroundColor = backgroundColor;

            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.width = Math.Max(2, (int)(desc.width * strength));
            desc.height = Math.Max(2, (int)(desc.height * strength));

            RenderingUtils.ReAllocateIfNeeded(ref targetHandle, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_BlurTexture");
        }

        public void Dispose()
        {
            targetHandle?.Release();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraColorRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
            if (renderingData.cameraData.isSceneViewCamera || cameraColorRT == null || cameraColorRT.rt == null)
            {
                return;
            }

            var cmd = CommandBufferPool.Get(ProfilerTag);

            material.SetFloat(strengthId, strength);
            material.SetColor(backgroundColorId, backgroundColor);

            // 最後にdestにBlit
            Blitter.BlitCameraTexture(cmd, cameraColorRT, targetHandle, material, 0);
            material.SetTexture(depthMapId, targetHandle.rt);
            Blitter.BlitCameraTexture(cmd, cameraColorRT, cameraColorRT, material, 1);

            // Blitter.BlitCameraTexture(cmd, cameraColorRT, cameraColorRT, material, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}