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
    public class PuzzleStartSequencer : MonoBehaviour
    {
        [SerializeField] private CanvasGroup tutorialCanvas;
        [SerializeField] private float tutorialGuideDelay = 4.5f;

        private Tweener fadeInTween;
        private Tweener fadeOutTween;

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
                OnTutorialGuide().Forget();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                FadeOutCanvas();
            }
        }

        private void FadeInCanvas()
        {
            fadeInTween = DOTween.To(() => tutorialCanvas.alpha, (v) => tutorialCanvas.alpha = v, 1, 1.0f);
        }

        private void FadeOutCanvas()
        {
            fadeOutTween = DOTween.To(() => tutorialCanvas.alpha, (v) => tutorialCanvas.alpha = v, 0, 1.0f);
        }

        private void OnDestroy()
        {
            fadeInTween?.Kill();
            fadeOutTween?.Kill();
        }
    }
}