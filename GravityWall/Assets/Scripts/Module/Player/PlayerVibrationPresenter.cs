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
    /// <summary>
    /// プレイヤーイベントとバイブレーションを接続するクラス
    /// </summary>
    public class PlayerVibrationPresenter : IInitializable
    {
        private float lastVibrationTime;
        private float lastRotationAngle;

        [Inject]
        public PlayerVibrationPresenter(PlayerController playerController, VibrationParameter parameter, GamepadVibrator gamepadVibrator)
        {
            // 死亡イベント
            playerController.IsDeath
                .Subscribe(value =>
                {
                    if (value != DeathType.None)
                    {
                        lastVibrationTime = Time.time;
                        gamepadVibrator.Vibrate(parameter.DeathDuration, parameter.DeathSpeed, parameter.DeathSpeed);
                    }
                })
                .AddTo(playerController);

            // 回転角度更新
            playerController.RotationAngle
                .Subscribe(value => { lastRotationAngle = value; })
                .AddTo(playerController);

            // 開店イベント
            playerController.IsRotating
                .Subscribe(value =>
                {
                    // 回転中で一定時間経過している場合バイブレーション
                    if (value && Time.time - lastVibrationTime > parameter.RotateVibrationInterval)
                    {
                        lastVibrationTime = Time.time;
                        
                        // 0 ~ 1を度数に変換してバイブレーションの強さを決める
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