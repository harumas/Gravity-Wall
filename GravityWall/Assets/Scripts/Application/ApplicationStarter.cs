﻿using System.Linq;
using System.Threading;
using CoreModule.Helper.Attribute;
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
        private readonly SplashScreen splashScreen;
        private SaveData loadedSaveData;

        [Inject]
        public ApplicationStarter(SaveDataLoader saveDataLoader, SceneGroupTable sceneGroupTable, SplashScreen splashScreen)
        {
            this.saveDataLoader = saveDataLoader;
            this.sceneGroupTable = sceneGroupTable;
            this.splashScreen = splashScreen;

            saveDataLoader.OnLoaded += (saveData, configData) => { loadedSaveData = saveData; };
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            // スプラッシュ画面を表示
            var splashAwaiter = splashScreen.Show();

            bool succeed = await saveDataLoader.Load(cancellation);

            if (!succeed)
            {
                return;
            }

            // 初回プレイによって初期シーンを切り替える
            bool isFirstPlay = loadedSaveData.ClearedStageList.All(clearFlag => !clearFlag);
            int rootSceneIndex = isFirstPlay ? 0 : 1;
            SceneField rootScene = sceneGroupTable.SceneGroups[rootSceneIndex].GetScenes().First();
            
            // スプラッシュ画面を表示し切るまで待つ
            await splashAwaiter;
            
            // タイトルシーンのロード
            await GameBoot.LoadRootScene(rootScene);
            
            // スプラッシュ画面を削除
            splashScreen.Destroy();
        }
    }
}