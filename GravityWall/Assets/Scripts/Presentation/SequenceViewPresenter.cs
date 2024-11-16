using Application.Sequence;
using CoreModule.Input;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class SequenceViewPresenter : IInitializable
    {
        private readonly ViewBehaviourNavigator behaviourNavigator;

        [Inject]
        public SequenceViewPresenter(RespawnManager respawnManager,  ViewBehaviourNavigator behaviourNavigator)
        {
            this.behaviourNavigator = behaviourNavigator;
        }

        public void Initialize()
        {
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