using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.Gimmick;
using R3;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using ThreadPriority = UnityEngine.ThreadPriority;

public class AdditiveSceneLoader
{
    private readonly List<string> additiveScenes = new List<string>(16);
    private const int DelayFrameCount = 10;

    public async UniTask Load((SceneField mainScene, List<SceneField> sceneFields) loadContext, CancellationToken cancellationToken)
    {
        UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;

        var loadStream = CreateLoadStream(loadContext.sceneFields);

        await foreach ((SceneField sceneField, AsyncOperation operation) context in loadStream.WithCancellation(cancellationToken))
        {
            context.operation.allowSceneActivation = true;
            await UniTask.DelayFrame(DelayFrameCount, cancellationToken: cancellationToken);

            additiveScenes.Add(context.sceneField.SceneName);
        }

        Scene lastScene = SceneManager.GetSceneByName(loadContext.mainScene.SceneName);
        if (lastScene.IsValid())
        {
            SceneManager.SetActiveScene(lastScene);
        }
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

    public async UniTask UnloadAdditiveScenes(CancellationToken cancellationToken)
    {
        foreach (string sceneName in additiveScenes)
        {
            await SceneManager.UnloadSceneAsync(sceneName).WithCancellation(cancellationToken);
            await UniTask.Yield();
        }

        await Resources.UnloadUnusedAssets();
        additiveScenes.Clear();
    }

    private async UniTask LoadSceneWithMetrics(string sceneName)
    {
        AsyncReadManagerMetrics.StartCollectingMetrics();

        var loader = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        CancellationTokenSource cTokenSource = new CancellationTokenSource();

        Observable.EveryUpdate(cTokenSource.Token)
            .Subscribe(_ =>
            {
                if (loader.isDone)
                {
                    cTokenSource.Cancel();
                    return;
                }

                Debug.Log($"LoadSceneAsync Progress:{loader.progress}%");
                AsyncReadManagerRequestMetric[] metrics = AsyncReadManagerMetrics.GetMetrics(AsyncReadManagerMetrics.Flags.ClearOnRead);
                foreach (AsyncReadManagerRequestMetric metric in metrics)
                {
                    Debug.Log($"metric: {metric.AssetName}, FileName: {metric.FileName}\n" +
                              $"SizeBytes: {metric.SizeBytes} bytes, CurrentBytes: {metric.CurrentBytesRead}\n" +
                              $"BatchReadCount: {metric.BatchReadCount}, State : {metric.State.ToString()}, PriorityLevel:{metric.PriorityLevel}, Subsystem: {metric.Subsystem.ToString()}\n" +
                              $"RequestTime: {metric.RequestTimeMicroseconds}, TimeInQueue(us): {metric.TimeInQueueMicroseconds}, TotalTime: {metric.TotalTimeMicroseconds}"
                    );
                }

                AsyncReadManagerSummaryMetrics summaryOfMetrics
                    = AsyncReadManagerMetrics.GetSummaryOfMetrics(metrics);
                Debug.Log(
                    $"Metric Summary: TotalBytesRead: {summaryOfMetrics.TotalBytesRead}, AverageBandwidthMBPerSecond: {summaryOfMetrics.AverageBandwidthMBPerSecond}\n" +
                    $"AverageReadSizeInBytes: {summaryOfMetrics.AverageReadSizeInBytes}, AverageWaitTimeMicroseconds: {summaryOfMetrics.AverageWaitTimeMicroseconds}\n" +
                    $"AverageReadTimeMicroseconds: {summaryOfMetrics.AverageReadTimeMicroseconds}, AverageTotalRequestTimeMicroseconds: {summaryOfMetrics.AverageTotalRequestTimeMicroseconds}\n" +
                    $"AverageThroughputMBPerSecond: {summaryOfMetrics.AverageThroughputMBPerSecond}, LongestWaitTimeMicroseconds: {summaryOfMetrics.LongestWaitTimeMicroseconds}");
            });

        AsyncReadManagerMetrics.StopCollectingMetrics();

        await loader;
    }
}