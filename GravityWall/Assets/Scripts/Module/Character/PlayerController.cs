using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Domain;
using Module.Gravity;
using R3;
using UnityEngine;

namespace Module.Character
{
    /// <summary>
    /// プレイヤーの移動と回転を制御するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour, ICharacter
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;
        [Header("速度減衰")] [SerializeField] private float speedDamping;
        [Header("ジャンプ中移動係数")] [SerializeField] private float airControl;
        [Header("回転のイージング")] [SerializeField] private Ease easeType;

        [Header("回転のイージング係数")]
        [SerializeField]
        private float rotateStep;

        [Header("最大速度")] [SerializeField] private float maxSpeed;
        [Header("ジャンプ力")] [SerializeField] private float jumpPower;
        [Header("ジャンプ中の重力")] [SerializeField] private float jumpingGravity;

        [Header("連続ジャンプを許可する間隔")]
        [SerializeField]
        private float allowJumpInterval;

        [Header("回転中とみなす角度")] [SerializeField] private float rotatingAngle;

        [SerializeField] private Rigidbody rigBody;
        [SerializeField] private Transform target;
        [SerializeField] private LocalGravity localGravity;

        public ReadOnlyReactiveProperty<bool> IsJumping => isJumping;
        private readonly ReactiveProperty<bool> isJumping = new ReactiveProperty<bool>();

        public ReadOnlyReactiveProperty<bool> IsRotating => isRotating;
        private readonly ReactiveProperty<bool> isRotating = new ReactiveProperty<bool>();

        public ReadOnlyReactiveProperty<float> MoveSpeed => moveSpeed;
        private readonly ReactiveProperty<float> moveSpeed = new ReactiveProperty<float>();

        private Vector2 moveInput;
        private Vector3 inertia;
        private float lastJumpTime;
        private float variableJumpingGravity;

        private void Start()
        {
            isRotating.Subscribe(isRotating =>
            {
                if (isRotating)
                {
                    //回転中はオブジェクトが落下しないようにする
                    WorldGravity.Instance.SetDisable(WorldGravity.Type.Object);
                }
                else
                {
                    WorldGravity.Instance.SetEnable(WorldGravity.Type.Object);
                }
            });
        }

        public void OnMoveInput(Vector2 moveInput)
        {
            this.moveInput = moveInput;
        }

        public void OnJumpInput()
        {
            DoJump(-WorldGravity.Instance.Gravity * jumpPower, jumpingGravity);
        }

        private void FixedUpdate()
        {
            PerformMove();
            AdjustVelocity();
            AdjustGravity();
            PerformGravityRotate();
            PerformInertia();
        }

        private void AdjustVelocity()
        {
            Vector3 gravity = WorldGravity.Instance.Gravity;
            Vector3 velocity = rigBody.velocity;

            float velocityAlongGravity = Vector3.Dot(velocity, gravity);

            //重力のベクトルに対してのローカル速度を取得
            Vector3 yVelocity = gravity * velocityAlongGravity;
            Vector3 xVelocity = velocity - gravity * velocityAlongGravity;

            if (moveInput == Vector2.zero)
            {
                //入力がない時は減衰させる
                xVelocity *= speedDamping;
            }
            else
            {
                //x軸の動きだけクランプ
                xVelocity = Vector3.ClampMagnitude(xVelocity, maxSpeed);
            }

            //元の座標系に戻す
            Vector3 originalVelocity = xVelocity + yVelocity;
            moveSpeed.Value = xVelocity.sqrMagnitude / (maxSpeed * maxSpeed);

            rigBody.velocity = originalVelocity;
        }

        private void PerformMove()
        {
            //移動方向の算出
            Vector3 forward = target.forward * moveInput.y;
            Vector3 right = target.right * moveInput.x;
            Vector3 moveDirection = Vector3.ClampMagnitude(forward + right, 1f);

            //重力と垂直な速度ベクトルに変換
            Quaternion targetRotation = Quaternion.FromToRotation(target.up, -WorldGravity.Instance.Gravity);
            Vector3 moveVelocity = targetRotation * moveDirection * controlSpeed;

            float airMultiplier = isJumping.Value ? airControl : 1f;
            rigBody.AddForce(moveVelocity * airMultiplier, ForceMode.Acceleration);
        }

        private void PerformGravityRotate()
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -WorldGravity.Instance.Gravity) * rigBody.rotation;

            //角度の差を求める
            float angle = Vector3.Angle(transform.up, -WorldGravity.Instance.Gravity);
            angle = Mathf.Max(angle, Mathf.Epsilon);

            //イージング関数を噛ませる
            float t = Evaluate(easeType, angle, rotateStep);
            rigBody.rotation = Quaternion.Slerp(rigBody.rotation, targetRotation, t);

            isRotating.Value = angle - rotateStep >= rotatingAngle;

            if (!isRotating.Value)
            {
                rigBody.rotation = targetRotation;
            }
        }

        private float Evaluate(Ease easeType, float angle, float step)
        {
            if (easeType == Ease.Unset)
            {
                return step;
            }

            return EaseManager.Evaluate(easeType, null, step, angle, 1f, 1f);
        }

        private void PerformInertia()
        {
            if (moveInput == Vector2.zero)
            {
                rigBody.MovePosition(rigBody.position + inertia);
            }
        }

        private void AdjustGravity()
        {
            if (!isJumping.Value)
            {
                return;
            }

            localGravity.SetMultiplierAtFrame(variableJumpingGravity);
        }

        private void OnCollisionEnter(Collision _)
        {
            isJumping.Value = false;
            inertia = Vector3.zero;
        }

        private void OnCollisionStay(Collision _)
        {
            if (!isJumping.Value)
            {
                return;
            }

            bool canJump = lastJumpTime + allowJumpInterval <= Time.time;
            if (canJump)
            {
                isJumping.Value = false;
            }
        }

        public void DoJump(Vector3 jumpForce, float jumpingGravity)
        {
            if (isJumping.Value)
            {
                return;
            }

            variableJumpingGravity = jumpingGravity;
            rigBody.AddForce(jumpForce, ForceMode.VelocityChange);
            isJumping.Value = true;
            lastJumpTime = Time.time;
        }

        public void AddExternalPosition(Vector3 delta)
        {
            rigBody.MovePosition(rigBody.position + delta);
        }

        public void AddInertia(Vector3 inertia)
        {
            this.inertia += inertia;
        }
    }
}