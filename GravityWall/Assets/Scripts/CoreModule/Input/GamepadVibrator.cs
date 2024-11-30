using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.UI;

namespace CoreModule.Input
{
    public class GamepadVibrator
    {
        private bool enabled;
        private IDualMotorRumble motorRumble;
        private CancellationTokenSource cTokenSource;

        public GamepadVibrator()
        {
            InputSystem.onDeviceChange += (device, change) =>
            {
                bool useDevice = change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected || change == InputDeviceChange.Enabled;

                if (device is IDualMotorRumble dualMotorRumble)
                {
                    if (useDevice)
                    {
                        motorRumble = dualMotorRumble;
                        enabled = true;
                    }
                    else if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled)
                    {
                        motorRumble = Gamepad.current;
                    }
                }
            };
        }

        public void Enable()
        {
            if (enabled && Gamepad.current == null)
            {
                return;
            }

            motorRumble = Gamepad.current;
            enabled = true;
        }

        public void Disable()
        {
            if (!enabled)
            {
                return;
            }

            Reset();
            motorRumble = null;
            enabled = false;
        }

        public void Vibrate(float right, float left, float duration)
        {
            if (!enabled)
            {
                return;
            }

            VibrateTask(right, left, duration).Forget();
        }

        private async UniTaskVoid VibrateTask(float right, float left, float duration)
        {
            SetMotorSpeed(right, left);
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cTokenSource.Token);
            Reset();
        }

        public void SetMotorSpeed(float right, float left)
        {
            if (!enabled)
            {
                return;
            }

            Reset();
            cTokenSource = new CancellationTokenSource();
            motorRumble.SetMotorSpeeds(left, right);
        }

        public void Reset()
        {
            if (!enabled)
            {
                return;
            }

            cTokenSource?.Cancel();
            cTokenSource?.Dispose();
            cTokenSource = null;
            motorRumble.ResetHaptics();
        }

        public void Pause()
        {
            if (!enabled)
            {
                return;
            }

            motorRumble.PauseHaptics();
        }

        public void Resume()
        {
            if (!enabled)
            {
                return;
            }

            motorRumble.ResumeHaptics();
        }
    }
}