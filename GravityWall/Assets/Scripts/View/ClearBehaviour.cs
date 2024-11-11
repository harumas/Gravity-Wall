using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class ClearBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.Clear;

        [SerializeField] private float clearDelay;

        public async UniTask SequenceClear()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(clearDelay));
            Deactivate(ViewBehaviourType.None);
        }

        protected override UniTask OnPreActivate(ViewBehaviourType type)
        {
            return UniTask.CompletedTask;
        }

        protected override void OnActivate() { }

        protected override void OnDeactivate() { }

        protected override UniTask OnPostDeactivate(ViewBehaviourType type)
        {
            return UniTask.CompletedTask;
        }
    }
}