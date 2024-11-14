using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public class LicenseBehaviour : ViewBehaviour
    {
        public override ViewBehaviourState ViewBehaviourState => ViewBehaviourState.License;
        
        [SerializeField] private LicenseView licenseView;
        public LicenseView LicenseView => licenseView;
        
        protected override async UniTask OnPreActivate(ViewBehaviourState beforeState)
        {
            licenseView.SelectFirst();
            await UniTask.CompletedTask;
        }

        protected override void OnActivate()
        {
            
        }

        protected override void OnDeactivate()
        {
        }

        protected override async UniTask OnPostDeactivate(ViewBehaviourState nextState)
        {
            await UniTask.CompletedTask;
        }
    }
}