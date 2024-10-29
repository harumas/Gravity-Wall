using System;
using Application.Respawn;
using Cysharp.Threading.Tasks;
using Module.Character;
using Module.Gravity;
using R3;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Application.Sequence
{
    public class RespawnManager
    {
        private readonly PlayerController playerController;
        private readonly CameraController cameraController;
        private readonly PlayerTargetSyncer playerTargetSyncer;
        private RespawnContext respawnData;
        private bool isRespawning;

        public event Func<UniTask> RespawnViewSequence;

        [Inject]
        public RespawnManager(PlayerController playerController,CameraController cameraController, PlayerTargetSyncer playerTargetSyncer)
        {
            this.playerController = playerController;
            this.cameraController = cameraController;
            this.playerTargetSyncer = playerTargetSyncer;
            Initialize();
        }

        private void Initialize()
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
            playerTargetSyncer.Reset();

            if (RespawnViewSequence != null)
            {
                await RespawnViewSequence();
            }

            WorldGravity.Instance.SetValue(respawnData.Gravity);

            respawnData.LevelResetter.ResetLevel();

            playerController.Respawn();
            playerController.transform.SetPositionAndRotation(respawnData.Position, respawnData.Rotation);
            cameraController.SetCameraRotation(respawnData.Rotation);

            isRespawning = false;
        }
    }
}