using DG.Tweening;
using UnityEngine;

namespace Module.Effect
{
    public class BlinkController : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;
        [SerializeField] private float blinkTime = 1.0f;
        [SerializeField] private float blinkTimeRandomDelay = 2.0f;
        
        private float blinkTimer = 0;

        private static readonly int rEyeBlunk = Shader.PropertyToID("_REyeBlink");
        private static readonly int lEyeBlunk = Shader.PropertyToID("_LEyeBlink");

        private readonly int faceMaterialEyesIndex = 1;

        private void Start()
        {
            blinkTimer = blinkTime;
        }

        private void Update()
        {
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0)
            {
                float scale = 0;
                DOTween.To(() => scale, (v) => scale = v, 3, 0.15f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnUpdate(() =>
                    {
                        faceMeshRenderer.materials[faceMaterialEyesIndex].SetFloat(rEyeBlunk, scale);
                        faceMeshRenderer.materials[faceMaterialEyesIndex].SetFloat(lEyeBlunk, scale);
                    });

                blinkTimer = Random.Range(blinkTime, blinkTime + blinkTimeRandomDelay);
            }
        }
    }
}
