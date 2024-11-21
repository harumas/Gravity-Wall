using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
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
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private OnTriggerEventBridge triggerEventBridge;
        [SerializeField] private UnityEvent onEvent;
        [SerializeField] private float pushDuration;
        [SerializeField] private Rigidbody buttonCollision;
        [SerializeField] private float collisionMoveOffset;
        [SerializeField] private float pushDelay;

        private static readonly int pushRatio = Shader.PropertyToID("_PushRatio");
        private CancellationTokenSource pushCanceller;
        private State state;

        private void Start()
        {
            Reset();

            triggerEventBridge.Enter += collider =>
            {
                if (state == State.Pushed)
                {
                    return;
                }

                if (state == State.NoTouch && !isEnabled.Value && targetTags.Any(tag => collider.CompareTag(tag)))
                {
                    state = State.Pushing;

                    pushCanceller?.Dispose();
                    pushCanceller = new CancellationTokenSource();

                    Enable();
                }
            };

            triggerEventBridge.Exit += collider =>
            {
                if (state == State.Pushed)
                {
                    return;
                }

                if (state == State.Pushing && !isEnabled.Value && targetTags.Any(tag => collider.CompareTag(tag)))
                {
                    state = State.NoTouch;

                    pushCanceller?.Cancel();
                    pushCanceller?.Dispose();
                    pushCanceller = null;
                }
            };
        }

        public override void Enable(bool doEffect = true)
        {
            EnableAsync(doEffect).Forget();
        }

        private async UniTaskVoid EnableAsync(bool doEffect)
        {
            // Emissionの色を変更
            if (doEffect)
            {
                await PushButtonEffect();
            }

            state = State.Pushed;

            isEnabled.Value = true;
            onEvent.Invoke();
        }

        private async UniTask PushButtonEffect()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(pushDelay), cancellationToken: pushCanceller.Token);

            state = State.Changing;

            Vector3 position = buttonCollision.position + transform.up * collisionMoveOffset;

            meshRenderer.material.DOFloat(1.0f, pushRatio, pushDuration);
            DOTween.To(() => buttonCollision.position, v => buttonCollision.position = v, position, pushDuration);

            await UniTask.Delay(TimeSpan.FromSeconds(pushDuration));
        }

        public override void Disable(bool doEffect = true)
        {
            pushCanceller?.Cancel();
            pushCanceller?.Dispose();
            pushCanceller = null;

            isEnabled.Value = false;
            meshRenderer.material.SetFloat(pushRatio, 0f);
        }

        public override void Reset()
        {
            Disable(false);
        }
    }
}