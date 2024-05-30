using System;
using Core.Input;
using Module.Gimmick;
using UnityEngine;

namespace Module.Character
{
    public class PlayerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;
        [Header("回転速度")] [SerializeField] private float rotateSpeed;
        [Header("最大速度")] [SerializeField] private float maxSpeed;
        [Header("ジャンプ力")] [SerializeField] private float jumpPower;

        [SerializeField] private Rigidbody rigBody;
        [SerializeField] private Transform target;
        [SerializeField] private float interpolateValue;

        private InputEvent controlEvent;
        private InputEvent jumpEvent;

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

            jumpEvent.Started += _ =>
            {
                if (!isJumping)
                {
                    rigBody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                    isJumping = true;
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

                rigBody.AddForce(vel, ForceMode.Acceleration);
            }

            ClampVelocity();
            PerformRotate();

            prevPos = nextPos;
            nextPos = rigBody.position;
        }

        private void OnCollisionEnter(Collision other)
        {
            Vector3 localVelocity = transform.InverseTransformVector(rigBody.velocity);
            localVelocity.y = 0f;
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

            rigBody.velocity = transform.TransformDirection(localVelocity);
        }

        private void PerformRotate()
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
    }
}