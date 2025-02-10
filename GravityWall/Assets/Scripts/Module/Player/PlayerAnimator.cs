using System;
using PropertyGenerator.Generated;
using R3;
using UnityEngine;

namespace Module.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerControllerWrapper animator;
        [SerializeField] private AnimationClip landingClip;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float damping;
        [SerializeField] private float landingSpeed;
        [SerializeField] private float landingTimeOffset;

        private float currentSpeed;
        private float prevSpeed;

        private void Awake()
        {
            playerController.Parameter.LandingTime = GetLandingTime();
        }

        private void Start()
        {
            // ジャンプのイベント登録
            playerController.ControlEvent.IsGrounding.Subscribe(isGrounding =>
            {
                    animator.IsJumping = !isGrounding;
            }).AddTo(this);

            //playerController.ControlEvent.IsJumping.Subscribe(isJumping => { animator.IsJumping = isJumping; }).AddTo(this);

            // 回転のイベント登録
            playerController.ControlEvent.IsRotating.Subscribe(isRotating => { animator.IsRotating = isRotating; })
                .AddTo(this);

            playerController.ControlEvent.MoveVelocity.Subscribe(velocity => { currentSpeed = velocity.xv.magnitude; })
                .AddTo(this);

            playerController.ControlEvent.DeathState.Subscribe(isDeath => { animator.IsDeath = isDeath != DeathType.None; })
                .AddTo(this);

            animator.LandingSpeed = landingSpeed;
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
            LerpMoveSpeed();
        }

        private void LerpMoveSpeed()
        {
            // 移動速度を滑らかに変化させる
            float speed = Mathf.Lerp(prevSpeed, currentSpeed, damping * Time.deltaTime);

            if (speed <= 0.01f)
            {
                speed = 0f;
            }

            animator.Speed = speed;
            prevSpeed = speed;
        }
    }
}