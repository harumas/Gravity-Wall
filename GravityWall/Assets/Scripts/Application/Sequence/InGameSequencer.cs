using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Gimmick.LevelGimmick;
using VContainer;
using VContainer.Unity;

namespace Application.Sequence
{
    public class InGameSequencer : IStartable, IDisposable
    {
        private readonly GameState gameState;
        private readonly HubSpawner hubSpawner;
        private readonly RespawnManager respawnManager;
        private readonly AdditiveSceneLoadExecutor loadExecutor;
        private CancellationTokenSource cTokenSource;

        [Inject]
        public InGameSequencer(GameState gameState, HubSpawner hubSpawner, RespawnManager respawnManager)
        {
            this.gameState = gameState;
            this.hubSpawner = hubSpawner;
            this.respawnManager = respawnManager;
            loadExecutor = new AdditiveSceneLoadExecutor();
        }

        public void Start()
        {
            cTokenSource = new CancellationTokenSource();
            loadExecutor.SetCancellationToken(cTokenSource.Token);

            Sequence().Forget();
        }

        private async UniTaskVoid Sequence()
        {
            // プレイ開始待機
            await gameState.WaitUntilState(GameState.State.Playing, cTokenSource.Token);

            // クリア待機
            await gameState.WaitUntilState(GameState.State.StageSelect, cTokenSource.Token);
            
            respawnManager.LockPlayer();

            // シーンアンロード
            await loadExecutor.UnloadAdditiveScenes();

            await hubSpawner.Respawn();
        }

        public void Dispose()
        {
            cTokenSource?.Cancel();
            cTokenSource?.Dispose();
        }
    }
}