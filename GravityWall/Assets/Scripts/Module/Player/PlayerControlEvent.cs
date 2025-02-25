using R3;
using UnityEngine;

namespace Module.Player
{
    public class PlayerControlEvent
    {
        public readonly ReactiveProperty<bool> IsExternalForce = new();
        public readonly ReactiveProperty<bool> CanJump = new();
        public readonly ReactiveProperty<DeathType> DeathState = new();
        public readonly ReactiveProperty<bool> IsGrounding = new(true);
        public readonly ReactiveProperty<bool> IsRotating = new();
        public readonly ReactiveProperty<float> RotatingAngle = new();
        public readonly ReactiveProperty<(Vector3 xv, Vector3 yv)> MoveVelocity = new();
        public readonly ReactiveProperty<bool> LockState = new();
    }
}