namespace Module.Player.HSM
{
    /// <summary>
    /// 死亡状態を表すステート
    /// </summary>
    public class DeathState : StateMachine.State
    {
        private readonly PlayerControlContext controlContext;

        public DeathState(PlayerControlContext controlContext)
        {
            this.controlContext = controlContext;
        }

        internal override void OnEnter()
        {
            // 死亡時に物理演算をリセット
            controlContext.ResetPhysics();
        }

        internal override void OnExit()
        {
        }

        internal override void Update()
        {
        }

        internal override void UpdatePhysics()
        {
        }
    }
}