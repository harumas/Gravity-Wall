using System.Collections.Generic;
using System.Threading;
using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using ThreadPriority = UnityEngine.ThreadPriority;

namespace Application
{
    /// <summary>
    /// 追加シーン読み込みを行うクラス
    /// </summary>
    public class AdditiveSceneLoader
    {
        private readonly List<string> additiveScenes = new List<string>(16);
        private const int AdditiveLoadIntervalFrames = 5;

        /// <summary>
        /// 追加シーン読み込みを一括で行います
        /// </summary>
        /// <param name="loadContext"></param>
        /// <param name="cancellationToken"></param>
        public async UniTask Load((SceneField mainScene, List<SceneField> sceneFields) loadContext, CancellationToken cancellationToken)
        {
            UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;

            var loadStream = CreateLoadStream(loadContext.sceneFields);

            await foreach ((SceneField sceneField, AsyncOperation operation) context in loadStream.WithCancellation(cancellationToken))
            {
                context.operation.allowSceneActivation = true;
                await UniTask.DelayFrame(AdditiveLoadIntervalFrames, cancellationToken: cancellationToken);

                additiveScenes.Add(context.sceneField.SceneName);
            }

            Scene lastScene = SceneManager.GetSceneByName(loadContext.mainScene.SceneName);
            if (lastScene.IsValid())
            {
                SceneManager.SetActiveScene(lastScene);
            }
        }

        public async UniTask UnloadAdditiveScenes(CancellationToken cancellationToken)
        {
            if (additiveScenes.Count == 0)
            {
                return;
            }

            foreach (string sceneName in additiveScenes)
            {
                await SceneManager.UnloadSceneAsync(sceneName).WithCancellation(cancellationToken);
                await UniTask.Yield();
            }

            await Resources.UnloadUnusedAssets();
            additiveScenes.Clear();
        }

        private IUniTaskAsyncEnumerable<(SceneField sceneField, AsyncOperation operation)> CreateLoadStream(List<SceneField> levelReference)
        {
            return UniTaskAsyncEnumerable.Create<(SceneField sceneField, AsyncOperation operation)>(async (writer, token) =>
            {
                foreach (SceneField reference in levelReference)
                {
                    var operation = SceneManager.LoadSceneAsync(reference.SceneName, LoadSceneMode.Additive);
                    operation.allowSceneActivation = false;

                    // 読み込みまで待機
                    await UniTask.WaitUntil(() => operation.progress >= 0.9f, cancellationToken: token);

                    await writer.YieldAsync((reference, operation));
                }
            });
        }
    }
}