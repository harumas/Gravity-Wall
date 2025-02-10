using Constants;
using Module.Gravity;
using UnityEngine;

namespace Module.Player.HSM
{
    public class PrepareJumpState : StateMachine.State
    {
        private readonly InputEventAdapter inputAdapter;
        private readonly PlayerComponent component;
        private readonly PlayerControlParameter parameter;
        private readonly PlayerControlEvent controlEvent;

        public PrepareJumpState(
            InputEventAdapter inputAdapter,
            PlayerComponent component,
            PlayerControlParameter parameter,
            PlayerControlEvent controlEvent)
        {
            this.inputAdapter = inputAdapter;
            this.component = component;
            this.parameter = parameter;
            this.controlEvent = controlEvent;
        }

        private const int GroundLayerMask = Layer.Mask.Base | Layer.Mask.Gravity | Layer.Mask.IgnoreGravity | Layer.Mask.IgnoreGimmick;
        private float groundTime;

        internal override void OnEnter()
        {
            groundTime = Time.time;
        }

        internal override void OnExit()
        {
        }

        internal override void Update()
        {
        }

        internal override void UpdatePhysics()
        {
            if (CanJumpAgain())
            {
                inputAdapter.Jump.Value = false;
                controlEvent.IsExternalForce.Value = false;
            }
        }

        private bool CanJumpAgain()
        {
            Transform transform = component.Transform;
            WorldGravity worldGravity = WorldGravity.Instance;

            // 前のジャンプから一定時間が経過していたらチェック開始
            bool canJump = groundTime + parameter.AllowJumpInterval <= Time.time;
            if (!canJump)
            {
                return false;
            }

            Vector3 rayDirection = worldGravity.Direction;
            bool isHit = Physics.Raycast(transform.position, rayDirection, parameter.AllowJumpDistance, GroundLayerMask);

            // 地面に接触していたらジャンプ可能
            return isHit;
        }
    }
}