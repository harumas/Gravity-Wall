using R3;
using UnityEngine;

namespace Module.Character
{
    public class PlayerState
    {
        public readonly ReadOnlyReactiveProperty<bool> IsJumping;
        public readonly ReadOnlyReactiveProperty<bool> IsRotating;
        public readonly ReadOnlyReactiveProperty<Vector3> OnMove;
        public readonly ReadOnlyReactiveProperty<bool> IsDeath;

        public PlayerState(
            ReadOnlyReactiveProperty<bool> isJumping,
            ReadOnlyReactiveProperty<bool> isRotating,
            ReadOnlyReactiveProperty<Vector3> onMove,
            ReadOnlyReactiveProperty<bool> isDeath)
        {
            IsJumping = isJumping;
            IsRotating = isRotating;
            OnMove = onMove;
            IsDeath = isDeath;
        }
    }
}