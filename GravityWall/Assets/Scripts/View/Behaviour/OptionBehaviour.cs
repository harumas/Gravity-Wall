using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace View
{
    public class OptionBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Option;

        [SerializeField] private OptionView optionView;
        [SerializeField] private float fadeDuration = 0.3f;
        public OptionView OptionView => optionView;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState, CancellationToken cancellation)
        {
            optionView.CanvasGroup.alpha = 0f;
            await DOTween.To(() => optionView.CanvasGroup.alpha, (v) => optionView.CanvasGroup.alpha = v, 1f, fadeDuration)
                .SetUpdate(true)
                .WithCancellation(cancellation);
        }

        protected override void OnActivate()
        {
            optionView.SelectFirst();
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState, CancellationToken cancellation)
        {
            optionView.CanvasGroup.alpha = 1f;
            await DOTween.To(() => optionView.CanvasGroup.alpha, (v) => optionView.CanvasGroup.alpha = v, 0f, fadeDuration)
                .SetUpdate(true)
                .WithCancellation(cancellation);
        }
    }
}