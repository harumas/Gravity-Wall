using System;
using Core.Input;
using Module.Gimmick;
using UnityEngine;

namespace Module.Player
{
    public class PlayerMovement : MonoBehaviour
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
                rigBody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
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

            Vector3 localVelocity = transform.InverseTransformVector(rigBody.velocity);
            float y = localVelocity.y;
            localVelocity = Vector3.ClampMagnitude(localVelocity, maxSpeed);
            localVelocity.y = y;

            rigBody.velocity = transform.TransformDirection(localVelocity);

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -Gravity.Value) * rigBody.rotation;
            rigBody.rotation = Quaternion.Slerp(rigBody.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            prevPos = nextPos;
            nextPos = rigBody.position;
        }
    }
}