using System;
using System.Threading;
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
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using View;
using View.View;

namespace Presentation
{
    public class PauseBehaviourPresenter : IStartable, IDisposable
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

        private readonly ReactiveProperty<bool> doInput;
        
        private readonly float endGameDelay = 0.5f;

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
            InputLocker inputLocker,
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

            doInput = new ReactiveProperty<bool>(true);
            inputLocker.AddCondition(doInput, pauseBehaviour.destroyCancellationToken);
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
                })
                .AddTo(pauseBehaviour);

            pauseBehaviour.OnActiveStateChanged.Subscribe(OnActiveStateChanged).AddTo(pauseBehaviour);
            pauseBehaviour.Construct(gameState, playerController.LockState);

            PauseView pauseView = pauseBehaviour.PauseView;
            pauseView.OnContinueButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Pause)).AddTo(pauseView);
            pauseView.OnReturnToHubButtonPressed.Subscribe(OnReturnToHubButtonPressed).AddTo(pauseView);
            pauseView.OnGoToSettingsButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourState.Option)).AddTo(pauseView);
            pauseView.OnEndGameButtonPressed.Subscribe(OnEndGameButtonPressed);

            ClearedLevelView clearedLevelView = pauseBehaviour.ClearedLevelView;
            clearedLevelView.SetClearedLevels(saveManager.Data.ClearedStageList);
            saveManager.OnSaved.Subscribe(SetClearedLevels).AddTo(clearedLevelView);
        }

        private async void OnEndGameButtonPressed(Unit _)
        {
            Debug.Log("End");
            await UniTask.Delay(TimeSpan.FromSeconds(endGameDelay));
            Debug.Log("pp");
            //applicationStopper.Quit();
        }

        private void OnActiveStateChanged((bool isActive, ViewBehaviourState behaviourType) context)
        {
            if (context.behaviourType != ViewBehaviourState.None)
            {
                return;
            }

            if (context.isActive)
            {
                cursorLocker.SetCursorLock(false);
                cursorLocker.IsCursorChangeBlock = true;

                playerTargetSyncer.Lock();
                gamepadVibrator.Pause();

                doInput.Value = false;
            }
            else
            {
                cursorLocker.IsCursorChangeBlock = false;
                cursorLocker.SetCursorLock(true);

                playerTargetSyncer.Unlock();
                gamepadVibrator.Resume();

                doInput.Value = true;
            }
        }

        private async void OnReturnToHubButtonPressed(Unit _)
        {
            // ポーズ画面のフェード中の入力をロック
            doInput.Value = false;

            navigator.DeactivateBehaviour(ViewBehaviourState.Pause);

            // フェード待機
            const float uiLockDuration = 1f;
            await UniTask.Delay(TimeSpan.FromSeconds(uiLockDuration));

            // ロック解除
            doInput.Value = true;

            // ハブにリスポーン
            gameState.SetState(GameState.State.StageSelect);
            // hubSpawner.Respawn().Forget();
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

        public void Dispose()
        {
            doInput?.Dispose();
        }
    }
}