using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TitleBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Title;

        [SerializeField] private TitleView titleView;
        private readonly Subject<bool> onCursorLockChange = new Subject<bool>();

        public TitleView TitleView => titleView;
        public Observable<bool> OnCursorLockChange => onCursorLockChange;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState)
        {
            // インゲームからタイトルに戻るときにカーソルをロック解除する
            if (beforeState == ViewBehaviourState.None)
            {
                onCursorLockChange.OnNext(false);
                titleView.FadeCanvasGroup.alpha = 1.0f;
                DOTween.To(() => titleView.FadeCanvasGroup.alpha, (v) => titleView.FadeCanvasGroup.alpha = v, 0, 0.3f);
                titleView.FadeCanvasGroup.blocksRaycasts = false;
            }

            titleView.SelectFirst();

            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate() { }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState)
        {
            // タイトルからインゲームに遷移するときにカーソルをロックする
            if (nextState == ViewBehaviourState.None)
            {
                await Task.Delay(500);

                DOTween.To(() => titleView.CanvasGroup.alpha, (v) => titleView.CanvasGroup.alpha = v, 0, 0.3f);

                if (titleView.StartSequencer != null)
                {
                    await titleView.StartSequencer.StartSequence();
                }

                onCursorLockChange.OnNext(true);
            }

            await UniTask.CompletedTask;
        }
    }
}