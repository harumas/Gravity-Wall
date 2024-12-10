using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
        [SerializeField] private float depthFocusDistance = 200;
        [SerializeField] private float depthFocalLength = 200;
        private DepthOfField depth;
        public async Task StartSequence()
        {
            await Task.Delay(1000);
            titleVirtualCamera.Priority = 0;
            if (volume.profile.TryGet<DepthOfField>(out depth))
            {
                depth.focusDistance.value = depthFocusDistance;
                depth.focalLength.value = depthFocalLength;
            }

            await Task.Delay(1500);

            Invoke("OnTutorialGuide", 3);
        }

        Tweener tweener;

        void OnTutorialGuide()
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