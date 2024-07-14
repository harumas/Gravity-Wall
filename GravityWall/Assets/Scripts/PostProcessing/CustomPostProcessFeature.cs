using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [System.Serializable]
    public class CustomPostProcessRenderer : ScriptableRendererFeature
    {
        [SerializeField] private Shader colorTransitionShader;
        [SerializeField] private Shader screenSpaceReflectionShader;

        private ColorTransitionPass colorTransitionPass;
        private ScreenSpaceReflectionPass screenSpaceReflectionPass;

        public override void Create()
        {
            colorTransitionPass = new ColorTransitionPass(colorTransitionShader);
            screenSpaceReflectionPass = new ScreenSpaceReflectionPass(screenSpaceReflectionShader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(colorTransitionPass);
            renderer.EnqueuePass(screenSpaceReflectionPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            colorTransitionPass.SetRTHandle(renderer.cameraColorTargetHandle);
            screenSpaceReflectionPass.SetRTHandle(renderer.cameraColorTargetHandle);
        }
    }
}