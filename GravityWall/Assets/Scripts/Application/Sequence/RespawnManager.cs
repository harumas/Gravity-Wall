using Application.Respawn;
using Cysharp.Threading.Tasks;
using Module.Character;
using Module.Gravity;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Application.Sequence
{
    public class RespawnManager : IInitializable
    {
        private readonly PlayerController playerController;
        private readonly UISequencer uiSequencer;
        private RespawnContext respawnData;
        private bool isRespawning;

        [Inject]
        public RespawnManager(PlayerController playerController, UISequencer uiSequencer)
        {
            this.playerController = playerController;
            this.uiSequencer = uiSequencer;
        }

        public void Initialize()
        {
            var savePoints = Object.FindObjectsByType<SavePoint>(FindObjectsSortMode.None);
            var deathFloors = Object.FindObjectsByType<DeathFloor>(FindObjectsSortMode.None);

            foreach (SavePoint savePoint in savePoints)
            {
                savePoint.OnEnterPoint.Subscribe(OnSave);
            }

            foreach (DeathFloor deathFloor in deathFloors)
            {
                deathFloor.OnEnter += () =>
                {
                    if (isRespawning)
                    {
                        return;
                    }

                    OnEnterDeathFloor().Forget();
                };
            }
        }

        private void OnSave(RespawnContext respawnContext)
        {
            respawnData = respawnContext;
        }

        private async UniTaskVoid OnEnterDeathFloor()
        {
            isRespawning = true;

            playerController.Kill();

            await uiSequencer.SequenceRespawn();
            
            WorldGravity.Instance.SetValue(respawnData.Gravity);
            
            respawnData.LevelResetter.ResetLevel();

            playerController.Respawn();
            playerController.transform.SetPositionAndRotation(respawnData.Position, respawnData.Rotation);

            isRespawning = false;
        }
    }
}