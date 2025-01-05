using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Gimmick.SystemGimmick;
using Module.InputModule;
using R3;

namespace Application.Spawn
{
    /// <summary>
    /// ハブ内のリスポーンを行うクラス
    /// </summary>
    public class HubSpawner
    {
        private readonly RespawnManager respawnManager;
        private readonly HubSpawnPoint spawnPoint;
        private readonly RespawnContext respawnContext;
        private readonly ReactiveProperty<bool> doInput;
        private float respawnLockDuration = 2f;

        public event Action OnRespawn;

        public HubSpawner(RespawnManager respawnManager, HubSpawnPoint spawnPoint, InputLocker inputLocker)
        {
            this.respawnManager = respawnManager;
            this.spawnPoint = spawnPoint;
            respawnContext = spawnPoint.GetContext();

            doInput = new ReactiveProperty<bool>(true);
            inputLocker.AddCondition(doInput, spawnPoint.destroyCancellationToken);
        }

        /// <summary>
        /// プレイヤーをハブにリスポーンします
        /// </summary>
        public async UniTask Respawn()
        {
            OnRespawn?.Invoke();
            
            doInput.Value = false;

            await respawnManager.RespawnPlayer(respawnContext, null);
            await UniTask.Delay(TimeSpan.FromSeconds(respawnLockDuration), cancellationToken: spawnPoint.destroyCancellationToken);
            
            doInput.Value = true;
        }
    }
}