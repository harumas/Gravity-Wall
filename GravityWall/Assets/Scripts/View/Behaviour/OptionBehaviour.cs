using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class OptionBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Option;

        [SerializeField] private OptionView optionView;

        public OptionView OptionView => optionView;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState)
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
            optionView.SelectFirst();
        }

        protected override void OnDeactivate() { }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState)
        {
            await UniTask.CompletedTask;
        }
    }
}