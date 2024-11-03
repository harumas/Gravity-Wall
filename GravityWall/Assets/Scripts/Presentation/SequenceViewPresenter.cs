using Application.Sequence;
using Cysharp.Threading.Tasks;
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
        public SequenceViewPresenter(RespawnManager respawnManager, ViewBehaviourNavigator behaviourNavigator)
        {
            this.respawnManager = respawnManager;
            this.behaviourNavigator = behaviourNavigator;
        }

        public void Initialize()
        {
            respawnManager.RespawnViewSequence += async () =>
            {
                var behaviour = behaviourNavigator.ActivateBehaviour<LoadingBehaviour>(BehaviourType.Loading);
                await behaviour.SequenceLoading();
                behaviourNavigator.DeactivateBehaviour(BehaviourType.Loading);
            };
        }
    }
}