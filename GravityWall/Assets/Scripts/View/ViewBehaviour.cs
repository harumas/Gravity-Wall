using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public abstract class ViewBehaviour : MonoBehaviour
    {
        public abstract BehaviourType BehaviourType { get; }
        
        protected abstract UniTask OnPreActivate();
        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
        protected abstract UniTask OnPostDeactivate();

        public async void Activate()
        {
            gameObject.SetActive(true);
            await OnPreActivate();
            OnActivate();
        }

        public async void Deactivate()
        {
            OnDeactivate();
            await OnPostDeactivate();
            gameObject.SetActive(false);
        }
    }
}