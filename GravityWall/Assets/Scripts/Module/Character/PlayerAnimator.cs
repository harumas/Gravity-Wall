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
                playerWrapper.IsJumping = isJumping;
            }).AddTo(this);

            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating =>
            {
                playerWrapper.IsRotating = isRotating;
            }).AddTo(this);

            playerController.MoveSpeed.Subscribe(velocity =>
            {
                playerWrapper.Speed = velocity;
            }).AddTo(this);
        }
    }
}