using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class CreditBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Credit;

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState)
        {
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState)
        {
        }
    }
}