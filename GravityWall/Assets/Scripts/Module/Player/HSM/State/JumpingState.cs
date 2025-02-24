using Constants;
using Module.Gravity;
using UnityEngine;

namespace Module.Player.HSM
{
    /// <summary>
    /// ジャンプ中を表すステート
    /// </summary>
    public class JumpingState : StateMachine.State
    {
        private readonly InputEventAdapter inputAdapter;
        private readonly PlayerControlParameter parameter;
        private readonly PlayerControlContext controlContext;
        private readonly PlayerComponent component;

        public JumpingState(
            InputEventAdapter inputAdapter,
            PlayerControlParameter parameter,
            PlayerControlContext controlContext,
            PlayerComponent component) 
        {
            this.inputAdapter = inputAdapter;
            this.parameter = parameter;
            this.controlContext = controlContext;
            this.component = component;
        }

        internal override void OnEnter()
        {
        }

        internal override void OnExit()
        {
        }

        internal override void Update()
        {
        }

        internal override void UpdatePhysics()
        {
            // ジャンプボタン長押しでジャンプ力を加算
            if (inputAdapter.Jump.CurrentValue)
            {
                PerformAdditionalJump();
            }
        }

        private void PerformAdditionalJump()
        {
            WorldGravity worldGravity = WorldGravity.Instance;

            // 時間経過に応じてジャンプ力を変化させる
            float time = Time.time - controlContext.LastJumpTime;
            float jumpPower = parameter.GetAdditionalJumpPower(time);
            Vector3 force = -worldGravity.Direction * jumpPower;

            component.RigidBody.AddForce(force, ForceMode.Acceleration);
        }
    }
}