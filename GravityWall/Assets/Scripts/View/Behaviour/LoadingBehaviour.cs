using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace View
{
    public class LoadingBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Loading;

        [SerializeField] private float loadingTime;
        [SerializeField] private LoadingView loadingView;
        public LoadingView LoadingView => loadingView;

        public async UniTask SequenceLoading()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
            loadingView.CircleMask.transform.DOScale(Vector3.zero, 0.5f).WaitForCompletion();
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
            loadingView.CircleMask.transform.DOScale(Vector3.one * 30, 1.0f).WaitForCompletion();
            await UniTask.Delay(TimeSpan.FromSeconds(loadingTime));
            await UniTask.CompletedTask;
        }
    }
}