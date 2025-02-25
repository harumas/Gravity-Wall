using System;
using Module.Gravity;
using Module.Player.HSM;
using UnityEngine;

namespace Module.Player
{
    [Serializable]
    public class PlayerComponent
    {
        [SerializeField] private Transform transform;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private LocalGravity localGravity;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private SimpleInertia simpleInertia;
            
        public Transform Transform => transform;
        public Rigidbody RigidBody => rigidBody;
        public Transform CameraPivot => cameraPivot;
        public LocalGravity LocalGravity => localGravity;
        public CapsuleCollider CapsuleCollider => capsuleCollider;
        public SimpleInertia ManualInertia => simpleInertia;
        public PlayerRotator PlayerRotator { get; set; }
        public PlayerMovement PlayerMovement { get; set; }
    }
}