using Application;
using Application.Sequence;
using CoreModule.Input;
using Cysharp.Threading.Tasks;
using Module.Character;
using Module.InputModule;
using R3;
using UnityEngine;
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
        private readonly PlayerController playerController;
        private readonly PlayerTargetSyncer playerTargetSyncer;
        private readonly GameState gameState;
        private readonly HubSpawner hubSpawner;
        private InputEvent exitEvent;

        [Inject]
        public PauseBehaviourPresenter(
            ViewBehaviourNavigator navigator,
            PauseBehaviour pauseBehaviour,
            CursorLocker cursorLocker,
            GameStopper gameStopper,
            PlayerController playerController,
            PlayerTargetSyncer playerTargetSyncer,
            GameState gameState,
            HubSpawner hubSpawner 
        )
        {
            this.navigator = navigator;
            this.pauseBehaviour = pauseBehaviour;
            this.cursorLocker = cursorLocker;
            this.gameStopper = gameStopper;
            this.playerController = playerController;
            this.playerTargetSyncer = playerTargetSyncer;
            this.gameState = gameState;
            this.hubSpawner = hubSpawner;
        }

        public void Start()
        {
            exitEvent = InputActionProvider.CreateEvent(ActionGuid.UI.ExitScreen);

            navigator.OnStateChanged.Subscribe(state =>
            {
                exitEvent.Started -= OnExitEvent;

                if (state == ViewBehaviourState.Pause || state == ViewBehaviourState.None)
                {
                    exitEvent.Started += OnExitEvent;
                }
            }).AddTo(pauseBehaviour);

            pauseBehaviour.OnActiveStateChanged.Subscribe(context =>
                {
                    if (context.isActive && context.behaviourType == ViewBehaviourState.None)
                    {
                        cursorLocker.SetCursorLock(false);
                        cursorLocker.IsCursorChangeBlock = true;
                        playerTargetSyncer.Lock();
                        playerController.Lock();
                    }
                    else if (!context.isActive && context.behaviourType == ViewBehaviourState.None)
                    {
                        cursorLocker.IsCursorChangeBlock = false;
                        cursorLocker.SetCursorLock(true);
                        playerTargetSyncer.Unlock();
                        playerController.Unlock();
                    }
                })
                .AddTo(pauseBehaviour);

            PauseView pauseView = pauseBehaviour.PauseView;

            pauseView.OnContinueButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Pause)).AddTo(pauseView);
            pauseView.OnReturnToHubButton.Subscribe(_ =>
            {
                gameState.SetState(GameState.State.StageSelect);
                navigator.DeactivateBehaviour(ViewBehaviourState.Pause);
                hubSpawner.Respawn().Forget();
            }).AddTo(pauseView);
            pauseView.OnGoToSettingsButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourState.Option)).AddTo(pauseView);
            pauseView.OnEndGameButtonPressed.Subscribe(_ => gameStopper.Quit());
        }

        private void OnExitEvent(InputAction.CallbackContext _)
        {
            if (pauseBehaviour.IsActive.CurrentValue)
            {
                navigator.DeactivateBehaviour(ViewBehaviourState.Pause);
            }
            else
            {
                navigator.ActivateBehaviour(ViewBehaviourState.Pause);
            }
        }
    }
}