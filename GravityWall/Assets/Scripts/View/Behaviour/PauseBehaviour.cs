using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class PauseBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Pause;

        [SerializeField] private PauseView pauseView;
        public PauseView PauseView => pauseView;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState)
        {
            // ポーズ中は時間を止める
            if (beforeState == ViewBehaviourState.None)
            {
                StopTime();
            }

            pauseView.SelectFirst();

            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState)
        {
            // ポーズが終わったら時間停止を再開する
            if (nextState == ViewBehaviourState.None)
            {
                StartTime();
            }

            await UniTask.CompletedTask;
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