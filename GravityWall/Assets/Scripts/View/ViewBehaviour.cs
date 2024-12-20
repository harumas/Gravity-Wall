using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace View
{
    public abstract class ViewBehaviour : MonoBehaviour
    {
        public abstract ViewBehaviourState ViewBehaviourState { get; }

        [SerializeField] private SerializableReactiveProperty<bool> isActive = new();
        private readonly Subject<(bool isActive, ViewBehaviourState behaviourType)> onActiveStateChanged = new();

        public ReadOnlyReactiveProperty<bool> IsActive => isActive;
        public Observable<(bool isActive, ViewBehaviourState behaviourType)> OnActiveStateChanged => onActiveStateChanged;

        protected abstract UniTask OnPreActivate(ViewBehaviourState beforeState, CancellationToken cancellation);
        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
        protected abstract UniTask OnPostDeactivate(ViewBehaviourState nextState, CancellationToken cancellation);

        public async void Activate(ViewBehaviourState beforeState)
        {
            gameObject.SetActive(true);
            await OnPreActivate(beforeState, destroyCancellationToken);
            onActiveStateChanged.OnNext((true, beforeState));
            isActive.Value = true;
            OnActivate();
        }

        public async void Deactivate(ViewBehaviourState nextState)
        {
            OnDeactivate();
            await OnPostDeactivate(nextState, destroyCancellationToken);
            onActiveStateChanged.OnNext((false, nextState));
            isActive.Value = false;
            gameObject.SetActive(false);
        }
    }
}