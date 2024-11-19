using System;
using Module.Character;
using Module.InputModule;
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
            PlayerController playerController,
            CameraController cameraController,
            PlayerTargetSyncer playerTargetSyncer,
            GravitySwitcher gravitySwitcher
        )
        {
            gameInput.Move
                .Subscribe(moveInput =>
                {
                    if (!playerController.IsDeath.CurrentValue)
                    {
                        playerController.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerController);

            gameInput.Jump
                .Subscribe(isStarted =>
                {
                    if (!playerController.IsDeath.CurrentValue)
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
                    if (!playerController.IsDeath.CurrentValue)
                    {
                        cameraController.OnRotateCameraInput(lookInput);
                    }
                })
                .AddTo(cameraController);

            gameInput.Move
                .Subscribe(moveInput =>
                {
                    if (!playerController.IsDeath.CurrentValue)
                    {
                        playerTargetSyncer.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerTargetSyncer);

            gameInput.Move
                .Subscribe(moveInput =>
                {
                    if (!playerController.IsDeath.CurrentValue)
                    {
                        gravitySwitcher.OnMoveInput(moveInput);
                    }
                })
                .AddTo(playerTargetSyncer);
        }

        public void Initialize() { }
    }
}