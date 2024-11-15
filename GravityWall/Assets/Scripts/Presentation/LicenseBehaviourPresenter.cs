using CoreModule.Input;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class LicenseBehaviourPresenter : IStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly LicenseBehaviour licenseBehaviour;
        private InputEvent backEvent;

        [Inject]
        public LicenseBehaviourPresenter(ViewBehaviourNavigator navigator, LicenseBehaviour licenseBehaviour)
        {
            this.navigator = navigator;
            this.licenseBehaviour = licenseBehaviour;
        }

        public void Start()
        {
            backEvent = InputActionProvider.CreateEvent(ActionGuid.UI.Cancel);

            licenseBehaviour.OnActiveStateChanged.Subscribe(context =>
                {
                    if (context.isActive)
                    {
                        backEvent.Started += OnBackButtonPressed;
                    }
                    else
                    {
                        backEvent.Started -= OnBackButtonPressed;
                    }
                })
                .AddTo(licenseBehaviour);
        }

        private void OnBackButtonPressed(InputAction.CallbackContext _)
        {
            navigator.DeactivateBehaviour(ViewBehaviourState.License);
        }
    }
}