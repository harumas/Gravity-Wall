using System;
using System.Threading;
using Application.SceneManagement;
using Application.Spawn;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.Gravity;
using UnityEngine.SceneManagement;
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
        private readonly SceneGroupTable sceneGroupTable;
        private readonly AdditiveSceneLoadExecutor loadExecutor;
        private CancellationTokenSource cTokenSource;

        [Inject]
        public InGameSequencer(GameState gameState, HubSpawner hubSpawner, RespawnManager respawnManager, SceneGroupTable sceneGroupTable)
        {
            this.gameState = gameState;
            this.hubSpawner = hubSpawner;
            this.respawnManager = respawnManager;
            this.sceneGroupTable = sceneGroupTable;
            loadExecutor = new AdditiveSceneLoadExecutor();
        }

        public void Start()
        {
            cTokenSource = new CancellationTokenSource();
            loadExecutor.SetCancellationToken(cTokenSource.Token);

            // 重力クラスの作成
            WorldGravity.Create();

            if (!IsNewGame())
            {
                gameState.SetState(GameState.State.StageSelect);
            }

            Sequence().Forget();

            loadExecutor.OnUnloadRequested += () => Sequence().Forget();
        }

        private async UniTaskVoid Sequence()
        {
            // プレイ開始待機
            await gameState.WaitUntilState(GameState.State.Playing, cTokenSource.Token);

            // クリア待機
            await gameState.WaitUntilState(GameState.State.StageSelect, cTokenSource.Token);

            respawnManager.LockPlayer();

            await UniTask.WhenAll(loadExecutor.UnloadAdditiveScenes(), hubSpawner.Respawn());
        }

        private bool IsNewGame()
        {
            SceneGroup sceneGroup = sceneGroupTable.SceneGroups[0];
            string mainSceneName = sceneGroup.GetMainScene();

            return mainSceneName == SceneManager.GetActiveScene().name;
        }

        public void Dispose()
        {
            cTokenSource?.Cancel();
            cTokenSource?.Dispose();
        }
    }
}