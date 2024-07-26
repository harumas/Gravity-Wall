using Module.Gimmick;
using Module.InputModule;
using R3;
using UnityEngine;

namespace Module.Character
{
    /// <summary>
    /// プレイヤーの移動と回転を制御するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;
        [Header("速度減衰")] [SerializeField] private float speedDamping;
        [Header("ジャンプ中移動係数")] [SerializeField] private float airControl;
        [Header("回転速度")] [SerializeField] private float rotateSpeed;
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

        private bool isJumping;
        private Vector2 input;
        private float lastJumpTime;

        private void Awake()
        {
            //入力イベントの生成
            GameInput.Move.Subscribe(value => input = value).AddTo(this);
            GameInput.Jump.Subscribe(_ => OnJump()).AddTo(this);
        }

        private void FixedUpdate()
        {
            PerformMove();
            ClampVelocity();
            AdjustGravity();
            PerformGravityRotate();
        }

        private void OnJump()
        {
            if (isJumping)
            {
                return;
            }

            rigBody.AddForce(-Gravity.Value * jumpPower, ForceMode.VelocityChange);
            isJumping = true;
            lastJumpTime = Time.time;
        }

        private void ClampVelocity()
        {
            Vector3 gravity = Gravity.Value;
            Vector3 velocity = rigBody.velocity;

            float velocityAlongGravity = Vector3.Dot(velocity, gravity);

            //重力のベクトルに対してのローカル速度を取得
            Vector3 yVelocity = gravity * velocityAlongGravity;
            Vector3 xVelocity = velocity - gravity * velocityAlongGravity;


            if (input == Vector2.zero)
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

            rigBody.velocity = originalVelocity;
        }

        private void PerformMove()
        {
            //移動方向の算出
            Vector3 forward = target.forward * input.y;
            Vector3 right = target.right * input.x;
            Vector3 moveDirection = (forward + right).normalized;

            //重力と垂直な速度ベクトルに変換
            Quaternion targetRotation = Quaternion.FromToRotation(target.up, -Gravity.Value);
            Vector3 moveVelocity = targetRotation * moveDirection * controlSpeed;

            float airMultiplier = isJumping ? airControl : 1f;

            rigBody.AddForce(moveVelocity * airMultiplier, ForceMode.Acceleration);
        }

        private void PerformGravityRotate()
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -Gravity.Value) * rigBody.rotation;
            rigBody.rotation = Quaternion.Slerp(rigBody.rotation, targetRotation, rotateSpeed);
            float angle = Vector3.Angle(transform.up, -Gravity.Value);

            if (angle >= rotatingAngle)
            {
                //回転中はオブジェクトが落下しないようにする
                Gravity.SetDisable(Gravity.Type.Object);
            }
            else
            {
                rigBody.rotation = targetRotation;
                Gravity.SetEnable(Gravity.Type.Object);
            }
        }

        private void AdjustGravity()
        {
            if (!isJumping)
            {
                return;
            }

            localGravity.SetMultiplierAtFrame(jumpingGravity);
        }

        private void OnCollisionEnter(Collision _)
        {
            isJumping = false;
        }

        private void OnCollisionStay(Collision _)
        {
            if (!isJumping)
            {
                return;
            }

            bool canJump = lastJumpTime + allowJumpInterval <= Time.time;
            if (canJump)
            {
                isJumping = false;
            }
        }
    }
}