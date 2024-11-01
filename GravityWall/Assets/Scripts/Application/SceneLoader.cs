using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using R3;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using ThreadPriority = UnityEngine.ThreadPriority;

public class SceneLoader
{
    public SceneLoader()
    {
        UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;
    }

    private async UniTask Load(string sceneName)
    {
#if UNITY_EDITOR
        await LoadSceneWithMetrics(sceneName);
#else
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
#endif
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