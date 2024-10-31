using PropertyGenerator.Generated;
using R3;
using UnityEngine;

namespace Module.Character
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float damping;

        private PlayerControllerWrapper playerWrapper;
        private float currentSpeed;
        private float prevSpeed;

        private void Start()
        {
            playerWrapper = new PlayerControllerWrapper(animator);

            // ジャンプのイベント登録
            playerController.IsJumping.Subscribe(isJumping => { playerWrapper.IsJumping = isJumping; }).AddTo(this);

            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating => { playerWrapper.IsRotating = isRotating; }).AddTo(this);

            playerController.OnMove.Subscribe(velocity => { currentSpeed = velocity.magnitude; }).AddTo(this);

            playerController.IsDeath.Subscribe(isDeath => { playerWrapper.IsDeath = isDeath; }).AddTo(this);
        }

        private void OnAnimatorMove()
        {
            float speed = Mathf.Lerp(prevSpeed, currentSpeed, damping * Time.deltaTime);

            if (playerController.IsJumping.CurrentValue || speed <= 0.01f)
            {
                speed = 0f;
            }
            
            playerWrapper.Speed = speed;
            prevSpeed = speed;
        }
    }
}