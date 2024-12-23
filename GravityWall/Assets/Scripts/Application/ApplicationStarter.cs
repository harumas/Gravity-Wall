using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Config;
using VContainer;
using VContainer.Unity;

namespace Application
{
    /// <summary>
    /// アプリケーションの開始クラス
    /// </summary>
    public class ApplicationStarter : IAsyncStartable
    {
        private readonly SaveDataLoader saveDataLoader;
        private readonly SceneGroupTable sceneGroupTable;
        private SaveData loadedSaveData;

        [Inject]
        public ApplicationStarter(SaveDataLoader saveDataLoader, SceneGroupTable sceneGroupTable)
        {
            this.saveDataLoader = saveDataLoader;
            this.sceneGroupTable = sceneGroupTable;

            saveDataLoader.OnLoaded += (saveData, configData) => { loadedSaveData = saveData; };
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            bool succeed = await saveDataLoader.Load(cancellation);

            if (!succeed)
            {
                return;
            }

            // 初回プレイによって初期シーンを切り替える
            bool isFirstPlay = loadedSaveData.ClearedStageList.All(clearFlag => !clearFlag);
            int rootSceneIndex = isFirstPlay ? 0 : 1;
            SceneGroup rootSceneGroup = sceneGroupTable.SceneGroups[rootSceneIndex];

            // タイトルシーンのロード
            GameBoot.LoadRootScene(rootSceneGroup);
        }
    }
}