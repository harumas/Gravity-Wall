using System;
using System.Linq;
using DG.Tweening;
using Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class PressurePlate : GimmickObject
    {
        [SerializeField, Tag] private string[] targetTags;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private OnCollisionEventBridge collisionEventBridge;
        [SerializeField] private UnityEvent onEvent;
        [SerializeField] private float pushDuration;
        [SerializeField] private Rigidbody buttonCollision;
        [SerializeField] private float collisionMoveOffset;
        [SerializeField] private float pushDelay;

        private static readonly int pushRatio = Shader.PropertyToID("_PushRatio");

        private void Start()
        {
            Reset();

            collisionEventBridge.Enter += collision =>
            {
                if (targetTags.Any(tag => collision.gameObject.CompareTag(tag)) && !isEnabled.Value)
                {
                    Enable();
                }
            };
        }

        public override void Enable(bool doEffect = true)
        {
            isEnabled.Value = true;
            onEvent.Invoke();

            // Emissionの色を変更
            meshRenderer.material.DOFloat(1.0f, pushRatio, pushDuration).SetDelay(pushDelay);
            DOTween.To(() => buttonCollision.position, v => buttonCollision.position = v,
                    buttonCollision.position + transform.up * collisionMoveOffset, pushDuration)
                .SetDelay(pushDelay);
        }

        public override void Disable(bool doEffect = true)
        {
            isEnabled.Value = false;
            meshRenderer.material.SetFloat(pushRatio, 0f);
        }

        public override void Reset()
        {
            Disable(false);
        }
    }
}