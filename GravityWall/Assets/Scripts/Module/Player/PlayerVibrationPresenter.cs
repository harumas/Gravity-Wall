using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using R3;
using System.Threading;
using CoreModule.Input;
using Module.InputModule;
using VContainer;
using VContainer.Unity;

namespace Module.Player
{
    public class PlayerVibrationPresenter : IInitializable
    {
        private float lastVibrationTime;

        [Inject]
        public PlayerVibrationPresenter(PlayerController playerController, VibrationParameter parameter, GamepadVibrator gamepadVibrator)
        {
            playerController.IsJumping
                .Subscribe(value =>
                {
                    if (!value && Time.time - lastVibrationTime > parameter.VibrationInterval)
                    {
                        lastVibrationTime = Time.time;
                        gamepadVibrator.Vibrate(parameter.Jump.Duration, parameter.Jump.RightSpeed, parameter.Jump.LeftSpeed);
                    }
                })
                .AddTo(playerController);

            playerController.IsDeath
                .Subscribe(value =>
                {
                    if (value && Time.time - lastVibrationTime > parameter.VibrationInterval)
                    {
                        lastVibrationTime = Time.time;
                        gamepadVibrator.Vibrate(parameter.Death.Duration, parameter.Death.RightSpeed, parameter.Death.LeftSpeed);
                    }
                })
                .AddTo(playerController);

            playerController.IsRotating
                .Subscribe(value =>
                {
                    if (value && Time.time - lastVibrationTime > parameter.VibrationInterval)
                    {
                        lastVibrationTime = Time.time;
                        gamepadVibrator.Vibrate(parameter.Rotate.Duration, parameter.Rotate.RightSpeed, parameter.Rotate.LeftSpeed);
                    }
                })
                .AddTo(playerController);
        }

        public void Initialize()
        {
        }
    }
}