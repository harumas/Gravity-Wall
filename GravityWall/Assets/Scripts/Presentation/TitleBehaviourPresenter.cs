using Application;
using Module.InputModule;
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

        [Inject]
        public TitleBehaviourPresenter(
            ViewBehaviourNavigator navigator,
            TitleBehaviour titleBehaviour,
            GameStopper gameStopper,
            CursorLocker cursorLocker
        )
        {
            this.navigator = navigator;
            this.titleBehaviour = titleBehaviour;
            this.gameStopper = gameStopper;
            this.cursorLocker = cursorLocker;
        }

        public void Start()
        {
            TitleView titleView = titleBehaviour.TitleView;

            //一度だけ入力を受け取る
            titleView.OnEndGameButtonPressed.Take(1).Subscribe(_ => gameStopper.Quit());
            titleView.OnLicenseButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourType.License));
            titleView.OnNewGameButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourType.Title));
            titleView.OnContinueGameButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourType.Title));

            titleBehaviour.OnCursorLockChange.Subscribe(isLock =>
            {
                cursorLocker.SetCursorLock(isLock);
            });
        }
    }
}