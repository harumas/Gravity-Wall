using UnityEngine;

namespace Module.Player
{
    public interface IPushable
    {
        void AddExternalPosition(Vector3 delta);
        void DoJump(Vector3 force, float forcedGravity);
    }
}