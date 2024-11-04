using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.Gimmick;
using R3;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using ThreadPriority = UnityEngine.ThreadPriority;

public class AdditiveSceneLoader
{
    private List<SceneInstance> additiveScenes = new List<SceneInstance>(16);

    public async UniTask Load(List<AssetReference> levelReference, CancellationToken cancellationToken)
    {
        UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;

        var loadStream = CreateLoadStream(levelReference);

        Scene lastScene = default;

        await foreach (SceneInstance instance in loadStream.WithCancellation(cancellationToken))
        {
            await instance.ActivateAsync();
            await UniTask.Yield();

            additiveScenes.Add(instance);

            lastScene = instance.Scene;
        }

        if (lastScene.IsValid())
        {
            SceneManager.SetActiveScene(lastScene);
        }
    }

    private IUniTaskAsyncEnumerable<SceneInstance> CreateLoadStream(List<AssetReference> levelReference)
    {
        return UniTaskAsyncEnumerable.Create<SceneInstance>(async (writer, token) =>
        {
            foreach (AssetReference reference in levelReference)
            {
                var handle = await Addressables.LoadSceneAsync(reference, LoadSceneMode.Additive, false).WithCancellation(token);
                await writer.YieldAsync(handle);
            }
        });
    }

    public async UniTask UnloadAdditiveScenes(CancellationToken cancellationToken)
    {
        foreach (SceneInstance instance in additiveScenes)
        {
            await Addressables.UnloadSceneAsync(instance).WithCancellation(cancellationToken);
            await UniTask.Yield();
        }

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