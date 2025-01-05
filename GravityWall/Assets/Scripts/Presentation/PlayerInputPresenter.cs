using Module.InputModule;
using Module.Player;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    /// <summary>
    /// プレイヤー入力を接続するクラス
    /// </summary>
    public class PlayerInputPresenter : IInitializable
    {
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
            var lockProperty = playerController.IsDeath.Select(value => value != PlayerController.DeathType.None).ToReadOnlyReactiveProperty();
            inputLocker.AddCondition(lockProperty, playerController.destroyCancellationToken);

            gameInput.Move
                .Subscribe(moveInput =>
                {
                    if (!inputLocker.IsLocked)
                    {
                        playerController.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerController);

            gameInput.Jump
                .Subscribe(isStarted =>
                {
                    if (!inputLocker.IsLocked)
                    {
                        if (isStarted)
                        {
                            playerController.OnJumpStart();
                        }
                        else
                        {
                            playerController.OnJumpEnd();
                        }
                    }
                })
                .AddTo(playerController);

            gameInput.LookDelta
                .Subscribe(lookInput =>
                {
                    if (!inputLocker.IsLocked)
                    {
                        cameraController.OnRotateCameraInput(lookInput);
                    }
                })
                .AddTo(cameraController);

            gameInput.Move
                .Subscribe(moveInput =>
                {
                    if (!inputLocker.IsLocked)
                    {
                        playerTargetSyncer.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerTargetSyncer);

            gameInput.Move
                .Subscribe(moveInput =>
                {
                    if (!inputLocker.IsLocked)
                    {
                        gravitySwitcher.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerTargetSyncer);
        }

        public void Initialize()
        {
        }
    }
}