using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TitleBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.Title;

        [SerializeField] private TitleView titleView;
        private readonly Subject<bool> onCursorLockChange = new Subject<bool>();

        public TitleView TitleView => titleView;
        public Observable<bool> OnCursorLockChange => onCursorLockChange;

        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState)
        {
            // インゲームからタイトルに戻るときにカーソルをロック解除する
            if (beforeState == ViewBehaviourState.None)
            {
                onCursorLockChange.OnNext(false);
            }
            
            titleView.SelectFirst();

            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate() { }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState)
        {
            // タイトルからインゲームに遷移するときにカーソルをロックする
            if (nextState == ViewBehaviourState.None)
            {
                onCursorLockChange.OnNext(true);
            }

            await UniTask.CompletedTask;
        }
    }
}