using System;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Module.Gimmick.LevelGimmick;

namespace Application.Spawn
{
    /// <summary>
    /// ハブ内のリスポーンを行うクラス
    /// </summary>
    public class HubSpawner
    {
        private readonly RespawnManager respawnManager;
        private readonly RespawnContext respawnContext;

        public HubSpawner(RespawnManager respawnManager, HubSpawnPoint spawnPoint)
        {
            this.respawnManager = respawnManager;
            respawnContext = spawnPoint.GetContext();
        }

        /// <summary>
        /// プレイヤーをハブにリスポーンします
        /// </summary>
        public UniTask Respawn(Func<UniTask> respawningTask = null)
        {
            return respawnManager.RespawnPlayer(respawnContext, respawningTask);
        }
    }
}