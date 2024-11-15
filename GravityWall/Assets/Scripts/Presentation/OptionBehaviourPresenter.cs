using R3;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class OptionBehaviourPresenter : IStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly OptionBehaviour optionBehaviour;

        public OptionBehaviourPresenter(ViewBehaviourNavigator navigator, OptionBehaviour optionBehaviour)
        {
            this.navigator = navigator;
            this.optionBehaviour = optionBehaviour;
        }

        public void Start()
        {
            var optionView = optionBehaviour.OptionView;
            optionView.OnBackButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Option)).AddTo(optionView);
            optionView.OnLicenseButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourState.License)).AddTo(optionView);
        }
    }
}