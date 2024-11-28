using System;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Module.Gimmick.LevelGimmick;

namespace Application.Sequence
{
    public class HubSpawner
    {
        private readonly RespawnManager respawnManager;
        private readonly RespawnContext respawnContext;

        public HubSpawner(RespawnManager respawnManager, HubSpawnPoint spawnPoint)
        {
            this.respawnManager = respawnManager;
            respawnContext = spawnPoint.GetContext();
        }

        public UniTask Respawn(Func<UniTask> respawningTask = null)
        {
            return respawnManager.RespawnPlayer(respawnContext, respawningTask);
        }
    }
}