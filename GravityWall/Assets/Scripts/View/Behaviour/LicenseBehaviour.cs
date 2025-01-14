using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace View
{
    public class LicenseBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.License;

        [SerializeField] private LicenseView licenseView;
        [SerializeField] private float fadeDuration = 0.3f;
        public LicenseView LicenseView => licenseView;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState, CancellationToken cancellation)
        {
            licenseView.SelectFirst();
            licenseView.CanvasGroup.alpha = 0f;
            await DOTween.To(() => licenseView.CanvasGroup.alpha, (v) => licenseView.CanvasGroup.alpha = v, 1f, fadeDuration)
                .SetUpdate(true)
                .WithCancellation(cancellation);
        }

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState, CancellationToken cancellation)
        {
            licenseView.CanvasGroup.alpha = 1f;
            await DOTween.To(() => licenseView.CanvasGroup.alpha, (v) => licenseView.CanvasGroup.alpha = v, 0f, fadeDuration)
                .SetUpdate(true)
                .WithCancellation(cancellation);
        }
    }
}