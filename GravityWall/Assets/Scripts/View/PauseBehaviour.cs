using Cysharp.Threading.Tasks;
using Module.InputModule;
using VContainer;

namespace View
{
    public class PauseBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.Pause;

        private CursorLocker cursorLocker;

        [Inject]
        private void Construct(CursorLocker cursorLocker)
        {
            this.cursorLocker = cursorLocker;
        }

        protected override async UniTask OnPreActivate()
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
            cursorLocker.IsCursorChangeBlock = true;
            cursorLocker.SetCursorLock(true);
        }

        protected override void OnDeactivate()
        {
            cursorLocker.IsCursorChangeBlock = false;
            cursorLocker.SetCursorLock(false);
        }

        protected override async UniTask OnPostDeactivate()
        {
            await UniTask.CompletedTask;
        }
    }
}