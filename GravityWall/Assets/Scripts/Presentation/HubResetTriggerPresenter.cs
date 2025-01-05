using Application.Spawn;
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
        private readonly HubSpawner hubSpawner;
        private readonly StartRoomShowTrigger[] startRoomShowTriggers;

        [Inject]
        public HubResetTriggerPresenter(HubSpawner hubSpawner)
        {
            this.hubSpawner = hubSpawner;
            startRoomShowTriggers = Object.FindObjectsByType<StartRoomShowTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        public void Initialize()
        {
            hubSpawner.OnRespawn += () =>
            {
                foreach (var trigger in startRoomShowTriggers)
                {
                    trigger.Reset();
                }
            };
        }
    }
}