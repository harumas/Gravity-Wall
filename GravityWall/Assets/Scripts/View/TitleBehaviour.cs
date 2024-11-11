using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TitleBehaviour : ViewBehaviour
    {
        public override ViewBehaviourType ViewBehaviourType => ViewBehaviourType.Title;

        [SerializeField] private TitleView titleView;
        private readonly Subject<bool> onCursorLockChange = new Subject<bool>();

        public TitleView TitleView => titleView;
        public Observable<bool> OnCursorLockChange => onCursorLockChange;

        protected override async UniTask OnPreActivate(ViewBehaviourType beforeType)
        {
            if (beforeType == ViewBehaviourType.None)
            {
                onCursorLockChange.OnNext(false);
            }

            await UniTask.CompletedTask;
        }

        protected override void OnActivate() { }

        protected override void OnDeactivate() { }

        protected override async UniTask OnPostDeactivate(ViewBehaviourType nextType)
        {
            if (nextType == ViewBehaviourType.None)
            {
                onCursorLockChange.OnNext(true);
            }

            await UniTask.CompletedTask;
        }
    }
}