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
            PlayerState playerState = playerController.State;

            // ジャンプのイベント登録
            playerState.IsJumping.Subscribe(isJumping => { playerWrapper.IsJumping = isJumping; }).AddTo(this);

            // 回転のイベント登録
            playerState.IsRotating.Subscribe(isRotating => { playerWrapper.IsRotating = isRotating; }).AddTo(this);

            playerState.OnMove.Subscribe(velocity => { currentSpeed = velocity.magnitude; }).AddTo(this);

            playerState.IsDeath.Subscribe(isDeath => { playerWrapper.IsDeath = isDeath; }).AddTo(this);
        }

        private void OnAnimatorMove()
        {
            float speed = Mathf.Lerp(prevSpeed, currentSpeed, damping * Time.deltaTime);

            if (speed <= 0.01f)
            {
                speed = 0f;
            }
            
            playerWrapper.Speed = speed;
            prevSpeed = speed;
        }
    }
}