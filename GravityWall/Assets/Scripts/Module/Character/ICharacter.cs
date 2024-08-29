using UnityEngine;

namespace Domain
{
    public interface ICharacter
    {
        void DoJump(Vector3 jumpForce, float jumpingGravity);
        void AddExternalPosition(Vector3 delta);
        void AddInertia(Vector3 inertia);
    }
}