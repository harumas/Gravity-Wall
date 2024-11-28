using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Contents.Shaders
{
    public class EdgeBlurRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private Shader shader;
        [SerializeField] private float strength = 2;
        [SerializeField] private Color backgroundColor;

        private EdgeBlurRendererPass renderPass;

        public override void Create()
        {
            renderPass = new EdgeBlurRendererPass(shader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(renderPass);
        }

        // renderer.cameraColorTargetはSetupRenderPasses内で呼ぶ
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            renderPass.Setup(strength, backgroundColor);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            renderPass?.Dispose();
        }
    }
}