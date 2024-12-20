using System.Threading;
using Application.Sequence;
using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using View.View;

namespace View
{
    public class PauseBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Pause;

        [SerializeField] private PauseView pauseView;
        [SerializeField] private ClearedLevelView clearedLevelView;
        [SerializeField] private float fadeDuration = 0.3f;
        private GameState gameState;

        public PauseView PauseView => pauseView;
        public ClearedLevelView ClearedLevelView => clearedLevelView;

        public void SetGameState(GameState gameState)
        {
            this.gameState = gameState;
        }

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState, CancellationToken cancellation)
        {
            // ポーズ中は時間を止める
            if (beforeState == ViewBehaviourState.None)
            {
                pauseView.CanvasGroup.alpha = 1f;
                StopTime();
            }
            else
            {
                pauseView.CanvasGroup.alpha = 0f;
                await DOTween.To(() => pauseView.CanvasGroup.alpha, (v) => pauseView.CanvasGroup.alpha = v, 1f, fadeDuration)
                    .SetUpdate(true)
                    .WithCancellation(cancellation);
            }

            pauseView.SetActiveReturnToHubButton(gameState.Current.CurrentValue == GameState.State.Playing);
            pauseView.SetActiveRestartButton(gameState.Current.CurrentValue != GameState.State.StageSelect);

            pauseView.SelectFirst();

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
            // ポーズが終わったら時間停止を再開する
            if (nextState == ViewBehaviourState.None)
            {
                StartTime();
            }

            await DOTween.To(() => pauseView.CanvasGroup.alpha, (v) => pauseView.CanvasGroup.alpha = v, 0f, fadeDuration)
                .SetUpdate(true)
                .WithCancellation(cancellation);
        }

        private void StopTime()
        {
            Time.timeScale = 0f;
        }

        private void StartTime()
        {
            Time.timeScale = 1f;
        }
    }
}