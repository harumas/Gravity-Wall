using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using R3;
using System.Threading;

namespace Module.Character
{
    public class GamepadVibrator : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        CancellationTokenSource cts;

        private void Start()
        {
            //ジャンプ中はチェッカーを無効化する
            playerController.IsJumping
            .Subscribe(value =>
            {
                if (!value)
                {
                    Vibrate(10,0.1f,0.1f);
                }
            })
            .AddTo(this);

            playerController.IsDeath
            .Subscribe(value =>
            {
                if (value)
                {
                    Vibrate(50, 0.7f, 0.7f);
                }
            })
            .AddTo(this);

            playerController.IsRotating
            .Subscribe(value =>
            {
                if (value)
                {
                    Vibrate(20, 0.6f, 0.6f);
                }
            })
            .AddTo(this);

        }

        public void Vibrate(int frame,float scaleLeft,float scaleRight)
        {
            cts?.Cancel();
            VibrateTask(frame,scaleLeft,scaleRight).Forget();
        }

        async UniTaskVoid VibrateTask(int frame,float scaleLeft,float scaleRight)
        {
            cts = new CancellationTokenSource();
            InputSystem.ResetHaptics();
            Gamepad.current.SetMotorSpeeds(scaleLeft, scaleRight);
            await UniTask.DelayFrame(frame);
            InputSystem.ResetHaptics();
            cts.Cancel();
        }
    }
}