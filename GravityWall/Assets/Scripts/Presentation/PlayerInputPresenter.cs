using System;
using Module.InputModule;
using Module.Player;
using Module.Player.HSM;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    /// <summary>
    /// プレイヤー入力を接続するクラス
    /// </summary>
    public class PlayerInputPresenter : IInitializable, IDisposable
    {
        private readonly InputEventAdapter adapter;

        [Inject]
        public PlayerInputPresenter(
            IGameInput gameInput,
            InputLocker inputLocker,
            PlayerController playerController,
            CameraController cameraController,
            PlayerTargetSyncer playerTargetSyncer,
            GravitySwitcher gravitySwitcher
        )
        {
            var lockProperty = playerController.ControlEvent.LockState.Select(value => !value).ToReadOnlyReactiveProperty();
            inputLocker.AddCondition(lockProperty, playerController.destroyCancellationToken);

            adapter = new InputEventAdapter(
                gameInput,
                gameInput.Move.Where(_ => !inputLocker.IsLocked.CurrentValue),
                gameInput.Jump.Where(_ => !inputLocker.IsLocked.CurrentValue),
                gameInput.LookDelta.Where(_ => !inputLocker.IsLocked.CurrentValue)
            );

            inputLocker.IsLocked
                .Subscribe(isLocked =>
                {
                    if (!isLocked)
                    {
                        adapter.Sync();
                    }
                })
                .AddTo(playerController);

            playerController.ControlEvent.DeathState
                .Subscribe(deathType =>
                {
                    if (deathType != DeathType.None)
                    {
                        adapter.Sync();
                    }
                })
                .AddTo(playerController);

            playerController.SetInputAdapter(adapter);

            adapter.LookDelta
                .Subscribe(lookInput =>
                {
                    if (!inputLocker.IsLocked.CurrentValue)
                    {
                        cameraController.OnRotateCameraInput(lookInput);
                    }
                })
                .AddTo(cameraController);

            adapter.Move
                .Subscribe(moveInput =>
                {
                    if (!inputLocker.IsLocked.CurrentValue)
                    {
                        playerTargetSyncer.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerTargetSyncer);

            adapter.Move
                .Subscribe(moveInput =>
                {
                    if (!inputLocker.IsLocked.CurrentValue)
                    {
                        gravitySwitcher.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerTargetSyncer);
        }

        public void Initialize()
        {
        }

        public void Dispose()
        {
            adapter.Dispose();
        }
    }
}