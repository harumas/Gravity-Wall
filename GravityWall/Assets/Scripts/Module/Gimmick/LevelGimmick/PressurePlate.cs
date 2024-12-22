using System;
using System.Linq;
using System.Threading;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick.LevelGimmick
{
    public class PressurePlate : GimmickObject
    {
        private enum State
        {
            Locked,
            NoTouch,
            Pushing,
            Changing,
            Pushed,
        }

        [SerializeField, Tag] private string[] targetTags;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private OnTriggerEventBridge triggerEventBridge;
        [SerializeField] private UnityEvent onEvent;
        [SerializeField] private bool unlockOnStart;
        [SerializeField, Header("当たり判定のRigidBody")] private Rigidbody buttonRig;
        [SerializeField, Header("押されたときに当たり判定を動かすオフセット")] private float collisionMoveOffset;
        [SerializeField, Header("押す時間")] private float pushDuration;
        [SerializeField, Header("押し始めるまでの遅延")] private float pushDelay;

        private static readonly int pushRatio = Shader.PropertyToID("_PushRatio");
        private CancellationTokenSource pushCanceller;
        private State state = State.Locked;

        private void Start()
        {
            Reset();

            state = unlockOnStart ? State.NoTouch : State.Locked;

            triggerEventBridge.Enter.AddListener(collider =>
            {
                //既に押されていたら終了
                if (state == State.Pushed || state == State.Locked)
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
            });

            triggerEventBridge.Exit.AddListener(collider =>
            {
                //既に押されていたら終了
                if (state == State.Pushed || state == State.Locked)
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
            });
        }

        public override void Enable(bool doEffect = true)
        {
            EnableAsync(doEffect).Forget();
        }
        
        public void Unlock()
        {
            state = State.NoTouch;
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
            onEvent.Invoke();
        }

        public override void Disable(bool doEffect = true)
        {
            CancelPush();
            isEnabled.Value = false;
            meshRenderer.material.SetFloat(pushRatio, 0f);
            state = State.NoTouch;
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
            meshRenderer.material.DOFloat(1.0f, pushRatio, pushDuration);
            buttonRig.DoMove(position, pushDuration);

            await UniTask.Delay(TimeSpan.FromSeconds(pushDuration), cancellationToken: pushCanceller.Token);
        }

        private void CancelPush()
        {
            pushCanceller?.Cancel();
            pushCanceller?.Dispose();
            pushCanceller = null;
        }
    }
}