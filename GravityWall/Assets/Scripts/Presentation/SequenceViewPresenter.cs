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
                var behaviour = behaviourNavigator.ActivateBehaviour<LoadingBehaviour>(ViewBehaviourState.Loading);
                await behaviour.SequenceLoading();
                behaviourNavigator.DeactivateBehaviour(ViewBehaviourState.Loading);
            };

            InputEvent exitScreenEvent = InputActionProvider.CreateEvent(ActionGuid.UI.ExitScreen);
            exitScreenEvent.Started += _ =>
            {
                if (behaviourNavigator.CurrentBehaviourState == ViewBehaviourState.None)
                {
                    behaviourNavigator.ActivateBehaviour(ViewBehaviourState.Pause);
                }
                else if (behaviourNavigator.CurrentBehaviourState == ViewBehaviourState.Option)
                {
                    behaviourNavigator.DeactivateBehaviour(ViewBehaviourState.Pause);
                }
            };
        }
    }
}