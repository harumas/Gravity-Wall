using System;
using Cysharp.Threading.Tasks;
using Module.Gravity;
using Module.Player.HSM;
using R3;
using UnityEngine;

namespace Module.Player
{
    /// <summary>
    /// プレイヤーの状態を制御するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour, IPushable
    {
        [SerializeField] private PlayerControlParameter parameter;
        [SerializeField] private PlayerComponent playerComponent;

        public bool HoldLock { get; set; }
        public PlayerControlParameter Parameter => parameter;
        public PlayerControlEvent ControlEvent => controlEvent;
        public PlayerComponent Component => playerComponent;

        private PlayerControlContext controlContext;
        private PlayerControlEvent controlEvent = new PlayerControlEvent();
        private InputEventAdapter inputEventAdapter;
        private StateMachine stateMachine;

        public void SetInputAdapter(InputEventAdapter adapter)
        {
            inputEventAdapter = adapter;
        }

        private void Start()
        {
            controlContext = new PlayerControlContext(playerComponent, controlEvent);
            playerComponent.PlayerRotator = new PlayerRotator(parameter, controlContext, controlEvent, playerComponent);
            playerComponent.PlayerMovement = new PlayerMovement(parameter, controlEvent, controlContext, playerComponent);

            stateMachine = new StateMachine();

            AliveState aliveState = new AliveState(controlEvent, Component);
            DeathState deathState = new DeathState(controlContext);
            GroundingState groundingState = new GroundingState(inputEventAdapter, controlEvent, playerComponent, parameter);
            InAirState inAirState = new InAirState(inputEventAdapter, parameter, controlEvent, controlContext, playerComponent);
            StandbyJumpState standbyJumpState = new StandbyJumpState(inputEventAdapter, parameter, controlContext, playerComponent, controlEvent);
            JumpingState jumpingState = new JumpingState(inputEventAdapter, parameter, controlContext, playerComponent);
            PrepareJumpState prepareJumpState = new PrepareJumpState(inputEventAdapter, playerComponent, parameter, controlEvent);
            FallingState fallingState = new FallingState();

            // ステートの登録
            stateMachine.AddState(aliveState);
            stateMachine.AddState(deathState);
            stateMachine.AddState<GroundingState, AliveState>(groundingState);
            stateMachine.AddState<InAirState, AliveState>(inAirState);
            stateMachine.AddState<PrepareJumpState, GroundingState>(prepareJumpState);
            stateMachine.AddState<StandbyJumpState, GroundingState>(standbyJumpState);
            stateMachine.AddState<JumpingState, InAirState>(jumpingState);
            stateMachine.AddState<FallingState, InAirState>(fallingState);

            // 遷移条件の登録
            stateMachine.AddTransition<GroundingState, InAirState>(() => !controlEvent.IsGrounding.CurrentValue);
            stateMachine.AddTransition<PrepareJumpState, StandbyJumpState>(() =>
                {
                    return !controlEvent.IsExternalForce.CurrentValue && controlEvent.CanJump.CurrentValue;
                }
            );
            stateMachine.AddTransition<InAirState, GroundingState>(() => controlEvent.IsGrounding.CurrentValue);
            stateMachine.AddTransition<JumpingState, FallingState>(() =>
            {
                bool isFalling = Vector3.Dot(WorldGravity.Instance.Gravity, playerComponent.RigidBody.velocity) > 0f;
                return isFalling;
            });
            stateMachine.AddTransition<AliveState, DeathState>(() => controlEvent.DeathState.CurrentValue != DeathType.None);
            stateMachine.AddTransition<DeathState, AliveState>(() => controlEvent.DeathState.CurrentValue == DeathType.None);

            playerComponent.PlayerRotator.Enabled = true;
            stateMachine.Start<AliveState>();
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.UpdatePhysics();
        }

        /// <summary>
        /// Rigidbodyの座標を更新します
        /// </summary>
        public void AddExternalPosition(Vector3 delta)
        {
            Rigidbody rigBody = playerComponent.RigidBody;
            rigBody.MovePosition(rigBody.position + delta);
        }

        /// <summary>
        /// 強制的にジャンプを行います
        /// </summary>
        public void DoJump(Vector3 force, float forcedGravity)
        {
            // 着地フラグを更新する
            controlEvent.IsGrounding.Value = false;

            // ジャンプ力を加える
            controlContext.AddForce(force, ForceMode.VelocityChange, forcedGravity);
        }

        public void Kill(DeathType type)
        {
            // 死亡状態を設定
            ControlEvent.DeathState.Value = type;
        }

        /// <summary>
        /// 死亡状態を解除します
        /// </summary>
        public void Revival()
        {
            ControlEvent.DeathState.Value = DeathType.None;
        }

        /// <summary>
        /// プレイヤーの動きをロックします
        /// </summary>
        public void Lock(RigidbodyConstraints freezeOption = RigidbodyConstraints.FreezeRotation, bool resetPhysics = false)
        {
            if (resetPhysics)
            {
                controlContext.ResetPhysics();
            }

            ControlEvent.LockState.Value = true;
            playerComponent.RigidBody.constraints = freezeOption;
        }


        /// <summary>
        /// プレイヤーのロックを解除します
        /// </summary>
        public async void Unlock()
        {
            // 外部からロックされている場合は解除しない
            if (HoldLock)
            {
                return;
            }

            playerComponent.RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            playerComponent.ManualInertia.enabled = false;

            // 有効化直後のゲームループが終わるまで1フレーム待機
            await UniTask.Yield();

            playerComponent.ManualInertia.enabled = true;
            ControlEvent.LockState.Value = false;
        }
    }

    public enum DeathType
    {
        None,
        Electric,
        Poison,
        Fall,
    }
}