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
        private float lastRotationAngle;

        [Inject]
        public PlayerVibrationPresenter(PlayerController playerController, VibrationParameter parameter, GamepadVibrator gamepadVibrator)
        {
            playerController.IsDeath
                .Subscribe(value =>
                {
                    if (value)
                    {
                        lastVibrationTime = Time.time;
                        gamepadVibrator.Vibrate(parameter.DeathDuration, parameter.DeathSpeed, parameter.DeathSpeed);
                    }
                })
                .AddTo(playerController);

            playerController.RotationAngle
                .Subscribe(value => { lastRotationAngle = value; })
                .AddTo(playerController);

            playerController.IsRotating
                .Subscribe(value =>
                {
                    if (value && Time.time - lastVibrationTime > parameter.RotateVibrationInterval)
                    {
                        lastVibrationTime = Time.time;
                        float speed = parameter.EvaluateAngleVibration(Mathf.Abs(lastRotationAngle) / 180f);
                        gamepadVibrator.Vibrate(parameter.RotateDuration, speed, speed);
                    }
                })
                .AddTo(playerController);
        }

        public void Initialize()
        {
        }
    }
}