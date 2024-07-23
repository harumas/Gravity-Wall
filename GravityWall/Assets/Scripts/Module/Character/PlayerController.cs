using System;
using Core.Input;
using DG.Tweening.Core.Easing;
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
        [Header("最大ジャンプ速度")] [SerializeField] private float maxJumpSpeed;
        [Header("ジャンプ中の重力")] [SerializeField] private float jumpingGravity;
        [Header("最大落下速度")] [SerializeField] private float maxFallSpeed;

        [Header("ジャンプを許可する重力との角度の差")]
        [SerializeField]
        private float allowJumpAngle;

        [SerializeField] private Rigidbody rigBody;
        [SerializeField] private Transform target;

        private InputEvent controlEvent;
        private InputEvent jumpEvent;
        private LocalGravity localGravity;

        private Vector2 input;
        private float jumpVelocity;
        private bool isJumping;
        private bool canJump;

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
                if (!isJumping && canJump)
                {
                    rigBody.AddForce(-Gravity.Value * jumpPower, ForceMode.VelocityChange);
                    isJumping = true;
                }
            };

            prevPos = rigBody.position;
            nextPos = prevPos;
        }

        private void Update()
        {
            input = controlEvent.ReadValue<Vector2>();

            Matrix4x4 transformMatrix = GetTransformationMatrix(-Gravity.Value);
            UGizmos.DrawArrow(transform.position, transform.position + transformMatrix.MultiplyPoint3x4(Vector3.up), Color.green);
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

        private void OnCollisionEnter(Collision other)
        {
            Vector3 localVelocity = transform.InverseTransformVector(rigBody.velocity);
            localVelocity.y = 0f;
            rigBody.velocity = transform.TransformDirection(localVelocity);
            isJumping = false;
        }

        private void ClampVelocity()
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -Gravity.Value) * rigBody.rotation;
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, targetRotation, Vector3.one);
            Vector3 localVelocity = matrix.MultiplyPoint3x4(rigBody.velocity);

            Debug.Log(localVelocity);
            UGizmos.DrawArrow(transform.position, transform.position + localVelocity * 0.1f, Color.red);

            //重力速度は保持する
            float y = localVelocity.y;
            localVelocity = Vector3.ClampMagnitude(localVelocity, maxSpeed);
            localVelocity.y = y;

            AdjustGravity();

            localVelocity = matrix.inverse.MultiplyPoint3x4(localVelocity);
            
            UGizmos.DrawArrow(transform.position, transform.position + localVelocity * 0.1f, Color.blue);
            rigBody.velocity = localVelocity;
        }

        private void PerformGravityRotate()
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -Gravity.Value) * rigBody.rotation;
            rigBody.rotation = Quaternion.Slerp(rigBody.rotation, targetRotation, rotateSpeed);
            float angle = Vector3.Angle(transform.up, -Gravity.Value);

            canJump = angle < allowJumpAngle;

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

        Matrix4x4 GetTransformationMatrix(Vector3 normal)
        {
            // 法線ベクトルを正規化
            Vector3 normalizedNormal = normal.normalized;

            // 新しい座標系のx軸を計算
            Vector3 up = Vector3.up;
            Vector3 newXAxis = Vector3.Cross(up, normalizedNormal).normalized;

            // 新しい座標系のz軸を計算
            Vector3 newZAxis = Vector3.Cross(normalizedNormal, newXAxis);

            // 座標変換行列を作成
            Matrix4x4 transformMatrix = new Matrix4x4();
            transformMatrix.SetColumn(0, new Vector4(newXAxis.x, newXAxis.y, newXAxis.z, 0));
            transformMatrix.SetColumn(1, new Vector4(normalizedNormal.x, normalizedNormal.y, normalizedNormal.z, 0));
            transformMatrix.SetColumn(2, new Vector4(newZAxis.x, newZAxis.y, newZAxis.z, 0));
            transformMatrix.SetColumn(3, new Vector4(0, 0, 0, 1));

            return transformMatrix;
        }
    }
}