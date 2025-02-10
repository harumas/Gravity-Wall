using Constants;
using Cysharp.Threading.Tasks;
using Module.Gravity;
using R3;
using UnityEngine;

namespace Module.Player.HSM
{
    public class StandbyJumpState : StateMachine.State
    {
        private readonly InputEventAdapter inputAdapter;
        private readonly PlayerControlParameter parameter;
        private readonly PlayerControlContext controlContext;
        private readonly PlayerControlEvent controlEvent;
        private readonly PlayerComponent component;

        private const int GroundLayerMask = Layer.Mask.Base | Layer.Mask.Gravity | Layer.Mask.IgnoreGravity | Layer.Mask.IgnoreGimmick;

        public StandbyJumpState(
            InputEventAdapter inputAdapter,
            PlayerControlParameter parameter,
            PlayerControlContext controlContext,
            PlayerComponent component,
            PlayerControlEvent controlEvent
        )
        {
            this.inputAdapter = inputAdapter;
            this.parameter = parameter;
            this.controlContext = controlContext;
            this.component = component;
            this.controlEvent = controlEvent;
        }

        internal override void OnEnter()
        {
            inputAdapter.Jump
                .Subscribe(doJump =>
                {
                    if (doJump)
                    {
                        OnJumpInput();
                    }
                })
                .AddTo(CancellationToken);
        }

        internal override void OnExit()
        {
            controlEvent.CanJump.Value = false;
        }

        internal override void Update()
        {
        }

        internal override void UpdatePhysics()
        {
        }

        private void OnJumpInput()
        {
            if (CanJump())
            {
                PerformJump();
                controlEvent.IsExternalForce.Value = true;
                controlEvent.IsGrounding.Value = false;
            }
        }

        private void PerformJump()
        {
            WorldGravity worldGravity = WorldGravity.Instance;
            Vector3 jumpForce = -worldGravity.Gravity * parameter.JumpPower;
            controlContext.AddForce(jumpForce, ForceMode.VelocityChange, parameter.JumpingGravity);
        }

        private bool CanJump()
        {
            Transform transform = component.Transform;
            WorldGravity worldGravity = WorldGravity.Instance;

            Vector3 rayDirection = worldGravity.Direction;
            bool isHit = Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, parameter.AllowJumpDistance, GroundLayerMask);

            return isHit && !hitInfo.transform.CompareTag(Tag.UnJumpable);
        }
    }
}