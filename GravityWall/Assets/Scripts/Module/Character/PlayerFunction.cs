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

        private float lastJumpTime;
        private float lastRotateTime;
        private bool isLastRotating;
        private float temporalGravity;

        public PlayerFunction(
            Transform transform,
            Transform cameraPivot,
            Rigidbody rigidbody,
            LocalGravity localGravity,
            PlayerControlParameter parameter)
        {
            this.transform = transform;
            this.cameraPivot = cameraPivot;
            this.rigidbody = rigidbody;
            this.localGravity = localGravity;
            this.parameter = parameter;

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
            Vector3 gravity = worldGravity.Gravity;
            Vector3 velocity = rigidbody.velocity;

            //重力のベクトルに対してのローカル速度を取得
            float velocityAlongGravity = Vector3.Dot(velocity, gravity);
            Vector3 yVelocity = gravity * velocityAlongGravity;
            Vector3 xVelocity = velocity - gravity * velocityAlongGravity;

            if (freezeX)
            {
                xVelocity = Vector3.zero;
            }
            else
            {
                if (isMoveInput)
                {
                    //x軸の動きだけクランプ
                    xVelocity = Vector3.ClampMagnitude(xVelocity, parameter.MaxSpeed);
                }
                else
                {
                    //入力がない時は減衰させる
                    xVelocity *= parameter.SpeedDamping;
                }
            }

            //元の座標系に戻す
            Vector3 originalVelocity = xVelocity + yVelocity;
            rigidbody.velocity = originalVelocity;
        }

        public bool CanJumpAgain()
        {
            // 前のジャンプから一定時間が経過していたらチェック開始
            bool canJump = lastJumpTime + parameter.AllowJumpInterval <= Time.time;
            if (!canJump)
            {
                return false;
            }

            int layerMask = Layer.Mask.Base | Layer.Mask.Gravity | Layer.Mask.IgnoreGravity;
            Vector3 rayDirection = worldGravity.Direction;
            bool isHit = Physics.Raycast(transform.position, rayDirection, parameter.AllowJumpDistance, layerMask);

            // 地面に接触していたらジャンプ可能
            return isHit;
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
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravity) * currentRotation;

            // 自分と重力の角度の差を求める
            float angle = Vector3.Angle(transform.up, -gravity);
            angle = Mathf.Max(angle, Mathf.Epsilon);

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