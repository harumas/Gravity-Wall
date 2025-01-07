using System;
using Application.Spawn;
using Cysharp.Threading.Tasks;
using Module.Gimmick.LevelGimmick;
using Module.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Presentation
{
    public class HubEventPresenter : IStartable, IDisposable
    {
        private readonly HubSpawner hubSpawner;
        private readonly PlayerController playerController;

        [Inject]
        public HubEventPresenter(HubSpawner hubSpawner, PlayerController playerController)
        {
            this.hubSpawner = hubSpawner;
            this.playerController = playerController;
        }

        public void Start()
        {
            var deathFloors = Object.FindObjectsByType<DeathFloor>(FindObjectsSortMode.None);

            //死亡床のイベント登録
            foreach (DeathFloor deathFloor in deathFloors)
            {
                deathFloor.OnEnter += async (type, isHubPoint) =>
                {
                    if (!isHubPoint)
                    {
                        return;
                    }
                    
                    playerController.Kill(type);

                    await UniTask.Delay(TimeSpan.FromSeconds(3f));

                    await hubSpawner.Respawn();

                    playerController.Revival();
                };
            }
        }

        public void Dispose() { }
    }
}