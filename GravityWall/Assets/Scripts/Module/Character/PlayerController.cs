using Core.Input;
using Module.Gimmick;
using UGizmo;
using UnityEngine;

namespace Module.Character
{
    public class PlayerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;
        [Header("ジャンプ中移動係数")] [SerializeField] private float airControl;
        [Header("回転速度")] [SerializeField] private float rotateSpeed;
        [Header("最大速度")] [SerializeField] private float maxSpeed;
        [Header("ジャンプ力")] [SerializeField] private float jumpPower;
        [Header("ジャンプ中の重力")] [SerializeField] private float jumpingGravity;

        [Header("連続ジャンプを許可する間隔")]
        [SerializeField]
        private float allowJumpInterval;

        [Header("Debug View")]
        [SerializeField]
        private bool isJumping;

        [SerializeField] private Rigidbody rigBody;
        [SerializeField] private Transform target;

        private InputEvent controlEvent;
        private InputEvent jumpEvent;
        private LocalGravity localGravity;

        private Vector2 input;
        private float jumpVelocity;
        private float lastJumpTime;

        private Vector3 prevPos;
        private Vector3 nextPos;

        private void Awake()
        {
            Application.targetFrameRate = -1;

            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Move);
            jumpEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Jump);
            localGravity = GetComponent<LocalGravity>();

            jumpEvent.Started += _ =>
            {
                if (!isJumping)
                {
                    rigBody.AddForce(-Gravity.Value * jumpPower, ForceMode.VelocityChange);
                    isJumping = true;
                    lastJumpTime = Time.time;
                }
            };

            prevPos = rigBody.position;
            nextPos = prevPos;
        }

        private void Update()
        {
            input = controlEvent.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (input != Vector2.zero)
            {
                Quaternion rot = Quaternion.FromToRotation(target.up, -Gravity.Value);

                var forward = target.forward * input.y;
                var right = target.right * input.x;
                var controlDir = (forward + right).normalized;
                var vel = rot * controlDir * controlSpeed;

                var airMultiplier = isJumping ? airControl : 1f;

                rigBody.AddForce(vel * airMultiplier, ForceMode.Acceleration);
            }

            ClampVelocity();
            PerformGravityRotate();

            UGizmos.DrawLine(prevPos, nextPos, Color.red, 5f);
            prevPos = nextPos;
            nextPos = rigBody.position;
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

        private void ClampVelocity()
        {
            Vector3 gravity = Gravity.Value;
            Vector3 velocity = rigBody.velocity;

            float velocityAlongGravity = Vector3.Dot(velocity, gravity);

            Vector3 yVelocity = gravity * velocityAlongGravity;
            Vector3 xVelocity = velocity - gravity * velocityAlongGravity;
            xVelocity = Vector3.ClampMagnitude(xVelocity, maxSpeed);

            // 元の座標系に戻すために、平行成分と垂直成分を合成
            Vector3 originalVelocity = xVelocity + yVelocity;

            AdjustGravity();

            rigBody.velocity = originalVelocity;
        }

        private void PerformGravityRotate()
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -Gravity.Value) * rigBody.rotation;
            rigBody.rotation = Quaternion.Slerp(rigBody.rotation, targetRotation, rotateSpeed);
            float angle = Vector3.Angle(transform.up, -Gravity.Value);

            if (angle >= 1f)
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
    }
}