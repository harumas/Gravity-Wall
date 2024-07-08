using System;
using Core.Input;
using Module.Gimmick;
using UGizmo;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private AnimationCurve decreaseGravityMultiplierCurve;
        [SerializeField] private AnimationCurve increaseGravityMultiplierCurve;
        private float[] decreaseGravityMultipliers;
        private float[] increaseGravityMultipliers;

        [SerializeField] private Rigidbody rigBody;
        [SerializeField] private Transform target;

        private InputEvent controlEvent;
        private InputEvent jumpEvent;
        private LocalGravity localGravity;

        private Vector2 input;
        private float jumpVelocity;
        private bool isJumping;

        private Vector3 prevPos;
        private Vector3 nextPos;

        private void Awake()
        {
            Application.targetFrameRate = -1;

            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Move);
            jumpEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Jump);
            localGravity = GetComponent<LocalGravity>();

            // SampleDecreaseGravityVariation();
            // SampleIncreaseGravityVariation();

            jumpEvent.Started += _ =>
            {
                if (!isJumping)
                {
                    rigBody.AddForce(transform.up * jumpPower, ForceMode.VelocityChange);
                    //localGravity.AddExternalMultiplier(decreaseGravityMultipliers);
                    isJumping = true;
                }
            };

            prevPos = rigBody.position;
            nextPos = prevPos;
        }

        private void SampleDecreaseGravityVariation()
        {
            Keyframe[] keyframes = decreaseGravityMultiplierCurve.keys;
            int lastIndex = keyframes.Length - 1;
            int length = (int)keyframes[lastIndex].time;

            decreaseGravityMultipliers = new float[length];

            for (int i = 0; i < length; i++)
            {
                decreaseGravityMultipliers[i] = decreaseGravityMultiplierCurve.Evaluate(i);
            }
        }

        private void SampleIncreaseGravityVariation()
        {
            Keyframe[] keyframes = increaseGravityMultiplierCurve.keys;
            int lastIndex = keyframes.Length - 1;
            int length = (int)keyframes[lastIndex].time;

            increaseGravityMultipliers = new float[length];

            for (int i = 0; i < length; i++)
            {
                increaseGravityMultipliers[i] = increaseGravityMultiplierCurve.Evaluate(i);
            }
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

                var multiplier = isJumping ? airControl : 1f;

                rigBody.AddForce(vel * multiplier, ForceMode.Acceleration);
            }

            ClampVelocity();
            PerformGravityRotate();

            UGizmos.DrawLine(prevPos, nextPos, Color.red, 5f);
            prevPos = nextPos;
            nextPos = rigBody.position;
        }

        private void OnCollisionEnter(Collision other)
        {
            Vector3 localVelocity = transform.InverseTransformVector(rigBody.velocity);
            localVelocity.y = 0f;
            prevVelocity = 0f;
            rigBody.velocity = transform.TransformDirection(localVelocity);
            isJumping = false;
        }

        private void ClampVelocity()
        {
            Vector3 localVelocity = transform.InverseTransformVector(rigBody.velocity);

            //重力速度は保持する
            float y = localVelocity.y;
            localVelocity = Vector3.ClampMagnitude(localVelocity, maxSpeed);
            localVelocity.y = y;

            AdjustGravity(localVelocity);

            rigBody.velocity = transform.TransformDirection(localVelocity);
        }

        private void PerformGravityRotate()
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -Gravity.Value) * rigBody.rotation;
            rigBody.rotation = Quaternion.Slerp(rigBody.rotation, targetRotation, rotateSpeed);

            if (Vector3.Angle(transform.up, -Gravity.Value) < 0.1f)
            {
                rigBody.rotation = targetRotation;
                Gravity.SetDisable(Gravity.Type.Player);
            }
            else
            {
                //回転中はオブジェクトが落下しないようにする
                Gravity.SetDisable(Gravity.Type.Object);
            }
        }

        private float prevVelocity;

        private void AdjustGravity(Vector3 localVelocity)
        {
            if (!isJumping)
            {
                return;
            }

            localGravity.SetMultiplierAtFrame(jumpingGravity);
            prevVelocity = localVelocity.y;
        }
    }
}