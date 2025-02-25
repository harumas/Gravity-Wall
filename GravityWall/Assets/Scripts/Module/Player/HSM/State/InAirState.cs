using Constants;
using Cysharp.Threading.Tasks;
using Module.Gravity;
using R3;
using UnityEngine;

namespace Module.Player.HSM
{
    /// <summary>
    /// 空中状態を表すステート
    /// </summary>
    public class InAirState : StateMachine.State
    {
        private readonly InputEventAdapter inputAdapter;
        private readonly PlayerControlParameter parameter;
        private readonly PlayerControlEvent controlEvent;
        private readonly PlayerControlContext controlContext;
        private readonly PlayerComponent component;
        private Vector2 moveInput;

        public InAirState(InputEventAdapter inputAdapter, PlayerControlParameter parameter, PlayerControlEvent controlEvent,
            PlayerControlContext controlContext, PlayerComponent component)
        {
            this.inputAdapter = inputAdapter;
            this.parameter = parameter;
            this.controlEvent = controlEvent;
            this.controlContext = controlContext;
            this.component = component;
        }

        private const int GroundLayerMask = Layer.Mask.Base | Layer.Mask.Gravity | Layer.Mask.IgnoreGravity | Layer.Mask.IgnoreGimmick;

        internal override void OnEnter()
        {
            controlEvent.IsExternalForce.Value = true;
            
            inputAdapter.Move
                .Subscribe(move => moveInput = move)
                .AddTo(CancellationToken);
        }

        internal override void OnExit()
        {
            moveInput = Vector2.zero;
        }

        internal override void Update()
        {
        }

        internal override void UpdatePhysics()
        {
            // 移動
            component.PlayerMovement.PerformMove(moveInput, parameter.AirControl);

            // 着地判定
            if (CanGroundingAgain(parameter.LandingTime))
            {
                controlEvent.IsGrounding.Value = true;
            }

            // 外部から力を加えられたら重力を調整する
            if (controlEvent.IsExternalForce.Value)
            {
                AdjustGravity();
            }
        }

        private void AdjustGravity()
        {
            component.LocalGravity.SetMultiplierAtFrame(controlContext.TemporalGravity);
        }

        private bool CanGroundingAgain(float landingTime)
        {
            // 前のジャンプから一定時間が経過していたらチェック開始
            bool canGrounding = controlContext.LastJumpTime + parameter.AllowLandingInterval <= Time.time;
            if (!canGrounding)
            {
                return false;
            }

            WorldGravity worldGravity = WorldGravity.Instance;
            CapsuleCollider capsuleCollider = component.CapsuleCollider;
            Transform transform = component.Transform;

            Vector3 yv = controlContext.GetSeperatedVelocity().yv;
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
    }
}