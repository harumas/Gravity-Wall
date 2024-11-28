using Application;
using Module.InputModule;
using Module.Player;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class TitleBehaviourPresenter : IStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly TitleBehaviour titleBehaviour;
        private readonly GameStopper gameStopper;
        private readonly CursorLocker cursorLocker;
        private readonly PlayerController playerController;
        private readonly PlayerTargetSyncer playerTargetSyncer;

        [Inject]
        public TitleBehaviourPresenter(
            ViewBehaviourNavigator navigator,
            TitleBehaviour titleBehaviour,
            GameStopper gameStopper,
            CursorLocker cursorLocker,
            PlayerController playerController,
            PlayerTargetSyncer playerTargetSyncer)
        {
            this.navigator = navigator;
            this.titleBehaviour = titleBehaviour;
            this.gameStopper = gameStopper;
            this.cursorLocker = cursorLocker;
            this.playerController = playerController;
            this.playerTargetSyncer = playerTargetSyncer;
        }

        public void Start()
        {
            TitleView titleView = titleBehaviour.TitleView;

            //ゲーム終了は一度だけ入力を受け取る
            titleView.OnEndGameButtonPressed.Take(1).Subscribe(_ => gameStopper.Quit());
            titleView.OnCreditButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourState.Credit));
            titleView.OnNewGameButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Title));
            titleView.OnContinueGameButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Title));

            //カーソルロックの変更に応じてプレイヤーの操作をロックする
            titleBehaviour.OnCursorLockChange.Subscribe(isLock =>
            {
                cursorLocker.SetCursorLock(isLock);

                if (isLock)
                {
                    playerTargetSyncer.Unlock();
                    playerController.Unlock();
                }
                else
                {
                    playerTargetSyncer.Lock();
                    playerController.Lock();
                }
            });
        }
    }
}