using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [System.Serializable]
    public class ColorTransitionPass : ScriptableRenderPass
    {
        private readonly int intensityId = Shader.PropertyToID("_Intensity");
        private readonly int speedId = Shader.PropertyToID("_Speed");
        private readonly int saturationId = Shader.PropertyToID("_Saturation");
        private readonly int brightnessId = Shader.PropertyToID("_Brightness");

        private RTHandle rtHandle;
        private Material effectMaterial;

        public ColorTransitionPass(Shader shader)
        {
            effectMaterial = CoreUtils.CreateEngineMaterial(shader);
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetRTHandle(RTHandle rtHandle)
        {
            this.rtHandle = rtHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // カスタムエフェクトを取得
            VolumeStack stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<ColorTransition>();

            // 無効だったらポストプロセスかけない
            if (!customEffect.IsActive())
                return;

            // CommandBufferを取得
            CommandBuffer cmd = CommandBufferPool.Get("ColorTransition");

            effectMaterial.SetFloat(intensityId, customEffect.intensity.value);
            effectMaterial.SetFloat(saturationId, customEffect.saturation.value);
            effectMaterial.SetFloat(brightnessId, customEffect.brightness.value);
            effectMaterial.SetFloat(speedId, customEffect.speed.value);

            Blitter.BlitCameraTexture(cmd, rtHandle, rtHandle, effectMaterial, 0);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }
    }
}