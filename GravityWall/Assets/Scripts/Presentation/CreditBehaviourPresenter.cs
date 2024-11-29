using CoreModule.Input;
using R3;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    /// <summary>
    /// クレジット画面のイベントを接続するクラス
    /// </summary>
    public class CreditBehaviourPresenter : IStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly CreditBehaviour creditBehaviour;
        private InputEvent backEvent;

        [Inject]
        public CreditBehaviourPresenter(ViewBehaviourNavigator navigator, CreditBehaviour creditBehaviour)
        {
            this.navigator = navigator;
            this.creditBehaviour = creditBehaviour;
        }

        public void Start()
        {
            backEvent = InputActionProvider.CreateEvent(ActionGuid.UI.Cancel);

            creditBehaviour.OnActiveStateChanged.Subscribe(context =>
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
                .AddTo(creditBehaviour);
        }

        private void OnBackButtonPressed(InputAction.CallbackContext _)
        {
            navigator.DeactivateBehaviour(ViewBehaviourState.Credit);
        }
    }
}