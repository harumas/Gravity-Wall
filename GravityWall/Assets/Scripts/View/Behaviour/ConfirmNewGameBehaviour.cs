using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View.Behaviour
{
    public class ConfirmNewGameBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.ConfirmNewGame;

        [SerializeField] private ConfirmNewGameView confirmNewGameView;
        public ConfirmNewGameView ConfirmNewGameView => confirmNewGameView;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState, CancellationToken cancellation)
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
            confirmNewGameView.SelectFirst();
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState, CancellationToken cancellation)
        {
            await UniTask.CompletedTask;
        }
    }
}