using R3;
using UnityEngine;

namespace Module.Gimmick
{
    public abstract class GimmickObject : MonoBehaviour
    {
        [SerializeField] protected SerializableReactiveProperty<bool> isEnabled;
        public ReadOnlyReactiveProperty<bool> IsEnabled => isEnabled;

        public abstract void Enable(bool doEffect = true);
        public abstract void Disable(bool doEffect = true);
        public abstract void Reset();
    }
}