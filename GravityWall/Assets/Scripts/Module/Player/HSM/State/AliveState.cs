using Cysharp.Threading.Tasks;
using Module.Gravity;
using R3;

namespace Module.Player.HSM
{
    /// <summary>
    /// 生存状態を表すステート
    /// </summary>
    public class AliveState : StateMachine.State
    {
        private readonly PlayerControlEvent controlEvent;
        private readonly SimpleInertia simpleInertia;

        public AliveState(
            PlayerControlEvent controlEvent,
            PlayerComponent component
        )
        {
            this.controlEvent = controlEvent;
            simpleInertia = component.ManualInertia;
        }

        internal override void OnEnter()
        {
            controlEvent.IsRotating.Subscribe(isRotating =>
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
            }).AddTo(CancellationToken);
        }

        internal override void OnExit()
        {
        }

        internal override void Update()
        {
        }

        internal override void UpdatePhysics()
        {
            // 慣性の適用
            simpleInertia.PerformInertia();
        }
    }
}