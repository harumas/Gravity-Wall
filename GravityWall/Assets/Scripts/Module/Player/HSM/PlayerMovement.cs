using Module.Gravity;
using UnityEngine;

namespace Module.Player.HSM
{
    public class PlayerMovement
    {
        private readonly PlayerControlParameter parameter;
        private readonly PlayerControlEvent controlEvent;
        private readonly PlayerControlContext controlContext;
        private readonly Transform cameraPivot;
        private readonly Rigidbody rigidbody;
        private readonly WorldGravity worldGravity;

        public PlayerMovement(
            PlayerControlParameter parameter,
            PlayerControlEvent controlEvent,
            PlayerControlContext controlContext,
            PlayerComponent component)
        {
            this.parameter = parameter;
            this.controlEvent = controlEvent;
            this.controlContext = controlContext;
            this.cameraPivot = component.CameraPivot;
            this.rigidbody = component.RigidBody;
            this.worldGravity = WorldGravity.Instance;
        }

        public void PerformMove(Vector2 moveInput, float multiplier)
        {
            bool isMoveInput = moveInput.sqrMagnitude > 0f;

            if (isMoveInput)
            {
                UpdateMove(moveInput, multiplier);
            }

            // 速度調整
            AdjustVelocity(isMoveInput, controlEvent.DeathState.Value != DeathType.None);

            (Vector3 xv, Vector3 yv) zeroVelocity = (Vector3.zero, Vector3.zero);
            controlEvent.MoveVelocity.Value = isMoveInput ? controlContext.GetSeperatedVelocity() : zeroVelocity;
        }

        private void UpdateMove(Vector2 moveInput, float multiplier)
        {
            // 移動方向の算出
            Vector3 forward = cameraPivot.forward * moveInput.y;
            Vector3 right = cameraPivot.right * moveInput.x;
            Vector3 moveDirection = Vector3.ClampMagnitude(forward + right, 1f);

            // 重力と垂直な速度ベクトルに変換
            moveDirection = Quaternion.FromToRotation(cameraPivot.up, -worldGravity.Gravity) * moveDirection;

            // 力を算出して加速
            Vector3 moveForce = moveDirection * (parameter.Acceleration * multiplier);
            rigidbody.AddForce(moveForce, ForceMode.Acceleration);
        }

        private void AdjustVelocity(bool isMoveInput, bool freezeX)
        {
            //重力のベクトルに対してのローカル速度を取得
            (Vector3 xv, Vector3 yv) = controlContext.GetSeperatedVelocity();

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
    }
}