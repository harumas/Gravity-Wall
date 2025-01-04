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
            var creditView = creditBehaviour.CreditView;

            creditView.OnBackButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Credit)).AddTo(creditView);
        }
    }
}