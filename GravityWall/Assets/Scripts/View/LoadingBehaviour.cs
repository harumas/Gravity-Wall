using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class LoadingBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.Loading;

        [SerializeField] private float loadingTime;

        public async UniTask SequenceLoading()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(loadingTime));
        }

        protected override async UniTask OnPreActivate(ViewBehaviourType beforeType)
        {
            await UniTask.CompletedTask;
        }

        protected override void OnActivate() { }

        protected override void OnDeactivate() { }

        protected override async UniTask OnPostDeactivate(ViewBehaviourType nextType)
        {
            await UniTask.CompletedTask;
        }
    }
}