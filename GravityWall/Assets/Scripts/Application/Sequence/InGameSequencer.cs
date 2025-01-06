using System;
using System.Linq;
using System.Threading;
using Application.SceneManagement;
using Application.Spawn;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.Gimmick.LevelGimmick;
using Module.Gravity;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Application.Sequence
{
    /// <summary>
    /// ゲーム中の進行を行うクラス
    /// </summary>
    public class InGameSequencer : IAsyncStartable, IDisposable
    {
        private readonly GameState gameState;
        private readonly HubSpawner hubSpawner;
        private readonly RespawnManager respawnManager;
        private readonly SceneGroupTable sceneGroupTable;
        private readonly AdditiveSceneLoadExecutor loadExecutor;
        private readonly SaveManager<SaveData> saveManager;
        private readonly MainGateOpenSequencer sequencer;
        private CancellationTokenSource cTokenSource;

        [Inject]
        public InGameSequencer(
            GameState gameState,
            HubSpawner hubSpawner,
            RespawnManager respawnManager,
            SceneGroupTable sceneGroupTable,
            SaveManager<SaveData> saveManager,
            MainGateOpenSequencer mainGateOpenSequencer
        )
        {
            this.gameState = gameState;
            this.hubSpawner = hubSpawner;
            this.respawnManager = respawnManager;
            this.sceneGroupTable = sceneGroupTable;
            this.saveManager = saveManager;
            this.sequencer = mainGateOpenSequencer;

            loadExecutor = new AdditiveSceneLoadExecutor();
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            // 重力クラスの作成
            WorldGravity.Create();


            cTokenSource = new CancellationTokenSource();
            loadExecutor.SetCancellationToken(cTokenSource.Token);

            await UniTask.Yield(cancellation);

            InitMainGate();

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
            
            RefreshClearState();
        }

        private void InitMainGate()
        {
            // チュートリアル以外のクリアデータをメインゲートに反映する
            bool[] clearedStageList = saveManager.Data.ClearedStageList.Skip(1).ToArray();

            sequencer.Initialize(clearedStageList);
        }

        private void RefreshClearState()
        {
            bool[] clearedStageList = saveManager.Data.ClearedStageList.Skip(1).ToArray();
            sequencer.SetHologram(clearedStageList);
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