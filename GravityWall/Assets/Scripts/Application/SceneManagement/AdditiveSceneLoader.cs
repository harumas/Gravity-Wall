using System.Collections.Generic;
using System.Threading;
using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ThreadPriority = UnityEngine.ThreadPriority;

namespace Application.SceneManagement
{
    /// <summary>
    /// 追加シーン読み込みを行うクラス
    /// </summary>
    public class AdditiveSceneLoader
    {
        private readonly List<string> additiveScenes = new List<string>(16);
        private const int AdditiveLoadIntervalFrames = 1;

        /// <summary>
        /// 追加シーン読み込みを一括で行います
        /// </summary>
        public async UniTask Load((SceneField mainScene, List<SceneField> sceneFields) loadContext, CancellationToken cancellationToken)
        {
            // バックグラウンドの読み込み速度を最低にする
            // できるだけ読み込みの負荷を下げるため
            UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;

            // シーン読み込みの非同期ストリームの作成
            var loadStream = CreateLoadStream(loadContext.sceneFields);

            int loadCount = 0;
            bool onMainSceneLoaded = false;

            // 1フレームおきに追加シーンを読み込む
            await foreach ((SceneField sceneField, AsyncOperation operation) context in loadStream.WithCancellation(cancellationToken))
            {
                loadCount++;

                if (loadCount == loadContext.sceneFields.Count)
                {
                    context.operation.completed += _ =>
                    {
                        SetActiveMainScene(loadContext.mainScene);
                        onMainSceneLoaded = true;
                    };
                }

                // シーンの有効化
                context.operation.allowSceneActivation = true;

                // 次の追加シーンの読み込みまで任意のフレーム待機
                await UniTask.DelayFrame(AdditiveLoadIntervalFrames, cancellationToken: cancellationToken);

                additiveScenes.Add(context.sceneField.SceneName);
            }

            await UniTask.WaitUntil(() => onMainSceneLoaded, cancellationToken: cancellationToken);
        }

        private void SetActiveMainScene(SceneField mainScene)
        {
            // メインシーンの有効化
            Scene lastScene = SceneManager.GetSceneByName(mainScene.SceneName);
            if (lastScene.IsValid())
            {
                SceneManager.SetActiveScene(lastScene);
            }
        }

        /// <summary>
        /// 追加シーンのアンロードを一括で行います
        /// </summary>
        public async UniTask UnloadAdditiveScenes(CancellationToken cancellationToken)
        {
            if (additiveScenes.Count == 0)
            {
                return;
            }

            // 読み込まれた追加シーンをアンロードする
            foreach (string sceneName in additiveScenes)
            {
                await SceneManager.UnloadSceneAsync(sceneName).WithCancellation(cancellationToken);
                await UniTask.Yield();
            }

            // ライトマップの初期化
            LightmapSettings.lightmaps = null;

            // アンロードされたシーンのリソースを解放する
            await Resources.UnloadUnusedAssets();

            additiveScenes.Clear();
        }

        private IUniTaskAsyncEnumerable<(SceneField sceneField, AsyncOperation operation)> CreateLoadStream(List<SceneField> levelReference)
        {
            return UniTaskAsyncEnumerable.Create<(SceneField sceneField, AsyncOperation operation)>(async (writer, token) =>
            {
                foreach (SceneField reference in levelReference)
                {
                    // 追加シーン読み込みを非同期で行う
                    var operation = SceneManager.LoadSceneAsync(reference.SceneName, LoadSceneMode.Additive);
                    operation.allowSceneActivation = false;

                    // 読み込み完了まで待機
                    await UniTask.WaitUntil(() => operation.progress >= 0.9f, cancellationToken: token);

                    await writer.YieldAsync((reference, operation));
                }
            });
        }
    }
}