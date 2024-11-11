using Application;
using CoreModule.Input;
using Module.InputModule;
using R3;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class PauseBehaviourPresenter : IStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly PauseBehaviour pauseBehaviour;
        private readonly CursorLocker cursorLocker;
        private readonly GameStopper gameStopper;
        private InputEvent exitEvent;

        [Inject]
        public PauseBehaviourPresenter(
            ViewBehaviourNavigator navigator,
            PauseBehaviour pauseBehaviour,
            CursorLocker cursorLocker,
            GameStopper gameStopper
        )
        {
            this.navigator = navigator;
            this.pauseBehaviour = pauseBehaviour;
            this.cursorLocker = cursorLocker;
            this.gameStopper = gameStopper;
        }

        public void Start()
        {
            exitEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ExitScreen);
            exitEvent.Started += OnExitEvent;

            pauseBehaviour.OnActiveStateChanged.Subscribe(context =>
                {
                    if (context.isActive && context.behaviourType == ViewBehaviourType.None)
                    {
                        cursorLocker.SetCursorLock(false);
                    }
                    else if (!context.isActive && context.behaviourType == ViewBehaviourType.None)
                    {
                        cursorLocker.SetCursorLock(true);
                    }
                    else if (!context.isActive && context.behaviourType != ViewBehaviourType.None)
                    {
                        exitEvent.Started -= OnExitEvent;
                    }
                    else if (context.isActive && context.behaviourType != ViewBehaviourType.None)
                    {
                        exitEvent.Started += OnExitEvent;
                    }
                })
                .AddTo(pauseBehaviour);

            PauseView pauseView = pauseBehaviour.PauseView;

            pauseView.OnContinueButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourType.Pause)).AddTo(pauseView);
            pauseView.OnGoToSettingsButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourType.Option)).AddTo(pauseView);
            pauseView.OnEndGameButtonPressed.Subscribe(_ => gameStopper.Quit());
        }

        private void OnExitEvent(InputAction.CallbackContext _)
        {
            if (pauseBehaviour.IsActive.CurrentValue)
            {
                navigator.DeactivateBehaviour(ViewBehaviourType.Pause);
            }
            else
            {
                navigator.ActivateBehaviour(ViewBehaviourType.Pause);
            }
        }
    }
}