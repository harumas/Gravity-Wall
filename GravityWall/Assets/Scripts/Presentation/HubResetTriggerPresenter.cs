using Module.Gimmick.SystemGimmick;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class HubResetTriggerPresenter : IInitializable
    {
        private readonly PauseView pauseView;
        private readonly StartRoomShowTrigger[] startRoomShowTriggers;

        [Inject]
        public HubResetTriggerPresenter(PauseBehaviour pauseBehaviour)
        {
            pauseView = pauseBehaviour.PauseView;
            startRoomShowTriggers = Object.FindObjectsByType<StartRoomShowTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        public void Initialize()
        {
            pauseView.OnReturnToHubButtonPressed.Subscribe(_ =>
            {
                foreach (var trigger in startRoomShowTriggers)
                {
                    trigger.Reset();
                }
            });
        }
    }
}