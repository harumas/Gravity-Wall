using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using View.View;

namespace View
{
    public class CreditBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Credit;

        [SerializeField] private CreditView creditView;
        [SerializeField] private float fadeDuration = 0.3f;

        public CreditView CreditView => creditView;

        protected override void OnActivate()
        {
            creditView.SelectFirst();
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState, CancellationToken cancellation)
        {
            creditView.CanvasGroup.alpha = 0f;
            
            await DOTween.To(() => creditView.CanvasGroup.alpha, (v) => creditView.CanvasGroup.alpha = v, 1f, fadeDuration)
                .WithCancellation(cancellation);
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState, CancellationToken cancellation)
        {
            await DOTween.To(() => creditView.CanvasGroup.alpha, (v) => creditView.CanvasGroup.alpha = v, 0f, fadeDuration)
                .WithCancellation(cancellation);
        }
    }
}