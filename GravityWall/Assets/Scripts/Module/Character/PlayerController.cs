using Domain;
using Module.Gravity;
using R3;
using UnityEngine;

namespace Module.Character
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
        private SimpleInertia simpleInertia;
        private PlayerFunction playerFunction;

        public ReadOnlyReactiveProperty<bool> IsJumping => isJumping;
        [SerializeField] private SerializableReactiveProperty<bool> isJumping = new SerializableReactiveProperty<bool>();

        public ReadOnlyReactiveProperty<bool> IsRotating => isRotating;
        [SerializeField] private SerializableReactiveProperty<bool> isRotating = new SerializableReactiveProperty<bool>();

        public ReadOnlyReactiveProperty<Vector3> OnMove => onMove;
        [SerializeField] private SerializableReactiveProperty<Vector3> onMove = new SerializableReactiveProperty<Vector3>();

        public ReadOnlyReactiveProperty<bool> IsDeath => isDeath;
        [SerializeField] private SerializableReactiveProperty<bool> isDeath = new SerializableReactiveProperty<bool>();
        private Vector2 moveInput;

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

        public void OnJumpInput()
        {
            if (isJumping.Value)
            {
                return;
            }

            playerFunction.PerformJump();
            isJumping.Value = true;
        }

        private void FixedUpdate()
        {
            // 再びジャンプ可能になったらフラグを解除
            if (isJumping.Value && playerFunction.CanJumpAgain())
            {
                isJumping.Value = false;
            }

            bool isMoveInput = moveInput != Vector2.zero;

            // 移動処理
            if (isMoveInput)
            {
                playerFunction.PerformMove(moveInput, isJumping.Value);
            }

            // 速度調整
            playerFunction.AdjustVelocity(isMoveInput, isDeath.Value);
            onMove.Value = rigBody.velocity;

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
                isRotating.Value = playerFunction.PerformGravityRotate();
            }
        }

        private void OnCollisionEnter(Collision _)
        {
            simpleInertia.OnCollisionEnter();
        }

        public void AddExternalPosition(Vector3 delta)
        {
            rigBody.MovePosition(rigBody.position + delta);
        }

        public void AddForce(Vector3 force, ForceMode mode, float forcedGravity)
        {
            playerFunction.AddForce(force, mode, forcedGravity);
            isJumping.Value = true;
        }

        public void AddInertia(Vector3 inertia)
        {
            simpleInertia.AddInertia(inertia);
        }

        public void Kill()
        {
            isDeath.Value = true;
            isJumping.Value = false;
            rigBody.velocity = Vector3.zero;
            moveInput = Vector2.zero;
            onMove.Value = Vector3.zero;
            enabled = false;
        }

        public void Respawn()
        {
            isDeath.Value = false;
            enabled = true;
        }
    }
}