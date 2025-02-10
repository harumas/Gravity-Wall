using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Constants;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Module.PlayTest
{
    public class StartSequencer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera titleVirtualCamera;
        [SerializeField] private Volume volume;
        [SerializeField] private CanvasGroup tutorialCanvas;
        [SerializeField] private float depthFocusDistance = 200;
        [SerializeField] private float depthFocalLength = 120;
        private readonly float defaultDepthAperture = 17;
        [SerializeField] private float startDelay = 1f;
        [SerializeField] private float endDelay = 1.5f;
        [SerializeField] private float tutorialGuideDelay = 4.5f;

        private DepthOfField depth;
        private Tweener fadeOutTween;

        public async UniTask StartSequence()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: destroyCancellationToken);

            titleVirtualCamera.Priority = 0;
            if (volume.profile.TryGet<DepthOfField>(out depth))
            {
                depth.focusDistance.value = depthFocusDistance;
                depth.focalLength.value = depthFocalLength;
                depth.aperture.value = defaultDepthAperture;
            }

            if (tutorialCanvas != null)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(endDelay), cancellationToken: destroyCancellationToken);
                OnTutorialGuide().Forget();
            }
        }


        private async UniTaskVoid OnTutorialGuide()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(tutorialGuideDelay), cancellationToken: destroyCancellationToken);

            fadeOutTween.Kill();
            FadeInCanvas();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                fadeOutTween = FadeOutCanvas();
            }
        }

        private void FadeInCanvas()
        {
            DOTween.To(() => tutorialCanvas.alpha, (v) => tutorialCanvas.alpha = v, 1, 1.0f);
        }

        private Tweener FadeOutCanvas()
        {
            return DOTween.To(() => tutorialCanvas.alpha, (v) => tutorialCanvas.alpha = v, 0, 1.0f);
        }
    }
}