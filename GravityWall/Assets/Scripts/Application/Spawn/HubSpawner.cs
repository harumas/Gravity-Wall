using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Module.Gimmick.SystemGimmick;
using Module.InputModule;
using R3;
using Object = UnityEngine.Object;

namespace Application.Spawn
{
    /// <summary>
    /// ハブ内のリスポーンを行うクラス
    /// </summary>
    public class HubSpawner
    {
        private readonly RespawnManager respawnManager;
        private readonly ReactiveProperty<bool> doInput;
        private float respawnLockDuration = 2f;

        public event Action OnRespawn;
        private HubSpawnPoint currentSpawnPoint;
        private RespawnContext currentContext;

        public HubSpawner(RespawnManager respawnManager, InputLocker inputLocker)
        {
            this.respawnManager = respawnManager;

            var hubSpawnPoints = Object.FindObjectsByType<HubSpawnPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (HubSpawnPoint spawnPoint in hubSpawnPoints)
            {
                spawnPoint.OnEnabled += context =>
                {
                    currentSpawnPoint = spawnPoint;
                    currentContext = context;
                };
            }
            
            doInput = new ReactiveProperty<bool>(true);
            inputLocker.AddCondition(doInput, hubSpawnPoints[0].destroyCancellationToken);
        }

        /// <summary>
        /// プレイヤーをハブにリスポーンします
        /// </summary>
        public async UniTask Respawn()
        {
            OnRespawn?.Invoke();

            doInput.Value = false;

            await respawnManager.RespawnPlayer(currentContext, null);
            await UniTask.Delay(TimeSpan.FromSeconds(respawnLockDuration), cancellationToken: currentSpawnPoint.destroyCancellationToken);

            doInput.Value = true;
        }
    }
}