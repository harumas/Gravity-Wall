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

            // ジャンプボードを膨らませる → 元に戻す
            DoBounce().OnComplete(DoReverse);
        }

        private Tween DoBounce()
        {
            float height = boardShaderWrapper.JumpOn;
            
            return DOTween.To(() => height, value => height = value, bounceHeight, bounceDuration)
                .SetEase(Ease.OutBounce)
                .OnUpdate(() => { boardShaderWrapper.JumpOn = height; });
        }

        private void DoReverse()
        {
            float height = boardShaderWrapper.JumpOn;
            
            DOTween.To(() => height, value => height = value, reverseHeight, reverseDuration)
                .SetEase(Ease.OutBounce)
                .OnUpdate(() => { boardShaderWrapper.JumpOn = height; });
        }
    }
}