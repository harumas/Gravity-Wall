using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Constants;
using DG.Tweening;

namespace Module.PlayTest
{
    public class StartSequencer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera titleVirtualCamera;
        [SerializeField] private Volume volume;
        [SerializeField] private CanvasGroup tutorialCanvas;
        private DepthOfField depth;
        public async Task StartSequence()
        {
            await Task.Delay(1000);
            titleVirtualCamera.Priority = 0;
            if (volume.profile.TryGet<DepthOfField>(out depth))
            {
                depth.focusDistance.value = 200;
                depth.focalLength.value = 200;
            }

            await Task.Delay(1500);

            Invoke("OnTutorial", 3);
        }

        Tweener tweener;

        void OnTutorial()
        {
            tweener.Kill();
            DOTween.To(() => tutorialCanvas.alpha, (v) => tutorialCanvas.alpha = v, 1, 1.0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                tweener = DOTween.To(() => tutorialCanvas.alpha, (v) => tutorialCanvas.alpha = v, 0, 1.0f);
            }
        }
    }
}