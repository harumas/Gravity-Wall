using PropertyGenerator.Generated;
using R3;
using UnityEngine;

namespace Module.Character
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerController playerController;

        private PlayerWrapper playerWrapper;

        private void Start()
        {
            playerWrapper = new PlayerWrapper(animator);

            // ジャンプのイベント登録
            playerController.IsJumping.Subscribe(isJumping =>
            {
                if (isJumping)
                {
                    playerWrapper.SetJumpTrigger();
                }
            }).AddTo(this);

            // 歩行のイベント登録
            playerController.IsWalking.Subscribe(isWalking => { playerWrapper.Walk = isWalking; }).AddTo(this);

            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating =>
            {
                if (isRotating)
                {
                    playerWrapper.SetRotatingTrigger();
                }
            }).AddTo(this);
        }
    }
}