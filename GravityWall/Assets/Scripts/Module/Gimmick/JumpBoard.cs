using System;
using Cysharp.Threading.Tasks;
using Domain;
using UnityEngine;

namespace Module.Gimmick
{
    public class JumpBoard : MonoBehaviour
    {
        [Header("ジャンプ力")] [SerializeField] private float jumpPower;
        [Header("ジャンプ中の重力")] [SerializeField] private float jumpingGravity;
        [Header("ジャンプまでの遅延")] [SerializeField] private float jumpDelay;

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
            
            pushable.AddForce(transform.up * jumpPower, ForceMode.VelocityChange, jumpingGravity);
        }
    }
}