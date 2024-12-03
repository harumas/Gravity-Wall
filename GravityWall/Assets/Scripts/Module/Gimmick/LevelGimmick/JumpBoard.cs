using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Player;
using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class JumpBoard : MonoBehaviour
    {
        [SerializeField, Header("ジャンプ力")] private float jumpPower;
        [SerializeField, Header("ジャンプ中の重力")] private float jumpingGravity;
        [SerializeField, Header("ジャンプまでの遅延")] private float jumpDelay;
        [SerializeField, Header("ジャンプ台が膨らむ高さ")] private float bounceHeight;
        [SerializeField, Header("ジャンプ台が膨らむ時間")] private float bounceDuration;
        [SerializeField, Header("ジャンプ台がもとに戻る高さ")] private float reverseHeight;
        [SerializeField, Header("ジャンプ台がもとに戻る時間")] private float reverseDuration;
        [SerializeField] private JumpBoardWrapper boardShaderWrapper;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out IPushable pushable))
            {
                Push(pushable).Forget();
            }
        }

        private async UniTaskVoid Push(IPushable pushable)
        {
            // ジャンプまでの溜め時間
            await UniTask.Delay(TimeSpan.FromSeconds(jumpDelay));

            // プレイヤーの上方向にジャンプ力を与える
            pushable.AddForce(transform.up * jumpPower, ForceMode.VelocityChange, jumpingGravity, false);

            float jumpOn = -1;

            // 膨らませる
            DOTween.To(() => jumpOn, (value) => jumpOn = value, bounceHeight, bounceDuration)
                .SetEase(Ease.OutBounce)
                .OnUpdate(() =>
                {
                    boardShaderWrapper.JumpOn = jumpOn;
                })
                // 膨らみきったらもとに戻る
                .OnComplete(() =>
                {
                    DOTween.To(() => jumpOn, (value) => jumpOn = value, reverseHeight, reverseDuration)
                        .SetEase(Ease.OutBounce)
                        .OnUpdate(() =>
                        {
                            boardShaderWrapper.JumpOn = jumpOn;
                        });
                });
        }
    }
}