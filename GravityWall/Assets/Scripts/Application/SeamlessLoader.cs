using System.Collections;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using R3;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SeamlessLoader : MonoBehaviour
{
    [SerializeField] private GimmickObject target;

    private void Start()
    {
        target.IsEnabled.Skip(1).Subscribe(isEnabled => StartCoroutine(OnEnabled(isEnabled)));
        UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;
    }

    private IEnumerator OnEnabled(bool isEnabled)
    {
        AsyncReadManagerMetrics.StartCollectingMetrics();

        var loader = SceneManager.LoadSceneAsync("AdditiveTest", LoadSceneMode.Additive);
        while (!loader.isDone)
        {
            Debug.Log($"LoadSceneAsync Progress:{loader.progress}%");
            AsyncReadManagerRequestMetric[] metrics
                = AsyncReadManagerMetrics.GetMetrics(AsyncReadManagerMetrics.Flags.ClearOnRead);
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

            yield return null;
        }

        AsyncReadManagerMetrics.StopCollectingMetrics();
    }
}