using System;
using Constants;
using CoreModule.Helper;
using DG.Tweening;
using Module.Gravity;
using R3;
using UGizmo;
using UnityEngine;

namespace Module.Character
{
    public class PlayerFunction
    {
        private readonly WorldGravity worldGravity;
        private readonly Transform transform;
        private readonly Transform cameraPivot;
        private readonly Rigidbody rigidbody;
        private readonly LocalGravity localGravity;
        private readonly PlayerControlParameter parameter;
        private readonly CapsuleCollider capsuleCollider;

        private const int GroundLayerMask = Layer.Mask.Base | Layer.Mask.Gravity | Layer.Mask.IgnoreGravity | Layer.Mask.IgnoreGimmick;

        private float lastJumpTime;
        private float lastRotateTime;
        private bool isLastRotating;
        private float temporalGravity;

        public PlayerFunction(
            Transform transform,
            Transform cameraPivot,
            Rigidbody rigidbody,
            LocalGravity localGravity,
            PlayerControlParameter parameter
        )
        {
            this.transform = transform;
            this.cameraPivot = cameraPivot;
            this.rigidbody = rigidbody;
            this.localGravity = localGravity;
            this.parameter = parameter;

            capsuleCollider = transform.GetComponent<CapsuleCollider>();

            worldGravity = WorldGravity.Instance;
        }

        public void PerformMove(Vector2 moveInput, bool isJumping)
        {
            // 移動方向の算出
            Vector3 forward = cameraPivot.forward * moveInput.y;
            Vector3 right = cameraPivot.right * moveInput.x;
            Vector3 moveDirection = Vector3.ClampMagnitude(forward + right, 1f);

            // 重力と垂直な速度ベクトルに変換
            moveDirection = Quaternion.FromToRotation(cameraPivot.up, -worldGravity.Gravity) * moveDirection;

            // 空中の制御率を適用
            float airMultiplier = isJumping ? parameter.AirControl : 1f;

            // 力を算出して加速
            Vector3 moveForce = moveDirection * (parameter.Acceleration * airMultiplier);
            rigidbody.AddForce(moveForce, ForceMode.Acceleration);
        }

        public void AdjustVelocity(bool isMoveInput, bool freezeX)
        {
            //重力のベクトルに対してのローカル速度を取得
            (Vector3 xv, Vector3 yv) = GetSeperatedVelocity();

            if (freezeX)
            {
                xv = Vector3.zero;
            }
            else
            {
                if (isMoveInput)
                {
                    //x軸の動きだけクランプ
                    xv = Vector3.ClampMagnitude(xv, parameter.MaxSpeed);
                }
                else
                {
                    //入力がない時は減衰させる
                    xv *= parameter.SpeedDamping;
                }
            }

            //元の座標系に戻す
            Vector3 originalVelocity = xv + yv;
            rigidbody.velocity = originalVelocity;
        }

        public (Vector3 xv, Vector3 yv) GetSeperatedVelocity()
        {
            Vector3 gravity = worldGravity.Gravity;
            Vector3 velocity = rigidbody.velocity;

            float velocityAlongGravity = Vector3.Dot(velocity, gravity);
            Vector3 xv = velocity - gravity * velocityAlongGravity;
            Vector3 yv = gravity * velocityAlongGravity;

            return (xv, yv);
        }

        public bool CanJumpAgain()
        {
            // 前のジャンプから一定時間が経過していたらチェック開始
            bool canJump = lastJumpTime + parameter.AllowJumpInterval <= Time.time;
            if (!canJump)
            {
                return false;
            }

            Vector3 rayDirection = worldGravity.Direction;
            bool isHit = Physics.Raycast(transform.position, rayDirection, parameter.AllowJumpDistance, GroundLayerMask);

            // 地面に接触していたらジャンプ可能
            return isHit;
        }

        public bool IsJumpable()
        {
            Vector3 rayDirection = worldGravity.Direction;
            bool isHit = Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, parameter.AllowJumpDistance, GroundLayerMask);

            return isHit && !hitInfo.transform.CompareTag(Tag.UnJumpable);
        }

        public bool IsGrounding()
        {
            Vector3 rayDirection = worldGravity.Direction;
            bool isHit = Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, parameter.AllowJumpDistance, GroundLayerMask);

            return isHit;
        }

        public bool CanGroundingAgain(float landingTime)
        {
            // 前のジャンプから一定時間が経過していたらチェック開始
            bool canGrounding = lastJumpTime + parameter.AllowLandingInterval <= Time.time;
            if (!canGrounding)
            {
                return false;
            }

            Vector3 yv = GetSeperatedVelocity().yv;
            bool isDown = Vector3.Dot(yv, worldGravity.Gravity) > 0f;
            float yVelocity = isDown ? yv.magnitude : 0f;
            float g = worldGravity.Gravity.magnitude;

            Vector3 rayDirection = worldGravity.Direction;

            //カプセルコライダーのオフセットを取得
            float offset = capsuleCollider.center.y + capsuleCollider.height * 0.5f;

            // 着地モーションが間に合う距離を算出
            float detectDistance = yVelocity * landingTime + 0.5f * g * landingTime * landingTime;

            // 着地モーションが間に合う距離に入ったら着地確定とする
            bool isHit = Physics.Raycast(transform.position, rayDirection, detectDistance + offset, GroundLayerMask);

            return isHit;
        }

        public void PerformAdditionalJump()
        {
            float time = Time.time - lastJumpTime;
            float jumpPower = parameter.GetAdditionalJumpPower(time);
            Vector3 force = -worldGravity.Direction * jumpPower;

            rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        public void PerformJump()
        {
            Vector3 jumpForce = -worldGravity.Gravity * parameter.JumpPower;
            AddForce(jumpForce, ForceMode.VelocityChange, parameter.JumpingGravity);
        }

        public void AddForce(Vector3 force, ForceMode mode, float forcedGravity)
        {
            temporalGravity = forcedGravity;
            rigidbody.AddForce(force, mode);
            lastJumpTime = Time.time;
        }

        public void AdjustGravity()
        {
            localGravity.SetMultiplierAtFrame(temporalGravity);
        }

        public bool PerformGravityRotate()
        {
            Vector3 gravity = worldGravity.Gravity;
            Quaternion currentRotation = rigidbody.rotation;

            // 自分と重力の角度の差を求める
            float angle = Vector3.Angle(transform.up, -gravity);
            angle = Mathf.Max(angle, Mathf.Epsilon);

            Quaternion targetRotation;

            if (angle >= parameter.CameraAxisRotateAngle)
            {
                targetRotation = Quaternion.AngleAxis(angle, Camera.main.transform.forward) * currentRotation;
            }
            else
            {
                targetRotation = Quaternion.FromToRotation(transform.up, -gravity) * currentRotation;
            }


            if (angle <= Mathf.Epsilon)
            {
                isLastRotating = false;
                return false;
            }

            bool isRotating = angle - parameter.RotateStep >= parameter.RotatingAngle;
            if (!isLastRotating)
            {
                lastRotateTime = Time.time;
            }

            isLastRotating = true;

            // Out-Sineで補間する
            float time = Time.time - lastRotateTime;
            float t = parameter.RotateStep * (Mathf.Sin(time / parameter.RotateTime * Mathf.PI * 0.5f) + 1.0f) + parameter.EasingOffset;

            rigidbody.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, t);

            // 回転がほぼ完了したら回転の目標に合わせる
            if (!isRotating)
            {
                rigidbody.rotation = targetRotation;
            }

            return isRotating;
        }
    }
}