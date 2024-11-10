using Cysharp.Threading.Tasks;

namespace View
{
    public class LicenseBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.License;

        protected override async UniTask OnPreActivate()
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPostDeactivate()
        {
            await UniTask.CompletedTask;
        }
    }
}