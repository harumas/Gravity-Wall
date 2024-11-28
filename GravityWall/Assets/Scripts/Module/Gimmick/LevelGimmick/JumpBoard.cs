using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain;
using UnityEngine;

namespace Module.Gimmick
{
    public class JumpBoard : MonoBehaviour
    {
        [Header("ジャンプ力")] [SerializeField] private float jumpPower;
        [Header("ジャンプ中の重力")] [SerializeField] private float jumpingGravity;
        [Header("ジャンプまでの遅延")] [SerializeField] private float jumpDelay;
        [SerializeField] private MeshRenderer meshRenderer;
        
        private static readonly int jumpOnProperty = Shader.PropertyToID("_JumpOn");

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out IPushable pushable))
            {
                Push(pushable).Forget();
            }
        }

        private async UniTaskVoid Push(IPushable pushable)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(jumpDelay));

            float jumpOn = -1;
            DOTween.To(() => jumpOn, (value) => jumpOn = value, 1.0f, 0.5f)
                .SetEase(Ease.OutBounce)
                .OnUpdate(() =>
                {
                    meshRenderer.material.SetFloat(jumpOnProperty, jumpOn);
                })
                .OnComplete(() =>
                {
                    DOTween.To(() => jumpOn, (value) => jumpOn = value, -0.65f, 0.3f)
                        .SetEase(Ease.OutBounce)
                        .OnUpdate(() =>
                        {
                            meshRenderer.material.SetFloat(jumpOnProperty, jumpOn);
                        });
                });

            pushable.AddForce(transform.up * jumpPower, ForceMode.VelocityChange, jumpingGravity, false);
        }
    }
}