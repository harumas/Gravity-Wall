using Cysharp.Threading.Tasks;
using Module.InputModule;
using UnityEngine;
using VContainer;

namespace View
{
    public class PauseBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.Pause;

        private CursorLocker cursorLocker;
        [SerializeField] private PauseView pauseView;
        public PauseView PauseView => pauseView;

        [Inject]
        private void Construct(CursorLocker cursorLocker)
        {
            this.cursorLocker = cursorLocker;
        }

        protected override async UniTask OnPreActivate(ViewBehaviourType beforeType)
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
            cursorLocker.IsCursorChangeBlock = true;
            cursorLocker.SetCursorLock(true);

            Time.timeScale = 0f;
        }

        protected override void OnDeactivate()
        {
            cursorLocker.IsCursorChangeBlock = false;
            cursorLocker.SetCursorLock(false);

            Time.timeScale = 1f;
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourType nextType)
        {
            await UniTask.CompletedTask;
        }
    }
}