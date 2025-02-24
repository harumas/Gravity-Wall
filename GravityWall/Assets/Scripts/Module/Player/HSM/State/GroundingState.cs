﻿using Constants;
using Cysharp.Threading.Tasks;
using Module.Gravity;
using R3;
using UnityEngine;

namespace Module.Player.HSM
{
    /// <summary>
    /// 着地状態を表すステート
    /// </summary>
    public class GroundingState : StateMachine.State
    {
        private readonly InputEventAdapter inputAdapter;
        private readonly PlayerControlEvent controlEvent;
        private readonly PlayerComponent component;
        private readonly PlayerControlParameter parameter;
        private Vector2 moveInput;

        private const int GroundLayerMask = Layer.Mask.Base | Layer.Mask.Gravity | Layer.Mask.IgnoreGravity | Layer.Mask.IgnoreGimmick;

        public GroundingState(
            InputEventAdapter inputAdapter,
            PlayerControlEvent controlEvent,
            PlayerComponent component,
            PlayerControlParameter parameter)
        {
            this.inputAdapter = inputAdapter;
            this.controlEvent = controlEvent;
            this.component = component;
            this.parameter = parameter;
        }

        internal override void OnEnter()
        {
            inputAdapter.Move
                .Subscribe(move =>
                {
                    moveInput = move;
                })
                .AddTo(CancellationToken);
            
            controlEvent.CanJump.Value = IsGround();
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
            if (controlEvent.CanJump.Value)
            {
                // 着地状態を更新する
                if (!IsGround())
                {
                    controlEvent.IsGrounding.Value = false;
                }
            }
            else
            {
                // ジャンプ可能状態を更新する
                if (IsGround())
                {
                    controlEvent.CanJump.Value = true;
                }
            }

            component.PlayerMovement.PerformMove(moveInput, 1f);
            component.PlayerRotator.Update();
        }

        private bool IsGround()
        {
            Transform transform = component.Transform;
            WorldGravity worldGravity = WorldGravity.Instance;

            Vector3 rayDirection = worldGravity.Direction;
            bool isHit = Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, parameter.AllowJumpDistance, GroundLayerMask);

            return isHit;
        }
    }
}