using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace View
{
    public class TitleBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Title;

        [SerializeField] private TitleView titleView;
        [SerializeField] private float deactivationDelay = 0.5f;
        [SerializeField] private float fadeDuration = 0.3f;
        private readonly Subject<bool> onCursorLockChange = new Subject<bool>();

        public TitleView TitleView => titleView;
        public Observable<bool> OnCursorLockChange => onCursorLockChange;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState, CancellationToken cancellation)
        {
            // インゲームからタイトルに戻るときにカーソルをロック解除する
            if (beforeState == ViewBehaviourState.None)
            {
                onCursorLockChange.OnNext(false);
            }

            titleView.CanvasGroup.alpha = 0.0f;
            DOTween.To(() => titleView.CanvasGroup.alpha, (v) => titleView.CanvasGroup.alpha = v, 1f, fadeDuration);
            
            await UniTask.Yield();
            
            titleView.SelectFirst();

            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState, CancellationToken cancellation)
        {
            await DOTween.To(() => titleView.CanvasGroup.alpha, (v) => titleView.CanvasGroup.alpha = v, 0, fadeDuration)
                .WithCancellation(cancellation);

            // タイトルからインゲームに遷移するときにカーソルをロックする
            if (nextState == ViewBehaviourState.None)
            {
                if (titleView.StartSequencer != null)
                {
                    await titleView.StartSequencer.StartSequence();
                }

                onCursorLockChange.OnNext(true);
            }
        }
    }
}