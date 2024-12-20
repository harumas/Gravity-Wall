using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class ClearBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Clear;

        [SerializeField] private float clearDelay;

        public async UniTask SequenceClear()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(clearDelay));
            Deactivate(ViewBehaviourState.None);
        }

        protected override UniTask OnPreActivate(ViewBehaviourState state, CancellationToken cancellation)
        {
            return UniTask.CompletedTask;
        }

        protected override void OnActivate() { }

        protected override void OnDeactivate() { }

        protected override UniTask OnPostDeactivate(ViewBehaviourState state, CancellationToken cancellation)
        {
            return UniTask.CompletedTask;
        }
    }
}