using Module.Gravity;
using R3;
using UnityEngine;

namespace Module.Player
{
    /// <summary>
    /// プレイヤーの移動と回転を制御するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour, IPushable
    {
        [SerializeField] private PlayerControlParameter parameter;
        [SerializeField] private Rigidbody rigBody;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private LocalGravity localGravity;

        public bool IsRotationLocked { get; set; }
        public bool HoldLock { get; set; }
        private SimpleInertia simpleInertia;
        private PlayerFunction playerFunction;

        public PlayerControlParameter Parameter => parameter;

        public ReadOnlyReactiveProperty<bool> IsJumping => isJumping;
        [SerializeField] private SerializableReactiveProperty<bool> isJumping = new SerializableReactiveProperty<bool>();

        public ReadOnlyReactiveProperty<bool> IsGrounding => isGrounding;
        [SerializeField] private SerializableReactiveProperty<bool> isGrounding = new SerializableReactiveProperty<bool>();

        public ReadOnlyReactiveProperty<bool> IsRotating => isRotating;
        [SerializeField] private SerializableReactiveProperty<bool> isRotating = new SerializableReactiveProperty<bool>();

        public ReadOnlyReactiveProperty<float> RotationAngle => rotationAngle;
        [SerializeField] private SerializableReactiveProperty<float> rotationAngle = new SerializableReactiveProperty<float>();

        public ReadOnlyReactiveProperty<DeathType> IsDeath => isDeath;
        [SerializeField] private SerializableReactiveProperty<DeathType> isDeath = new SerializableReactiveProperty<DeathType>();

        public enum DeathType
        {
            None,
            Electric,
            Poison,
        }

        public ReadOnlyReactiveProperty<(Vector3 xv, Vector3 yv)> OnMove => onMove;
        private ReactiveProperty<(Vector3 xv, Vector3 yv)> onMove = new ReactiveProperty<(Vector3 xv, Vector3 yv)>();

        private Vector2 moveInput;
        private float landingTime;
        private bool isJumpingInput;
        

        private void Awake()
        {
            isDeath.Value = DeathType.None;
        }

        private void Start()
        {
            simpleInertia = new SimpleInertia(rigBody);
            playerFunction = new PlayerFunction(transform, cameraPivot, rigBody, localGravity, parameter);

            isRotating.Subscribe(isRotating =>
            {
                if (isRotating)
                {
                    //回転中はオブジェクトが落下しないようにする
                    WorldGravity.Instance.SetDisable(WorldGravity.Type.Object);
                }
                else
                {
                    WorldGravity.Instance.SetEnable(WorldGravity.Type.Object);
                }
            });
        }

        public void OnMoveInput(Vector2 moveInput)
        {
            this.moveInput = moveInput;
        }

        public void OnJumpStart()
        {
            if (!enabled || isJumping.Value || !playerFunction.IsJumpable())
            {
                return;
            }

            playerFunction.PerformJump();
            isJumping.Value = true;
            isGrounding.Value = false;
            isJumpingInput = true;
        }

        public void OnJumpEnd()
        {
            if (!enabled || !isJumping.Value)
            {
                return;
            }

            isJumpingInput = false;
        }

        public void SetLandingTime(float time)
        {
            landingTime = time;
        }

        private void FixedUpdate()
        {
            //接地判定

            if (isJumping.Value)
            {
                // 再びジャンプ可能になったらフラグを解除
                if (playerFunction.CanJumpAgain())
                {
                    isJumping.Value = false;
                }

                if (isJumpingInput)
                {
                    playerFunction.PerformAdditionalJump();
                }
            }

            isGrounding.Value = playerFunction.CanGroundingAgain(landingTime);

            bool isMoveInput = moveInput != Vector2.zero;

            // 移動処理
            if (isMoveInput)
            {
                playerFunction.PerformMove(moveInput, isJumping.Value);
            }

            // 速度調整
            playerFunction.AdjustVelocity(isMoveInput, isDeath.Value != DeathType.None);
            onMove.Value = isMoveInput ? playerFunction.GetSeperatedVelocity() : (Vector3.zero, Vector3.zero);

            // ジャンプ中の重力を調整
            if (isJumping.Value)
            {
                playerFunction.AdjustGravity();
            }

            // 慣性の適用
            simpleInertia.PerformInertia();

            // 重力に応じた回転
            if (!IsRotationLocked)
            {
                (bool rotating, float angle) = playerFunction.PerformGravityRotate();
                rotationAngle.Value = angle;
                isRotating.Value = rotating;
            }
        }

        private void OnCollisionEnter(Collision _)
        {
            simpleInertia?.OnCollisionEnter();
        }

        public void AddExternalPosition(Vector3 delta)
        {
            rigBody.MovePosition(rigBody.position + delta);
        }

        public void AddForce(Vector3 force, ForceMode mode, float forcedGravity, bool allowAdditionalPower)
        {
            playerFunction.AddForce(force, mode, forcedGravity);
            isJumping.Value = true;
            isJumping.ForceNotify();
            isGrounding.Value = false;
            isJumpingInput = allowAdditionalPower;
        }

        public void AddInertia(Vector3 inertia)
        {
            simpleInertia?.AddInertia(inertia);
        }

        public void Kill(DeathType type)
        {
            isDeath.Value = type;
            Refresh();
        }

        public void Refresh()
        {
            isJumping.Value = false;
            isGrounding.Value = true;
            rigBody.velocity = Vector3.zero;
            moveInput = Vector2.zero;
            onMove.Value = (Vector3.zero, Vector3.zero);
        }

        public void Revival()
        {
            isDeath.Value = DeathType.None;
        }

        public void Lock()
        {
            enabled = false;
        }

        public void Unlock()
        {
            if (HoldLock)
            {
                return;
            }

            enabled = true;
        }
    }
}