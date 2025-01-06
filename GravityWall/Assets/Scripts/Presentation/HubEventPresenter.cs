using Application.Spawn;
using Cysharp.Threading.Tasks;
using Module.Gimmick.LevelGimmick;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    public class HubEventPresenter : IStartable
    {
        private readonly HubSpawner hubSpawner;

        [Inject]
        public HubEventPresenter(HubSpawner hubSpawner)
        {
            this.hubSpawner = hubSpawner;
        }

        public void Start()
        {
            var deathFloors = Object.FindObjectsByType<DeathFloor>(FindObjectsSortMode.None);

            //死亡床のイベント登録
            foreach (DeathFloor deathFloor in deathFloors)
            {
                deathFloor.OnEnter += (type) =>
                {
                    hubSpawner.Respawn().Forget();
                };
            }
        }
    }
}