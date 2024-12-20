using Application;
using Application.Sequence;
using Application.Spawn;
using CoreModule.Input;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.InputModule;
using Module.Player;
using R3;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using View;
using View.View;

namespace Presentation
{
    public class PauseBehaviourPresenter : IStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly PauseBehaviour pauseBehaviour;
        private readonly CursorLocker cursorLocker;
        private readonly GamepadVibrator gamepadVibrator;
        private readonly ApplicationStopper applicationStopper;
        private readonly PlayerController playerController;
        private readonly PlayerTargetSyncer playerTargetSyncer;
        private readonly GameState gameState;
        private readonly HubSpawner hubSpawner;
        private readonly SaveManager<SaveData> saveManager;
        private InputEvent exitEvent;

        [Inject]
        public PauseBehaviourPresenter(
            ViewBehaviourNavigator navigator,
            PauseBehaviour pauseBehaviour,
            CursorLocker cursorLocker,
            GamepadVibrator gamepadVibrator,
            ApplicationStopper applicationStopper,
            PlayerController playerController,
            PlayerTargetSyncer playerTargetSyncer,
            GameState gameState,
            HubSpawner hubSpawner,
            SaveManager<SaveData> saveManager
        )
        {
            this.navigator = navigator;
            this.pauseBehaviour = pauseBehaviour;
            this.cursorLocker = cursorLocker;
            this.gamepadVibrator = gamepadVibrator;
            this.applicationStopper = applicationStopper;
            this.playerController = playerController;
            this.playerTargetSyncer = playerTargetSyncer;
            this.gameState = gameState;
            this.hubSpawner = hubSpawner;
            this.saveManager = saveManager;
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
                        gamepadVibrator.Pause();
                    }
                    else if (!context.isActive && context.behaviourType == ViewBehaviourState.None)
                    {
                        cursorLocker.IsCursorChangeBlock = false;
                        cursorLocker.SetCursorLock(true);
                        
                        playerTargetSyncer.Unlock();
                        playerController.Unlock();
                        gamepadVibrator.Resume();
                    }
                })
                .AddTo(pauseBehaviour);
            
            pauseBehaviour.SetGameState(gameState);

            PauseView pauseView = pauseBehaviour.PauseView;

            pauseView.OnContinueButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Pause)).AddTo(pauseView);
            pauseView.OnReturnToHubButtonPressed.Subscribe(_ =>
            {
                gameState.SetState(GameState.State.StageSelect);
                navigator.DeactivateBehaviour(ViewBehaviourState.Pause);
                hubSpawner.Respawn().Forget();
            }).AddTo(pauseView);
            pauseView.OnGoToSettingsButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourState.Option)).AddTo(pauseView);
            pauseView.OnEndGameButtonPressed.Subscribe(_ => applicationStopper.Quit());

            ClearedLevelView clearedLevelView = pauseBehaviour.ClearedLevelView;
            clearedLevelView.SetClearedLevels(saveManager.Data.ClearedStageList);
            saveManager.OnSaved -= SetClearedLevels;
            saveManager.OnSaved += SetClearedLevels;
        }
        
        private void SetClearedLevels(SaveData saveData)
        {
            pauseBehaviour.ClearedLevelView.SetClearedLevels(saveData.ClearedStageList);
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