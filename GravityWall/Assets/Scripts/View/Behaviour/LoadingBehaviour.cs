using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class LoadingBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Loading;

        [SerializeField] private float loadingTime;

        public async UniTask SequenceLoading()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(loadingTime));
        }

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState)
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate() { }

        protected override void OnDeactivate() { }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState)
        {
            await UniTask.CompletedTask;
        }
    }
}