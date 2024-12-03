using System;
using System.Linq;
using System.Threading;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PropertyGenerator.Generated;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick.LevelGimmick
{
    public class PressurePlate : GimmickObject
    {
        private enum State
        {
            NoTouch,
            Pushing,
            Changing,
            Pushed,
        }

        [SerializeField, Tag] private string[] targetTags;
        [SerializeField] private PressSwitchWrapper shaderWrapper;
        [SerializeField] private OnTriggerEventBridge triggerEventBridge;
        [SerializeField] private UnityEvent onPressed;

        [SerializeField, Header("当たり判定のRigidBody")] private Rigidbody buttonRig;
        [SerializeField, Header("押されたときに当たり判定を動かすオフセット")] private float collisionMoveOffset;
        [SerializeField, Header("押す時間")] private float pushDuration;
        [SerializeField, Header("押し始めるまでの遅延")] private float pushDelay;

        private CancellationTokenSource pushCanceller;
        private State state;

        private void Start()
        {
            Reset();

            triggerEventBridge.Enter += collider =>
            {
                //既に押されていたら終了
                if (state == State.Pushed)
                {
                    return;
                }

                // 押されていない状態でターゲットのタグが触れたら押し始める
                if (!isEnabled.Value && state == State.NoTouch && targetTags.Any(tag => collider.CompareTag(tag)))
                {
                    state = State.Pushing;
                    pushCanceller = new CancellationTokenSource();
                    Enable();
                }
            };

            triggerEventBridge.Exit += collider =>
            {
                //既に押されていたら終了
                if (state == State.Pushed)
                {
                    return;
                }

                // 押されている状態でターゲットのタグが離れたら押し終わる
                if (!isEnabled.Value && state == State.Pushing && targetTags.Any(tag => collider.CompareTag(tag)))
                {
                    state = State.NoTouch;
                    CancelPush();
                }
            };
        }

        public override void Enable(bool doEffect = true)
        {
            EnableAsync(doEffect).Forget();
        }

        private async UniTaskVoid EnableAsync(bool doEffect)
        {
            // 演出の実行
            if (doEffect)
            {
                await PushButtonEffect();
            }

            state = State.Pushed;

            isEnabled.Value = true;
            onPressed.Invoke();
        }

        public override void Disable(bool doEffect = true)
        {
            CancelPush();
            isEnabled.Value = false;
            shaderWrapper.PushRatio = 0f;
        }

        public override void Reset()
        {
            Disable(false);
        }

        private async UniTask PushButtonEffect()
        {
            // 押し始めは少し遅延する
            await UniTask.Delay(TimeSpan.FromSeconds(pushDelay), cancellationToken: pushCanceller.Token);

            state = State.Changing;

            Vector3 position = buttonRig.position + transform.up * collisionMoveOffset;

            // 演出の遷移
            DOTween.To(() => shaderWrapper.PushRatio, v => shaderWrapper.PushRatio = v, 1.0f, pushDuration);
            buttonRig.DoMove(position, pushDuration);

            await UniTask.Delay(TimeSpan.FromSeconds(pushDuration));
        }

        private void CancelPush()
        {
            pushCanceller?.Cancel();
            pushCanceller?.Dispose();
            pushCanceller = null;
        }
    }
}