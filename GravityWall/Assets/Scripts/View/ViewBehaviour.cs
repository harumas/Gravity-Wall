using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace View
{
    public abstract class ViewBehaviour : MonoBehaviour
    {
        public abstract ViewBehaviourType ViewBehaviourType { get; }

        [SerializeField] private SerializableReactiveProperty<bool> isActive = new();
        private readonly Subject<(bool isActive, ViewBehaviourType behaviourType)> onActiveStateChanged = new();

        public ReadOnlyReactiveProperty<bool> IsActive => isActive;
        public Observable<(bool isActive, ViewBehaviourType behaviourType)> OnActiveStateChanged => onActiveStateChanged;

        protected abstract UniTask OnPreActivate(ViewBehaviourType beforeType);
        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
        protected abstract UniTask OnPostDeactivate(ViewBehaviourType nextType);

        public async void Activate(ViewBehaviourType beforeType)
        {
            gameObject.SetActive(true);
            await OnPreActivate(beforeType);
            onActiveStateChanged.OnNext((true, beforeType));
            isActive.Value = true;
            OnActivate();
        }

        public async void Deactivate(ViewBehaviourType nextType)
        {
            OnDeactivate();
            await OnPostDeactivate(nextType);
            onActiveStateChanged.OnNext((false, nextType));
            isActive.Value = false;
            gameObject.SetActive(false);
        }
    }
}