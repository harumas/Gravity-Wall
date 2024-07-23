using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [System.Serializable]
    public class ScreenSpaceReflectionPass : ScriptableRenderPass
    {
        private readonly int intensityId = Shader.PropertyToID("_Intensity");

        private RTHandle rtHandle;
        private Material effectMaterial;

        public ScreenSpaceReflectionPass(Shader shader)
        {
            effectMaterial = CoreUtils.CreateEngineMaterial(shader);
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetRTHandle(RTHandle rtHandle)
        {
            this.rtHandle = rtHandle;
        }

        // The actual execution of the pass. This is where custom rendering occurs.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // カスタムエフェクトを取得
            VolumeStack stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<ScreenSpaceReflection>();

            // 無効だったらポストプロセスかけない
            if (!customEffect.IsActive())
                return;

            // CommandBufferを取得
            CommandBuffer cmd = CommandBufferPool.Get("ScreenSpaceReflection");

            using (new ProfilingScope(cmd, profilingSampler))
            {
                effectMaterial.SetFloat(intensityId, customEffect.intensity.value);
                effectMaterial.SetVector("_BlitScaleBias", new Vector4(1, 1, 0, 0));

                Blitter.BlitCameraTexture(cmd, rtHandle, rtHandle, effectMaterial, 0);
            }

            context.ExecuteCommandBuffer(cmd);


            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }
}