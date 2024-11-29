using CoreModule.Helper;
using PropertyGenerator.Generated;
using R3;
using UnityEngine;

namespace Module.Character
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip landingClip;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float damping;
        [SerializeField] private float landingSpeed;
        [SerializeField] private float landingTimeOffset;

        private PlayerControllerWrapper playerWrapper;
        private float currentSpeed;
        private float prevSpeed;

        private void Start()
        {
            playerWrapper = new PlayerControllerWrapper(animator);
            playerController.SetLandingTime(GetLandingTime());

            // ジャンプのイベント登録
            playerController.IsJumping.Subscribe(isJumping =>
            {
                if (isJumping)
                {
                    playerWrapper.IsJumping = true;
                }
            }).AddTo(this);
            
            playerController.IsGrounding.Subscribe(isGrounding =>
            {
                if (!isGrounding)
                {
                    playerWrapper.IsJumping = true;
                }
            }).AddTo(this);

            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating => { playerWrapper.IsRotating = isRotating; }).AddTo(this);

            playerController.OnMove.Subscribe(velocity => { currentSpeed = velocity.xv.magnitude; }).AddTo(this);

            playerController.IsDeath.Subscribe(isDeath => { playerWrapper.IsDeath = isDeath; }).AddTo(this);

            playerWrapper.LandingSpeed = landingSpeed;
        }

        /// <summary>
        /// 着地モーションの時間を取得する関数
        /// </summary>
        /// <returns></returns>
        private float GetLandingTime()
        {
            return (landingClip.length + landingTimeOffset) / landingSpeed;
        }

        private void OnAnimatorMove()
        {
            UpdateGrounding();
            LerpMoveSpeed();
        }

        private void UpdateGrounding()
        {
            // 地面についたら着地モーションの再生
            if (playerController.IsGrounding.CurrentValue && playerWrapper.IsJumping)
            {
                playerWrapper.IsJumping = false;
            }
        }

        private void LerpMoveSpeed()
        {
            // 移動速度を滑らかに変化させる
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