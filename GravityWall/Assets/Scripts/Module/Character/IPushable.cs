using UnityEngine;

namespace Domain
{
    public interface IPushable
    {
        void AddExternalPosition(Vector3 delta);
        void AddForce(Vector3 force, ForceMode mode, float forcedGravity, bool allowAdditionalPower);
        void AddInertia(Vector3 inertia);
    }
}