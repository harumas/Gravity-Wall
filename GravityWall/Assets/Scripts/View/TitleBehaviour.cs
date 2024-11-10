using Cysharp.Threading.Tasks;

namespace View
{
    public class TitleBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.Title;

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