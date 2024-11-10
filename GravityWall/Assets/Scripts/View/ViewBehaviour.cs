using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace View
{
    public abstract class ViewBehaviour : MonoBehaviour
    {
        public abstract ViewBehaviourType ViewBehaviourType { get; }

        [SerializeField] private SerializableReactiveProperty<bool> isActive = new SerializableReactiveProperty<bool>(false);
        public ReadOnlyReactiveProperty<bool> IsActive => isActive;

        protected abstract UniTask OnPreActivate();
        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
        protected abstract UniTask OnPostDeactivate();

        public async void Activate()
        {
            gameObject.SetActive(true);
            await OnPreActivate();
            isActive.Value = true;
            OnActivate();
        }

        public async void Deactivate()
        {
            OnDeactivate();
            await OnPostDeactivate();
            isActive.Value = false;
            gameObject.SetActive(false);
        }
    }
}