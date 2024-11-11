using Application.Sequence;
using CoreModule.Input;
using Cysharp.Threading.Tasks;
using Module.InputModule;
using R3;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class SequenceViewPresenter : IInitializable
    {
        private readonly RespawnManager respawnManager;
        private readonly ViewBehaviourNavigator behaviourNavigator;

        [Inject]
        public SequenceViewPresenter(RespawnManager respawnManager,  ViewBehaviourNavigator behaviourNavigator)
        {
            this.respawnManager = respawnManager;
            this.behaviourNavigator = behaviourNavigator;
        }

        public void Initialize()
        {
            respawnManager.RespawnViewSequence += async () =>
            {
                var behaviour = behaviourNavigator.ActivateBehaviour<LoadingBehaviour>(ViewBehaviourType.Loading);
                await behaviour.SequenceLoading();
                behaviourNavigator.DeactivateBehaviour(ViewBehaviourType.Loading);
            };

            InputEvent exitScreenEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ExitScreen);
            exitScreenEvent.Started += _ =>
            {
                if (behaviourNavigator.CurrentBehaviourType == ViewBehaviourType.None)
                {
                    behaviourNavigator.ActivateBehaviour(ViewBehaviourType.Pause);
                }
                else if (behaviourNavigator.CurrentBehaviourType == ViewBehaviourType.Option)
                {
                    behaviourNavigator.DeactivateBehaviour(ViewBehaviourType.Pause);
                }
            };

            var optionView = behaviourNavigator.GetBehaviour<OptionBehaviour>(ViewBehaviourType.Option).OptionView;
            optionView.OnBackButtonPressed.Subscribe(_ => behaviourNavigator.DeactivateBehaviour(ViewBehaviourType.Option)).AddTo(optionView);
        }
    }
}