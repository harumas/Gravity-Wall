using Module.Gravity;
using UnityEngine;

namespace Module.Player.HSM
{
    public class PlayerControlContext
    {
        private readonly PlayerComponent component;
        private readonly PlayerControlEvent controlEvent;

        public float LastJumpTime;
        public float LastRotateTime;
        public bool IsLastRotating;
        public float TemporalGravity;

        public PlayerControlContext(PlayerComponent component, PlayerControlEvent controlEvent)
        {
            this.component = component;
            this.controlEvent = controlEvent;
        }

        public void AddForce(Vector3 force, ForceMode mode, float forcedGravity)
        {
            TemporalGravity = forcedGravity;
            component.RigidBody.AddForce(force, mode);
            LastJumpTime = Time.time;
        }

        public (Vector3 xv, Vector3 yv) GetSeperatedVelocity()
        {
            Vector3 gravity = WorldGravity.Instance.Gravity;
            Vector3 velocity = component.RigidBody.velocity;

            float velocityAlongGravity = Vector3.Dot(velocity, gravity);
            Vector3 xv = velocity - gravity * velocityAlongGravity;
            Vector3 yv = gravity * velocityAlongGravity;

            return (xv, yv);
        }

        /// <summary>
        /// プレイヤーの物理状態をリセットします
        /// </summary>
        public void ResetPhysics()
        {
            controlEvent.IsExternalForce.Value = false;
            controlEvent.CanJump.Value = false;
            component.RigidBody.velocity = Vector3.zero;
            controlEvent.MoveVelocity.Value = (Vector3.zero, Vector3.zero);
            component.ManualInertia.SetInertia(Vector3.zero);
        }
    }
}