using System;
using System.Threading;
using Application.SceneManagement;
using Application.Spawn;
using Cysharp.Threading.Tasks;
using Module.Gravity;
using VContainer;
using VContainer.Unity;

namespace Application.Sequence
{
    /// <summary>
    /// ゲーム中の進行を行うクラス
    /// </summary>
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
            // 重力クラスの作成
            WorldGravity.Create();
            
            // プレイ開始待機
            await gameState.WaitUntilState(GameState.State.Playing, cTokenSource.Token);

            // クリア待機
            await gameState.WaitUntilState(GameState.State.StageSelect, cTokenSource.Token);
            
            respawnManager.LockPlayer();

            await UniTask.WhenAll(loadExecutor.UnloadAdditiveScenes(), hubSpawner.Respawn());
        }

        public void Dispose()
        {
            cTokenSource?.Cancel();
            cTokenSource?.Dispose();
        }
    }
}